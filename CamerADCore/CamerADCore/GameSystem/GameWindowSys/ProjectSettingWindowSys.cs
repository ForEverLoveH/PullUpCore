using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameHelper;
using CamerADCore.GameSystem.GameWindow;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using LogHlper;
using Newtonsoft.Json.Linq;
using Sunny.UI;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class ProjectSettingWindowSys
    {
        public static ProjectSettingWindowSys Instance;
        private SQLiteHelper SQLiteHelper = null;
        private ProjectSettingWindow ProjectSettingWindow = null;

        public void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqLiteHelper"></param>
        /// <returns></returns>
        public bool ShowProjectSettingWindow(SQLiteHelper sqLiteHelper)
        {
            SQLiteHelper = sqLiteHelper;
            ProjectSettingWindow = new ProjectSettingWindow();
            DialogResult dia  =  ProjectSettingWindow.ShowDialog();
            if (dia == DialogResult.Yes || dia == DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectTreeView"></param>
        /// <param name="projects"></param>
        public void UpDataProjectListView(TreeView projectTreeView, ref List<ProjectModel> projects)
        {
            TreeViewHelper.Instance.UpdataGroupTreeView(projectTreeView, ref projects, SQLiteHelper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectName"></param>
        /// <param name="txtType"></param>
        /// <param name="roundCbx"></param>
        /// <param name="txtBestScoreMode"></param>
        /// <param name="txtTestMethord"></param>
        /// <param name="txtFloatType"></param>
        /// <param name="txt_Type"></param>
        /// <param name="uiComboBox"></param>
        public void UpDataProjectAttribute(string name, UITextBox projectName, UIComboBox txtType, UIComboBox roundCbx, UIComboBox txtBestScoreMode, UIComboBox txtTestMethord, UIComboBox txtFloatType, UIComboBox txt_Type, ref string projectID)
        {
            var ds = SQLiteHelper.ExecuteReader("SELECT spi.Name,spi.Type,spi.RoundCount,spi.BestScoreMode,spi.TestMethod,spi.FloatType,spi.Id " +
                                                $"FROM SportProjectInfos AS spi WHERE spi.Name='{name}'");
            while (ds.Read())
            {
                string Name = ds.GetString(0);
                int Type = ds.GetInt16(1);
                int RoundCount = ds.GetInt16(2);
                int BestScoreMode = ds.GetInt16(3);
                int TestMethod = ds.GetInt16(4);
                int FloatType = ds.GetInt16(5);
                projectID = ds.GetValue(6).ToString();
                projectName.Text = Name;
                txt_Type.SelectedIndex = 0;
                roundCbx.SelectedIndex = RoundCount;
                txtBestScoreMode.SelectedIndex = BestScoreMode;
                txtTestMethord.SelectedIndex = TestMethod;
                txtFloatType.SelectedIndex = FloatType;
                break;
            }
              
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="ucDataGridView1"></param>
        /// <param name="projectID"></param>
        public void UpDataStudentDataView(string groupName, UCDataGridView ucDataGridView1, string projectID)
        {
            ucDataGridView1.DataSource = null;
            List<DataGridViewColumnEntity> lstCulumns = new List<DataGridViewColumnEntity>();
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "ID", HeadText = "序号", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "GroupName", HeadText = "组别名称", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "School", HeadText = "学校", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Grade", HeadText = "年级", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Class", HeadText = "班级", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Name", HeadText = "姓名", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Sex", HeadText = "性别", Width = 5, WidthType = SizeType.AutoSize, Format = (a) => { return ((int)a) == 0 ? "男" : "女"; } });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "IdNumber", HeadText = "准考证号", Width = 20, WidthType = SizeType.AutoSize });
            ucDataGridView1.Columns = lstCulumns;
            ucDataGridView1.IsShowCheckBox = true;
            List<object> lstSource = new List<object>();
            var ds = SQLiteHelper.ExecuteReaderList($"SELECT d.GroupName,d.SchoolName,d.GradeName,d.ClassNumber,d.Name,d.Sex,d.IdNumber " +
                                                    $"FROM DbPersonInfos AS d WHERE d.GroupName='{groupName}' AND d.ProjectId='{projectID}'");
            int i = 1;
            foreach (var item in ds)
            {
                DataGridViewModel model = new DataGridViewModel()
                {
                    ID = i.ToString(),
                    GroupName = item["GroupName"],
                    School = item["SchoolName"],
                    Grade = item["GradeName"],
                    Class = item["ClassNumber"],
                    Name = item["Name"],
                    Sex = Convert.ToInt32(item["Sex"]),
                    IdNumber = item["IdNumber"],
                };
                lstSource.Add(model);
                i++;
            }
            ucDataGridView1.DataSource = lstSource;
            ucDataGridView1.ReloadSource();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="count"></param>
        /// <param name="ucDataGridView1"></param>
        /// <returns></returns>
        public bool DeleteCurrentChooseStudentData(string text, int count, UCDataGridView ucDataGridView1)
        {
            try
            {
                var value = SQLiteHelper.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{text}'");
                string projectId = value.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewModel osoure = (DataGridViewModel)ucDataGridView1.SelectRows[i].DataSource;
                        var vpersonId = SQLiteHelper.ExecuteScalar(
                            $"SELECT  Id FROM DbPersonInfos WHERE ProjectId='{projectId}' and Name='{osoure.Name}' and IdNumber='{osoure.IdNumber}'");
                        //删除人
                        SQLiteHelper.ExecuteNonQuery($"DELETE FROM DbPersonInfos WHERE Id='{vpersonId}'");
                        //删除成绩
                        SQLiteHelper.ExecuteNonQuery($"DELETE FROM ResultInfos WHERE PersonId='{vpersonId}'");
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public bool DeleteCurrentChooseGroupData(string groupName, string projectName)
        {
            try
            {
                var value = SQLiteHelper.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{projectName}'");
                string projectId = value.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    //删除组
                    SQLiteHelper.ExecuteNonQuery(
                        $"DELETE FROM DbGroupInfos WHERE ProjectId='{projectId}' and Name='{groupName}'");
                    var ds = SQLiteHelper.ExecuteReader(
                        $"SELECT Id FROM DbPersonInfos WHERE ProjectId='{projectId}' AND GroupName='{groupName}'");
                    while (ds.Read())
                    {
                        var vpersonId = ds.GetValue(0).ToString();
                        ;
                        //删除成绩
                        SQLiteHelper.ExecuteNonQuery($"DELETE FROM ResultInfos WHERE PersonId='{vpersonId}'");
                    }

                    //删除人
                    SQLiteHelper.ExecuteNonQuery(
                        $"DELETE FROM DbPersonInfos WHERE ProjectId='{projectId}' AND GroupName='{groupName}'");
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CreateProjectWindow(string name)
        {
            try
            {
                string sql = $"select Id from SportProjectInfos where Name='{name}' LIMIT 1";
                var existProject = SQLiteHelper.ExecuteScalar(sql);
                int si = 1;
                var ds = SQLiteHelper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM SportProjectInfos").ToString();
                int.TryParse(ds, out si);

                if (existProject != null)
                {
                    UIMessageBox.ShowError("当前项目已经存在");
                    return false;
                }
                else
                {
                    sql = $"INSERT INTO SportProjectInfos (CreateTime, SortId, IsRemoved, Name, Type, RoundCount, BestScoreMode, TestMethod, FloatType ) " + $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{si}," +
                  $"0,'{name}',0,2,0,0,2)";
                    int result =SQLiteHelper.ExecuteNonQuery(sql);
                    if(result == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }catch(Exception exception)
            {
                LoggerHelper.Debug(exception);
                return false;
            }
        }
        public bool DeleteProjectData(string projectName)
        {
            try
            {
                var value = SQLiteHelper.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{projectName}'");
                string projectId = value.ToString();

                int result = SQLiteHelper.ExecuteNonQuery($"DELETE FROM SportProjectInfos WHERE Id = '{projectId}'");
                if (result == 1)
                {
                    SQLiteHelper.ExecuteNonQuery($"DELETE FROM DbGroupInfos WHERE ProjectId = '{projectId}'");
                    SQLiteHelper.ExecuteNonQuery($"DELETE FROM DbPersonInfos WHERE ProjectId = '{projectId}'");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception exception) { 
                LoggerHelper.Debug(exception);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool ShowPersonImportDataWindow(string projectName)
        {
            return PersonDataImportWindowSys.Instance.ShowPersonImportDataWindow(projectName, SQLiteHelper);
        }

        public bool  SaveProjectSetting(string name0, string name, int type, int roundCount, int bestScoreMode, int testMethod, int floatType)
        {
            try
            { 
                string projectID = SQLiteHelper.ExecuteScalar($"select Id from SportProjectInfos where Name='{name0}'").ToString();

                string sql = $"UPDATE SportProjectInfos SET Name='{name}', Type={type},RoundCount={roundCount},BestScoreMode={bestScoreMode},TestMethod={testMethod},FloatType={floatType} where Id='{projectID}'";
                int result = SQLiteHelper.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
               LoggerHelper.Debug(e);
               return false;
            }
            
        }
    }
}