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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects {
    [Serializable]
    public class Cone : Primitive, ITransformable3D {
        private Point3D apex;
        //private Point3D pBase;
        private float apexDotCenterAxis;
        private Vector3D axisA;
        private Vector3D axisB;
        //private float halfHeight;
        private Vector3D baseNormal;
        private Vector3D centralAxis;
        private float coefBasePlane;
        private float height;
        private float radiusA;
        private float radiusB;
        private float slopeA;
        private float slopeB;
        public Cone() : this(Point3D.Zero, Vector3D.UnitY, 20, 20) {}

        public Cone(Point3D center, Vector3D centralAxis, Vector3D radialAxis, float radiusA, float radiusB,
                    float height) {
            this.RadiusA = radiusA;
            this.RadiusB = radiusB;
            this.CentralAxis = centralAxis;
            //this.Apex = apex;
            this.Center = center;
            this.Height = height;
            this.RadialAxis = radialAxis;
        }

        public Cone(Point3D center, Vector3D centralAxis, float radius, float height)
            : this(center, centralAxis, Vector3D.UnitZ, radius, radius, height) {}

        public Point3D Apex {
            get { return this.apex; }
            set {
                this.apex = value;
                this.center = this.apex - this.centralAxis * this.height * 0.5f;
                this.apexDotCenterAxis = this.apex * this.centralAxis;
                this.coefBasePlane = -(this.apexDotCenterAxis - this.height);
            }
        }

        public Point3D Center {
            get { return base.center; }
            set {
                base.center = value;
                this.apex = this.center + this.centralAxis * this.height * 0.5f;
                this.apexDotCenterAxis = this.apex * this.centralAxis;
                this.coefBasePlane = -(this.apexDotCenterAxis - this.height);
            }
        }

        public Vector3D CentralAxis {
            get { return this.centralAxis; }
            set {
                this.centralAxis = value;
                this.centralAxis.Normalize();
                this.apexDotCenterAxis = (this.apex * this.centralAxis);
                Vector3D.Orthonormalize(this.centralAxis, out this.axisA, out this.axisB);
                if (this.slopeA > 0) {
                    this.axisA /= this.slopeA;
                }
                if (this.slopeB > 0) {
                    this.axisB /= this.slopeB;
                }
                this.baseNormal = -this.centralAxis;
                this.coefBasePlane = -((this.apexDotCenterAxis) - this.height);
            }
        }

        public float RadiusA {
            get { return this.radiusA; }
            set {
                if (value > 0) {
                    this.radiusA = value;
                    if (this.height > 0) {
                        this.slopeA = this.radiusA / this.height;
                        this.axisA /= this.slopeA;
                    }
                }
            }
        }

        public float RadiusB {
            get { return this.radiusB; }
            set {
                if (value > 0) {
                    this.radiusB = value;
                    if (this.height > 0) {
                        this.slopeB = this.radiusB / this.height;
                        this.axisB /= this.slopeB;
                    }
                }
            }
        }

        public Vector3D RadialAxis {
            get { return this.axisA; }
            set {
                this.axisA = value;
                Vector3D.Orthogonalize(ref this.axisA, this.centralAxis);
                //this.axisA -= (this.axisA * this.centerAxis) * this.centerAxis;
                if (this.axisA.Length.NearZero()) {
                    throw new Exception("O eixo radial não deve ter tamanho ZERO!");
                }
                //this.axisA.Normalize();
                if (!this.slopeA.NearZero()) {
                    this.axisA /= this.slopeA;
                }
                this.axisB = this.centralAxis ^ this.axisA;
                //this.axisB.Normalize();
                if (!this.slopeB.NearZero()) {
                    this.axisB /= this.slopeB;
                }
            }
        }

        public float Height {
            get { return this.height; }
            set {
                if (value > 0) {
                    this.height = value;
                    if (this.radiusB > 0) {
                        this.slopeB = this.radiusB / this.height;
                        this.axisB /= (this.slopeB / 2);
                    }
                    if (this.radiusA > 0) {
                        this.slopeA = this.radiusA / this.height;
                        this.axisA /= (this.slopeA / 2);
                    }
                }
                this.apex = this.center + this.centralAxis * this.height * 0.5f;
                this.apexDotCenterAxis = this.apex * this.centralAxis;
                this.coefBasePlane = -(this.apexDotCenterAxis - this.height);
            }
        }

        #region ITransformable3D Members

        public void Rotate(float angle, Vector3D axis) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAxisX(float angle) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAxisY(float angle) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAxisZ(float angle) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Scale(float factor) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(float tx, float ty, float tz) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(Vector3D translateVector) {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }

        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect) {
            intersect = new Intersection();
            float maxFrontDist = float.NegativeInfinity;
            float minBackDist = float.PositiveInfinity;
            HitSide frontType = HitSide.None, backType = HitSide.None; // 0, 1 = base, side
            float viewPosdotCtr = ray.Origin * this.centralAxis;
            float udotuCtr = ray.Direction * this.centralAxis;
            if (viewPosdotCtr > (this.apexDotCenterAxis) && udotuCtr >= 0.0f) {
                return false; // Above the cone's apex
            }
            // Start with the bounding base plane
            float pdotnCap = this.baseNormal * ray.Origin;
            float udotnCap = this.baseNormal * ray.Direction;
            if (pdotnCap > this.coefBasePlane) {
                if (udotnCap >= 0.0f) {
                    return false; // Above (=outside) base plane, pointing away
                }
                maxFrontDist = (this.coefBasePlane - pdotnCap) / udotnCap;
                frontType = HitSide.BottomPlane;
            } else if (pdotnCap < this.coefBasePlane) {
                if (udotnCap > 0.0f) {
                    // Below (=inside) base plane, pointing towards the plane
                    minBackDist = (this.coefBasePlane - pdotnCap) / udotnCap;
                    backType = HitSide.BottomPlane;
                }
            }
            // Now handle the cone's sides
            Vector3D v = ray.Origin - this.apex;
            float pdotuCtr = v * this.centralAxis;
            float pdotuA = v * this.axisA;
            float pdotuB = v * this.axisB;
            // udotuCtr already defined above
            float udotuA = ray.Direction * this.axisA;
            float udotuB = ray.Direction * this.axisB;
            float C = pdotuA * pdotuA + pdotuB * pdotuB - pdotuCtr * pdotuCtr;
            float B = (pdotuA * udotuA + pdotuB * udotuB - pdotuCtr * udotuCtr);
            B += B;
            float A = udotuA * udotuA + udotuB * udotuB - udotuCtr * udotuCtr;
            float alpha1, alpha2; // The roots, in order
            int numRoots = EquationSolver.SolveQuadric(A, B, C, out alpha1, out alpha2);
            if (numRoots == 0) {
                return false; // No intersection
            }
            bool viewMoreVertical = (A < 0.0f);
            if (viewMoreVertical) {
                // View line leaves and then enters the cone
                if (alpha1 < minBackDist && pdotuCtr + alpha1 * udotuCtr <= 0.0f) {
                    if (alpha1 < maxFrontDist) {
                        return false;
                    }
                    minBackDist = alpha1;
                    backType = HitSide.Cone;
                } else if (numRoots == 2 && alpha2 > maxFrontDist && pdotuCtr + alpha2 * udotuCtr <= 0.0f) {
                    if (alpha2 > minBackDist) {
                        return false;
                    }
                    maxFrontDist = alpha2;
                    frontType = HitSide.Cone;
                }
            } else {
                // view line enters and then leaves
                if (alpha1 > maxFrontDist) {
                    if (pdotuCtr + alpha1 * udotuCtr > 0.0f) {
                        return false; // Enters dual cone instead
                    }
                    if (alpha1 > minBackDist) {
                        return false;
                    }
                    maxFrontDist = alpha1;
                    frontType = HitSide.Cone;
                }
                if (numRoots == 2 && alpha2 < minBackDist) {
                    if (pdotuCtr + alpha2 * udotuCtr > 0.0f) {
                        return false; // Is leaving dual cone instead
                    }
                    if (alpha2 < maxFrontDist) {
                        return false;
                    }
                    minBackDist = alpha2;
                    backType = HitSide.Cone;
                }
            }
            // Put it all together:
            float alpha;
            HitSide hitSurface = HitSide.None;
            if (maxFrontDist > 0.0f) {
                alpha = maxFrontDist;
                hitSurface = frontType;
            } else {
                alpha = minBackDist;
                hitSurface = backType;
            }
            if (alpha < 0.0) {
                return false;
            }
            intersect.TMin = alpha;
            // Set v to the intersection point
            intersect.HitPoint = ray.Origin + intersect.TMin * ray.Direction;
            intersect.HitPrimitive = this;
            // Now set v equal to returned position relative to the apex
            v = intersect.HitPoint - this.apex;
            float vdotuA = v * this.axisA;
            float vdotuB = v * this.axisB;
            float vdotuCtr = v * this.centralAxis;
            switch (hitSurface) {
                case HitSide.BottomPlane: // Base face
                    intersect.Normal = this.baseNormal;
                    if (this.material != null && this.material.IsTexturized) {
                        // Calculate U-V values for texture coordinates
                        vdotuA /= vdotuCtr; // vdotuCtr is negative
                        vdotuB /= vdotuCtr;
                        vdotuA = 0.5f * (1.0f - vdotuA);
                        vdotuB = 0.5f * (1.0f - vdotuB);
                        //int widthTex = this.material.Texture.Width - 1;
                        //int heightTex = this.material.Texture.Height - 1;
                        //this.material.Color =
                        //    this.material.Texture.GetPixel((int)(vdotuB * widthTex), (int)(vdotuA * heightTex));
                        intersect.CurrentTextureCoordinate.U = vdotuB;
                        intersect.CurrentTextureCoordinate.V = vdotuA;
                    }
                    break;
                case HitSide.Cone: // Cone's side
                    intersect.Normal = vdotuA * this.axisA;
                    intersect.Normal += vdotuB * this.axisB;
                    intersect.Normal -= vdotuCtr * this.centralAxis;
                    intersect.Normal.Normalize();
                    if (this.material != null && this.material.IsTexturized) {
                        // Calculate u-v coordinates for texture mapping (in range[0,1]x[0,1])
                        float uCoord = (float) (Math.Atan2(vdotuB, vdotuA) / (Math.PI + Math.PI) + 0.5);
                        float vCoord = (vdotuCtr + this.height) / this.height;
                        //int widthTex = this.material.Texture.Width - 1;
                        //int heightTex = this.material.Texture.Height - 1;
                        //this.material.Color =
                        //    this.material.Texture.GetPixel((int)(uCoord * widthTex), (int)(vCoord * heightTex));
                        intersect.CurrentTextureCoordinate.U = uCoord;
                        intersect.CurrentTextureCoordinate.V = vCoord;
                    }
                    break;
            }
            return true;
        }

        public override Vector3D NormalOnPoint(Point3D pointInPrimitive) {
            Vector3D v = pointInPrimitive - this.apex;
            float vdotuA = v * this.axisA;
            float vdotuB = v * this.axisB;
            float vdotuCtr = v * this.centralAxis;
            if (vdotuCtr < -this.height + 0.01f && vdotuCtr > -this.height - 0.01f) {
                return this.baseNormal;
            } else {
                Vector3D normal = vdotuA * this.axisA;
                normal += vdotuB * this.axisB;
                normal -= vdotuCtr * this.centralAxis;
                normal.Normalize();
                return normal;
            }
        }

        public override bool IsInside(Point3D point) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsOverlap(BoundBox boundBox) {
            throw new NotImplementedException();
        }

        #region Nested type: HitSide

        private enum HitSide {
            BottomPlane,
            Cone,
            None
        }

        #endregion
    }
}