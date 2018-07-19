using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using ZXClient.control.EventControls;
using ZXClient.Properties;

namespace ZXClient.control
{
    public delegate void ScreenCaptured(object sender, CutEventArgs e);

    public partial class CutPopUp : Form
    {
        const int CLOSE_HOTKEY_ID = 2, CAPTURE_HOTKEY_ID = 3;
        const int CLOSE_HOTKEY_KEY = (int)Keys.Escape, CAPTURE_HOTKEY_KEY = (int)Keys.Enter;
        const int REGION_SIZE = 24, REGION_WIDTH = 4;

        bool selection_resizing = false, selection_creating = false, selection_moving = false, redraw = false;
        int selection_region_clicked = -1;
        float zoom_factor = 1;
        Screen screen_on_focus;
        Bitmap capture_bmp;
        Graphics overlay_gfx;
        Rectangle selection;
        Rectangle[] mouse_regions;
        Point creating_origin, moving_offset;
        EventSenderPictureBox cTopLeft, cTopRight, cBottomRight, cBottomLeft, hHighlight;
        EventIntermediaryPictureBox hContainer;
        Form workForm;

        public CutPopUp(Bitmap screen_bmp, Screen dest_scr, Rectangle previous_selection, Form workForm)
        {
            this.workForm = workForm;
            mouse_regions = new Rectangle[4] {
                Rectangle.Empty,
                Rectangle.Empty,
                Rectangle.Empty,
                Rectangle.Empty
            };
            capture_bmp = screen_bmp;
            screen_on_focus = dest_scr;
            selection = previous_selection;

            InitializeComponent();

            this.Icon = Resources.Icon;
            this.DoubleBuffered = true;
            if (
                selection.Right > screen_on_focus.Bounds.Width ||
                selection.Left > screen_on_focus.Bounds.Width ||
                selection.Bottom > screen_on_focus.Bounds.Height ||
                selection.Top > screen_on_focus.Bounds.Height ||
                selection.Width <= 0 ||
                selection.Height <= 0
            )
            {
                selection = new Rectangle((screen_on_focus.Bounds.Width - 128) / 2, (screen_on_focus.Bounds.Height - 128) / 2, 128, 128);
            }
            SetMouseRegions();
            
            this.Location = screen_on_focus.WorkingArea.Location;
            this.Width = screen_on_focus.Bounds.Width;
            this.Height = screen_on_focus.Bounds.Height;

            _doubleClickTimer = new Timer();
            _doubleClickTimer.Interval = 100;
            _doubleClickTimer.Tick += new EventHandler(_doubleClickTimer_Tick);
        }

