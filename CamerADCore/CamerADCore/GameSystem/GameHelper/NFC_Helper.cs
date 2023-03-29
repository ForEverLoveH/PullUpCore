using System;
using System.Data.Entity.Infrastructure.Design;
using System.Management;
using LogHlper;
using Sunny.UI;

namespace CamerADCore.GameSystem.GameHelper
{
    public class USBControllerDevice
    {
        /// <summary>
        /// ucb控制器设备id
        /// </summary>
        public  string Antecedent { get; set; }
        /// <summary>
        /// usb即插即用设备id
        /// </summary>
        public  string Dependent { get; set; }
    }
    /// <summary>
    /// 监视USB   插拔
    /// </summary>
    public partial class NFC_Helper
    {
        /// <summary>
        /// ucb插入事件监视
        /// </summary>
        private ManagementEventWatcher insertWatcher = null;
        /// <summary>
        /// usb拔出事件监视
        /// </summary>
        private ManagementEventWatcher removeWatcher = null;

        /// <summary>
        /// 添加usb事件监听器
        /// </summary>
        /// <param name="usbInsertHandler">插入事件处理器</param>
        /// <param name="usbRemoveHandler">拔出事件处理器</param>
        /// <param name="withinInterval">发送通知滞后时间</param>
        /// <returns></returns>
        public bool AddUSBEventWatcher(EventArrivedEventHandler usbInsertHandler,
            EventArrivedEventHandler usbRemoveHandler, TimeSpan withinInterval)
        {
            try
            {
                ManagementScope scope = new ManagementScope("root\\CIMV2");
                scope.Options.EnablePrivileges = true;
                if (usbInsertHandler != null)
                {
                    WqlEventQuery InsertQuery = new WqlEventQuery("__InstanceCreationEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");

                    insertWatcher = new ManagementEventWatcher(scope, InsertQuery);
                    insertWatcher.EventArrived += usbInsertHandler;
                    insertWatcher.Start();
                }

                if (usbRemoveHandler != null)
                {
                    WqlEventQuery RemoveQuery = new WqlEventQuery("__InstanceDeletionEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");

                    removeWatcher = new ManagementEventWatcher(scope, RemoveQuery);
                    removeWatcher.EventArrived += usbRemoveHandler;
                    removeWatcher.Start();
                }

                return true;
            }
            catch (Exception exception)
            {
                LoggerHelper.Debug(exception);
                RemoveUSBEventWatcher();
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void RemoveUSBEventWatcher()
        {
            if (insertWatcher != null)
            {
                insertWatcher.Stop();
                insertWatcher = null;
            }

            if (removeWatcher != null)
            {
                removeWatcher.Stop();
                removeWatcher = null;
            }
        }
        /// <summary>
        /// 定位发生插拔的USB设备
        /// </summary>
        /// <param name="e">USB插拔事件参数</param>
        /// <returns>发生插拔现象的USB控制设备ID</returns>
        public static USBControllerDevice[] WhoUSBControllerDevice(EventArrivedEventArgs e)
        {
            ManagementBaseObject mbo = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (mbo != null && mbo.ClassPath.ClassName == "Win32_USBControllerDevice")
            {
                String Antecedent = (mbo["Antecedent"] as String).Replace("\"", String.Empty).Split(new Char[] { '=' })[1];
                String Dependent = (mbo["Dependent"] as String).Replace("\"", String.Empty).Split(new Char[] { '=' })[1];
                return new USBControllerDevice[1] { new USBControllerDevice { Antecedent = Antecedent, Dependent = Dependent } };
            }

            return null;
        }
    }
}