using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.Util.IO;

namespace DrawEngine.Renderer.Mesh.Importers {
    public enum ElementMesh {
        Vertex,
        VextexIndice,
        VertexNormal
    }

    public enum PlyFormat {
        ascii,
        binary_big_endian,
        binary_little_endian
    }

    public enum PropertyType {
        @char,
        uchar,
        @short,
        @ushort,
        @int,
        @uint,
        @float,
        @double,

        int8,
        uint8,
        int16,
        uint16,
        int32,
        uint32,
        float32,
        float64,
        list
    }

    public class NamedList<T> : List<T> where T : IName {
        public NamedList() : base() {}
        public NamedList(int capacity) : base(capacity) {}
        public NamedList(IEnumerable<T> collection) : base(collection) {}

        public T this[string name] {
            get { return (from element in this where element.Name == name select element).First(); }
        }
    }

    public interface IName {
        String Name { get; set; }
    }

    public class ElementDescription : IName {
        public ElementDescription(String name, int count) {
            this.Name = name;
            this.Count = count;
            this.Properties = new NamedList<PropertyDescription>();
        }

        public string Name { get; set; }
        public int Count { get; set; }
        public NamedList<PropertyDescription> Properties { get; set; }
    }

    public class PropertyDescription : IName {
        public PropertyDescription(PropertyType type, String name) {
            this.Type = type;
            this.Name = name;
        }

        public PropertyType Type { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Only if Type is a list
        /// </summary>
        public bool IsList {
            get { return Type == PropertyType.list; }
        }

        /// <summary>
        /// Only if Type is a list
        /// </summary>
        public PropertyType CountType { get; set; }

        /// <summary>
        /// Only if Type is a list
        /// </summary>
        public PropertyType TypeOfList { get; set; }
    }

    public class PlyObjectHeader {
        public PlyObjectHeader() {
            this.Comments = new List<string>();
            this.Elements = new NamedList<ElementDescription>();
        }

        public PlyFormat Format { get; set; }
        public float Vesion { get; set; }
        public List<String> Comments { get; set; }
        public NamedList<ElementDescription> Elements { get; set; }
    }

    public static class Extensions {
        public static string ReadLine(this Stream sr) {
            int byteCount;
            return ReadLine(sr, out byteCount);
        }

        public static string ReadLine(this Stream sr, out int byteCount) {
            String line = null;
            byteCount = 0;
            int c;
            while ((c = sr.ReadByte()) != -1) {
                byteCount++;
                if (c == 10 || c == 13) {
                    break;
                }
                line += ((char) c);
            }
            long posB = sr.Position;
            c = sr.ReadByte();
            if (c == 10 || c == 13) {
                byteCount++;
            } else {
                sr.Position = posB;
            }
            return line;
        }
    }

    public class PlyMeshImporter : AbstractMeshImporter {
        public override event MeshModel.ElementLoadEventHandler OnElementLoaded;

        public override List<String> RegisteredExtensions {
            get { return new List<String> {".ply"}; }
        }

        public override void Import(ref MeshModel mesh) {
            using (BufferedStream sr = new BufferedStream(File.OpenRead(mesh.FilePath))) {
                int byteCount;
                PlyObjectHeader headerObj = this.GetPlyObjectHeader(sr, out byteCount);

                sr.Position = byteCount;
                switch (headerObj.Format) {
                    case PlyFormat.ascii:
                        this.ParserASCII(headerObj, sr, ref mesh);
                        break;
                    case PlyFormat.binary_big_endian:
                        using (EndianessBinaryReader br = new EndianessBinaryReader(sr, Endianess.BigEndian)) {
                            this.ParserBinary(headerObj, br, ref mesh);
                        }
                        break;
                    case PlyFormat.binary_little_endian:
                        using (EndianessBinaryReader br = new EndianessBinaryReader(sr, Endianess.LittleEndian)) {
                            this.ParserBinary(headerObj, br, ref mesh);
                        }
                        break;
                    default:
                        throw new FormatException("Invalid File format!");
                        break;
                }
            }
        }

