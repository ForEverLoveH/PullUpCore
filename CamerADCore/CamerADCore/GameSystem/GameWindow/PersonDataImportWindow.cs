using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamerADCore.GameSystem.GameWindowSys;
using HZH_Controls.Forms;
using Sunny.UI;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class PersonDataImportWindow : Form
    {
        public List<Dictionary<string, string>> localInfos = new List<Dictionary<string, string>>();
        public Dictionary<string, string> localValues = new Dictionary<string, string>();
        public static string projectName = string.Empty;
        
        private int proval = 0;
        private int proMax = 0;
        public PersonDataImportWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (PersonDataImportWindowSys.Instance.InitDataBase())
            {
                UIMessageBox.ShowSuccess("初始化数据库成功！！");
                
            }
            else
            {
                UIMessageBox.ShowError("初始化数据库成功！！");
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
            if (PersonDataImportWindowSys.Instance.ExportDataBase())
            {
                UIMessageBox.ShowSuccess("数据库备份成功！！");
                
            }
            else
            {
                UIMessageBox.ShowError("数据库备份失败！！！！");
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
            string path = PersonDataImportWindowSys.Instance.OpenLocalFile();
            if (string.IsNullOrEmpty(path))
            {
                UIMessageBox.ShowWarning("请选择正确的文件！！");
                return;
            }
            else
            {
                pathInput.Text = path;
            }
        }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void uibutton5_Click(object sender, EventArgs e)
         { 
             string path = pathInput.Text.Trim();
            if ( string.IsNullOrEmpty(path))
            {
                UIMessageBox.ShowWarning("请选择文件！！");
                return;
            }
            else
            {
                bool s =PersonDataImportWindowSys.Instance.ExcelListInputDataBase(path, ref proval, ref proMax,projectName);
                if (s)
                {
                    UIMessageBox.ShowSuccess("导入成功！！");
                    
                }
                else
                {
                    UIMessageBox.ShowError("导入失败！！");
                    return;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Filter = "xlsx file(*.xlsx)|*.xlsx|All files(*.*)|*.*";
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.FileName = $"导出模板{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string path = Application.StartupPath + "\\excel\\output.xlsx";

            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveImageDialog.FileName;
               System.IO. File.Copy(@"./模板/导入名单模板1.xlsx", path);
                FrmTips.ShowTipsSuccess(this, "导出成功");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            PersonDataImportWindowSys.Instance.ShowEquipMentSettingWindow();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (proMax != 0)
            {
                progressBar1.Maximum = proMax;
                if (proval > proMax)
                {
                    proval = proMax;
                    timer1.Stop();
                }
                progressBar1.Value = proval;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton7_Click(object sender, EventArgs e)
        {
            string ps = uiTextBox1.Text.Trim();
            if (!string.IsNullOrEmpty(ps))
            {
                PersonDataImportWindowSys.Instance.GetStudentDataFromServer(projectName, ps, localValues, ref proval,
                    ref proMax, timer1);
                
            }
            else
            {
                UIMessageBox.ShowError("请先确定你需要拉取的数目");
            }
        }

        private void PersonDataImportWindow_Load(object sender, EventArgs e)
        {
            PersonDataImportWindowSys.Instance.LoadingInitData(ref localInfos, ref localValues);
        }
    }
}
