using System.Collections.Generic;

namespace CameraADCoreModel.GameModel
{
    public class ProjectModel
    {
        public  string projectName { get; set; }
        public  List<GroupModel>GroupModels { get; set; }
    }

    public class GroupModel
    {
        public  string GroupName { get; set; }
        public  int IsAllTested { get; set; }
    }
}