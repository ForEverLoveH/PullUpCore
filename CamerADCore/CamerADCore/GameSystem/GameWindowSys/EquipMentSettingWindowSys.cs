using CameraADCoreModel.ADCoreSqlite;
using CamerADCore.GameSystem.GameWindow;
using HZH_Controls.Forms;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameModel;
using LogHlper;
using Newtonsoft.Json;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class EquipMentSettingWindowSys
    {
        public static EquipMentSettingWindowSys Instance;
        private SQLiteHelper SQLiteHelper = null;
        private  EquipMentSettingWindow EquipMentSettingWindow = null;
        public void Awake()
        {
            Instance = this;
        }
        public void ShowEquipMentSettingWindow(SQLiteHelper sQLiteHelper)
        {
            SQLiteHelper= sQLiteHelper;
            EquipMentSettingWindow= new EquipMentSettingWindow();
            EquipMentSettingWindow.Show();
        }

        public void LoadingInitData(ref List<Dictionary<string, string>> localInfos, ref Dictionary<string, string> localValues, UIComboBox uiComboBox2, UIComboBox uiComboBox3, UIComboBox uiComboBox1)
        {
            localInfos = SQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
            if (localInfos.Count > 0)
            {
                string MachineCode = String.Empty;
                string ExamId = String.Empty;
                string Platform = String.Empty;
                string Platforms = String.Empty;
                foreach (var item in localInfos)
                {
                    localValues.Add(item["key"], item["value"]);
                    switch (item["key"])
                    {
                        case "MachineCode":
                            MachineCode = item["value"];
                            break;
                        case "ExamId":
                            ExamId = item["value"];
                            break;
                        case "Platform":
                            Platform = item["value"];
                            break;
                        case "Platforms":
                            Platforms = item["value"];
                            break;
                    }
                }

                if (string.IsNullOrEmpty(MachineCode))
                {
                   UIMessageBox.ShowError("设备码为空");
                }
                else
                {
                  uiComboBox1.Text = MachineCode;
                }

                if (string.IsNullOrEmpty(ExamId))
                {
                    UIMessageBox.ShowError( "考试id为空");
                }
                else
                {
                  uiComboBox3.Text = ExamId;
                }
                if (string.IsNullOrEmpty(Platforms))
                {
                    UIMessageBox.ShowError( "平台码为空");
                }
                else
                {
                    string[] Platformss = Platforms.Split(';');
                  uiComboBox2.Items.Clear();
                    foreach (var item in Platformss)
                    {
                       uiComboBox1.Items.Add(item);
                    }

                }
                if (string.IsNullOrEmpty(Platform))
                {
                    UIMessageBox.ShowError("平台码为空");
                }
                else
                {
                    uiComboBox2.Text = Platform;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiComboBox2"></param>
        /// <param name="uiComboBox3"></param>
        /// <param name="uiComboBox1"></param>
        /// <param name="localValues"></param>
        /// <returns></returns>
        public bool LoadingEquipMentCodeData(UIComboBox uiComboBox2, UIComboBox uiComboBox3, UIComboBox uiComboBox1,
            Dictionary<string, string> localValues)
        {
            try
            {
                uiComboBox1.Items.Clear();
                string examId = uiComboBox2.Text;
                if (string.IsNullOrEmpty(examId))
                {
                    UIMessageBox.ShowError("考试id为空!");
                    return false;
                }
                if (examId.IndexOf('_') != -1)
                {
                    examId = examId.Substring(examId.IndexOf('_') + 1);
                }
                string url =uiComboBox2.Text.Trim();
                if (string.IsNullOrEmpty(url))
                {
                    UIMessageBox.ShowError("网址为空!");
                    return false;
                }
                url += RequestUrl.GetMachineCodeListUrl;
                RequestParameter RequestParameter = new RequestParameter();
                RequestParameter.AdminUserName = localValues["AdminUserName"];
                RequestParameter.TestManUserName = localValues["TestManUserName"];
                RequestParameter.TestManPassword = localValues["TestManPassword"];
                RequestParameter.ExamId = examId;
                //序列化
                string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);
                var formDatas = new List<FormItemModel>();
                //添加其他字段
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = JsonStr
                });
                var httpUpload = new HttpUpload();
                string result = HttpUpload.PostForm(url, formDatas);
                GetMachineCodeList upload_Result = JsonConvert.DeserializeObject<GetMachineCodeList>(result);
                if (upload_Result == null || upload_Result.Results == null || upload_Result.Results.Count == 0)
                {
                    string error = string.Empty;
                    try
                    {
                        error = upload_Result.Error;

                    }
                    catch (Exception exception)
                    {
                        LoggerHelper.Debug(exception);
                        error = string.Empty;
                    }

                    UIMessageBox.ShowError($"提交错误,错误码:[{error}]");
                    return false;
                }
                foreach (var item in upload_Result.Results)
                {
                    string str = $"{item.title}_{item.MachineCode}";
                    uiComboBox1.Items.Add(str);
                }
                return true;
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
        /// <param name="platform"></param>
        /// <param name="examId"></param>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public bool SavePlatformSetting(string platform, string examId, string machineCode)
        {
            try
            {
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = SQLiteHelper.BeginTransaction();
                SQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{platform}' WHERE key = 'Platform'");
                SQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{examId}' WHERE key = 'ExamId'");
                SQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{machineCode}' WHERE key = 'MachineCode'"); 
                SQLiteHelper.CommitTransaction(ref sQLiteTransaction);
                return true;
            }
            catch (Exception e)
            {
               LoggerHelper.Debug(e);
               return false;
            }
             
        }
    }
}
