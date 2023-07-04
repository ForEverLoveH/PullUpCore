namespace CamerADCore.GameSystem.GameWindow
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem1 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem2 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem3 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem4 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem5 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem6 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem7 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem8 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem9 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem10 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem11 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem12 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem13 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem14 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem15 = new HZH_Controls.Controls.NavigationMenuItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.ucProcessLine1 = new HZH_Controls.Controls.UCProcessLine();
            this.uiTitlePanel3 = new Sunny.UI.UITitlePanel();
            this.StudentDataListview = new System.Windows.Forms.ListView();
            this.uiTitlePanel2 = new Sunny.UI.UITitlePanel();
            this.GroupTreeView = new System.Windows.Forms.TreeView();
            this.ucNavigationMenu1 = new HZH_Controls.Controls.UCNavigationMenu();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.uiTitlePanel1.SuspendLayout();
            this.uiTitlePanel3.SuspendLayout();
            this.uiTitlePanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.AutoSize = true;
            this.uiTitlePanel1.Controls.Add(this.ucProcessLine1);
            this.uiTitlePanel1.Controls.Add(this.uiTitlePanel3);
            this.uiTitlePanel1.Controls.Add(this.uiTitlePanel2);
            this.uiTitlePanel1.Controls.Add(this.ucNavigationMenu1);
            this.uiTitlePanel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel1.Location = new System.Drawing.Point(0, 2);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(1499, 798);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "德育龙测试系统";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // ucProcessLine1
            // 
            this.ucProcessLine1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(231)))), ((int)(((byte)(237)))));
            this.ucProcessLine1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucProcessLine1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ucProcessLine1.ForeColor = System.Drawing.Color.Black;
            this.ucProcessLine1.Location = new System.Drawing.Point(0, 784);
            this.ucProcessLine1.Margin = new System.Windows.Forms.Padding(2);
            this.ucProcessLine1.MaxValue = 100;
            this.ucProcessLine1.Name = "ucProcessLine1";
            this.ucProcessLine1.Size = new System.Drawing.Size(1499, 14);
            this.ucProcessLine1.TabIndex = 118;
            this.ucProcessLine1.Text = "ucProcessLine1";
            this.ucProcessLine1.Value = 0;
            this.ucProcessLine1.ValueBGColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(231)))), ((int)(((byte)(237)))));
            this.ucProcessLine1.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            this.ucProcessLine1.ValueTextType = HZH_Controls.Controls.ValueTextType.Percent;
            this.ucProcessLine1.Visible = false;
            // 
            // uiTitlePanel3
            // 
            this.uiTitlePanel3.AutoSize = true;
            this.uiTitlePanel3.Controls.Add(this.StudentDataListview);
            this.uiTitlePanel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel3.Location = new System.Drawing.Point(479, 76);
            this.uiTitlePanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel3.Name = "uiTitlePanel3";
            this.uiTitlePanel3.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel3.ShowText = false;
            this.uiTitlePanel3.Size = new System.Drawing.Size(1016, 713);
            this.uiTitlePanel3.TabIndex = 103;
            this.uiTitlePanel3.Text = "考生信息";
            this.uiTitlePanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // StudentDataListview
            // 
            this.StudentDataListview.FullRowSelect = true;
            this.StudentDataListview.HideSelection = false;
            this.StudentDataListview.Location = new System.Drawing.Point(4, 39);
            this.StudentDataListview.Name = "StudentDataListview";
            this.StudentDataListview.Size = new System.Drawing.Size(1006, 671);
            this.StudentDataListview.TabIndex = 0;
            this.StudentDataListview.UseCompatibleStateImageBehavior = false;
            // 
            // uiTitlePanel2
            // 
            this.uiTitlePanel2.AutoSize = true;
            this.uiTitlePanel2.Controls.Add(this.GroupTreeView);
            this.uiTitlePanel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel2.Location = new System.Drawing.Point(4, 76);
            this.uiTitlePanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel2.Name = "uiTitlePanel2";
            this.uiTitlePanel2.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel2.ShowText = false;
            this.uiTitlePanel2.Size = new System.Drawing.Size(457, 717);
            this.uiTitlePanel2.TabIndex = 102;
            this.uiTitlePanel2.Text = "组别信息";
            this.uiTitlePanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // GroupTreeView
            // 
            this.GroupTreeView.Location = new System.Drawing.Point(4, 38);
            this.GroupTreeView.Name = "GroupTreeView";
            this.GroupTreeView.Size = new System.Drawing.Size(450, 675);
            this.GroupTreeView.TabIndex = 0;
            this.GroupTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.GroupTreeView_NodeMouseClick);
            this.GroupTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GroupTreeView_MouseDown);
            // 
            // ucNavigationMenu1
            // 
            this.ucNavigationMenu1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucNavigationMenu1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ucNavigationMenu1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucNavigationMenu1.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucNavigationMenu1.ForeColor = System.Drawing.Color.Black;
            navigationMenuItem1.AnchorRight = false;
            navigationMenuItem1.DataSource = null;
            navigationMenuItem1.HasSplitLintAtTop = false;
            navigationMenuItem1.Icon = null;
            navigationMenuItem2.AnchorRight = false;
            navigationMenuItem2.DataSource = null;
            navigationMenuItem2.HasSplitLintAtTop = false;
            navigationMenuItem2.Icon = null;
            navigationMenuItem2.Items = null;
            navigationMenuItem2.ItemWidth = 100;
            navigationMenuItem2.ShowTip = false;
            navigationMenuItem2.Text = "项目设置";
            navigationMenuItem2.TipText = null;
            navigationMenuItem3.AnchorRight = false;
            navigationMenuItem3.DataSource = null;
            navigationMenuItem3.HasSplitLintAtTop = false;
            navigationMenuItem3.Icon = null;
            navigationMenuItem3.Items = null;
            navigationMenuItem3.ItemWidth = 100;
            navigationMenuItem3.ShowTip = false;
            navigationMenuItem3.Text = "系统参数设置";
            navigationMenuItem3.TipText = null;
            navigationMenuItem1.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem2,
        navigationMenuItem3};
            navigationMenuItem1.ItemWidth = 130;
            navigationMenuItem1.ShowTip = false;
            navigationMenuItem1.Text = "系统管理";
            navigationMenuItem1.TipText = null;
            navigationMenuItem4.AnchorRight = false;
            navigationMenuItem4.DataSource = null;
            navigationMenuItem4.HasSplitLintAtTop = false;
            navigationMenuItem4.Icon = null;
            navigationMenuItem5.AnchorRight = false;
            navigationMenuItem5.DataSource = null;
            navigationMenuItem5.HasSplitLintAtTop = false;
            navigationMenuItem5.Icon = null;
            navigationMenuItem5.Items = null;
            navigationMenuItem5.ItemWidth = 100;
            navigationMenuItem5.ShowTip = false;
            navigationMenuItem5.Text = "初始化数据库";
            navigationMenuItem5.TipText = null;
            navigationMenuItem6.AnchorRight = false;
            navigationMenuItem6.DataSource = null;
            navigationMenuItem6.HasSplitLintAtTop = false;
            navigationMenuItem6.Icon = null;
            navigationMenuItem6.Items = null;
            navigationMenuItem6.ItemWidth = 100;
            navigationMenuItem6.ShowTip = false;
            navigationMenuItem6.Text = "数据库备份";
            navigationMenuItem6.TipText = null;
            navigationMenuItem4.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem5,
        navigationMenuItem6};
            navigationMenuItem4.ItemWidth = 130;
            navigationMenuItem4.ShowTip = false;
            navigationMenuItem4.Text = "数据管理";
            navigationMenuItem4.TipText = null;
            navigationMenuItem7.AnchorRight = false;
            navigationMenuItem7.DataSource = null;
            navigationMenuItem7.HasSplitLintAtTop = false;
            navigationMenuItem7.Icon = null;
            navigationMenuItem8.AnchorRight = false;
            navigationMenuItem8.DataSource = null;
            navigationMenuItem8.HasSplitLintAtTop = false;
            navigationMenuItem8.Icon = null;
            navigationMenuItem8.Items = null;
            navigationMenuItem8.ItemWidth = 100;
            navigationMenuItem8.ShowTip = false;
            navigationMenuItem8.Text = "导入成绩模板";
            navigationMenuItem8.TipText = null;
            navigationMenuItem9.AnchorRight = false;
            navigationMenuItem9.DataSource = null;
            navigationMenuItem9.HasSplitLintAtTop = false;
            navigationMenuItem9.Icon = null;
            navigationMenuItem9.Items = null;
            navigationMenuItem9.ItemWidth = 100;
            navigationMenuItem9.ShowTip = false;
            navigationMenuItem9.Text = "导入名单模板";
            navigationMenuItem9.TipText = null;
            navigationMenuItem7.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem8,
        navigationMenuItem9};
            navigationMenuItem7.ItemWidth = 100;
            navigationMenuItem7.ShowTip = false;
            navigationMenuItem7.Text = "帮助";
            navigationMenuItem7.TipText = null;
            navigationMenuItem10.AnchorRight = true;
            navigationMenuItem10.DataSource = null;
            navigationMenuItem10.HasSplitLintAtTop = false;
            navigationMenuItem10.Icon = null;
            navigationMenuItem10.Items = null;
            navigationMenuItem10.ItemWidth = 120;
            navigationMenuItem10.ShowTip = false;
            navigationMenuItem10.Text = "退出";
            navigationMenuItem10.TipText = null;
            navigationMenuItem11.AnchorRight = true;
            navigationMenuItem11.DataSource = null;
            navigationMenuItem11.HasSplitLintAtTop = false;
            navigationMenuItem11.Icon = null;
            navigationMenuItem11.Items = null;
            navigationMenuItem11.ItemWidth = 130;
            navigationMenuItem11.ShowTip = false;
            navigationMenuItem11.Text = "启动测试";
            navigationMenuItem11.TipText = "";
            navigationMenuItem12.AnchorRight = false;
            navigationMenuItem12.DataSource = null;
            navigationMenuItem12.HasSplitLintAtTop = false;
            navigationMenuItem12.Icon = null;
            navigationMenuItem13.AnchorRight = false;
            navigationMenuItem13.DataSource = null;
            navigationMenuItem13.HasSplitLintAtTop = false;
            navigationMenuItem13.Icon = null;
            navigationMenuItem13.Items = null;
            navigationMenuItem13.ItemWidth = 100;
            navigationMenuItem13.ShowTip = false;
            navigationMenuItem13.Text = "上传成绩";
            navigationMenuItem13.TipText = null;
            navigationMenuItem14.AnchorRight = false;
            navigationMenuItem14.DataSource = null;
            navigationMenuItem14.HasSplitLintAtTop = false;
            navigationMenuItem14.Icon = null;
            navigationMenuItem14.Items = null;
            navigationMenuItem14.ItemWidth = 100;
            navigationMenuItem14.ShowTip = false;
            navigationMenuItem14.Text = "修正成绩";
            navigationMenuItem14.TipText = null;
            navigationMenuItem15.AnchorRight = false;
            navigationMenuItem15.DataSource = null;
            navigationMenuItem15.HasSplitLintAtTop = false;
            navigationMenuItem15.Icon = null;
            navigationMenuItem15.Items = null;
            navigationMenuItem15.ItemWidth = 100;
            navigationMenuItem15.ShowTip = false;
            navigationMenuItem15.Text = "导出成绩";
            navigationMenuItem15.TipText = null;
            navigationMenuItem12.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem13,
        navigationMenuItem14,
        navigationMenuItem15};
            navigationMenuItem12.ItemWidth = 100;
            navigationMenuItem12.ShowTip = false;
            navigationMenuItem12.Text = "成绩管理";
            navigationMenuItem12.TipText = null;
            this.ucNavigationMenu1.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem1,
        navigationMenuItem4,
        navigationMenuItem7,
        navigationMenuItem10,
        navigationMenuItem11,
        navigationMenuItem12};
            this.ucNavigationMenu1.Location = new System.Drawing.Point(0, 35);
            this.ucNavigationMenu1.Name = "ucNavigationMenu1";
            this.ucNavigationMenu1.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.ucNavigationMenu1.Size = new System.Drawing.Size(1499, 37);
            this.ucNavigationMenu1.TabIndex = 101;
            this.ucNavigationMenu1.TabStop = false;
            this.ucNavigationMenu1.TipColor = System.Drawing.Color.Black;
            this.ucNavigationMenu1.ClickItemed += new System.EventHandler(this.ucNavigationMenu1_ClickItemed);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1404, 819);
            this.Controls.Add(this.uiTitlePanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "主页面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTitlePanel1.PerformLayout();
            this.uiTitlePanel3.ResumeLayout(false);
            this.uiTitlePanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private HZH_Controls.Controls.UCProcessLine ucProcessLine1;
        private Sunny.UI.UITitlePanel uiTitlePanel3;
        private System.Windows.Forms.ListView StudentDataListview;
        private Sunny.UI.UITitlePanel uiTitlePanel2;
        private System.Windows.Forms.TreeView GroupTreeView;
        private HZH_Controls.Controls.UCNavigationMenu ucNavigationMenu1;
        private System.Windows.Forms.Timer timer1;
    }
}