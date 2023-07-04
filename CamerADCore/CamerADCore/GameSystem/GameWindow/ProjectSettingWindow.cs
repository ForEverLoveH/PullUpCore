using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameWindowSys;
using System.Collections.Generic;
using System.Windows.Forms;
using Sunny.UI;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using System;
using System.ComponentModel;
using System.IO;
using CamerADCore.GameSystem.AutoSize;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class ProjectSettingWindow : Form
    {
        public ProjectSettingWindow()
        {
            InitializeComponent();
        }
         private  List<ProjectModel> projects = new List<ProjectModel>();
         private string projectID = string.Empty;
        AutoSizeFormClass asc = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectSettingWindow_Load(object sender, System.EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
          ///   asc.controllInitializeSize(this);
           
            ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
        }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void ProjectTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string groupName = e.Node.Text;
                if (!string.IsNullOrEmpty(groupName))
                {
                    if (e.Node.Level == 0)
                    {
                        ProjectSettingWindowSys.Instance.UpDataProjectAttribute(groupName, txt_projectName, txt_Type,
                            txt_RoundCount, txt_BestScoreMode, txt_TestMethod, txt_FloatType, txt_Type, ref projectID);
                    }

                    if (e.Node.Level == 1)
                    {
                        uiTextBox1.Text = groupName;
                        ProjectSettingWindowSys.Instance.UpDataStudentDataView(groupName, ucDataGridView1 , projectID);
                    }
                }
            }
        }
        /// <summary>
        /// 删除选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, System.EventArgs e)
        {
            int count = ucDataGridView1.SelectRows.Count;
            if ( count <= 0)
            {
                UIMessageBox.ShowWarning("删除失败，请先选择你需要删除的学生数据！！");
                return;
            }
            else
            {
                if (ProjectSettingWindowSys.Instance.DeleteCurrentChooseStudentData(txt_projectName.Text,count,ucDataGridView1))
                {
                    UIMessageBox.ShowSuccess("删除成功！！");
                    ProjectSettingWindowSys.Instance.UpDataStudentDataView(txt_projectName.Text, ucDataGridView1 , projectID);
                }
                else
                {
                    UIMessageBox.ShowError("删除失败！！");
                    return;
                }
            }
        }

        /// <summary>
        /// 删除本组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, System.EventArgs e)
        {
            string projectName = txt_projectName.Text;
            string groupName = uiTextBox1.Text;
           
            if (ProjectSettingWindowSys.Instance.DeleteCurrentChooseGroupData(groupName,projectName))
            {
                UIMessageBox.ShowSuccess("删除成功！！");
                ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                ProjectSettingWindowSys.Instance.UpDataStudentDataView(txt_projectName.Text, ucDataGridView1 , projectID);
            }
            else
            {
                UIMessageBox.ShowSuccess("删除失败！！");
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(ProjectTreeView, e.Location);
            }

            if (e.Button == MouseButtons.Left)
            {
                TreeNode curr = ProjectTreeView.GetNodeAt(e.X, e.Y);
                if (curr != null)
                {
                    ProjectTreeView.SelectedNode = curr;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 插入项目ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            CreateProjectWindow createProjectWindow = new    CreateProjectWindow();
            var sl = createProjectWindow.ShowDialog();
            if ( sl ==DialogResult.Cancel)
            {
                string name = CreateProjectWindow.projectName;
                if (!string.IsNullOrEmpty(name))
                {
                    if (ProjectSettingWindowSys.Instance.CreateProjectWindow(name))
                    {
                        UIMessageBox.ShowSuccess("创建项目成功！！");
                        ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);

                    }
                    else
                    {
                        UIMessageBox.ShowError("创建项目失败！！");
                        return;
                    }
                }
                else
                {
                    UIMessageBox.ShowError("创建项目失败！！");
                    return;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除项目ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (ProjectTreeView.SelectedNode != null)
            {
                if (ProjectTreeView.SelectedNode.Level != 0)
                {
                    FrmTips.ShowTipsInfo(this, "请选择一个项目");
                    return;
                }
                else
                {
                    string projectName = ProjectTreeView.SelectedNode.Text;
                    DialogResult dialog = MessageBox.Show($"该操作将会删除{projectName}项目，是否继续？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes || dialog == DialogResult.OK)
                    {
                        if (ProjectSettingWindowSys.Instance.DeleteProjectData(projectName))
                        {
                            UIMessageBox.ShowSuccess($"项目{projectName}删除成功！！");
                            ucDataGridView1.DataSource = null;
                            ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                        }
                        else
                        {
                            UIMessageBox.ShowError($"项目{projectName}删除失败！！"); return;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 名单导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_projectName.Text.Trim())){
                if (ProjectSettingWindowSys.Instance.ShowPersonImportDataWindow(txt_projectName.Text))
                {
                    UIMessageBox.ShowSuccess("名单导入成功！！");
                    ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                }
                else
                {
                    UIMessageBox.ShowError("名单导入失败！！");
                    return;
                }
            }
            else
            {
                UIMessageBox.ShowWarning("请先确定项目信息！！");
                return;
            }
        }
        /// <summary>
        /// 模板导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Filter = "xlsx file(*.xlsx)|*.xlsx|All files(*.*)|*.*";
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.FileName = $"导出模板{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string path = Application.StartupPath + "\\excel\\output.xlsx";

            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveImageDialog.FileName;
                File.Copy(@"./模板/导入名单模板1.xlsx", path);
                FrmTips.ShowTipsSuccess(this, "导出成功");
            }

        }
        /// <summary>
        /// 保存项目设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, System.EventArgs e)
        {
            string name0 = ProjectTreeView.SelectedNode.Text;
            string Name = txt_projectName.Text;
            int Type = txt_Type.SelectedIndex;
            int RoundCount = txt_RoundCount.SelectedIndex;
            int BestScoreMode = txt_BestScoreMode.SelectedIndex;
            int TestMethod = txt_TestMethod.SelectedIndex;
            int FloatType = txt_FloatType.SelectedIndex;
            if (!string.IsNullOrEmpty(name0) && !string.IsNullOrEmpty(Name))
            {
                if (ProjectSettingWindowSys.Instance.SaveProjectSetting(name0, Name, Type, RoundCount, BestScoreMode, TestMethod, FloatType))
                {
                    UIMessageBox.ShowSuccess("保存成功！！");
                    ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                }
                else
                {
                    UIMessageBox.ShowError("保存失败！！");
                    return;
                }
            }
            else
            {
                UIMessageBox.ShowError("请先确定项目数据");
                return;
            }

        }

        private void ProjectSettingWindow_SizeChanged(object sender, EventArgs e)
        {
           // asc.controlAutoSize(this);
        }
    }
}