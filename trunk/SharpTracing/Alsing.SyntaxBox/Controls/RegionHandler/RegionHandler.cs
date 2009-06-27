// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Alsing.Windows.Forms.CoreLib
{
    [ToolboxItem(true)]
    public class RegionHandler : Component
    {
        private Container components;

        #region PUBLIC PROPERTY TRANSPARENCYKEY
        private Color _TransparencyKey = Color.FromArgb(255, 0, 255);
        public Color TransparencyKey
        {
            get { return this._TransparencyKey; }
            set { this._TransparencyKey = value; }
        }
        #endregion

        #region PUBLIC PROPERTY CONTROL
        public Control Control { get; set; }
        #endregion

        #region PUBLIC PROPERTY MASKIMAGE
        public Bitmap MaskImage { get; set; }
        #endregion

        public RegionHandler(IContainer container)
        {
            container.Add(this);
            this.InitializeComponent();
        }
        public RegionHandler()
        {
            this.InitializeComponent();
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        public void ApplyRegion(Control Target, Bitmap MaskImage, Color TransparencyKey)
        {
            this.Control = Target;
            this.MaskImage = MaskImage;
            this.TransparencyKey = TransparencyKey;
            this.ApplyRegion();
        }
        public void ApplyRegion()
        {
            var r = new Region(new Rectangle(0, 0, this.MaskImage.Width, this.MaskImage.Height));
            for(int y = 0; y < this.MaskImage.Height; y++){
                for(int x = 0; x < this.MaskImage.Width; x++){
                    if(this.MaskImage.GetPixel(x, y) == this.TransparencyKey){
                        r.Exclude(new Rectangle(x, y, 1, 1));
                    }
                }
            }
            this.Control.Region = r;
            this.Control.BackgroundImage = this.MaskImage;
        }
    }
}