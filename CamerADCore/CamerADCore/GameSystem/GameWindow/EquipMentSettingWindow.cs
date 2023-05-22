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
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameModel;
using HZH_Controls.Forms;
using LogHlper;
using Newtonsoft.Json;
using Sunny.UI;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class EquipMentSettingWindow : Form
    {
        public EquipMentSettingWindow()
        {
            InitializeComponent();
        }
        public List<Dictionary<string, string>> localInfos = new List<Dictionary<string, string>>();
        public Dictionary<string, string> localValues = new Dictionary<string, string>();
        private void EquipMentSettingWindow_Load(object sender, EventArgs e)
        {
             
            EquipMentSettingWindowSys.Instance.LoadingInitData(ref localInfos,ref localValues  , uiComboBox2, uiCombox3, uiComboBox1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        { 
            var s=EquipMentSettingWindowSys.Instance.GetExamNumber(uiCombox3, uiComboBox1, uiComboBox2, localValues);
            if (s)
            {
                UIMessageBox.ShowSuccess("获取成功！！");
            }
            else
            {
                UIMessageBox.ShowError("获取失败！！");
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
           bool s  = EquipMentSettingWindowSys.Instance.LoadingEquipMentCodeData(uiComboBox2,uiCombox3,uiComboBox1,localValues);
           if (s)
           {
               UIMessageBox.ShowSuccess("获取成功！！");
           }
           else
           {
               UIMessageBox.ShowError("获取失败！！");
               return;
           }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            string Platform =uiComboBox2.Text;
            string ExamId = uiCombox3.Text;
            string MachineCode = uiComboBox1.Text;
            if (!string.IsNullOrEmpty(Platform) && !string.IsNullOrEmpty(ExamId) && !string.IsNullOrEmpty(MachineCode))
            {
                bool  S=   EquipMentSettingWindowSys.Instance.SavePlatformSetting(Platform, ExamId, MachineCode);
                if (S)
                {
                    UIMessageBox.ShowSuccess("保存成功");
                    this.Close();
                }
                else
                {
                    UIMessageBox.ShowError("保存失败！！");
                    return;
                }
            }
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
             this.Close();
        }
    }
}
