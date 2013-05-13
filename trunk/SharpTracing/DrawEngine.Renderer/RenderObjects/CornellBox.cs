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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects {
    //http://www.graphics.cornell.edu/online/box/data.html
    public class CornellBox : Primitive {
        private readonly Dictionary<String, Primitive> cornellBoxGroup;
        //private Quadrilatero floor, ceiling, backWall, rightWall, leftWall;
        private Box shortBlock, tallBlock;

        public CornellBox() {
            this.cornellBoxGroup = new Dictionary<string, Primitive>(10);
            this.cornellBoxGroup["floor"] = new Quadrilatero(new Point3D(0, -50, 0), Vector3D.UnitY, 100, 100);
            this.cornellBoxGroup["ceiling"] = new Quadrilatero(new Point3D(0, 50, 0), -Vector3D.UnitY, 100, 100);
            this.cornellBoxGroup["backWall"] = new Quadrilatero(new Point3D(0, 0, 50), -Vector3D.UnitZ, 100, 100);
            Material white = new PhongMaterial(1, 0, 0, 0, 0, 0, 0, RGBColor.White);
            this.cornellBoxGroup["floor"].Material =
                this.cornellBoxGroup["ceiling"].Material = this.cornellBoxGroup["backWall"].Material = white;

            this.cornellBoxGroup["rightWall"] = new Quadrilatero(new Point3D(50, 0, 0), -Vector3D.UnitX, 100, 100);
            Material green = new PhongMaterial(1, 0, 0, 0, 0, 0, 0, RGBColor.Green);
            this.cornellBoxGroup["rightWall"].Material = green;

            this.cornellBoxGroup["leftWall"] = new Quadrilatero(new Point3D(-50, 0, 0), Vector3D.UnitX, 100, 100);
            Material red = new PhongMaterial(1, 0, 0, 0, 0, 0, 0, RGBColor.Red);

            this.cornellBoxGroup["leftWall"].Material = red;
        }

        public override bool FindIntersection(Ray ray, out Intersection intersect) {
            intersect = new Intersection();
            intersect.TMin = float.MaxValue;
            Intersection intersection_comp;
            bool hit = false;
            foreach (Primitive hitPrimitive in this.cornellBoxGroup.Values) {
                if (hitPrimitive.FindIntersection(ray, out intersection_comp) && intersection_comp.TMin < intersect.TMin) {
                    intersect = intersection_comp;
                    //if(intersect.Normal * ray.Direction > 0) {
                    //    intersect.Normal.Flip();
                    //}
                    hit = true;
                }
            }

            return hit;
        }

        public override bool IsInside(Point3D point) {
            throw new NotImplementedException();
        }

        public override bool IsOverlap(BoundBox boundBox) {
            throw new NotImplementedException();
        }

        public override Vector3D NormalOnPoint(Point3D pointInPrimitive) {
            throw new NotImplementedException();
        }
    }
}