using System;
using System.Xml;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components
{
    partial class Toolbox
    {
        internal const string Serializer_Category = "Category";
        private const string Serializer_Deletable = "deletable";
        internal const string Serializer_Enabled = "enabled";
        internal const string Serializer_General = "general";
        internal const string Serializer_Item = "Item";
        internal const string Serializer_Opened = "opened";
        internal const string Serializer_Text = "text";
        private const string Serializer_Tip = "tip";
        internal const string Serializer_Visible = "visible";
        internal const string Serializer_XmlConfig = "ToolboxConfig";
        internal const string Serializer_XmlHeader = "Toolbox";
        /// <summary>
        /// Saves the state of the <see cref="Toolbox"/> to the file.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        public virtual void Save(string filename)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement toolbox = doc.CreateElement(Serializer_XmlConfig);
            doc.AppendChild(toolbox);
            this.SaveXml(toolbox);
            doc.Save(filename);
        }
        /// <summary>
        /// Saves the state of the <see cref="Toolbox"/> to the <see cref="XmlElement"/> object.
        /// </summary>
        /// <param name="xml"></param>
        public virtual void SaveXml(XmlElement xml)
        {
            if(xml == null){
                throw new ArgumentNullException("xml");
            }
            XmlElement toolbox = xml.OwnerDocument.CreateElement(Serializer_XmlHeader);
            xml.AppendChild(toolbox);
            toolbox.SetAttribute(Serializer_General, this.CreateGeneralCategory.ToString());
            foreach(Tab tab in this.Categories){
                this.SerializeCategory(toolbox, tab);
            }
        }
        /// <summary>
        /// Loads the state of the <see cref="Toolbox"/> from the file.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        public virtual void Load(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement toolbox = (XmlElement)doc.SelectSingleNode(Serializer_XmlConfig);
            this.LoadXml(toolbox);
        }
        /// <summary>
        /// Loads the state of the <see cref="Toolbox"/> from the <see cref="XmlElement"/> object.
        /// </summary>
        /// <param name="xml"></param>
        public virtual void LoadXml(XmlElement xml)
        {
            if(xml == null){
                throw new ArgumentNullException("xml");
            }
            this.GeneralCategory = null;
            this.Categories.Clear();
            XmlElement toolbox = (XmlElement)xml.SelectSingleNode(Serializer_XmlHeader);
            bool createGeneral;
            if(!GetAttribute(toolbox, Serializer_General, out createGeneral)){
                createGeneral = true;
            }
            this.CreateGeneralCategory = createGeneral;
            if(this.CreateGeneralCategory){
                this.GeneralCategory = this.CreateNewTab(Resources.ToolboxTabGeneral);
                this.GeneralCategory.AllowDelete = false;
            }
            XmlNodeList categories = toolbox.SelectNodes(Serializer_Category);
            foreach(XmlElement category in categories){
                this.DeserializeCategory(category);
            }
        }
        /// <summary>
        /// Serializes a category <paramref name="tab"/> to the <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="toolbox">A <see cref="XmlElement"/> representing the toolbox.</param>
        /// <param name="tab">A <see cref="Tab"/> to serialize.</param>
        protected virtual void SerializeCategory(XmlElement toolbox, Tab tab)
        {
            XmlElement category = toolbox.OwnerDocument.CreateElement(Serializer_Category);
            toolbox.AppendChild(category);
            category.SetAttribute(Serializer_Text, tab.Text);
            category.SetAttribute(Serializer_Opened, tab.Opened.ToString());
            if(!tab.Visible){
                category.SetAttribute(Serializer_Visible, tab.Visible.ToString());
            }
            if(!tab.AllowDelete){
                category.SetAttribute(Serializer_Deletable, tab.AllowDelete.ToString());
            }
            if(tab == this.GeneralCategory){
                category.SetAttribute(Serializer_General, bool.TrueString);
            }
            foreach(Item item in tab.Items){
                this.SerializeItem(category, item);
            }
        }
        /// <summary>
        /// Serializes an <paramref name="item"/> to the <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="category">A <see cref="XmlElement"/> representing the category.</param>
        /// <param name="item">An <see cref="Item"/> to serialize.</param>
        protected virtual void SerializeItem(XmlElement category, Item item)
        {
            XmlElement tabItem = category.OwnerDocument.CreateElement(Serializer_Item);
            category.AppendChild(tabItem);
            tabItem.SetAttribute(Serializer_Text, item.Text);
            if(!item.Visible){
                tabItem.SetAttribute(Serializer_Visible, item.Visible.ToString());
            }
            if(!item.Enabled){
                tabItem.SetAttribute(Serializer_Enabled, item.Enabled.ToString());
            }
            if(!string.IsNullOrEmpty(item.Tooltip)){
                tabItem.SetAttribute(Serializer_Tip, item.Tooltip);
            }
        }
        /// <summary>
        /// Deserializes a <paramref name="category"/> from the xml and adds it to the <see cref="Toolbox"/>.
        /// </summary>
        /// <param name="category">A <see cref="XmlElement"/> object that represents a <see cref="Tab"/>.</param>
        protected virtual void DeserializeCategory(XmlElement category)
        {
            string stringValue;
            bool boolValue;
            if(GetAttribute(category, Serializer_Text, out stringValue)){
                Tab tab = null;
                if(GetAttribute(category, Serializer_General, out boolValue) && boolValue){
                    tab = this.GeneralCategory;
                } else{
                    tab = new Tab(stringValue);
                    if(GetAttribute(category, Serializer_Deletable, out boolValue)){
                        tab.AllowDelete = boolValue;
                    }
                    if(GetAttribute(category, Serializer_Visible, out boolValue)){
                        tab.Visible = boolValue;
                    }
                }
                if(GetAttribute(category, Serializer_Opened, out boolValue)){
                    tab.Opened = boolValue;
                }
                this.Categories.Add(tab);
                XmlNodeList items = category.SelectNodes(Serializer_Item);
                foreach(XmlElement itemNode in items){
                    this.DeserializeItem(itemNode, tab);
                }
            }
        }
        /// <summary>
        /// Deserializes an <paramref name="item"/> from the xml and adds it to the category <paramref name="tab"/>.
        /// </summary>
        /// <param name="item">A <see cref="XmlElement"/> object that represents an <see cref="Item"/>.</param>
        /// <param name="tab">A category <see cref="Tab"/> to which an item has to be added.</param>
        protected virtual void DeserializeItem(XmlElement item, Tab tab)
        {
            string stringValue;
            bool boolValue;
            if(GetAttribute(item, Serializer_Text, out stringValue)){
                Item tabItem = new Item(stringValue);
                tab.Items.Add(tabItem);
                if(GetAttribute(item, Serializer_Visible, out boolValue)){
                    tabItem.Visible = boolValue;
                }
                if(GetAttribute(item, Serializer_Enabled, out boolValue)){
                    tabItem.Enabled = boolValue;
                }
                if(GetAttribute(item, Serializer_Tip, out stringValue)){
                    tabItem.Tooltip = stringValue;
                }
            }
        }

        #region Static methods
        /// <summary>
        /// Gets a boolean attribute from the xml <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The <see cref="XmlElement"/> containing an <paramref name="attribute"/>.</param>
        /// <param name="attribute">The name of the attribute.</param>
        /// <param name="result">An output value.</param>
        /// <returns><b>true</b> if attribute exists and its value is boolean; otherwise <b>false</b>.</returns>
        protected static bool GetAttribute(XmlElement element, string attribute, out bool result)
        {
            if(element == null){
                throw new ArgumentNullException("element");
            }
            result = false;
            string resultString = element.GetAttribute(attribute);
            if(!string.IsNullOrEmpty(resultString)){
                try{
                    result = bool.Parse(resultString);
                    return true;
                } catch(FormatException){}
            }
            return false;
        }
        /// <summary>
        /// Gets a string attribute from the xml <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The <see cref="XmlElement"/> containing an <paramref name="attribute"/>.</param>
        /// <param name="attribute">The name of the attribute.</param>
        /// <param name="result">An output value.</param>
        /// <returns><b>true</b> if attribute exists; otherwise <b>false</b>.</returns>
        protected static bool GetAttribute(XmlElement element, string attribute, out string result)
        {
            result = element.GetAttribute(attribute);
            return !string.IsNullOrEmpty(result);
        }
        #endregion
    }
}