using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ZXClient
{
    public class ParentForm : Form
    {
        protected int ActualHeight = 300;

        protected virtual void drawBorder(Graphics fx)
        {
            fx.DrawRectangle(Pens.Silver, 2, 2, Width - 4, ActualHeight - 4);

            // Top border
            fx.DrawLine(Pens.Silver, 0, 0, Width, 0);
            fx.DrawLine(Pens.White, 0, 1, Width, 1);
            fx.DrawLine(Pens.DarkGray, 3, 3, Width - 4, 3);
            fx.DrawLine(Pens.DimGray, 4, 4, Width - 5, 4);

            // Left border
            fx.DrawLine(Pens.Silver, 0, 0, 0, ActualHeight);
            fx.DrawLine(Pens.White, 1, 1, 1, ActualHeight);
            fx.DrawLine(Pens.DarkGray, 3, 3, 3, ActualHeight - 4);
            fx.DrawLine(Pens.DimGray, 4, 4, 4, ActualHeight - 5);

            // Bottom border
            fx.DrawLine(Pens.DarkGray, 1, ActualHeight - 1, Width - 1, ActualHeight - 1);
            fx.DrawLine(Pens.White, 3, ActualHeight - 3, Width - 3, ActualHeight - 3);
            fx.DrawLine(Pens.Silver, 4, ActualHeight - 4, Width - 4, ActualHeight - 4);

            // Right border
            fx.DrawLine(Pens.DarkGray, Width - 1, 1, Width - 1, ActualHeight - 1);
            fx.DrawLine(Pens.White, Width - 3, 3, Width - 3, ActualHeight - 3);
            fx.DrawLine(Pens.Silver, Width - 4, 4, Width - 4, ActualHeight - 4);
        }
    }
}
