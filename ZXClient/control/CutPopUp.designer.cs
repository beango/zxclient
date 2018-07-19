namespace ZXClient.control
{
    partial class CutPopUp
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
            if (disposing && (components != null))
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
            this.components = new System.ComponentModel.Container();
            this.OptionsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OptionsMenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.CutPicture = new System.Windows.Forms.PictureBox();
            this.CutOverlay = new ZXClient.control.EventControls.EventReceiverPictureBox();
            this.OptionsContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CutPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CutOverlay)).BeginInit();
            this.SuspendLayout();
            // 
            // OptionsContextMenu
            // 
            this.OptionsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OptionsMenuSave});
            this.OptionsContextMenu.Name = "CutOptions";
            this.OptionsContextMenu.Size = new System.Drawing.Size(137, 26);
            // 
            // OptionsMenuSave
            // 
            this.OptionsMenuSave.Name = "OptionsMenuSave";
            this.OptionsMenuSave.Size = new System.Drawing.Size(136, 22);
            this.OptionsMenuSave.Text = "截屏并发送";
            this.OptionsMenuSave.Click += new System.EventHandler(this.OptionsMenuSave_Click);
            // 
            // CutPicture
            // 
            this.CutPicture.BackColor = System.Drawing.Color.Fuchsia;
            this.CutPicture.Location = new System.Drawing.Point(1, 1);
            this.CutPicture.Name = "CutPicture";
            this.CutPicture.Size = new System.Drawing.Size(4, 4);
            this.CutPicture.TabIndex = 0;
            this.CutPicture.TabStop = false;
            // 
            // CutOverlay
            // 
            this.CutOverlay.BackColor = System.Drawing.Color.Red;
            this.CutOverlay.ContextMenuStrip = this.OptionsContextMenu;
            this.CutOverlay.Cursor = System.Windows.Forms.Cursors.Cross;
            this.CutOverlay.Location = new System.Drawing.Point(2, 2);
            this.CutOverlay.Name = "CutOverlay";
            this.CutOverlay.Size = new System.Drawing.Size(2, 2);
            this.CutOverlay.TabIndex = 1;
            this.CutOverlay.TabStop = false;
            this.CutOverlay.Paint += new System.Windows.Forms.PaintEventHandler(this.CutOverlay_Paint);
            this.CutOverlay.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CutOverlay_MouseDoubleClick);
            this.CutOverlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CutOverlay_MouseDown);
            this.CutOverlay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CutOverlay_MouseMove);
            this.CutOverlay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CutOverlay_MouseUp);
            // 
            // CutPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(600, 554);
            this.Controls.Add(this.CutOverlay);
            this.Controls.Add(this.CutPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CutPopUp";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ScreenCut";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Deactivate += new System.EventHandler(this.CutPopUp_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CutPopUp_FormClosing);
            this.Load += new System.EventHandler(this.CutPopUp_Load);
            this.OptionsContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CutPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CutOverlay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox CutPicture;
        private System.Windows.Forms.ContextMenuStrip OptionsContextMenu;
        private ZXClient.control.EventControls.EventReceiverPictureBox CutOverlay;
        private System.Windows.Forms.ToolStripMenuItem OptionsMenuSave;
    }
}