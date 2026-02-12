using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;               
using System.Windows.Threading;

namespace CustomGameDB
{
    public class Game
    {
        /*
        public int id { get; set; }
        public String? slug { get; set; }
        public String? name { get; set; }
        public String? date { get; set; }
        public Boolean tba { get; set; }
        public String? background_image { get; set; }
        public double rating { get; set; }
        public int rating_top { get; set; }
        List<ratings>? ratings { get; set; }
        public int ratings_count { get; set; }
        public String? reviews_text_count { get; set; }
        public int added { get; set; }
        List<added_by_status>? added_By_Status{ get; set; }
        public int metacritic { get; set; }
        public int playtime { get; set; }
        public int suggestions_count { get; set; }
        public String? updated { get; set; }
        public int reviews_count   { get; set; }
        List<platforms>? platforms { get; set; }
        List<platform>? parent_platforms { get; set; }
        List<genres> genres { get; set; }*/

        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string released { get; set; }
        public bool tba { get; set; }
        public string background_image { get; set; }
        public double rating { get; set; }
        public int rating_top { get; set; }
        public ObservableCollection<ratings> ratings { get; set; }
        public int ratings_count { get; set; }
        public int reviews_text_count { get; set; }
        public int added { get; set; }
        public added_by_status added_by_status { get; set; }
        public int? metacritic { get; set; }
        public int playtime { get; set; }
        public int suggestions_count { get; set; }
        public DateTime updated { get; set; }
        public ObservableCollection<object> user_game { get; set; }
        public int reviews_count { get; set; }
        public string saturated_color { get; set; }
        public string dominant_color { get; set; }
        public ObservableCollection<platforms>? platforms { get; set; }
        public ObservableCollection<Parent_platforms> parent_platforms { get; set; }
        public ObservableCollection<genres> genres { get; set; }
        public ObservableCollection<stores> stores { get; set; }
        public ObservableCollection<object> clip { get; set; }
        public ObservableCollection<tags> tags { get; set; }
        public Esrb_rating esrb_rating { get; set; }
        public ObservableCollection<Short_screenshots> short_screenshots { get; set; }

        //private readonly ImageCacheService _cache = new();

        /*
        public async Task LoadImageAsync()
        {
            ImagenPrincipal = await _cache.GetImageAsync(background_image);
        }*/

        private static readonly ImageCacheService _cache = new ImageCacheService();
        public SKBitmap ImagenPrincipal { get; set; }

        public async Task LoadImageAsync(int preferredMaxWidth = 400, int preferredMaxHeight = 300)
        {
            if (string.IsNullOrWhiteSpace(background_image)) return;

            SKBitmap? bmp = null;

            // Comprobar caché primero (rápido)
            if (_cache.IsCached(background_image, preferredMaxWidth, preferredMaxHeight))
            {
                bmp = await _cache.GetCachedImageAsync(background_image, preferredMaxWidth, preferredMaxHeight).ConfigureAwait(false);
            }
            else
            {
                // No está en caché: descargar y generar caché
                bmp = await _cache.GetImageAsync(background_image, preferredMaxWidth, preferredMaxHeight).ConfigureAwait(false);
            }

            if (bmp == null) return;

            // Asignar en hilo UI (evitar dependencias en modelos: ideal usar ViewModel/INotifyPropertyChanged)
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null)
            {
                await dispatcher.InvokeAsync(() => ImagenPrincipal = bmp).Task.ConfigureAwait(false);
            }
            else
            {
                ImagenPrincipal = bmp;
            }
        }
    }
}
