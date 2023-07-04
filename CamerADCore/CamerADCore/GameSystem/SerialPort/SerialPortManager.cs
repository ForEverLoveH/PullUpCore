using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamerADCore.GameSystem.GameHelper;
using CamerADCore.GameSystem.GameModel;
using LogHlper;
using Sunny.UI;

namespace CamerADCore.GameSystem 
{
    public class SerialPortManager
    {
        public static SerialPortManager Instance;
       
        public void Awake()
        {
            Instance= this;
        }
        /// <summary>
        /// 读卡器接受数据
        /// </summary>
        /// <param name="btAryReceiveData"></param>
        public void ReceiveData(byte[] btAryReceiveData)
        {
            string code = CCommondMethod.ByteArrayToString(btAryReceiveData, 0, btAryReceiveData.Length);
            Console.WriteLine($"------receiveCount:{btAryReceiveData.Length}   recv:{code}");
        }
        /// <summary>
        /// 读卡器发送数据
        /// </summary>
        /// <param name="btArySendData"></param>
        public void SendData(byte[] btArySendData)
        {
            string code = CCommondMethod.ByteArrayToString(btArySendData, 0, btArySendData.Length);
            Console.WriteLine($"sendCount:{btArySendData.Length}   send:{code}");
        }
        
        /// <summary>
        /// 获取串口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[] GetPortDeviceName(string name)
        {
            List<string> sts = new List<string>();
            using ( ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PnPEntity where Name like '%(COM%'"))
            {
                var hardInfos = searcher.Get();
                foreach (var item in  hardInfos)
                {
                    if (item.Properties["Name"].Value != null)
                    {
                        string deviceName = item.Properties["Name"].Value.ToString();
                        if (deviceName.Contains(name) || deviceName.Contains("Prolific"))
                        {
                            sts.Add(deviceName);
                        }
                    }
                }
            }
            return sts.ToArray();
        }
        /// <summary>
        /// 更具串口名字取串口号
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public string PortNameToPort(string deviceName)
        {
            string str=String.Empty;
            try
            {
                int a = deviceName.IndexOf("(COM") + 1;//a会等于1
                str = deviceName.Substring(a, deviceName.Length - a);
                a = str.IndexOf(")");//a会等于1
                str = str.Substring(0, a);
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                return null;
            }
            return str;
        }
        /// <summary>
        /// 检查串口是否已经打开
        /// </summary>
        /// <returns></returns>
        public bool IsSerialOpenConnection(SerialReader serialReader)
        {
            if (serialReader != null && serialReader.IsComOpen())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="serialReader"></param>
        /// <param name="strComPort">串口号</param>
        /// <returns></returns>
        public bool  OpenSerialPortConnection(SerialReader serialReader,string strComPort )
        {
            try
            {
                if (serialReader.IsComOpen())
                {
                    serialReader.CloseCom();
                }
                string strs = string.Empty;
                int nRet = serialReader.OpenCom(strComPort, 115200, out strs );
                if (nRet == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex );
                return false;
            }
        }
        /// <summary>
        /// 获取设备数量
        /// </summary>
        /// <param name="sReader"></param>
        public void GetMachineNums(SerialReader sReader)
        {
            try
            {
                if (!sReader.IsComOpen())
                {
                   UIMessageBox.ShowError( "未打开串口");
                    return;
                }
                if (sReader.isExaming())
                {
                    UIMessageBox.ShowError( "考试中请勿操作");
                    return;
                }
                Task.Run(() =>
                {
                    string code = "paircount";
                    //发送获取设备数量数据
                    sReader.qType = code;
                    //code = code.ToCharArray().Aggregate("", (result, c) => result += ((!string.IsNullOrEmpty(result) && (result.Length + 1) % 2 == 0) ? " " : "") + c.ToString());
                    byte[] paircount_b = Encoding.UTF8.GetBytes(code);
                    sReader.SendMessage(paircount_b);
                });
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
    }
}
