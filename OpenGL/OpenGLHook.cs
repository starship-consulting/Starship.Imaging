using System;

namespace Starship.Imaging.OpenGL {
    
    /*public class OpenGLHook : IEntryPoint {
        GlMon.GlMonInterface Interface;
        Stack<String> Queue = new Stack<String>();
        public LocalHook CreateBufferHook = null;
        Bitmap bitmap = null;

        public OpenGLHook(RemoteHooking.IContext InContext, String InChannelName) {
            Interface = RemoteHooking.IpcConnectClient<GlMon.GlMonInterface>(InChannelName);
            Interface.Ping();
        }

        public void Run(RemoteHooking.IContext InContext, string InChannelName) {
            // install hook...
            try {
                CreateBufferHook = LocalHook.Create(LocalHook.GetProcAddress("opengl32.dll", "wglSwapBuffers"), new DwglSwapBuffers(SwapBuffers_Hooked), this);
                CreateBufferHook.ThreadACL.SetExclusiveACL(new Int32[] {0});
            }
            catch (Exception ExtInfo) {
                Interface.ReportException(ExtInfo);

                return;
            }

            Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());

            RemoteHooking.WakeUpProcess();
            try {
                while (true) {
                    Thread.Sleep(500);

                    // transmit newly monitored file accesses...
                    if (this.bitmap != null) {
                        Interface.onSwapBuffer(RemoteHooking.GetCurrentProcessId(), "bitmap received. size=" + bitmap.Size);
                        this.bitmap = null;
                    }
                    else {
                        Interface.onSwapBuffer(RemoteHooking.GetCurrentProcessId(), "ping..");

                        Interface.Ping();
                    }
                }
            }
            catch {
                // Ping() will raise an exception if host is unreachable
            }
            // wait for host process termination...
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr DwglSwapBuffers(IntPtr hdc);


        [DllImport("opengl32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr wglSwapBuffers(IntPtr hdc);


        // this is where we are intercepting all file accesses!
        static IntPtr SwapBuffers_Hooked(IntPtr hdc) {
            try {
                Bitmap bitmap = new Bitmap(860, 720);
                Rectangle bounds = new Rectangle(0, 0, 860, 720);
                BitmapData bmpData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                wgl.glReadBuffer((uint) wgl.DrawBufferMode.GL_FRONT_AND_BACK);
                wgl.glReadPixels(0, 0, bounds.Width, bounds.Height, (uint) wgl.PixelFormat.GL_RGB, (uint) wgl.DataType.GL_UNSIGNED_BYTE, bmpData.Scan0);
                bitmap.UnlockBits(bmpData);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                int i = 0;
                while (File.Exists("images/test_" + i + ".png")) {
                    i++;
                }
                bitmap.Save("images/test_" + i + ".png");
                return wgl.SwapBuffers(hdc);
            }
            catch {
            }
            // call original API...
            return wglSwapBuffers(hdc);
        }
    }*/
}