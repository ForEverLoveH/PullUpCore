using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameModel;
using CamerADCore.GameSystem.GameWindow;
using HZH_Controls;
using HZH_Controls.Forms;
using LogHlper;
using MiniExcelLibs;
using Newtonsoft.Json;
using Serilog.Core;
using Sunny.UI;
using Timer = System.Windows.Forms.Timer;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class PersonDataImportWindowSys
    {
        public static PersonDataImportWindowSys Instance;
        private PersonDataImportWindow PersonDataImportWindow = null;
        private SQLiteHelper SQLiteHelper = null;
        
        public void Awake() { Instance = this; }

        public bool ShowPersonImportDataWindow(string projectName, SQLiteHelper sqLiteHelper)
        {
            PersonDataImportWindow = new PersonDataImportWindow();
            SQLiteHelper = sqLiteHelper;
            PersonDataImportWindow.projectName = projectName;
            if (PersonDataImportWindow.ShowDialog() == DialogResult.OK)
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
        public bool InitDataBase()
        {
            return SQLiteHelper.InitDataBase();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ExportDataBase()
        {
            return SQLiteHelper.BackDataBase();
        }

        public string OpenLocalFile()
        {
            string path= String.Empty;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件 ！";
            fileDialog.InitialDirectory = Application.StartupPath + "\\";
            fileDialog.Filter=    "MicroSoft Excel文件(*.xlsx)|*.xlsx";       //筛选文件
            fileDialog.ShowHelp = true;     //是否显示“帮助”按钮
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = fileDialog.FileName;
            }
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="proval"></param>
        /// <param name="proMax"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public bool ExcelListInputDataBase(string path, ref int proval, ref int proMax,string projectName)
        {
            try
            {
                string projectID = SQLiteHelper
                    .ExecuteScalar($"select Id from SportProjectInfos where name='{projectName}'").ToString();
               var rows = MiniExcel.Query<InputData>(path).ToList();
               proval = 0;
               proMax = rows.Count;
               HashSet<string> hashSet = new HashSet<string>();
               for (int i = 0; i < rows.Count; i++)
               {
                   string[] examTime = rows[i].examTime.Split(' ');
                   hashSet.Add(rows[i].GroupName + "#" + examTime[0]);
               }
               List<String> rolesMarketList = new List<string>();
               rolesMarketList.AddRange(hashSet);

               System.Data.SQLite.SQLiteTransaction sQLiteTransaction = SQLiteHelper.BeginTransaction();
               for (int i = 0; i < rolesMarketList.Count; i++)
               {
                  string role = rolesMarketList[i];
                  string[] roles = role.Split('#');
                  string GroupName = roles[0];
                  string examTime = roles[1];
                  string countstr = SQLiteHelper.ExecuteScalar($"SELECT COUNT(*) FROM DbGroupInfos where ProjectId='{projectID}' and Name='{GroupName}'").ToString();
                  int.TryParse(countstr, out int count);
                  if (count == 0)
                  {
                     string groupsortidstr = SQLiteHelper.ExecuteScalar("select MAX( SortId ) + 1 from DbGroupInfos").ToString();
                     int groupsortid = 1;
                     int.TryParse(groupsortidstr, out groupsortid);
                     /* string insertsql = $"INSERT INTO DbGroupInfos(CreateTime,SortId,IsRemoved,ProjectId,Name,IsAllTested) " +
                          $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{groupsortid},0,'{projectid}','{GroupName}',0)";*/
                     string insertsql = $"INSERT INTO DbGroupInfos(CreateTime,SortId,IsRemoved,ProjectId,Name,IsAllTested) " +
                       $"VALUES('{examTime}',{groupsortid},0,'{projectID}','{GroupName}',0)";
                     //插入组
                     SQLiteHelper.ExecuteNonQuery(insertsql);
                  }
               }
               SQLiteHelper.CommitTransaction(ref sQLiteTransaction);
               int groupNum=(rows.Count+5000)/ 5000;
               List<List<InputData>> rowsSpiltList = SpiltList(rows, groupNum);
               foreach (var inputDatas in rowsSpiltList)
               {
                  sQLiteTransaction = SQLiteHelper.BeginTransaction();
                  foreach (var idata in inputDatas)
                  {
                     //InputData idata = rows[i];
                     string PersonIdNumber = idata.IdNumber;
                     string name = idata.Name;
                     int Sex = idata.Sex == "男" ? 0 : 1;
                     string SchoolName = idata.School;
                     string GradeName = idata.GradeName;
                     string classNumber = idata.ClassName;
                     string GroupName = idata.GroupName;
                     string[] examTimes = idata.examTime.Split(' ');
                     string examTime = examTimes[0];
                     string countstr = SQLiteHelper.ExecuteScalar($"SELECT COUNT(*) FROM DbPersonInfos WHERE ProjectId='{projectID}' AND IdNumber='{PersonIdNumber}'").ToString();
                     int.TryParse(countstr, out int count);
                     if (count == 0)
                     {
                        int personsortid = 1;
                        string personsortidstr = SQLiteHelper.ExecuteScalar("select MAX( SortId ) + 1 from DbPersonInfos").ToString();
                        int.TryParse(personsortidstr, out personsortid);
                        string insertsql = $"INSERT INTO DbPersonInfos(CreateTime,SortId,IsRemoved,ProjectId,SchoolName,GradeName,ClassNumber,GroupName,Name,IdNumber,Sex,State,FinalScore,uploadState) " +
                            $"VALUES('{examTime}',{personsortid},0,'{projectID}','{SchoolName}','{GradeName}','{classNumber}','{GroupName}'," +
                            $"'{name}','{PersonIdNumber}',{Sex},0,-1,0)";
                        SQLiteHelper.ExecuteNonQuery(insertsql);
                     }
                     proval++;
                  }
                  SQLiteHelper.CommitTransaction(ref sQLiteTransaction);
               }
               if (rows.Count == 0)
               {
                 return false;
               }
               else
               {
                  return true;
               }
            }
            catch (Exception e)
            {
               LoggerHelper.Debug(e);
               return false;
            }
        }

        public static List<List<T>> SpiltList<T>(List<T> Lists, int num) //where T:class
        {
            List<List<T>> fz = new List<List<T>>();
            //元素数量大于等于 分组数量
            if (Lists.Count >= num)
            {
                int avg = Lists.Count / num; //每组数量
                int vga = Lists.Count % num; //余数
                for (int i = 0; i < num; i++)
                {
                    List<T> cList = new List<T>();
                    if (i + 1 == num)
                    {
                        cList = Lists.Skip(avg * i).ToList<T>();
                    }
                    else
                    {
                        cList = Lists.Skip(avg * i).Take(avg).ToList<T>();
                    }
                    fz.Add(cList);
                }
            }
            else
            {
                fz.Add(Lists);//元素数量小于分组数量
            }
            return fz;
        }
        /// <summary>
        /// 
        /// </summary>
        
        public void ShowEquipMentSettingWindow()
        {
            EquipMentSettingWindowSys.Instance.ShowEquipMentSettingWindow(SQLiteHelper);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localInfos"></param>
        /// <param name="localValues"></param>
        public void LoadingInitData(ref List<Dictionary<string, string>> localInfos, ref Dictionary<string, string> localValues)
        {
            localInfos = new List<Dictionary<string, string>>();
            localInfos = SQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
            if (localInfos.Count > 0)
            {
                localValues = new Dictionary<string, string>();
                foreach (var item in localInfos)
                {
                    localValues.Add(item["key"], item["value"]);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="localValues"></param>
        /// <returns></returns>
        public void GetStudentDataFromServer( string ProjectName,string num, Dictionary<string, string> localValues,ref  int proval , ref  int proMax ,Timer timer)
        {
            try
            {
                RequestParameter RequestParameter = new RequestParameter();
                RequestParameter.AdminUserName = localValues["AdminUserName"];
                RequestParameter.TestManUserName = localValues["TestManUserName"];
                RequestParameter.TestManPassword = localValues["TestManPassword"];
                string ExamId0 = localValues["ExamId"];
                ExamId0 = ExamId0.Substring(ExamId0.IndexOf('_') + 1);
                string MachineCode0 = localValues["MachineCode"];
                MachineCode0 = MachineCode0.Substring(MachineCode0.IndexOf('_') + 1);
                string MachineCode1 = localValues["MachineCode1"];
                MachineCode1 = MachineCode1.Substring(MachineCode1.IndexOf('_') + 1);
                RequestParameter.ExamId = ExamId0;
                RequestParameter.MachineCode = MachineCode0;
                RequestParameter.GroupNums = num + "";
                //序列化
                string JsonStr = string.Empty;
                string url = localValues["Platform"] + RequestUrl.GetGroupStudentUrl;
                GetGroupStudent upload_Result = null;
                if (!string.IsNullOrEmpty(MachineCode0))
                {
                    JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);
                    //? 下载男
                    var formDatas = new List<FormItemModel>();
                    //添加其他字段
                    formDatas.Add(new FormItemModel()
                    {
                        Key = "data",
                        Value = JsonStr
                    });
                    string result = HttpUpload.PostForm(url, formDatas);
                    //GetGroupStudent upload_Result = JsonConvert.DeserializeObject<GetGroupStudent>(result);
                    string[] strs = GetGroupStudent.CheckJson(result);
                    if (strs[0] == "1")
                    {
                        upload_Result = JsonConvert.DeserializeObject<GetGroupStudent>(result);
                    }
                    else
                    {
                        upload_Result = new GetGroupStudent();
                        upload_Result.Error = strs[1];
                    }
                }
                GetGroupStudent studentList = new GetGroupStudent();
                studentList.Results = new Results();
                studentList.Results.groups = new List<GroupsItem>();
                bool bFlag = false;
                if (upload_Result == null || upload_Result.Results == null ||upload_Result.Results.groups.Count == 0)
                {
                    string error = string.Empty;
                    try
                    {
                        error = upload_Result.Error;
                    }
                    catch (Exception exception)
                    {
                        error = string.Empty;
                    }
                    UIMessageBox.ShowError( $"男生组提交错误,错误码:[{error}]");
                    return;
                }
                else
                {
                    bFlag = true;
                }
                int step1 = 0;
                int stepMax1 = 0;
                try
                {
                    stepMax1 = upload_Result.Results.groups.Count;
                }
                catch (Exception exception)
                {
                    LoggerHelper.Debug(exception);
                    stepMax1 = 0;
                }
                if (!bFlag) stepMax1 = 0;
                int step2 = 0;
                try
                {
                    if (upload_Result.Results.groups.Count > 0)
                    {
                        studentList.Results.groups.AddRange(upload_Result.Results.groups);
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }
                proval = 0;
                proMax = 0;
                if (studentList.Results.groups.Count > 0)
                {
                    timer.Start();
                    DownLoadStudentData(studentList, ref proval, ref proMax, ProjectName);
                }
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return  ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentList"></param>
        /// <param name="proVal"></param>
        /// <param name="proMax"></param>
        /// <param name="projectName"></param>
        private void DownLoadStudentData(GetGroupStudent studentList, ref int proVal, ref int proMax,
            string projectName)
        {
            List<GroupsItem> Groups = studentList.Results.groups;
            List<InputData> doc = new List<InputData>();
            int step = 1;
            proVal = 0;
            proMax = 0;
            //序号	学校	 年级	班级 	姓名	 性别	准考证号	 组别名称
            foreach (var Group in Groups)
            {
                string groupId = Group.GroupId;
                string groupName = Group.GroupName;
                foreach (var StudentInfo in Group.StudentInfos)
                {
                    InputData idata = new InputData();
                    idata.Id = step;
                    idata.School = StudentInfo.SchoolName;
                    idata.GradeName = StudentInfo.GradeName;
                    idata.ClassName = StudentInfo.ClassName;
                    idata.Name = StudentInfo.Name;
                    idata.Sex = StudentInfo.Sex;
                    idata.IdNumber = StudentInfo.IdNumber;
                    idata.GroupName = groupId;
                    idata.examTime = StudentInfo.examTime;
                    doc.Add(idata);
                    step++;
                }
            }
            string saveDir = Application.StartupPath + $"\\模板\\下载名单\\";
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            string path = Path.Combine(saveDir, $"downList{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            ExcelUtils.MiniExcel_OutPutExcel(path, doc);
            bool s =   ExcelListInputDataBase(path, ref proVal, ref proMax, projectName);
            if(s==true)
                UIMessageBox.ShowSuccess("拉取成功！！");
            else
            {
                UIMessageBox.ShowError("拉取失败！！");
                return;
            }
        }
    }
}
