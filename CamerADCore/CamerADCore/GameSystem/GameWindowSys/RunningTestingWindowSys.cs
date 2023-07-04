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
        public bool ShowRunningTestingWindow(string[] fusp, Dictionary<string, string> dic, SQLiteHelper sqLiteHelper)
        {
            
            RunningTestingWindow = new RunningTestingWindow();
            RunningTestingWindow.Helper = sqLiteHelper;
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
            if (RunningTestingWindow.ShowDialog() == DialogResult.OK)
            {
                return true;
            }
            else { return false; }
        }



    }
}
