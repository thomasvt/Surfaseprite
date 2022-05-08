using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Surfaseprite.SceneViewer
{
    public static class Model3DBuilder
    {
        public static Model3D MakeSpriteQuad(WriteableBitmap bitmap)
        {
            var mesh = new MeshGeometry3D();

            var halfW = bitmap.PixelWidth * 0.5;
            var halfH = bitmap.PixelHeight * 0.5;

            // 0 --- 1
            // |   / |
            // | /   |
            // 3 --- 2

            mesh.Positions.Add(new Point3D(-halfW, halfH, 0));
            mesh.Positions.Add(new Point3D(halfW, halfH, 0));
            mesh.Positions.Add(new Point3D(halfW, -halfH, 0));
            mesh.Positions.Add(new Point3D(-halfW, -halfH, 0));

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(0, 1));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);

            var image = new Image {Source = bitmap};
            image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.NearestNeighbor);
            image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.NearestNeighbor);

            var brush = new VisualBrush
            {
                Visual = image
            };

            return new GeometryModel3D(mesh, new DiffuseMaterial(brush));
        }
    }
}
