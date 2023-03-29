using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using LogHlper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamerADCore.GameSystem.GameHelper
{
    public class TreeViewHelper
    {
        public static TreeViewHelper Instance;
        public void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupTreeView"></param>
        /// <param name="projectsModels"></param>
        /// <param name="SQLiteHelper"></param>
        public void UpdataGroupTreeView(TreeView groupTreeView, ref List<ProjectModel> projectsModels ,SQLiteHelper SQLiteHelper)
        {
            try
            {
                projectsModels.Clear();
                groupTreeView.Nodes.Clear();
                var dsReader = SQLiteHelper.ExecuteReader($"SELECT Id,Name FROM SportProjectInfos");
                while (dsReader.Read())
                {
                    string projectID = dsReader.GetValue(0).ToString();
                    string ProjectName = dsReader.GetString(1);
                    var ds = SQLiteHelper.ExecuteReader(
                        $"SELECT Name,IsAllTested FROM DbGroupInfos WHERE ProjectId='{projectID}'");
                    projectsModels.Add(new ProjectModel()
                    {
                        projectName = ProjectName,
                        GroupModels = new List<GroupModel>(),
                    });
                    while (ds.Read())
                    {
                        string GroupName = ds.GetString(0);
                        int IsAllTested = ds.GetInt32(1);
                        ProjectModel ppModel = projectsModels.FirstOrDefault(a => a.projectName == ProjectName);
                        if (ppModel != null)
                        {
                            ppModel.GroupModels.Add(new GroupModel()
                            { GroupName = GroupName, IsAllTested = IsAllTested });
                        }
                        else
                        {
                            projectsModels.Add(new ProjectModel()
                            {
                                GroupModels = new List<GroupModel>
                                {
                                    new GroupModel() { GroupName = GroupName, IsAllTested = IsAllTested }
                                },
                                projectName = ProjectName
                            });
                        }
                    }
                }

                for (int i = 0; i < projectsModels.Count; i++)
                {
                    TreeNode treeNode = new TreeNode(projectsModels[i].projectName);
                    List<GroupModel> groupModels = projectsModels[i].GroupModels;
                    for (int j = 0; j < groupModels.Count; j++)
                    {
                        treeNode.Nodes.Add(groupModels[j].GroupName);
                    }
                    groupTreeView.Nodes.Add(treeNode);
                    for (int j = 0; j < groupModels.Count; j++)
                    {
                        if (groupModels[j].IsAllTested != 0)
                        {
                            groupTreeView.Nodes[i].Nodes[j].BackColor = Color.MediumSpringGreen;
                        }
                        else
                        {
                            groupTreeView.Nodes[i].Nodes[j].ForeColor = Color.Black;
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="projectName"></param>
        /// <param name="projectID"></param>
        /// <param name="studentView"></param>
        /// <param name="SQLiteHelper"></param>
        public void UpDataStudentDataView(string groupName, string projectName, ref string projectID,
            ListView studentView, SQLiteHelper SQLiteHelper)
        {
            try
            {
                var ds = SQLiteHelper.ExecuteReader(
                    $"SELECT b.Id,b.RoundCount,b.FloatType,b.Type FROM DbGroupInfos AS a,SportProjectInfos AS b WHERE a.ProjectId=b.Id AND a.Name='{groupName}' AND b.Name='{projectName}'");
                int roundCount = 0;
                int FloatType = 0;
                int type = 0;
                while (ds.Read())
                {
                    projectID = ds.GetValue(0).ToString();
                    roundCount = ds.GetInt16(1);
                    FloatType = ds.GetInt16(2);
                    type = ds.GetInt16(3);
                }

                var sl = SQLiteHelper.ExecuteReader(
                    ($"SELECT dpi.GroupName,dpi.Name,dpi.IdNumber,dpi.State,dpi.FinalScore,dpi.Id  FROM DbPersonInfos as dpi WHERE dpi.GroupName='{groupName}' AND dpi.ProjectId='{projectID}'"));
                int i = 1;
                studentView.BeginUpdate();
                studentView.Clear();
                studentView.Items.Clear();
                InitListViewHeader(roundCount, studentView);
                while (sl.Read())
                {
                    string idNum = sl.GetString(2);
                    int state = sl.GetInt16(3);
                    string personID = sl.GetValue(5).ToString();
                    ListViewItem li = new ListViewItem();
                    li.UseItemStyleForSubItems = false;
                    li.Text = i.ToString();
                    li.SubItems.Add(projectName);
                    li.SubItems.Add(sl.GetString(0));
                    li.SubItems.Add(sl.GetString(1));
                    li.SubItems.Add(idNum);
                    if (state == 1)
                    {
                        li.SubItems.Add("已测试");
                        li.SubItems[li.SubItems.Count - 1].BackColor = Color.MediumSpringGreen;
                    }
                    else
                    {
                        li.SubItems.Add("未测试");
                    }

                    bool isUpLoadeState = false;
                    double[] scoers = new double[roundCount];
                    double maxScore = 0;
                    if (type == 5)
                    {
                        maxScore = 0;
                    }
                    else
                    {
                        var res = SQLiteHelper.ExecuteReaderList(
                            ($"SELECT SortId,RoundId,Result,State,CreateTime,uploadState FROM ResultInfos WHERE PersonId='{personID}' ORDER BY RoundId LIMIT {roundCount}"));
                        if (res.Count > 0) isUpLoadeState = true;
                        for (int j = 0; j < res.Count; j++)
                        {
                            Dictionary<string, string> dic = res[j];
                            int.TryParse(dic["RoundId"], out int RoundID);
                            int.TryParse(dic["uploadState"], out int uploadState);
                            if (uploadState == 0)
                            {
                                isUpLoadeState = false;
                            }
                            double.TryParse(dic["Result"], out double Result);

                            if (maxScore < Result)
                            {
                                maxScore = Result;
                            }
                            scoers[RoundID - 1] = Result;
                        }
                    }
                    for (int j = 0; j < roundCount; j++)
                    {
                        string resultStr = "无成绩";
                        switch (scoers[j])
                        {
                            case 0:
                                resultStr = "无成绩";
                                break;
                            case -1:
                                resultStr = "犯规";
                                break;

                            case -2:
                                resultStr = "中退";
                                break;
                            case -3:
                                resultStr = "缺考";
                                break;
                            default:
                                resultStr = decimal.Round(decimal.Parse(scoers[j].ToString("0.0000")), FloatType)
                                    .ToString();
                                break;
                        }

                        li.SubItems.Add(resultStr);
                        if (scoers[j] == -1 || scoers[j] == -2 || scoers[j] == -3)
                            li.SubItems[li.SubItems.Count - 1].ForeColor = Color.Red;
                        else if (resultStr != "无成绩")
                        {
                            li.SubItems[li.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F,
                                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                        }
                    }
                    if (maxScore > 0)
                    {
                        li.SubItems.Add(decimal.Round(decimal.Parse(maxScore.ToString("0.0000")), FloatType)
                            .ToString());
                        li.SubItems[li.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F,
                            System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                    }
                    else
                    {
                        li.SubItems.Add("无成绩");
                    }

                    if (isUpLoadeState)
                    {
                        li.SubItems.Add("已上传");
                        li.SubItems[li.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F,
                            System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                        li.SubItems[li.SubItems.Count - 1].ForeColor = Color.Green;
                    }
                    else
                    {
                        li.SubItems.Add("未上传");
                        li.SubItems[li.SubItems.Count - 1].ForeColor = Color.Red;
                    }

                    studentView.Items.Insert(studentView.Items.Count, li);
                    i++;
                }

                //自动列宽
                AutoResizeColumnWidth(studentView);
                studentView.EndUpdate();
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }
         /// <summary>
        /// 自动适应列宽
        /// </summary>
        /// <param name="lv"></param>
        private void AutoResizeColumnWidth(ListView lv)
        {
            int allWidth = lv.Width;
            int count = lv.Columns.Count;
            int MaxWidth = 0;
            Graphics graphics = lv.CreateGraphics();
            int width;
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (count == 0) return;
            for (int i = 0; i < count; i++)
            {
                string str = lv.Columns[i].Text;
                //MaxWidth = lv.Columns[i].Width;
                MaxWidth = 0;

                foreach (ListViewItem item in lv.Items)
                {
                    try
                    {
                        str = item.SubItems[i].Text;
                        width = (int)graphics.MeasureString(str, lv.Font).Width;
                        if (width > MaxWidth)
                        {
                            MaxWidth = width;
                        }
                    }
                    catch (Exception)
                    {

                        break;
                    }

                }
                lv.Columns[i].Width = MaxWidth;
                allWidth -= MaxWidth;
            }
            if (allWidth > count && count != 0)
            {
                allWidth /= count;
                for (int i = 0; i < count; i++)
                {
                    lv.Columns[i].Width += allWidth;
                }
            }

        }

        /// <summary>
        /// 初始化表头
        /// </summary>
        /// <param name="roundCount"></param>
        /// <param name="studentView"></param>
        private void InitListViewHeader(int roundCount, ListView studentView)
        {
            studentView.View = View.Details;
            studentView.Columns.Clear();
            ColumnHeader[] columnHeaders = new ColumnHeader[200];
            int sp = 0;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "序号";
            columnHeaders[sp].Width = 80;
            sp++;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "项目名称";
            columnHeaders[sp].Width = 150;
            sp++;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "组别名称";
            columnHeaders[sp].Width = 150;
            sp++;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "姓名";
            columnHeaders[sp].Width = 150;
            sp++;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "准考证号";
            columnHeaders[sp].Width = 150;
            sp++;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "考试状态";
            columnHeaders[sp].Width = 80;
            sp++;
            for (int i = 1; i <=roundCount; i++)
            {
                columnHeaders[sp] = new ColumnHeader();
                columnHeaders[sp].Text = $"第{i}";
                columnHeaders[sp].Width = 80;
                sp++;
            }
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "最好成绩";
            columnHeaders[sp].Width = 80;
            sp++;
            columnHeaders[sp] = new ColumnHeader();
            columnHeaders[sp].Text = "上传状态";
            columnHeaders[sp].Width = 80;
            sp++;
            ColumnHeader[] col = new ColumnHeader[sp];
            for (int i = 0; i < col.Length; i++)
            {
                col[i] = columnHeaders[i];
            }
            studentView.Columns.AddRange(col);
        }
    }
}
