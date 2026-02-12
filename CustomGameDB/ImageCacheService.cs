using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using System.IO;
using System.Security.Cryptography;


namespace CustomGameDB
{
    

        public class ImageCacheService
        {
            private readonly string cacheFolder;
            private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(4);
            private static readonly HttpClient _httpClient = new HttpClient();

            private const int DefaultMaxWidth = 800;
            private const int DefaultMaxHeight = 600;
            private const int WebpQuality = 80;

            public ImageCacheService()
            {
                cacheFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "customGameDB", "ImageCache");

                Directory.CreateDirectory(cacheFolder);
            }

            private static string HashString(string s)
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }

            // Nuevo: obtener ruta del fichero de caché para una URL y dimensiones
            public string GetCacheFilePath(string url, int maxWidth = DefaultMaxWidth, int maxHeight = DefaultMaxHeight)
            {
                var key = HashString(url) + $"_{maxWidth}x{maxHeight}.webp";
                return Path.Combine(cacheFolder, key);
            }

            // Nuevo: comprobación rápida si existe en disco (sin abrirlo)
            public bool IsCached(string url, int maxWidth = DefaultMaxWidth, int maxHeight = DefaultMaxHeight)
            {
                var file = GetCacheFilePath(url, maxWidth, maxHeight);
                return File.Exists(file);
            }

            // Nuevo: leer desde caché (si existe) y decodificar a SKBitmap
            public async Task<SKBitmap?> GetCachedImageAsync(string url, int maxWidth = DefaultMaxWidth, int maxHeight = DefaultMaxHeight)
            {
                var file = GetCacheFilePath(url, maxWidth, maxHeight);
                if (!File.Exists(file)) return null;

                var data = await File.ReadAllBytesAsync(file).ConfigureAwait(false);
                return SKBitmap.Decode(data);
            }

            // Método existente: descarga/decodifica/genera caché; ya comprueba File.Exists internamente
            public async Task<SKBitmap?> GetImageAsync(string url, int maxWidth = DefaultMaxWidth, int maxHeight = DefaultMaxHeight)
            {
                if (string.IsNullOrWhiteSpace(url)) return null;

                var fileName = GetCacheFilePath(url, maxWidth, maxHeight);

                await _semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (File.Exists(fileName))
                    {
                        var data = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);
                        return SKBitmap.Decode(data);
                    }

                    var bytes = await _httpClient.GetByteArrayAsync(url).ConfigureAwait(false);

                    using var original = SKBitmap.Decode(bytes);
                    if (original == null) return null;

                    int targetW = original.Width;
                    int targetH = original.Height;

                    float scale = Math.Min((float)maxWidth / original.Width, (float)maxHeight / original.Height);
                    if (scale < 1f)
                    {
                        targetW = Math.Max(1, (int)(original.Width * scale));
                        targetH = Math.Max(1, (int)(original.Height * scale));
                    }

                    SKBitmap result;
                    if (targetW != original.Width || targetH != original.Height)
                    {
                        result = new SKBitmap(targetW, targetH);
                        original.ScalePixels(result, SKFilterQuality.Medium);
                    }
                    else
                    {
                        result = new SKBitmap(original.Info.Width, original.Info.Height);
                        original.CopyTo(result);
                    }

                    using var image = SKImage.FromBitmap(result);
                    using var encoded = image.Encode(SKEncodedImageFormat.Webp, WebpQuality);
                    if (encoded != null)
                    {
                        var encodedBytes = encoded.ToArray();
                        await File.WriteAllBytesAsync(fileName, encodedBytes).ConfigureAwait(false);
                    }

                    return result;
                }
                finally
                {
                    _semaphore.Release();
                }
            }


        }

    }


