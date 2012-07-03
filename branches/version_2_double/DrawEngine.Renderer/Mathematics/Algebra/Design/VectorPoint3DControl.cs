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
        public double Increment
        {
            get { return (double)this.X.Increment; }
            set
            {
                if(value > 0.0d){
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
            this.vector.X = (double)this.X.Value;
            this.point.X = (double)this.X.Value;
        }
        private void Y_ValueChanged(object sender, EventArgs e)
        {
            this.vector.Y = (double)this.Y.Value;
            this.point.Y = (double)this.Y.Value;
        }
        private void Z_ValueChanged(object sender, EventArgs e)
        {
            this.vector.Z = (double)this.Z.Value;
            this.point.Z = (double)this.Z.Value;
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