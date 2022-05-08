using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Surfaseprite.Input;
using Surfaseprite.SceneViewer;

namespace Surfaseprite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PaintInputProcessor _paintInput;

        public MainWindow()
        {
            InitializeComponent();

            _paintInput = new PaintInputProcessor(SceneViewportPanel);
            _paintInput.DotPlaced += args =>
            {
                var hit = SceneViewport.ScreenPick(args.Location);
                if (hit != null)
                    PaintPixel(hit.SceneItem.Bitmap, hit.HitTexelCoords);
            };
            _paintInput.StrokePointAdded += args =>
            {
                var hit = SceneViewport.ScreenPick(args.Stroke.Points.Last());
                if (hit != null)
                    PaintPixel(hit.SceneItem.Bitmap, hit.HitTexelCoords);
            };
        }

        private void PaintPixel(WriteableBitmap bitmap, Vector2 location)
        {
            var offset = ((int)location.Y * bitmap.PixelWidth + (int)location.X) * 4;
            bitmap.UpdatePixels(pixels =>
            {
                pixels[offset] = 255;
                pixels[offset + 1] = 255;
            });
            SceneViewport.InvalidateVisual();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var bitmap = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Pbgra32, null);
            var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
            bitmap.CopyPixels(pixels, bitmap.BackBufferStride, 0);
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = 122;
            }
            bitmap.WritePixels(new Int32Rect(0, 0, 100, 100), pixels, bitmap.BackBufferStride, 0);
            
            SceneViewport.AddSceneItem(1u, bitmap);
            SceneViewport.SetTransform(1u, Matrix3x2.CreateScale(4));
            SceneViewport.InvalidateVisual();
        }
    }
}
