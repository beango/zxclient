using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ZXClient.Imports.gdi32
{
    /// <summary>
    /// Engloba a declaração de métodos importados da gdi32.dll
    /// </summary>
    public static class NativeMethods
    {
        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hDc);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
    }
}
