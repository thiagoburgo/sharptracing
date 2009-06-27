using System;
using System.Drawing;
using Alsing.Windows;

namespace Alsing.Drawing
{
    public class DesktopGraphics : IDisposable
    {
        public readonly Graphics Graphics;
        protected IntPtr handle = new IntPtr(0);
        protected IntPtr hdc = new IntPtr(0);
        public DesktopGraphics()
        {
            this.handle = NativeMethods.GetDesktopWindow();
            this.hdc = NativeMethods.GetWindowDC(this.hdc);
            this.Graphics = Graphics.FromHdc(this.hdc);
        }

        #region IDisposable Members
        public void Dispose()
        {
            NativeMethods.ReleaseDC(this.handle, this.hdc);
        }
        #endregion
    }
}