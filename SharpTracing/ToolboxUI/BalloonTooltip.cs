using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TooboxUI.Components {
    [ProvideProperty("BalloonText", typeof (Control))]
    internal class BalloonToolTip : Component, IExtenderProvider {
        #region BalloonIcons enum

        public enum BalloonIcons : int {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        #endregion

        private const int CW_USEDEFAULT = unchecked((int) 0x80000000);
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int TTDT_AUTOPOP = 2;
        private const int TTDT_INITIAL = 3;
        private const int TTF_SUBCLASS = 0x0010;
        private const int TTF_TRANSPARENT = 0x0100;
        private const int TTM_ACTIVATE = WM_USER + 1;
        private const int TTM_ADDTOOL = WM_USER + 50;
        private const int TTM_DELTOOL = WM_USER + 51;
        private const int TTM_GETTOOLINFO = WM_USER + 53;
        private const int TTM_SETDELAYTIME = WM_USER + 3;
        private const int TTM_SETMAXTIPWIDTH = WM_USER + 24;
        private const int TTM_SETTIPBKCOLOR = WM_USER + 19;
        private const int TTM_SETTIPTEXTCOLOR = WM_USER + 20;
        private const int TTM_SETTITLE = WM_USER + 33;
        private const int TTM_SETTOOLINFO = WM_USER + 54;
        private const int TTM_UPDATETIPTEXT = WM_USER + 57;
        private const int TTS_ALWAYSTIP = 0x01;
        private const int TTS_BALLOON = 0x40;
        private const int TTS_NOPREFIX = 0x02;
        private const int WM_USER = 0x0400;
        private const int WS_POPUP = unchecked((int) 0x80000000);
        private int autopop;
        private Color bgcolor;
        private bool enabled;
        private Color fgcolor;
        private BalloonIcons icon;
        private int initial;
        private int max;
        private IntPtr tempptr;
        private toolinfo tf;
        private string title;
        private Hashtable tooltexts;
        private readonly IntPtr toolwindow;
        private readonly IntPtr TOPMOST = new IntPtr(-1);

        public BalloonToolTip() {
            // Private members initial values.
            this.max = 200;
            this.autopop = 5000;
            this.initial = 500;
            this.title = string.Empty;
            this.bgcolor = Color.FromKnownColor(KnownColor.Info);
            this.fgcolor = Color.FromKnownColor(KnownColor.InfoText);
            this.tooltexts = new Hashtable();
            this.enabled = true;
            this.icon = BalloonIcons.None;
            // Creating the tooltip control.
            this.toolwindow = CreateWindowEx(0, "tooltips_class32", string.Empty,
                                             WS_POPUP | TTS_NOPREFIX | TTS_ALWAYSTIP, CW_USEDEFAULT, CW_USEDEFAULT,
                                             CW_USEDEFAULT, CW_USEDEFAULT, IntPtr.Zero, 0, 0, 0);
            SetWindowPos(this.toolwindow, this.TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SendMessage(this.toolwindow, TTM_SETMAXTIPWIDTH, 0, new IntPtr(this.max));
            // Creating the toolinfo structure to be used later.
            this.tf = new toolinfo();
            this.tf.flag = TTF_SUBCLASS | TTF_TRANSPARENT;
            this.tf.size = Marshal.SizeOf(typeof (toolinfo));
        }

        // Extend any control except itself and the form, this function get called for use automaticly by the designer.

        #region IExtenderProvider Members

        public bool CanExtend(object extendee) {
            if (extendee is Control && !(extendee is BalloonToolTip) && !(extendee is Form)) {
                return true;
            }
            return false;
        }

        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr CreateWindowEx(int exstyle, string classname, string windowtitle, int style, int x,
                                                    int y, int width, int height, IntPtr parent, int menu, int nullvalue,
                                                    int nullptr);

        [DllImport("user32.dll")]
        private static extern int DestroyWindow(IntPtr hwnd);

        [DllImport("User32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
                                                int uFlags);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        ~BalloonToolTip() {
            this.Dispose(false);
        }

        // This is not a regular funtion, its our extender property seprated as two functions for get and set.
        public string GetBalloonText(Control parent) {
            if (this.tooltexts.Contains(parent)) {
                return this.tooltexts[parent].ToString();
            } else {
                return null;
            }
        }

        // This is where the tool text validated and updated for the controls.
        public void SetBalloonText(Control parent, string value) {
            if (value == null) {
                value = string.Empty;
            }
            // If the tool text have been cleared, remove the control from our service list.
            if (value == string.Empty) {
                this.tooltexts.Remove(parent);
                this.tf.parent = parent.Handle;
                this.tempptr = Marshal.AllocHGlobal(this.tf.size);
                Marshal.StructureToPtr(this.tf, this.tempptr, false);
                SendMessage(this.toolwindow, TTM_DELTOOL, 0, this.tempptr);
                Marshal.FreeHGlobal(this.tempptr);
                parent.Resize -= new EventHandler(this.Control_Resize);
            } else {
                this.tf.parent = parent.Handle;
                this.tf.rect = parent.ClientRectangle;
                this.tf.text = value;
                this.tempptr = Marshal.AllocHGlobal(this.tf.size);
                Marshal.StructureToPtr(this.tf, this.tempptr, false);
                if (this.tooltexts.Contains(parent)) {
                    this.tooltexts[parent] = value;
                    SendMessage(this.toolwindow, TTM_UPDATETIPTEXT, 0, this.tempptr);
                } else {
                    this.tooltexts.Add(parent, value);
                    SendMessage(this.toolwindow, TTM_ADDTOOL, 0, this.tempptr);
                    parent.Resize += new EventHandler(this.Control_Resize);
                }
                Marshal.FreeHGlobal(this.tempptr);
            }
        }

        // Overriding Dispose is a must to free our window handle we created at the constructor.
        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.tooltexts.Clear();
                this.tooltexts = null;
            }
            DestroyWindow(this.toolwindow); // Free the window handle obtained by CreateWindowEx.
            base.Dispose(disposing);
        }

        #region Private Methods

        private void Control_Resize(object sender, EventArgs e) {
            Control caller = (Control) sender;
            this.tf.parent = caller.Handle;
            this.tempptr = Marshal.AllocHGlobal(this.tf.size);
            Marshal.StructureToPtr(this.tf, this.tempptr, false);
            SendMessage(this.toolwindow, TTM_GETTOOLINFO, 0, this.tempptr);
            this.tf = (toolinfo) Marshal.PtrToStructure(this.tempptr, typeof (toolinfo));
            this.tf.rect = caller.ClientRectangle;
            Marshal.StructureToPtr(this.tf, this.tempptr, false);
            SendMessage(this.toolwindow, TTM_SETTOOLINFO, 0, this.tempptr);
            Marshal.FreeHGlobal(this.tempptr);
        }

        #endregion

        #region Public Properties

        [DefaultValue(BalloonIcons.None)]
        public BalloonIcons Icon {
            [DebuggerStepThrough] get { return this.icon; }
            set {
                this.icon = value;
                this.Title = this.title;
            }
        }

        [DefaultValue(200)]
        public int MaximumWidth {
            [DebuggerStepThrough] get { return this.max; }
            set {
                // Refuse any strange values, (feel free to modify).
                if (this.max >= 100 && this.max <= 2000) {
                    this.max = value;
                    SendMessage(this.toolwindow, TTM_SETMAXTIPWIDTH, 0, new IntPtr(this.max));
                }
            }
        }

        [DefaultValue(true)]
        public bool Enabled {
            [DebuggerStepThrough] get { return this.enabled; }
            set {
                this.enabled = value;
                SendMessage(this.toolwindow, TTM_ACTIVATE, Convert.ToInt32(this.enabled), new IntPtr(0));
            }
        }

        public string Title {
            [DebuggerStepThrough] get { return this.title; }
            set {
                this.title = value;
                this.tempptr = Marshal.StringToHGlobalUni(this.title);
                SendMessage(this.toolwindow, TTM_SETTITLE, (int) this.icon, this.tempptr);
                Marshal.FreeHGlobal(this.tempptr);
            }
        }

        // This property is by seconds.
        [DefaultValue(5)]
        public int AutoPop {
            [DebuggerStepThrough] get { return this.autopop / 1000; }
            set {
                // Refuse any strange values, (feel free to modify).
                if (value >= 1 && value < 120) {
                    this.autopop = value * 1000;
                    SendMessage(this.toolwindow, TTM_SETDELAYTIME, TTDT_AUTOPOP, new IntPtr(this.autopop));
                }
            }
        }

        // This property is by milliseconds ( 1 second = 1000 millisecond ).
        [DefaultValue(500)]
        public int Initial {
            [DebuggerStepThrough] get { return this.initial; }
            set {
                // Refuse any strange values, (feel free to modify).
                if (value >= 100 && value <= 2000) {
                    this.initial = value;
                    SendMessage(this.toolwindow, TTM_SETDELAYTIME, TTDT_INITIAL, new IntPtr(this.initial));
                }
            }
        }

        public Color BackColor {
            [DebuggerStepThrough] get { return this.bgcolor; }
            set {
                this.bgcolor = value;
                SendMessage(this.toolwindow, TTM_SETTIPBKCOLOR, ColorTranslator.ToWin32(value), new IntPtr(0));
            }
        }

        public Color ForeColor {
            [DebuggerStepThrough] get { return this.fgcolor; }
            set {
                this.fgcolor = value;
                SendMessage(this.toolwindow, TTM_SETTIPTEXTCOLOR, ColorTranslator.ToWin32(value), new IntPtr(0));
            }
        }

        #endregion

        #region Nested type: toolinfo

        private struct toolinfo {
            public int flag;
            public int id;
            public int nullvalue;
            public int param;
            public IntPtr parent;
            public Rectangle rect;
            public int size;
            [MarshalAs(UnmanagedType.LPTStr)] public string text;
        }

        #endregion
    }
}