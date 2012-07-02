// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Alsing.Design
{
    public class ComponaCollectionEditor : CollectionEditor
    {
        #region EditorImplementation
        private ComponaCollectionForm Form;
        public ComponaCollectionEditor(Type t) : base(t) {}
        public IDesignerHost DesignerHost
        {
            get
            {
                var designer = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                return designer;
            }
        }
        public void AddObject(object o)
        {
            this.Form.AddObject(o);
        }
        public void RemoveObject(object o)
        {
            this.Form.RemoveObject(o);
        }
        protected virtual CollectionEditorGui CreateGUI()
        {
            return new CollectionEditorGui();
        }
        protected override CollectionForm CreateCollectionForm()
        {
            this.Form = new ComponaCollectionForm(this){StartPosition = FormStartPosition.CenterScreen};
            return this.Form;
        }
        #endregion

        #region CollectionForm
        protected class ComponaCollectionForm : CollectionForm
        {
            private readonly ArrayList CreatedItems = new ArrayList();
            private readonly ComponaCollectionEditor Editor;
            private readonly CollectionEditorGui GUI;
            private readonly ArrayList RemovedItems = new ArrayList();
            private bool IsDirty;
            public ComponaCollectionForm(CollectionEditor e) : base(e)
            {
                this.Editor = e as ComponaCollectionEditor;
                if(this.Editor != null){
                    this.GUI = this.Editor.CreateGUI();
                }
                this.GUI.Visible = true;
                this.GUI.Dock = DockStyle.Fill;
                this.Controls.Add(this.GUI);
                this.AcceptButton = this.GUI.btnOK;
                this.CancelButton = this.GUI.btnCancel;
                this.Size = new Size(630, 470);
                this.GUI.Editor = e as ComponaCollectionEditor;
                Type[] types = this.NewItemTypes;
                if(types.Length > 1){
                    this.GUI.btnDropdown.Visible = true;
                    this.GUI.btnDropdown.ContextMenu = new ContextMenu();
                    for(int i = 0; (i < types.Length); i ++){
                        this.GUI.btnDropdown.ContextMenu.MenuItems.Add(new TypeMenuItem(types[i],
                                                                                        this.btnDropDownMenuItem_Click));
                    }
                }
                this.GUI.btnRemove.Click += this.btnRemove_Click;
                this.GUI.btnAdd.Click += this.btnAdd_Click;
                this.GUI.btnCancel.Click += this.btnCancel_Click;
                this.GUI.btnOK.Click += this.btnOK_Click;
                this.GUI.btnUp.Click += this.btnUp_Click;
                this.GUI.btnDown.Click += this.btnDown_Click;
                this.GUI.btnDropdown.Click += this.btnDropDown_Click;
            }
            public void RemoveObject(object o)
            {
                int index = this.GUI.lstMembers.Items.IndexOf(o);
                this.RemovedItems.Add(o);
                object i = o;
                this.Editor.DestroyInstance(i);
                this.CreatedItems.Remove(i);
                this.GUI.lstMembers.Items.RemoveAt(this.GUI.lstMembers.SelectedIndex);
                this.IsDirty = true;
                if(index < this.GUI.lstMembers.Items.Count){
                    this.GUI.lstMembers.SelectedIndex = index;
                } else if(this.GUI.lstMembers.Items.Count > 0){
                    this.GUI.lstMembers.SelectedIndex = this.GUI.lstMembers.Items.Count - 1;
                }
            }
            public void AddObject(object o)
            {
                var e = this.GUI.EditValue as IList;
                e.Add(o);
                this.IsDirty = true;
                this.GUI.lstMembers.Items.Add(o);
                this.CreatedItems.Add(o);
                if(o is Component){
                    var cp = o as Component;
                    this.Editor.DesignerHost.Container.Add(cp);
                }
                var Items = new object[((uint)this.GUI.lstMembers.Items.Count)];
                for(int i = 0; (i < Items.Length); i++){
                    Items[i] = this.GUI.lstMembers.Items[i];
                }
            }
            protected void btnUp_Click(object o, EventArgs e)
            {
                int i = this.GUI.lstMembers.SelectedIndex;
                if(i < 1){
                    return;
                }
                this.IsDirty = true;
                int j = this.GUI.lstMembers.TopIndex;
                object item = this.GUI.lstMembers.Items[i];
                this.GUI.lstMembers.Items[i] = this.GUI.lstMembers.Items[(i - 1)];
                this.GUI.lstMembers.Items[(i - 1)] = item;
                if(j > 0){
                    this.GUI.lstMembers.TopIndex = (j - 1);
                }
                this.GUI.lstMembers.ClearSelected();
                this.GUI.lstMembers.SelectedIndex = (i - 1);
            }
            protected void btnDropDown_Click(object o, EventArgs e)
            {
                this.GUI.btnDropdown.ContextMenu.Show(this.GUI.btnDropdown, new Point(0, this.GUI.btnDropdown.Height));
            }
            protected void btnDropDownMenuItem_Click(object o, EventArgs e)
            {
                var tmi = o as TypeMenuItem;
                if(tmi != null){
                    this.CreateAndAddInstance(tmi.Type as Type);
                }
            }
            protected void btnDown_Click(object o, EventArgs e)
            {
                int i = this.GUI.lstMembers.SelectedIndex;
                if(i >= this.GUI.lstMembers.Items.Count - 1 && i >= 0){
                    return;
                }
                this.IsDirty = true;
                int j = this.GUI.lstMembers.TopIndex;
                object item = this.GUI.lstMembers.Items[i];
                this.GUI.lstMembers.Items[i] = this.GUI.lstMembers.Items[(i + 1)];
                this.GUI.lstMembers.Items[(i + 1)] = item;
                if(j < this.GUI.lstMembers.Items.Count - 1){
                    this.GUI.lstMembers.TopIndex = (j + 1);
                }
                this.GUI.lstMembers.ClearSelected();
                this.GUI.lstMembers.SelectedIndex = (i + 1);
            }
            protected void btnRemove_Click(object o, EventArgs e)
            {
                int index = this.GUI.lstMembers.SelectedIndex;
                this.RemovedItems.Add(this.GUI.lstMembers.SelectedItem);
                object i = this.GUI.lstMembers.SelectedItem;
                this.Editor.DestroyInstance(i);
                this.CreatedItems.Remove(i);
                this.GUI.lstMembers.Items.RemoveAt(this.GUI.lstMembers.SelectedIndex);
                this.IsDirty = true;
                if(index < this.GUI.lstMembers.Items.Count){
                    this.GUI.lstMembers.SelectedIndex = index;
                } else if(this.GUI.lstMembers.Items.Count > 0){
                    this.GUI.lstMembers.SelectedIndex = this.GUI.lstMembers.Items.Count - 1;
                }
            }
            protected void btnAdd_Click(object o, EventArgs e)
            {
                this.CreateAndAddInstance(base.NewItemTypes[0]);
            }
            protected void btnCancel_Click(object o, EventArgs e)
            {
                if(this.IsDirty){
                    foreach(object i in this.RemovedItems){
                        base.DestroyInstance(i);
                    }
                    //					object[] items = new object[((uint) GUI.lstMembers.Items.Count)];
                    //					for (int i = 0; i < items.Length; i++)
                    //					{
                    //						items[i] = GUI.lstMembers.Items[i];
                    //					}
                    //					base.Items = items;
                }
                this.ClearAll();
            }
            protected void btnOK_Click(object o, EventArgs e)
            {
                if(this.IsDirty){
                    foreach(object i in this.RemovedItems){
                        base.DestroyInstance(i);
                    }
                    var items = new object[((uint)this.GUI.lstMembers.Items.Count)];
                    for(int i = 0; i < items.Length; i++){
                        items[i] = this.GUI.lstMembers.Items[i];
                    }
                    base.Items = items;
                }
                this.ClearAll();
            }
            private void ClearAll()
            {
                this.CreatedItems.Clear();
                this.RemovedItems.Clear();
                this.IsDirty = false;
            }
            protected override void OnEditValueChanged() {}
            protected static void OnComponentChanged(object o, ComponentChangedEventArgs e) {}
            protected override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
            {
                IComponentChangeService Service = null;
                DialogResult Result;
                Result = DialogResult.Cancel;
                this.GUI.EditorService = edSvc;
                try{
                    Service = ((IComponentChangeService)this.Editor.Context.GetService(typeof(IComponentChangeService)));
                    if(Service != null){
                        Service.ComponentChanged += OnComponentChanged;
                    }
                    this.GUI.EditValue = this.EditValue;
                    this.GUI.Bind();
                    this.GUI.ActiveControl = this.GUI.lstMembers;
                    this.ActiveControl = this.GUI;
                    Result = base.ShowEditorDialog(edSvc);
                } finally{
                    if(Service != null){
                        Service.ComponentChanged -= OnComponentChanged;
                    }
                }
                return Result;
            }
            private void CreateAndAddInstance(Type type)
            {
                try{
                    object NewInstance = this.CreateInstance(type);
                    if(NewInstance != null){
                        this.IsDirty = true;
                        this.CreatedItems.Add(NewInstance);
                        this.GUI.lstMembers.Items.Add(NewInstance);
                        this.GUI.lstMembers.Invalidate();
                        this.GUI.lstMembers.ClearSelected();
                        this.GUI.lstMembers.SelectedIndex = (this.GUI.lstMembers.Items.Count - 1);
                        var array1 = new object[((uint)this.GUI.lstMembers.Items.Count)];
                        for(int i = 0; (i < array1.Length); i++){
                            array1[i] = this.GUI.lstMembers.Items[i];
                        }
                        this.Items = array1;
                    }
                    this.IsDirty = true;
                } catch(Exception x){
                    base.DisplayError(x);
                }
            }
        }
        #endregion

        #region Nested type: TypeMenuItem
        public class TypeMenuItem : MenuItem
        {
            #region PUBLIC PROPERTY TYPE
            public object Type { get; set; }
            #endregion

            public TypeMenuItem(object o, EventHandler e)
            {
                this.Text = o.ToString();
                this.Type = o;
                this.Click += e;
            }
        }
        #endregion
    }
}