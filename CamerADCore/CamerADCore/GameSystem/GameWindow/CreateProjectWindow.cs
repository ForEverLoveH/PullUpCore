using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class CreateProjectWindow : Form
    {
        public CreateProjectWindow()
        {
            InitializeComponent();
        }
        public static string projectName = string.Empty;

        private void uiButton1_Click(object sender, EventArgs e)
        {
            BeSure();
        }

        private void BeSure()
        {
            string name = ProjectNameTxxt.Text;
            if (string.IsNullOrEmpty(name))
            {
                UIMessageBox.ShowWarning("请输入项目名称！！");
                return;
            }
            else
            {
                projectName = name;

                this.Close();
            }
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
             
            this.Close();
        }

        private void ProjectNameTxxt_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                BeSure();
            }
        }
    }
}
