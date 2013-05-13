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
    public class LoaderByuModel : AbstractLoaderModel {
        private StreamReader sr;
        public LoaderByuModel() : base() {}

        public LoaderByuModel(string path) : base(path) {
            this.sr = new StreamReader(path);
        }

        public override event ElementLoadEventHandler OnElementLoaded;

        public override Triangle[] Load() {
            if (!this.Validate()) {
                throw new Exception("Invalid file type!");
            }
            this.ParserByuModel();
            return this.triangles;
        }

        private bool Validate() {
            if (Path.GetExtension(this.path) != ".byu") {
                return false;
            }
            return true;
        }

        public override List<string> Extensions {
            get { return new List<string> {".byu"}; }
        }

        private void ParserByuModel() {
            Point3D[] vertices;
            PointerToVertex[] pointersToVertex;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            using (this.sr = new StreamReader(this.path)) {
                string[] str = this.sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                vertices = new Point3D[Convert.ToInt32(str[0])];
                this.triangles = new Triangle[Convert.ToInt32(str[1])];
                pointersToVertex = new PointerToVertex[this.triangles.Length];
                Point3D pmin, pmax;
                pmin = pmax = Point3D.Zero;
                this.BoundBox = new BoundBox(pmin, pmax);
                for (int i = 0; i < vertices.Length; i++) {
                    str = this.sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    vertices[i] = new Point3D(float.Parse(str[0], nfi), float.Parse(str[1], nfi),
                                              float.Parse(str[2], nfi));
                    //pmin = this.boundBox.PMin;
                    //pmin.X = vertices[i].X < pmin.X ? vertices[i].X : pmin.X;
                    //pmin.Y = vertices[i].Y < pmin.Y ? vertices[i].Y : pmin.Y;
                    //pmin.Z = vertices[i].Z < pmin.Z ? vertices[i].Z : pmin.Z;
                    //pmax = this.boundBox.PMax;
                    //pmax.X = vertices[i].X > pmax.X ? vertices[i].X : pmax.X;
                    //pmax.Y = vertices[i].Y > pmax.Y ? vertices[i].Y : pmax.Y;
                    //pmax.Z = vertices[i].Z > pmax.Z ? vertices[i].Z : pmax.Z;
                    //this.boundBox.PMin = pmin;
                    //this.boundBox.PMax = pmax;
                    this.BoundBox.Include(vertices[i]);
                    int percent = (int) (i * 100 / vertices.Length);
                    if ((percent % 20) == 0) {
                        this.OnElementLoaded((int) ((i * 100 / vertices.Length)), ElementMesh.Vertex);
                    }
                }
                for (int i = 0; i < this.triangles.Length; i++) {
                    str = this.sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    pointersToVertex[i] = new PointerToVertex(Convert.ToInt32(str[0]) - 1, Convert.ToInt32(str[1]) - 1,
                                                              Convert.ToInt32(str[2]) - 1);
                    this.triangles[i] = new Triangle(vertices[pointersToVertex[i].Vertex1],
                                                     vertices[pointersToVertex[i].Vertex2],
                                                     vertices[pointersToVertex[i].Vertex3]);
                    int percent = i * 100 / this.triangles.Length;
                    if ((percent % 20) == 0) {
                        this.OnElementLoaded(i * 100 / this.triangles.Length, ElementMesh.Triangle);
                    }
                }
                this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);
                vertices = null;
                pointersToVertex = null;
                GC.Collect();
            }
        }

        protected void ProcessNormalsPerVertex(PointerToVertex[] pointersToVertex, int verticesCount) {
            Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
            for (int i = 0; i < this.triangles.Length; i++) {
                normalsPerVertex[pointersToVertex[i].Vertex1 - 1] += this.triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex2 - 1] += this.triangles[i].Normal;
                normalsPerVertex[pointersToVertex[i].Vertex3 - 1] += this.triangles[i].Normal;
                int percent = (int) (i * 100 / this.triangles.Length);
                if ((percent % 5) == 0) {
                    this.OnElementLoaded((int) ((i * 100 / this.triangles.Length * 0.5)), ElementMesh.VertexNormal);
                }
            }
            for (int i = 0; i < this.triangles.Length; i++) {
                this.triangles[i].NormalOnVertex1 = normalsPerVertex[pointersToVertex[i].Vertex1 - 1];
                this.triangles[i].NormalOnVertex1.Normalize();
                this.triangles[i].NormalOnVertex2 = normalsPerVertex[pointersToVertex[i].Vertex2 - 1];
                this.triangles[i].NormalOnVertex2.Normalize();
                this.triangles[i].NormalOnVertex3 = normalsPerVertex[pointersToVertex[i].Vertex3 - 1];
                this.triangles[i].NormalOnVertex3.Normalize();
                int percent = (int) (i * 100 / this.triangles.Length);
                if ((percent % 5) == 0) {
                    this.OnElementLoaded(50 + (int) ((i * 100 / this.triangles.Length * 0.5)), ElementMesh.VertexNormal);
                }
            }
            normalsPerVertex = null;
            //for (int i = 0; i < this.triangles.Length; i++) {
            //    this.triangles[i].NormalOnVertex1 = this.normalsPerVertex[this.pointersToVertex[i].Vertex1];
            //    this.triangles[i].NormalOnVertex2 = this.normalsPerVertex[this.pointersToVertex[i].Vertex2];
            //    this.triangles[i].NormalOnVertex3 = this.normalsPerVertex[this.pointersToVertex[i].Vertex3];
            //}
        }
    }
}