using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Algebra
{
    public interface ITransformable3D
    {
        /// <summary>
        /// Rotate angle� in clockwise, around an arbitrary axis
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="axis">Arbitrary axis</param>
        void Rotate(double angle, Vector3D axis);
        /// <summary>
        /// Rotate angle� in clockwise in axis X
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        void RotateAxisX(double angle);
        /// <summary>
        /// Rotate angle� in clockwise in axis Y
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        void RotateAxisY(double angle);
        /// <summary>
        /// Rotate angle� in clockwise in axis Z
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        void RotateAxisZ(double angle);
        /// <summary>
        /// Scale in the same proportions in x, y and z, axis
        /// </summary>
        /// <param name="factor">Factor to scale</param>
        void Scale(double factor);
        /// <summary>
        /// Translate in X, Y and Z axis 
        /// </summary>
        /// <param name="tx">Factor to translate in X</param>
        /// <param name="ty">Factor to translate in Y</param>
        /// <param name="tz">Factor to translate in Z</param>
        void Translate(double tx, double ty, double tz);
        void Translate(Vector3D translateVector);
        //void Transform(Matrix3D matrix);
    }
}