using System.ComponentModel;

namespace CamerADCore.GameSystem.GameWindow
{
    partial class ProjectSettingWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectSettingWindow));
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiTitlePanel5 = new Sunny.UI.UITitlePanel();
            this.ucDataGridView1 = new HZH_Controls.Controls.UCDataGridView();
            this.uiTitlePanel4 = new Sunny.UI.UITitlePanel();
            this.uiButton2 = new Sunny.UI.UIButton();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiTitlePanel3 = new Sunny.UI.UITitlePanel();
            this.txt_Type = new Sunny.UI.UIComboBox();
            this.txt_BestScoreMode = new Sunny.UI.UIComboBox();
            this.txt_FloatType = new Sunny.UI.UIComboBox();
            this.uiLabel7 = new Sunny.UI.UILabel();
            this.uiLabel6 = new Sunny.UI.UILabel();
            this.uiLabel5 = new Sunny.UI.UILabel();
            this.txt_TestMethod = new Sunny.UI.UIComboBox();
            this.uiLabel4 = new Sunny.UI.UILabel();
            this.txt_RoundCount = new Sunny.UI.UIComboBox();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.txt_projectName = new Sunny.UI.UITextBox();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiButton5 = new Sunny.UI.UIButton();
            this.uiButton4 = new Sunny.UI.UIButton();
            this.uiButton3 = new Sunny.UI.UIButton();
            this.uiTitlePanel2 = new Sunny.UI.UITitlePanel();
            this.ProjectTreeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.插入项目ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除项目ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiTitlePanel1.SuspendLayout();
            this.uiTitlePanel5.SuspendLayout();
            this.uiTitlePanel4.SuspendLayout();
            this.uiTitlePanel3.SuspendLayout();
            this.uiTitlePanel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiTitlePanel5);
            this.uiTitlePanel1.Controls.Add(this.uiTitlePanel4);
            this.uiTitlePanel1.Controls.Add(this.uiTitlePanel3);
            this.uiTitlePanel1.Controls.Add(this.uiTitlePanel2);
            this.uiTitlePanel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel1.Location = new System.Drawing.Point(0, 2);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(1462, 733);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "德育龙测试系统";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiTitlePanel5
            // 
            this.uiTitlePanel5.Controls.Add(this.ucDataGridView1);
            this.uiTitlePanel5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel5.Location = new System.Drawing.Point(358, 216);
            this.uiTitlePanel5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel5.Name = "uiTitlePanel5";
            this.uiTitlePanel5.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel5.ShowText = false;
            this.uiTitlePanel5.Size = new System.Drawing.Size(1100, 512);
            this.uiTitlePanel5.TabIndex = 3;
            this.uiTitlePanel5.Text = "学生名单";
            this.uiTitlePanel5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // ucDataGridView1
            // 
            this.ucDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.ucDataGridView1.BackColor = System.Drawing.Color.White;
            this.ucDataGridView1.Columns = null;
            this.ucDataGridView1.DataSource = null;
            this.ucDataGridView1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucDataGridView1.HeadFont = new System.Drawing.Font("微软雅黑", 12F);
            this.ucDataGridView1.HeadHeight = 25;
            this.ucDataGridView1.HeadPadingLeft = 0;
            this.ucDataGridView1.HeadTextColor = System.Drawing.Color.Black;
            this.ucDataGridView1.IsShowCheckBox = false;
            this.ucDataGridView1.IsShowHead = true;
            this.ucDataGridView1.Location = new System.Drawing.Point(13, 48);
            this.ucDataGridView1.Name = "ucDataGridView1";
            this.ucDataGridView1.Padding = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.ucDataGridView1.RowHeight = 23;
            this.ucDataGridView1.RowType = typeof(HZH_Controls.Controls.UCDataGridViewRow);
            this.ucDataGridView1.Size = new System.Drawing.Size(1080, 452);
            this.ucDataGridView1.TabIndex = 2;
            // 
            // uiTitlePanel4
            // 
            this.uiTitlePanel4.Controls.Add(this.uiButton2);
            this.uiTitlePanel4.Controls.Add(this.uiButton1);
            this.uiTitlePanel4.Controls.Add(this.uiTextBox1);
            this.uiTitlePanel4.Controls.Add(this.uiLabel1);
            this.uiTitlePanel4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel4.Location = new System.Drawing.Point(1127, 39);
            this.uiTitlePanel4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel4.Name = "uiTitlePanel4";
            this.uiTitlePanel4.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel4.ShowText = false;
            this.uiTitlePanel4.Size = new System.Drawing.Size(331, 167);
            this.uiTitlePanel4.TabIndex = 2;
            this.uiTitlePanel4.Text = "组别操作";
            this.uiTitlePanel4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiButton2
            // 
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.Location = new System.Drawing.Point(224, 129);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.Size = new System.Drawing.Size(100, 35);
            this.uiButton2.TabIndex = 3;
            this.uiButton2.Text = "删除本组";
            this.uiButton2.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton2.Click += new System.EventHandler(this.uiButton2_Click);
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Location = new System.Drawing.Point(37, 127);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(100, 35);
            this.uiButton1.TabIndex = 2;
            this.uiButton1.Text = "删除选择";
            this.uiButton1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox1.Location = new System.Drawing.Point(127, 44);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(196, 29);
            this.uiTextBox1.TabIndex = 1;
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox1.Watermark = "";
            this.uiTextBox1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(17, 44);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(103, 29);
            this.uiLabel1.TabIndex = 0;
            this.uiLabel1.Text = "组别操作：";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiTitlePanel3
            // 
            this.uiTitlePanel3.Controls.Add(this.txt_Type);
            this.uiTitlePanel3.Controls.Add(this.txt_BestScoreMode);
            this.uiTitlePanel3.Controls.Add(this.txt_FloatType);
            this.uiTitlePanel3.Controls.Add(this.uiLabel7);
            this.uiTitlePanel3.Controls.Add(this.uiLabel6);
            this.uiTitlePanel3.Controls.Add(this.uiLabel5);
            this.uiTitlePanel3.Controls.Add(this.txt_TestMethod);
            this.uiTitlePanel3.Controls.Add(this.uiLabel4);
            this.uiTitlePanel3.Controls.Add(this.txt_RoundCount);
            this.uiTitlePanel3.Controls.Add(this.uiLabel3);
            this.uiTitlePanel3.Controls.Add(this.txt_projectName);
            this.uiTitlePanel3.Controls.Add(this.uiLabel2);
            this.uiTitlePanel3.Controls.Add(this.uiButton5);
            this.uiTitlePanel3.Controls.Add(this.uiButton4);
            this.uiTitlePanel3.Controls.Add(this.uiButton3);
            this.uiTitlePanel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel3.Location = new System.Drawing.Point(358, 40);
            this.uiTitlePanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel3.Name = "uiTitlePanel3";
            this.uiTitlePanel3.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel3.ShowText = false;
            this.uiTitlePanel3.Size = new System.Drawing.Size(761, 166);
            this.uiTitlePanel3.TabIndex = 1;
            this.uiTitlePanel3.Text = "项目操作";
            this.uiTitlePanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_Type
            // 
            this.txt_Type.DataSource = null;
            this.txt_Type.FillColor = System.Drawing.Color.White;
            this.txt_Type.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_Type.Items.AddRange(new object[] { "仰卧起坐", "引体向上" });
            this.txt_Type.Location = new System.Drawing.Point(120, 129);
            this.txt_Type.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Type.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_Type.Name = "txt_Type";
            this.txt_Type.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_Type.Size = new System.Drawing.Size(150, 29);
            this.txt_Type.TabIndex = 14;
            this.txt_Type.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_Type.Watermark = "";
            this.txt_Type.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_BestScoreMode
            // 
            this.txt_BestScoreMode.DataSource = null;
            this.txt_BestScoreMode.FillColor = System.Drawing.Color.White;
            this.txt_BestScoreMode.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_BestScoreMode.Items.AddRange(new object[] { "末位删除", "非零进一", "四舍五入" });
            this.txt_BestScoreMode.Location = new System.Drawing.Point(384, 129);
            this.txt_BestScoreMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_BestScoreMode.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_BestScoreMode.Name = "txt_BestScoreMode";
            this.txt_BestScoreMode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_BestScoreMode.Size = new System.Drawing.Size(128, 29);
            this.txt_BestScoreMode.TabIndex = 13;
            this.txt_BestScoreMode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_BestScoreMode.Watermark = "";
            this.txt_BestScoreMode.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_FloatType
            // 
            this.txt_FloatType.DataSource = null;
            this.txt_FloatType.FillColor = System.Drawing.Color.White;
            this.txt_FloatType.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_FloatType.Items.AddRange(new object[] { "小数点后0位", "小数点后1位", "小数点后2位", "小数点后3位", "小数点后4位" });
            this.txt_FloatType.Location = new System.Drawing.Point(614, 129);
            this.txt_FloatType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_FloatType.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_FloatType.Name = "txt_FloatType";
            this.txt_FloatType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_FloatType.Size = new System.Drawing.Size(143, 32);
            this.txt_FloatType.TabIndex = 12;
            this.txt_FloatType.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_FloatType.Watermark = "";
            this.txt_FloatType.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel7
            // 
            this.uiLabel7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel7.Location = new System.Drawing.Point(519, 129);
            this.uiLabel7.Name = "uiLabel7";
            this.uiLabel7.Size = new System.Drawing.Size(100, 23);
            this.uiLabel7.TabIndex = 11;
            this.uiLabel7.Text = "保留位数：";
            this.uiLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel7.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel6
            // 
            this.uiLabel6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel6.Location = new System.Drawing.Point(281, 129);
            this.uiLabel6.Name = "uiLabel6";
            this.uiLabel6.Size = new System.Drawing.Size(100, 23);
            this.uiLabel6.TabIndex = 10;
            this.uiLabel6.Text = "成绩取值：";
            this.uiLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel6.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel5
            // 
            this.uiLabel5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel5.Location = new System.Drawing.Point(13, 128);
            this.uiLabel5.Name = "uiLabel5";
            this.uiLabel5.Size = new System.Drawing.Size(100, 23);
            this.uiLabel5.TabIndex = 9;
            this.uiLabel5.Text = "项目类型：";
            this.uiLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_TestMethod
            // 
            this.txt_TestMethod.DataSource = null;
            this.txt_TestMethod.FillColor = System.Drawing.Color.White;
            this.txt_TestMethod.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_TestMethod.Items.AddRange(new object[] { "自动下一位", "自动下一轮" });
            this.txt_TestMethod.Location = new System.Drawing.Point(614, 80);
            this.txt_TestMethod.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_TestMethod.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_TestMethod.Name = "txt_TestMethod";
            this.txt_TestMethod.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_TestMethod.Size = new System.Drawing.Size(143, 29);
            this.txt_TestMethod.TabIndex = 8;
            this.txt_TestMethod.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_TestMethod.Watermark = "";
            this.txt_TestMethod.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel4
            // 
            this.uiLabel4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel4.Location = new System.Drawing.Point(519, 80);
            this.uiLabel4.Name = "uiLabel4";
            this.uiLabel4.Size = new System.Drawing.Size(100, 29);
            this.uiLabel4.TabIndex = 7;
            this.uiLabel4.Text = "比赛方式：";
            this.uiLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_RoundCount
            // 
            this.txt_RoundCount.DataSource = null;
            this.txt_RoundCount.FillColor = System.Drawing.Color.White;
            this.txt_RoundCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_RoundCount.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            this.txt_RoundCount.Location = new System.Drawing.Point(384, 80);
            this.txt_RoundCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_RoundCount.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_RoundCount.Name = "txt_RoundCount";
            this.txt_RoundCount.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_RoundCount.Size = new System.Drawing.Size(128, 29);
            this.txt_RoundCount.TabIndex = 6;
            this.txt_RoundCount.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_RoundCount.Watermark = "";
            this.txt_RoundCount.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel3.Location = new System.Drawing.Point(277, 80);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(100, 23);
            this.uiLabel3.TabIndex = 5;
            this.uiLabel3.Text = "比赛轮次：";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_projectName
            // 
            this.txt_projectName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_projectName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_projectName.Location = new System.Drawing.Point(120, 80);
            this.txt_projectName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_projectName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txt_projectName.Name = "txt_projectName";
            this.txt_projectName.ShowText = false;
            this.txt_projectName.Size = new System.Drawing.Size(150, 29);
            this.txt_projectName.TabIndex = 4;
            this.txt_projectName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_projectName.Watermark = "";
            this.txt_projectName.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(13, 80);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(100, 23);
            this.uiLabel2.TabIndex = 3;
            this.uiLabel2.Text = "项目名称：";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiButton5
            // 
            this.uiButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton5.Location = new System.Drawing.Point(253, 38);
            this.uiButton5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton5.Name = "uiButton5";
            this.uiButton5.Size = new System.Drawing.Size(109, 35);
            this.uiButton5.TabIndex = 2;
            this.uiButton5.Text = "保存项目设置";
            this.uiButton5.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton5.Click += new System.EventHandler(this.uiButton5_Click);
            // 
            // uiButton4
            // 
            this.uiButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton4.Location = new System.Drawing.Point(131, 38);
            this.uiButton4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton4.Name = "uiButton4";
            this.uiButton4.Size = new System.Drawing.Size(100, 35);
            this.uiButton4.TabIndex = 1;
            this.uiButton4.Text = "模板导出";
            this.uiButton4.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton4.Click += new System.EventHandler(this.uiButton4_Click);
            // 
            // uiButton3
            // 
            this.uiButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton3.Location = new System.Drawing.Point(13, 38);
            this.uiButton3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton3.Name = "uiButton3";
            this.uiButton3.Size = new System.Drawing.Size(100, 35);
            this.uiButton3.TabIndex = 0;
            this.uiButton3.Text = "名单导入";
            this.uiButton3.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton3.Click += new System.EventHandler(this.uiButton3_Click);
            // 
            // uiTitlePanel2
            // 
            this.uiTitlePanel2.Controls.Add(this.ProjectTreeView);
            this.uiTitlePanel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel2.Location = new System.Drawing.Point(4, 40);
            this.uiTitlePanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel2.Name = "uiTitlePanel2";
            this.uiTitlePanel2.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel2.ShowText = false;
            this.uiTitlePanel2.Size = new System.Drawing.Size(346, 688);
            this.uiTitlePanel2.TabIndex = 0;
            this.uiTitlePanel2.Text = "项目组";
            this.uiTitlePanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // ProjectTreeView
            // 
            this.ProjectTreeView.Location = new System.Drawing.Point(4, 43);
            this.ProjectTreeView.Name = "ProjectTreeView";
            this.ProjectTreeView.Size = new System.Drawing.Size(339, 642);
            this.ProjectTreeView.TabIndex = 0;
            this.ProjectTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ProjectTreeView_NodeMouseClick);
            this.ProjectTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProjectTreeView_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.插入项目ToolStripMenuItem, this.删除项目ToolStripMenuItem });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // 插入项目ToolStripMenuItem
            // 
            this.插入项目ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.插入项目ToolStripMenuItem.Name = "插入项目ToolStripMenuItem";
            this.插入项目ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.插入项目ToolStripMenuItem.Text = "插入项目";
            this.插入项目ToolStripMenuItem.Click += new System.EventHandler(this.插入项目ToolStripMenuItem_Click);
            // 
            // 删除项目ToolStripMenuItem
            // 
            this.删除项目ToolStripMenuItem.Name = "删除项目ToolStripMenuItem";
            this.删除项目ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.删除项目ToolStripMenuItem.Text = "删除项目";
            this.删除项目ToolStripMenuItem.Click += new System.EventHandler(this.删除项目ToolStripMenuItem_Click);
            // 
            // ProjectSettingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1463, 740);
            this.Controls.Add(this.uiTitlePanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProjectSettingWindow";
            this.Text = "项目设置";
            this.Load += new System.EventHandler(this.ProjectSettingWindow_Load);
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTitlePanel5.ResumeLayout(false);
            this.uiTitlePanel4.ResumeLayout(false);
            this.uiTitlePanel3.ResumeLayout(false);
            this.uiTitlePanel2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UITitlePanel uiTitlePanel5;
        private Sunny.UI.UITitlePanel uiTitlePanel4;
        private Sunny.UI.UIButton uiButton2;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UITitlePanel uiTitlePanel3;
        private Sunny.UI.UIComboBox txt_Type;
        private Sunny.UI.UIComboBox txt_BestScoreMode;
        private Sunny.UI.UIComboBox txt_FloatType;
        private Sunny.UI.UILabel uiLabel7;
        private Sunny.UI.UILabel uiLabel6;
        private Sunny.UI.UILabel uiLabel5;
        private Sunny.UI.UIComboBox txt_TestMethod;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UIComboBox txt_RoundCount;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UITextBox txt_projectName;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIButton uiButton5;
        private Sunny.UI.UIButton uiButton4;
        private Sunny.UI.UIButton uiButton3;
        private Sunny.UI.UITitlePanel uiTitlePanel2;
        private System.Windows.Forms.TreeView ProjectTreeView;
        private HZH_Controls.Controls.UCDataGridView ucDataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 插入项目ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除项目ToolStripMenuItem;
    }
}