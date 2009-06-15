using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

// For serialization of an object to an XML Document file.
// For serialization of an object to an XML Binary file.
// For reading/writing data to an XML file.
// For accessing user isolated data.

namespace DrawEngine.Renderer.Util
{
    /// <summary>
    /// Serialization format types.
    /// </summary>
    public enum SerializedFormats
    {
        /// <summary>
        /// Binary serialization format.
        /// </summary>
        Binary,
        /// <summary>
        /// Document serialization format.
        /// </summary>
        Document
    }
    public delegate void SerializationHandler();
    /// <summary>
    /// Facade to XML serialization and deserialization of strongly typed objects to/from an XML file.
    /// 
    /// References: XML Serialization at http://samples.gotdotnet.com/:
    /// http://samples.gotdotnet.com/QuickStart/howto/default.aspx?url=/quickstart/howto/doc/xmlserialization/rwobjfromxml.aspx
    /// </summary>
    public static class ObjectXMLSerializer<T> //where T : class // Specify that T must be a class.
    {
        public static event SerializationHandler OnDeserialized;
        public static event SerializationHandler OnSerialized;

        #region Load methods
        /// <summary>
        /// Loads an object from an XML file in Document format.
        /// </summary>
        /// <example>
        /// <code>
        /// // Always create a new object prior to passing to ObjectXMLSerializer.Load method.
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(serializableObject, @"C:\XMLObjects.xml");
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be loaded from file.</param>
        /// <param name="path">Path of the file to load the object from.</param>
        /// <returns>Object loaded from an XML file in Document format.</returns>
        public static T Load(string path)
        {
            return LoadFromDocumentFormat(null, path, null);
        }
        /// <summary>
        /// Loads an object from an XML file using a specified serialized format.
        /// </summary>
        /// <example>
        /// <code>
        /// // Always create a new object prior to passing to ObjectXMLSerializer.Load method.
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(serializableObject, @"C:\XMLObjects.xml", SerializedFormats.Binary);
        /// </code>
        /// </example>		
        /// <param name="serializableObject">Serializable object to be loaded from file.</param>
        /// <param name="path">Path of the file to load the object from.</param>
        /// <param name="serializedFormat">XML serialized format used to load the object.</param>
        /// <returns>Object loaded from an XML file using the specified serialized format.</returns>
        public static T Load(string path, SerializedFormats serializedFormat)
        {
            T serializableObject = default(T);
            switch(serializedFormat){
                case SerializedFormats.Binary:
                    serializableObject = LoadFromBinaryFormat(path, null);
                    break;
                case SerializedFormats.Document:
                default:
                    serializableObject = LoadFromDocumentFormat(null, path, null);
                    break;
            }
            return serializableObject;
        }
        /// <summary>
        /// Loads an object from an XML file in Document format, supplying extra data types to enable deserialization of custom types within the object.
        /// </summary>
        /// <example>
        /// <code>
        /// // Always create a new object prior to passing to ObjectXMLSerializer.Load method.
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(serializableObject, @"C:\XMLObjects.xml", new Type[] { typeof(MyCustomType) });
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be loaded from file.</param>
        /// <param name="path">Path of the file to load the object from.</param>
        /// <param name="extraTypes">Extra data types to enable deserialization of custom types within the object.</param>
        /// <returns>Object loaded from an XML file in Document format.</returns>
        public static T Load(string path, Type[] extraTypes)
        {
            return LoadFromDocumentFormat(extraTypes, path, null);
        }
        /// <summary>
        /// Loads an object from an XML file in Document format, located in a specified isolated storage area.
        /// </summary>
        /// <example>
        /// <code>
        /// // Always create a new object prior to passing to ObjectXMLSerializer.Load method.
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(serializableObject, "XMLObjects.xml", IsolatedStorageFile.GetUserStoreForAssembly());
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be loaded from file.</param>
        /// <param name="fileName">Name of the file in the isolated storage area to load the object from.</param>
        /// <param name="isolatedStorageDirectory">Isolated storage area directory containing the XML file to load the object from.</param>
        /// <returns>Object loaded from an XML file in Document format located in a specified isolated storage area.</returns>
        public static T Load(string fileName, IsolatedStorageFile isolatedStorageDirectory)
        {
            return LoadFromDocumentFormat(null, fileName, isolatedStorageDirectory);
        }
        /// <summary>
        /// Loads an object from an XML file located in a specified isolated storage area, using a specified serialized format.
        /// </summary>
        /// <example>
        /// <code>
        /// // Always create a new object prior to passing to ObjectXMLSerializer.Load method.
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(serializableObject, "XMLObjects.xml", IsolatedStorageFile.GetUserStoreForAssembly(), SerializedFormats.Binary);
        /// </code>
        /// </example>		
        /// <param name="serializableObject">Serializable object to be loaded from file.</param>
        /// <param name="fileName">Name of the file in the isolated storage area to load the object from.</param>
        /// <param name="isolatedStorageDirectory">Isolated storage area directory containing the XML file to load the object from.</param>
        /// <param name="serializedFormat">XML serialized format used to load the object.</param>        
        /// <returns>Object loaded from an XML file located in a specified isolated storage area, using a specified serialized format.</returns>
        public static T Load(string fileName, IsolatedStorageFile isolatedStorageDirectory,
                             SerializedFormats serializedFormat)
        {
            T serializableObject = default(T);
            switch(serializedFormat){
                case SerializedFormats.Binary:
                    serializableObject = LoadFromBinaryFormat(fileName, isolatedStorageDirectory);
                    break;
                case SerializedFormats.Document:
                default:
                    serializableObject = LoadFromDocumentFormat(null, fileName, isolatedStorageDirectory);
                    break;
            }
            return serializableObject;
        }
        /// <summary>
        /// Loads an object from an XML file in Document format, located in a specified isolated storage area, and supplying extra data types to enable deserialization of custom types within the object.
        /// </summary>
        /// <example>
        /// <code>
        /// // Always create a new object prior to passing to ObjectXMLSerializer.Load method.
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(serializableObject, "XMLObjects.xml", IsolatedStorageFile.GetUserStoreForAssembly(), new Type[] { typeof(MyCustomType) });
        /// </code>
        /// </example>		
        /// <param name="serializableObject">Serializable object to be loaded from file.</param>
        /// <param name="fileName">Name of the file in the isolated storage area to load the object from.</param>
        /// <param name="isolatedStorageDirectory">Isolated storage area directory containing the XML file to load the object from.</param>
        /// <param name="extraTypes">Extra data types to enable deserialization of custom types within the object.</param>
        /// <returns>Object loaded from an XML file located in a specified isolated storage area, using a specified serialized format.</returns>
        public static T Load(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory,
                             Type[] extraTypes)
        {
            return LoadFromDocumentFormat(null, fileName, isolatedStorageDirectory);
        }
        #endregion

