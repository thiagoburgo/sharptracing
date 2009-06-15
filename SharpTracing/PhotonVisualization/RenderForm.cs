using System;
using System.Drawing;
using System.Windows.Forms;
using DrawEngine.Renderer;
using DrawEngine.Renderer.PhotonMapping;
using DrawEngine.Renderer.Tracers;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace PhotonVisualization
{
    public partial class RenderForm : Form
    {
        // The Direct3D device.
        private Device m_Device;

        #region "D3D Setup Code"
        // Data variables.
        // The vertex buffer that holds drawing data.
        private VertexBuffer m_VertexBuffer = null;
        private int NUM_POINTS = 0;
        // Initialize the graphics device. Return True if successful.
        public bool InitializeGraphics()
        {
            PresentParameters parms = new PresentParameters();
            parms.Windowed = true;
            parms.SwapEffect = SwapEffect.Discard;
            parms.EnableAutoDepthStencil = true; // Depth stencil on.
            parms.AutoDepthStencilFormat = DepthFormat.D16;
            // Best: Hardware device and hardware vertex processing.
            try{
                this.m_Device = new Device(0, DeviceType.Hardware, this.pic3d, CreateFlags.HardwareVertexProcessing,
                                           parms);
            } catch{}
            // Good: Hardware device and software vertex processing.
            if(this.m_Device == null){
                try{
                    this.m_Device = new Device(0, DeviceType.Hardware, this.pic3d, CreateFlags.SoftwareVertexProcessing,
                                               parms);
                } catch{}
            }
            // Adequate?: Software device and software vertex processing.
            if(this.m_Device == null){
                try{
                    this.m_Device = new Device(0, DeviceType.Hardware, this.pic3d, CreateFlags.HardwareVertexProcessing,
                                               parms);
                } catch(Exception ex){
                    // If we still can't make a device, give up.
                    MessageBox.Show("Error initializing Direct3D\n\n" + ex.Message, "Direct3D Error",
                                    MessageBoxButtons.OK);
                    return false;
                }
            }
            // Turn off D3D lighting because 
            // we set the vertex colors explicitly.
            this.m_Device.RenderState.Lighting = false;
            // Turn on the Z-buffer.
            this.m_Device.RenderState.ZBufferEnable = true;
            // Cull triangles that are oriented counter clockwise.
            this.m_Device.RenderState.CullMode = Cull.CounterClockwise;
            // Make points bigger so they're easy to see.
            this.m_Device.RenderState.PointSize = 1;
            // Start in solid mode.
            this.m_Device.RenderState.FillMode = FillMode.Point;
            // Create the vertex data.
            this.CreateVertexBuffer();
            // We succeeded.
            return true;
        }
        // Create a vertex buffer for the device.
        public void CreateVertexBuffer()
        {
            String sceneName = @"D:\Models & Textures\cornellBox2.1.xml";
            //OpenFileDialog dialog = new OpenFileDialog();
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    sceneName = dialog.FileName;
            //}
            PhotonTracer tracer = new PhotonTracer(Scene.Load(sceneName), 100000);
            tracer.Render(null);

            Photon[] photons = tracer.IndirectEnlightenment.Photons;
            this.NUM_POINTS = photons.Length;
            // Create a buffer.
            this.m_VertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), this.NUM_POINTS, this.m_Device,
                                                   0, CustomVertex.PositionColored.Format, Pool.Default);
            // Lock the vertex buffer. 
            // Lock returns an array of positionColored objects.
            CustomVertex.PositionColored[] vertices = (CustomVertex.PositionColored[])this.m_VertexBuffer.Lock(0, 0);
            for(int i = 1; i < photons.Length; i++){
                vertices[i].X = photons[i].Position.X;
                vertices[i].Y = photons[i].Position.Y;
                vertices[i].Z = photons[i].Position.Z;
                //Color color = photons[i].Power.ToColor();
                //int argb = 0;
                //argb = argb | color.A;
                //argb |= color.B << 8;
                ////argb += color.B;
                //argb |= color.G << 16;
                ////argb += color.G;
                //argb |= color.R << 24;
                ////argb += color.R;
                vertices[i].Color = photons[i].Power.ToColor().ToArgb();
            }
            this.m_VertexBuffer.Unlock();
        }
        #endregion // D3D Setup Code

        #region "D3D Drawing Code"
        // Draw.
        public void Render()
        {
            // Clear the back buffer and the Z-buffer.
            this.m_Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1, 0);
            // Make a scene.
            this.m_Device.BeginScene();
            // Draw stuff here...
            // Setup the world, view, and projection matrices.
            this.SetupMatrices();
            // Set the device//s data stream source (the vertex buffer).
            this.m_Device.SetStreamSource(0, this.m_VertexBuffer, 0);
            // Tell the device the format of the vertices.
            this.m_Device.VertexFormat = CustomVertex.PositionColored.Format;
            // Draw the primitives in the data stream.
            this.m_Device.DrawPrimitives(PrimitiveType.PointList, 0, this.NUM_POINTS);
            // End the scene and display.
            this.m_Device.EndScene();
            this.m_Device.Present();
        }
        // Setup the world, view, and projection matrices.
        private void SetupMatrices()
        {
            // World Matrix:
            //const int TICKS_PER_REV = 4000;
            //double angle = Environment.TickCount * (2 * Math.PI) / TICKS_PER_REV;
            //m_Device.Transform.World = Matrix.RotationAxis(new Vector3(1f, 1f, 1f), (float)angle);
            // View Matrix:
            this.m_Device.Transform.View = Matrix.LookAtLH(new Vector3(0, 20, -150), new Vector3(0, 0, 0),
                                                           new Vector3(0, 1, 0));
            // Projection Matrix:
            // Perspective transformation defined by:
            //       Field of view           Pi / 4
            //       Aspect ratio            1
            //       Near clipping plane     Z = 1
            //       Far clipping plane      Z = 100
            this.m_Device.Transform.Projection = Matrix.PerspectiveFovLH(1f, 1, 0, 300);
        }
        #endregion // D3D Drawing Code

        public RenderForm()
        {
            this.InitializeComponent();
        }
        private void RenderForm_Resize(object sender, EventArgs e)
        {
            this.InitializeGraphics();
        }
    }
}