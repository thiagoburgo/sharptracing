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

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree {
    /// <summary>
    /// Hyper-Rectangle public class supporting KDTree class
    /// </summary>
    public class HRect : ICloneable {
        private readonly HPoint max;
        private readonly HPoint min;

        public HRect(int ndims) {
            this.min = new HPoint(ndims);
            this.max = new HPoint(ndims);
        }

        public HRect(HPoint vmin, HPoint vmax) {
            this.min = (HPoint) vmin.Clone();
            this.max = (HPoint) vmax.Clone();
        }

        public HPoint Min {
            get { return this.min; }
        }

        public HPoint Max {
            get { return this.max; }
        }

        public double Area {
            get {
                double a = 1;
                for (int i = 0; i < this.min.Coord.Length; ++i) {
                    a *= (this.max.Coord[i] - this.min.Coord[i]);
                }
                return a;
            }
        }

        #region ICloneable Members

        public object Clone() {
            return new HRect(this.min, this.max);
        }

        #endregion

        // from Moore's eqn. 6.6
        public HPoint Closest(HPoint t) {
            HPoint p = new HPoint(t.Coord.Length);
            for (int i = 0; i < t.Coord.Length; ++i) {
                if (t.Coord[i] <= this.min.Coord[i]) {
                    p.Coord[i] = this.min.Coord[i];
                } else if (t.Coord[i] >= this.max.Coord[i]) {
                    p.Coord[i] = this.max.Coord[i];
                } else {
                    p.Coord[i] = t.Coord[i];
                }
            }
            return p;
        }

        // used in initial conditions of KDTree.nearest()
        public static HRect InfiniteHRect(int d) {
            HPoint vmin = new HPoint(d);
            HPoint vmax = new HPoint(d);
            for (int i = 0; i < d; ++i) {
                vmin.Coord[i] = double.NegativeInfinity;
                vmax.Coord[i] = double.PositiveInfinity;
            }
            return new HRect(vmin, vmax);
        }

        // currently unused
        public HRect Intersection(HRect r) {
            HPoint newmin = new HPoint(this.min.Coord.Length);
            HPoint newmax = new HPoint(this.min.Coord.Length);
            for (int i = 0; i < this.min.Coord.Length; ++i) {
                newmin.Coord[i] = Math.Max(this.min.Coord[i], r.min.Coord[i]);
                newmax.Coord[i] = Math.Min(this.max.Coord[i], r.max.Coord[i]);
                if (newmin.Coord[i] >= newmax.Coord[i]) {
                    return null;
                }
            }
            return new HRect(newmin, newmax);
        }

        // currently unused
        public override string ToString() {
            return this.min + "\n" + this.max + "\n";
        }
    }
}