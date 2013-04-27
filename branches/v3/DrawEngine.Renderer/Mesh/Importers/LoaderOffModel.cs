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

namespace DrawEngine.Renderer.Importers {
    [Serializable]
    public class LoaderOffModel : AbstractLoaderModel {
        public LoaderOffModel() : base() {}
        public LoaderOffModel(string path) : base(path) {}
        public override event ElementLoadEventHandler OnElementLoaded;

        public override Triangle[] Load() {
            this.ParserOffModel();
            return this.triangles;
        }

        public override List<string> Extensions {
            get { return new List<string> {".off", ".noff", ".cnoff"}; }
        }

        private void ParserOffModel() {
            Point3D[] vertices;
            Vector3D[] vertexNormals = null;
            PointerToVertex[] pointersToVertex;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            String[] line;
            char[] separator = new char[] {' '};
            bool hasNormals = false;

            using (StreamReader sr = new StreamReader(this.path)) {
                string header = sr.ReadLine().ToUpper(); //read header (OFF)
                while (String.IsNullOrEmpty(header)) {
                    header = sr.ReadLine().ToUpper(); //read header (OFF)
                }
                if (header.EndsWith("OFF")) {
                    if (header.EndsWith("NOFF")) {
                        hasNormals = true;
                    }
                }
                line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                while (line.Length == 0) {
                    line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                }
                vertices = new Point3D[int.Parse(line[0], nfi)];
                this.triangles = new Triangle[int.Parse(line[1], nfi)];
                pointersToVertex = new PointerToVertex[this.triangles.Length];
                if (hasNormals) {
                    vertexNormals = new Vector3D[vertices.Length];
                }
                int percent = 0;
                for (int i = 0; i < vertices.Length; i++) {
                    line = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (line.Length == 0) {
                        i--;
                        continue;
                    }
                    vertices[i].X = float.Parse(line[0], nfi);
                    vertices[i].Y = float.Parse(line[1], nfi);
                    vertices[i].Z = float.Parse(line[2], nfi);
                    //Adjusting BoundBox...
                    this.BoundBox.Include(vertices[i]);
                    //Reporting progress
                    percent = (int) (((float) i / vertices.Length) * 100.0f);
                    if ((percent % 10) == 0) {
                        this.OnElementLoaded(percent, ElementMesh.Vertex);
                    }
                    if (hasNormals) {
                        vertexNormals[i].X = float.Parse(line[3], nfi);
                        vertexNormals[i].Y = float.Parse(line[4], nfi);
                        vertexNormals[i].Z = float.Parse(line[5], nfi);
                        percent = (int) (((float) i / vertices.Length) * 100.0f);
                        if ((percent % 10) == 0) {
                            this.OnElementLoaded(percent, ElementMesh.VertexNormal);
                        }
                    }
                }
                for (int i = 0; i < this.triangles.Length; i++) {
                    line = sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    pointersToVertex[i] = new PointerToVertex(Int32.Parse(line[1], nfi), Int32.Parse(line[2], nfi),
                                                              Int32.Parse(line[3], nfi));
                    this.triangles[i] = new Triangle(vertices[pointersToVertex[i].Vertex1],
                                                     vertices[pointersToVertex[i].Vertex2],
                                                     vertices[pointersToVertex[i].Vertex3]);
                    percent = (int) (((float) i / this.triangles.Length) * 100.0f);
                    if ((percent % 10) == 0) {
                        this.OnElementLoaded(percent, ElementMesh.Triangle);
                    }
                }
            }
            if (!hasNormals) {
                this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);
            }
            vertices = null;
            vertexNormals = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void ProcessNormalsPerVertex(PointerToVertex[] pointersToVertex, int verticesCount) {
            Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
            for (int i = 0; i < this.triangles.Length; i++) {
                normalsPerVertex[pointersToVertex[i].Vertex1] += this.triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex2] += this.triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex3] += this.triangles[i].Normal;
                int percent = (int) (((float) i / this.triangles.Length) * 100.0f);
                if ((percent % 20) == 0) {
                    this.OnElementLoaded(percent / 2, ElementMesh.VertexNormal);
                }
            }
            for (int i = 0; i < this.triangles.Length; i++) {
                this.triangles[i].NormalOnVertex1 = normalsPerVertex[pointersToVertex[i].Vertex1];
                this.triangles[i].NormalOnVertex1.Normalize();
                this.triangles[i].NormalOnVertex2 = normalsPerVertex[pointersToVertex[i].Vertex2];
                this.triangles[i].NormalOnVertex2.Normalize();
                this.triangles[i].NormalOnVertex3 = normalsPerVertex[pointersToVertex[i].Vertex3];
                this.triangles[i].NormalOnVertex3.Normalize();
                int percent = (int) (( i / (float)this.triangles.Length) * 100.0f);
                if ((percent % 20) == 0) {
                    this.OnElementLoaded((percent / 2) + 50, ElementMesh.VertexNormal);
                }
            }
            pointersToVertex = null;
            normalsPerVertex = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}