using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Importers
{
    public enum ElementMesh
    {
        Vertex,
        Triangle,
        VertexNormal
    }

    public class LoaderPlyModel : AbstractLoaderModel
    {
        private string path;
        public LoaderPlyModel(string path)
        {
            this.path = path;
        }
        public override event ElementLoadEventHandler OnElementLoaded;
        public override Triangle[] Load()
        {
            if(!this.Validate()){
                throw new Exception("Invalid file type!");
            }
            this.ParserPlyModel();
            return this.triangles;
        }
        private bool Validate()
        {
            using(StreamReader sr = new StreamReader(this.path)){
                string hLine = "";
                while(!sr.EndOfStream){
                    hLine = sr.ReadLine().Trim();
                    while(hLine == string.Empty){
                        hLine = sr.ReadLine().Trim();
                    }
                    if(hLine.ToLower() == "ply"){
                        return true;
                    } else{
                        return false;
                    }
                }
                return false;
            }
        }
        private void ParserPlyModel()
        {
            List<string[]> header = new List<string[]>();
            Point3D[] vertices = null;
            PointerToVertex[] pointersToVertex = null;
            using(StreamReader sr = new StreamReader(this.path)){
                string hLine = "";
                while(!sr.EndOfStream){
                    hLine = sr.ReadLine().Trim();
                    if(hLine.ToLower() == "end_header"){
                        break;
                    }
                    header.Add(hLine.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            string format = "";
            foreach(string[] line in header){
                switch(line[0].ToLower()){
                    case "format":
                        format = line[1].ToLower();
                        break;
                    case "comment":
                        //TODO Usar esse comentario pra alguma coisa
                        break;
                    case "element":
                        switch(line[1].ToLower()){
                            case "vertex":
                                vertices = new Point3D[Convert.ToInt32(line[2])];
                                break;
                            case "face":
                                int count = Convert.ToInt32(line[2]);
                                this.triangles = new Triangle[count];
                                pointersToVertex = new PointerToVertex[count];
                                break;
                        }
                        break;
                }
            }
            switch(format){
                case "ascii":
                    this.ParserASCII(vertices, pointersToVertex);
                    break;
                case "binary_big_endian":
                case "binary_little_endian":
                    //this.ParserBinary();
                    throw new FormatException("Invalid File! Only 'ASCII' formats are supported!");
                    break;
                default:
                    throw new FormatException("Invalid File! Only 'ASCII' formats are supported!");
            }
        }
        private void ParserASCII(Point3D[] vertices, PointerToVertex[] pointersToVertex)
        {
            //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            //BufferedStream bs = new BufferedStream(fs);
            using(StreamReader sr = new StreamReader(this.path)){
                //int headerLength = header.Count + 1;
                while(sr.ReadLine().ToLower() != "end_header"){} //Pass header...
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberGroupSeparator = ",";
                //Initialize bound box calculation
                Point3D pmin, pmax;
                pmin = pmax = Point3D.Zero;
                this.boundBox = new BoundBox();
                String[] str;
                //for number of vertices readed in header do...
                for(int i = 0; i < vertices.Length; i++){
                    str = sr.ReadLine().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    vertices[i] = new Point3D(float.Parse(str[0], nfi), float.Parse(str[1], nfi),
                                              float.Parse(str[2], nfi));
                    //Adjusting BoundBox...
                    this.boundBox.Include(vertices[i]);
                    //Reporting progress
                    int percent = (int)(((float)i / vertices.Length) * 100.0f);
                    if((percent % 20) == 0){
                        this.OnElementLoaded(percent, ElementMesh.Vertex);
                    }
                }
                for(int i = 0; i < this.triangles.Length; i++){
                    str = sr.ReadLine().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    pointersToVertex[i] = new PointerToVertex(Int32.Parse(str[1], nfi), Int32.Parse(str[2], nfi),
                                                              Int32.Parse(str[3], nfi));
                    this.triangles[i] = new Triangle(vertices[pointersToVertex[i].Vertex1],
                                                     vertices[pointersToVertex[i].Vertex2],
                                                     vertices[pointersToVertex[i].Vertex3]);
                    int percent = (int)(((float)i / this.triangles.Length) * 100.0f);
                    if((percent % 20) == 0){
                        this.OnElementLoaded(percent, ElementMesh.Triangle);
                    }
                }
                this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);
                vertices = null;
                pointersToVertex = null;
                GC.Collect();
            }
        }
        protected void ProcessNormalsPerVertex(PointerToVertex[] pointersToVertex, int verticesCount)
        {
            Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
            for(int i = 0; i < this.triangles.Length; i++){
                normalsPerVertex[pointersToVertex[i].Vertex1] += this.triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex2] += this.triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex3] += this.triangles[i].Normal;
                int percent = (int)(((float)i / this.triangles.Length) * 100.0f);
                if((percent % 20) == 0){
                    this.OnElementLoaded(percent / 2, ElementMesh.VertexNormal);
                }
            }
            for(int i = 0; i < this.triangles.Length; i++){
                this.triangles[i].NormalOnVertex1 = normalsPerVertex[pointersToVertex[i].Vertex1];
                this.triangles[i].NormalOnVertex1.Normalize();
                this.triangles[i].NormalOnVertex2 = normalsPerVertex[pointersToVertex[i].Vertex2];
                this.triangles[i].NormalOnVertex2.Normalize();
                this.triangles[i].NormalOnVertex3 = normalsPerVertex[pointersToVertex[i].Vertex3];
                this.triangles[i].NormalOnVertex3.Normalize();
                int percent = (int)(((float)i / this.triangles.Length) * 100.0f);
                if((percent % 20) == 0){
                    this.OnElementLoaded(percent / 2 + 50, ElementMesh.VertexNormal);
                }
            }
            normalsPerVertex = null;
        }

        #region ParseBinary
        //private void ParserBinary()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    using (BinaryReader br = new BinaryReader(new FileStream(this.path, FileMode.Open, FileAccess.Read)))
        //    {
        //        while (br.ReadChar() != '\f')
        //        {
        //        }
        //        //br.ReadChar();
        //        //br.ReadChar();
        //        //int i = 0;
        //        //while (i < 49132)
        //        //{
        //        //    byte[] bVertex = br.ReadBytes(4);
        //        //    Array.Reverse(bVertex);
        //        //    float vertex = BitConverter.ToSingle(bVertex, 0);
        //        //    sb.Append(vertex);
        //        //    sb.Append(" ");
        //        //    if ((i++ % 3) == 0)
        //        //    {
        //        //        sb.AppendLine();
        //        //    }
        //        //}
        //        #region MyRegion
        //        byte[] bVertex = br.ReadBytes(4);
        //        Array.Reverse(bVertex);
        //        this.vertices[0].X = BitConverter.ToSingle(bVertex, 0);
        //        bVertex = br.ReadBytes(4);
        //        Array.Reverse(bVertex);
        //        this.vertices[0].Y = BitConverter.ToSingle(bVertex, 0);
        //        bVertex = br.ReadBytes(4);
        //        Array.Reverse(bVertex);
        //        this.vertices[0].Z = BitConverter.ToSingle(bVertex, 0);
        //        Point3D pmin, pmax;
        //        pmin = pmax = this.vertices[0];
        //        this.boundBox = new BoundBox(pmin, pmax);
        //        for (int i = 1; i < this.vertices.Length; i++)
        //        {
        //            bVertex = br.ReadBytes(4);
        //            Array.Reverse(bVertex);
        //            this.vertices[i].X = BitConverter.ToSingle(bVertex, 0);
        //            bVertex = br.ReadBytes(4);
        //            Array.Reverse(bVertex);
        //            this.vertices[i].Y = BitConverter.ToSingle(bVertex, 0);
        //            bVertex = br.ReadBytes(4);
        //            Array.Reverse(bVertex);
        //            this.vertices[i].Z = BitConverter.ToSingle(bVertex, 0);
        //            pmin = this.boundBox.PMin;
        //            pmin.X = this.vertices[i].X < pmin.X ? this.vertices[i].X : pmin.X;
        //            pmin.Y = this.vertices[i].Y < pmin.Y ? this.vertices[i].Y : pmin.Y;
        //            pmin.Z = this.vertices[i].Z < pmin.Z ? this.vertices[i].Z : pmin.Z;
        //            pmax = this.boundBox.PMax;
        //            pmax.X = this.vertices[i].X > pmax.X ? this.vertices[i].X : pmax.X;
        //            pmax.Y = this.vertices[i].Y > pmax.Y ? this.vertices[i].Y : pmax.Y;
        //            pmax.Z = this.vertices[i].Z > pmax.Z ? this.vertices[i].Z : pmax.Z;
        //            this.boundBox.PMin = pmin;
        //            this.boundBox.PMax = pmax;
        //            int percent = (int)(((float)i / this.vertices.Length) * 100.0f);
        //            if ((percent % 10) == 0)
        //            {
        //                OnElementLoaded(percent, ElementMesh.Vertex);
        //            }
        //        }
        //        this.triangles = new Triangle[98260];
        //        this.pointersToVertex = new PointerToVertex[98260];
        //        for (int i = 0; i < 98260; i++)
        //        {
        //            bVertex = br.ReadBytes(4); //Lendo numero de lados do poligono
        //            //Array.Reverse(bVertex);
        //            bVertex = br.ReadBytes(4);
        //            Array.Reverse(bVertex);
        //            this.pointersToVertex[i].Vertex1 = BitConverter.ToInt32(bVertex, 0);
        //            bVertex = br.ReadBytes(4);
        //            Array.Reverse(bVertex);
        //            this.pointersToVertex[i].Vertex2 = BitConverter.ToInt32(bVertex, 0);
        //            bVertex = br.ReadBytes(4);
        //            Array.Reverse(bVertex);
        //            this.pointersToVertex[i].Vertex3 = BitConverter.ToInt32(bVertex, 0);
        //            //this.pointersToVertex[i] =
        //            //    new PointerToVertex(Convert.ToInt32(str[1]), Convert.ToInt32(str[2]), Convert.ToInt32(str[3]));
        //            this.Triangles[i] = new Triangle(this.vertices[this.pointersToVertex[i].Vertex1],
        //                                             this.vertices[this.pointersToVertex[i].Vertex2],
        //                                             this.vertices[this.pointersToVertex[i].Vertex3]);
        //            int percent = (int)(((float)i / this.Triangles.Length) * 100.0f);
        //            if ((percent % 10) == 0)
        //            {
        //                OnElementLoaded(percent, ElementMesh.Triangle);
        //            }
        //            //triangles[i].Material = base.material;
        //        }
        //        this.ProcessNormalsPerVertex();
        //        #endregion
        //    }
        //}
        #endregion
    }
}