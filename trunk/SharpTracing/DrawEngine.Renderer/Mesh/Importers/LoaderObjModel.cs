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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Importers
{
    //[Serializable]public class LoaderObjModel : AbstractLoaderModel
    //{
    //    private string path;
    //    public override event ElementLoadEventHandler OnElementLoaded;
    //    public LoaderObjModel(string path, string name)
    //    {
    //        this.path = path;
    //        this.CountElements();
    //    }
    //    public override Triangle[] Load()
    //    {
    //        if (!Validate())
    //        {
    //            throw new Exception("Invalid file type!");
    //        }
    //        this.ParserObjModel();
    //        return this.Triangles;
    //    }
    //    private bool Validate()
    //    {
    //        return Path.GetExtension(this.path).ToLower() == ".obj";
    //    }
    //    private Dictionary<string, int> elementCount;
    //    private int CountElements()
    //    {
    //        elementCount = new Dictionary<string, int>();
    //        int count = 0;
    //        using (StreamReader sr = new StreamReader(path))
    //        {
    //            string[] line;
    //            while (!sr.EndOfStream)
    //            {
    //                line = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //                if (line.Length != 0)
    //                {
    //                    if (!elementCount.ContainsKey(line[0]))
    //                    {
    //                        elementCount.Add(line[0], 1);
    //                    }
    //                    else
    //                    {
    //                        elementCount[line[0]] = elementCount[line[0]] + 1;
    //                    }
    //                }
    //            }
    //            sr.Close();
    //            sr.Dispose();
    //        }
    //        return count;
    //    }
    //    private void ParserObjModel()
    //    {
    //        List<Point3D> vertices = new List<Point3D>(elementCount["v"]);
    //        List<Point3D> vertexNormals = new List<Point3D>();
    //        List<PointerToVertex> lPointersToVertex = new List<PointerToVertex>(elementCount["f"]);
    //        List<Triangle> triangles = new List<Triangle>(elementCount["f"]);
    //        NumberFormatInfo nfi = new NumberFormatInfo();
    //        nfi.NumberDecimalSeparator = ".";
    //        nfi.NumberGroupSeparator = ",";
    //        using (StreamReader sr = new StreamReader(path))
    //        {
    //            string[] line;
    //            char[] separator = new char[] { ' ' };
    //            Point3D p = Point3D.Zero;
    //            Point3D pmin = Point3D.Zero, pmax = Point3D.Zero;
    //            while (!sr.EndOfStream)
    //            {
    //                line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
    //                if (line.Length != 0)
    //                {
    //                    switch (line[0])
    //                    {
    //                        case "v": //vertex
    //                            p.X = float.Parse(line[1], nfi);
    //                            p.Y = float.Parse(line[2], nfi);
    //                            p.Z = float.Parse(line[3], nfi);
    //                            this.boundBox.IncludePoint(p);
    //                            vertices.Add(p);
    //                            int percent = (int)(((float)vertices.Count / this.elementCount["v"]) * 100.0f);
    //                            if ((percent % 20) == 0)
    //                            {
    //                                OnElementLoaded(percent, ElementMesh.Vertex);
    //                            }
    //                            break;
    //                        case "vn": //vertex normal
    //                            p.X = float.Parse(line[1], nfi);
    //                            p.Y = float.Parse(line[2], nfi);
    //                            p.Z = float.Parse(line[3], nfi);
    //                            vertexNormals.Add(p);
    //                            percent = (int)(((float)vertexNormals.Count / this.elementCount["vn"]) * 100.0f);
    //                            if ((percent % 20) == 0)
    //                            {
    //                                OnElementLoaded(percent, ElementMesh.VertexNormal);
    //                            }
    //                            break;
    //                        //case "vt": //texture
    //                        //    break;
    //                        case "f": //face
    //                            Triangle t;
    //                            if (vertexNormals.Count > 0)
    //                            {
    //                                string[] pointToFaceComponet1 = line[1].Split('/');
    //                                string[] pointToFaceComponet2 = line[2].Split('/');
    //                                string[] pointToFaceComponet3 = line[3].Split('/');
    //                                t = new Triangle(vertices[int.Parse(pointToFaceComponet1[0])],
    //                                                 vertices[int.Parse(pointToFaceComponet2[0])],
    //                                                 vertices[int.Parse(pointToFaceComponet3[0])]);
    //                                t.NormalOnVertex1 = vertexNormals[int.Parse(pointToFaceComponet1[2])].ToVector3D();
    //                                triangles.Add(t);
    //                            }
    //                            else
    //                            {
    //                                int pToFace1 = int.Parse(line[1]) - 1;
    //                                int pToFace2 = int.Parse(line[2]) - 1;
    //                                int pToFace3 = int.Parse(line[3]) - 1;
    //                                t = new Triangle(vertices[pToFace1],
    //                                                 vertices[pToFace2],
    //                                                 vertices[pToFace3]);
    //                                lPointersToVertex.Add(new PointerToVertex(pToFace1, pToFace2, pToFace3));
    //                                triangles.Add(t);
    //                            }
    //                            percent = (int)(((float)triangles.Count / this.elementCount["f"]) * 100.0f);
    //                            if ((percent % 20) == 0)
    //                            {
    //                                OnElementLoaded(percent, ElementMesh.Triangle);
    //                            }
    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                }
    //            }
    //            this.ProcessNormalsPerVertex(lPointersToVertex, triangles, vertices.Count);
    //            this.triangles = triangles.ToArray();
    //            triangles.Clear();
    //            vertices.Clear();
    //            vertices = null;
    //            triangles = null;
    //            GC.SuppressFinalize(this);
    //            GC.Collect();
    //        }
    //    }
    //    protected void ProcessNormalsPerVertex(List<PointerToVertex> pointersToVertex, List<Triangle> triangles, int verticesCount)
    //    {
    //        Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
    //        for (int i = 0; i < triangles.Count; i++)
    //        {
    //            normalsPerVertex[pointersToVertex[i].Vertex1] += triangles[i].Normal;
    //            normalsPerVertex[pointersToVertex[i].Vertex2] += triangles[i].Normal;
    //            normalsPerVertex[pointersToVertex[i].Vertex3] += triangles[i].Normal;
    //            int percent = (int)(((float)i / triangles.Count) * 100.0f);
    //            if ((percent % 20) == 0)
    //            {
    //                OnElementLoaded(percent / 2, ElementMesh.VertexNormal);
    //            }
    //        }
    //        for (int i = 0; i < triangles.Count; i++)
    //        {
    //            triangles[i].NormalOnVertex1 = normalsPerVertex[pointersToVertex[i].Vertex1];
    //            triangles[i].NormalOnVertex1.Normalize();
    //            triangles[i].NormalOnVertex2 = normalsPerVertex[pointersToVertex[i].Vertex2];
    //            triangles[i].NormalOnVertex2.Normalize();
    //            triangles[i].NormalOnVertex3 = normalsPerVertex[pointersToVertex[i].Vertex3];
    //            triangles[i].NormalOnVertex3.Normalize();
    //            int percent = (int)(((float)i / triangles.Count) * 100.0f);
    //            if ((percent % 20) == 0)
    //            {
    //                OnElementLoaded(percent / 2 + 50, ElementMesh.VertexNormal);
    //            }
    //        }
    //        pointersToVertex.Clear();
    //        pointersToVertex = null;
    //        normalsPerVertex = null;
    //    }
    //}
    [Serializable]
    public class LoaderObjModel : AbstractLoaderModel
    {
        private Dictionary<string, int> elementCount;
        public LoaderObjModel()
            : base()
        {   
        }
        public LoaderObjModel(string path)
            : base()
        { }
        public override event ElementLoadEventHandler OnElementLoaded;
        private void CountElements()
        {
            this.elementCount = new Dictionary<string, int>();
            using(StreamReader sr = new StreamReader(this.path)){
                string[] line;
                while(!sr.EndOfStream){
                    line = sr.ReadLine().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    if(line.Length != 0){
                        if(!this.elementCount.ContainsKey(line[0])){
                            this.elementCount.Add(line[0], 1);
                        } else{
                            this.elementCount[line[0]] = this.elementCount[line[0]] + 1;
                        }
                    }
                }
                sr.Close();
                sr.Dispose();
            }
        }
        public override Triangle[] Load()
        {
            if(!this.Validate()){
                throw new Exception("Invalid file type!");
            }
            this.CountElements();
            this.ParserObjModel();
            return this.triangles;
        }
        private bool Validate()
        {
            return Path.GetExtension(this.path).ToLower() == ".obj";
        }
        public override List<string> Extensions
        {
            get { return new List<string> { ".obj" }; }
        }
        private void ParserObjModel()
        {
            List<Point3D> vertices = new List<Point3D>(this.elementCount["v"]);
            List<Vector3D> vertexNormals = new List<Vector3D>();
            List<PointerToVertex> lPointersToVertex = new List<PointerToVertex>(this.elementCount["f"]);
            List<Triangle> triangles = new List<Triangle>(this.elementCount["f"]);
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            String[] line;
            char[] separator = new char[]{' '};
            Point3D p = Point3D.Zero;
            Vector3D v = Vector3D.Zero;
           
            //foreach (String[] line in file)
            //{
            using(StreamReader sr = new StreamReader(this.path)){
                while(!sr.EndOfStream){
                    line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if(line.Length != 0){
                        switch(line[0]){
                            case "v": //vertex
                                p.X = float.Parse(line[1], nfi);
                                p.Y = float.Parse(line[2], nfi);
                                p.Z = float.Parse(line[3], nfi);
                                this.BoundBox.Include(p);
                                vertices.Add(p);
                                int percent = (int)(((float)vertices.Count / this.elementCount["v"]) * 100.0f);
                                if((percent % 20) == 0){
                                    this.OnElementLoaded(percent, ElementMesh.Vertex);
                                }
                                break;
                            case "vn": //vertex normal
                                v.X = float.Parse(line[1], nfi);
                                v.Y = float.Parse(line[2], nfi);
                                v.Z = float.Parse(line[3], nfi);
                                vertexNormals.Add(v);
                                percent = (int)(((float)vertexNormals.Count / this.elementCount["vn"]) * 100.0f);
                                if((percent % 20) == 0){
                                    this.OnElementLoaded(percent, ElementMesh.VertexNormal);
                                }
                                break;
                                //case "vt": //texture
                                //    break;
                            case "f": //face
                                Triangle t;
                                if(vertexNormals.Count > 0){
                                    string[] pointToFaceComponet1 = line[1].Split('/');
                                    string[] pointToFaceComponet2 = line[2].Split('/');
                                    string[] pointToFaceComponet3 = line[3].Split('/');
                                    t = new Triangle(vertices[int.Parse(pointToFaceComponet1[0]) - 1],
                                                     vertices[int.Parse(pointToFaceComponet2[0]) - 1],
                                                     vertices[int.Parse(pointToFaceComponet3[0]) - 1]);
                                    t.NormalOnVertex1 = vertexNormals[int.Parse(pointToFaceComponet1[2]) - 1];
                                    t.NormalOnVertex2 = vertexNormals[int.Parse(pointToFaceComponet2[2]) - 1];
                                    t.NormalOnVertex3 = vertexNormals[int.Parse(pointToFaceComponet3[2]) - 1];
                                    triangles.Add(t);
                                } else{
                                    int pToFace1 = int.Parse(line[1]) - 1;
                                    int pToFace2 = int.Parse(line[2]) - 1;
                                    int pToFace3 = int.Parse(line[3]) - 1;
                                    t = new Triangle(vertices[pToFace1], vertices[pToFace2], vertices[pToFace3]);
                                    lPointersToVertex.Add(new PointerToVertex(pToFace1, pToFace2, pToFace3));
                                    triangles.Add(t);
                                }
                                percent = (int)(((float)triangles.Count / this.elementCount["f"]) * 100.0f);
                                if((percent % 20) == 0){
                                    this.OnElementLoaded(percent, ElementMesh.Triangle);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if(vertexNormals.Count == 0){
                this.ProcessNormalsPerVertex(lPointersToVertex, triangles, vertices.Count);
            }
            this.triangles = triangles.ToArray();
            vertices = null;
            triangles = null;
            //this.file.Clear();
            //this.file = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private void ProcessNormalsPerVertex(List<PointerToVertex> pointersToVertex, List<Triangle> triangles,
                                               int verticesCount)
        {
            Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
            for(int i = 0; i < triangles.Count; i++){
                normalsPerVertex[pointersToVertex[i].Vertex1] += triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex2] += triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex3] += triangles[i].Normal;
                int percent = (int)(((float)i / triangles.Count) * 100.0f);
                if((percent % 20) == 0){
                    this.OnElementLoaded(percent / 2, ElementMesh.VertexNormal);
                }
            }
            for(int i = 0; i < triangles.Count; i++){
                triangles[i].NormalOnVertex1 = normalsPerVertex[pointersToVertex[i].Vertex1];
                triangles[i].NormalOnVertex1.Normalize();
                triangles[i].NormalOnVertex2 = normalsPerVertex[pointersToVertex[i].Vertex2];
                triangles[i].NormalOnVertex2.Normalize();
                triangles[i].NormalOnVertex3 = normalsPerVertex[pointersToVertex[i].Vertex3];
                triangles[i].NormalOnVertex3.Normalize();
                int percent = (int)(((float)i / triangles.Count) * 100.0f);
                if((percent % 20) == 0){
                    this.OnElementLoaded(percent / 2 + 50, ElementMesh.VertexNormal);
                }
            }
            pointersToVertex.Clear();
            pointersToVertex = null;
            normalsPerVertex = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}