        public event ScreenCaptured OnCut;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case CLOSE_HOTKEY_ID:
                        this.workForm.Show();
                        this.workForm.Focus();
                        this.Close();
                        break;
                    case CAPTURE_HOTKEY_ID:
                        CutSelection();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void InitializeCaptureScreen()
        {
            Imports.user32.NativeMethods.RegisterHotKey(this.Handle, CLOSE_HOTKEY_ID, 0, CLOSE_HOTKEY_KEY);
            Imports.user32.NativeMethods.RegisterHotKey(this.Handle, CAPTURE_HOTKEY_ID, 0, CAPTURE_HOTKEY_KEY);

            CutPicture.Top = 0;
            CutPicture.Left = 0;
            CutPicture.Width = this.Width;
            CutPicture.Height = this.Height;
            CutPicture.Image = capture_bmp;
            CutPicture.Controls.Add(CutOverlay);

            CutOverlay.Top = 0;
            CutOverlay.Left = 0;
            CutOverlay.Width = CutPicture.Width;
            CutOverlay.Height = CutPicture.Height;
            CutOverlay.BackColor = Color.FromArgb(157, Color.Black);
            CutOverlay.Image = new Bitmap(CutOverlay.Width, CutOverlay.Height);

            overlay_gfx = Graphics.FromImage(CutOverlay.Image);
            overlay_gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
            overlay_gfx.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            overlay_gfx.SmoothingMode = SmoothingMode.HighSpeed;
            

            hContainer = new EventIntermediaryPictureBox(CutOverlay)
            {
                Name = "hContainer",
                Location = new Point(0, 0),
                Size = new Size(10, 10),
                BackColor = Color.Red
            };
            CutOverlay.Controls.Add(hContainer);

            hHighlight = new EventSenderPictureBox(hContainer)
            {
                Name = "hHighlight",
                Location = new Point(0, 0),
                Size = new Size(this.Width, this.Height),
                BackColor = Color.Black,
                Image = capture_bmp
            };
            hContainer.Controls.Add(hHighlight);

            cTopLeft = new EventSenderPictureBox(CutOverlay)
            {
                Name = "cTopLeft",
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                BackColor = Color.Transparent,
                Image = Resources.TopLeft
            };
            cTopRight = new EventSenderPictureBox(CutOverlay)
            {
                Name = "cTopRight",
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                BackColor = Color.Transparent,
                Image = Resources.TopRight
            };
            cBottomRight = new EventSenderPictureBox(CutOverlay)
            {
                Name = "cBottomRight",
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                BackColor = Color.Transparent,
                Image = Resources.BottomRight
            };
            cBottomLeft = new EventSenderPictureBox(CutOverlay)
            {
                Name = "cBottomLeft",
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                BackColor = Color.Transparent,
                Image = Resources.BottomLeft
            };

            cTopLeft.MouseEnter += Region_MouseEnter;
            cTopRight.MouseEnter += Region_MouseEnter;
            cBottomRight.MouseEnter += Region_MouseEnter;
            cBottomLeft.MouseEnter += Region_MouseEnter;
            cTopLeft.MouseLeave += Region_MouseLeave;
            cTopRight.MouseLeave += Region_MouseLeave;
            cBottomRight.MouseLeave += Region_MouseLeave;
            cBottomLeft.MouseLeave += Region_MouseLeave;

            CutOverlay.Controls.Add(cTopLeft);
            CutOverlay.Controls.Add(cTopRight);
            CutOverlay.Controls.Add(cBottomRight);
            CutOverlay.Controls.Add(cBottomLeft);
           
            UpdateOverlay();
        }

        

        private void SetMouseRegions()
        {
            mouse_regions[0].Location = new Point(selection.Left - REGION_WIDTH, selection.Top - REGION_WIDTH);
            mouse_regions[1].Location = new Point(selection.Right - REGION_SIZE + REGION_WIDTH, selection.Top - REGION_WIDTH);
            mouse_regions[2].Location = new Point(selection.Right - REGION_SIZE + REGION_WIDTH, selection.Bottom - REGION_SIZE + REGION_WIDTH);
            mouse_regions[3].Location = new Point(selection.Left - REGION_WIDTH, selection.Bottom - REGION_SIZE + REGION_WIDTH);

            mouse_regions[0].Size = new Size(REGION_SIZE, REGION_SIZE);
            mouse_regions[1].Size = new Size(REGION_SIZE, REGION_SIZE);
            mouse_regions[2].Size = new Size(REGION_SIZE, REGION_SIZE);
            mouse_regions[3].Size = new Size(REGION_SIZE, REGION_SIZE);
        }

        private void UpdateOverlay()
        {
            CutOverlay.Refresh();
            redraw = true;
        }

        private string saveCutImg(Bitmap bm)
        {
            string dir = "CutImg";//System.AppDomain.CurrentDomain.BaseDirectory + 
            string imgName = string.Format(dir + @"\captureimg.{0:yyyyMMddHHmmss.ffff}.jpg", DateTime.Now);
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            bm.Save(imgName, ImageFormat.Jpeg);
            return imgName;
        }

        private void CutSelection()
        {
            try
            {
                Bitmap cut_bmp = null;
                ProcessBitmap(ref cut_bmp);
                //Clipboard.SetImage(cut_bmp);
                string imageName = saveCutImg(cut_bmp);
                OnCut(this, new CutEventArgs(selection, imageName));
                this.Close();
            }
            catch (Exception)
            {
                
            }
        }

