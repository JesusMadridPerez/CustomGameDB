using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomGameDB
{
    public class Short_screenshots
    {
        public int id { get; set; }
        public string image { get; set; }

        public SKBitmap captura { get; set; }
        private static readonly ImageCacheService _cache = new ImageCacheService();
        public async Task LoadImageAsync(int preferredMaxWidth = 400, int preferredMaxHeight = 300)
        {
            if (string.IsNullOrWhiteSpace(image)) return;

            SKBitmap? bmp = null;

            // Comprobar caché primero (rápido)
            if (_cache.IsCached(image, preferredMaxWidth, preferredMaxHeight))
            {
                bmp = await _cache.GetCachedImageAsync(image, preferredMaxWidth, preferredMaxHeight).ConfigureAwait(false);
            }
            else
            {
                // No está en caché: descargar y generar caché
                bmp = await _cache.GetImageAsync(image, preferredMaxWidth, preferredMaxHeight).ConfigureAwait(false);
            }

            if (bmp == null) return;

            // Asignar en hilo UI (evitar dependencias en modelos: ideal usar ViewModel/INotifyPropertyChanged)
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null)
            {
                await dispatcher.InvokeAsync(() => captura = bmp).Task.ConfigureAwait(false);
            }
            else
            {
                captura = bmp;
            }
        }


    }
}
