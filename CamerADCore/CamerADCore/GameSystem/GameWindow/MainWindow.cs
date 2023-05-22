using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.AutoSize;
using CamerADCore.GameSystem.GameWindowSys;
using HZH_Controls;
using HZH_Controls.Forms;
using LogHlper;
using Sunny.UI;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class MainWindow : Form
    {
        private List<ProjectModel> ProjectsModels = new List<ProjectModel>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private string projectId = string.Empty;
        private string projectName = string.Empty;
        private string treeGroupText = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            Control. CheckForIllegalCrossThreadCalls= false;
            asc.controllInitializeSize(this);
            MainWindowSys.Instance.UpdataGroupTreeView(GroupTreeView,ref ProjectsModels);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string name = e.Node.Text;
            if (string.IsNullOrEmpty(name))
            {
                UIMessageBox.ShowWarning("请确定项目组数据！！");
                return;
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.Node.Level == 0)
                    {
                        projectName = e.Node.Text;
                        Dictionary<string, string> dic = MainWindowSys.Instance.GetProjectName(projectName);
                        if (dic.Count > 0) projectId = dic["Id"];
                        StudentDataListview.Clear();
                    }
                    if (e.Node.Level == 1)
                    {
                        treeGroupText = e.Node.Text;
                        if(!string.IsNullOrEmpty(e.Node.Text))
                            MainWindowSys.Instance.UpDataStudentDataView(e.Node.Text,projectName,ref  projectId,StudentDataListview);
                        else
                        {
                            UIMessageBox.ShowWarning("请确定组号信息！！");
                            return;
                        }
                    }
                }
                else{return;}
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucNavigationMenu1_ClickItemed(object sender, EventArgs e)
        {
            string name = ucNavigationMenu1.SelectItem.Text;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            else
            {
                switch (name)
                {
                    case "启动测试":
                        string FullPaths = GroupTreeView.SelectedNode.FullPath;
                        if (!string.IsNullOrEmpty(FullPaths))
                        {
                            string[] fusp = FullPaths.Split('\\');
                            if (fusp.Length > 0)
                            {
                                List<Dictionary<string, string>> list = MainWindowSys.Instance.GetCurrentGroupData(fusp[0]);
                                if (list.Count == 1)
                                {
                                    try
                                    {
                                        Dictionary<string, string> dic = list[0];
                                        int.TryParse(dic["Type"], out int state);
                                        this.Hide();
                                        MainWindowSys.Instance.ShowRunningTestingWindow(fusp, dic);
                                        if (!string.IsNullOrEmpty(projectName))
                                        {
                                            HZH_Controls.ControlHelper.ThreadInvokerControl(this,
                                                () =>
                                                {
                                                    MainWindowSys.Instance.UpDataStudentDataView(treeGroupText, projectName,
                                                        ref projectId, StudentDataListview);
                                                });
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        LoggerHelper.Debug(exception);
                                        return;
                                    }
                                    finally
                                    {
                                        this.Show();
                                    }
                                }
                            }
                        }
                        else
                            return;
                        break;
                    case "项目设置":
                        MainWindowSys.Instance.ShowProjectSettingWindow();
                        MainWindowSys.Instance.UpdataGroupTreeView(GroupTreeView, ref ProjectsModels);
                        
                        break;
                    case "系统参数设置":
                        break;
                    case "初始化数据库":
                        DialogResult dialog =  MessageBox.Show("该操作将会将数据库初始化，是否继续？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (dialog == DialogResult.OK|| dialog==DialogResult.Yes)
                        {
                            if (MainWindowSys.Instance.InitDataBase())
                            {
                                UIMessageBox.ShowSuccess("数据库初始化成功");
                                HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                                    {
                                        MainWindowSys.Instance.UpdataGroupTreeView(GroupTreeView, ref ProjectsModels);
                                    });
                            }
                            else
                            {
                                UIMessageBox.ShowError("数据库初始化失败！！");
                                return;
                            }
                        }
                        else
                        {
                            UIMessageBox.ShowError("数据库初始化失败！！");
                            return;
                        }
                             
                        break;
                    case "数据库备份":
                        DialogResult di = MessageBox.Show("该操作将会将数据库备份，是否继续？", "提示", MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question);
                        if (di == DialogResult.OK || di == DialogResult.Yes)
                        {
                            if (MainWindowSys.Instance.ExportDataBase())
                            {
                                UIMessageBox.ShowSuccess("数据库备份成功！！");
                                return;
                            }
                            else
                            {
                                UIMessageBox.ShowError("数据库备份失败！！");
                                return;
                            }
                        }
                        else
                        {
                            UIMessageBox.ShowError("数据库备份失败！！");
                            return;
                        }
                        break;
                    case "导入成绩模板":
                        string paths = Application.StartupPath + "\\模板\\导入成绩模板.xls";
                        if (File.Exists(paths))
                            System.Diagnostics.Process.Start(paths);
                        else
                        {
                            UIMessageBox.ShowError("没有找到对应的文件");
                            return;
                        }
                        break;
                    case "导入名单模板":
                        string path = Application.StartupPath + "\\模板\\导入名单模板.xlsx";
                        if (File.Exists(path))
                            System.Diagnostics.Process.Start(path);
                        else
                        {
                            UIMessageBox.ShowError("没有找到对应的文件");
                            return;
                        }
                        break;
                     case "退出":
                         this.Close();
                         break;
                    case "修正成绩":
                        if (StudentDataListview.SelectedItems.Count > 0)
                        {
                            if (MainWindowSys.Instance.ModifyScore(StudentDataListview ,projectId))
                            {
                                UIMessageBox.ShowSuccess("修改成绩成功");
                                MainWindowSys.Instance.UpDataStudentDataView(treeGroupText,projectName,ref  projectId,StudentDataListview);
                            }
                            else
                            {
                                UIMessageBox.ShowError("修改成绩失败！！");
                                return;
                            }
                        }
                        break;
                    case "上传成绩":
                        UpLoadeScore();
                        break;
                    case"导出成绩":
                        if (MainWindowSys.Instance.ShowExportGradeWindow(projectId, treeGroupText, ProductName))
                        {
                            UIMessageBox.ShowSuccess("导出成功！！");
                        }
                        else
                        {
                            UIMessageBox.ShowError("导出失败");
                            return;
                        }
                        break;
                    
                }
            }
        }
        int proMax = 0;
        int proVal = 0;

        /// <summary>
        /// 
        /// </summary>
        private void UpLoadeScore()
        {
            if (GroupTreeView.SelectedNode != null)
            {
                string fullPath = GroupTreeView.SelectedNode.FullPath;
                string[] fsp = fullPath.Split('\\');
                string projectName = string.Empty;
                if (fsp.Length > 0)
                {
                    projectName = fsp[0];
                }
                if (string.IsNullOrEmpty(projectName))
                {
                    UIMessageBox.ShowError("请选择你需要上传的项目数据");
                    return;
                }
                Thread thread = new Thread(new ParameterizedThreadStart((o) =>
                {
                    try
                    {
                        string message = MainWindowSys.Instance.UpLoadingCurrentScore(fsp, ref proMax, ref proVal, ucProcessLine1, timer1);
                        message = message.Trim();
                        if (String.IsNullOrEmpty(message))
                        {
                            HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                            {
                                FrmTips.ShowTipsInfo(this, "上传结束");
                            });
                        }
                        else
                        {
                            MessageBox.Show(message);
                        }

                        if (!string.IsNullOrEmpty(projectName))
                        {
                            HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                            {
                                MainWindowSys.Instance.UpDataStudentDataView(treeGroupText, projectName, ref projectId, StudentDataListview);
                            });
                        };
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Debug(ex);
                    }
                    finally
                    {
                        HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            MainWindowSys.Instance.UpDataStudentDataView(treeGroupText, projectName, ref projectId, StudentDataListview);
                        });
                    }
                }));
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                UIMessageBox.ShowWarning("请先选择项目数据！！");
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (proMax == 0 || proMax == proVal || proVal == 0)
            {
                return;
            }
            int upV = (int)(((double)proVal / (double)proMax) * 100);
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                ucProcessLine1.Value = upV;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer1.Enabled)
            {
                DialogResult result = MessageBox.Show("有上传成绩未完成是否退出!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainWindowSys.Instance.CloseSQLite();
            Environment.Exit(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                TreeNode treeNode = GroupTreeView.GetNodeAt(e.X, e.Y);
                if(treeNode != null)
                {
                    GroupTreeView.SelectedNode = treeNode;
                }
            }
        }
        AutoSizeFormClass asc = new AutoSizeFormClass();
        

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
    }
}
