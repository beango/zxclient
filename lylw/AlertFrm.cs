using System;
using System.Windows.Forms;

namespace ZXClient
{
    public partial class AlertFrm : Form
    {
        public AlertFrm(String msg)
        {
            InitializeComponent();
            this.lblmsg.Text = msg;
        }
    }
}
