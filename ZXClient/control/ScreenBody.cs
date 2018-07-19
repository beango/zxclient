using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZXClient.control
{
    public partial class ScreenBody : Form
    {
        private Graphics MainPainter;  //the main painter
        private bool isDowned;         //check whether the mouse is down
        private bool RectReady;         //check whether the rectangle is finished
        private Image baseImage;       //the back ground of the screen

        private Point moveModeDownPoint;    //the mouse location when you move the rectangle

        private MyRectangle myRectangle;    //the rectangle

        private bool moveMode; //check whether the rectangle is on move mode or not
        private bool changeSizeMode;  //check whether the rectangle is on change size mode or not

        public ScreenBody()
        {
            InitializeComponent();
            panel1.Location = new Point(this.Left, this.Top);
            myRectangle = new MyRectangle();
            moveModeDownPoint = new Point();
            this.Cursor = myRectangle.MyCursor;
        }

        private void ScreenBody_DoubleClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left && myRectangle.Contains(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y))
            {
                panel1.Visible = false;
                MainPainter.DrawImage(baseImage, 0, 0);
                Image memory = new Bitmap(myRectangle.Width, myRectangle.Height);
                Graphics g = Graphics.FromImage(memory);
                
                g.CopyFromScreen(myRectangle.X, myRectangle.Y, 0, 0, myRectangle.Size);
                Clipboard.SetImage(memory);
                
                this.Close();
            }
        }

        private void ScreenBody_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                
                isDowned = true;

                if (!RectReady)
                {
                    myRectangle.DownPointX = e.X;
                    myRectangle.DownPointY = e.Y;
                    myRectangle.X = e.X;
                    myRectangle.Y = e.Y;
                }
                if (RectReady == true)
                {
                    moveModeDownPoint = new Point(e.X, e.Y);    
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (!RectReady)
                {
                    
                    this.Close();
                    return;
                }
                MainPainter.DrawImage(baseImage, 0, 0);
                myRectangle.Initialize(0, 0, 0, 0);
                myRectangle.setAllModeFalse();
                this.Cursor = myRectangle.MyCursor;
                RectReady = false;
            }

        }

        private void ScreenBody_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDowned = false;
                RectReady = true;
            }
        }

        private void ScreenBody_MouseMove(object sender, MouseEventArgs e)
        {
            
            labelWidth.Text = myRectangle.Width.ToString();
            labelHeight.Text = myRectangle.Height.ToString();
            labelLocation.Text = "X "+myRectangle.X.ToString() + ", Y " + myRectangle.Y.ToString();
            if (!RectReady)
            {
                if (isDowned)
                {
                    myRectangle.Draw(e, this.BackColor);
                }
            }
            else
            {
                myRectangle.CheckMouseLocation(e);

                this.Cursor = myRectangle.MyCursor;
                
                this.changeSizeMode = myRectangle.ChangeSizeMode;
                this.moveMode = myRectangle.MoveMode&&myRectangle.Contains(moveModeDownPoint.X,moveModeDownPoint.Y);
                if (changeSizeMode)
                {
                    this.moveMode = false;
                    myRectangle.Draw(BackColor);
                    myRectangle.ChangeSize(e);
                    myRectangle.Draw(BackColor);
                }
                if (moveMode)
                {
                    this.changeSizeMode = false;
                    myRectangle.Draw(BackColor);
                    myRectangle.X = myRectangle.X + e.X - moveModeDownPoint.X;
                    myRectangle.Y = myRectangle.Y + e.Y - moveModeDownPoint.Y;

                    moveModeDownPoint.X = e.X;
                    moveModeDownPoint.Y = e.Y;

                    myRectangle.Draw(this.BackColor);
                }
            }
        }

        private void ScreenBody_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            MainPainter = this.CreateGraphics();
            isDowned = false;
            baseImage = this.BackgroundImage;
            panel1.Visible = true;
            RectReady = false;
            changeSizeMode = false;
            moveMode = false;

        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            if (panel1.Location==new Point(this.Left,this.Top))
            {
                panel1.Location = new Point(this.Right-panel1.Width, this.Top);
            }
            else
            {
                panel1.Location = new Point(this.Left,this.Top);
            }
        }

    }
}