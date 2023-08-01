using System.Windows.Forms;

namespace ZXClient.control.EventControls
{
    /// <summary>
    /// Extende o controle PictureBox a um controle que propaga certos eventos de mouse ao seu controle pai.
    /// </summary>
    public partial class EventSenderPictureBox : PictureBox
    {
        public EventSenderPictureBox(IMouseEventReceiver parent)
        {
            MouseMove += parent.HandleMouseMove;
            MouseDown += parent.HandleMouseDown;
            MouseUp += parent.HandleMouseUp;
        }
    }
}
