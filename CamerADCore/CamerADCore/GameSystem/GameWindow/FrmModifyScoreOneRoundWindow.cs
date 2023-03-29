using CameraADCoreModel.ADCoreSqlite;
using CamerADCore.GameSystem.GameWindowSys;
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
    public partial class FrmModifyScoreOneRoundWindow : Form
    {
        
        public string projectName { set; get; }
        public string groupName { set; get; }
        public string IdNumber { set; get; }
        public string stuName { set; get; }
        public string projectId { set; get; }

        public string personId { set; get; }

        /// <summary>
        /// 轮次
        /// </summary>
        public int roundId { set; get; }

        public string score { set; get; }

        public string State { set; get; }
        public int iState { set; get; }
        public FrmModifyScoreOneRoundWindow()
        {
            InitializeComponent();
        }

        private void FrmModifyScoreOneRoundWindow_Load(object sender, EventArgs e)
        {
            var sl =    FrmModifyScoreOneRoundSys.Instance.LoadingInitPersonData(personId, IdNumber);
            if (sl.Count > 0)
                personId = sl["Id"];
            var sls = FrmModifyScoreOneRoundSys.Instance.LoadingScoreData(personId, roundId);
            if (sls.Count>0)
            {
                score = sls["Result"];
                State = sls["State"];
            }

            NameTxt.Text = stuName;
            IDNumberTxt.Text = IdNumber;
            GroupTxt.Text = groupName;
            ProjectTxt.Text = projectName;
            uiTextBox2.Text = $"第{roundId}轮";
            uiTextBox1.Text = score;
            int.TryParse(State, out int iState0);
            iState = iState0;
            uiComboBox2.SelectedIndex = iState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiTextBox1_TextChanged(object sender, EventArgs e)
        {
            score = uiTextBox1.Text.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            iState = uiComboBox2.SelectedIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(uiTextBox1.Text.Trim()))
            {
                FrmModifyScoreOneRoundSys.Instance.SetBackData(score, iState, personId);
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
