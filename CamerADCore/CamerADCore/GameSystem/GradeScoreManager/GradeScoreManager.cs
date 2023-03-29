using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameModel;
using LogHlper;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamerADCore.GameSystem.GameHelper;

namespace CamerADCore.GameSystem 
{
    public class GradeScoreManager
    {
        public static GradeScoreManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <param name="groupName"></param>
        /// <param name="SQLiteHelper"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool OutPutScore(string projectId, string projectName, string groupName, SQLiteHelper SQLiteHelper,
            bool flag = false)
        {
            bool result = false;
            try
            {
                SaveFileDialog saveImageDialog = new SaveFileDialog();
                saveImageDialog.Filter = "xlsx file(*.xlsx)|*.xlsx";
                saveImageDialog.RestoreDirectory = true;
                string path = Application.StartupPath +
                              $"\\excel\\output{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                Dictionary<string, string> dic =
                    SQLiteHelper.ExecuteReaderOne(
                        $"SELECT RoundCount,Name,Type FROM SportProjectInfos WHERE Id='{projectId}' ");
                string names = dic["Name"];
                string typeName = "仰卧起坐";
                if (dic["Type"] == "1") typeName = "引体向上";
                saveImageDialog.FileName = $"{projectName}_{typeName}成绩_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                if (saveImageDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveImageDialog.FileName;

                    if (dic.Count == 0)
                    {
                        UIMessageBox.Show("数据错误");
                        return false;
                    }

                    int.TryParse(dic["RoundCount"], out int RoundCount);
                    List<Dictionary<string, string>> ldic = new List<Dictionary<string, string>>();
                    //序号 项目名称    组别名称 姓名  准考证号 考试状态    第1轮 第2轮 最好成绩
                    string sql =
                        $"SELECT BeginTime, Id, GroupName, Name, IdNumber,State,Sex FROM DbPersonInfos WHERE ProjectId='{projectId}' ";
                    if (!flag)
                    {
                        sql += $" AND GroupName = '{groupName}'";
                    }

                    List<Dictionary<string, string>> list1 = SQLiteHelper.ExecuteReaderList(sql);
                    int step = 1;
                    foreach (var item1 in list1)
                    {
                        Dictionary<string, string> dics = new Dictionary<string, string>();
                        dics.Add("序号", step + "");
                        dics.Add("项目名称", projectName);
                        dics.Add("组别名称", item1["GroupName"]);
                        dics.Add("姓名", item1["Name"]);
                        dics.Add("准考证号", item1["IdNumber"]);
                        dics.Add("性别", item1["Sex"] == "0" ? "男" : "女");
                        string state0 = ResultState.ResultState2Str(item1["State"]);
                        dics.Add("考试状态", state0);
                        List<Dictionary<string, string>> list2 = SQLiteHelper.ExecuteReaderList(
                            $"SELECT * FROM ResultInfos WHERE PersonId='{item1["Id"]}' And IsRemoved=0 ORDER BY RoundId ASC LIMIT {RoundCount}");
                        int step2 = 1;
                        double maxScore = 0;
                        foreach (var item2 in list2)
                        {
                            double.TryParse(item2["Result"], out double result0);
                            int.TryParse(item2["State"], out int state1);
                            if (result0 > maxScore) maxScore = result0;
                            if (state1 == 1)
                            {
                                dics.Add($"第{step2}轮", result0 + "");
                            }
                            else
                            {
                                dics.Add($"第{step2}轮", ResultState.ResultState2Str(state1));
                            }

                            step2++;
                        }

                        for (int i = step2; i <= RoundCount; i++)
                        {
                            dics.Add($"第{i}轮", "");
                        }

                        if (step2 > 1)
                        {
                            dics.Add($"最终成绩", maxScore + "");
                        }
                        else
                        {
                            dics.Add($"最终成绩", "");
                        }

                        ldic.Add(dics);
                        step++;
                    }

                    result = ExcelUtils.OutPutExcel(ldic, path);
                }

                return result;

            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }
        }

        /// <summary>
        /// 上传当前成绩
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="SQLiteHelper"></param>
        /// <param name="proMax"></param>
        /// <param name="proval"></param>
        /// <param name="ucProcessLine1"></param>
        /// <param name="timer1"></param>
        /// <returns></returns>
        public string UpLoadingCurrentScore(Object obj, SQLiteHelper SQLiteHelper, ref int proMax, ref int proval,
            HZH_Controls.Controls.UCProcessLine ucProcessLine1, Timer timer1)
        {
            try
            {
                List<Dictionary<string, string>> sucessList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> dis = SQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in dis)
                {
                    localInfos.Add(item["key"], item["value"]);
                }

                string[] fsp = obj as string[];
                string projectName = string.Empty;
                string groupName = string.Empty;
                if (fsp.Length > 0)
                {
                    projectName = fsp[0];
                }

                if (fsp.Length > 1)
                    groupName = fsp[1];
                Dictionary<string, string> SportProjectDic = SQLiteHelper.ExecuteReaderOne(
                    $"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                    $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                string sql = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}'";
                //查询本项目已考
                if (!string.IsNullOrEmpty(groupName))
                {
                    sql += $" and Name='{groupName}'";
                }

                List<Dictionary<string, string>> groups = SQLiteHelper.ExecuteReaderList(sql);
                UploadResultsRequestParameter urrp = new UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];
                string ExamId = localInfos["ExamId"];
                if (ExamId.IndexOf('_') != -1)
                {
                    ExamId = ExamId.Substring(ExamId.IndexOf('_') + 1);
                }

                urrp.ExamId = ExamId;
                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }

                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                proMax = groups.Count;
                proval = 0;
                ucProcessLine1.Visible = true;
                ucProcessLine1.Value = 0;
                timer1.Start();
                foreach (var sqlGroupsResult in groups)
                {
                    proval++;
                    String Sql =
                        $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                        $" WHERE ProjectId='{SportProjectDic["Id"]}' AND GroupName = '{sqlGroupsResult["Name"]}'";

                    List<Dictionary<string, string>> list = SQLiteHelper.ExecuteReaderList(Sql);
                    //轮次
                    int turn = 0;

                    if (list.Count > 0)
                    {
                        Dictionary<string, string> stu = list[0];
                        int.TryParse(SportProjectDic["RoundCount"], out turn);
                        urrp.MachineCode = MachineCode;
                    }
                    else
                    {
                        continue;
                    }

                    StringBuilder resultSb = new StringBuilder();
                    List<SudentsItem> sudentsItems = new List<SudentsItem>();
                    //IdNumber 对应Id
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    for (int i = 1; i <= turn; i++)
                    {
                        foreach (var stu in list)
                        {
                            List<RoundsItem> roundsItems = new List<RoundsItem>();
                            ///成绩
                            List<Dictionary<string, string>> resultScoreList1 = SQLiteHelper.ExecuteReaderList(
                                $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 And RoundId={i} LIMIT 1");

                            #region 查询文件

                            //成绩根目录
                            Dictionary<string, string> dic_images = new Dictionary<string, string>();
                            Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                            Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                            //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";

                            #endregion 查询文件

                            foreach (var item in resultScoreList1)
                            {
                                if (item["uploadState"] != "0") continue;
                                ///
                                DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                                string dateStr = dtBeginTime.ToString("yyyyMMdd");
                                string GroupNo = $"{dateStr}_{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                                //轮次成绩
                                RoundsItem rdi = new RoundsItem();
                                rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                                rdi.State = ResultState.ResultState2Str(item["State"]);
                                rdi.Time = item["CreateTime"];
                                double.TryParse(item["Result"], out double score);
                                if (item["State"] != "1") score = 0;
                                rdi.Result = score;
                                //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                                rdi.GroupNo = GroupNo;
                                rdi.Text = dic_texts;
                                rdi.Images = dic_images;
                                rdi.Videos = dic_viedos;
                                rdi.Memo = "";
                                rdi.Ip = "";
                                roundsItems.Add(rdi);
                            }

                            if (roundsItems.Count > 0)
                            {
                                SudentsItem ssi = new SudentsItem();
                                ssi.SchoolName = stu["SchoolName"];
                                ssi.GradeName = stu["GradeName"];
                                ssi.ClassNumber = stu["ClassNumber"];
                                ssi.Name = stu["Name"];
                                ssi.IdNumber = stu["IdNumber"];
                                ssi.Rounds = roundsItems;
                                sudentsItems.Add(ssi);
                                if (!map.Keys.Contains(stu["IdNumber"]))
                                {
                                    map.Add(stu["IdNumber"], stu["Id"]);
                                }
                            }
                        }

                    }

                    #region 上传数据包装

                    if (sudentsItems.Count == 0) continue;
                    urrp.Sudents = sudentsItems;
                    string jsonData = JsonConvert.SerializeObject(urrp);
                    string url = localInfos["Platform"] + RequestUrl.UploadResults;

                    var formDatas = new List<FormItemModel>();
                    //添加其他字段
                    formDatas.Add(new FormItemModel()
                    {
                        Key = "data",
                        Value = jsonData
                    });
                    logWirte.AppendLine();
                    logWirte.AppendLine();
                    logWirte.AppendLine(jsonData);
                    string res = string.Empty;
                    try
                    {
                        res = HttpUpload.PostForm(url, formDatas);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("网络异常！！");
                        break;
                    }

                    Upload_Results upLoad_Result = JsonConvert.DeserializeObject<Upload_Results>(res);
                    string errorStr = string.Empty;
                    var ret = upLoad_Result.Result;
                    foreach (var items in sudentsItems)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("Id", map[items.IdNumber]);
                        dic.Add("IdNumber", map[items.IdNumber]);
                        dic.Add("Name", items.Name);
                        dic.Add("uploadGroup", items.Rounds[0].GroupNo);
                        dic.Add("RoundId", items.Rounds[0].RoundId.ToString());
                        var value = 0;
                        ret.Find(a => a.TryGetValue(items.IdNumber, out value));
                        if (value == 1 || value == -4)
                        {
                            sucessList.Add(dic);
                        }
                        else if (value != 0)
                        {
                            errorStr = UpLoadResult.Match(value);
                            dic.Add("error", errorStr);
                            errorList.Add(dic);
                            messageSb.AppendLine(
                                $"{sqlGroupsResult["Name"]}组 考号:{items.IdNumber} 姓名:{items.Name}, 第{items.Rounds[0].RoundId}轮错误内容:{errorStr}");
                        }
                    }

                    #endregion
                }

                LoggerHelper.Monitor(logWirte.ToString());
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{sucessList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");
                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }

                sb.AppendLine("*****************error**********************");
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = SQLiteHelper.BeginTransaction();
                sb.AppendLine("******************success*********************");
                foreach (var item in sucessList)
                {
                    string sql1 =
                        $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]} and RoundId={item["RoundId"]}";
                    SQLiteHelper.ExecuteNonQuery(sql1);
                    //更新成绩上传状态
                    sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                    SQLiteHelper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }

                SQLiteHelper.CommitTransaction(ref sQLiteTransaction);
                sb.AppendLine("*******************success********************");
                string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                if (!Directory.Exists(txtpath))
                {
                    Directory.CreateDirectory(txtpath);
                }

                if (sucessList.Count != 0 || errorList.Count != 0)
                {
                    txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    File.WriteAllText(txtpath, sb.ToString());
                }

                string outpitMessage = messageSb.ToString();
                return outpitMessage;

            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
            finally
            {
                ucProcessLine1.Visible = false;
                ucProcessLine1.Value = 0;
                timer1.Stop();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="SQLiteHelper"></param>
        public   bool UpLoadingCurrentScore(object obj, SQLiteHelper SQLiteHelper,int _RoundCount)
        {
            try
            {
                string cpuid = CPUHelper.GetCpuID();
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = SQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in list0)
                {
                    localInfos.Add(item["key"], item["value"]);
                }
                string[] fusp = obj as string[];
                ///项目名称
                string projectName = string.Empty;
                //组
                string groupName = string.Empty;
                int nowRound = 0;
                if (fusp.Length > 0)
                    projectName = fusp[0];
                if (fusp.Length > 1)
                    groupName = fusp[1];
                if (fusp.Length > 2)
                    int.TryParse(fusp[2], out nowRound);
                Dictionary<string, string> SportProjectDic = SQLiteHelper.ExecuteReaderOne(
                    $"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                    $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                string sql0 = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}'";
                ///查询本项目已考组
                if (!string.IsNullOrEmpty(groupName))
                {
                    sql0 += $" AND Name = '{groupName}'";
                }
                List<Dictionary<string, string>> sqlGroupsResults = SQLiteHelper.ExecuteReaderList(sql0);
                UploadResultsRequestParameter urrp = new UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];
                string ExamId = localInfos["ExamId"];
                if (ExamId.IndexOf('_') != -1)
                {
                    ExamId = ExamId.Substring(ExamId.IndexOf('_') + 1);
                }
                urrp.ExamId = ExamId;
                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }
                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                ///按组上传
                foreach (var sqlGroupsResult in sqlGroupsResults)
                {
                    string sql =
                        $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                        $" WHERE ProjectId='{SportProjectDic["Id"]}' AND GroupName = '{sqlGroupsResult["Name"]}'";
                    List<Dictionary<string, string>> list = SQLiteHelper.ExecuteReaderList(sql);
                    //轮次
                    urrp.MachineCode = MachineCode;
                    if (list.Count == 0)
                    {
                        continue;
                    }
                    StringBuilder resultSb = new StringBuilder();
                    List<SudentsItem> sudentsItems = new List<SudentsItem>();
                    //IdNumber 对应Id
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    ///按轮次上传
                    for (int i = 1; i <= _RoundCount; i++)
                    {
                        foreach (var stu in list)
                        {
                            List<RoundsItem> roundsItems = new List<RoundsItem>();
                            ///成绩
                            List<Dictionary<string, string>> resultScoreList1 = SQLiteHelper.ExecuteReaderList(
                                $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos " +
                                $"WHERE PersonId='{stu["Id"]}' And IsRemoved=0 And RoundId={i} LIMIT 1");
                            #region 查询文件

                            //成绩根目录
                            Dictionary<string, string> dic_images = new Dictionary<string, string>();
                            Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                            Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                            //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";

                            #endregion 查询文件
                            foreach (var item in resultScoreList1)
                            {
                                //if (item["uploadState"] != "0") continue;
                                /// 可重复上传
                                DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                                string dateStr = dtBeginTime.ToString("yyyyMMdd");
                                string GroupNo = $"{dateStr}_{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                                //轮次成绩
                                RoundsItem rdi = new RoundsItem();
                                rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                                rdi.State = ResultState.ResultState2Str(item["State"]);
                                rdi.Time = item["CreateTime"];
                                double.TryParse(item["Result"], out double score);
                                if (item["State"] != "1") score = 0;
                                rdi.Result = score;
                                //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                                rdi.GroupNo = GroupNo;
                                rdi.Text = dic_texts;
                                rdi.Images = dic_images;
                                rdi.Videos = dic_viedos;
                                rdi.Memo = "";
                                rdi.Ip = "";
                                roundsItems.Add(rdi);
                            }
                            if (roundsItems.Count > 0)
                            {
                                SudentsItem ssi = new SudentsItem();
                                ssi.SchoolName = stu["SchoolName"];
                                ssi.GradeName = stu["GradeName"];
                                ssi.ClassNumber = stu["ClassNumber"];
                                ssi.Name = stu["Name"];
                                ssi.IdNumber = stu["IdNumber"];
                                ssi.Rounds = roundsItems;
                                sudentsItems.Add(ssi);
                                if (roundsItems.Count > 0)
                                {
                                    sudentsItems.Add(ssi);
                                    if (!map.Keys.Contains(stu["IdNumber"]))
                                    {
                                        map.Add(stu["IdNumber"], stu["Id"]);
                                    }
                                }
                            }
                        }
                    }
                    #region 上传数据包装

                    if (sudentsItems.Count == 0) continue;
                    urrp.Sudents = sudentsItems;

                    //序列化json
                    string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                    string url = localInfos["Platform"] + RequestUrl.UploadResults;

                    var httpUpload = new HttpUpload();
                    var formDatas = new List<FormItemModel>();
                    //添加其他字段
                    formDatas.Add(new FormItemModel()
                    {
                        Key = "data",
                        Value = JsonStr
                    });

                    logWirte.AppendLine();
                    logWirte.AppendLine();
                    logWirte.AppendLine(JsonStr);

                    //上传学生成绩
                    string result = HttpUpload.PostForm(url, formDatas);
                    Upload_Results upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject<Upload_Results>(result);
                    string errorStr = "null";
                    List<Dictionary<string, int>> result1 = upload_Result.Result;
                    foreach (var item in sudentsItems)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        //map
                        dic.Add("Id", map[item.IdNumber]);
                        dic.Add("IdNumber", item.IdNumber);
                        dic.Add("Name", item.Name);
                        dic.Add("uploadGroup", item.Rounds[0].GroupNo);
                        dic.Add("RoundId", item.Rounds[0].RoundId.ToString());
                        var value = 0;
                        result1.Find(a => a.TryGetValue(item.IdNumber, out value));
                        if (value == 1 || value == -4)
                        {
                            successList.Add(dic);
                        }
                        else if (value != 0)
                        {
                            errorStr = UpLoadResult.Match(value);
                            dic.Add("error", errorStr);
                            errorList.Add(dic);
                            messageSb.AppendLine(
                                $"{sqlGroupsResult["Name"]}组 考号:{item.IdNumber} 姓名:{item.Name},第{item.Rounds[0].RoundId}轮上传失败,错误内容:{errorStr}");
                        }
                    }

                    #endregion 上传数据包装
                }
                LoggerHelper.Monitor(logWirte.ToString());
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");
                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = SQLiteHelper.BeginTransaction();
                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    //更新成绩上传状态
                    string sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]} and RoundId={item["RoundId"]}";
                    SQLiteHelper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                SQLiteHelper.CommitTransaction(ref sQLiteTransaction);
                sb.AppendLine("*******************success********************");
                try
                {
                    string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                    if (!Directory.Exists(txtpath))
                    {
                        Directory.CreateDirectory(txtpath);
                    }

                    if (successList.Count != 0 || errorList.Count != 0)
                    {
                        txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                        File.WriteAllText(txtpath, sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }

                string outpitMessage = messageSb.ToString();
                outpitMessage = outpitMessage.Trim();
                if (string.IsNullOrEmpty(outpitMessage))
                {
                    outpitMessage = "上传成功";
                }
                
                MessageBox.Show(outpitMessage);
                return true;
                // return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                MessageBox.Show(ex.Message);
                return false;
                //return ex.Message;
            }
        }
    }
}

