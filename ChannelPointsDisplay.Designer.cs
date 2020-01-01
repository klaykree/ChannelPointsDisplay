namespace ChannelPointsDisplay
{
    partial class ChannelPointsDisplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelPointsDisplay));
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.VideoBox = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.BackColor = System.Drawing.Color.Magenta;
            this.ImageBox.Image = ((System.Drawing.Image)(resources.GetObject("ImageBox.Image")));
            this.ImageBox.Location = new System.Drawing.Point(0, 0);
            this.ImageBox.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(923, 647);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageBox.TabIndex = 1;
            this.ImageBox.TabStop = false;
            this.ImageBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageBox_MouseDown);
            // 
            // VideoBox
            // 
            this.VideoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VideoBox.Enabled = true;
            this.VideoBox.Location = new System.Drawing.Point(0, 0);
            this.VideoBox.Name = "VideoBox";
            this.VideoBox.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("VideoBox.OcxState")));
            this.VideoBox.Size = new System.Drawing.Size(923, 647);
            this.VideoBox.TabIndex = 2;
            // 
            // ChannelPointsDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(923, 647);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.VideoBox);
            this.Name = "ChannelPointsDisplay";
            this.ShowIcon = false;
            this.Text = "ChannelPointsDisplay";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox ImageBox;
        private AxWMPLib.AxWindowsMediaPlayer VideoBox;
    }
}

