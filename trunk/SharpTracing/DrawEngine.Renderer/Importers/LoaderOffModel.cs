using System;
using System.Globalization;
using System.IO;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Importers
{
    [Serializable]
    public class LoaderOffModel : AbstractLoaderModel
    {
        private string path;
        //private List<string[]> file;
        public LoaderOffModel(string path, string name)
        {
            this.path = path;
        }
        public override event ElementLoadEventHandler OnElementLoaded;
        public override Triangle[] Load()
        {
            this.ParserOffModel();
            return this.triangles;
        }
        private void ParserOffModel()
        {
            Point3D[] vertices;
            Vector3D[] vertexNormals;
            PointerToVertex[] pointersToVertex;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            String[] line;
            char[] separator = new char[]{' '};
            Point3D p = Point3D.Zero;
            Point3D pmin = Point3D.Zero, pmax = Point3D.Zero;
            bool hasNormals = false;
            using(StreamReader sr = new StreamReader(this.path)){
                string header = sr.ReadLine().ToUpper(); //read header (OFF)
                while(String.IsNullOrEmpty(header)){
                    header = sr.ReadLine().ToUpper(); //read header (OFF)
                }
                if(header.EndsWith("OFF")){
                    if(header.EndsWith("NOFF")){
                        hasNormals = true;
                    }
                }
                line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                while(line.Length == 0){
                    line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                }
                vertices = new Point3D[int.Parse(line[0], nfi)];
                this.triangles = new Triangle[int.Parse(line[1], nfi)];
                vertexNormals = new Vector3D[vertices.Length];
                pointersToVertex = new PointerToVertex[this.triangles.Length];
                //while (!sr.EndOfStream)
                //{
                int percent = 0;
                for(int i = 0; i < vertices.Length; i++){
                    line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if(line.Length == 0){
                        i--;
                        continue;
                    }
                    vertices[i].X = float.Parse(line[0], nfi);
                    vertices[i].Y = float.Parse(line[1], nfi);
                    vertices[i].Z = float.Parse(line[2], nfi);
                    //Adjusting BoundBox...
                    this.boundBox.Include(vertices[i]);
                    //Reporting progress
                    percent = (int)(((float)i / vertices.Length) * 100.0f);
                    if((percent % 20) == 0){
                        this.OnElementLoaded(percent, ElementMesh.Vertex);
                    }
                    if(hasNormals){
                        vertexNormals[i].X = float.Parse(line[3], nfi);
                        vertexNormals[i].Y = float.Parse(line[4], nfi);
                        vertexNormals[i].Z = float.Parse(line[5], nfi);
                        percent = (int)(((float)i / vertices.Length) * 100.0f);
                        if((percent % 20) == 0){
                            this.OnElementLoaded(percent, ElementMesh.VertexNormal);
                        }
                    }
                }
                for(int i = 0; i < this.triangles.Length; i++){
                    line = sr.ReadLine().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    pointersToVertex[i] = new PointerToVertex(Int32.Parse(line[1], nfi), Int32.Parse(line[2], nfi),
                                                              Int32.Parse(line[3], nfi));
                    this.triangles[i] = new Triangle(vertices[pointersToVertex[i].Vertex1],
                                                     vertices[pointersToVertex[i].Vertex2],
                                                     vertices[pointersToVertex[i].Vertex3]);
                    percent = (int)(((float)i / this.triangles.Length) * 100.0f);
                    if((percent % 20) == 0){
                        this.OnElementLoaded(percent, ElementMesh.Triangle);
                    }
                }
            }
            if(!hasNormals){
                this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);
            }
            Array.Clear(vertices, 0, vertices.Length);
            Array.Clear(vertexNormals, 0, vertexNormals.Length);
            vertices = null;
            GC.Collect();
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
            Array.Clear(pointersToVertex, 0, pointersToVertex.Length);
            Array.Clear(normalsPerVertex, 0, normalsPerVertex.Length);
            pointersToVertex = null;
            normalsPerVertex = null;
        }
    }
}