using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Starship.Imaging.Extensions;
using Starship.Imaging.Wrappers;
using Image = System.Drawing.Image;

namespace Starship.Imaging.Detection {
    public class ImageColorReplacer {

        public ImageColorReplacer(Image source) {
            Source = new Bitmap(source);
        }

        public Bitmap MakeGrayscale() {
            var filter = new Grayscale(0.2125, 0.7154, 0.0721);
            return filter.Apply(Source);
        }

        public void ConvertDarkest(Color sourceColor) {

            var data = Source.LockBits(new Rectangle(0, 0, Source.Width, Source.Height), ImageLockMode.ReadWrite, Source.PixelFormat);

            ForEachPixel(pixel => {

            });
        }

        public void Convert(Color sourceColor, Color targetColor, float threshold = 1) {
            var time = DateTime.Now;
            var pixelThreshold = (int)(255 * (1 - threshold));

            using(var source = Source.ChangePixelFormat(PixelFormat.Format32bppRgb).FastLock()) {

                var data = Source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, Source.PixelFormat);

                Parallel.For(0, source.Width, (x) => {
                    Parallel.For(0, source.Height, (y) => {
                        var pixel = source.GetPixel(x, y);
                        
                        if(Math.Abs(pixel.R - sourceColor.R) > pixelThreshold || Math.Abs(pixel.G - sourceColor.G) > pixelThreshold || Math.Abs(pixel.B - sourceColor.B) > pixelThreshold) {

                        }
                        else {
                            Drawing.FillRectangle(data, new Rectangle(x, y, 1, 1), targetColor);
                        }
                    });
                });

                Source.UnlockBits(data);
                Source.OpenInPaint();
            }

            LastMatchMilliseconds = (int) (DateTime.Now - time).TotalMilliseconds;
        }

        private void ForEachPixel(Action<Color> action) {
            using(var source = Source.ChangePixelFormat(PixelFormat.Format32bppRgb).FastLock()) {
                Parallel.For(0, source.Width, (x) => {
                    Parallel.For(0, source.Height, (y) => {
                        var pixel = source.GetPixel(x, y);
                        action(pixel);
                    });
                });
            }
        }

        public int LastMatchMilliseconds { get; set; }

        private Bitmap Source { get; set; }
    }
}