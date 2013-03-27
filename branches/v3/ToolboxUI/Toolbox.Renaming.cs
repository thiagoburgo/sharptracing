using System;
using System.Drawing;
using System.Windows.Forms;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region RenameTextBox

        private class RenameTextBox : TextBox {
            private string _currentCaption;
            private readonly Toolbox _owner;
            private IToolboxObject _tool;

            public RenameTextBox(Toolbox toolbox) {
                this._owner = toolbox;
                this.Visible = false;
                //this.BorderStyle = BorderStyle.None;
                toolbox.Controls.Add(this);
            }

            public IToolboxObject Tool {
                get { return this._tool; }
                set { this._tool = value; }
            }

            public string Caption {
                get {
                    if (this.Tool is Item && string.IsNullOrEmpty(this._currentCaption)) {
                        this._currentCaption = ((Item) this.Tool).Text;
                    }
                    return this._currentCaption;
                }
                set { this._currentCaption = value; }
            }

            public new void Show() {
                if (this._tool == null) {
                    throw new Exception(Resources.ToolboxRenamerNothingToRename);
                }
                Rectangle rect;
                if (this._tool is Tab) {
                    Tab tab = (Tab) this._tool;
                    rect = tab.GetCaptionRectangle(false);
                    tab.Renaming = true;
                } else if (this._tool is Item) {
                    Item item = (Item) this._tool;
                    rect = item.GetBounds(false);
                    item.Renaming = true;
                } else {
                    throw new Exception(Resources.ToolboxRenamerNotItemNorTab);
                }
                this.Visible = true;
                this.Text = this.Caption;
                this.Capture = true;
                this.MaximumSize = rect.Size;
                int yOffset = (rect.Height > this.Height) ? (rect.Height - this.Height) / 2 : 0;
                this.Bounds = new Rectangle(rect.Left, rect.Top + yOffset, rect.Width, rect.Height);
                this.BringToFront();
                this.Focus();
            }

            protected override void OnValidated(EventArgs e) {
                if (this.Text == string.Empty && string.IsNullOrEmpty(this.Caption) && this.Tool is Tab) {
                    Tab tab = (Tab) this.Tool;
                    bool allowDelete = tab.AllowDelete;
                    tab.AllowDelete = true;
                    try {
                        this._owner.Categories.Remove(tab);
                    } finally {
                        tab.AllowDelete = allowDelete;
                    }
                } else if (this.Text != string.Empty) {
                    if (this.Tool is Tab) {
                        ((Tab) this.Tool).Text = this.Text;
                    } else if (this.Tool is Item) {
                        ((Item) this.Tool).Text = this.Text;
                    }
                }
                this.HideRenamer();
                base.OnValidated(e);
            }

            private void ShowWarning(string text) {
                MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            protected override void OnKeyDown(KeyEventArgs e) {
                if (e.KeyData == Keys.Enter) {
                    if (this.Text == string.Empty) {
                        this.ShowWarning(Resources.ToolboxEmptyTextMessage);
                    } else {
                        if (this.Tool is Tab) {
                            ((Tab) this.Tool).Text = this.Text;
                        } else if (this.Tool is Item) {
                            ((Item) this.Tool).Text = this.Text;
                        }
                        this.HideRenamer();
                    }
                } else if (e.KeyData == Keys.Escape) {
                    this.HideRenamer();
                } else {
                    base.OnKeyDown(e);
                }
            }

            protected override void OnMouseDown(MouseEventArgs e) {
                if (!this.ClientRectangle.Contains(e.Location)) {
                    this.OnValidated(EventArgs.Empty);
                }
                base.OnMouseDown(e);
            }

            private void HideRenamer() {
                this.Capture = false;
                if (this.Focused) {
                    this.InvokeLostFocus(this, EventArgs.Empty);
                }
                if (this.Tool is Tab) {
                    ((Tab) this.Tool).Renaming = false;
                } else if (this.Tool is Item) {
                    ((Item) this.Tool).Renaming = false;
                }
                this.Visible = false;
            }
        }

        #endregion

        private RenameTextBox _renamerBox;

        private RenameTextBox RenamerBox {
            get {
                if (this._renamerBox == null) {
                    this._renamerBox = new RenameTextBox(this);
                }
                return this._renamerBox;
            }
        }

        /// <summary>
        /// Starts renaming of the <paramref name="tab"/> using the <paramref name="baseName"/>.
        /// </summary>
        /// <param name="tab">A <see cref="Tab"/> to rename.</param>
        /// <param name="baseName">A default name displayed in the rename textbox.</param>
        protected virtual void OnRenameTab(Tab tab, string baseName) {
            RenameTextBox box = this.RenamerBox;
            box.Tool = tab;
            box.Caption = baseName;
            box.Show();
        }

        /// <summary>
        /// Starts renaming of the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An <see cref="Item"/> to rename.</param>
        protected virtual void OnRenameItem(Item item) {
            RenameTextBox box = this.RenamerBox;
            box.Tool = item;
            box.Caption = item.Text;
            box.Show();
        }
    }
}