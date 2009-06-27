using System;
using System.Drawing.Design;
using System.Xml;

namespace TooboxUI.Components
{
    partial class HostToolbox
    {
        private const string Serializer_TypeName = "type";
        /// <summary>
        /// Saves the state of the <see cref="HostToolbox"/> to the <see cref="XmlElement"/> object.
        /// </summary>
        /// <param name="xml"></param>
        public override void SaveXml(XmlElement xml)
        {
            if(xml == null){
                throw new ArgumentNullException("xml");
            }
            XmlElement toolbox = xml.OwnerDocument.CreateElement(Serializer_XmlHeader);
            xml.AppendChild(toolbox);
            foreach(Tab tab in this.Categories){
                this.SerializeCategory(toolbox, tab);
            }
        }
        /// <summary>
        /// Loads the state of the <see cref="HostToolbox"/> from the <see cref="XmlElement"/> object.
        /// </summary>
        /// <param name="xml"></param>
        public override void LoadXml(XmlElement xml)
        {
            if(xml == null){
                throw new ArgumentNullException("xml");
            }
            this.GeneralCategory = null;
            this.Categories.Clear();
            XmlElement toolbox = (XmlElement)xml.SelectSingleNode(Serializer_XmlHeader);
            XmlNodeList categories = toolbox.SelectNodes(Serializer_Category);
            foreach(XmlElement category in categories){
                this.DeserializeCategory(category);
            }
        }
        /// <summary>
        /// Serializes a category <paramref name="tab"/> to the <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="toolbox">A <see cref="XmlElement"/> representing the toolbox.</param>
        /// <param name="tab">A <see cref="Toolbox.Tab"/> to serialize.</param>
        protected override void SerializeCategory(XmlElement toolbox, Tab tab)
        {
            XmlElement category = toolbox.OwnerDocument.CreateElement(Serializer_Category);
            toolbox.AppendChild(category);
            category.SetAttribute(Serializer_Text, tab.Text);
            category.SetAttribute(Serializer_Opened, tab.Opened.ToString());
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
        /// <param name="item">An <see cref="Toolbox.Item"/> to serialize.</param>
        protected override void SerializeItem(XmlElement category, Item item)
        {
            HostItem hostItem = item as HostItem;
            if(hostItem != null){
                XmlElement tabItem = category.OwnerDocument.CreateElement(Serializer_Item);
                category.AppendChild(tabItem);
                tabItem.SetAttribute(Serializer_Text, hostItem.Text);
                tabItem.SetAttribute(Serializer_TypeName, hostItem.ToolboxItem.TypeName);
            } else{
                base.SerializeItem(category, item);
            }
        }
        /// <summary>
        /// Deserializes a <paramref name="category"/> from the xml and adds it to the <see cref="Toolbox"/>.
        /// </summary>
        /// <param name="category">A <see cref="XmlElement"/> object that represents a <see cref="Toolbox.Tab"/>.</param>
        protected override void DeserializeCategory(XmlElement category)
        {
            string stringValue;
            bool boolValue;
            if(GetAttribute(category, Serializer_Text, out stringValue)){
                Tab tab = null;
                if(GetAttribute(category, Serializer_General, out boolValue) && boolValue){
                    tab = this.GeneralCategory;
                } else{
                    tab = new Tab(stringValue);
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
        /// <param name="item">A <see cref="XmlElement"/> object that represents an <see cref="Toolbox.Item"/>.</param>
        /// <param name="tab">A category <see cref="Toolbox.Tab"/> to which an item has to be added.</param>
        protected override void DeserializeItem(XmlElement item, Tab tab)
        {
            string stringValue;
            if(GetAttribute(item, Serializer_TypeName, out stringValue)){
                Type type = Type.GetType(stringValue);
                if(type != null){
                    HostItem hostItem = new HostItem(new ToolboxItem(type));
                    tab.Items.Add(hostItem);
                    if(GetAttribute(item, Serializer_Text, out stringValue)){
                        hostItem.Text = stringValue;
                    }
                }
            } else{
                base.DeserializeItem(item, tab);
            }
        }
    }
}