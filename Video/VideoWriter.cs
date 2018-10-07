using System;
using System.Drawing;
using Accord.Video.FFMPEG;

namespace Starship.Imaging.Video {
    public class VideoWriter {

        public VideoWriter(string path, int width, int height) {
            Path = path;
            Width = width;
            Height = height;
        }

        public void Write(params Bitmap[] frames) {

            using (var writer = new VideoFileWriter()) {

                writer.Open(Path, Width, Height);
                
                foreach (var frame in frames) {
                    writer.WriteVideoFrame(frame);
                }

                writer.Close();
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Path { get; set; }
    }
}