namespace CameraADCoreModel.GameModel
{
    public class RequestUrl
    {
        /// <summary>
        /// 设备端 获取体测学校列表
        /// </summary>
        public static string GetExamListUrl = "api/GetExamList/";

        //GetExamList
        /// <summary>
        /// 设备端 获取机器码
        /// </summary>
        public static string GetMachineCodeListUrl = "api/GetMachineCodeList/";

        /// <summary>
        /// 设备端 获取组列表
        /// </summary>
        public static string GetGroupsUrl = "api/GetGroup/";


        /// <summary>
        /// 设备端 获取学生列表
        /// </summary>
        public static string FetchStudentsUrl = "api/FetchStudents/";

        /// <summary>
        /// 设备端 按组数获取学生
        /// </summary>
        public static string GetGroupStudentUrl = "api/GetGroupStudent/";

        /// <summary>
        /// 上报成绩接口
        /// </summary>

        public static string UploadResults = "api/UploadResults/";

    }

    public class RequestParameter
    {
        /// <summary>
        /// 注册软件生成的机器码
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// 管理员账号
        /// </summary>
        public string AdminUserName { get; set; }

        /// <summary>
        /// 裁判员账号
        /// </summary>
        public string TestManUserName { get; set; }

        /// <summary>
        /// 裁判员密码
        /// </summary>
        public string TestManPassword { get; set; }

        /// <summary>
        /// 考试id
        /// </summary>
        public string ExamId { get; set; }

        /// <summary>
        /// 组id
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 准考证号
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 要下载的组数
        /// </summary>
        public string GroupNums { get; set; }
    }
}