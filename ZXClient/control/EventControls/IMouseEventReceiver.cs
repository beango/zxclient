using System.Windows.Forms;

namespace ZXClient.control.EventControls
{
    /// <summary>
    /// Define uma interface que declara métodos relacionados a eventos de mouse.
    /// </summary>
    public interface IMouseEventReceiver
    {
        void HandleMouseMove(object sender, MouseEventArgs e);
        void HandleMouseDown(object sender, MouseEventArgs e);
        void HandleMouseUp(object sender, MouseEventArgs e);
    }
}
