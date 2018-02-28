using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Starship.Imaging.Extensions;
using Image = System.Drawing.Image;

namespace Starship.Imaging.Matching {
    public class AccordImageMatcher {

        public AccordImageMatcher(Image source) {
            Source = new Bitmap(source);
        }

        public List<TemplateMatch> PyramidFind(Image compare, float threshold = .95f, int divisor = 2) {
            var sourceImage = Source.ChangePixelFormat(PixelFormat.Format24bppRgb);
            var compareImage = new Bitmap(compare).ChangePixelFormat(PixelFormat.Format24bppRgb);
            var matching = new ExhaustiveTemplateMatching();

            return matching.ProcessImage(
                new ResizeNearestNeighbor(sourceImage.Width / divisor, sourceImage.Height / divisor).Apply(sourceImage),
                new ResizeNearestNeighbor(compareImage.Width / divisor, compareImage.Height / divisor).Apply(compareImage)).ToList();
        }

        public bool IsExactMatch(Image comparator) {
            return Compare(comparator, .95f).Any(each => each.Similarity == 1);
        }

        public bool IsCloseMatch(Image comparator) {
            return Compare(comparator, .95f).Any();
        }
        
        public List<TemplateMatch> Compare(Image comparator, float threshold) {
            var time = DateTime.Now;
            var bmp1 = Source.ChangePixelFormat(PixelFormat.Format24bppRgb);
            var bmp2 = new Bitmap(comparator).ChangePixelFormat(PixelFormat.Format24bppRgb);
            var matching = new ExhaustiveTemplateMatching(threshold);
            var results = matching.ProcessImage(bmp1, bmp2).ToList();
            LastMatchMilliseconds = (int)(DateTime.Now - time).TotalMilliseconds;
            return results;
        }

        public int LastMatchMilliseconds { get; set; }

        private Bitmap Source { get; set; }
    }
}