        #region Save methods
        /// <summary>
        /// Saves an object to an XML file in Document format.
        /// </summary>
        /// <example>
        /// <code>        
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// ObjectXMLSerializer&lt;SerializableObject&gt;.Save(serializableObject, @"C:\XMLObjects.xml");
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be saved to file.</param>
        /// <param name="path">Path of the file to save the object to.</param>
        public static void Save(T serializableObject, string path)
        {
            SaveToDocumentFormat(serializableObject, null, path, null);
        }
        /// <summary>
        /// Saves an object to an XML file using a specified serialized format.
        /// </summary>
        /// <example>
        /// <code>
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// ObjectXMLSerializer&lt;SerializableObject&gt;.Save(serializableObject, @"C:\XMLObjects.xml", SerializedFormats.Binary);
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be saved to file.</param>
        /// <param name="path">Path of the file to save the object to.</param>
        /// <param name="serializedFormat">XML serialized format used to save the object.</param>
        public static void Save(T serializableObject, string path, SerializedFormats serializedFormat)
        {
            switch(serializedFormat){
                case SerializedFormats.Binary:
                    SaveToBinaryFormat(serializableObject, path, null);
                    break;
                case SerializedFormats.Document:
                default:
                    SaveToDocumentFormat(serializableObject, null, path, null);
                    break;
            }
        }
        /// <summary>
        /// Saves an object to an XML file in Document format, supplying extra data types to enable serialization of custom types within the object.
        /// </summary>
        /// <example>
        /// <code>        
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// ObjectXMLSerializer&lt;SerializableObject&gt;.Save(serializableObject, @"C:\XMLObjects.xml", new Type[] { typeof(MyCustomType) });
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be saved to file.</param>
        /// <param name="path">Path of the file to save the object to.</param>
        /// <param name="extraTypes">Extra data types to enable serialization of custom types within the object.</param>
        public static void Save(T serializableObject, string path, Type[] extraTypes)
        {
            SaveToDocumentFormat(serializableObject, extraTypes, path, null);
        }
        /// <summary>
        /// Saves an object to an XML file in Document format, located in a specified isolated storage area.
        /// </summary>
        /// <example>
        /// <code>        
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// ObjectXMLSerializer&lt;SerializableObject&gt;.Save(serializableObject, "XMLObjects.xml", IsolatedStorageFile.GetUserStoreForAssembly());
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be saved to file.</param>
        /// <param name="fileName">Name of the file in the isolated storage area to save the object to.</param>
        /// <param name="isolatedStorageDirectory">Isolated storage area directory containing the XML file to save the object to.</param>
        public static void Save(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory)
        {
            SaveToDocumentFormat(serializableObject, null, fileName, isolatedStorageDirectory);
        }
        /// <summary>
        /// Saves an object to an XML file located in a specified isolated storage area, using a specified serialized format.
        /// </summary>
        /// <example>
        /// <code>        
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// ObjectXMLSerializer&lt;SerializableObject&gt;.Save(serializableObject, "XMLObjects.xml", IsolatedStorageFile.GetUserStoreForAssembly(), SerializedFormats.Binary);
        /// </code>
        /// </example>
        /// <param name="serializableObject">Serializable object to be saved to file.</param>
        /// <param name="fileName">Name of the file in the isolated storage area to save the object to.</param>
        /// <param name="isolatedStorageDirectory">Isolated storage area directory containing the XML file to save the object to.</param>
        /// <param name="serializedFormat">XML serialized format used to save the object.</param>        
        public static void Save(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory,
                                SerializedFormats serializedFormat)
        {
            switch(serializedFormat){
                case SerializedFormats.Binary:
                    SaveToBinaryFormat(serializableObject, fileName, isolatedStorageDirectory);
                    break;
                case SerializedFormats.Document:
                default:
                    SaveToDocumentFormat(serializableObject, null, fileName, isolatedStorageDirectory);
                    break;
            }
        }
        /// <summary>
        /// Saves an object to an XML file in Document format, located in a specified isolated storage area, and supplying extra data types to enable serialization of custom types within the object.
        /// </summary>
        /// <example>
        /// <code>
        /// SerializableObject serializableObject = new SerializableObject();
        /// 
        /// ObjectXMLSerializer&lt;SerializableObject&gt;.Save(serializableObject, "XMLObjects.xml", IsolatedStorageFile.GetUserStoreForAssembly(), new Type[] { typeof(MyCustomType) });
        /// </code>
        /// </example>		
        /// <param name="serializableObject">Serializable object to be saved to file.</param>
        /// <param name="fileName">Name of the file in the isolated storage area to save the object to.</param>
        /// <param name="isolatedStorageDirectory">Isolated storage area directory containing the XML file to save the object to.</param>
        /// <param name="extraTypes">Extra data types to enable serialization of custom types within the object.</param>
        public static void Save(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory,
                                Type[] extraTypes)
        {
            SaveToDocumentFormat(serializableObject, null, fileName, isolatedStorageDirectory);
        }
        #endregion

