namespace CameraADCoreModel.GameModel
{
    public class FrmModifyScoreWindowBackData
    {
         public  int RoundId { get; set; }
         public double UpdateScore { get; set; }
         
         public  string  Status { get; set; }
    }

    public class FrmModifyScoreBackData
    {
        public string projectId { set; get; }
        public string score { set; get; }
        public int iState { set; get; }
    }
}