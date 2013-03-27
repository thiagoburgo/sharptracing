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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects {
    [Serializable]
    public class Box : Primitive, ITransformable3D {
        private float bottomCoefABC;
        private float bottomCoefABD;
        private float bottomCoefACD;
        private Vector3D edgeAB, edgeAC, edgeAD;
        private float halfLenX, halfLenY, halfLenZ;
        private float inv_TopBottomABC, inv_TopBottomABD, inv_TopBottomACD;
        private float lengthX, lengthY, lengthZ;
        private Vector3D normalABC, normalABD, normalACD;
        private float topCoefABC;
        private float topCoefABD;
        private float topCoefACD;
        private Point3D vB, vC, vD;
        private Point3D vHigh;
        private Point3D vLow;
        public Box() : this(Point3D.Zero, 20, 20, 20) {}

        public Box(Point3D center, float lengthX, float lengthY, float lengthZ) {
            this.center = center;
            this.lengthX = lengthX;
            this.halfLenX = this.lengthX * 0.5f;
            this.lengthY = lengthY;
            this.halfLenY = this.lengthY * 0.5f;
            this.lengthZ = lengthZ;
            this.halfLenZ = this.lengthZ * 0.5f;
            this.CalculateBounds();
        }

        [RefreshProperties(RefreshProperties.All)]
        public float LengthX {
            get { return this.lengthX; }
            set {
                if (value > 0.0f) {
                    this.lengthX = value;
                    this.halfLenX = value * 0.5f;
                    this.vLow.X = this.center.X - this.halfLenX;
                    this.vHigh.X = this.center.X + this.halfLenX;
                    this.CalculateBounds();
                } else {
                    throw new ArgumentException("A dimensão em X (LengthX) tem que ser maior de Zero (lengthX > 0)!");
                }
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public Point3D Center {
            get { return base.center; }
            set {
                base.center = value;
                this.CalculateBounds();
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public float LengthY {
            get { return this.lengthY; }
            set {
                if (value > 0.0f) {
                    this.lengthY = value;
                    this.halfLenY = value * 0.5f;
                    this.vLow.Y = this.center.Y - this.halfLenY;
                    this.vHigh.Y = this.center.Y + this.halfLenY;
                    this.CalculateBounds();
                } else {
                    throw new ArgumentException("A dimensão em Y (LengthY) tem que ser maior de Zero (lengthY > 0)!");
                }
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public float LengthZ {
            get { return this.lengthZ; }
            set {
                if (value > 0.0f) {
                    this.lengthZ = value;
                    this.halfLenZ = value * 0.5f;
                    this.vLow.Z = this.center.Z - this.halfLenZ;
                    this.vHigh.Z = this.center.Z + this.halfLenZ;
                    this.CalculateBounds();
                } else {
                    throw new ArgumentException("A dimensão em Z (LengthZ) tem que ser maior de Zero (lengthZ > 0)!");
                }
            }
        }

        public Point3D this[int index] {
            get {
                if (index % 2 == 0) {
                    return this.vLow;
                } else {
                    return this.vHigh;
                }
            }
            set {
                if (index % 2 == 0) {
                    this.vLow = value;
                } else {
                    this.vHigh = value;
                }
            }
        }

        #region ITransformable3D Members

        public void Rotate(float angle, Vector3D axis) {
            this.vHigh.Rotate(angle, axis);
            this.vLow.Rotate(angle, axis);
            //throw new Exception("The method or operation is not implemented.");
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
            this.LengthX = this.lengthX * factor;
            this.LengthY = this.lengthY * factor;
            this.LengthZ = this.lengthZ * factor;
            //this.vLow.Scale(factor);
            //this.vHigh.Scale(factor);
        }

        public void Translate(float tx, float ty, float tz) {
            this.center.Translate(tx, ty, tz);
            this.Center = this.center;
        }

        public void Translate(Vector3D translateVector) {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }

        #endregion

        private void CalculateBounds() {
            this.vLow.X = this.center.X - this.halfLenX;
            this.vLow.Y = this.center.Y - this.halfLenY;
            this.vLow.Z = this.center.Z - this.halfLenZ;
            this.vHigh.X = this.center.X + this.halfLenX;
            this.vHigh.Y = this.center.Y + this.halfLenY;
            this.vHigh.Z = this.center.Z + this.halfLenZ;
            this.vB.X = this.vHigh.X;
            this.vB.Y = this.vLow.Y;
            this.vB.Z = this.vLow.Z;
            this.vC.X = this.vLow.X;
            this.vC.Y = this.vHigh.Y;
            this.vC.Z = this.vLow.Z;
            this.vD.X = this.vLow.X;
            this.vD.Y = this.vLow.Y;
            this.vD.Z = this.vHigh.Z;
            this.edgeAB = this.vB - this.vLow;
            this.edgeAC = this.vC - this.vLow;
            this.edgeAD = this.vD - this.vLow;
            this.normalABC = this.edgeAB ^ this.edgeAC;
            this.normalABD = this.edgeAB ^ this.edgeAD;
            this.normalACD = this.edgeAC ^ this.edgeAD;
            this.normalABC.Normalize();
            this.topCoefABC = this.normalABC * this.vD; // Front face coef.
            this.bottomCoefABC = this.normalABC * this.vLow; // Back face coef.
            if (this.topCoefABC < this.bottomCoefABC) {
                float temp = this.topCoefABC;
                this.topCoefABC = this.bottomCoefABC;
                this.bottomCoefABC = temp;
            }
            this.normalABD.Normalize();
            this.topCoefABD = this.normalABD * this.vC; // Front face coef.
            this.bottomCoefABD = this.normalABD * this.vLow; // Back face coef.
            if (this.topCoefABD < this.bottomCoefABD) {
                float temp = this.topCoefABD;
                this.topCoefABD = this.bottomCoefABD;
                this.bottomCoefABD = temp;
            }
            this.normalACD.Normalize();
            this.topCoefACD = this.normalACD * this.vB; // Front face coef.
            this.bottomCoefACD = this.normalACD * this.vLow; // Back face coef.
            if (this.topCoefACD < this.bottomCoefACD) {
                float temp = this.topCoefACD;
                this.topCoefACD = this.bottomCoefACD;
                this.bottomCoefACD = temp;
            }
            /* Pre-Calculo Para a Texturizacao */
            this.inv_TopBottomABC = 1.0f / (this.topCoefABC - this.bottomCoefABC);
            this.inv_TopBottomABD = 1.0f / (this.topCoefABD - this.bottomCoefABD);
            this.inv_TopBottomACD = 1.0f / (this.topCoefACD - this.bottomCoefACD);
            this.boundBox = new BoundBox(this.vLow, this.vHigh);
            this.center = this.boundBox.Center;
        }

        private enum Sides {
            None,
            Left,
            Right,
            Top,
            Bottom,
            Front,
            Back
        }

        public override bool FindIntersection(Ray ray, out Intersection intersection) {
            intersection = new Intersection();
            float tNear = float.NegativeInfinity, tFar = float.PositiveInfinity, t1, t2;
            float oX = ray.Origin.X, oY = ray.Origin.Y, oZ = ray.Origin.Z;
            float inv_dX = ray.InvertedDirection.X, inv_dY = ray.InvertedDirection.Y, inv_dZ = ray.InvertedDirection.Z;
            float temp;
            //Sides hitSide = Sides.None;
            //bool x = false, y = false, z = false;

            #region X

            if (ray.Direction.X.NearZero()) {
                if ((oX < this.vLow.X || oX > this.vHigh.X)) {
                    return false;
                }
            } else {
                t1 = (this.vLow.X - oX) * inv_dX;
                t2 = (this.vHigh.X - oX) * inv_dX;

                if (t1 > t2) {
                    temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                //    hitSide = Sides.Right;
                //} else{
                //    hitSide = Sides.Left;    
                //}
                if (t1 > tNear) {
                    tNear = t1;
                }
                if (t2 < tFar) {
                    tFar = t2;
                }
                if (tNear > tFar || tFar < 0.1f) {
                    return false;
                }
            }

            #endregion

            #region Y

            if (ray.Direction.Y.NearZero()) {
                if ((oY < this.vLow.Y || oY > this.vHigh.Y)) {
                    return false;
                }
            } else {
                t1 = (this.vLow.Y - oY) * inv_dY;
                t2 = (this.vHigh.Y - oY) * inv_dY;

                if (t1 > t2) {
                    temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                //    hitSide = Sides.Top;
                //} else{
                //    hitSide = Sides.Bottom;    
                //}
                if (t1 > tNear) {
                    tNear = t1;
                    //y = true;
                }
                if (t2 < tFar) {
                    tFar = t2;
                }
                if (tNear > tFar || tFar < 0.1f) {
                    return false;
                }
            }

            #endregion

            #region Z

            if (ray.Direction.Z.NearZero()) {
                if ((oZ < this.vLow.Z || oZ > this.vHigh.Z)) {
                    return false;
                }
            } else {
                t1 = (this.vLow.Z - oZ) * inv_dZ;
                t2 = (this.vHigh.Z - oZ) * inv_dZ;

                if (t1 > t2) {
                    temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                //    hitSide = Sides.Back;
                //} else{
                //    hitSide = Sides.Front;    
                //}
                if (t1 > tNear) {
                    tNear = t1;
                }
                if (t2 < tFar) {
                    tFar = t2;
                }
                if (tNear > tFar || tFar < 0.1f) {
                    return false;
                }
            }

            #endregion

            intersection.TMin = tNear;
            intersection.TMax = tFar;
            //if (tNear > 0.00001f)
            //{   
            //    intersection.TMin = tNear;
            //    intersection.TMax = tFar;
            //}
            //else
            //{
            //    intersection.TMin = tFar;
            //    intersection.TMax = tNear;
            //}
            //if (intersection.TMin < 0.01f) {
            //    return false;
            //}
            intersection.HitPoint = (ray.Origin + (intersection.TMin * ray.Direction));
            intersection.HitPrimitive = this;


            //switch (hitSide)
            //{
            //    case Sides.Left:
            //        intersection.Normal.X = -1.0f;
            //        break;
            //    case Sides.Right:
            //        intersection.Normal.X = 1.0f;
            //        break;
            //    case Sides.Top:
            //        intersection.Normal.Y = 1.0f;
            //        break;
            //    case Sides.Bottom:
            //        intersection.Normal.Y = -1.0f;
            //        break;
            //    case Sides.Front:
            //        intersection.Normal.Z = -1.0f;
            //        break;
            //    case Sides.Back:
            //        intersection.Normal.Z = 1.0f;
            //        break;
            //}

            #region Calculo da Normal

            Sides hitSide = Sides.None;
            if (MathUtil.NearZero(intersection.HitPoint.X - this.vLow.X, 0.01)) {
                intersection.Normal.X = -1;
                hitSide = Sides.Left;
            } else if (MathUtil.NearZero(intersection.HitPoint.X - this.vHigh.X, 0.01)) {
                intersection.Normal.X = 1;
                hitSide = Sides.Right;
            } else if (MathUtil.NearZero(intersection.HitPoint.Y - this.vLow.Y, 0.01)) {
                intersection.Normal.Y = -1;
                hitSide = Sides.Bottom;
            } else if (MathUtil.NearZero(intersection.HitPoint.Y - this.vHigh.Y, 0.01)) {
                intersection.Normal.Y = 1;
                hitSide = Sides.Top;
            } else if (MathUtil.NearZero(intersection.HitPoint.Z - this.vLow.Z, 0.01)) {
                intersection.Normal.Z = -1;
                hitSide = Sides.Back;
            } else if (MathUtil.NearZero(intersection.HitPoint.Z - this.vHigh.Z, 0.01)) {
                intersection.Normal.Z = 1;
                hitSide = Sides.Front;
            }
            //if ((intersection.HitPoint.Z > this.vLow.Z - 0.0001f && intersection.HitPoint.Z < this.vLow.Z + 0.0001f))
            //{
            //    intersection.Normal.Z = -1;
            //}
            //else if ((intersection.HitPoint.Z > this.vHigh.Z - 0.0001f && intersection.HitPoint.Z < this.vHigh.Z + 0.0001f))
            //{
            //    intersection.Normal.Z = 1;
            //}
            //else if ((intersection.HitPoint.X > this.vLow.X - 0.0001f
            //         && intersection.HitPoint.X < this.vLow.X + 0.0001f))
            //{
            //    intersection.Normal.X = -1;
            //}
            //else if ((intersection.HitPoint.X > this.vHigh.X - 0.0001f
            //         && intersection.HitPoint.X < this.vHigh.X + 0.0001f))
            //{
            //    intersection.Normal.X = 1;
            //}
            //else if ((intersection.HitPoint.Y > this.vLow.Y - 0.0001f
            //         && intersection.HitPoint.Y < this.vLow.Y + 0.0001f))
            //{
            //    intersection.Normal.Y = -1;
            //}
            //else if ((intersection.HitPoint.Y > this.vHigh.Y - 0.0001f
            //         && intersection.HitPoint.Y < this.vHigh.Y + 0.0001f))
            //{
            //    intersection.Normal.Y = 1;
            //}

            #endregion

            //Vector3D normal = Vector3D.Zero;
            //switch (hitSide)
            //{
            //    case Sides.Left:
            //        normal.X = -1.0f;
            //        break;
            //    case Sides.Right:
            //        normal.X = 1.0f;
            //        break;
            //    case Sides.Top:
            //        normal.Y = 1.0f;
            //        break;
            //    case Sides.Bottom:
            //        normal.Y = -1.0f;
            //        break;
            //    case Sides.Front:
            //        normal.Z = -1.0f;
            //        break;
            //    case Sides.Back:
            //        normal.Z = 1.0f;
            //        break;
            //}
            //if(normal == intersection.Normal){
            //    Console.WriteLine("------");
            //}

            if (this.material != null && this.material.IsTexturized) {
                float u = 0, v = 0;
                switch (hitSide) {
                    case Sides.Left:
                    case Sides.Right:
                        u = (this.topCoefABC - (intersection.HitPoint * this.normalABC)) * this.inv_TopBottomABC;
                        v = ((intersection.HitPoint * this.normalABD) - this.bottomCoefABD) * this.inv_TopBottomABD;
                        if (intersection.Normal.X.IsEqual(1.0)) {
                            u = 1 - u;
                        }
                        break;
                    case Sides.Top:
                    case Sides.Bottom:
                        u = ((intersection.HitPoint * this.normalACD) - this.bottomCoefACD) * this.inv_TopBottomACD;
                        v = (this.topCoefABC - (intersection.HitPoint * this.normalABC)) * this.inv_TopBottomABC;
                        if (intersection.Normal.Y.IsEqual(-1.0)) {
                            u = 1 - u;
                        }
                        break;
                    case Sides.Front:
                    case Sides.Back:
                        u = ((intersection.HitPoint * this.normalACD) - this.bottomCoefACD) * this.inv_TopBottomACD;
                        v = ((intersection.HitPoint * this.normalABD) - this.bottomCoefABD) * this.inv_TopBottomABD;
                        if (intersection.Normal.Z.IsEqual(1.0)) {
                            u = 1 - u;
                        }
                        break;
                }
                intersection.CurrentTextureCoordinate.U = u;
                intersection.CurrentTextureCoordinate.V = v;
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(u * widthTex), (int)((v) * heightTex));
            }
            return true;
        }

        //public override bool IntersectPoint(out Intersection intersection, Ray ray) {
        //    intersection = new Intersection();
        //    float tNear = float.NegativeInfinity, tFar = float.PositiveInfinity, t1, t2;
        //    float oX = ray.Origin.X, oY = ray.Origin.Y, oZ = ray.Origin.Z;
        //    float inv_dX = ray.Inv_Direction.X, inv_dY = ray.Inv_Direction.Y, inv_dZ = ray.Inv_Direction.Z;
        //    float tX = float.NaN, tY = float.NaN, tZ = float.NaN;
        //    float temp;
        //    #region X
        //    if ((ray.Direction.X == 0)) {
        //        if ((oX < this.vLow.X || oX > this.vHigh.X)) {
        //            return false;
        //        }
        //    } else {
        //        t1 = (vLow.X - oX) * inv_dX;
        //        t2 = (vHigh.X - oX) * inv_dX;
        //        if (t1 > t2) {
        //            temp = t1;
        //            t1 = t2;
        //            t2 = temp;
        //        }
        //        if (t1 > tNear) {
        //            tNear = t1;
        //        }
        //        if (t2 < tFar) {
        //            tFar = t2;
        //        }
        //        if (tNear > tFar || tFar < 0) {
        //            return false;
        //        }
        //        tX = t1;
        //    }
        //    #endregion
        //    #region Y
        //    if ((ray.Direction.Y == 0)) {
        //        if ((oY < vLow.Y || oY > vHigh.Y)) {
        //            return false;
        //        }
        //    } else {
        //        t1 = (vLow.Y - oY) * inv_dY;
        //        t2 = (vHigh.Y - oY) * inv_dY;
        //        if (t1 > t2) {
        //            temp = t1;
        //            t1 = t2;
        //            t2 = temp;
        //        }
        //        if (t1 > tNear) {
        //            tNear = t1;
        //        }
        //        if (t2 < tFar) {
        //            tFar = t2;
        //        }
        //        if (tNear > tFar || tFar < 0) {
        //            return false;
        //        }
        //        tY = t1;
        //    }
        //    #endregion
        //    #region Z
        //    if ((ray.Direction.Z == 0)) {
        //        if ((oZ < vLow.Z || oZ > vHigh.Z)) {
        //            return false;
        //        }
        //    } else {
        //        t1 = (vLow.Z - oZ) * inv_dZ;
        //        t2 = (vHigh.Z - oZ) * inv_dZ;
        //        if (t1 > t2) {
        //            temp = t1;
        //            t1 = t2;
        //            t2 = temp;
        //        }
        //        if (t1 > tNear) {
        //            tNear = t1;
        //        }
        //        if (t2 < tFar) {
        //            tFar = t2;
        //        }
        //        if (tNear > tFar || tFar < 0) {
        //            return false;
        //        }
        //        tZ = t1;
        //    }
        //    #endregion
        //    if (tNear > 0) {
        //        intersection.TMin = tNear;
        //        intersection.TMax = tFar;
        //    } else {
        //        intersection.TMin = tFar;
        //        intersection.TMax = tNear;
        //    }
        //    Component compConst = Component.X;
        //    if (tX == intersection.TMin) {
        //        compConst = Component.X;
        //        intersection.Normal.X = (ray.Origin.X >= 0.0f) ? 1 : -1;
        //    } else if (tY == intersection.TMin) {
        //        compConst = Component.Y;
        //        intersection.Normal.Y = (ray.Origin.Y >= 0.0f) ? 1 : -1;
        //    } else if (tZ == intersection.TMin) {
        //        compConst = Component.Z;
        //        intersection.Normal.Z = (ray.Origin.Z >= 0.0f) ? 1 : -1;
        //    }
        //    intersection.Normal.Normalize();
        //    intersection.HitPoint = (ray.Origin + (intersection.TMin * ray.Direction));
        //    intersection.HitPrimitive = this;
        //    if (this.material != null && this.material.IsTexturized) {
        //        float u = 0, v = 0;
        //        switch (compConst) {
        //            case Component.X:
        //                u = (this.topCoefABC - (intersection.HitPoint * this.normalABC)) * this.inv_TopBottomABC;
        //                v = ((intersection.HitPoint * this.normalABD) - this.bottomCoefABD) * this.inv_TopBottomABD;
        //                if (intersection.Normal.X == 1) {
        //                    u = 1 - u;
        //                }
        //                break;
        //            case Component.Y:
        //                u = ((intersection.HitPoint * this.normalACD) - this.bottomCoefACD) * this.inv_TopBottomACD;
        //                v = (this.topCoefABC - (intersection.HitPoint * this.normalABC)) * this.inv_TopBottomABC;
        //                if (intersection.Normal.Y == -1) {
        //                    u = 1 - u;
        //                }
        //                break;
        //            case Component.Z:
        //                u = ((intersection.HitPoint * this.normalACD) - this.bottomCoefACD) * this.inv_TopBottomACD;
        //                v = ((intersection.HitPoint * this.normalABD) - this.bottomCoefABD) * this.inv_TopBottomABD;
        //                if (intersection.Normal.Z == 1) {
        //                    u = 1 - u;
        //                }
        //                break;
        //        }
        //        int widthTex = this.material.Texture.Width - 1;
        //        int heightTex = this.material.Texture.Height - 1;
        //        this.material.Color = RGBColoray.FromColor(this.material.Texture.GetPixel((int)(u * widthTex), (int)((v) * heightTex)));
        //    }
        //    //if (this.csgOperationType != CSGOperationType.None) {
        //    //    return base.ExecuteCSGOperation(ref intersection, ray);
        //    //}
        //    return true;
        //}
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive) {
            Vector3D normal = new Vector3D();
            if ((pointInPrimitive.Z > this.vLow.Z - 0.0001f && pointInPrimitive.Z < this.vLow.Z + 0.0001f)) {
                normal.Z = -1;
            } else if ((pointInPrimitive.Z > this.vHigh.Z - 0.0001f && pointInPrimitive.Z < this.vHigh.Z + 0.0001f)) {
                normal.Z = 1;
            } else if ((pointInPrimitive.X > this.vLow.X - 0.0001f && pointInPrimitive.X < this.vLow.X + 0.0001f)) {
                normal.X = -1;
            } else if ((pointInPrimitive.X > this.vHigh.X - 0.0001f && pointInPrimitive.X < this.vHigh.X + 0.0001f)) {
                normal.X = 1;
            } else if ((pointInPrimitive.Y > this.vLow.Y - 0.0001f && pointInPrimitive.Y < this.vLow.Y + 0.0001f)) {
                normal.Y = -1;
            } else if ((pointInPrimitive.Y > this.vHigh.Y - 0.0001f && pointInPrimitive.Y < this.vHigh.Y + 0.0001f)) {
                normal.Y = 1;
            }
            return normal;
        }

        public override bool IsInside(Point3D point) {
            return ((point.X >= this.vLow.X - 0.001f && point.X <= this.vHigh.X + 0.001f) &&
                    (point.Y >= this.vLow.Y - 0.001f && point.Y <= this.vHigh.Y + 0.001f) &&
                    (point.Z >= this.vLow.Z - 0.001f && point.Z <= this.vHigh.Z + 0.001f));
        }

        public override bool IsOverlap(BoundBox boundBox) {
            throw new NotImplementedException();
        }
    }
}