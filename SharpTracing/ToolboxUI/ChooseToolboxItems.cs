using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TooboxUI.Components {
    public partial class ChooseToolboxItemsDialog : Form {
        private List<ListViewItem> colOrig;
        private Type[] selectedTypes = null;
        private Type[] tarray;
        private List<ListViewItem> view;

        public ChooseToolboxItemsDialog() {
            this.InitializeComponent();
            Application.EnableVisualStyles();
            Application.DoEvents();
            this._lvwItemComparer = new ListViewItemComparer();
            this.listViewComponents.ListViewItemSorter = this._lvwItemComparer;
        }

        public Type[] SelectedTypes {
            get { return this.selectedTypes; }
        }

        private void btnChoose_Click(object sender, EventArgs e) {
            //openFileDialog1.InitialDirectory = @"C:\WINDOWS\Microsoft.NET\Framework\v1.0.3705";
            this.openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            this.openFileDialog1.Filter = "Assemblies.NET (*.dll, *.exe)|*.dll;*.exe";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK) {
                //                this.txtFilter.Text = openFileDialog1.FileName;
                Assembly a;
                try {
                    a = Assembly.LoadFrom(this.openFileDialog1.FileName);
                    this.tarray = a.GetExportedTypes();
                } catch (Exception ex) {
                    MessageBox.Show("Error in loading assembly");
                    return;
                }
                foreach (Type type in this.tarray) {
                    if ((ImplementsInterface(type, typeof (IComponent))) && !type.IsSubclassOf(typeof (Form)) &&
                        !type.IsAbstract) {
                        bool isDesignTimeVisible;
                        if (this.HasDesignTimeVisible(type, out isDesignTimeVisible)) {
                            if (isDesignTimeVisible) {
                                ListViewItem lvi =
                                    new ListViewItem(new string[] {
                                                                      type.Name, type.Namespace, type.Module.Name,
                                                                      type.Assembly.GetName().Version.ToString()
                                                                  });
                                lvi.Tag = type;
                                if (!this.listViewComponents.Items.Contains(lvi)) {
                                    this.listViewComponents.Items.Add(lvi);
                                }
                                if (this.view != null) {
                                    if (!this.view.Contains(lvi)) {
                                        this.view.Add(lvi);
                                    }
                                }
                            }
                        } else {
                            ListViewItem lvi =
                                new ListViewItem(new string[] {
                                                                  type.Name, type.Namespace, type.Module.Name,
                                                                  type.Assembly.GetName().Version.ToString()
                                                              });
                            lvi.Tag = type;
                            if (!this.listViewComponents.Items.Contains(lvi)) {
                                this.listViewComponents.Items.Add(lvi);
                            }
                            if (this.view != null) {
                                if (!this.view.Contains(lvi)) {
                                    this.view.Add(lvi);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool HasDesignTimeVisible(Type type, out bool isVisible) {
            DesignTimeVisibleAttribute[] atts =
                type.GetCustomAttributes(typeof (DesignTimeVisibleAttribute), true) as DesignTimeVisibleAttribute[];
            isVisible = false;
            if (atts != null && atts.Length > 0) {
                foreach (DesignTimeVisibleAttribute att in atts) {
                    if (att.Visible) {
                        isVisible = att.Visible;
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        private static bool ImplementsInterface(Type objToVerify, Type interfaceTypeToVerify) {
            if (objToVerify == null) {
                return false;
            }
            foreach (Type type in objToVerify.GetInterfaces()) {
                if (type == interfaceTypeToVerify) {
                    return true;
                }
            }
            return false;
        }

        // Perform Sorting on Column Headers
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e) {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == this._lvwItemComparer.SortColumn) {
                // Reverse the current sort direction for this column.
                if (this._lvwItemComparer.Order == SortOrder.Ascending) {
                    this._lvwItemComparer.Order = SortOrder.Descending;
                } else {
                    this._lvwItemComparer.Order = SortOrder.Ascending;
                }
            } else {
                // Set the column number that is to be sorted; default to ascending.
                this._lvwItemComparer.SortColumn = e.Column;
                this._lvwItemComparer.Order = SortOrder.Ascending;
            }
            // Perform the sort with these new sort options.
            this.listViewComponents.Sort();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            if (e.IsSelected) {
                Type type = ((Type) e.Item.Tag);
                this.componentIcon.Image = this.GetBitmapFromComponent(type);
                this.lblLanguage.Text = type.Assembly.GetName().CultureInfo.DisplayName;
                this.lblVersion.Text = type.Assembly.GetName().Version.ToString();
                this.groupType.Text = type.Name;
            }

            #region

            ListView.SelectedListViewItemCollection itens = ((ListView) sender).SelectedItems;
            if (itens != null) {
                this.selectedTypes = new Type[itens.Count];
                for (int i = 0; i < itens.Count; i++) {
                    this.selectedTypes[i] = (Type) itens[i].Tag;
                }
            }

            #endregion
        }

        private Image GetBitmapFromComponent(Type type) {
            Image bmp = ToolboxBitmapAttribute.GetImageFromResource(type, null, false) as Bitmap;
            if (bmp != null) {
                return bmp;
            } else {
                return ToolboxBitmapAttribute.Default.GetImage(type);
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e) {
            if (this.view == null) {
                if (this.listViewComponents.Items.Count > 0) {
                    this.view = new List<ListViewItem>(this.listViewComponents.Items.Count);
                    foreach (ListViewItem item in this.listViewComponents.Items) {
                        this.view.Add((ListViewItem) item.Clone());
                    }
                }
            }
            if (this.view != null) {
                if (this.txtFilter.Text != null && this.txtFilter.Text.Trim() != "") {
                    this.colOrig = new List<ListViewItem>(this.view);
                    foreach (ListViewItem item in this.view) {
                        if (!item.Text.ToLower().Contains(this.txtFilter.Text.ToLower())) {
                            this.colOrig.Remove(item);
                        }
                    }
                    this.listViewComponents.Items.Clear();
                    this.listViewComponents.Items.AddRange(this.colOrig.ToArray());
                } else {
                    this.listViewComponents.Items.Clear();
                    this.listViewComponents.Items.AddRange(this.view.ToArray());
                }
            }
        }

        private void btnFilter_Click(object sender, EventArgs e) {}

        private void btnOk_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.selectedTypes = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listViewComponents_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listViewComponents_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}