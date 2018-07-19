using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZXClient.control
{
    class MyRectangle
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the main rectangle
        /// </summary>
        private int x;

        /// <summary>
        /// The star x-coordinate when you draw the main rectangle
        /// </summary>
        private int downPointX;

        /// <summary>
        /// The y-coordinate of the upper-left corner of the main rectangle
        /// </summary>
        private int y;

        /// <summary>
        /// The star y-coordinate when you draw the main rectangle
        /// </summary>
        private int downPointY;

        /// <summary>
        /// The width of the main rectangle
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the main rectangle
        /// </summary>
        private int height;

        /// <summary>
        /// The least width of the main rectangle
        /// </summary>
        private int minWidth;

        /// <summary>
        /// The least height of the main rectangle
        /// </summary>
        private int minHeight;

        /// <summary>
        /// Sign the main rectangle is on change size mode or not
        /// </summary>
        private bool changeSizeMode;

        /// <summary>
        /// Sign the main rectangle is on move mode or not
        /// </summary>
        private bool moveMode;

        /// <summary>
        /// Sign the current mouse position is on the upper-left corner of the main rectangle or not
        /// </summary>
        private bool mouseOnLeftTop;
        /// <summary>
        /// Sign the current mouse position is on the middle point of the left line of the main rectangle or not
        /// </summary>
        private bool mouseOnLeftMiddle;
        /// <summary>
        /// Sign the current mouse position is on the bottom-left corner of the main rectangle or not
        /// </summary>
        private bool mouseOnLeftBottom;

        /// <summary>
        /// Sign the current mouse position is on the upper-right corner of the mian rectangle or not
        /// </summary>
        private bool mouseOnRightTop;
        /// <summary>
        /// 鼠标落点在右中点标志
        /// </summary>
        private bool mouseOnRightMiddle;
        /// <summary>
        /// Sign the current mouse position is on the middle point of the right line of the main rectangle or not
        /// </summary>
        private bool mouseOnRightBottom;

        /// <summary>
        /// Sign the current mouse position is on the middle point of the top line of the main rectangle or not
        /// </summary>
        private bool mouseOnTopMiddle;
        /// <summary>
        /// Sign the current mouse position is on the middle point of the bottom line of the main rectangle or not
        /// </summary>
        private bool mouseOnBottomMiddle;

        /// <summary>
        /// Sign the current mouse position is in the main rectangle or not
        /// </summary>
        private bool mouseOnMiddle;
        /// <summary>
        /// The width of the 8 little rectangles that on the 4 corners and the 4 middle points that of the 4 lines of the main rectangle
        /// </summary>
        private int littleRectangleWidth;
        /// <summary>
        /// The height of the 8 little rectangles that on the 4 corners and the 4 middle points that of the 4 lines of the main rectangle
        /// </summary>
        private int littleRectangleHeight;

        /// <summary>
        /// The little rectangle on the upper-left corner of the main rectangle
        /// </summary>
        private Rectangle leftTopRectangle;
        /// <summary>
        /// The little rectangle on the middle point of the left line of the main rectangle
        /// </summary>
        private Rectangle leftMiddleRectangle;
        /// <summary>
        /// The little rectangle on the bottom-left corner of the main rectangle
        /// </summary>
        private Rectangle leftBottomRectangle;

        /// <summary>
        /// The little rectangle on the upper-right corner of the main rectangle
        /// </summary>
        private Rectangle rightTopRectangle;
        /// <summary>
        /// The little rectangle on the middle point of the right line of the main rectangle
        /// </summary>
        private Rectangle rightMiddleRectangle;
        /// <summary>
        /// The little rectangle on the bottom-right corner of the main rectangle
        /// </summary>
        private Rectangle rightBottomRectangle;

        /// <summary>
        /// The little rectangle on the middle point of the top line of the main rectangle
        /// </summary>
        private Rectangle topMiddleRectangle;
        /// <summary>
        /// The little rectangle on the middle point of the bottom line of the main rectangle
        /// </summary>
        private Rectangle bottomMiddleRectangle;

        /// <summary>
        /// The main rectangle
        /// </summary>
        private Rectangle rect;
        /// <summary>
        /// The background image of the screen
        /// </summary>
        private Image backImage;

        /// <summary>
        /// The cursor manner
        /// </summary>
        private Cursor myCursor;

        /// <summary>
        /// Gets of sets the x-coordinate of the upper-left corner of the main rectangle
        /// </summary>
        public int X
        {
            get { return x; }
            set
            {
                x = value;
                rect.X = value;
            }
        }
        /// <summary>
        /// Gets of sets the y-coordinate of the upper-left corner of the main rectangle
        /// </summary>
        public int Y
        {
            get { return y; }
            set
            {
                y = value;
                rect.Y = value;
            }
        }

        /// <summary>
        /// Gets of sets the star x-coordinate when you draw the main rectangle
        /// </summary>
        public int DownPointX
        {
            get { return downPointX; }
            set { downPointX = value; }
        }
        /// <summary>
        /// Gets of sets the star y-coordinate when you draw the main rectangle
        /// </summary>
        public int DownPointY
        {
            get { return downPointY; }
            set { downPointY = value; }
        }
        /// <summary>
        /// Gets of sets the width of the main rectangle
        /// </summary>
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                rect.Width = value;
            }
        }
        /// <summary>
        /// Gets or sets the height of the main rectangle
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                rect.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the least width of the main  rectangle
        /// </summary>
        public int MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
        }

        /// <summary>
        /// Gets or sets the least height of the main rectangle
        /// </summary>
        public int MinHeight
        {
            get { return minHeight; }
            set { minHeight = value; }
        }

        /// <summary>
        /// Gets or sets the sign of the change size mode of the main rectangle
        /// </summary>
        public bool ChangeSizeMode
        {
            get { return changeSizeMode; }
            set
            {
                changeSizeMode = value;
                moveMode = !value;
            }
        }

        /// <summary>
        /// Gets or sets the sign of the move mode of the main rectangle
        /// </summary>
        public bool MoveMode
        {
            get { return moveMode; }
            set
            {
                moveMode = value;
                changeSizeMode = !value;
            }
        }

        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the upper-left corner of the main rectangle or not)
        /// </summary>
        public bool MouseOnLeftTop
        {
            get { return mouseOnLeftTop; }
            set
            {
                mouseOnLeftTop = value;
                if (value)
                {
                    mouseOnLeftMiddle = false;
                    mouseOnLeftBottom = false;

                    mouseOnRightTop = false;
                    mouseOnRightMiddle = false;
                    mouseOnRightBottom = false;

                    mouseOnTopMiddle = false;
                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the middle point of the left line of the main rectangle or not)
        /// </summary>
        public bool MouseOnLeftMiddle
        {
            get { return mouseOnLeftMiddle; }
            set
            {
                mouseOnLeftMiddle = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftBottom = false;

                    mouseOnRightTop = false;
                    mouseOnRightMiddle = false;
                    mouseOnRightBottom = false;

                    mouseOnTopMiddle = false;
                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the bottom-left corner of the main rectangle or not)
        /// </summary>
        public bool MouseOnLeftBottom
        {
            get { return mouseOnLeftBottom; }
            set
            {
                mouseOnLeftBottom = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftMiddle = false;

                    mouseOnRightTop = false;
                    mouseOnRightMiddle = false;
                    mouseOnRightBottom = false;

                    mouseOnTopMiddle = false;
                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the upper-right corner of the main rectangle or not)
        /// </summary>
        public bool MouseOnRightTop
        {
            get { return mouseOnRightTop; }
            set
            {
                mouseOnRightTop = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    MouseOnLeftMiddle = false;
                    mouseOnLeftBottom = false;

                    mouseOnRightMiddle = false;
                    mouseOnRightBottom = false;

                    mouseOnTopMiddle = false;
                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the middle point of the right line of the main rectangle or not)
        /// </summary>
        public bool MouseOnRightMiddle
        {
            get { return mouseOnRightMiddle; }
            set
            {
                mouseOnRightMiddle = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftBottom = false;
                    mouseOnLeftMiddle = false;

                    mouseOnRightTop = false;
                    mouseOnRightBottom = false;

                    mouseOnTopMiddle = false;
                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the bottom-right corner of the main rectangle or not)
        /// </summary>
        public bool MouseOnRightBottom
        {
            get { return mouseOnRightBottom; }
            set
            {
                mouseOnRightBottom = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftBottom = false;
                    mouseOnLeftMiddle = false;

                    mouseOnRightTop = false;
                    mouseOnRightMiddle = false;

                    mouseOnTopMiddle = false;
                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the middle point of the top line of the main rectangle or not)
        /// </summary>
        public bool MouseOnTopMiddle
        {
            get { return mouseOnTopMiddle; }
            set
            {
                mouseOnTopMiddle = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftBottom = false;
                    mouseOnLeftMiddle = false;

                    mouseOnRightTop = false;
                    mouseOnRightBottom = false;
                    mouseOnRightMiddle = false;

                    mouseOnBottomMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is on the middle point of the middle line of the main rectangle or not)
        /// </summary>
        public bool MouseOnBottomMiddle
        {
            get { return mouseOnBottomMiddle; }
            set
            {
                mouseOnBottomMiddle = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftBottom = false;
                    mouseOnLeftMiddle = false;

                    mouseOnRightTop = false;
                    mouseOnRightBottom = false;
                    mouseOnRightMiddle = false;

                    mouseOnTopMiddle = false;

                    mouseOnMiddle = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sign of current mouse position 
        /// (is in the main rectangle or not)
        /// </summary>
        public bool MouseOnMiddle
        {
            get { return mouseOnMiddle; }
            set
            {
                mouseOnMiddle = value;
                if (value)
                {
                    mouseOnLeftTop = false;
                    mouseOnLeftBottom = false;
                    mouseOnLeftMiddle = false;

                    mouseOnRightTop = false;
                    mouseOnRightBottom = false;
                    mouseOnRightMiddle = false;

                    mouseOnTopMiddle = false;
                    MouseOnBottomMiddle = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the width of the 8 little rectangles
        /// (rectangles that on the 4 corners and the 4 middle points that of the 4 lines of the main rectangle)
        /// </summary>
        public int LittleRectangleWidth
        {
            get { return littleRectangleWidth; }
            set { littleRectangleWidth = value; }
        }
        /// <summary>
        /// Gets or sets the height of the 8 little rectangles
        /// (rectangles that on the 4 corners and the 4 middle points that of the 4 lines of the main rectangle)
        /// </summary>
        public int LittleRectangleHeight
        {
            get { return littleRectangleHeight; }
            set { littleRectangleHeight = value; }
        }

        /// <summary>
        /// Gets or sets he little rectangle on the upper-left corner of the main rectangle
        /// </summary>
        public Rectangle LeftTopRectangle
        {
            get { return leftTopRectangle; }
            set { leftTopRectangle = value; }
        }
        /// <summary>
        /// Gets or sets he little rectangle on the middle point of the left line of the main rectangle
        /// </summary>
        public Rectangle LeftMiddleRectangle
        {
            get { return leftMiddleRectangle; }
            set { leftMiddleRectangle = value; }
        }
        /// <summary>
        /// Gets or sets he little rectangle on the bottom-left corner of the main rectangle
        /// </summary>
        public Rectangle LeftBottomRectangle
        {
            get { return leftBottomRectangle; }
            set { leftBottomRectangle = value; }
        }

        /// <summary>
        /// Gets or sets he little rectangle on the upper-right corner of the main rectangle
        /// </summary>
        public Rectangle RightTopRectangle
        {
            get { return rightTopRectangle; }
            set { rightTopRectangle = value; }
        }
        /// <summary>
        /// Gets or sets he little rectangle on the middle point of the right line of the main rectangle
        /// </summary>
        public Rectangle RightMiddleRectangle
        {
            get { return rightMiddleRectangle; }
            set { rightMiddleRectangle = value; }
        }
        /// <summary>
        /// Gets or sets he little rectangle on the bottom-right corner of the main rectangle
        /// </summary>
        public Rectangle RightBottomRectangle
        {
            get { return rightBottomRectangle; }
            set { rightBottomRectangle = value; }
        }

        /// <summary>
        /// Gets or sets he little rectangle on the middle point of the top line of the main rectangle
        /// </summary>
        public Rectangle TopMiddleRectangle
        {
            get { return topMiddleRectangle; }
            set { topMiddleRectangle = value; }
        }
        /// <summary>
        /// Gets or sets he little rectangle on the middle point of the bottom line of the main rectangle
        /// </summary>
        public Rectangle BottomMiddleRectangle
        {
            get { return bottomMiddleRectangle; }
            set { bottomMiddleRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the main rectangle
        /// </summary>
        public Rectangle Rect
        {
            get { return rect; }
            set
            {
                rect = value;
                x = value.X;
                y = value.Y;
                width = value.Width;
                height = value.Height;
            }
        }

        /// <summary>
        /// Gets the size of the main rectangle
        /// </summary>
        public Size Size
        {
            get { return rect.Size; }
        }

        /// <summary>
        /// Gets or sets the background image of the screen
        /// </summary>
        public Image BackImage
        {
            get { return backImage; }
            set { backImage = value; }
        }


        /// <summary>
        /// Gets or sets the manner of the cursor
        /// </summary>
        public Cursor MyCursor
        {
            get { return myCursor; }
            set { myCursor = value; }
        }


        /// <summary>
        /// Constructor function
        /// </summary>
        public MyRectangle()
        {
            Rect = new Rectangle();

            setAllModeFalse();

            LittleRectangleWidth = 4;
            LittleRectangleHeight = 4;

            MinHeight = 5;
            MinWidth = 5;

            LeftTopRectangle = new Rectangle();
            LeftMiddleRectangle = new Rectangle();
            LeftBottomRectangle = new Rectangle();

            RightTopRectangle = new Rectangle();
            RightMiddleRectangle = new Rectangle();
            RightBottomRectangle = new Rectangle();

            TopMiddleRectangle = new Rectangle();
            BottomMiddleRectangle = new Rectangle();

            MyCursor = new Cursor(@"..\..\Cursors\hcross.cur");

        }

        public void SetLittleRectangle()
        {
            int excursionX = LittleRectangleWidth / 2;
            int excursionY = LittleRectangleHeight / 2;
            LeftTopRectangle = new Rectangle(X - excursionX, Y - excursionY, LittleRectangleWidth, LittleRectangleHeight);
            leftMiddleRectangle = new Rectangle(X - excursionX, Y - excursionY + Height / 2, LittleRectangleWidth, LittleRectangleHeight);
            leftBottomRectangle = new Rectangle(X - excursionX, Y - excursionY + Height, LittleRectangleWidth, LittleRectangleHeight);

            rightTopRectangle = new Rectangle(X - excursionX + Width, Y - excursionY, LittleRectangleWidth, LittleRectangleHeight);
            rightMiddleRectangle = new Rectangle(X - excursionX + Width, Y - excursionY + Height / 2, LittleRectangleWidth, LittleRectangleHeight);
            rightBottomRectangle = new Rectangle(X - excursionX + Width, Y - excursionY + Height, LittleRectangleWidth, LittleRectangleHeight);

            topMiddleRectangle = new Rectangle(X - excursionX + Width / 2, Y - excursionY, LittleRectangleWidth, LittleRectangleHeight);
            bottomMiddleRectangle = new Rectangle(X - excursionX + Width / 2, Y - excursionY + Height, LittleRectangleWidth, LittleRectangleHeight);
        }

        /// <summary>
        /// draw rectangle function
        /// </summary>
        /// <param name="e">mouse event </param>
        /// <param name="backColor">back color</param>
        public void Draw(MouseEventArgs e, Color backColor)
        {
            Draw(backColor);
            if (e.X < DownPointX)
            {
                Width = DownPointX - e.X;
                X = e.X;
            }
            else
            {
                Width = e.X - DownPointX;
            }
            if (e.Y < DownPointY)
            {
                Height = DownPointY - e.Y;
                Y = e.Y;
            }
            else
            {
                Height = e.Y - DownPointY;
            }
            Draw(backColor);
        }
        /// <summary>
        /// draw rectangle function
        /// </summary>
        /// <param name="backColor">back color</param>
        public void Draw(Color backColor)
        {
            //Initialize the 8 little rectangles 
            SetLittleRectangle();

            //draw the main rectangle and the 8 little rectangles
            ControlPaint.DrawReversibleFrame(rect, backColor, FrameStyle.Dashed);

            ControlPaint.FillReversibleRectangle(leftTopRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(leftMiddleRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(leftBottomRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(rightTopRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(rightMiddleRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(rightBottomRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(topMiddleRectangle, Color.White);
            ControlPaint.FillReversibleRectangle(bottomMiddleRectangle, Color.White);
        }

        /// <summary>
        /// change size when the rectangle is on the change size mode
        /// </summary>
        /// <param name="e"></param>
        public void ChangeSize(MouseEventArgs e)
        {
            //change size according the mouse location
            if (ChangeSizeMode)
            {

                if (MouseOnLeftTop)
                {
                    if ((Rect.Right - e.X < MinWidth) || (Rect.Bottom - e.Y < MinHeight))
                    {
                        return;
                    }
                    Width = Math.Abs(Rect.Right - e.X);
                    Height = Math.Abs(Rect.Bottom - e.Y);
                    X = e.X;
                    Y = e.Y;
                }
                else if (MouseOnLeftMiddle)
                {
                    if (Rect.Right - e.X < MinWidth)
                    {
                        return;
                    }
                    else
                    {
                        Width = Math.Abs(Rect.Right - e.X);
                        X = e.X;
                    }
                }
                else if (MouseOnLeftBottom)
                {
                    if (Rect.Right - e.X < MinWidth || e.Y - Rect.Top < MinHeight)
                    {
                        return;
                    }
                    Width = Math.Abs(Rect.Right - e.X);
                    Height = Math.Abs(e.Y - Rect.Top);
                    X = e.X;
                }
                else if (MouseOnRightTop)
                {
                    if (e.X - Rect.Left < MinWidth || Rect.Bottom - e.Y < MinHeight)
                    {
                        return;
                    }
                    Width = Math.Abs(e.X - X);
                    Height = Math.Abs(Rect.Bottom - e.Y);
                    Y = e.Y;
                }
                else if (MouseOnRightMiddle)
                {
                    if (e.X - Rect.Left < MinWidth)
                    {
                        return;
                    }
                    Width = Math.Abs(e.X - X);

                }
                else if (MouseOnRightBottom)
                {
                    if (e.X - Rect.Left < MinWidth || e.Y - Rect.Top < MinHeight)
                    {
                        return;
                    }
                    Width = Math.Abs(e.X - X);
                    Height = Math.Abs(e.Y - Y);
                }
                else if (MouseOnTopMiddle)
                {
                    if (Rect.Bottom - e.Y < MinHeight)
                    {
                        return;
                    }
                    Height = Math.Abs(Rect.Bottom - e.Y);
                    Y = e.Y;
                }
                else if (MouseOnBottomMiddle)
                {
                    if (e.Y - Rect.Top < MinHeight)
                    {
                        return;
                    }
                    Height = Math.Abs(e.Y - Y);
                }

            }
        }

        /// <summary>
        /// move the location of the rectangle
        /// </summary>
        /// <param name="newX">The x-coordinate of the new location</param>
        /// <param name="newY">The y-coordinate of the new location</param>
        public void Move(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }

        /// <summary>
        /// check the current mouse location and set the cursor manner according the mouse location
        /// </summary>
        /// <param name="e">mouse</param>
        public void CheckMouseLocation(MouseEventArgs e)
        {
            if (leftTopRectangle.Contains(e.X, e.Y))
            {
                if (!onChangingMode())
                {
                    MouseOnLeftTop = true;
                    MyCursor = Cursors.SizeNWSE;
                   // myCursor = new Cursor(@"..\..\Cursors\size2_m.cur");
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (leftMiddleRectangle.Contains(e.X, e.Y))
            {
                if (!onChangingMode())
                {
                    MouseOnLeftMiddle = true;
                    myCursor = Cursors.SizeWE;
                    //myCursor = new Cursor(@"..\..\Cursors\size3_m.cur");
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (leftBottomRectangle.Contains(e.X, e.Y))
            {
                 if (!onChangingMode())
                {
                    MouseOnLeftBottom = true;
                    myCursor = Cursors.SizeNESW;
                    //myCursor = new Cursor(@"..\..\Cursors\size1_m.cur");
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }

            else if (rightTopRectangle.Contains(e.X, e.Y))
            {
                mouseOnLeftBottom = false;

                if (!onChangingMode())
                {
                    MouseOnRightTop = true;
                    myCursor = Cursors.SizeNESW;
                    //myCursor = new Cursor(@"..\..\Cursors\size1_m.cur");
                }

                mouseOnMiddle = false;

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (rightMiddleRectangle.Contains(e.X, e.Y))
            {
                if (!onChangingMode())
                {
                    MouseOnRightMiddle = true;
                    myCursor = Cursors.SizeWE;
                    //myCursor = new Cursor(@"..\..\Cursors\size3_m.cur");
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (rightBottomRectangle.Contains(e.X, e.Y))
            {
                if (!onChangingMode())
                {
                    MouseOnRightBottom = true;
                    myCursor = Cursors.SizeNWSE;
                    //myCursor = new Cursor(@"..\..\Cursors\size2_m.cur");
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (topMiddleRectangle.Contains(e.X, e.Y))
            {

                if (!onChangingMode())
                {
                    MouseOnTopMiddle = true;
                    myCursor = Cursors.SizeNS;
                    //myCursor = new Cursor(@"..\..\Cursors\size4_m.cur");
                }


                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (bottomMiddleRectangle.Contains(e.X, e.Y))
            {
                if (!onChangingMode())
                {
                    MouseOnBottomMiddle = true;
                    myCursor = Cursors.SizeNS;
                    //myCursor = new Cursor(@"..\..\Cursors\size4_m.cur");
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!MoveMode)
                    {
                        ChangeSizeMode = true;
                    }
                }
                else
                {
                    changeSizeMode = false;
                    moveMode = false;
                }
            }
            else if (rect.Contains(e.X, e.Y))
            {

                if (!changeSizeMode)
                {
                    MouseOnMiddle = true;
                    myCursor = Cursors.SizeAll;
                    //myCursor = new Cursor(@"..\..\Cursors\move_m.cur");
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (!ChangeSizeMode)
                    {
                        MoveMode = true;
                    }
                }
                else
                {
                    moveMode = false;
                    changeSizeMode = false;
                }
            }
            else
            {
                if (e.Button != MouseButtons.Left)
                {
                    setAllModeFalse();

                    myCursor = new Cursor(@"..\..\Cursors\hcross.cur");
                }
            }
        }
        public bool Contains(int x, int y)
        {
            if (rect.Contains(x, y))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// set all mode false
        /// (the sign of the mouse location and the change size mode ,move mode)
        /// </summary>
        public void setAllModeFalse()
        {
            mouseOnLeftTop = false;
            mouseOnLeftMiddle = false;
            mouseOnLeftBottom = false;

            mouseOnRightTop = false;
            mouseOnRightMiddle = false;
            mouseOnRightBottom = false;

            mouseOnTopMiddle = false;
            mouseOnBottomMiddle = false;

            mouseOnMiddle = false;
            changeSizeMode = false;
            moveMode = false;
            myCursor = new Cursor(@"..\..\Cursors\hcross.cur");
        }

        /// <summary>
        /// check whether the rectangle is on change mode now
        /// </summary>
        /// <returns></returns>
        public bool onChangingMode()
        {
            return ((MouseOnLeftTop || MouseOnLeftMiddle || mouseOnLeftBottom || mouseOnRightTop || mouseOnRightMiddle || MouseOnRightBottom || mouseOnTopMiddle || MouseOnBottomMiddle) && changeSizeMode);
        }

        /// <summary>
        /// Initialize the rectangle
        /// </summary>
        /// <param name="x">The x-coordinate of the rectangle</param>
        /// <param name="y">The y-coordinate of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public void Initialize(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            SetLittleRectangle();
        }
    }
}
