using System;
using System.Buffers;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Surfaseprite.SceneViewer
{
    internal static class WritableBitmapExtensions
    {
        public static Vector2 GetTexelCoordsFromUV(this WriteableBitmap bitmap, Vector2 uv)
        {
            return new Vector2(uv.X * bitmap.PixelWidth, uv.Y * bitmap.PixelHeight);
        }

        public delegate void PixelUpdateAction(Span<byte> pixelData);

        /// <summary>
        /// Allows to update the pixel data in one read+write operation to the gpu.
        /// </summary>
        public static void UpdatePixels(this WriteableBitmap bitmap, PixelUpdateAction updateAction)
        {
            var length = bitmap.PixelWidth * bitmap.PixelHeight * 4;
            var pixels = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                bitmap.CopyPixels(pixels, bitmap.BackBufferStride, 0);
                updateAction(new Span<byte>(pixels, 0, length));
                bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, bitmap.BackBufferStride, 0);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pixels);
            }
        }
    }
}
