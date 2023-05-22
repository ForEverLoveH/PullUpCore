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
        AutoSizeFormClass asc = new AutoSizeFormClass();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectSettingWindow_Load(object sender, System.EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
             asc.controllInitializeSize(this);
           
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
        /// ɾ��ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, System.EventArgs e)
        {
            int count = ucDataGridView1.SelectRows.Count;
            if ( count <= 0)
            {
                UIMessageBox.ShowWarning("ɾ��ʧ�ܣ�����ѡ������Ҫɾ����ѧ�����ݣ���");
                return;
            }
            else
            {
                if (ProjectSettingWindowSys.Instance.DeleteCurrentChooseStudentData(txt_projectName.Text,count,ucDataGridView1))
                {
                    UIMessageBox.ShowSuccess("ɾ���ɹ�����");
                    ProjectSettingWindowSys.Instance.UpDataStudentDataView(txt_projectName.Text, ucDataGridView1 , projectID);
                }
                else
                {
                    UIMessageBox.ShowError("ɾ��ʧ�ܣ���");
                    return;
                }
            }
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, System.EventArgs e)
        {
            string projectName = txt_projectName.Text;
            string groupName = uiTextBox1.Text;
           
            if (ProjectSettingWindowSys.Instance.DeleteCurrentChooseGroupData(groupName,projectName))
            {
                UIMessageBox.ShowSuccess("ɾ���ɹ�����");
                ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                ProjectSettingWindowSys.Instance.UpDataStudentDataView(txt_projectName.Text, ucDataGridView1 , projectID);
            }
            else
            {
                UIMessageBox.ShowSuccess("ɾ��ʧ�ܣ���");
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
        private void ������ĿToolStripMenuItem_Click(object sender, System.EventArgs e)
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
                        UIMessageBox.ShowSuccess("������Ŀ�ɹ�����");
                        ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);

                    }
                    else
                    {
                        UIMessageBox.ShowError("������Ŀʧ�ܣ���");
                        return;
                    }
                }
                else
                {
                    UIMessageBox.ShowError("������Ŀʧ�ܣ���");
                    return;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ɾ����ĿToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (ProjectTreeView.SelectedNode.Level != 0)
            {
                FrmTips.ShowTipsInfo(this, "��ѡ��һ����Ŀ");
                return;
            }
            else
            {
                string projectName = ProjectTreeView.SelectedNode.Text;
                DialogResult dialog = MessageBox.Show($"�ò�������ɾ��{projectName }��Ŀ���Ƿ������", "��ʾ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes|| dialog==DialogResult.OK)
                { 
                    if(ProjectSettingWindowSys.Instance.DeleteProjectData(projectName))
                    {
                        UIMessageBox.ShowSuccess($"��Ŀ{projectName}ɾ���ɹ�����");
                        ucDataGridView1.DataSource = null;
                        ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                    }
                    else
                    {
                        UIMessageBox.ShowError($"��Ŀ{projectName}ɾ��ʧ�ܣ���"); return;
                    }
                }
            }

        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, System.EventArgs e)
        {
            if(ProjectSettingWindowSys.Instance.ShowPersonImportDataWindow(txt_projectName.Text))
            {
                 UIMessageBox.ShowSuccess("��������ɹ�����");
                 ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);  
            }
            else
            {
                UIMessageBox.ShowError("��������ʧ�ܣ���");
                return;
            }
        }
        /// <summary>
        /// ģ�嵼��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Filter = "xlsx file(*.xlsx)|*.xlsx|All files(*.*)|*.*";
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.FileName = $"����ģ��{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string path = Application.StartupPath + "\\excel\\output.xlsx";

            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveImageDialog.FileName;
                File.Copy(@"./ģ��/��������ģ��1.xlsx", path);
                FrmTips.ShowTipsSuccess(this, "�����ɹ�");
            }

        }
        /// <summary>
        /// ������Ŀ����
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
                    UIMessageBox.ShowSuccess("����ɹ�����");
                    ProjectSettingWindowSys.Instance.UpDataProjectListView(ProjectTreeView, ref projects);
                }
                else
                {
                    UIMessageBox.ShowError("����ʧ�ܣ���");
                    return;
                }
            }
            else
            {
                UIMessageBox.ShowError("����ȷ����Ŀ����");
                return;
            }

        }

        private void ProjectSettingWindow_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
    }
}