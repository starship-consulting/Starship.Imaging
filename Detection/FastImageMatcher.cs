using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Accord.Imaging;
using Starship.Imaging.Extensions;
using Starship.Imaging.Wrappers;

namespace Starship.Imaging.Detection {
    public class FastImageMatcher {
        public FastImageMatcher(Bitmap source) {
            Source = source;
        }

        public List<TemplateMatch> FindMatches(Bitmap template, float threshold = 1) {
            var results = new List<TemplateMatch>();
            var time = DateTime.Now;
            var pixelThreshold = (int)(255 * (1 - threshold));// * template.Width * template.Height;

            using(var source = Source.ChangePixelFormat(PixelFormat.Format32bppRgb).FastLock()) {
                using(var target = template.ChangePixelFormat(PixelFormat.Format32bppRgb).FastLock()) {

                    Parallel.For(0, source.Height, y => {
                        for (var x = 0; x < source.Width; x++) {
                            var match = IsMatch(x, y, source, target, pixelThreshold);

                            if(match) {
                                results.Add(new TemplateMatch(new Rectangle(x, y, target.Width, target.Height), 1));
                            }
                        };
                    });
                }
            }

            LastMatchMilliseconds = (int) (DateTime.Now - time).TotalMilliseconds;

            return results;
        }

        private bool IsMatch(int x, int y, FastBitmap source, FastBitmap target, int threshold) {
            if(x + target.Width > source.Width || y + target.Height > source.Height) {
                return false;
            }
            
            var allowance = 0;
            
            for(var x2 = 0; x2 < target.Width; x2++) {
                for(var y2 = 0; y2 < target.Height; y2++) {
                    allowance += threshold;

                    var sourcePixel = source.GetPixel(x + x2, y + y2);
                    var targetPixel = target.GetPixel(x2, y2);

                    if(sourcePixel.A != 0 && targetPixel.A != 0) {
                        allowance -= Math.Abs(sourcePixel.R - targetPixel.R);
                        allowance -= Math.Abs(sourcePixel.G - targetPixel.G);
                        allowance -= Math.Abs(sourcePixel.B - targetPixel.B);
                    }

                    if(allowance <= 0) {
                        return false;
                    }
                    
                    /*if(Math.Abs(sourcePixel.R - targetPixel.R) > threshold ||
                       Math.Abs(sourcePixel.G - targetPixel.G) > threshold ||
                       Math.Abs(sourcePixel.B - targetPixel.B) > threshold) {
                        return false;
                    }*/
                }
            }

            return true;
        }

        public List<TemplateMatch> FindExact(Bitmap target) {
            var results = new List<TemplateMatch>();

            if (target == null || Source.Width < target.Width || Source.Height < target.Height) {
                return results;
            }

            var time = DateTime.Now;
            var haystackArray = Source.GetPixelArray();
            var needleArray = target.GetPixelArray();

            foreach (var match in FindMatch(haystackArray.Take(Source.Height - target.Height), needleArray[0])) {
                if (IsNeedlePresentAtLocation(haystackArray, needleArray, match, 1)) {
                    results.Add(new TemplateMatch(new Rectangle(match.X, match.Y, target.Width, target.Height), 1));
                }
            }

            LastMatchMilliseconds = (int) (DateTime.Now - time).TotalMilliseconds;

            return results;
        }
        
        private IEnumerable<Point> FindMatch(IEnumerable<int[]> haystackLines, int[] needleLine) {
            var y = 0;

            foreach (var haystackLine in haystackLines) {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x) {
                    if (ContainSameElements(haystackLine, x, needleLine, 0, needleLine.Length)) {
                        yield return new Point(x, y);
                    }
                }

                y += 1;
            }
        }

        private bool ContainSameElements(int[] first, int firstStart, int[] second, int secondStart, int length) {
            for (int i = 0; i < length; ++i) {
                if (first[i + firstStart] != second[i + secondStart]) {
                    return false;
                }
            }

            return true;
        }

        private bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int alreadyVerified) {
            for (int y = alreadyVerified; y < needle.Length; ++y) {
                if (!ContainSameElements(haystack[y + point.Y], point.X, needle[y], 0, needle.Length)) {
                    return false;
                }
            }

            return true;
        }

        public int LastMatchMilliseconds { get; set; }

        private Bitmap Source { get; set; }
    }
}