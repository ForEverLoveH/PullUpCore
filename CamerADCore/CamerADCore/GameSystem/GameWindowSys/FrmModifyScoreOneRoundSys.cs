using System.Collections.Generic;
using System.Windows.Forms;
using CameraADCoreModel.ADCoreSqlite;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameWindow;

namespace CamerADCore.GameSystem.GameWindowSys
{
    public class FrmModifyScoreOneRoundSys
    {
        public static FrmModifyScoreOneRoundSys Instance;
        private FrmModifyScoreOneRoundWindow FrmModifyScoreOneRound = null;
        private SQLiteHelper _helper;
        private static FrmModifyScoreBackData FrmModifyScoreBackData= null;

        public void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqLiteHelper"></param>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <param name="currentRoundCount"></param>
        /// <param name="idNumber"></param>
        /// <param name="name"></param>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public bool ShowFrmModifyScoreOneRound(SQLiteHelper sqLiteHelper, string projectId, string projectName, int currentRoundCount, string idNumber, string name, string groupname)
        {
            _helper = sqLiteHelper;
            FrmModifyScoreOneRound = new FrmModifyScoreOneRoundWindow();
            FrmModifyScoreOneRound.projectName = projectName;
            FrmModifyScoreOneRound.projectId = projectId;
            FrmModifyScoreOneRound.groupName = groupname;
            FrmModifyScoreOneRound.IdNumber = idNumber;
            FrmModifyScoreOneRound.Name = name;
            FrmModifyScoreOneRound.roundId = currentRoundCount;
            if (FrmModifyScoreOneRound.ShowDialog() == DialogResult.OK ||
                FrmModifyScoreOneRound.ShowDialog() == DialogResult.Yes)
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
        public FrmModifyScoreBackData GetFrmModifyScoreWindowBackData()
        {
            if (FrmModifyScoreBackData != null)
            {
                return FrmModifyScoreBackData;
                
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        public Dictionary<string,string> LoadingInitPersonData(string personId, string idNumber)
        {
            string sql = $"SELECT Id FROM DbPersonInfos WHERE ProjectId='{personId}' AND IdNumber='{idNumber}'";
            return _helper.ExecuteReaderOne(sql);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="roundId"></param>
        /// <returns></returns>
        public Dictionary<string,string> LoadingScoreData(string personId, int roundId)
        {
            string sql = $"SELECT * FROM ResultInfos WHERE RoundId='{roundId}'  AND PersonId='{personId}'";
            return _helper.ExecuteReaderOne(sql);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="score"></param>
        /// <param name="iState"></param>
        /// <param name="personId"></param>
        public void SetBackData(string score, int iState, string personId)
        {
            FrmModifyScoreBackData = new FrmModifyScoreBackData()
            {
                projectId = personId,
                iState = iState,
                score = score,
            };
        }
    }
}