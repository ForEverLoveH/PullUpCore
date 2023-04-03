using System;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameHelper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamerADCore.GameSystem.GameModel;
using CamerADCore.GameSystem.GameWindowSys;
using CamerADCore.GameSystem.MyControll;
using HZH_Controls;
using LogHlper;
using SpeechLib;
using Sunny.UI;
using Sunny.UI.Win32;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class RunningTestingWindow : Form
    {
        public string formTitle=String.Empty;
        public string ProjectID = string.Empty;
        public string Type = string.Empty;
        public RunningTestingWindow()
        {
            InitializeComponent();
        }
        public string ProjectName = string.Empty;
        public string GroupName = string.Empty;
        public int RoundCount = 1;
        public int CurrentRoundCount = 1;
        public int BestScoreMode = 1;
        public int TestMethod = 0;
        public int FloatType = -1;
        
        private int equipmentCount = 0;
        private bool autoMatchFlag = false;
        //是否播放开始语音
        private bool voiceFlag = true;
        //开始计时后恢复
        private bool titleFlag = false;
        //是否收到最终成绩
        private bool getFinalFlag = false;
        private Thread reconnectThread = null;
        private bool isReconnect = false;
        private bool reconnecting = false;
        private bool isSendMachineInfo = false;
        
        private  string connectionPortName = String.Empty;
        
        private List<bool> showTitleBools = new List<bool>();
        protected SerialReader SerialReader = null;
        private List<UserControl1> _userControl1s = null;
        private List<RaceStudentData> RaceStudentDataLists = new List<RaceStudentData>();
        private NFC_Helper USBWatcher = new NFC_Helper();
        
        private void RunningTestingWindow_Load(object sender, System.EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            LoadingInitData();
            RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID, GroupName, 1, CurrentGroupStudentData, label13);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitSerialPortData();
            USBWatcher.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 1)); 
            sw.Stop();
            Console.WriteLine($"-------{sw.ElapsedMilliseconds}");

        }
        /// <summary>
        /// usb监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void USBEventHandler(object sender, EventArrivedEventArgs e)
        {
            var watcher = sender as ManagementEventWatcher;
            watcher.Stop();
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                UIMessageBox.ShowWarning("设备未连接！！");
                return;
            } 
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {
                if  (SerialReader == null || !SerialReader.IsComOpen())
                {
                    ReConnnectionSerial();
                }
            }

            watcher.Start();
        }
        /// <summary>
        /// 重连串口
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ReConnnectionSerial()
        {
            if(isReconnect)return;
            isReconnect = true;
            reconnecting = true;
            UIMessageBox.ShowWarning("设备断开请检查");
            reconnectThread = new Thread(new ThreadStart(TryReconnectionSerial));
            reconnectThread.IsBackground = true;
            reconnectThread.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        private void TryReconnectionSerial()
        {
            isReconnect = true;
            reconnecting = true;
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                timer1.Stop();
                serialConnectStripStatusLabel1.Text = "重连中";
                serialConnectStripStatusLabel1.ForeColor = Color.Red;
            }); 
            try
            {
                while (isReconnect)
                {
                    ///重连
                    InitSerialPortData();
                    if ( SerialReader != null &&  SerialReader.IsComOpen())
                    {
                        isReconnect = false;
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            finally
            {
                isReconnect = false;
                reconnecting = false;
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    timer1.Start();
                });
            }
        }

        /// <summary>
        /// 初始化端口信息
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InitSerialPortData()
        {
            if (SerialReader != null)
            {
                SerialReader.CloseCom();
            }
            SerialReader = new SerialReader();
            SerialReader.AnalyCallback = AnalyDataFromSerial;
            SerialReader.ReceiveCallback = ReceiveDataFromSerial;
            SerialReader.SendCallback = SendDataToSerial;
            InitSerialPort();
        }
        /// <summary>
        /// 发送信息到端口
        /// </summary>
        /// <param name="data"></param>
        private void SendDataToSerial(byte[] data)
        {
            RunningTestingWindowSys.Instance.SendDataToSerial(data);
        }
        /// <summary>
        /// 接收来自端口的信息
        /// </summary>
        /// <param name="data"></param>
        private void ReceiveDataFromSerial(byte[] data)
        {
            RunningTestingWindowSys.Instance.ReceiveDataFromSerial(data);
        }

        /// <summary>
        /// 分析来自端口的信息
        /// </summary>
        /// <param name="data"></param>
        private void AnalyDataFromSerial(SerialMessageTran data)
        {
            if (SerialReader.qType == "paircount")
            {
                try
                {
                    byte[] analyData = data.btAryTranData;
                    string v = Encoding.UTF8.GetString(analyData);
                    int vNum = 0;
                    if (v.Contains("pairplaycount:") && analyData.Length == 17)
                    {
                        vNum = analyData[14];
                    }
                    if (vNum > 0 && vNum < 11)
                    {
                        SerialReader.machineNum = vNum;
                        vNum--;
                        equipmentCountCbx.SelectedIndex = vNum;
                    }
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    LoggerHelper.Debug(e);
                }
                return;
            }
            switch (data.strCmd)
            {
                case 0xFE:
                    try
                    {

                        int k = 0;
                        int nlen = data.ints0.Count;
                        nlen = nlen > _userControl1s.Count ? _userControl1s.Count : nlen;
                        foreach (var str in data.ints0)
                        {
                            int state0 = _userControl1s[k].panel_status;
                            string state1 = ResultState.ResultState2Str(state0);
                            if (state0 > 1)
                            {
                                _userControl1s[k].panel_Score = state1;
                            }
                            else
                            {
                                _userControl1s[k].panel_Score = str;
                            }
                            if (showTitleBools.Count > k && showTitleBools[k])
                            {
                                if (data.ints[k] == 1)
                                {
                                    _userControl1s[k].Panel_Ready = false;
                                    showTitleBools[k] = false;
                                }
                            }
                            k++;
                            if (k >= nlen) break;
                        }

                        TimeSpan ts = data.timeSpan;
                        if (ts.TotalMilliseconds > 0)
                        {
                            ControlHelper.ThreadInvokerControl(this, () =>
                            {
                                if (voiceFlag)
                                {
                                    RunningTestingWindowSys.Instance.  VoiceOut("开始考试", 2);
                                    showTitleBools.Clear();
                                    for (int i = 0; i < _userControl1s.Count; i++)
                                    {
                                        _userControl1s[i].Panel_Ready = true;
                                        showTitleBools.Add(true);
                                    }
                                    voiceFlag = false;
                                    titleFlag = true;
                                }
                                ucledNums1.Value =
                                    $"{String.Format("{0:00}", ts.Minutes)}:{String.Format("{0:00}", ts.Seconds)}";
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        LoggerHelper.Debug(exception);
                    }
                    break;
                case 0xFF:
                    try
                    {
                        autoMatchFlag = false;
                        //? 结束
                        StringBuilder writeLogSb = new StringBuilder();
                        int k1 = 0;
                        var list = data.ints1;
                        int nlen = list.Count;
                        if (nlen == 0)
                        {
                            if (SerialReader != null &&  SerialReader.IsComOpen())
                            {
                                string code = "Post All Score";
                                byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(code);
                                SerialReader.qType = "qStart";
                                SerialReader.SendMessage(WriteBufferALL);
                            }
                            return;
                        }
                        string nowTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        for (int i = 0; i < nlen; i++)
                        {
                            string str = list[i]; //score
                            string idnumber = _userControl1s[k1].panel_idNumber;
                            string stuName = _userControl1s[k1].panel_name;
                            int state0 = _userControl1s[k1].panel_status;
                            string stuState = ResultState.ResultState2Str(state0);
                            if (state0 > 1)
                            {
                               _userControl1s[k1].panel_Score = stuState;
                            }
                            else
                            {
                                if (state0 == 0)
                                {
                                   _userControl1s[k1].panel_status = 1;
                                }
                                _userControl1s[k1].panel_Score = str;
                            }
                            writeLogSb.AppendLine(
                                $"考号:{idnumber},姓名:{stuName},成绩:{str},状态:{stuState},测试时间:{nowTimeStr}");
                            k1++;
                        }
                        try
                        {
                            File.AppendAllText("成绩日志.txt", writeLogSb.ToString());
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Debug(ex);
                        }
                        ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            ucledNums1.Value = "01:00";
                            tsetLabel.Text = "测试结束";
                            btnExec.Enabled = true;
                            btnAutoMatch.Enabled = true;
                            btnSelectMatch.Enabled = true;
                            btnWriteScore.Enabled = true;
                            btnClearMatch.Enabled = true;
                            btnUpload.Enabled = true;
                            btnReGetScore.Enabled = true;
                        });
                        getFinalFlag = true;
                    }
                    catch (Exception ex)
                    {
                        getFinalFlag = false;
                        LoggerHelper.Debug(ex);
                    }
                    break;
            }
        }

        /// <summary>
        /// 初始化串口
        /// </summary>
        private void InitSerialPort()
        {
            RunningTestingWindowSys.Instance.RefreshSerialPortConnect(cmbComPort);
            OpenConnectionSerial();

        }
        /// <summary>
        /// 打开已连接的串口
        /// </summary>
        private void OpenConnectionSerial()
        {
            try
            {
                string port = cmbComPort.Text.Trim();
                if (SerialReader.IsComOpen())
                {
                    SerialReader.CloseCom();
                }
                else
                {
                    if (string.IsNullOrEmpty(port))
                    {
                        serialConnectStripStatusLabel1.Text = $"串口连接失败";
                        serialConnectStripStatusLabel1.ForeColor = Color.Red;
                    }
                    string portName = port;
                    if (!string.IsNullOrEmpty(portName))
                    {
                        RunningTestingWindowSys.Instance.OpenConnectionSerial(SerialReader,portName);
                    }
                }
                if (SerialReader.IsComOpen())
                {
                    connectionPortName = port;
                    serialConnectStripStatusLabel1.Text = $"串口：{port}已连接";
                    serialConnectStripStatusLabel1.ForeColor = Color.Green;
                    ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        button12.Text = "关闭串口";
                        uiGroupBox5.Enabled = true;
                        timer1.Start();
                    });
                    RunningTestingWindowSys.Instance.GetMachineCount(SerialReader);
                }
                else
                {
                    connectionPortName=String.Empty;
                    serialConnectStripStatusLabel1.Text = $"串口连接失败";
                    serialConnectStripStatusLabel1.ForeColor = Color.Red;
                    uiGroupBox5.Enabled = false;
                    button12.Text = "打开端口";
                }

            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }

        /// <summary>
        /// 加载初始信息
        /// </summary>
        private void LoadingInitData()
        {
            UpDataGroupComBox();
            UpDataRoundCountComBox();
            _userControl1s= new  List<UserControl1>()
            {
                userControl11,
                userControl12,
                userControl13,
                userControl14,
                userControl15,
                userControl16,
                userControl17,
                userControl18,
                userControl19,
                userControl110,
            };
            int len = _userControl1s.Count;
            for (int i = 0; i < _userControl1s.Count; i++)
            {
                _userControl1s[i].StateSwitchCallback = StateSwitchCallbackFun;
            }
            if (Type == "1") this.Text = "引体向上测试管理";
            else
            {
                this.Text = "仰卧起坐测试管理";
            }

            equipmentCountCbx.SelectedIndex = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        private void StateSwitchCallbackFun()
        {
            try
            {
                RunningTestingWindowSys.Instance.MatchDataSendInfoFun(RaceStudentDataLists,autoMatchFlag,SerialReader,_userControl1s);
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return;
            }
        }
        
        
        /// <summary>
        /// 更新轮次信息
        /// </summary>
        private void UpDataRoundCountComBox()
        {
            RoundCountCombox.Items.Clear();
            for (int i = 1; i <=RoundCount; i++)
            {
                RoundCountCombox.Items.Add($"第{i}轮" );
            }
            RoundCountCombox.SelectedIndex = 0;
        }
        /// <summary>
        /// 跟新组信息
        /// </summary>
        private void UpDataGroupComBox(string groupName="")
        {
            GroupCombox.Items.Clear();
            GroupCombox.Text = string.Empty;
            AutoCompleteStringCollection lstsourece = new AutoCompleteStringCollection();
            var sl=RunningTestingWindowSys.Instance.GetGroupData(ProjectID, groupName);
            while (sl.Read())
            {
               GroupCombox.Items.Add(sl.GetString(0));
                lstsourece.Add(sl.GetString(0));
            }
            GroupCombox.AutoCompleteCustomSource = lstsourece;
            int index = -1;
            GroupCombox.SelectedIndex = index;
            if (string.IsNullOrEmpty(GroupName) && GroupCombox.Items.Count > 0)
            {
                GroupName = GroupCombox.Items[0].ToString();
                GroupCombox.SelectedIndex = 0;
            }
            else
            {
                if ((index = GroupCombox.Items.IndexOf(GroupName)) >= 0)
                {
                   GroupCombox.SelectedIndex = index;
                }
            }
        }
        /// <summary>
        /// 写入成绩
        /// </summary>
        private void WriteScoreIntoDataBase()
        {
            if (!getFinalFlag)
            {
                DialogResult result = MessageBox.Show("未收到最终成绩,是否重取成绩!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(result!=DialogResult.Yes|| result!=DialogResult.OK) return;
                if (SerialReader != null && SerialReader.IsComOpen())
                {
                    string code = "Post All Score";
                    byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(code);
                    SerialReader.qType = "qStart";
                    SerialReader.SendMessage(WriteBufferALL);
                }
                else
                    UIMessageBox.ShowError("设备已断开");
                return;
            }
            RunningTestingWindowSys.Instance.UpdateRaceStudentDataListsScore(_userControl1s,RaceStudentDataLists);
            if (RaceStudentDataLists.Count == 0)
            {
                UIMessageBox.ShowError("数据异常");
                return;
            }

            bool sl =  RunningTestingWindowSys.Instance.WriteScoreIntoDataBase(RaceStudentDataLists, CurrentRoundCount);
            if (sl)
            {
                UIMessageBox.ShowSuccess("写入成功@！！");
                if (CurrentRoundCount>0)
                {
                    RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,uiLabel3);
                    RunningTestingWindowSys.Instance.ClearMatchUser(_userControl1s,ref  RaceStudentDataLists); 
                }
            }

        }
        /// <summary>
        /// 上传成绩
        /// </summary>
        /// <param name="obj"></param>
        private void UpLoadCurrentGroupGrade(object obj)
        {
            try
            {
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    btnUpload.BackColor = Color.Red;
                    btnUpload.Text = "上传中";
                    btnUpload.Enabled = false;
                });
                if (RunningTestingWindowSys.Instance.UpLoadCurrentGroupGradea(obj, RoundCount))
                {
                    UIMessageBox.ShowSuccess("上传成功！！");
                    if (CurrentRoundCount > 0)
                    {
                        RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID, GroupName, CurrentRoundCount, CurrentGroupStudentData, uiLabel3);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerHelper.Debug(e);
                return;
            }
            finally
            {
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    btnUpload.BackColor = System.Drawing.SystemColors.Control;
                    btnUpload.Text = "成绩上传";
                    btnUpload.Enabled = true;
                });
            }
        }
        #region  页面事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void equipmentCountCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                int index = equipmentCountCbx.SelectedIndex + 1;
                for (int i = 0; i < index; i++)
                {
                    _userControl1s[i].Visible = true;
                }
                if (index < _userControl1s.Count)
                {
                    for (int i = index; i < _userControl1s.Count; i++)
                    {
                        _userControl1s[i].Visible = false;
                    }
                }
                equipmentCount = index;
                
            });
        }
        /// <summary>
        /// 刷新端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            RunningTestingWindowSys.Instance.RefreshSerialPortConnect(cmbComPort);
        }
        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            OpenConnectionSerial();
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            RunningTestingWindowSys.Instance.GetMachineCount(SerialReader);
        }
        /// <summary>
        /// 自动匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoMatch_Click(object sender, EventArgs e)
        {
             RunningTestingWindowSys.Instance.ClearMatchUser(_userControl1s,ref  RaceStudentDataLists);
             RunningTestingWindowSys.Instance.AutoMatchStudentData(ProjectID, GroupName, CurrentGroupStudentData,ref RaceStudentDataLists,equipmentCount,CurrentRoundCount,_userControl1s,ref autoMatchFlag,SerialReader);
        }
        /// <summary>
        /// 选择匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectMatch_Click(object sender, EventArgs e)
        {
            RunningTestingWindowSys.Instance.ClearMatchUser(_userControl1s,ref  RaceStudentDataLists);
            RunningTestingWindowSys.Instance.SelectMatchStudentData(CurrentGroupStudentData, ref RaceStudentDataLists, CurrentRoundCount, _userControl1s,ref autoMatchFlag,SerialReader  );
        } 
        /// <summary>
        /// 清空匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void btnClearMatch_Click(object sender, EventArgs e)
        {
            RunningTestingWindowSys.Instance.ClearMatchUser(_userControl1s,ref RaceStudentDataLists);
        }
        /// <summary>
        /// 全部暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void btnAllStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialReader == null || !SerialReader.IsComOpen())
                {
                    UIMessageBox.ShowWarning("设备断开");
                    return;
                }
                if (RunningTestingWindowSys.Instance.IsSerialOpenConnection(SerialReader))
                {
                    SerialReader.qType = "stop";
                    string command = "stop";
                    byte[] buff = System.Text.Encoding.Default.GetBytes(command);
                    SerialReader.SendMessage(buff);
                    btnExec.Enabled = true;
                    btnAutoMatch.Enabled = true;
                    btnSelectMatch.Enabled = true;
                    btnWriteScore.Enabled = true;
                    btnClearMatch.Enabled = true;
                    btnUpload.Enabled = true;
                    btnReGetScore.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }
        /// <summary>
        /// 全部结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void btnAllFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialReader == null || !SerialReader.IsComOpen())
                {
                    UIMessageBox.ShowWarning("设备断开");
                    return;
                }
                if (RunningTestingWindowSys.Instance.IsSerialOpenConnection(SerialReader))
                {
                    string EndgameCommand = "Endgame";
                    byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(EndgameCommand);
                    SerialReader.SendMessage(WriteBufferALL);;
                    btnExec.Enabled = true;
                    btnAutoMatch.Enabled = true;
                    btnSelectMatch.Enabled = true;
                    btnWriteScore.Enabled = true;
                    btnClearMatch.Enabled = true;
                    btnUpload.Enabled = true;
                    btnReGetScore.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
        }
         /// <summary>
         /// 开始考试
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         /// <exception cref="NotImplementedException"></exception>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                isSendMachineInfo = false;
                getFinalFlag = false;
                if (RunningTestingWindowSys.Instance.IsSerialOpenConnection(SerialReader))
                {
                    if (SerialReader!=null)
                    {
                        SerialReader.machineNum = equipmentCountCbx.SelectedIndex + 1;
                    }
                    if (RaceStudentDataLists.Count==0)
                    {
                        UIMessageBox.ShowError("未分配考生");
                        return;
                    }
                    RunningTestingWindowSys.Instance.VoiceOut("各就各位",1);
                    voiceFlag = true;
                    SerialReader.qType = "qStart";
                    string command = "qStart";
                    byte[] buff = System.Text.Encoding.Default.GetBytes(command);
                    SerialReader.SendMessage(buff);
                    ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        btnExec.Enabled = false;
                        btnAutoMatch.Enabled = false;
                        btnSelectMatch.Enabled = false;
                        btnWriteScore.Enabled = false;
                        btnClearMatch.Enabled = false;
                        btnUpload.Enabled = false;
                        btnReGetScore.Enabled = false;
                       tsetLabel.Text = "测试中";
                        for (int i = 0; i < _userControl1s.Count; i++)
                        {
                            _userControl1s[i].Panel_Ready = false;
                        }
                    });
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
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void btnWriteScore_Click(object sender, EventArgs e) 
        {
            try
            {
                btnWriteScore.Enabled = false;
                WriteScoreIntoDataBase();

            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return;
            }
            finally
            {
                btnWriteScore.Enabled = true;
            }
        }
         /// <summary>
         /// 重取成绩
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         /// <exception cref="NotImplementedException"></exception>
         private void btnReGetScore_Click(object sender, EventArgs e) {
             try
             {
                 DialogResult result = MessageBox.Show("是否重取成绩!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                 if (result != DialogResult.Yes) return;
                 //string code = "reSendScore";
                 if (RunningTestingWindowSys.Instance.IsSerialOpenConnection( SerialReader))
                 {
                     string code = "Post All Score";
                     byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(code);
                     SerialReader.qType = "qStart";
                     SerialReader.SendMessage(WriteBufferALL);
                 }
             }
             catch (Exception ex)
             {
                 LoggerHelper.Debug(ex);
                 return;
             }
         }
         /// <summary>
         /// 成绩上传
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void btnUpload_Click(object sender, EventArgs e)
         {
             string[] fusp = new string[3];
             fusp[0] = ProjectName;
             fusp[1] = GroupName;
             fusp[2] = CurrentRoundCount.ToString();
             ParameterizedThreadStart ParStart = new ParameterizedThreadStart(UpLoadCurrentGroupGrade);
             Thread t = new Thread(ParStart);
             t.IsBackground = true;
             t.Start(fusp);
         }
         /// <summary>
         /// 组号下拉
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void GroupCombox_SelectedIndexChanged(object sender, EventArgs e)
         {
             GroupName = GroupCombox.Text.Trim();
             label13.Text = $"当前测试组:{GroupName}";
             if (CurrentRoundCount>0)
             {
                 RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
             }
             else
             {
                 return;
             }
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void RoundCountCombox_SelectedIndexChanged(object sender, EventArgs e)
         {
             if (RoundCountCombox.SelectedIndex != -1)
             {
                 CurrentRoundCount = RoundCountCombox.SelectedIndex + 1;
                 RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
                 label7.Text =  CurrentRoundCount.ToString() ;
             }
             else
             {
                 return;
             }
         }
         /// <summary>
         /// 刷新
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void uiButton1_Click(object sender, EventArgs e)
         {
             if (CurrentRoundCount > 0)
             {
                 RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
             }
         }
         private bool FrmModifyScoreOneRoundShow = false;
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void 修正成绩ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             try
             {
                 if (CurrentGroupStudentData.SelectedRows.Count == 0)
                 {
                     UIMessageBox.ShowError("请先选择考生信息！！");
                     return;
                 }
                 else
                 {
                     string groupName =    GroupCombox.Text;
                     if (RunningTestingWindowSys.Instance.ShowModifyWindow(ProjectID,ProjectName,groupName,CurrentRoundCount,CurrentGroupStudentData,ref  FrmModifyScoreOneRoundShow))
                     {
                         UIMessageBox.ShowSuccess("修改成功！！");
                         RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
                     }
                     else
                     {
                         UIMessageBox.ShowError("修改失败！！");
                         return;
                     }
                 }
             }
             catch (Exception exception)
             {
                 LoggerHelper.Debug(exception);
                 return;
             }
             finally
             {
                 FrmModifyScoreOneRoundShow = false;
             }
              
         }
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void 缺考ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (RunningTestingWindowSys.Instance.SetStudentState(CurrentGroupStudentData, CurrentRoundCount, "缺考"))
             {
                 UIMessageBox.ShowSuccess("更新成功！！");
                 if(CurrentRoundCount>0)
                     RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
             }
             else
             {
                 UIMessageBox.Show("更新失败！！");
                 return;
             }
             
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void 弃权ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if ( RunningTestingWindowSys.Instance.SetStudentState(CurrentGroupStudentData, CurrentRoundCount, "弃权"))
             {
                 UIMessageBox.ShowSuccess("更新成功！！");
                 if(CurrentRoundCount>0)
                     RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
             }
             else
             {
                 UIMessageBox.Show("更新失败！！");
                 return;
             }
         }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void 中退ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (RunningTestingWindowSys.Instance.SetStudentState(CurrentGroupStudentData, CurrentRoundCount, "中退"))
             {
                 UIMessageBox.ShowSuccess("更新成功！！");
                 if(CurrentRoundCount>0)
                     RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
             }
             else
             {
                 UIMessageBox.Show("更新失败！！");
                 return;
             
             }

              
         }
          /// <summary>
          /// 
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
         private void 犯规ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (RunningTestingWindowSys.Instance.SetStudentState(CurrentGroupStudentData, CurrentRoundCount, "犯规"))
             {
                 UIMessageBox.ShowSuccess("更新成功！！");
                 if(CurrentRoundCount>0)
                     RunningTestingWindowSys.Instance.UpdataGroupListView(ProjectID,GroupName,CurrentRoundCount,CurrentGroupStudentData,label13);
             }
             else
             {
                 UIMessageBox.Show("更新失败！！");
                 return;
             
             }
         }
          private int m_reader_connect_mode = 0;
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void timer1_Tick(object sender, EventArgs e)
         {
             try
             {
                 if (SerialReader != null && SerialReader.IsComOpen())
                 {
                     if (m_reader_connect_mode != 1)
                     {
                         string port = connectionPortName;
                         if (string.IsNullOrEmpty(port))
                         {
                             port = SerialReader.iSerialPort.PortName;
                         }

                         serialConnectStripStatusLabel1.Text = $"串口:{port}已连接";
                         serialConnectStripStatusLabel1.ForeColor = Color.Green;
                        uiGroupBox5.Enabled = true;
                         m_reader_connect_mode = 1;
                     }
                 }
                 else
                 {
                     connectionPortName = string.Empty;
                     if (m_reader_connect_mode != 0)
                     {
                         serialConnectStripStatusLabel1.Text = $"串口未连接";
                         serialConnectStripStatusLabel1.ForeColor = Color.Red;
                         m_reader_connect_mode = 0;
                         //MessageBox.Show("串口断开");
                     }
                 }
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
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void RunningTestingWindow_FormClosing(object sender, FormClosingEventArgs e)
         {
             timer1.Stop();
         }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void RunningTestingWindow_FormClosed(object sender, FormClosedEventArgs e)
         {
             if (SerialReader != null)
                 SerialReader.CloseCom();
         }
         #endregion


         
    }

        
}