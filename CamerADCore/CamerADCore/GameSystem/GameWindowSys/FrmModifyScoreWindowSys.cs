using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameWindow;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class FrmModifyScoreWindowSys
    {
        public static FrmModifyScoreWindowSys Instance;
        protected FrmModifyScoreWindow FrmModifyScoreWindow = null;
        protected SQLiteHelper SQLiteHelper = null;
        private static FrmModifyScoreWindowBackData FrmModifyScoreWindowBackData = null;

        public void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="idNumber"></param>
        /// <param name="status"></param>
        /// <param name="rountid"></param>
        /// <param name="projectId"></param>
        /// <param name="sqLiteHelper"></param>
        /// <returns></returns>
        public bool ShowFrmModifyScoreWindow(string projectName, string groupName, string name, string idNumber, string status, int rountid, string projectId, SQLiteHelper sqLiteHelper)
        {
            SQLiteHelper = sqLiteHelper;
            FrmModifyScoreWindow = new FrmModifyScoreWindow();
            FrmModifyScoreWindow.projectName = projectName;
            FrmModifyScoreWindow.groupName = groupName;
            FrmModifyScoreWindow.projectId= projectId;
            FrmModifyScoreWindow.stuName = name;
            FrmModifyScoreWindow.IdNumber = idNumber;
            FrmModifyScoreWindow.State = status;
            FrmModifyScoreWindow.roundId = rountid;
            var SL = FrmModifyScoreWindow.ShowDialog();
            if (SL == DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FrmModifyScoreWindowBackData GetFrmModifyScoreWindowBackData()
        {
            if (FrmModifyScoreWindowBackData != null)
            {
                return FrmModifyScoreWindowBackData;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="updaterountId"></param>
        /// <param name="updateScore"></param>
        public void SetFrmModifyScoreWindowBackData(string status, int updaterountId, double updateScore)
        {
            FrmModifyScoreWindowBackData = new FrmModifyScoreWindowBackData()
            {
                RoundId = updaterountId,
                Status = status,
                UpdateScore = updateScore,
            };
        }
    }
}