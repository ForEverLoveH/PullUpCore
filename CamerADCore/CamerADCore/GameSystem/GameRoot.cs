using CamerADCore.GameSystem.GameHelper;
using CamerADCore.GameSystem.GameWindowSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CameraADCoreModel.GameModel;

namespace CamerADCore.GameSystem
{
    public class GameRoot
    {
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern void SetForegroundWindow(IntPtr mainWindowHandle);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private static WriteLoggerHelper WriteLoggerHelper  =  new WriteLoggerHelper();
        private static TreeViewHelper TreeViewHelper = new TreeViewHelper();
        private static GradeScoreManager GradeScoreManager= new GradeScoreManager();
        private static SerialPortManager SerialPortManager = new SerialPortManager();
        private static MainWindowSys MainWindowSys = new MainWindowSys();
        private static ProjectSettingWindowSys ProjectSettingWindowSys = new ProjectSettingWindowSys();
        private static PersonDataImportWindowSys PersonDataImportWindowSys = new PersonDataImportWindowSys();
        private  static  EquipMentSettingWindowSys EquipMentSettingWindowSys = new EquipMentSettingWindowSys();
        private static FrmModifyScoreWindowSys FrmModifyScoreWindowSys = new FrmModifyScoreWindowSys();
        private static ExportGradeWindowSys ExportGradeWindowSys = new ExportGradeWindowSys();
        private static RunningTestingWindowSys RunningTestingWindowSys = new RunningTestingWindowSys();
        private static FrmModifyScoreOneRoundSys FrmModifyScoreOneRoundSys = new FrmModifyScoreOneRoundSys();
        public void StartGame()
        {
            Awake();
            WriteLoggerHelper.Instance.WriteLog();
        }

        private void Awake()
        {
            WriteLoggerHelper.Awake();
            TreeViewHelper.Awake();
            MainWindowSys.Awake();
            GradeScoreManager.Awake();
            SerialPortManager.Awake();
            ProjectSettingWindowSys.Awake();
            PersonDataImportWindowSys.Awake();
            EquipMentSettingWindowSys.Awake();
            FrmModifyScoreWindowSys.Awake();
            ExportGradeWindowSys.Awake();
            RunningTestingWindowSys.Awake();
            FrmModifyScoreOneRoundSys.Awake();
        }
    }
}
