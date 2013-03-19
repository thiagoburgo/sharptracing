using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TooboxUI.Components
{
    /// <summary>
    /// Represents a cursor wrapper of the <see cref="ToolboxItem"/> bitmap.
    /// </summary>
    public class ToolCursor : IDisposable
    {
        private static Pen _crossPen = new Pen(Color.Blue);
        private Cursor _cursor;
        private bool _disposed = false;
        private IntPtr _hIcon = IntPtr.Zero;
        private ToolboxItem _item;
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolCursor"/> class.
        /// </summary>
        /// <param name="item">A <see cref="ToolboxItem"/> object.</param>
        public ToolCursor(ToolboxItem item)
        {
            this._item = item;
            if(item.Bitmap != null){
                using(Bitmap bitmap = this.CreateCursor(item.Bitmap)){
                    this._hIcon = bitmap.GetHicon();
                    this._cursor = new Cursor(this._hIcon);
                }
            }
        }
        /// <summary>
        /// Gets a <see cref="Cursor"/> with the <see cref="ToolboxItem.Bitmap"/> and a cross.
        /// </summary>
        public Cursor Cursor
        {
            [DebuggerStepThrough]
            get { return this._cursor; }
        }
        /// <summary>
        /// Gets a <see cref="ToolboxItem"/>.
        /// </summary>
        public ToolboxItem Item
        {
            [DebuggerStepThrough]
            get { return this._item; }
        }

        #region IDisposable Members
        /// <summary>
        /// Disposes the <see cref="ToolCursor"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion

        [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
        private static extern bool DestroyIcon(IntPtr hIcon);
        private Bitmap CreateCursor(Bitmap itemBitmap)
        {
            Bitmap bitmap = new Bitmap(24, 24);
            using(Graphics graphics = Graphics.FromImage(bitmap)){
                graphics.DrawImage(itemBitmap, new Rectangle(7, 7, 16, 16));
                graphics.DrawLine(_crossPen, 6, 1, 6, 10);
                graphics.DrawLine(_crossPen, 1, 6, 10, 6);
            }
            return bitmap;
        }
        /// <summary>
        /// Finalizes the <see cref="ToolCursor"/> if it was not disposed.
        /// </summary>
        ~ToolCursor()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false)is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            if(!this._disposed){
                if(this._hIcon != IntPtr.Zero){
                    DestroyIcon(this._hIcon);
                }
                if(disposing){
                    GC.SuppressFinalize(this);
                } else{
                    this._cursor.Dispose();
                }
                this._disposed = true;
            }
        }
    }
}