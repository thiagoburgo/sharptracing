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
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design;

namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps {
    [Editor(typeof (SphereMapEditor), typeof (UITypeEditor))]
    public class SphereMap : EnvironmentMap {
        private string imagePath;
        private Texture panorama;
        private float radius;
        private float radius2;
        public SphereMap() : this(300, null) {}

        public SphereMap(float radius, string imagePath) {
            this.ImagePath = imagePath;
            this.Radius = radius;
        }

        public String ImagePath {
            get { return this.imagePath; }
            set {
                if (!String.IsNullOrEmpty(value)) {
                    this.imagePath = value;
                    this.panorama = new Texture(this.imagePath);
                }
            }
        }

        public float Radius {
            get { return this.radius; }
            set {
                this.radius = value;
                this.radius2 = this.radius * this.radius;
            }
        }

        public override RGBColor GetColor(Ray ray) {
            Vector3D oc = Point3D.Zero - ray.Origin;
            float l2oc = (oc * oc);
            float tmin = float.PositiveInfinity;
            if (l2oc < this.radius2) {
                // starts inside of the sphere
                float tca = (oc * ray.Direction);
                float l2hc = (this.radius2 - l2oc) / (ray.Direction * ray.Direction) + tca * tca; // division
                tmin = tca + (float) Math.Sqrt(l2hc);
            } else {
                float tca = (oc * ray.Direction);
                if (tca < 0) // points away from the sphere
                {
                    return RGBColor.Black;
                }
                float l2hc = (this.radius2 - l2oc) / (ray.Direction * ray.Direction) + (tca * tca); // division
                if (l2hc > 0) {
                    tmin = tca - (float) Math.Sqrt(l2hc);
                }
            }
            if (!float.IsPositiveInfinity(tmin)) {
                Point3D hitPoint = ray.Origin + tmin * ray.Direction;
                Vector3D normal = hitPoint.ToVector3D().Normalized;
                double theta = Math.Atan2(-normal.X, normal.Z);
                double temp = -normal.Y;
                double phi = Math.Acos(temp);
                double uCoord = theta * (1.0 / (Math.PI + Math.PI));
                double vCoord = 1.0 - phi * (1.0 / Math.PI);
                if (uCoord < 0.0) {
                    uCoord++;
                }
                return this.panorama.GetPixel((float) ((this.panorama.Width - 1) * uCoord),
                                              (float) ((this.panorama.Height - 1) * vCoord));
            }
            return RGBColor.Black;
        }

        public override string ToString() {
            return String.IsNullOrEmpty(this.ImagePath) ? "SphereMap" : Path.GetFileNameWithoutExtension(this.ImagePath);
        }
    }
}