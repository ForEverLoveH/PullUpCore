using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameWindow;
using HZH_Controls.Forms;
using LogHlper;
using Serilog.Core;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class ExportGradeWindowSys
    {
        public static ExportGradeWindowSys Instance;
        protected ExportGradeWindow ExportGradeWindow = null;
        protected SQLiteHelper SQLiteHelper= null;
        protected GradeScoreManager GradeScoreManager = new GradeScoreManager();
        public void Awake()
        {
            Instance = this; 
        }
        public bool ShowExportGradeWindow(string projectId, string treeGroupText, string productName,SQLiteHelper sQLiteHelper)
        {
            SQLiteHelper = sQLiteHelper;
            ExportGradeWindow = new ExportGradeWindow();
            ExportGradeWindow.projectId= projectId;
            ExportGradeWindow._ProjectName = productName;
            ExportGradeWindow._GroupName= treeGroupText;
            var sl  = ExportGradeWindow.ShowDialog();
            if(sl==System.Windows.Forms.DialogResult.OK)
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
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <param name="groupName"></param>
        /// <param name="flag"></param>
        /// <returns></returns>

        public  bool OutPutScore(string projectId, string projectName, string groupName, bool flag =false)
        {
             return  GradeScoreManager.OutPutScore(projectId,projectName,groupName,SQLiteHelper,flag);
        }
    }
}