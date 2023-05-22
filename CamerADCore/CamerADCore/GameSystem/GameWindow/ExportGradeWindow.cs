using CamerADCore.GameSystem.GameWindowSys;
using HZH_Controls.Forms;
using Sunny.UI;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class ExportGradeWindow : Form
    {
        public ExportGradeWindow()
        {
            InitializeComponent();
        }
        public string _ProjectName =string.Empty;
        public  string _GroupName = string.Empty;
        public string projectId = string .Empty;

        private void ExportGradeWindow_Load(object sender, System.EventArgs e)
        {
             uiLabel1.Text = $"项目:{_ProjectName},当前选择组别:{_GroupName}";
        }

        private void uiButton1_Click(object sender, System.EventArgs e)
        {
            if (ExportGradeWindowSys.Instance.OutPutScore(projectId, _ProjectName, _GroupName, true))
            {
                //UIMessageBox.ShowSuccess("导出成绩成功");
                DialogResult = DialogResult.OK;
            }
            else
            {

               // UIMessageBox.ShowError("导出成绩失败");
                DialogResult = DialogResult.None;
            }
            this.Close();
        }

        private void uiButton2_Click(object sender, System.EventArgs e)
        {
            if (ExportGradeWindowSys.Instance.OutPutScore(projectId, _ProjectName, _GroupName))
            {
                FrmTips.ShowTipsSuccess(this, "导出成绩成功");
                DialogResult = DialogResult.OK;
            }
            else
            {
                FrmTips.ShowTipsError(this, "导出成绩失败");
                DialogResult = DialogResult.None;
            }
            this.Close();
        }
    }
}