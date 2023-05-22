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
             uiLabel1.Text = $"��Ŀ:{_ProjectName},��ǰѡ�����:{_GroupName}";
        }

        private void uiButton1_Click(object sender, System.EventArgs e)
        {
            if (ExportGradeWindowSys.Instance.OutPutScore(projectId, _ProjectName, _GroupName, true))
            {
                //UIMessageBox.ShowSuccess("�����ɼ��ɹ�");
                DialogResult = DialogResult.OK;
            }
            else
            {

               // UIMessageBox.ShowError("�����ɼ�ʧ��");
                DialogResult = DialogResult.None;
            }
            this.Close();
        }

        private void uiButton2_Click(object sender, System.EventArgs e)
        {
            if (ExportGradeWindowSys.Instance.OutPutScore(projectId, _ProjectName, _GroupName))
            {
                FrmTips.ShowTipsSuccess(this, "�����ɼ��ɹ�");
                DialogResult = DialogResult.OK;
            }
            else
            {
                FrmTips.ShowTipsError(this, "�����ɼ�ʧ��");
                DialogResult = DialogResult.None;
            }
            this.Close();
        }
    }
}