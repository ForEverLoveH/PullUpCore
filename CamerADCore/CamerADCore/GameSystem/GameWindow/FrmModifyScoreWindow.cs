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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class FrmModifyScoreWindow : Form
    {
        public FrmModifyScoreWindow()
        {
            InitializeComponent();
        }
         
        public string projectName { set; get; }
        public string groupName { set; get; }
        public string IdNumber { set; get; }
        public string stuName { set; get; }
        public string projectId { set; get; }
        public  string status { get; set; }


        /// <summary>
        /// 轮次
        /// </summary>
        public int roundId { set; get; }

        

        public string State { set; get; }
        
        private  int updaterountId = 0;
        private double updateScore =0;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmModifyScoreWindow_Load(object sender, EventArgs e)
        {
             
            ProjectNameTxt.Text = projectName;
            studentNameTxt.Text = stuName;
            GroupNameTxt.Text = groupName;
            IDNumberTxt.Text = IdNumber;
            for(int i=0; i < roundId; i++)
            {
                uiComboBox2.Items.Add($"第{i + 1}轮");
                uiComboBox2.SelectedIndex = 0;
            }
            List<string> list = new List<string>();
            for (int i =0;i < uiComboBox1.Items.Count; i++)
            {
                list.Add(uiComboBox1.Items[i].ToString());
            }
            bool S = false;
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j]==State)
                {
                    S= true;
                    break;
                }
            }
            if (S)
            {
                uiComboBox1.SelectedIndex=uiComboBox1.Items.IndexOf(State);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uiComboBox2.SelectedIndex != -1)
            {
                updaterountId = uiComboBox2.SelectedIndex + 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            status = uiComboBox1.Text.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(uiTextBox2.Text.Trim()))
            {
                double.TryParse(uiTextBox2.Text, out updateScore);
                FrmModifyScoreWindowSys.Instance.SetFrmModifyScoreWindowBackData(status, updaterountId, updateScore);
                DialogResult = DialogResult.OK;
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
            Close();
            
        }
    }
}
