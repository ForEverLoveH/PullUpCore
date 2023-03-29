using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraADCoreModel.GameModel
{
    public  class RaceStudentData
    {
        public int RaceStudentDataId { get; set; }
        public string id { get; set; }
        public string idNumber { get; set; }
        public string name { get; set; }
        public int score { get; set; }
        public int RoundId { get; set; }

        //状态 0:未测试 1:已测试 2:中退 3:缺考 4:犯规
        public int state { get; set; }
    }
}
