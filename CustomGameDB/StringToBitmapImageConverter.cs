using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CustomGameDB
{
    public class StringToBitmapImageConverter : IValueConverter
    {
        // Evitar cargas síncronas desde HTTP: si es URL remota, devolver null (placeholder).
        // El control debe usar Game.ImagenPrincipal (SkiaImage) para mostrar imágenes descargadas en background.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return null;

            try
            {
                var uri = new Uri(s, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    // No cargar imagen remota de forma síncrona; devolver null para placeholder.
                    return null;
                }

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = uri;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
