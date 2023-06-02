
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameHelper;
using LogHlper;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class MainWindowSys
    {
        public static MainWindowSys Instance;
        public SQLiteHelper SQLiteHelper  =new SQLiteHelper();
       
        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupTreeView"></param>
        /// <param name="projectsModels"></param>
        public void UpdataGroupTreeView(TreeView groupTreeView, ref List<ProjectModel> projectsModels)
        {
            TreeViewHelper.Instance.UpdataGroupTreeView(groupTreeView, ref projectsModels, SQLiteHelper);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectName(string projectName)
        {
            return   SQLiteHelper.ExecuteReaderOne($"SELECT Id FROM SportProjectInfos WHERE Name='{projectName}'");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="projectName"></param>
        /// <param name="projectID"></param>
        /// <param name="studentView"></param>
        public void UpDataStudentDataView(string groupName, string projectName, ref string projectID,ListView studentView)
        {
            TreeViewHelper.Instance.UpDataStudentDataView(groupName,projectName,ref projectID,studentView,SQLiteHelper);
        }
        

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        public bool InitDataBase()
        {
            return SQLiteHelper.InitDataBase();
        }
        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <returns></returns>
        public bool ExportDataBase()
        {
            return SQLiteHelper.BackDataBase();
        }
         /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>
        public bool ShowProjectSettingWindow()
        {
           return ProjectSettingWindowSys.Instance.ShowProjectSettingWindow(SQLiteHelper);
        }

        /// <summary>
        /// 修成绩
        /// </summary>
        /// <param name="selectedListViewItemCollection"></param>
        /// <returns></returns>
        public bool ModifyScore(ListView  studentListView ,string projectId)
        {
            try
            {
                int index = studentListView.SelectedItems[0].Index;
                string projectName = studentListView.SelectedItems[0].SubItems[1].Text;
                string groupName = studentListView.SelectedItems[0].SubItems[2].Text;
                string Name = studentListView.SelectedItems[0].SubItems[3].Text;
                string IdNumber = studentListView.SelectedItems[0].SubItems[4].Text;
                string status = studentListView.SelectedItems[0].SubItems[5].Text;
                int rountid = 0;
                //查询项目数据信息
                Dictionary<string, string> SportProjectDic = SQLiteHelper.ExecuteReaderOne(
                    $"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                    $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                int FloatType = 0;
                if (SportProjectDic.Count > 0)
                {
                    FloatType = Convert.ToInt32(SportProjectDic["FloatType"]);
                    rountid = Convert.ToInt32(SportProjectDic["RoundCount"]);
                }
                if (FrmModifyScoreWindowSys.Instance.ShowFrmModifyScoreWindow(projectName, groupName, Name, IdNumber, status, rountid, projectId, SQLiteHelper))
                {
                    FrmModifyScoreWindowBackData FrmModifyScoreWindowBackData = FrmModifyScoreWindowSys.Instance.GetFrmModifyScoreWindowBackData();
                    if (FrmModifyScoreWindowBackData != null)
                    {
                        int roundid = FrmModifyScoreWindowBackData.RoundId;
                        double updateScore = FrmModifyScoreWindowBackData.UpdateScore;
                        decimal.Round(decimal.Parse(updateScore.ToString("0.0000")), FloatType).ToString();
                        string updatestatus = FrmModifyScoreWindowBackData.Status;
                        int Resultinfo_State = ResultState.ResultState2Int(updatestatus);
                        string perid = "";
                        var ds0 = SQLiteHelper.ExecuteReaderOne(
                            $"SELECT Id FROM DbPersonInfos WHERE IdNumber='{IdNumber}' and ProjectId='{projectId}'");
                        if (ds0 == null || ds0.Count == 0) return false;
                        perid = ds0["Id"];
                        string sql = $"UPDATE ResultInfos SET Result={updateScore},State={Resultinfo_State} WHERE PersonId='{perid}' AND RoundId={roundid}";
                        int result = SQLiteHelper.ExecuteNonQuery(sql);
                        if (result == 0)
                        {
                            if (string.IsNullOrEmpty(perid))
                            {
                                return false;
                            }
                            sql =
                                $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                                $"VALUES (datetime(CURRENT_TIMESTAMP, 'localtime') ,(SELECT MAX(SortId)+1 FROM ResultInfos),0," +
                                $"'{perid}',0,'{Name}','{IdNumber}',{roundid},{updateScore},{Resultinfo_State})";
                            int result0 = SQLiteHelper.ExecuteNonQuery(sql);
                        }
                        else if (result > 1)
                        {
                            return false;
                        }
                        sql = $"UPDATE DbPersonInfos  SET State=1,FinalScore=1 WHERE ProjectId='{projectId}' AND IdNumber='{IdNumber}'";
                        SQLiteHelper.ExecuteNonQuery(sql);
                    }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="treeGroupText"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        public bool ShowExportGradeWindow(string projectId, string treeGroupText, string productName)
        {
             return ExportGradeWindowSys.Instance.ShowExportGradeWindow(projectId, treeGroupText, productName,SQLiteHelper);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="proMax"></param>
        /// <param name="proVal"></param>
        /// <param name="ucProcessLine1"></param>
        /// <param name="timer1"></param>
        /// <returns></returns>
        public string UpLoadingCurrentScore(Object obj, ref int proMax, ref int proVal, HZH_Controls.Controls.UCProcessLine ucProcessLine1, Timer timer1)
        {
            return GradeScoreManager.Instance.UpLoadingCurrentScore(obj,SQLiteHelper,ref proMax,ref proVal,ucProcessLine1,timer1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetCurrentGroupData(string data)
        {
             return SQLiteHelper.ExecuteReaderList($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                                                   $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{data}'");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fusp"></param>
        /// <param name="dic"></param>
        public bool ShowRunningTestingWindow(string[] fusp, Dictionary<string, string> dic)
        {
           return  RunningTestingWindowSys.Instance.ShowRunningTestingWindow(fusp, dic,SQLiteHelper);
        }
        /// <summary>
        /// 
        /// </summary>
        public void CloseSQLite()
        {
            SQLiteHelper.CloseDataBaseConnection();
        }
    }
}
