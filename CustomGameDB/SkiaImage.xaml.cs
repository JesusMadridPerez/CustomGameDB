using SkiaSharp;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomGameDB
{
    /// <summary>
    /// Lógica de interacción para SkiaImage.xaml
    /// </summary>
    public partial class SkiaImage : UserControl
    {
        public static readonly DependencyProperty BitmapProperty =
        DependencyProperty.Register(nameof(Bitmap), typeof(SKBitmap),
            typeof(SkiaImage), new PropertyMetadata(null, OnBitmapChanged));

        public SKBitmap Bitmap
        {
            get => (SKBitmap)GetValue(BitmapProperty);
            set => SetValue(BitmapProperty, value);
        }
        private SKElement _skElement;
        public SkiaImage()
        {
            InitializeComponent();
            _skElement = new SKElement();
            _skElement.PaintSurface += OnPaintSurface;
            Content = _skElement;
        }

        private static void OnBitmapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SkiaImage)d;
            control._skElement.InvalidateVisual();
        }

        private void OnPaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            if (Bitmap == null) return;

            var info = e.Info;

            
            float scale = Math.Min(
                info.Width / (float)Bitmap.Width,
                info.Height / (float)Bitmap.Height);

            float width = Bitmap.Width * scale;
            float height = Bitmap.Height * scale;

            float x = (info.Width - width) / 2;
            float y = (info.Height - height) / 2;

            
            var dest = new SKRect(x, y, x + width, y + height);

            using (var image = SKImage.FromBitmap(Bitmap))
            {
                var fastSampling = new SKSamplingOptions(SKFilterMode.Linear);
                canvas.DrawImage(image, dest, fastSampling, null);
            }

        }



    }
}
