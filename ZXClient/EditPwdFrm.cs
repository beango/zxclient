using System;
using System.Windows.Forms;
using ZXClient.dao;
using ZXClient.model;
using ZXClient.util;

namespace ZXClient
{
    public partial class EditPwdFrm : Form
    {
        public EditPwdFrm()
        {
            InitializeComponent();

            this.TopMost = true;
            CommonHelper.SetMid(this);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tbOld.Text == "")
            {
                MessageBox.Show("旧密码不能为空!");
                return;
            }
            if (tbNew.Text == "")
            {
                MessageBox.Show("新密码不能为空!");
                return;
            }
            if (tbNew2.Text == "")
            {
                MessageBox.Show("确认新密码不能为空!");
                return;
            }
            if (tbNew.Text != tbNew2.Text)
            {
                MessageBox.Show("两次输入的密码不一致!");
                return;
            }
            if (tbNew.Text.Length<6)
            {
                MessageBox.Show("密码最小长度为6!");
                return;
            }
            String oldpwd = EDncryptHelper.MD5Encrypt16(tbOld.Text);
            String newpwd = EDncryptHelper.MD5Encrypt16(tbNew.Text);
            string data = String.Format("cardnum={0}&oldpsw={1}&newpsw={2}", MainData.USERCARD, oldpwd, newpwd);
            String ReturnDatastr = HttpUtil.RequestData(MainData.ServerAddr + MainData.INTE_EMPLOYEECHANGEPSW, data);
            if (ReturnDatastr == "errorPassword")
            {
                MessageBox.Show("旧密码错误!");
                return;
            }
            if (ReturnDatastr == "changeSuccess")
            {
                db_EmployeeLoginDao.logout(MainData.USERCARD);
                MessageBox.Show("密码修改成功,请重新登录!");
                Application.ExitThread();
                MainData.Restart();
                return;
            }
            MessageBox.Show(ReturnDatastr);//
        }

        private void EditPwdFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != MainData.wf)
                MainData.wf.Visible = true;
        }
    }
}
