/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */

using System;
using System.Collections;
using System.IO;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Importers {
    /// <summary>
    /// Converts a 3DS data stream into its component material and mesh definitions.
    /// This class will also write them to files that can be easily imported into the
    /// RealmForge GDK Framework.
    /// </summary>
    public class Loader3DSModel {
        protected MaterialData3DS currentMaterialData;
        protected MeshData3DS currentMeshData;
        protected FileStream dataFile;
        protected DataReader3DS dataReader;
        protected string filePath;
        protected Hashtable materialDataStore;
        protected Hashtable meshDataStore;
        protected Stream stream;
        protected string version;

        public Loader3DSModel(String filePath) {
            if (filePath != null && File.Exists(filePath)) {
                this.filePath = filePath;
                this.stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader byteExtractor = new BinaryReader(this.stream);
                this.dataReader = new DataReader3DS(byteExtractor.ReadBytes((int) this.stream.Length));
                this.meshDataStore = new Hashtable();
                this.materialDataStore = new Hashtable();
                this.currentMeshData = null;
                this.currentMaterialData = null;
            } else {
                throw new FileNotFoundException();
            }
        }

        #region Public methods

        public void Load() {
            DataReader3DS dataSubSegment = null;
            //Validate that the 3DS file is good
            if ((this.dataReader != null) && (this.dataReader.Tag == 0x4D4D)) {
                dataSubSegment = this.dataReader.GetNextSubSegment();
            } else {
                throw new FileLoadException("3DS file is either corrupted or otherwise not recognizable.");
            }
            // Check to see what kind of data is contained in the current data subsegment
            while (dataSubSegment != null) {
                // Check the tag to see what sort of data is in this subsegment (or "chunk")
                switch (dataSubSegment.Tag) {
                    case 0x0002: // Subsegment contains 3DS version
                        ushort version3DS = dataSubSegment.GetUShort(); //This is the 3DS version
                        this.version = version3DS.ToString();
                        break;
                    case 0x3D3D: // Subsegment contains 3DS data
                        this.ParseData(dataSubSegment);
                        break;
                }
                dataSubSegment = this.dataReader.GetNextSubSegment();
            }
            //// Attempt to write mesh data to the .mesh.xml file
            //if (meshDataStore.Count > 0) {
            //    try {
            //        WriteMeshFile(meshFileName);
            //    } catch (Exception anyException) {
            //        System.Console.WriteLine("Error writing mesh XML file. See below for error...");
            //        System.Console.WriteLine(anyException.Message);                
            //    }
            //}
            //// Attempt to write material data to the .material file
            //if (materialDataStore.Count > 0) {
            //    try {
            //        WriteMaterialFile(materialFileName);
            //    } catch (Exception anyException) {
            //        System.Console.WriteLine("Error writing material file. See below for error...");
            //        System.Console.WriteLine(anyException.Message);                   
            //    }
            //}            
        }

        #endregion

        public string Version {
            get { return this.version; }
        }

        public MeshData3DS MeshData {
            get { return this.currentMeshData; }
        }

        public MaterialData3DS MaterialData {
            get { return this.currentMaterialData; }
        }

        public string FilePath {
            get { return this.filePath; }
        }

        #region Protected methods

        /// <summary>
        /// Parses the 3DS data stream into mesh and material information.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        protected void ParseData(DataReader3DS dataSegment) {
            DataReader3DS subSegment = dataSegment.GetNextSubSegment();
            while (subSegment != null) {
                switch (subSegment.Tag) {
                    case 0xafff: // Current subsegment holds material data
                        this.ParseMaterialData(subSegment);
                        break;
                    case 0x4000: // Current subsegment holds mesh data
                        this.ParseMeshData(subSegment);
                        break;
                }
                subSegment = dataSegment.GetNextSubSegment();
            }
        }

        #region Protected: Material-related sub-parsers

        /// <summary>
        /// Parses the material data from the current data segment.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        protected void ParseMaterialData(DataReader3DS dataSegment) {
            DataReader3DS subSegment = dataSegment.GetNextSubSegment();
            this.currentMaterialData = new MaterialData3DS();
            while (subSegment != null) {
                switch (subSegment.Tag) {
                    case 0xa000: // Subsegment holds material name
                        this.currentMaterialData.name = subSegment.GetString();
                        Console.WriteLine("Material is named: " + this.currentMaterialData.name);
                        break;
                    case 0xa010: // Subsegment holds ambient color
                        this.currentMaterialData.ambient = this.ParseColorData(subSegment.GetNextSubSegment());
                        break;
                    case 0xa020: // Subsegment holds diffuse color (this is iffy...)
                        this.currentMaterialData.diffuse = this.ParseColorData(subSegment.GetNextSubSegment());
                        break;
                    case 0xa200: // Subsegment holds texture map info
                        this.ParseTextureWeight(subSegment.GetNextSubSegment());
                        this.currentMaterialData.textureName = subSegment.GetNextSubSegment().GetString();
                        break;
                    default: // Ignore all other subsegment types
                        break;
                }
                subSegment = dataSegment.GetNextSubSegment();
            }
            // Store newly created material in data store and change name if another exists with its name
            while (this.materialDataStore.ContainsKey(this.currentMaterialData.name)) {
                this.currentMaterialData.name += "X";
            }
            this.materialDataStore.Add(this.currentMaterialData.name, this.currentMaterialData);
        }

        /// <summary>
        /// Parses color data from the current data segment.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        /// <returns>
        /// Returns the color that was parsed.
        /// </returns>
        protected RGBColor ParseColorData(DataReader3DS dataSegment) {
            RGBColor result = RGBColor.Black;
            switch (dataSegment.Tag) {
                case 0x0010: // Color is in float format
                    result.R = dataSegment.GetFloat();
                    result.G = dataSegment.GetFloat();
                    result.B = dataSegment.GetFloat();
                    break;
                case 0x0011: // Color is in byte format
                    result.R = dataSegment.GetByte() / 255.0f;
                    result.G = dataSegment.GetByte() / 255.0f;
                    result.B = dataSegment.GetByte() / 255.0f;
                    break;
                default: // If there are any other formats, then we ignore them
                    break;
            }
            return result;
        }

        /// <summary>
        /// Parses the texture weight from the current data segment.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        protected void ParseTextureWeight(DataReader3DS dataSegment) {
            switch (dataSegment.Tag) {
                case 0x0030: // Percentage is in short format
                    this.currentMaterialData.textureWeight = ((float) dataSegment.GetUShort()) / (float) 100.0;
                    break;
                case 0x0031: // Percentage is in float format
                    this.currentMaterialData.textureWeight = dataSegment.GetFloat();
                    break;
                default: // There should be no other formats, but if there are then we ignore
                    break;
            }
        }

        #endregion

        #region Protected: Mesh-related sub-parsers

        /// <summary>
        /// Parses the mesh data from the current data segment.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        protected void ParseMeshData(DataReader3DS dataSegment) {
            string name = dataSegment.GetString(); // mesh object name
            DataReader3DS subSegment = dataSegment.GetNextSubSegment(); // working subsegment
            this.currentMeshData = new MeshData3DS();
            this.currentMeshData.Name = name;
            while (subSegment != null) {
                switch (subSegment.Tag) {
                    case 0x4100: // Current subsegment contains the polygonal information
                        this.ParsePolygonalData(subSegment);
                        break;
                    default: // Ignore all other subsegment types
                        break;
                }
                subSegment = dataSegment.GetNextSubSegment();
            }
            while (this.meshDataStore.ContainsKey(this.currentMeshData.Name)) {
                this.currentMeshData.Name += "X";
            }
            this.meshDataStore.Add(this.currentMeshData.Name, this.currentMeshData);
        }

        /// <summary>
        /// Parses the polygonal data from the current data segment.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        protected void ParsePolygonalData(DataReader3DS dataSegment) {
            int i; // counter
            DataReader3DS subSegment = dataSegment.GetNextSubSegment(); // working data subsegment
            while (subSegment != null) {
                switch (subSegment.Tag) {
                    case 0x4110: // Subsegment contains vertex information
                        this.currentMeshData.Vertices = new Point3D[subSegment.GetUShort()];
                        for (i = 0; i < this.currentMeshData.Vertices.Length; i++) {
                            this.currentMeshData.Vertices[i].X = subSegment.GetFloat();
                            this.currentMeshData.Vertices[i].Y = subSegment.GetFloat();
                            this.currentMeshData.Vertices[i].Z = subSegment.GetFloat();
                        }
                        break;
                    case 0x4160: // Subsegment contains translation matrix info (ignore for now)
                        break;
                    case 0x4120: // Subsegment contains face information
                        this.ParseFaceData(subSegment);
                        break;
                    case 0x4140: // Subsegment contains texture mapping information
                        this.currentMeshData.TextureCoordinates = new Point2D[subSegment.GetUShort()];
                        //HACK: This is because the above array allocation doesn't automatically
                        //HACK: create each element.
                        for (i = 0; i < this.currentMeshData.TextureCoordinates.Length; i++) {
                            this.currentMeshData.TextureCoordinates[i] = new Point2D();
                        }
                        //HACK: End hack.
                        if (this.currentMeshData.TextureCoordinates.Length != this.currentMeshData.Vertices.Length) {
                            Console.WriteLine("WARNING: Possible errors in texture coordinate mapping!");
                        }
                        for (i = 0; i < this.currentMeshData.TextureCoordinates.Length; i++) {
                            this.currentMeshData.TextureCoordinates[i].X = subSegment.GetFloat();
                            this.currentMeshData.TextureCoordinates[i].Y = subSegment.GetFloat();
                        }
                        break;
                }
                subSegment = dataSegment.GetNextSubSegment();
            }
            // Also use face data to calculate vertex normals
            this.CalculateVertexNormals();
        }

        /// <summary>
        /// Parses the face-specific polygonal data from the current data segment.
        /// </summary>
        /// <param name="dataSegment">
        /// Contains the data to be parsed.
        /// </param>
        protected void ParseFaceData(DataReader3DS dataSegment) {
            int i; // counter
            DataReader3DS subSegment; // will be used to read other subsegments (do not initialize yet)
            this.currentMeshData.Faces = new MeshData3DS.FaceData3DS[dataSegment.GetUShort()];
            // Read face data
            for (i = 0; i < this.currentMeshData.Faces.Length; i++) {
                this.currentMeshData.Faces[i].Vertex1 = dataSegment.GetUShort();
                this.currentMeshData.Faces[i].Vertex2 = dataSegment.GetUShort();
                this.currentMeshData.Faces[i].Vertex3 = dataSegment.GetUShort();
                this.currentMeshData.Faces[i].Flags = dataSegment.GetUShort();
            }
            // Read other subsegments
            subSegment = dataSegment.GetNextSubSegment();
            while (subSegment != null) {
                switch (subSegment.Tag) {
                    case 0x4130: // Name of material used
                        this.currentMeshData.MaterialUsed = subSegment.GetString();
                        break;
                }
                subSegment = dataSegment.GetNextSubSegment();
            }
        }

        /// <summary>
        /// Calculates the normal vector at each vertex based on their configuration.
        /// </summary>
        protected void CalculateVertexNormals() {
            Vector3D[] faceNormals = new Vector3D[this.currentMeshData.Faces.Length];
            this.currentMeshData.Normals = new Vector3D[this.currentMeshData.Vertices.Length];
            MeshData3DS.FaceData3DS currentFace;
            Point3D faceV1;
            Point3D faceV2;
            Point3D faceV3;
            Vector3D faceEdge1;
            Vector3D faceEdge2;
            Point3D vertexVectorSum;
            int i;
            int j;
            ulong faceCount;
            // Calculate face normals
            for (i = 0; i < this.currentMeshData.Faces.Length; i++) {
                currentFace = this.currentMeshData.Faces[i];
                faceV1 = new Point3D(this.currentMeshData.Vertices[currentFace.Vertex1].X,
                                     this.currentMeshData.Vertices[currentFace.Vertex1].Y,
                                     this.currentMeshData.Vertices[currentFace.Vertex1].Z);
                faceV2 = new Point3D(this.currentMeshData.Vertices[currentFace.Vertex2].X,
                                     this.currentMeshData.Vertices[currentFace.Vertex2].Y,
                                     this.currentMeshData.Vertices[currentFace.Vertex2].Z);
                faceV3 = new Point3D(this.currentMeshData.Vertices[currentFace.Vertex3].X,
                                     this.currentMeshData.Vertices[currentFace.Vertex3].Y,
                                     this.currentMeshData.Vertices[currentFace.Vertex3].Z);
                faceEdge1 = faceV2 - faceV1;
                faceEdge2 = faceV3 - faceV1;
                faceNormals[i] = faceEdge2 ^ faceEdge1; //Note - This may need to change
                faceNormals[i].Normalize();
            }
            // Calculate vertex normals using face normal data (average of face normals)
            for (i = 0; i < this.currentMeshData.Vertices.Length; i++) {
                // Find faces attached to our current vertex
                faceCount = 0;
                vertexVectorSum = Point3D.Zero;
                for (j = 0; j < this.currentMeshData.Faces.Length; j++) {
                    currentFace = this.currentMeshData.Faces[j];
                    if ((i == currentFace.Vertex1) || (i == currentFace.Vertex2) || (i == currentFace.Vertex3)) {
                        faceCount++;
                        vertexVectorSum = vertexVectorSum + faceNormals[j];
                    }
                }
                // Use sum of face normals to calculate this vertex's normal
                this.currentMeshData.Normals[i] = vertexVectorSum.ToVector3D() / faceCount;
                this.currentMeshData.Normals[i].Normalize();
            }
        }

        #endregion

        #region Protected: Mesh and material file writers

        ///// <summary>
        ///// Attempts to write the mesh data to the .mesh.xml file
        ///// </summary>
        ///// <param name="fileName">
        ///// The name of the .mesh.xml file that will hold the mesh data.
        ///// </param>
        ///// <exception cref="System.Exception">
        ///// Thrown when file cannot be written to.
        ///// </exception>
        ///// <remarks>
        ///// This method uses .NET DOM (aka XmlDocument in C#) to generate the XML code.
        ///// </remarks>
        //protected void WriteMeshFile(string fileName) {
        //    // All the local variable stuff
        //    FileStream outputFile;
        //    StreamWriter outputStream;
        //    XmlDocument resultXML = new XmlDocument();
        //    XmlElement meshElem = resultXML.CreateElement("mesh");
        //    XmlElement submeshesElem = resultXML.CreateElement("submeshes");
        //    XmlElement currentSubmeshElem;
        //    XmlElement facesElem = resultXML.CreateElement("faces");
        //    XmlElement currentFaceElem;
        //    XmlElement geometryElem = resultXML.CreateElement("geometry");
        //    XmlElement vbufferElem = resultXML.CreateElement("vertexbuffer");
        //    XmlElement currentVertexElem;
        //    MeshData3DS.FaceData3DS face;
        //    Point3D vertex;
        //    Vector3D normal;
        //    Point2D textureCoord;
        //    int i;
        //    // Try to get the output XML file opened and ready to go.
        //    try {
        //        outputFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        //    } catch (Exception e) {
        //        Console.WriteLine("Error when writing .mesh.xml file!");
        //        throw e;
        //    }
        //    outputStream = new StreamWriter(outputFile);
        //    // These XML nodes should be present whether or not mesh data exists.
        //    resultXML.AppendChild(meshElem);
        //    meshElem.AppendChild(submeshesElem);
        //    // We only care about writing actual mesh information if a mesh actually exists.
        //    if (meshDataStore.Count > 0) {
        //        foreach (MeshData3DS currentMeshData in meshDataStore.Values) {
        //            //currentMeshData = (MeshData3DS)currentSubMesh;
        //            // Start with the mesh info
        //            currentSubmeshElem = resultXML.CreateElement("submesh");
        //            submeshesElem.AppendChild(currentSubmeshElem);
        //            currentSubmeshElem.Attributes.Append(resultXML.CreateAttribute("name"));
        //            currentSubmeshElem.Attributes["name"].Value = currentMeshData.Name;
        //            currentSubmeshElem.Attributes.Append(resultXML.CreateAttribute("material"));
        //            currentSubmeshElem.Attributes["material"].Value = currentMeshData.MaterialUsed;
        //            currentSubmeshElem.Attributes.Append(resultXML.CreateAttribute("usesharedvertices"));
        //            currentSubmeshElem.Attributes["usesharedvertices"].Value = "false";
        //            currentSubmeshElem.Attributes.Append(resultXML.CreateAttribute("use32bitindexes"));
        //            currentSubmeshElem.Attributes["use32bitindexes"].Value = "false";
        //            currentSubmeshElem.Attributes.Append(resultXML.CreateAttribute("operationtype"));
        //            currentSubmeshElem.Attributes["operationtype"].Value = "triangle_list";
        //            // Write face data
        //            currentSubmeshElem.AppendChild(facesElem);
        //            facesElem.Attributes.Append(resultXML.CreateAttribute("count"));
        //            facesElem.Attributes["count"].Value = currentMeshData.Faces.Length.ToString();
        //            for (i = 0; i < currentMeshData.Faces.Length; i++) {
        //                face = currentMeshData.Faces[i];
        //                currentFaceElem = resultXML.CreateElement("face");
        //                facesElem.AppendChild(currentFaceElem);
        //                currentFaceElem.Attributes.Append(resultXML.CreateAttribute("v1"));
        //                currentFaceElem.Attributes["v1"].Value = face.Vertex1.ToString();
        //                currentFaceElem.Attributes.Append(resultXML.CreateAttribute("v2"));
        //                currentFaceElem.Attributes["v2"].Value = face.Vertex2.ToString();
        //                currentFaceElem.Attributes.Append(resultXML.CreateAttribute("v3"));
        //                currentFaceElem.Attributes["v3"].Value = face.Vertex3.ToString();
        //            }
        //            // Write vertex data
        //            currentSubmeshElem.AppendChild(geometryElem);
        //            geometryElem.Attributes.Append(resultXML.CreateAttribute("vertexcount"));
        //            geometryElem.Attributes["vertexcount"].Value = currentMeshData.Vertices.Length.ToString();
        //            // ...now to write the actual vertex info...
        //            geometryElem.AppendChild(vbufferElem);
        //            vbufferElem.Attributes.Append(resultXML.CreateAttribute("positions"));
        //            vbufferElem.Attributes["positions"].Value = "true";
        //            for (i = 0; i < currentMeshData.Vertices.Length; i++) {
        //                vertex = currentMeshData.Vertices[i];
        //                currentVertexElem = resultXML.CreateElement("vertex");
        //                vbufferElem.AppendChild(currentVertexElem);
        //                currentVertexElem.AppendChild(resultXML.CreateElement("position"));
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("x"));
        //                currentVertexElem.FirstChild.Attributes["x"].Value = vertex.X.ToString();
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("y"));
        //                currentVertexElem.FirstChild.Attributes["y"].Value = vertex.Y.ToString();
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("z"));
        //                currentVertexElem.FirstChild.Attributes["z"].Value = vertex.Z.ToString();
        //            }
        //            // Write normal data
        //            vbufferElem = resultXML.CreateElement("vertexbuffer");
        //            geometryElem.AppendChild(vbufferElem);
        //            vbufferElem.Attributes.Append(resultXML.CreateAttribute("normals"));
        //            vbufferElem.Attributes["normals"].Value = "true";
        //            for (i = 0; i < currentMeshData.Vertices.Length; i++) {
        //                normal = currentMeshData.Normals[i];
        //                currentVertexElem = resultXML.CreateElement("vertex");
        //                vbufferElem.AppendChild(currentVertexElem);
        //                currentVertexElem.AppendChild(resultXML.CreateElement("normal"));
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("x"));
        //                currentVertexElem.FirstChild.Attributes["x"].Value = normal.X.ToString();
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("y"));
        //                currentVertexElem.FirstChild.Attributes["y"].Value = normal.Y.ToString();
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("z"));
        //                currentVertexElem.FirstChild.Attributes["z"].Value = normal.Z.ToString();
        //            }
        //            // Write texture coordinate data
        //            vbufferElem = resultXML.CreateElement("vertexbuffer");
        //            geometryElem.AppendChild(vbufferElem);
        //            vbufferElem.Attributes.Append(resultXML.CreateAttribute("texture_coord_dimensions_0"));
        //            vbufferElem.Attributes["texture_coord_dimensions_0"].Value = "2";
        //            vbufferElem.Attributes.Append(resultXML.CreateAttribute("texture_coords"));
        //            vbufferElem.Attributes["texture_coords"].Value = "1";
        //            for (i = 0; i < currentMeshData.TextureCoordinates.Length; i++) {
        //                textureCoord = currentMeshData.TextureCoordinates[i];
        //                currentVertexElem = resultXML.CreateElement("vertex");
        //                vbufferElem.AppendChild(currentVertexElem);
        //                currentVertexElem.AppendChild(resultXML.CreateElement("texcoord"));
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("u"));
        //                currentVertexElem.FirstChild.Attributes["u"].Value = textureCoord.X.ToString();
        //                currentVertexElem.FirstChild.Attributes.Append(resultXML.CreateAttribute("v"));
        //                currentVertexElem.FirstChild.Attributes["v"].Value = textureCoord.Y.ToString();
        //            }
        //        }
        //    }
        //    // Write XML to file
        //    outputStream.Write(resultXML.OuterXml);
        //    outputStream.Close();
        //}
        ///// <summary>
        ///// Attempts to write the material data to the .material file
        ///// </summary>
        ///// <param name="fileName">
        ///// The name of the .material file that will hold the material data.
        ///// </param>
        ///// <exception cref="System.Exception">
        ///// Thrown when file cannot be written to.
        ///// </exception>
        //protected void WriteMaterialFile(string fileName) {
        //    // All the local variable stuff
        //    FileStream outputFile;
        //    StreamWriter outputStream;
        //    string indent = "";
        //    // Try to get the output material file opened and ready to go.
        //    try {
        //        outputFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        //    } catch (Exception e) {
        //        Console.WriteLine("Error when writing .material file!");
        //        throw e;
        //    }
        //    outputStream = new StreamWriter(outputFile);
        //    // Only write data if material actually exists
        //    if (materialDataStore.Count > 0) {
        //        foreach (MaterialData3DS currentMaterialData in materialDataStore.Values) {
        //            // Write the bracket header info
        //            outputStream.WriteLine("material " + currentMaterialData.name);
        //            outputStream.WriteLine("{");
        //            indent += "\n";
        //            outputStream.WriteLine(indent + "technique");
        //            outputStream.WriteLine(indent + "{");
        //            indent += "\n";
        //            outputStream.WriteLine(indent + "pass");
        //            outputStream.WriteLine(indent + "{");
        //            indent += "\n";
        //            // Write ambient and diffuse data
        //            outputStream.WriteLine(indent + "ambient " +
        //            currentMaterialData.ambient.R.ToString() + " " +
        //            currentMaterialData.ambient.G.ToString() + " " +
        //            currentMaterialData.ambient.B.ToString());
        //            outputStream.WriteLine(indent + "diffuse " +
        //            currentMaterialData.diffuse.R.ToString() + " " +
        //            currentMaterialData.diffuse.G.ToString() + " " +
        //            currentMaterialData.diffuse.B.ToString());
        //            // Write material texture name data
        //            if (currentMaterialData.name != "NONE") {
        //                outputStream.WriteLine();
        //                outputStream.WriteLine(indent + "texture_unit");
        //                outputStream.WriteLine(indent + "{");
        //                indent += "\n";
        //                outputStream.WriteLine(indent + "texture " + currentMaterialData.textureName);
        //            }
        //            // Close out all the brackets
        //            do {
        //                outputStream.WriteLine(indent + "}");
        //                indent = indent.Substring(0, indent.Length - 1);
        //            } while (indent != "");
        //        }
        //    }
        //}

        #endregion

        #endregion

        #region Nested type: MaterialData3DS

        public class MaterialData3DS {
            public RGBColor ambient;
            public RGBColor diffuse;
            public string name;
            public string textureName;
            public float textureWeight;

            public MaterialData3DS() {
                this.ambient = RGBColor.Black;
                this.diffuse = RGBColor.Black;
                this.name = "";
                this.textureName = "";
                this.textureWeight = (float) 0.0;
            }
        }

        #endregion

        #region Nested type: MeshData3DS

        public class MeshData3DS {
            //PointersToVertex
            public FaceData3DS[] Faces;
            public string MaterialUsed;
            public string Name;
            public Vector3D[] Normals;
            public Point2D[] TextureCoordinates;
            public Point3D[] Vertices;

            public MeshData3DS() {
                this.MaterialUsed = "";
                this.Faces = new FaceData3DS[0];
                this.Normals = new Vector3D[0];
                this.TextureCoordinates = new Point2D[0];
                this.Vertices = new Point3D[0];
            }

            #region Nested type: FaceData3DS

            public struct FaceData3DS {
                public ushort Flags;
                public ushort Vertex1;
                public ushort Vertex2;
                public ushort Vertex3;
            }

            #endregion
        }

        #endregion
    }
}