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
using System.Diagnostics;
using System.Text;
using DrawEngine.Renderer.Mathematics.Algebra;

// NOTE.  The (x,y,z) coordinate system is assumed to be right-handed.
// Coordinate axis rotation matrices are of the form
//   RX =    1       0       0
//           0     cos(t) -sin(t)
//           0     sin(t)  cos(t)
// where t > 0 indicates a counterclockwise rotation in the yz-plane
//   RY =  cos(t)    0     sin(t)
//           0       1       0
//        -sin(t)    0     cos(t)
// where t > 0 indicates a counterclockwise rotation in the zx-plane
//   RZ =  cos(t) -sin(t)    0
//         sin(t)  cos(t)    0
//           0       0       1
// where t > 0 indicates a counterclockwise rotation in the xy-plane.

namespace DrawEngine.Renderer.Algebra
{
    /// <summary>
    /// A 3x3 matrix which can represent rotations around axes.
    /// </summary>
    public struct Matrix3D
    {
        #region Member variables and constants
        private static readonly Matrix3D identityMatrix = new Matrix3D(1, 0, 0, 0, 1, 0, 0, 0, 1);
        private static readonly Matrix3D zeroMatrix = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0);
        public float M00, M01, M02;
        public float M10, M11, M12;
        public float M20, M21, M22;
        #endregion

