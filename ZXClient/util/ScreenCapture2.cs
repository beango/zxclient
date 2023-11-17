using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ZXClient.util
{
    class ScreenCapture2 : IDisposable
    {
        private static ScreenCapture2 instance = null;

        internal static ScreenCapture2 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScreenCapture2();
                }
                return ScreenCapture2.instance;
            }
        }

        int hdcSrc, hdcDest;

        private ScreenCapture2()
        {
            hdcSrc = User32.GetWindowDC(User32.GetDesktopWindow());
            hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
        }

        /// <summary>
        /// 屏幕捕捉
        /// </summary>
        /// <param name="rct">要捕捉的桌面区域</param>
        /// <returns>捕获后的图形</returns>
        internal Bitmap Capture(Rectangle rct)
        {
            int hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, rct.Width, rct.Height);

            GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, rct.Width, rct.Height,
                            hdcSrc, rct.Left, rct.Top, 0x00CC0020);
            Bitmap p_w_picpath = new Bitmap(Image.FromHbitmap(new IntPtr(hBitmap)),
                     Image.FromHbitmap(new IntPtr(hBitmap)).Width,
                     Image.FromHbitmap(new IntPtr(hBitmap)).Height);

            GDI32.DeleteObject(hBitmap);
            return p_w_picpath;

        }

        #region IDisposable Members

        public void Dispose()
        {
            User32.ReleaseDC(User32.GetDesktopWindow(), hdcSrc);
            GDI32.DeleteDC(hdcDest);
        }
        #endregion
    }

    //下面二个类来自:http://www.c-sharpcorner.com/Code/2003/Dec/ScreenCapture.asp
    class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(int hdcDest, int nXDest, int nYDest,
                                         int nWidth, int nHeight, int hdcSrc,
                                         int nXSrc, int nYSrc, int dwRop);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleBitmap(int hdc, int nWidth,
                                                         int nHeight);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(int hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);
        [DllImport("GDI32.dll")]
        public static extern int SelectObject(int hdc, int hgdiobj);
    }

    class User32
    {
        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();
        [DllImport("User32.dll")]
        public static extern int GetWindowDC(int hWnd);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);
    }


}