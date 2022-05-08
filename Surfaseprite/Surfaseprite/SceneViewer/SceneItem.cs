using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Surfaseprite.SceneViewer
{
    /// <summary>
    /// Container with info and references of concepts that represent a single Node in the WPF SceneViewport.
    /// </summary>
    public class SceneItem
    {
        /// <summary>
        /// Domain id of the <see cref="SceneItem"/>.
        /// </summary>
        public ulong ItemId { get; }

        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// A direct reference to the WPF resource of the bitmap.
        /// </summary>
        public readonly WriteableBitmap Bitmap;

        /// <summary>
        /// The graphical WPF item inside the viewport representing the Node.
        /// </summary>
        public readonly Visual3D Visual;
        /// <summary>
        /// A direct reference to the transform of the Visual in the viewport, so we can manipulate it easily.
        /// </summary>
        public readonly MatrixTransform3D Transform;

        public SceneItem(Visual3D visual, ulong itemId, MatrixTransform3D transform, WriteableBitmap bitmap)
        {
            ItemId = itemId;
            Visual = visual;
            Transform = transform;
            Bitmap = bitmap;
        }
    }
}
