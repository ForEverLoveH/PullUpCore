using CamerADCore.GameSystem.GameWindowSys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraADCoreModel.GameModel;
using CamerADCore.GameSystem.GameModel;
using HZH_Controls.Forms;
using LogHlper;
using Newtonsoft.Json;
using Sunny.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CamerADCore.GameSystem.GameWindow
{
    public partial class EquipMentSettingWindow : Form
    {
        public EquipMentSettingWindow()
        {
            InitializeComponent();
        }
        public List<Dictionary<string, string>> localInfos = new List<Dictionary<string, string>>();
        public Dictionary<string, string> localValues = new Dictionary<string, string>();
        private void EquipMentSettingWindow_Load(object sender, EventArgs e)
        {
             
            EquipMentSettingWindowSys.Instance.LoadingInitData(ref localInfos,ref localValues  , ComboBox2, combox3, ComboBox1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
           combox3.Items.Clear();
            string url = ComboBox2.Text;

            if (string.IsNullOrEmpty(url))
            {
                FrmTips.ShowTipsError(this, "网址为空!");
                return;
            }
            url += RequestUrl.GetExamListUrl;
            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];

            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);

            // string url = localValues["Platform"] + RequestUrl.GetExamListUrl;

            var formDatas = new List<FormItemModel>();
            //添加其他字段
            formDatas.Add(new FormItemModel()
            {
                Key = "data",
                Value = JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostForm(url, formDatas);
            GetExamList upload_Result = JsonConvert.DeserializeObject<GetExamList>(result);

            if (upload_Result == null || upload_Result.Results == null || upload_Result.Results.Count == 0)
            {
                string error = string.Empty;
                try
                {
                    error = upload_Result.Error;

                }
                catch (Exception)
                {

                    error = string.Empty;
                }
                FrmTips.ShowTipsError(this, $"提交错误,错误码:[{error}]");
                return;
            }

            foreach (var item in upload_Result.Results)
            {
                string str = $"{item.title}_{item.exam_id}";
               combox3.Items.Add(str);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            ComboBox1.Items.Clear();

            string examId = combox3.Text;
            if (string.IsNullOrEmpty(examId))
            {
                FrmTips.ShowTipsError(this, "考试id为空!");
                return;
            }
            if (examId.IndexOf('_') != -1)
            {
                examId = examId.Substring(examId.IndexOf('_') + 1);
            }
            string url = ComboBox2.Text;
            if (string.IsNullOrEmpty(url))
            {
                FrmTips.ShowTipsError(this, "网址为空!");
                return;
            }
            url += RequestUrl.GetMachineCodeListUrl;

            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];
            RequestParameter.ExamId = examId;
            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);

            var formDatas = new List<FormItemModel>();
            //添加其他字段
            formDatas.Add(new FormItemModel()
            {
                Key = "data",
                Value = JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostForm(url, formDatas);
            GetMachineCodeList upload_Result = JsonConvert.DeserializeObject<GetMachineCodeList>(result);
            if (upload_Result == null || upload_Result.Results == null || upload_Result.Results.Count == 0)
            {
                string error = string.Empty;
                try
                {
                    error = upload_Result.Error;

                }
                catch (Exception)
                {

                    error = string.Empty;
                }
                FrmTips.ShowTipsError(this, $"提交错误,错误码:[{error}]");
                return;
            }

            foreach (var item in upload_Result.Results)
            {
                string str = $"{item.title}_{item.MachineCode}";
                ComboBox1.Items.Add(str);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            string Platform =ComboBox2.Text;
            string ExamId = combox3.Text;
            string MachineCode = ComboBox1.Text;
            if (!string.IsNullOrEmpty(Platform) && !string.IsNullOrEmpty(ExamId) && !string.IsNullOrEmpty(MachineCode))
            {
                bool  S=   EquipMentSettingWindowSys.Instance.SavePlatformSetting(Platform, ExamId, MachineCode);
                if (S)
                {
                    UIMessageBox.ShowSuccess("保存成功");
                    this.Close();
                }
                else
                {
                    UIMessageBox.ShowError("保存失败！！");
                    return;
                }
            }
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
             this.Close();
        }
    }
}