        #region Private
        private static FileStream CreateFileStream(IsolatedStorageFile isolatedStorageFolder, string path)
        {
            FileStream fileStream = null;
            if(isolatedStorageFolder == null){
                fileStream = new FileStream(path, FileMode.OpenOrCreate);
            } else{
                fileStream = new IsolatedStorageFileStream(path, FileMode.OpenOrCreate, isolatedStorageFolder);
            }
            return fileStream;
        }
        private static T LoadFromBinaryFormat(string path, IsolatedStorageFile isolatedStorageFolder)
        {
            T serializableObject = default(T);
            using(FileStream fileStream = CreateFileStream(isolatedStorageFolder, path)){
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //serializableObject = binaryFormatter.Deserialize(fileStream) as T;
                serializableObject = (T)binaryFormatter.Deserialize(fileStream);
            }
            if(OnDeserialized != null){
                OnDeserialized();    
            }
            return serializableObject;
        }
        private static T LoadFromDocumentFormat(Type[] extraTypes, string path,
                                                IsolatedStorageFile isolatedStorageFolder)
        {
            T serializableObject = default(T);
            using(TextReader textReader = CreateTextReader(isolatedStorageFolder, path)){
                XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
                
                //serializableObject = xmlSerializer.Deserialize(textReader) as T;
                serializableObject = (T)xmlSerializer.Deserialize(textReader);
            }
            if(OnDeserialized != null) {
                OnDeserialized();
            }
            return serializableObject;
        }
        private static TextReader CreateTextReader(IsolatedStorageFile isolatedStorageFolder, string path)
        {
            TextReader textReader = null;
            if(isolatedStorageFolder == null){
                textReader = new StreamReader(path);
            } else{
                textReader = new StreamReader(new IsolatedStorageFileStream(path, FileMode.Open, isolatedStorageFolder));
            }
            return textReader;
        }
        private static TextWriter CreateTextWriter(IsolatedStorageFile isolatedStorageFolder, string path)
        {
            TextWriter textWriter = null;
            if(isolatedStorageFolder == null){
                textWriter = new StreamWriter(path);
            } else{
                textWriter =
                        new StreamWriter(new IsolatedStorageFileStream(path, FileMode.OpenOrCreate,
                                                                       isolatedStorageFolder));
            }
            return textWriter;
        }
        private static XmlSerializer CreateXmlSerializer(Type[] extraTypes)
        {
            Type ObjectType = typeof(T);
            XmlSerializer xmlSerializer = null;
            //extraTypes = new Type[] { typeof(Color)};
            if(extraTypes != null){
                xmlSerializer = new XmlSerializer(ObjectType, extraTypes);
            } else{
                xmlSerializer = new XmlSerializer(ObjectType);
            }
            return xmlSerializer;
        }
        private static void SaveToDocumentFormat(T serializableObject, Type[] extraTypes, string path,
                                                 IsolatedStorageFile isolatedStorageFolder)
        {
            using(TextWriter textWriter = CreateTextWriter(isolatedStorageFolder, path)){
                XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
                xmlSerializer.Serialize(textWriter, serializableObject);
                if(OnSerialized != null) {
                    OnSerialized();
                }
            }
        }
        private static void SaveToBinaryFormat(T serializableObject, string path,
                                               IsolatedStorageFile isolatedStorageFolder)
        {
            using(FileStream fileStream = CreateFileStream(isolatedStorageFolder, path)){
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, serializableObject);
                if(OnSerialized != null) {
                    OnSerialized();
                }
            }
        }
        #endregion
    }
}