        private void SaveSelection()
        {
            Bitmap cut_bmp = null;
            ProcessBitmap(ref cut_bmp);
            string imageName = saveCutImg(cut_bmp);
            OnCut(this, new CutEventArgs(selection, imageName));
            this.Close();

            //SaveFileDialog save_box = new SaveFileDialog();
            //save_box.Title = "Salvar Como...";
            //save_box.Filter = "PNG|*.png|JPEG|*.jpg|GIF|*.gif|BMP|*.bmp";

            //this.Deactivate -= CutPopUp_Deactivate;
            //save_box.ShowDialog();
            //this.Deactivate += CutPopUp_Deactivate;

            //if (save_box.FileName != "")
            //{
            //    FileStream fs = (FileStream)save_box.OpenFile();
            //    switch (save_box.FilterIndex)
            //    {
            //        case 1:
            //            cut_bmp.Save(fs, ImageFormat.Png);
            //            break;
            //        case 2:
            //            cut_bmp.Save(fs, ImageFormat.Jpeg);
            //            break;
            //        case 3:
            //            cut_bmp.Save(fs, ImageFormat.Gif);
            //            break;
            //        case 4:
            //            cut_bmp.Save(fs, ImageFormat.Bmp);
            //            break;
            //    }
            //    fs.Close();

            //    OnCut(this, new CutEventArgs(selection));
            //    this.Close();
            //}
            //else
            //{
            //    save_box.Dispose();
            //}
        }

        private void ProcessBitmap(ref Bitmap cut_bmp)
        {
            using (Bitmap cut_region = capture_bmp.Clone(selection, capture_bmp.PixelFormat))
            {
                if (zoom_factor != 1)
                {
                    int zwidth = (int)(cut_region.Width * zoom_factor);
                    int zheight = (int)(cut_region.Height * zoom_factor);

                    using (Bitmap zoomed = new Bitmap(zwidth, zheight))
                    {
                        zoomed.SetResolution(cut_region.HorizontalResolution, cut_region.VerticalResolution);
                        using (Graphics zoomed_gfx = Graphics.FromImage(zoomed))
                        {
                            zoomed_gfx.CompositingMode = CompositingMode.SourceCopy;
                            zoomed_gfx.CompositingQuality = CompositingQuality.HighQuality;
                            zoomed_gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            zoomed_gfx.SmoothingMode = SmoothingMode.HighQuality;
                            zoomed_gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            using (ImageAttributes wm = new ImageAttributes())
                            {
                                wm.SetWrapMode(WrapMode.TileFlipXY);
                                zoomed_gfx.DrawImage(cut_region, new Rectangle(0, 0, zwidth, zheight), 0, 0, cut_region.Width, cut_region.Height, GraphicsUnit.Pixel, wm);
                            }
                        }

                        cut_bmp = (Bitmap)zoomed.Clone();
                        return;
                    }
                }

                cut_bmp = (Bitmap)cut_region.Clone();
                return;
            }
        }

        private void CutPopUp_Load(object sender, EventArgs e)
        {
            InitializeCaptureScreen();
        }

        private void CutOverlay_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("鼠标双击:111111111111111111");
            SaveSelection();
        }

        private void CutPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            overlay_gfx.Dispose();
            CutOverlay.Image.Dispose();
            capture_bmp.Dispose();

