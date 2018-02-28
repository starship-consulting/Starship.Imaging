using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Image = System.Drawing.Image;

namespace Starship.Imaging.Extensions {
    public static class ImageExtensions {
        
        public static Bitmap Crop(this Image source, Rectangle selection) {
            var image = new Bitmap(selection.Width, selection.Height);

            using (var graphics = Graphics.FromImage(image)) {
                graphics.DrawImage(source, 0, 0, selection, GraphicsUnit.Pixel);
            }

            return image;
        }

        public static byte[] ToBytes(this Image image, ImageFormat format = null) {
            using (var stream = new MemoryStream()) {
                if (format == null) {
                    format = image.RawFormat;
                }

                image.Save(stream, format);
                return stream.ToArray();
            }
        }

        public static void OpenInPaint(this Image image) {
            var path = Path.GetTempFileName() + ".png";
            image.Save(path, ImageFormat.Png);

            Process.Start("mspaint.exe", path);
        }
        
        public static Bitmap ChangePixelFormat(this Bitmap inputImage, PixelFormat newFormat) {
            return inputImage.Clone(new Rectangle(0, 0, inputImage.Width, inputImage.Height), newFormat);
        }

        public static int[][] GetPixelArray(this Bitmap bitmap) {
            var result = new int[bitmap.Height][];

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            for (int y = 0; y < bitmap.Height; ++y) {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }
    }
}