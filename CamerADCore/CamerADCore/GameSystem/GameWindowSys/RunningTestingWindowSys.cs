using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameHelper;
using CamerADCore.GameSystem.GameWindow;
using CamerADCore.GameSystem.MyControll;
using LogHlper;
using SpeechLib;
using Sunny.UI;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class RunningTestingWindowSys
    {
        public static RunningTestingWindowSys Instance;
        protected RunningTestingWindow RunningTestingWindow = null;
        private SQLiteHelper SQLiteHelper;

        public void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fusp"></param>
        /// <param name="dic"></param>
        /// <param name="sqLiteHelper"></param>
        public void ShowRunningTestingWindow(string[] fusp, Dictionary<string, string> dic, SQLiteHelper sqLiteHelper)
        {
            SQLiteHelper = sqLiteHelper;
            RunningTestingWindow = new RunningTestingWindow();
            RunningTestingWindow.ProjectName = fusp[0];
            if (fusp.Length > 1)
            {
                RunningTestingWindow.GroupName = fusp[1];
            }

            RunningTestingWindow.ProjectID = dic["Id"];
            RunningTestingWindow.Type = dic["Type"];
            RunningTestingWindow.RoundCount = Convert.ToInt32(dic["RoundCount"]);
            RunningTestingWindow.BestScoreMode = Convert.ToInt32(dic["BestScoreMode"]);
            RunningTestingWindow.TestMethod = Convert.ToInt32(dic["TestMethod"]);
            RunningTestingWindow.FloatType = Convert.ToInt32(dic["FloatType"]);
            RunningTestingWindow.formTitle = string.Format("考试项目:{0}", fusp[0]);
            RunningTestingWindow.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public SQLiteDataReader GetGroupData(string projectId, string groupName = "")
        {
            return SQLiteHelper.ExecuteReader(
                $"SELECT Name FROM DbGroupInfos WHERE Name LIKE'%{groupName}%' AND ProjectId='{projectId}'");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raceStudentDataLists"></param>
        /// <param name="serialReader"></param>
        public void MatchDataSendInfo(List<RaceStudentData> list, SerialReader serialReader,
            List<UserControl1> userControl)
        {
            try
            {
                if (serialReader == null || !serialReader.IsComOpen())
                {
                    UIMessageBox.ShowError("设备断开");
                    return;
                }
                else
                {
                    Byte[] bytesData = new Byte[] { 0xff };
                    //发送名单头码
                    Byte[] headCode = System.Text.Encoding.Default.GetBytes("name:");
                    Byte[] endByte = System.Text.Encoding.Default.GetBytes("end");
                    //发送名单结束码
                    Byte[] endCode = new Byte[endByte.Length + bytesData.Length];
                    endByte.CopyTo(endCode, 0);
                    bytesData.CopyTo(endCode, endByte.Length);
                    Byte[] WriteBufferALL = Enumerable.Repeat((Byte)0x00, 404).ToArray();
                    int li = 0;
                    Byte[] AbsenceList = System.Text.Encoding.Default.GetBytes("lack:");
                    Byte[] WriteAbsenceBufferALL = new byte[AbsenceList.Length + 20];
                    int abStep = AbsenceList.Length;
                    Array.Copy(AbsenceList, 0, WriteAbsenceBufferALL, 0, abStep);
                    //添加头码
                    for (int l = li; l < headCode.Length; l++)
                    {
                        WriteBufferALL[l] = headCode[l];
                        li++;
                    }

                    Byte[] contentByte = new byte[20];
                    int contentByteStep = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        string GName = list[i].name;
                        int state0 = userControl[i].panel_status;
                        if (state0 == 3)
                        {
                            contentByte[contentByteStep] = 0xee;
                            contentByteStep++;
                        }
                        else
                        {
                            contentByte[contentByteStep] = 0x00;
                            contentByteStep++;
                        }

                        if (GName.Trim().Length > 4)
                        {
                            GName = GName.Trim().Substring(0, 4);
                        }

                        Byte[] name1hex = System.Text.Encoding.Default.GetBytes(GName);
                        Byte[] name2hex = Enumerable.Repeat((Byte)0x00, 8).ToArray();
                        name1hex.CopyTo(name2hex, 0);
                        for (int ii = 0; ii < name2hex.Length; ii++)
                        {
                            WriteBufferALL[li] = name2hex[ii];
                            li++;
                        }
                    }

                    //添加结束码
                    for (int l = 0; l < endCode.Length; l++)
                    {
                        WriteBufferALL[li] = endCode[l];
                        li++;
                    }

                    Array.Copy(contentByte, 0, WriteAbsenceBufferALL, abStep, contentByte.Length);
                    Task.Run(() =>
                    {
                        //发送正常名单
                        serialReader.SendMessage(WriteBufferALL, li);
                        Thread.Sleep(1000);
                        //发送缺考
                        serialReader.SendMessage(WriteAbsenceBufferALL, 0);
                    });
                }
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <param name="roundCount"></param>
        /// <param name="currentGroupStudentData"></param>
        /// <param name="label13"></param>
        public void UpdataGroupListView(string projectId, string groupName, int roundCount,
            UIDataGridView currentGroupStudentData, UILabel label13)
        {
            try
            {
                var ds = SQLiteHelper.ExecuteReaderList(
                    $"SELECT Id,Name,IdNumber FROM DbPersonInfos WHERE ProjectId='{projectId}' and GroupName='{groupName}'");
                int listViewCount = 1;
                int len = ds.Count;
                DataGridViewRow[] rows = new DataGridViewRow[len];
                int sum = ds.Count;
                int finishSum = 0;
                foreach (var item in ds)
                {
                    string stuId = item["Id"];
                    string stuName = item["Name"];
                    string idNumber = item["IdNumber"];
                    DataGridViewRow dataGridViewRow = new DataGridViewRow();
                    dataGridViewRow.Cells.Add(GetNewDataGridViewCell(listViewCount, Color.Black, Color.White));
                    dataGridViewRow.Cells.Add(GetNewDataGridViewCell(stuName, Color.Black, Color.White));
                    var sl = SQLiteHelper.ExecuteReader(
                        $"SELECT PersonName,Result,State,uploadState  FROM ResultInfos WHERE PersonId='{stuId}' AND RoundId={roundCount}");
                    bool flag = true;
                    while (sl.Read())
                    {
                        finishSum++;
                        flag = false;
                        string personName = sl.GetString(0);
                        double res = (double)sl.GetValue(1);
                        int state = sl.GetInt32(2);
                        int upLoadState = sl.GetInt32(3);
                        if (state > 1)
                        {
                            string sstate = ResultState.ResultState2Str(state);
                            dataGridViewRow.Cells.Add(GetNewDataGridViewCell(sstate, Color.Red, Color.White));
                        }
                        else
                        {
                            dataGridViewRow.Cells.Add(GetNewDataGridViewCell(res.ToString(), Color.Black,
                                Color.MediumSpringGreen));
                        }

                        dataGridViewRow.Cells.Add(GetNewDataGridViewCell(idNumber, Color.Black, Color.White));
                        if (upLoadState == 1)
                        {
                            dataGridViewRow.Cells.Add(GetNewDataGridViewCell("已上传", Color.Green, Color.White));
                        }
                        else
                        {
                            dataGridViewRow.Cells.Add(GetNewDataGridViewCell("未上传", Color.Red, Color.White));
                        }

                        break;
                    }

                    if (flag)
                    {
                        dataGridViewRow.Cells.Add(GetNewDataGridViewCell("无成绩", Color.Black, Color.White));
                        dataGridViewRow.Cells.Add(GetNewDataGridViewCell(idNumber, Color.Black, Color.White));
                        dataGridViewRow.Cells.Add(GetNewDataGridViewCell("未上传", Color.Red, Color.White));
                    }

                    dataGridViewRow.Cells.Add(GetNewDataGridViewCell(stuId, Color.Black, Color.White));
                    rows[listViewCount - 1] = dataGridViewRow;
                    listViewCount++;
                }

                currentGroupStudentData.Rows.Clear();
                currentGroupStudentData.Rows.AddRange(rows);
                int mcont = sum - finishSum;
                mcont = mcont < 0 ? 0 : mcont;
                label13.Text = $"{groupName}组内还有{mcont}人未参加 测试！！";
                if (mcont > 0)
                {
                    label13.ForeColor = Color.Red;
                }
                else
                {
                    label13.ForeColor = Color.Black;
                }
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        private DataGridViewCell GetNewDataGridViewCell(object value, Color foreColor, Color backColor)
        {
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = value.ToString();
            cell.Style.ForeColor = foreColor;
            cell.Style.BackColor = backColor;
            return cell;
        }

        /// <summary>
        /// 刷新串口
        /// </summary>
        /// <param name="cmbComPort"></param>
        public void RefreshSerialPortConnect(ComboBox cmbComPort, string portn = "USB Serial Port")
        {
            try
            {
                cmbComPort.Items.Clear();
                cmbComPort.Text = " ";
                string[] PortNames = SerialPortManager.Instance.GetPortDeviceName(portn);
                if (PortNames.Length == 0)
                {
                    portn = "USB-SERIAL";
                    PortNames = SerialPortManager.Instance.GetPortDeviceName(portn);
                }

                if (PortNames.Length == 0)
                {
                    portn = "USB-to-Serial";
                    PortNames = SerialPortManager.Instance.GetPortDeviceName(portn);
                }

                if (PortNames != null && PortNames.Length > 0)
                {
                    foreach (string portName in PortNames)
                    {
                        cmbComPort.Items.Add(SerialPortManager.Instance.PortNameToPort(portName));
                    }
                }

                if (cmbComPort.Items.Count > 0)
                {
                    cmbComPort.SelectedIndex = cmbComPort.Items.Count - 1;
                }
                else
                {
                    cmbComPort.SelectedIndex = -1;
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }

        /// <summary>
        /// 打开已连接的串口
        /// </summary>
        /// <param name="serialReader"></param>
        /// <param name="portName"></param>
        public void OpenConnectionSerial(SerialReader serialReader, string portName)
        {
            SerialPortManager.Instance.OpenSerialPortConnection(serialReader, portName);
        }

        /// <summary>
        /// 获取串口的数量
        /// </summary>
        /// <param name="serialReader"></param>
        public void GetMachineCount(SerialReader serialReader)
        {
            SerialPortManager.Instance.GetMachineNums(serialReader);
        }

        /// <summary>
        /// 语音播报
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rate"></param>
        public void VoiceOut(string str, int rate = 3)
        {
            Task.Run(() =>
            {
                SpVoice voice = new SpVoice();
                ISpeechObjectTokens obj = voice.GetVoices();
                voice.Voice = obj.Item(0);
                voice.Rate = rate;
                voice.Speak(str, SpeechVoiceSpeakFlags.SVSFIsXML | SpeechVoiceSpeakFlags.SVSFDefault);
            });
        }

        /// <summary>
        /// 发送数据到断口
        /// </summary>
        /// <param name="data"></param>
        public void SendDataToSerial(byte[] data)
        {
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// J接受来自端口的数据
        /// </summary>
        /// <param name="data"></param>
        public void ReceiveDataFromSerial(byte[] data)
        {
            SerialPortManager.Instance.ReceiveData(data);
        }

        /// <summary>
        /// 清空已经匹配的学生数据
        /// </summary>
        /// <param name="userControl1S"></param>
        /// <param name="raceStudentDataLists"></param>
        public void ClearMatchUser(List<UserControl1> userControl1S, ref List<RaceStudentData> raceStudentDataLists)
        {
            try
            {
                raceStudentDataLists.Clear();
                for (int i = 0; i < 10; i++)
                {
                    string str = String.Empty;
                    userControl1S[i].panel_name = str;
                    userControl1S[i].panel_Score = str;
                    userControl1S[i].panel_status = 0;
                    userControl1S[i].panel_idNumber = str;
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
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <param name="currentGroupStudentData"></param>
        /// <param name="raceStudentDatas"></param>
        /// <param name="equipMentCount"></param>
        /// <param name="currentRoundCount"></param>
        /// <param name="userControl1s"></param>
        /// <param name="autoMatchFlag"></param>
        /// <param name="serialReader"></param>
        public void AutoMatchStudentData(string projectId, string groupName, UIDataGridView currentGroupStudentData,
            ref List<RaceStudentData> raceStudentDatas, int equipMentCount, int currentRoundCount,
            List<UserControl1> userControl1s, ref bool autoMatchFlag, SerialReader serialReader)
        {
            try
            {
                currentGroupStudentData.Rows.Clear();
                var sl = SQLiteHelper.ExecuteReaderList(
                    $"SELECT Id,Name,IdNumber FROM DbPersonInfos WHERE ProjectId='{projectId}' and GroupName='{groupName}'");
                int listviewCount = 1;
                int len = sl.Count;
                DataGridViewRow[] dataRows = new DataGridViewRow[len];
                List<int> chooseList = new List<int>();
                foreach (var dic in sl)
                {
                    string stuId = dic["Id"];
                    string stuName = dic["Name"];
                    string idNumber = dic["IdNumber"];
                    DataGridViewRow dgr = new DataGridViewRow();
                    dgr.Cells.Add(GetNewDataGridViewCell(listviewCount, Color.Black, Color.White));
                    dgr.Cells.Add(GetNewDataGridViewCell(stuName, Color.Black, Color.White));
                    var ds = SQLiteHelper.ExecuteReaderList(
                        $"SELECT PersonName,Result,State,uploadState FROM ResultInfos WHERE PersonId='{stuId}' AND RoundId={currentRoundCount}");
                    bool flag = true;
                    foreach (var row in ds)
                    {
                        flag = false;
                        string PersonName0 = row["PersonName"];
                        double.TryParse(row["Result"], out double Result0);
                        int.TryParse(row["State"], out int State0);
                        int.TryParse(row["uploadState"], out int uploadState0);
                        if (State0 > 1)
                        {
                            string sstate = ResultState.ResultState2Str(State0);
                            dgr.Cells.Add(GetNewDataGridViewCell(sstate, Color.Red, Color.White));
                        }
                        else
                        {
                            dgr.Cells.Add(GetNewDataGridViewCell(Result0.ToString(), Color.Black,
                                Color.MediumSpringGreen));
                        }

                        dgr.Cells.Add(GetNewDataGridViewCell(idNumber, Color.Black, Color.White));
                        if (uploadState0 == 1)
                        {
                            dgr.Cells.Add(GetNewDataGridViewCell("已上传", Color.Green, Color.White));
                        }
                        else
                        {
                            dgr.Cells.Add(GetNewDataGridViewCell("未上传", Color.Red, Color.White));
                        }

                        break;
                    }

                    if (flag)
                    {
                        dgr.Cells.Add(GetNewDataGridViewCell("无成绩", Color.Black, Color.White));
                        dgr.Cells.Add(GetNewDataGridViewCell(idNumber, Color.Black, Color.White));
                        dgr.Cells.Add(GetNewDataGridViewCell("未上传", Color.Black, Color.White));
                        if (raceStudentDatas.Count < equipMentCount)
                        {
                            raceStudentDatas.Add(new RaceStudentData()
                            {
                                RaceStudentDataId = listviewCount,
                                id = stuId,
                                name = stuName,
                                idNumber = idNumber,
                                score = 0,
                                state = 0,
                                RoundId = currentRoundCount,
                            });
                            chooseList.Add(listviewCount - 1);
                        }
                    }

                    dgr.Cells.Add(GetNewDataGridViewCell(stuId, Color.Black, Color.White));
                    dataRows[listviewCount - 1] = dgr;
                    listviewCount++;
                }

                currentGroupStudentData.Rows.AddRange(dataRows);
                int lens = currentGroupStudentData.Rows.Count;
                for (int i = 0; i < len; i++)
                {
                    if (chooseList.Contains(i))
                    {
                        currentGroupStudentData.Rows[i].Selected = true;
                    }
                    else
                    {
                        currentGroupStudentData.Rows[i].Selected = false;
                    }
                }

                AutoMatchStudentUserControll(raceStudentDatas, userControl1s, ref autoMatchFlag, serialReader);
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raceStudentDatas"></param>
        /// <param name="userControl1S"></param>
        /// <param name="autoMatchFlag"></param>
        /// <param name="serialReader"></param>
        private void AutoMatchStudentUserControll(List<RaceStudentData> raceStudentDatas,
            List<UserControl1> userControl1S, ref bool autoMatchFlag, SerialReader serialReader)
        {
            try
            {
                autoMatchFlag = true;
                int len = 10;
                if (len > raceStudentDatas.Count) len = raceStudentDatas.Count;
                List<RaceStudentData> raceStudentData = new List<RaceStudentData>();
                for (int i = 0; i < len; i++)
                {
                    var rs = raceStudentDatas[i];
                    userControl1S[i].panel_name = rs.name;
                    userControl1S[i].panel_Score = 0.ToString();
                    userControl1S[i].panel_status = rs.state;
                    userControl1S[i].panel_idNumber = rs.idNumber;
                    raceStudentData.Add(rs);
                }

                if (len == 0)
                {
                    UIMessageBox.ShowWarning("本轮已经全部测试完成！！");
                }
                else
                {
                    MatchDataSendInfoFun(raceStudentDatas, autoMatchFlag, serialReader, userControl1S);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 发送名单
        /// </summary>
        public void MatchDataSendInfoFun(List<RaceStudentData> raceStudentDatas, bool autoMatchFlag,
            SerialReader SerialReader, List<UserControl1> _userControl1s)
        {
            try
            {
                if (raceStudentDatas.Count == 0) return;
                if (!autoMatchFlag) return;
                RunningTestingWindowSys.Instance.MatchDataSendInfo(raceStudentDatas, SerialReader, _userControl1s);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentGroupStudentData"></param>
        /// <param name="raceStudentDataLists"></param>
        /// <param name="currentRoundCount"></param>
        /// <param name="userControl1S"></param>
        public void SelectMatchStudentData(UIDataGridView currentGroupStudentData,
            ref List<RaceStudentData> raceStudentDataLists, int currentRoundCount, List<UserControl1> userControl1S,
            ref bool auto, SerialReader serialReader)
        {
            try
            {
                bool flag = true;
                int index = 0;
                for (int i = 0; i < currentGroupStudentData.SelectedRows.Count; i++)
                {
                    index = currentGroupStudentData.SelectedRows.Count - 1 - i;
                    string stuid = currentGroupStudentData.SelectedRows[index].Cells[5].Value.ToString();
                    string stuName = currentGroupStudentData.SelectedRows[index].Cells[1].Value.ToString();
                    string score = currentGroupStudentData.SelectedRows[index].Cells[2].Value.ToString();
                    string idNumber = currentGroupStudentData.SelectedRows[index].Cells[1].Value.ToString();
                    if (score != "无成绩")
                    {
                        flag = false;
                        UIMessageBox.ShowWarning("选中的考生已经参加过考试！！");
                        raceStudentDataLists.Clear();
                        break;
                    }

                    raceStudentDataLists.Add(new RaceStudentData()
                    {
                        RaceStudentDataId = i + 1,
                        id = stuid,
                        name = stuName,
                        idNumber = idNumber,
                        score = 0,
                        RoundId = currentRoundCount,
                    });
                }

                if (flag)
                {
                    AutoMatchStudentUserControll(raceStudentDataLists, userControl1S, ref auto, serialReader);
                }
                else
                {
                    ClearMatchUser(userControl1S, ref raceStudentDataLists);
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }

        /// <summary>
        /// 查看端口是否连接
        /// </summary>
        /// <param name="serialReader"></param>
        /// <returns></returns>
        public bool IsSerialOpenConnection(SerialReader serialReader)
        {
            return SerialPortManager.Instance.IsSerialOpenConnection(serialReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userControl1S"></param>
        /// <param name="RaceStudentDataLists"></param>
        public void UpdateRaceStudentDataListsScore(List<UserControl1> userControl1S,
            List<RaceStudentData> RaceStudentDataLists)
        {
            for (int i = 0; i < RaceStudentDataLists.Count; i++)
            {
                string ScoreStr = userControl1S[i].panel_Score;
                int.TryParse(ScoreStr, out int Score);
                int State = userControl1S[i].panel_status;
                RaceStudentDataLists[i].state = State;
                RaceStudentDataLists[i].score = Score;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raceStudentDataLists"></param>
        /// <param name="currentRoundCount"></param>
        /// <returns></returns>
        public bool WriteScoreIntoDataBase(List<RaceStudentData> raceStudentDataLists, int currentRoundCount)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = SQLiteHelper.BeginTransaction();
                foreach (var item in raceStudentDataLists)
                {
                    string sql =
                        $"select Result from ResultInfos where PersonId='{item.id}' and RoundId='{currentRoundCount}' and IsRemoved=0";
                    var sl = SQLiteHelper.ExecuteReaderList(sql);
                    int state = item.state == 0 ? 1 : item.state;
                    if (sl.Count == 0)
                    {
                        var dl = SQLiteHelper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                        string sortId = 1.ToString();
                        if (dl != null && dl.ToString() != "")
                        {
                            sortId = dl.ToString();
                        }

                        sql =
                            $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                            $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortId}, 0, '{item.id}',0,'{item.name}','{item.idNumber}',{currentRoundCount},{item.score},{state})";
                        SQLiteHelper.ExecuteNonQuery(sql);
                        SQLiteHelper.ExecuteNonQuery(
                            $"UPDATE DbPersonInfos SET State = 1, FinalScore = 1 WHERE Id = '{item.id}'");
                    }
                    else
                    {
                        stringBuilder.Append($"{item.idNumber},{item.name} 本轮已测试\n");
                    }
                }

                SQLiteHelper.CommitTransaction(ref sQLiteTransaction);
                if (stringBuilder.Length > 0)
                {
                    UIMessageBox.Show(stringBuilder.ToString());
                }

                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return false;
            }
        }

        /// <summary>
        /// 上传成绩
        /// </summary>
        /// <param name="o"></param>

        public bool UpLoadCurrentGroupGradea(object obj, int RoundCount)
        {
            return GradeScoreManager.Instance.UpLoadingCurrentScore(obj, SQLiteHelper, RoundCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <param name="currentRoundCount"></param>
        /// <param name="stuView"></param>
        /// <param name="frmModifyScoreOneRoundShow"></param>
        /// <param name="currentGroupStudentData"></param>
        /// <returns></returns>
        public bool ShowModifyWindow(string projectId, string projectName, string groupname, int currentRoundCount,
            UIDataGridView stuView, ref bool frmModifyScoreOneRoundShow)
        {
            try
            {

                string idNumber = stuView.SelectedRows[0].Cells[3].Value.ToString();
                string name = stuView.SelectedRows[0].Cells[1].Value.ToString();
                string id = stuView.SelectedRows[0].Cells[5].Value.ToString();
                if (frmModifyScoreOneRoundShow)
                {
                    UIMessageBox.ShowError("未处理上一个数据");
                    return false;
                }
                else
                {
                    if (FrmModifyScoreOneRoundSys.Instance.ShowFrmModifyScoreOneRound(SQLiteHelper, projectId,
                            projectName, currentRoundCount, idNumber, name, groupname))
                    {
                        var sl = FrmModifyScoreOneRoundSys.Instance.GetFrmModifyScoreWindowBackData();
                        string score = sl.score;
                        string perid = sl.projectId;
                        int istate = sl.iState;
                        if (istate > 1)
                        {
                            string sql =
                                $"UPDATE ResultInfos SET Result='0',State='{istate}' WHERE PersonId='{perid}' AND RoundId='{currentRoundCount}'";
                            SQLiteHelper.ExecuteNonQuery(sql);
                        }
                        else if (istate == 1)
                        {
                            string sql =
                                $"UPDATE ResultInfos SET Result='{score}' WHERE PersonId='{perid}' AND RoundId='{currentRoundCount}'";
                            SQLiteHelper.ExecuteNonQuery(sql);
                        }
                        if (currentRoundCount > 0)
                        {
                            try
                            {
                                File.AppendAllText("操作日志.txt",
                                    $"考号:{idNumber},姓名:{name},修正成绩:第{currentRoundCount}轮成绩为{score}");
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Debug(ex);
                            }
                        }
                        return true;

                    }
                    else
                    {
                        return false;
                    }


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
        /// <param name="currentGroupStudentData"></param>
        /// <param name="currentRoundCount"></param>
        /// <param name="state"></param>
        public bool SetStudentState(UIDataGridView stuView, int currentRoundCount, string state)
        {
            try
            {
                if (stuView.SelectedRows.Count == 0)
                {
                    UIMessageBox.ShowError("未选择考生");
                    return false;
                }

                int state0 = ResultState.ResultState2Int(state);
                string idNumber = stuView.SelectedRows[0].Cells[3].Value.ToString();
                string name = stuView.SelectedRows[0].Cells[1].Value.ToString();
                string id = stuView.SelectedRows[0].Cells[5].Value.ToString();
                string sql = $"UPDATE DbPersonInfos SET State=1,FinalScore=1 WHERE Id='{id}'";
                int result = SQLiteHelper.ExecuteNonQuery(sql);
                sql =
                    $"UPDATE ResultInfos SET State={state0},Result='0' WHERE PersonId='{id}' AND RoundId={currentRoundCount} AND IsRemoved=0";
                result = SQLiteHelper.ExecuteNonQuery(sql);

                if (result == 0)
                {
                    var sortid = SQLiteHelper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                    string sortid0 = "1";
                    if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();

                    sql =
                        $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                        $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{id}',0,'{name}','{idNumber}',{currentRoundCount},{0},{state0})";
                    //处理写入数据库
                    SQLiteHelper.ExecuteNonQuery(sql);
                }
                return true;
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return false; 
            }
        }
    }
}