            Imports.user32.NativeMethods.UnregisterHotKey(this.Handle, CLOSE_HOTKEY_ID);
            Imports.user32.NativeMethods.UnregisterHotKey(this.Handle, CAPTURE_HOTKEY_ID);
        }

        private void CutPopUp_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void CutOverlay_Paint(object sender, PaintEventArgs e)
        {
            if (redraw)
            {
                SuspendLayout();

                hContainer.Location = new Point(selection.Left, selection.Top);
                hContainer.Size = new Size(selection.Width, selection.Height);
                hHighlight.Location = new Point(0 - selection.Left, 0 - selection.Top);

                cTopLeft.Location = new Point(selection.Left - REGION_WIDTH, selection.Top - REGION_WIDTH);
                cTopRight.Location = new Point(selection.Right - REGION_SIZE + REGION_WIDTH, selection.Top - REGION_WIDTH);
                cBottomRight.Location = new Point(selection.Right - REGION_SIZE + REGION_WIDTH, selection.Bottom - REGION_SIZE + REGION_WIDTH);
                cBottomLeft.Location = new Point(selection.Left - REGION_WIDTH, selection.Bottom - REGION_SIZE + REGION_WIDTH);

                ResumeLayout();
                redraw = false;
            }
        }
        private bool _isFirstClick = true;
        private bool _isDoubleClick = false;
        private int _milliseconds = 0;
        private Timer _doubleClickTimer;
        private Rectangle _doubleRec;

        //_doubleClickTimer的Tick事件
        private void _doubleClickTimer_Tick(object sender, EventArgs e)
        {
            _milliseconds += 100;
            if (_milliseconds >= SystemInformation.DoubleClickTime)
            {
                _doubleClickTimer.Stop();
                if (_isDoubleClick)
                {
                    CutSelection();
                }
                else
                {
                }
                _isDoubleClick = false;
                _isFirstClick = true;
                _milliseconds = 0;
            }
        }

        private void CutOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isFirstClick)
            {
                _doubleRec = new Rectangle(e.X - SystemInformation.DoubleClickSize.Width / 2,
                    e.Y - SystemInformation.DoubleClickSize.Height / 2,
                    SystemInformation.DoubleClickSize.Width,
                    SystemInformation.DoubleClickSize.Height);
                _isFirstClick = false;
                _doubleClickTimer.Start();
            }
            else
            {
                if (_doubleRec.Contains(e.Location))
                {
                    _isDoubleClick = true;
                }
            }
            
            if (e.Button == MouseButtons.Left && (ModifierKeys.CompareTo(Keys.Control)==0 || !selection.Contains(e.Location)))
            {
                for (int i = 0; i < mouse_regions.Length; i++)
                {
                    if (mouse_regions[i].Contains(e.Location))
                    {
                        selection_region_clicked = i;
                        selection_resizing = true;
                        i = mouse_regions.Length;
                    }
                }
                if (!selection_resizing)
                {
                    selection_creating = true;

                    creating_origin = new Point(e.Location.X, e.Location.Y);
                    if (creating_origin.X < 0)
                        creating_origin.X = 0;
                    if (creating_origin.X > screen_on_focus.Bounds.Width)
                        creating_origin.X = screen_on_focus.Bounds.Width;
                    if (creating_origin.Y < 0)
                        creating_origin.Y = 0;
                    if (creating_origin.Y > screen_on_focus.Bounds.Height)
                        creating_origin.Y = screen_on_focus.Bounds.Height;

                    selection.Location = creating_origin;
                    selection.Size = new Size(REGION_SIZE * 2, REGION_SIZE * 2);
                    SetMouseRegions();
                    UpdateOverlay();
                }
            }
            else if (e.Button == MouseButtons.Left && ModifierKeys.CompareTo(Keys.Control)!=0 && selection.Contains(e.Location))
            {
                moving_offset = new Point(e.Location.X - selection.X, e.Location.Y - selection.Y);
                selection_moving = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                CutSelection();
            }
        }

        private void CutOverlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (selection_resizing)
            {
                Point cursor = new Point(e.Location.X, e.Location.Y);
                if (cursor.X < 0)
                    cursor.X = 0;
                if (cursor.X > screen_on_focus.Bounds.Width)
                    cursor.X = screen_on_focus.Bounds.Width;
                if (cursor.Y < 0)
                    cursor.Y = 0;
                if (cursor.Y > screen_on_focus.Bounds.Height)
                    cursor.Y = screen_on_focus.Bounds.Height;

                Point loc;

                switch (selection_region_clicked)
                {
                    case 0:
                        if (cursor.X > selection.Right - REGION_SIZE - REGION_SIZE)
                            cursor.X = selection.Right - REGION_SIZE - REGION_SIZE;
                        if (cursor.Y > selection.Bottom - REGION_SIZE - REGION_SIZE)
                            cursor.Y = selection.Bottom - REGION_SIZE - REGION_SIZE;

                        loc = new Point(selection.Right, selection.Bottom);
                        selection.Location = cursor;
                        selection.Size = new Size(loc.X - selection.Left, loc.Y - selection.Top);
                        break;
                    case 1:
                        if (cursor.X < selection.Left + REGION_SIZE + REGION_SIZE)
                            cursor.X = selection.Left + REGION_SIZE + REGION_SIZE;
                        if (cursor.Y > selection.Bottom - REGION_SIZE - REGION_SIZE)
                            cursor.Y = selection.Bottom - REGION_SIZE - REGION_SIZE;

                        loc = new Point(selection.Right, selection.Bottom);
                        selection.Y = cursor.Y;
                        selection.Width = cursor.X - selection.Left;
                        selection.Height = loc.Y - selection.Top;
                        break;
                    case 2:
                        if (cursor.X < selection.Left + REGION_SIZE + REGION_SIZE)
                            cursor.X = selection.Left + REGION_SIZE + REGION_SIZE;
                        if (cursor.Y < selection.Top + REGION_SIZE + REGION_SIZE)
                            cursor.Y = selection.Top + REGION_SIZE + REGION_SIZE;

                        selection.Width = cursor.X - selection.Left;
                        selection.Height = cursor.Y - selection.Top;
                        break;
                    case 3:
                        if (cursor.X > selection.Right - REGION_SIZE - REGION_SIZE)
                            cursor.X = selection.Right - REGION_SIZE - REGION_SIZE;
                        if (cursor.Y < selection.Top + REGION_SIZE + REGION_SIZE)
                            cursor.Y = selection.Top + REGION_SIZE + REGION_SIZE;

                        loc = new Point(selection.Right, selection.Bottom);
                        selection.X = cursor.X;
                        selection.Width = loc.X - selection.Left;
                        selection.Height = cursor.Y - selection.Top;
                        break;
                }

                SetMouseRegions();
                UpdateOverlay();
            }
            else if (selection_creating)
            {
                double dist = Math.Sqrt(Math.Pow(creating_origin.X - e.Location.X, 2) + Math.Pow(creating_origin.Y - e.Location.Y, 2));

                if (dist > 24)
                {
                    if (e.Location.X > creating_origin.X && e.Location.Y > creating_origin.Y)
                    {
                        selection_region_clicked = 2;
                        selection_creating = false;
                        selection_resizing = true;
                    }
                    else if (e.Location.X < creating_origin.X && e.Location.Y > creating_origin.Y)
                    {
                        selection_region_clicked = 3;
                        selection.Location = new Point(creating_origin.X - REGION_SIZE * 2, creating_origin.Y);
                        selection_creating = false;
                        selection_resizing = true;
                    }
                    else if (e.Location.X < creating_origin.X && e.Location.Y < creating_origin.Y)
                    {
                        selection_region_clicked = 0;
                        selection.Location = new Point(creating_origin.X - REGION_SIZE * 2, creating_origin.Y - REGION_SIZE * 2);
                        selection_creating = false;
                        selection_resizing = true;
                    }
                    else if (e.Location.X > creating_origin.X && e.Location.Y < creating_origin.Y)
                    {
                        selection_region_clicked = 1;
                        selection.Location = new Point(creating_origin.X, creating_origin.Y - REGION_SIZE * 2);
                        selection_creating = false;
                        selection_resizing = true;
                    }
                }
            }
            else if (selection_moving)
            {
                Point next = new Point(e.Location.X - moving_offset.X, e.Location.Y - moving_offset.Y);
                if (next.X < 0)
                    next.X = 0;
                if (next.X > screen_on_focus.Bounds.Width)
                    next.X = screen_on_focus.Bounds.Width;
                if (next.Y < 0)
                    next.Y = 0;
                if (next.Y > screen_on_focus.Bounds.Height)
                    next.Y = screen_on_focus.Bounds.Height;
                if (next.X + selection.Width > screen_on_focus.Bounds.Width)
                    next.X = screen_on_focus.Bounds.Width - selection.Width;
                if (next.Y + selection.Height > screen_on_focus.Bounds.Height)
                    next.Y = screen_on_focus.Bounds.Height - selection.Height;

                selection.Location = next;

                SetMouseRegions();
                UpdateOverlay();
            }
            else
            {
            }
        }

        private void CutOverlay_MouseUp(object sender, MouseEventArgs e)
        {
            selection_region_clicked = -1;
            selection_resizing = false;
            selection_creating = false;
            selection_moving = false;
        }

        private void Region_MouseEnter(object sender, EventArgs e)
        {
            CutOverlay.Cursor = Cursors.SizeAll;
        }

        private void Region_MouseLeave(object sender, EventArgs e)
        {
            CutOverlay.Cursor = Cursors.Cross;
        }

        private void OptionsMenuSave_Click(object sender, EventArgs e)
        {
            SaveSelection();
        }
    }

    public class CutEventArgs : EventArgs
    {
        Rectangle selection;
        string _imgsrc;

        public CutEventArgs(Rectangle selection, string imgsrc)
        {
            this.selection = selection;
            this._imgsrc = imgsrc;
        }

        public Rectangle Selection
        {
            get { return selection; }
        }

        public string imgsrc
        {
            get { return _imgsrc; }
        }
    }
}
