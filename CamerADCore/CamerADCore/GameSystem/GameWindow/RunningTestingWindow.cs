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
using CamerADCore.GameSystem.AutoSize;
using CameraADCoreModel.ADCoreSqlite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Linq;
using Newtonsoft.Json.Linq;
using static HZH_Controls.MouseHook;
using System.Xml.Linq;
using HZH_Controls.Forms;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class RunningTestingWindow : Form
    {
        public string formTitle = String.Empty;
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

        private string connectionPortName = String.Empty;

        private List<bool> showTitleBools = new List<bool>();
        protected SerialReader SerialReader = null;
        private List<UserControl1> _userControl1s = null;
        private List<RaceStudentData> RaceStudentDataLists = new List<RaceStudentData>();
        //AutoSizeFormClass AutoSizeFormClass = null;
        private NFC_Helper USBWatcher = new NFC_Helper();
        int x, y;
        public SQLiteHelper Helper { get; internal set; }
        private void RunningTestingWindow_Load(object sender, System.EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text = $"德育龙{ProjectName}测试系统";
           // AutoSizeFormClass= new AutoSizeFormClass(this) ;
            
            LoadingInitData();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitSerialPortData();
            USBWatcher.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 1));
            sw.Stop();
            Console.WriteLine($"-------{sw.ElapsedMilliseconds}");
            //asc.controllInitializeSize(this);

        }
        #region 初始化
        private void USBEventHandler(object sender, EventArrivedEventArgs e)
        {
            //暂未实现
            var watcher = sender as ManagementEventWatcher;
            watcher.Stop();

            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                Console.WriteLine("设备连接");
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {
                if (SerialReader == null || !SerialReader.IsComOpen())
                {

                    Reconnect();
                }
            }

            watcher.Start();
        }

        private void Reconnect()
        {
            if (isReconnect) return;
            isReconnect = true;
            reconnecting = true;
            //检测断开,断开提示
            MessageBox.Show("设备断开请检查");
            reconnectThread = new Thread(new ThreadStart(TryReconnect));
            reconnectThread.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        private void TryReconnect()
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
                    if (SerialReader != null && SerialReader.IsComOpen())
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
        /// 
        /// </summary>
        private void InitSerialPortData()
        {
            if (SerialReader != null)
                SerialReader.CloseCom();
            //初始化访问读写器实例
            SerialReader = new SerialReader();
            //回调函数
            SerialReader.AnalyCallback = AnalyData;
            SerialReader.ReceiveCallback = ReceiveData;
            SerialReader.SendCallback = SendData;
            ComPortsInit();
        }
        /// <summary>
        ///加载初始化函数
        /// </summary>
        private void LoadingInitData()
        {
            UpdateGroupCombox();
            UpdateRoundCountCombox();
            RoundCountCombox.SelectedIndex = 0;
            _userControl1s = new List<UserControl1>()
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
            int nlen = _userControl1s.Count;
            for (int i = 0; i < nlen; i++)
            {
                _userControl1s[i].StateSwitchCallback = StateSwitchCallbackFun;
            }
            if (Type == "1") this.Text = "引体向上测试管理";
            else
            {
                this.Text = "仰卧起坐测试管理";
            }
            UpdateListView(ProjectID, GroupName, 1);

        }
        #endregion
        private void UpdateRoundCountCombox()
        {
            RoundCountCombox.Items.Clear();
            for (int i = 1; i <= RoundCount; i++)
            {
                RoundCountCombox.Items.Add(i.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupname"></param>
        private void UpdateGroupCombox(String groupname = "")
        {
            GroupCombox.Items.Clear();
            GroupCombox.Text = "";
            var ds = Helper.ExecuteReader($"SELECT Name FROM DbGroupInfos WHERE Name LIKE'%{groupname}%' AND ProjectId='{ProjectID}'");
            AutoCompleteStringCollection lstsourece = new AutoCompleteStringCollection();

            while (ds.Read())
            {
                GroupCombox.Items.Add(ds.GetString(0));
                lstsourece.Add(ds.GetString(0));
            }
            GroupCombox.AutoCompleteCustomSource = lstsourece;
            int index = -1;
            GroupCombox.SelectedIndex = index;
            GroupCombox.Text = "";
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
        /// 
        /// </summary>
        private void StateSwitchCallbackFun()
        {
            try
            {
                MatchDataSendInfoFun();
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 发送名单
        /// </summary>
        private void MatchDataSendInfoFun()
        {
            try
            {
                if (RaceStudentDataLists.Count == 0) return;
                if (!autoMatchFlag) return;

                MatchDataSendInfo(RaceStudentDataLists);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 发送名单
        /// </summary>
        /// <param name="raceStudentDataLists"></param>
        private void MatchDataSendInfo(List<RaceStudentData> raceStudentDataLists)
        {
            try
            {
                if (SerialReader == null || !SerialReader.IsComOpen())
                {
                    MessageBox.Show("设备断开");
                    return;
                }
                Byte[] ff = new Byte[] { 0xff };
                //发送名单头码
                Byte[] headCode = System.Text.Encoding.Default.GetBytes("name:");
                Byte[] endByte = System.Text.Encoding.Default.GetBytes("end");
                //发送名单结束码
                Byte[] endCode = new Byte[endByte.Length + ff.Length];
                endByte.CopyTo(endCode, 0);
                ff.CopyTo(endCode, endByte.Length);
                Byte[] WriteBufferALL = Enumerable.Repeat((Byte)0x00, 404).ToArray(); ;
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
                for (int i = 0; i < raceStudentDataLists.Count; i++)
                {
                    string GName = raceStudentDataLists[i].name;
                    int state0 = _userControl1s[i].panel_status;
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
                    SerialReader.SendMessage(WriteBufferALL, li);
                    Thread.Sleep(1000);
                    //发送缺考
                    SerialReader.SendMessage(WriteAbsenceBufferALL, 0);
                });
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
        /// <param name="projectID"></param>
        /// <param name="groupName"></param>
        /// <param name="roundid"></param>
        private void UpdateListView(string projectID, string groupName, int roundid)
        {
            try
            {
                var ds = Helper.ExecuteReaderList($"SELECT Id,Name,IdNumber FROM DbPersonInfos WHERE ProjectId='{projectID}' and GroupName='{GroupName}'");
                int listviewCount = 1;
                int nlen = ds.Count;
                DataGridViewRow[] dataRows = new DataGridViewRow[nlen];
                int sum = ds.Count;
                int finishSum = 0;
                foreach (var dic in ds)
                {
                    //int k = stuView.Rows.Add(new DataGridViewRow());
                    string stuId = dic["Id"];
                    string stuName = dic["Name"];
                    string idNumber = dic["IdNumber"];
                    DataGridViewRow dgr = new DataGridViewRow();

                    dgr.Cells.Add(GetNewDataGridViewCell(listviewCount, Color.Black, Color.White));
                    dgr.Cells.Add(GetNewDataGridViewCell(stuName, Color.Black, Color.White));

                    var ds1 = Helper.ExecuteReader($"SELECT PersonName,Result,State,uploadState  FROM ResultInfos WHERE PersonId='{stuId}' AND RoundId={roundid}");
                    bool flag = true;
                    while (ds1.Read())
                    {
                        finishSum++;
                        flag = false;
                        string PersonName0 = ds1.GetString(0);
                        double Result0 = (double)ds1.GetValue(1);
                        int State0 = ds1.GetInt32(2);
                        int uploadState0 = ds1.GetInt32(3);

                        if (State0 > 1)
                        {
                            //犯规异常操作
                            string sstate = ResultState.ResultState2Str(State0);
                            dgr.Cells.Add(GetNewDataGridViewCell(sstate, Color.Red, Color.White));
                        }
                        else
                        {
                            dgr.Cells.Add(GetNewDataGridViewCell(Result0.ToString(), Color.Black, Color.MediumSpringGreen));
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
                        dgr.Cells.Add(GetNewDataGridViewCell("未上传", Color.Red, Color.White));
                    }

                    dgr.Cells.Add(GetNewDataGridViewCell(stuId, Color.Black, Color.White));
                    dataRows[listviewCount - 1] = dgr;
                    listviewCount++;
                }
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    CurrentGroupStudentData.Rows.Clear();
                    CurrentGroupStudentData.Rows.AddRange(dataRows);
                    int noSum = sum - finishSum;
                    noSum = noSum < 0 ? 0 : noSum;
                    label13.Text = $"{GroupName}组内还有{noSum}人未测试";
                    if (noSum > 0)
                    {
                        label13.ForeColor = Color.Red;
                    }
                    else
                    {
                        label13.ForeColor = Color.Black;
                    }
                });
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
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
        #region 串口
        private void ComPortsInit()
        {
            RefreshComPorts();
            BtnOpenComPort();
            equipmentCountCbx.SelectedIndex = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        private void BtnOpenComPort()
        {
            try
            {
                string port = cmbComPort.Text;
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
                    string portname = port;
                    if (!string.IsNullOrEmpty(portname))
                    {
                        OpenComPort(portname);
                    }
                }

                if (SerialReader.IsComOpen())
                {
                    connectionPortName = port;
                    serialConnectStripStatusLabel1.Text = $"串口:{port}已连接";
                    serialConnectStripStatusLabel1.ForeColor = Color.Green;
                    ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        button12.Text = "关闭端口";
                        uiGroupBox5.Enabled = true;
                        timer1.Start();
                    });
                    GetMachineNums();
                }
                else
                {
                    connectionPortName = string.Empty;
                    serialConnectStripStatusLabel1.Text = $"串口连接失败";
                    serialConnectStripStatusLabel1.ForeColor = Color.Red;
                    uiGroupBox5.Enabled = false;
                    button12.Text = "打开端口";
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void GetMachineNums()
        {
            try
            {
                if (!SerialReader.IsComOpen())
                {
                    FrmTips.ShowTipsError(this, "未打开串口");
                    return;
                }
                if (SerialReader.isExaming())
                {
                    FrmTips.ShowTipsError(this, "考试中请勿操作");
                    return;
                }
                Task.Run(() =>
                {
                    string code = "paircount";
                    //发送获取设备数量数据
                    SerialReader.qType = code;
                    //code = code.ToCharArray().Aggregate("", (result, c) => result += ((!string.IsNullOrEmpty(result) && (result.Length + 1) % 2 == 0) ? " " : "") + c.ToString());
                    byte[] paircount_b = Encoding.UTF8.GetBytes(code);
                    SerialReader.SendMessage(paircount_b);
                });
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 打开连接的串口
        /// </summary>
        /// <param name="strComPort"></param>
        /// <returns></returns>
        private bool OpenComPort(string strComPort)
        {
            try
            {
                if (SerialReader.IsComOpen())
                {
                    SerialReader.CloseCom();
                }
                string strException = string.Empty;
                int nRet = SerialReader.OpenCom(strComPort, 115200, out strException);
                if (nRet == 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// 刷新串口连接
        /// </summary>
        /// <param name="portn"></param>
        private void RefreshComPorts(string portn = "USB Serial Port")
        {
            try
            {
                cmbComPort.Items.Clear();
                cmbComPort.Text = "";
                string[] portNames = GetPortDeviceName(portn);
                if (portNames.Length == 0)
                {
                    portn = "USB-SERIAL";
                    portNames = GetPortDeviceName(portn);
                }
                if (portNames.Length == 0)
                {
                    portn = "USB-to-Serial";
                    portNames = GetPortDeviceName(portn);
                }

                if (portNames != null && portNames.Length > 0)
                {
                    foreach (string portName in portNames)
                    {
                        cmbComPort.Items.Add(PortName2Port(portName));
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
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private string PortName2Port(string deviceName)
        {
            string str = "";
            try
            {
                int a = deviceName.IndexOf("(COM") + 1;//a会等于1
                str = deviceName.Substring(a, deviceName.Length - a);
                a = str.IndexOf(")");//a会等于1
                str = str.Substring(0, a);
            }
            catch (Exception ex)
            {
                str = "";
                LoggerHelper.Debug(ex);
            }
            return str;
        }

        /// <summary>
        /// 获取串口
        /// </summary>
        /// <param name="portn"></param>
        /// <returns></returns>
        private string[] GetPortDeviceName(string name)
        {
            List<string> strs = new List<string>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PnPEntity where Name like '%(COM%'"))
            {
                var hardInfos = searcher.Get();
                foreach (var hardInfo in hardInfos)
                {
                    if (hardInfo.Properties["Name"].Value != null)
                    {
                        string deviceName = hardInfo.Properties["Name"].Value.ToString();
                        if (deviceName.Contains(name) || deviceName.Contains("Prolific"))
                        {
                            strs.Add(deviceName);
                        }
                        //Console.WriteLine(deviceName);
                    }
                }
            }
            return strs.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="btArySendData"></param>
        private void SendData(byte[] btArySendData)
        {
            string code = CCommondMethod.ByteArrayToString(btArySendData, 0, btArySendData.Length);
            Console.WriteLine($"sendCount:{btArySendData.Length}   send:{code}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="btAryReceiveData"></param>
        private void ReceiveData(byte[] btAryReceiveData)
        {
            string code = CCommondMethod.ByteArrayToString(btAryReceiveData, 0, btAryReceiveData.Length);
            Console.WriteLine($"------receiveCount:{btAryReceiveData.Length}   recv:{code}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgTran"></param>
        private void AnalyData(SerialMessageTran msgTran)
        {
            if (SerialReader.qType == "paircount")
            {
                //获取设备数量
                try
                {
                    byte[] anayData = msgTran.btAryTranData;
                    string v = Encoding.UTF8.GetString(anayData, 0, anayData.Length);
                    int vNum = 0;
                    if (v.Contains("pairplaycount:") && anayData.Length == 17)
                    {
                        vNum = anayData[14];
                    }

                    if (vNum > 0 && vNum < 11)
                    {
                        SerialReader.machineNum = vNum;
                        vNum--;
                        equipmentCountCbx.SelectedIndex = vNum;
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }
                return;
            }
            switch (msgTran.strCmd)
            {
                case 0xFE:
                    try
                    {
                        int k = 0;
                        int nlen = msgTran.ints0.Count;
                        nlen = nlen > _userControl1s.Count ? _userControl1s.Count : nlen;
                        foreach (var str in msgTran.ints0)
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
                                if (msgTran.ints[k] == 1)
                                {
                                    _userControl1s[k].Panel_Ready = false;
                                    showTitleBools[k] = false;
                                }
                            }
                            k++;
                            if (k >= nlen) break;
                        }
                        TimeSpan ts = msgTran.timeSpan;
                        if (ts.TotalMilliseconds > 0)
                        {
                            ControlHelper.ThreadInvokerControl(this, () =>
                            {
                                if (voiceFlag)
                                {
                                    VoiceOut0("开始考试", 2);
                                    showTitleBools.Clear();
                                    for (int i = 0; i < _userControl1s.Count; i++)
                                    {
                                        _userControl1s[i].Panel_Ready = true;
                                        showTitleBools.Add(true);
                                    }
                                    voiceFlag = false;
                                    titleFlag = true;
                                }
                                ucledNums1.Value = $"{String.Format("{0:00}", ts.Minutes)}:{String.Format("{0:00}", ts.Seconds)}";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Debug(ex);
                    }
                    break;

                case 0xFF:
                    try
                    {
                        autoMatchFlag = false;
                        //? 结束
                        StringBuilder writeLogSb = new StringBuilder();
                        int k1 = 0;
                        var list = msgTran.ints1;
                        int nlen = list.Count;
                        if (nlen == 0)
                        {
                            if (SerialReader != null && SerialReader.IsComOpen())
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
                            string str = list[i];//score
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
                            writeLogSb.AppendLine($"考号:{idnumber},姓名:{stuName},成绩:{str},状态:{stuState},测试时间:{nowTimeStr}");
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
                            // testLabel.Text = "测试结束";
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

                default: break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsSerialOpen()
        {
            bool flag = false;
            if (SerialReader != null && SerialReader.IsComOpen())
            {
                flag = true;
            }
            if (!flag)
            {
                FrmTips.ShowTipsError(this, "串口未连接");
            }
            return flag;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rate"></param>
        private void VoiceOut0(string str, int rate)
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
        #region 页面事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            RefreshComPorts();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            GetMachineNums();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (CurrentRoundCount > 0)
            {
                UpdateListView(ProjectID, GroupName, CurrentRoundCount);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            BtnOpenComPort();
        }
        private void CurrentGroupStudentData_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0&&e.ColumnIndex>=0)
                {
                    CurrentGroupStudentData.ClearSelection();
                    CurrentGroupStudentData.Rows[e.RowIndex].Selected = true;
                    CurrentGroupStudentData.CurrentCell = CurrentGroupStudentData.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }
        /// <summary>
        /// 自动匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoMatch_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMatchCushion1s();
                CurrentGroupStudentData.Rows.Clear();
                var ds = Helper.ExecuteReaderList($"SELECT Id,Name,IdNumber FROM DbPersonInfos WHERE ProjectId='{ProjectID}' and GroupName='{GroupName}'");
                int listviewCount = 1;
                int nlen = ds.Count;
                DataGridViewRow[] dataRows = new DataGridViewRow[nlen];
                List<int> chooseList = new List<int>();
                foreach (var dic in ds)
                {
                    //int k = stuView.Rows.Add(new DataGridViewRow());
                    string stuId = dic["Id"];
                    string stuName = dic["Name"];
                    string idNumber = dic["IdNumber"];
                    DataGridViewRow dgr = new DataGridViewRow();

                    dgr.Cells.Add(GetNewDataGridViewCell(listviewCount, Color.Black, Color.White));
                    dgr.Cells.Add(GetNewDataGridViewCell(stuName, Color.Black, Color.White));

                    var ds1 = Helper.ExecuteReaderList($"SELECT PersonName,Result,State,uploadState FROM ResultInfos WHERE PersonId='{stuId}' AND RoundId={CurrentRoundCount}");
                    bool flag = true;
                    foreach (var row in ds1)
                    {
                        flag = false;
                        string PersonName0 = row["PersonName"];
                        double.TryParse(row["Result"], out double Result0);
                        int.TryParse(row["State"], out int State0);
                        int.TryParse(row["uploadState"], out int uploadState0);

                        if (State0 > 1)
                        {
                            //犯规异常操作
                            string sstate = ResultState.ResultState2Str(State0);
                            dgr.Cells.Add(GetNewDataGridViewCell(sstate, Color.Red, Color.White));
                        }
                        else
                        {
                            dgr.Cells.Add(GetNewDataGridViewCell(Result0.ToString(), Color.Black, Color.MediumSpringGreen));
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
                        dgr.Cells.Add(GetNewDataGridViewCell("未上传", Color.Red, Color.White));
                        if (RaceStudentDataLists.Count < equipmentCount)
                        {
                            RaceStudentDataLists.Add(new RaceStudentData()
                            {
                                RaceStudentDataId = listviewCount,
                                id = stuId,
                                name = stuName,
                                idNumber = idNumber,
                                score = 0,
                                state = 0,
                                RoundId = CurrentRoundCount
                            });
                            chooseList.Add(listviewCount - 1);
                        }
                    }

                    dgr.Cells.Add(GetNewDataGridViewCell(stuId, Color.Black, Color.White));
                    dataRows[listviewCount - 1] = dgr;
                    listviewCount++;
                }
                CurrentGroupStudentData.Rows.AddRange(dataRows);
                int nlen0 = CurrentGroupStudentData.Rows.Count;
                for (int i = 0; i < nlen0; i++)
                {
                    if (chooseList.Contains(i))
                    {
                        CurrentGroupStudentData.Rows[i].Selected = true;
                    }
                    else
                    {
                        CurrentGroupStudentData.Rows[i].Selected = false;
                    }
                }
                AutoMatchCushion1s();
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 选择匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectMatch_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMatchCushion1s();
                bool flag = true;
                int index = 0;
                for (int i = 0; i < CurrentGroupStudentData.SelectedRows.Count; i++)
                {
                    index = CurrentGroupStudentData.SelectedRows.Count - 1 - i;
                    string stuId = CurrentGroupStudentData.SelectedRows[index].Cells[5].Value.ToString();
                    string stuName = CurrentGroupStudentData.SelectedRows[index].Cells[1].Value.ToString();
                    string score = CurrentGroupStudentData.SelectedRows[index].Cells[2].Value.ToString();
                    string idNumber = CurrentGroupStudentData.SelectedRows[index].Cells[3].Value.ToString();
                    if (score != "无成绩")
                    {
                        flag = false;
                        FrmTips.ShowTipsError(this, "选择考生中有考生已有成绩");
                        RaceStudentDataLists.Clear();
                        break;
                    }
                    RaceStudentDataLists.Add(new RaceStudentData()
                    {
                        RaceStudentDataId = i + 1,
                        id = stuId,
                        name = stuName,
                        idNumber = idNumber,
                        score = 0,
                        state = 0,
                        RoundId = CurrentRoundCount
                    });
                }
                if (flag)
                {
                    AutoMatchCushion1s();
                }
                else
                {
                    ClearMatchCushion1s();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearMatch_Click(object sender, EventArgs e)
        {
            ClearMatchCushion1s();
        }
        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                isSendMachineInfo = false;
                getFinalFlag = false;
                if (IsSerialOpen())
                {
                    if (SerialReader != null)
                    {
                        SerialReader.machineNum = equipmentCountCbx.SelectedIndex + 1;
                    }
                    if (RaceStudentDataLists.Count == 0)
                    {
                        FrmTips.ShowTipsError(this, "未分配考生");
                        return;
                    }
                    VoiceOut0("各 就 位", 1);
                    voiceFlag = true;
                    SerialReader.qType = "qStart";
                    string startCommand = "start";
                    byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(startCommand);
                    SerialReader.SendMessage(WriteBufferALL);

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

                        //btnWriteScore.Enabled = false;
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }

        /// <summary>
        /// 全部暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialReader == null || !SerialReader.IsComOpen())
                {
                    MessageBox.Show("设备断开");
                    return;
                }

                if (IsSerialOpen())
                {
                    SerialReader.qType = "stop";
                    string startCommand = "stop";
                    byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(startCommand);
                    SerialReader.SendMessage(WriteBufferALL);

                    btnExec.Enabled = true;
                    btnAutoMatch.Enabled = true;
                    btnSelectMatch.Enabled = true;
                    btnWriteScore.Enabled = true;
                    btnClearMatch.Enabled = true;
                    btnUpload.Enabled = true;
                    btnReGetScore.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialReader == null || !SerialReader.IsComOpen())
                {
                    MessageBox.Show("设备断开");
                    return;
                }
                if (IsSerialOpen())
                {
                    string EndgameCommand = "Endgame";
                    byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(EndgameCommand);
                    SerialReader.SendMessage(WriteBufferALL);
                    btnExec.Enabled = true;
                    btnAutoMatch.Enabled = true;
                    btnSelectMatch.Enabled = true;
                    btnWriteScore.Enabled = true;
                    btnClearMatch.Enabled = true;
                    btnUpload.Enabled = true;
                    btnReGetScore.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 写入成绩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWriteScore_Click(object sender, EventArgs e)
        {
            try
            {
                btnWriteScore.Enabled = false;
                WriteScore2Db();
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            finally
            {
                btnWriteScore.Enabled = true;
            }
        }
        private bool FrmModifyScoreOneRoundShow = false;
        private void 修正成绩ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentGroupStudentData.SelectedRows.Count == 0)
            {
                FrmTips.ShowTipsError(this, "未选择考生");
                return;
            }
            //int state0 = ResultState.ResultState2Int(state);
            string idNumber = CurrentGroupStudentData.SelectedRows[0].Cells[3].Value.ToString();
            string name = CurrentGroupStudentData.SelectedRows[0].Cells[1].Value.ToString();
            string id = CurrentGroupStudentData.SelectedRows[0].Cells[5].Value.ToString();
            string groupname0 = GroupCombox.Text;

            if (FrmModifyScoreOneRoundShow)
            {
                FrmTips.ShowTipsError(this, "未处理上一个数据");
                return;
            }
            FrmModifyScoreOneRoundShow = true;
            Thread newThread = new Thread(new ParameterizedThreadStart((o) =>
            {
                try
                {
                    if (FrmModifyScoreOneRoundSys.Instance.ShowFrmModifyScoreOneRound(Helper, ProjectID, ProjectName, CurrentRoundCount, idNumber, name, GroupName))
                    {
                        var sl = FrmModifyScoreOneRoundSys.Instance.GetFrmModifyScoreWindowBackData();
                        string score = sl.score;
                        int istate = sl.iState;
                        string PersonId = sl.projectId;
                        if (istate > 1)
                        {
                            //犯规
                            string sql = $"UPDATE ResultInfos SET Result='0',State='{istate}' WHERE PersonId='{PersonId}' AND RoundId='{CurrentRoundCount}'";
                            Helper.ExecuteNonQuery(sql);
                        }
                        else if (istate == 1)
                        {
                            string sql = $"UPDATE ResultInfos SET Result='{score}' WHERE PersonId='{PersonId}' AND RoundId='{CurrentRoundCount}'";
                            Helper.ExecuteNonQuery(sql);
                        }
                        if (CurrentRoundCount > 0)
                        {
                            try
                            {
                                File.AppendAllText("操作日志.txt", $"考号:{idNumber},姓名:{name},修正成绩:第{CurrentRoundCount}轮成绩为{score}");
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Debug(ex);
                            }
                            UpdateListView(ProjectID, GroupName, CurrentRoundCount);
                        }

                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }
                finally
                {
                    FrmModifyScoreOneRoundShow = false;
                }
            }));
            newThread.IsBackground = true;
            newThread.Start();

        }

        private void 缺考ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentState("缺考");
        }

        private void 弃权ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentState("弃权");
        }

        private void 中退ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentState("中退");
        }

        private void 成绩重测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentState("重测");
        }
        private void 犯规ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentState("犯规");
        }
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReGetScore_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("是否重取成绩!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
                //string code = "reSendScore";

                if (IsSerialOpen())
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
                UpdateListView(ProjectID, GroupName, CurrentRoundCount);
                label7.Text = RoundCountCombox.Text;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GroupName = GroupCombox.Text;
            label13.Text = "当前测试组:" + GroupName;
            if (CurrentRoundCount > 0)
            {
                //UpdateListView(_ProjectId, groupName, RoundCount0);
                UpdateListView(ProjectID, GroupName, CurrentRoundCount);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            string[] fusp = new string[3];
            fusp[0] = ProjectName;
            fusp[1] = GroupName;
            fusp[2] = CurrentRoundCount.ToString();

            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(UploadStudentThreadFun);
            Thread t = new Thread(ParStart);
            t.IsBackground = true;
            t.Start(fusp);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void UploadStudentThreadFun(object obj)
        {
            try
            {
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    btnUpload.BackColor = Color.Red;
                    btnUpload.Text = "上传中";
                    btnUpload.Enabled = false;
                });
                string cpuid = CPUHelper.GetCpuID();
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = Helper.ExecuteReaderList("SELECT * FROM localInfos");
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

                Dictionary<string, string> SportProjectDic = Helper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                         $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                string sql0 = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}'";
                ///查询本项目已考组
                if (!string.IsNullOrEmpty(groupName))
                {
                    sql0 += $" AND Name = '{groupName}'";
                }
                List<Dictionary<string, string>> sqlGroupsResults = Helper.ExecuteReaderList(sql0);
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
                    string sql = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                        $" WHERE ProjectId='{SportProjectDic["Id"]}' AND GroupName = '{sqlGroupsResult["Name"]}'";

                    List<Dictionary<string, string>> list = Helper.ExecuteReaderList(sql);
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
                    for (int i = 1; i <= CurrentRoundCount; i++)
                    {
                        foreach (var stu in list)
                        {
                            List<RoundsItem> roundsItems = new List<RoundsItem>();
                            ///成绩
                            List<Dictionary<string, string>> resultScoreList1 = Helper.ExecuteReaderList(
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
                            messageSb.AppendLine($"{sqlGroupsResult["Name"]}组 考号:{item.IdNumber} 姓名:{item.Name},第{item.Rounds[0].RoundId}轮上传失败,错误内容:{errorStr}");
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
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = Helper.BeginTransaction();

                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    //更新成绩上传状态
                    string sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]} and RoundId={item["RoundId"]}";
                    Helper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                Helper.CommitTransaction(ref sQLiteTransaction);
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

                if (CurrentRoundCount > 0)
                {
                    UpdateListView(ProjectID, GroupName, CurrentRoundCount);
                }
                MessageBox.Show(outpitMessage);
                // return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                MessageBox.Show(ex.Message);
                //return ex.Message;
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

        #endregion
        /// <summary>
        /// 自动匹配
        /// </summary>
        private void AutoMatchCushion1s()
        {
            try
            {
                autoMatchFlag = true;
                int len = 10;
                if (len > RaceStudentDataLists.Count) len = RaceStudentDataLists.Count;
                List<RaceStudentData> list = new List<RaceStudentData>();

                for (int i = 0; i < len; i++)
                {
                    var rs = RaceStudentDataLists[i];
                    _userControl1s[i].panel_name = rs.name;
                    _userControl1s[i].panel_Score = "0";
                    _userControl1s[i].panel_status = rs.state;
                    _userControl1s[i].panel_idNumber = rs.idNumber;
                    list.Add(rs);
                }
                if (len == 0)
                {
                    MessageBox.Show("本轮组已全部测试");
                }
                else
                {
                    MatchDataSendInfoFun();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }

        /// <summary>
        /// 清空当前匹配
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ClearMatchCushion1s()
        {
            try
            {
                RaceStudentDataLists.Clear();
                for (int i = 0; i < 10; i++)
                {
                    string str = string.Empty;
                    _userControl1s[i].panel_name = str;
                    _userControl1s[i].panel_Score = str;
                    _userControl1s[i].panel_status = 0;
                    _userControl1s[i].panel_idNumber = str;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return;
            }
        }
        /// <summary>
        /// 写入成绩
        /// </summary>
        private void WriteScore2Db()
        {
            if (!getFinalFlag)
            {
                DialogResult result = MessageBox.Show("未收到最终成绩,是否重取成绩!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
                if (SerialReader != null && SerialReader.IsComOpen())
                {
                    string code = "Post All Score";
                    byte[] WriteBufferALL = System.Text.Encoding.Default.GetBytes(code);
                    SerialReader.qType = "qStart";
                    SerialReader.SendMessage(WriteBufferALL);
                }
                else
                {
                    MessageBox.Show("设备断开");
                }
                return;
            }
            UpdateRaceStudentDataListsScore();
            if (RaceStudentDataLists.Count == 0)
            {
                FrmTips.ShowTipsError(this, "数据异常");
                return;
            }
            StringBuilder sb = new StringBuilder();
            System.Data.SQLite.SQLiteTransaction sQLiteTransaction = Helper.BeginTransaction();
            foreach (var rs in RaceStudentDataLists)
            {
                try
                {
                    string sql = $"select Result from ResultInfos where PersonId='{rs.id}' and RoundId='{CurrentRoundCount}' and IsRemoved=0";
                    List<Dictionary<string, string>> list = Helper.ExecuteReaderList(sql);
                    int state0 = rs.state == 0 ? 1 : rs.state;
                    if (list.Count == 0)
                    {
                        var sortid = Helper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                        string sortid0 = "1";
                        if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();
                        sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                            $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{rs.id}',0,'{rs.name}','{rs.idNumber}',{CurrentRoundCount},{rs.score},{state0})";
                        //处理写入数据库
                        Helper.ExecuteNonQuery(sql);
                        //更新状态为已考
                        Helper.ExecuteNonQuery($"UPDATE DbPersonInfos SET State = 1, FinalScore = 1 WHERE Id = '{rs.id}'");
                    }
                    else
                    {
                        sb.Append($"{rs.idNumber},{rs.name} 本轮已测试\n");
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                    FrmTips.ShowTipsError(this, ex.Message);
                }
            }
            Helper.CommitTransaction(ref sQLiteTransaction);

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString());
            }
            if (CurrentRoundCount > 0)
            {
                UpdateListView(ProjectID, GroupName, CurrentRoundCount);
                ClearMatchCushion1s();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateRaceStudentDataListsScore()
        {
            try
            {
                StringBuilder tp_errorsb = new StringBuilder();
                for (int i = 0; i < RaceStudentDataLists.Count; i++)
                {
                    string ScoreStr = _userControl1s[i].panel_Score;
                    int.TryParse(ScoreStr, out int Score);
                    int State = _userControl1s[i].panel_status;
                    RaceStudentDataLists[i].state = State;
                    RaceStudentDataLists[i].score = Score;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }

        private void SetStudentState(string state)
        {
            if (CurrentGroupStudentData.SelectedRows.Count == 0)
            {
                FrmTips.ShowTipsError(this, "未选择考生");
                return;
            }
            int state0 = ResultState.ResultState2Int(state);
            string idNumber = CurrentGroupStudentData.SelectedRows[0].Cells[3].Value.ToString();
            string name = CurrentGroupStudentData.SelectedRows[0].Cells[1].Value.ToString();
            string id = CurrentGroupStudentData.SelectedRows[0].Cells[5].Value.ToString();
            if (state != "重测")
            {
                string sql = $"UPDATE DbPersonInfos SET State=1,FinalScore=1 WHERE Id='{id}'";
                int result = Helper.ExecuteNonQuery(sql);
                sql = $"UPDATE ResultInfos SET State={state0},Result='0' WHERE PersonId='{id}' AND RoundId={CurrentRoundCount} AND IsRemoved=0";
                result = Helper.ExecuteNonQuery(sql);

                if (result == 0)
                {
                    var sortid = Helper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                    string sortid0 = "1";
                    if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();

                    sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                             $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{id}',0,'{name}','{idNumber}',{CurrentRoundCount},{0},{state0})";
                    //处理写入数据库
                    Helper.ExecuteNonQuery(sql);
                }
            }
            else
            {
                if (FrmDialog.ShowDialog(this, $"是否清空学生:{Name}的成绩？", "提示", true) == System.Windows.Forms.DialogResult.OK)
                {
                    string sql = $"DELETE FROM ResultInfos WHERE PersonIdNumber = '{idNumber}'";
                    int result = Helper.ExecuteNonQuery(sql);
                    sql = $"update DbPersonInfos SET State=0 where IdNumber='{idNumber}'";
                    int result1 = Helper.ExecuteNonQuery(sql);
                    if (result1 == 1 && result > 0)
                    {
                        UIMessageBox.ShowSuccess("清除成功！！");

                    }
                    else
                    {
                        UIMessageBox.ShowError("清除失败！！");

                    }
                }
                else
                {
                    UIMessageBox.ShowError("清除失败！！");
                }
            }
            if (CurrentRoundCount > 0)
            {
                UpdateListView(ProjectID, GroupName, CurrentRoundCount);
            }
        }
        private int m_reader_connect_mode = 0;

        private void RunningTestingWindow_SizeChanged(object sender, EventArgs e)
        {
           // AutoSizeFormClass.FormSizeChanged();
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (SerialReader != null &&SerialReader.IsComOpen())
                {
                    if (m_reader_connect_mode != 1)
                    {
                        string port = connectionPortName;
                        if (string.IsNullOrEmpty(port))
                        {
                            port =SerialReader.iSerialPort.PortName;
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
            }
        }

        
    }
}