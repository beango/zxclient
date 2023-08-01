﻿using System.Windows.Forms;

namespace ZXClient.control.EventControls
{
    /// <summary>
    /// Extende o controle PictureBox a um controle que pode receber certos eventos de mouse de controles filhos e passa ao controle pai.
    /// </summary>
    public sealed partial class EventIntermediaryPictureBox : PictureBox, IMouseEventReceiver
    {
        public EventIntermediaryPictureBox(IMouseEventReceiver parent)
        {
            MouseMove += parent.HandleMouseMove;
            MouseDown += parent.HandleMouseDown;
            MouseUp += parent.HandleMouseUp;
        }

        void IMouseEventReceiver.HandleMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + ((Control)sender).Location.X, e.Y + ((Control)sender).Location.Y, e.Delta));
        }

        void IMouseEventReceiver.HandleMouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + ((Control)sender).Location.X, e.Y + ((Control)sender).Location.Y, e.Delta));
        }

        void IMouseEventReceiver.HandleMouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + ((Control)sender).Location.X, e.Y + ((Control)sender).Location.Y, e.Delta));
        }
    }
}