        #region Constructors
        /// <summary>
        ///		Creates a new Matrix3D with all the specified parameters.
        /// </summary>
        public Matrix3D(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21,
                        float m22)
        {
            this.M00 = m00;
            this.M01 = m01;
            this.M02 = m02;
            this.M10 = m10;
            this.M11 = m11;
            this.M12 = m12;
            this.M20 = m20;
            this.M21 = m21;
            this.M22 = m22;
        }
        /// <summary>
        /// Create a new Matrix from 3 Vertex3 objects.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public Matrix3D(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
        {
            this.M00 = xAxis.X;
            this.M01 = yAxis.X;
            this.M02 = zAxis.X;
            this.M10 = xAxis.Y;
            this.M11 = yAxis.Y;
            this.M12 = zAxis.Y;
            this.M20 = xAxis.Z;
            this.M21 = yAxis.Z;
            this.M22 = zAxis.Z;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Identity Matrix
        /// </summary>
        public static Matrix3D Identity
        {
            get { return identityMatrix; }
        }
        /// <summary>
        /// Zero matrix.
        /// </summary>
        public static Matrix3D Zero
        {
            get { return zeroMatrix; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix3D Transpose()
        {
            return new Matrix3D(this.M00, this.M10, this.M20, this.M01, this.M11, this.M21, this.M02, this.M12, this.M22);
        }
        /// <summary>
        ///		Gets a matrix column by index.
        /// </summary>
        /// <param name="col"></param>
        /// <returns>A Vector3D representing one of the Matrix columns.</returns>
        public Vector3D GetColumn(int col)
        {
            Debug.Assert(col >= 0 && col < 3, "Attempt to retreive a column of a Matrix3D greater than 2.");
            unsafe{
                fixed(float* pM = &this.M00){
                    return new Vector3D(*(pM + col), //m[0,col], 
                                        *(pM + 3 + col), //m[1,col], 
                                        *(pM + 6 + col)); //m[2,col]);
                }
            }
        }
        /// <summary>
        ///		Sets one of the columns of the Matrix with a Vector3D.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public void SetColumn(int col, Vector3D vector)
        {
            Debug.Assert(col >= 0 && col < 3, "Attempt to set a column of a Matrix3D greater than 2.");
            this[0, col] = vector.X;
            this[1, col] = vector.Y;
            this[2, col] = vector.Z;
        }
        /// <summary>
        ///		Creates a Matrix3D from 3 axes.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public void FromAxes(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
        {
            this.SetColumn(0, xAxis);
            this.SetColumn(1, yAxis);
            this.SetColumn(2, zAxis);
        }
        /// <summary>
        ///    Constructs this Matrix from 3 euler angles, in degrees.
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        public void FromEulerAnglesXYZ(float yaw, float pitch, float roll)
        {
            double cos = Math.Cos(yaw);
            double sin = Math.Sin(yaw);
            Matrix3D xMat = new Matrix3D(1, 0, 0, 0, (float)cos, (float)-sin, 0, (float)sin, (float)cos);
            cos = Math.Cos(pitch);
            sin = Math.Sin(pitch);
            Matrix3D yMat = new Matrix3D((float)cos, 0, (float)sin, 0, 1, 0, (float)-sin, 0, (float)cos);
            cos = Math.Cos(roll);
            sin = Math.Sin(roll);
            Matrix3D zMat = new Matrix3D((float)cos, (float)-sin, 0, (float)sin, (float)cos, 0, 0, 0, 1);
            this = xMat * (yMat * zMat);
        }
        #endregion

        #region Operator overloads + CLS complient method equivalents
        /// <summary>
        /// Indexer for accessing the matrix like a 2d array (i.e. matrix[2,3]).
        /// </summary>
        public float this[int row, int col]
        {
            get
            {
                //Debug.Assert((row >= 0 && row < 3) && (col >= 0 && col < 3), "Attempt to access Matrix3D indexer out of bounds.");
                unsafe{
                    fixed(float* pM = &this.M00){
                        return *(pM + ((3 * row) + col));
                    }
                }
            }
            set
            {
                //Debug.Assert((row >= 0 && row < 3) && (col >= 0 && col < 3), "Attempt to access Matrix3D indexer out of bounds.");
                unsafe{
                    fixed(float* pM = &this.M00){
                        *(pM + ((3 * row) + col)) = value;
                    }
                }
            }
        }
        /// <summary>
        ///		Allows the Matrix to be accessed linearly (m[0] -> m[8]).  
        /// </summary>
        public float this[int index]
        {
            get
            {
                //Debug.Assert(index >= 0 && index <= 8, "Attempt to access Matrix4D linear indexer out of bounds.");
                unsafe{
                    fixed(float* pMatrix = &this.M00){
                        return *(pMatrix + index);
                    }
                }
            }
            set
            {
                //Debug.Assert(index >= 0 && index <= 8, "Attempt to access Matrix4D linear indexer out of bounds.");
                unsafe{
                    fixed(float* pMatrix = &this.M00){
                        *(pMatrix + index) = value;
                    }
                }
            }
        }
        /// <summary>
        /// Multiply (concatenate) two Matrix3D instances together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3D Multiply(Matrix3D left, Matrix3D right)
        {
            return left * right;
        }
        /// <summary>
        /// Multiply (concatenate) two Matrix3D instances together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3D operator *(Matrix3D left, Matrix3D right)
        {
            Matrix3D result = new Matrix3D();
            result.M00 = left.M00 * right.M00 + left.M01 * right.M10 + left.M02 * right.M20;
            result.M01 = left.M00 * right.M01 + left.M01 * right.M11 + left.M02 * right.M21;
            result.M02 = left.M00 * right.M02 + left.M01 * right.M12 + left.M02 * right.M22;
            result.M10 = left.M10 * right.M00 + left.M11 * right.M10 + left.M12 * right.M20;
            result.M11 = left.M10 * right.M01 + left.M11 * right.M11 + left.M12 * right.M21;
            result.M12 = left.M10 * right.M02 + left.M11 * right.M12 + left.M12 * right.M22;
            result.M20 = left.M20 * right.M00 + left.M21 * right.M10 + left.M22 * right.M20;
            result.M21 = left.M20 * right.M01 + left.M21 * right.M11 + left.M22 * right.M21;
            result.M22 = left.M20 * right.M02 + left.M21 * right.M12 + left.M22 * right.M22;
            return result;
        }
        /// <summary>
        ///		vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D Multiply(Vector3D vector, Matrix3D matrix)
        {
            return vector * matrix;
        }
        public static Point3D Multiply(Point3D point, Matrix3D matrix)
        {
            return point * matrix;
        }
        /// <summary>
        ///		vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D vector, Matrix3D matrix)
        {
            Vector3D product = new Vector3D();
            product.X = matrix.M00 * vector.X + matrix.M01 * vector.Y + matrix.M02 * vector.Z;
            product.Y = matrix.M10 * vector.X + matrix.M11 * vector.Y + matrix.M12 * vector.Z;
            product.Z = matrix.M20 * vector.X + matrix.M21 * vector.Y + matrix.M22 * vector.Z;
            return product;
        }
        public static Point3D operator *(Point3D point, Matrix3D matrix)
        {
            Point3D product = new Point3D();
            product.X = matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z;
            product.Y = matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z;
            product.Z = matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z;
            return product;
        }
        /// <summary>
        ///		matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D Multiply(Matrix3D matrix, Vector3D vector)
        {
            return (matrix * vector);
        }
        public static Point3D Multiply(Matrix3D matrix, Point3D point)
        {
            return (matrix * point);
        }
        /// <summary>
        ///		matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D operator *(Matrix3D matrix, Vector3D vector)
        {
            Vector3D product = new Vector3D();
            product.X = matrix.M00 * vector.X + matrix.M01 * vector.Y + matrix.M02 * vector.Z;
            product.Y = matrix.M10 * vector.X + matrix.M11 * vector.Y + matrix.M12 * vector.Z;
            product.Z = matrix.M20 * vector.X + matrix.M21 * vector.Y + matrix.M22 * vector.Z;
            return product;
        }
        public static Point3D operator *(Matrix3D matrix, Point3D point)
        {
            Point3D product = new Point3D();
            product.X = matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z;
            product.Y = matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z;
            product.Z = matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z;
            return product;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3D by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3D Multiply(Matrix3D matrix, float scalar)
        {
            return matrix * scalar;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3D by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3D operator *(Matrix3D matrix, float scalar)
        {
            Matrix3D result = new Matrix3D();
            result.M00 = matrix.M00 * scalar;
            result.M01 = matrix.M01 * scalar;
            result.M02 = matrix.M02 * scalar;
            result.M10 = matrix.M10 * scalar;
            result.M11 = matrix.M11 * scalar;
            result.M12 = matrix.M12 * scalar;
            result.M20 = matrix.M20 * scalar;
            result.M21 = matrix.M21 * scalar;
            result.M22 = matrix.M22 * scalar;
            return result;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3D by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3D Multiply(float scalar, Matrix3D matrix)
        {
            return scalar * matrix;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3D by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3D operator *(float scalar, Matrix3D matrix)
        {
            Matrix3D result = new Matrix3D();
            result.M00 = matrix.M00 * scalar;
            result.M01 = matrix.M01 * scalar;
            result.M02 = matrix.M02 * scalar;
            result.M10 = matrix.M10 * scalar;
            result.M11 = matrix.M11 * scalar;
            result.M12 = matrix.M12 * scalar;
            result.M20 = matrix.M20 * scalar;
            result.M21 = matrix.M21 * scalar;
            result.M22 = matrix.M22 * scalar;
            return result;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3D Add(Matrix3D left, Matrix3D right)
        {
            return left + right;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3D operator +(Matrix3D left, Matrix3D right)
        {
            Matrix3D result = new Matrix3D();
            for(int row = 0; row < 3; row++){
                for(int col = 0; col < 3; col++){
                    result[row, col] = left[row, col] + right[row, col];
                }
            }
            return result;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3D Subtract(Matrix3D left, Matrix3D right)
        {
            return left - right;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3D operator -(Matrix3D left, Matrix3D right)
        {
            Matrix3D result = new Matrix3D();
            for(int row = 0; row < 3; row++){
                for(int col = 0; col < 3; col++){
                    result[row, col] = left[row, col] - right[row, col];
                }
            }
            return result;
        }
        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3D Negate(Matrix3D matrix)
        {
            return -matrix;
        }
        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3D operator -(Matrix3D matrix)
        {
            Matrix3D result = new Matrix3D();
            result.M00 = -matrix.M00;
            result.M01 = -matrix.M01;
            result.M02 = -matrix.M02;
            result.M10 = -matrix.M10;
            result.M11 = -matrix.M11;
            result.M12 = -matrix.M12;
            result.M20 = -matrix.M20;
            result.M21 = -matrix.M21;
            result.M22 = -matrix.M22;
            return result;
        }
        /// <summary>
        /// 	Test two matrices for (value) equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix3D left, Matrix3D right)
        {
            if(left.M00 == right.M00 && left.M01 == right.M01 && left.M02 == right.M02 && left.M10 == right.M10
               && left.M11 == right.M11 && left.M12 == right.M12 && left.M20 == right.M20 && left.M21 == right.M21
               && left.M22 == right.M22){
                return true;
            }
            return false;
        }
        public static bool operator !=(Matrix3D left, Matrix3D right)
        {
            return !(left == right);
        }
        #endregion

        #region Properties
        public float Determinant
        {
            get
            {
                float cofactor00 = this.M11 * this.M22 - this.M12 * this.M21;
                float cofactor10 = this.M12 * this.M20 - this.M10 * this.M22;
                float cofactor20 = this.M10 * this.M21 - this.M11 * this.M20;
                float result = this.M00 * cofactor00 + this.M01 * cofactor10 + this.M02 * cofactor20;
                return result;
            }
        }
        #endregion Properties

        #region Object overloads
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Matrix4D.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(" | {0} {1} {2} |\n", this.M00, this.M01, this.M02);
            builder.AppendFormat(" | {0} {1} {2} |\n", this.M10, this.M11, this.M12);
            builder.AppendFormat(" | {0} {1} {2} |", this.M20, this.M21, this.M22);
            return builder.ToString();
        }
        /// <summary>
        ///		Provides a unique hash code based on the member variables of this
        ///		class.  This should be done because the equality operators (==, !=)
        ///		have been overriden by this class.
        ///		<p/>
        ///		The standard implementation is a simple XOR operation between all local
        ///		member variables.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            unsafe{
                fixed(float* pM = &this.M00){
                    for(int i = 0; i < 9; i++){
                        hashCode ^= (int)(*(pM + i));
                    }
                }
                return hashCode;
            }
        }
        /// <summary>
        ///		Compares this Matrix to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is Matrix3D){
                return (this == (Matrix3D)obj);
            } else{
                return false;
            }
        }
        #endregion
    }
}