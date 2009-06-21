using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace DrawEngine.Renderer.Mathematics.Algebra.Design
{
    public partial class VectorPoint3DControl : UserControl
    {
        private Point3D point;
        private Vector3D vector;
        public VectorPoint3DControl()
        {
            this.InitializeComponent();
            this.Increment = 1;
        }
        [Browsable(true), Editor(typeof(VectorOrPointEditor), typeof(UITypeEditor))]
        public Vector3D Vector3D
        {
            get { return this.vector; }
            set
            {
                this.vector = value;
                this.X.Value = (decimal)this.vector.X;
                this.Y.Value = (decimal)this.vector.Y;
                this.Z.Value = (decimal)this.vector.Z;
            }
        }
        [Browsable(true), Editor(typeof(VectorOrPointEditor), typeof(UITypeEditor))]
        public Point3D Point3D
        {
            get { return this.point; }
            set
            {
                this.point = value;
                this.X.Value = (decimal)this.point.X;
                this.Y.Value = (decimal)this.point.Y;
                this.Z.Value = (decimal)this.point.Z;
            }
        }
        public float Increment
        {
            get { return (float)this.X.Increment; }
            set
            {
                if(value > 0.0f){
                    this.X.Increment = this.Y.Increment = this.Z.Increment = (decimal)value;
                }
            }
        }
        public String NameVectorPoint3D
        {
            get { return this.lblVectorName.Text; }
            set
            {
                if(value != null){
                    this.lblVectorName.Text = value;
                }
            }
        }
        private void X_ValueChanged(object sender, EventArgs e)
        {
            this.vector.X = (float)this.X.Value;
            this.point.X = (float)this.X.Value;
        }
        private void Y_ValueChanged(object sender, EventArgs e)
        {
            this.vector.Y = (float)this.Y.Value;
            this.point.Y = (float)this.Y.Value;
        }
        private void Z_ValueChanged(object sender, EventArgs e)
        {
            this.vector.Z = (float)this.Z.Value;
            this.point.Z = (float)this.Z.Value;
        }
        private void UnitX_Click(object sender, EventArgs e)
        {
            this.X.Value = 1;
        }
        private void UnitY_Click(object sender, EventArgs e)
        {
            this.Y.Value = 1;
        }
        private void UnitZ_Click(object sender, EventArgs e)
        {
            this.Z.Value = 1;
        }
        private void Zero_Click(object sender, EventArgs e)
        {
            this.X.Value = this.Y.Value = this.Z.Value = 0;
        }
    }
}