        private PlyObjectHeader GetPlyObjectHeader(Stream sr, out int byteCount) {
            PlyObjectHeader headerObj = new PlyObjectHeader();
            int lineByteCount;
            string line = Extensions.ReadLine(sr, out lineByteCount);
            byteCount = lineByteCount;
            if (line.ToLower() != "ply") {
                throw new ArgumentException("Invalid file format. PLY indentifier expected in header file.");
            }
            String[] lineSplits;
            while (line.ToLower() != "end_header") {
                line = Extensions.ReadLine(sr, out lineByteCount);
                byteCount += lineByteCount;
                lineSplits = line.Trim().Split(new char[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                switch (lineSplits[0]) {
                    case "format":
                        lineSplits = lineSplits[1].Split(' ');
                        headerObj.Format = (PlyFormat) Enum.Parse(typeof (PlyFormat), lineSplits[0]);
                        NumberFormatInfo nfi = new NumberFormatInfo();
                        nfi.NumberDecimalSeparator = ".";
                        nfi.NumberGroupSeparator = ",";
                        headerObj.Vesion = float.Parse(lineSplits[1], nfi);
                        break;
                    case "comment":
                        headerObj.Comments.Add(lineSplits[1]);
                        break;
                    case "element":
                        lineSplits = lineSplits[1].Split(' ');
                        ElementDescription element = new ElementDescription(lineSplits[0].ToLower(),
                                                                            Convert.ToInt32(lineSplits[1]));
                        headerObj.Elements.Add(element);
                        break;
                    case "property":
                        lineSplits = lineSplits[1].Split(' ');
                        PropertyType pType = (PropertyType) Enum.Parse(typeof (PropertyType), lineSplits[0]);
                        String pName = lineSplits[lineSplits.Length - 1];
                        PropertyDescription property = new PropertyDescription(pType, pName);
                        if (pType == PropertyType.list) {
                            property.CountType = (PropertyType) Enum.Parse(typeof (PropertyType), lineSplits[1]);
                            property.TypeOfList = (PropertyType) Enum.Parse(typeof (PropertyType), lineSplits[2]);
                        }
                        headerObj.Elements[headerObj.Elements.Count - 1].Properties.Add(property);
                        break;
                }
            }
            return headerObj;
        }

        private void ParserASCII(PlyObjectHeader header, Stream sr, ref MeshModel mesh) {
            MeshVertex[] vertices = new MeshVertex[header.Elements["vertex"].Count];
            int[] indices = new int[header.Elements["face"].Count * 3];
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            //Initialize bound box calculation
            String[] str;
            //for number of vertices readed in header do...
            BoundBox boundBox = new BoundBox();
            for (int i = 0; i < vertices.Length; i++) {
                str = sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);


                vertices[i].Position = new Point3D(float.Parse(str[0], nfi), float.Parse(str[1], nfi),
                                                   float.Parse(str[2], nfi));
                //Adjusting BoundBox...
                boundBox.Include(vertices[i].Position);
                //Reporting progress
                int percent = (int) (((float) i / vertices.Length) * 100.0f);
                if ((percent % 20) == 0) {
                    this.OnElementLoaded(percent, ElementMesh.Vertex);
                }
            }

            //MeshModel mesh = new MeshModel(header.Elements["face"].Count);
            mesh.Triangles = new MeshTriangle[header.Elements["face"].Count];
            mesh.BoundBox = boundBox;

            for (int i = 0, ptr = 0; i < mesh.Triangles.Length; i++) {
                str = sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                indices[ptr++] = Int32.Parse(str[1], nfi);
                mesh.Triangles[i].Vertex1 = vertices[indices[ptr - 1]];
                indices[ptr++] = Int32.Parse(str[2], nfi);
                mesh.Triangles[i].Vertex2 = vertices[indices[ptr - 1]];
                indices[ptr++] = Int32.Parse(str[3], nfi);
                mesh.Triangles[i].Vertex3 = vertices[indices[ptr - 1]];

                int percent = (int) (((float) i / indices.Length) * 100.0f);
                if ((percent % 20) == 0) {
                    this.OnElementLoaded(percent, ElementMesh.VextexIndice);
                }
            }
            int verticesCount = vertices.Length;
            vertices = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ProcessNormalsPerVertex(indices, ref mesh, verticesCount);
            indices = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private MeshModel ParserBinary(PlyObjectHeader header, BinaryReader ebr, ref MeshModel mesh) {
            //MeshModel mesh = new MeshModel(header.Elements["face"].Count * 3, header.Elements["vertex"].Count);
            //for (int i = 0; i < header.Elements.Count; i++)
            //{
            //    ElementDescription element = header.Elements[i];
            //    for (int e = 0; e < element.Count; e++)
            //    {
            //        for (int p = 0; p < element.Properties.Count; p++)
            //        {
            //            PropertyDescription property = element.Properties[p];

            //            if (property.IsList)
            //            {
            //                int listCount = 0;
            //                if (property.CountType == PropertyType.uchar || property.CountType == PropertyType.uint8)
            //                {
            //                    listCount = ebr.ReadByte();
            //                }
            //                else
            //                {
            //                    throw new ArgumentException("The type expected for List Count is 'uchar'. Type in file: '" + property.CountType + "'");
            //                }

            //                if (property.TypeOfList == PropertyType.@int || property.TypeOfList == PropertyType.int32)
            //                {
            //                    if (element.Name == "face")
            //                    {
            //                        for (int j = 0; j < listCount; j += 3)
            //                        {
            //                            mesh.Indices[e] = ebr.ReadInt32();
            //                            mesh.Indices[e + 1] = ebr.ReadInt32();
            //                            mesh.Indices[e + 2] = ebr.ReadInt32();

            //                            int percent = (int)(((float)e / mesh.Indices.Length) * 100.0f);
            //                            if ((percent % 20) == 0)
            //                            {
            //                                this.OnElementLoaded(percent, ElementMesh.VextexIndice);
            //                            }
            //                        }
            //                    }
            //                    else if (element.Name == "vertex")
            //                    {
            //                        #region Ignore other properties for vertex
            //                        for (int j = 0; j < listCount; j += 3)
            //                        {
            //                            ebr.ReadInt32();
            //                            ebr.ReadInt32();
            //                            ebr.ReadInt32();
            //                        }
            //                        #endregion
            //                    }
            //                }
            //                else
            //                {
            //                    throw new ArgumentException("The type expected for List elements is 'int'. Type in file: '" + property.TypeOfList + "'");
            //                }
            //            }
            //            else
            //            {
            //                if (element.Name == "face")
            //                {
            //                    #region Ignore other properties for face
            //                    switch (property.Type)
            //                    {
            //                        case PropertyType.@char:
            //                        case PropertyType.int8:
            //                            ebr.ReadSByte();
            //                            break;
            //                        case PropertyType.uchar:
            //                        case PropertyType.uint8:
            //                            ebr.ReadByte();
            //                            break;
            //                        case PropertyType.@short:
            //                        case PropertyType.int16:
            //                            ebr.ReadInt16();
            //                            break;
            //                        case PropertyType.@ushort:
            //                        case PropertyType.uint16:
            //                            ebr.ReadUInt16();
            //                            break;
            //                        case PropertyType.@int:
            //                        case PropertyType.int32:
            //                            ebr.ReadInt32();
            //                            break;
            //                        case PropertyType.@uint:
            //                        case PropertyType.uint32:
            //                            ebr.ReadUInt32();
            //                            break;
            //                        case PropertyType.@float:
            //                        case PropertyType.float32:
            //                            ebr.ReadSingle();
            //                            break;
            //                        case PropertyType.@double:
            //                        case PropertyType.float64:
            //                            ebr.ReadDouble();
            //                            break;
            //                        default:
            //                            break;
            //                    }
            //                    #endregion
            //                }
            //                else if (element.Name == "vertex")
            //                {

            //                    switch (property.Type)
            //                    {
            //                        case PropertyType.@char:
            //                        case PropertyType.int8:
            //                            ebr.ReadSByte();
            //                            break;
            //                        case PropertyType.uchar:
            //                        case PropertyType.uint8:
            //                            ebr.ReadByte();
            //                            break;
            //                        case PropertyType.@short:
            //                        case PropertyType.int16:
            //                            ebr.ReadInt16();
            //                            break;
            //                        case PropertyType.@ushort:
            //                        case PropertyType.uint16:
            //                            ebr.ReadUInt16();
            //                            break;
            //                        case PropertyType.@int:
            //                        case PropertyType.int32:
            //                            ebr.ReadInt32();
            //                            break;
            //                        case PropertyType.@uint:
            //                        case PropertyType.uint32:
            //                            ebr.ReadUInt32();
            //                            break;
            //                        case PropertyType.@float:
            //                        case PropertyType.float32:
            //                            mesh.Vertices[e].Position.X = ebr.ReadSingle();
            //                            mesh.Vertices[e].Position.Y = ebr.ReadSingle();
            //                            mesh.Vertices[e].Position.Z = ebr.ReadSingle();
            //                            p += 2;
            //                            //Adjusting BoundBox...
            //                            mesh.BoundBox.Include(mesh.Vertices[e].Position);
            //                            //Reporting progress
            //                            int percent = (int)(((float)e / mesh.Vertices.Length) * 100.0f);
            //                            if ((percent % 20) == 0)
            //                            {
            //                                this.OnElementLoaded(percent, ElementMesh.Vertex);
            //                            }
            //                            break;
            //                        case PropertyType.@double:
            //                        case PropertyType.float64:
            //                            ebr.ReadDouble();
            //                            break;
            //                        default:
            //                            break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //GC.Collect();
            //ForceGenerateNormals(mesh);
            //return mesh;
            return null;
        }

        protected void ProcessNormalsPerVertex(int[] indices, ref MeshModel mesh, int verticesCount) {
            Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
            int ptr = 0;
            for (int i = 0; i < mesh.Triangles.Length; i++) {
                normalsPerVertex[indices[ptr++]] += mesh.Triangles[i].Normal;
                normalsPerVertex[indices[ptr++]] += mesh.Triangles[i].Normal;
                normalsPerVertex[indices[ptr++]] += mesh.Triangles[i].Normal;

                int percent = (int) (((float) i / mesh.Triangles.Length) * 100.0f);


                if ((percent % 20) == 0) {
                    this.OnElementLoaded(percent, ElementMesh.VertexNormal);
                }
            }
            ptr = 0;
            for (int i = 0; i < mesh.Triangles.Length; i++) {
                mesh.Triangles[i].Vertex1.Normal = normalsPerVertex[indices[ptr++]];
                mesh.Triangles[i].Vertex1.Normal.Normalize();

                mesh.Triangles[i].Vertex2.Normal = normalsPerVertex[indices[ptr++]];
                mesh.Triangles[i].Vertex2.Normal.Normalize();

                mesh.Triangles[i].Vertex3.Normal = normalsPerVertex[indices[ptr++]];
                mesh.Triangles[i].Vertex3.Normal.Normalize();

                int percent = (int) (((float) i / mesh.Triangles.Length) * 100.0f);
                if ((percent % 20) == 0) {
                    this.OnElementLoaded(percent / 2 + 50, ElementMesh.VertexNormal);
                }
            }
            normalsPerVertex = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// SLOW but smoother normals (across same-positioned vertices)
        /// </summary>
        //public static void ForceGenerateNormals(MeshModel mesh)
        //{
        //    int numFaces = mesh.Indices.Length / 3;

        //    // make face normals
        //    Vector3D[] faceNormals = new Vector3D[numFaces];
        //    for (int i = 0; i < mesh.Indices.Length; i += 3)
        //    {
        //        faceNormals[i / 3] = Vector3D.Normal(mesh.Vertices[mesh.Indices[i]].Position,
        //                                         mesh.Vertices[mesh.Indices[i + 1]].Position,
        //                                         mesh.Vertices[mesh.Indices[i + 2]].Position);

        //    }

        //    for (int i = 0; i < mesh.Vertices.Length; i++)
        //    {
        //        Vector3D nrm = Vector3D.Zero;
        //        int nrmCount = 0;
        //        Point3D pos = mesh.Vertices[i].Position;

        //        for (int o = 0; o < numFaces; o++)
        //        {
        //            // examine this face to see if it borders this vertex
        //            Point3D one = mesh.Vertices[mesh.Indices[o * 3]].Position;
        //            if (one.X == pos.X && one.Y == pos.Y && one.Z == pos.Z)
        //            {
        //                nrm += faceNormals[o];
        //                nrmCount++;
        //            }

        //            one = mesh.Vertices[mesh.Indices[o * 3 + 1]].Position;
        //            if (one.X == pos.X && one.Y == pos.Y && one.Z == pos.Z)
        //            {
        //                nrm += faceNormals[o];
        //                nrmCount++;
        //            }

        //            one = mesh.Vertices[mesh.Indices[o * 3 + 2]].Position;
        //            if (one.X == pos.X && one.Y == pos.Y && one.Z == pos.Z)
        //            {
        //                nrm += faceNormals[o];
        //                nrmCount++;
        //            }
        //        }

        //        nrm *= (1.0f / nrmCount);
        //        nrm.Normalize();
        //        mesh.Vertices[i].Normal = nrm;

        //    }
        //}

        //public void GenerateNormals()
        //{
        //    // make vertex => face mapping
        //    Vector3[] faceNormals = new Vector3[Indices.Length / 3];
        //    List<int>[] facesUsed = new List<int>[Vertices.Length];
        //    for (int i = 0; i < Indices.Length; i += 3)
        //    {
        //        Vector3 one, two, three;

        //        int vidx = Indices[i];
        //        one = Vertices[vidx].Position;
        //        if (facesUsed[vidx] == null)
        //            facesUsed[vidx] = new List<int>();
        //        facesUsed[vidx].Add(i);

        //        vidx = Indices[i + 1];
        //        two = Vertices[vidx].Position;
        //        if (facesUsed[vidx] == null)
        //            facesUsed[vidx] = new List<int>();
        //        facesUsed[vidx].Add(i);

        //        vidx = Indices[i + 2];
        //        three = Vertices[vidx].Position;
        //        if (facesUsed[vidx] == null)
        //            facesUsed[vidx] = new List<int>();
        //        facesUsed[vidx].Add(i);

        //        faceNormals[i / 3] = Vector3.NormalFrom(ref three, ref two, ref one);
        //    }

        //    for (int i = 0; i < Vertices.Length; i++)
        //    {
        //        if (facesUsed[i] == null)
        //            continue; // weird! no face uses this vertex?
        //        Vector3 nrm = Vector3.Zero;

        //        foreach (int face in facesUsed[i])
        //            nrm += faceNormals[face / 3];
        //        nrm.Normalize();

        //        MeshVertex v = Vertices[i];
        //        v.Normal = nrm;
        //        Vertices[i] = v;
        //    }
        //}
    }
}