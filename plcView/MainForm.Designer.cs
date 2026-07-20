namespace plcView
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxControl = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBoxPoints = new System.Windows.Forms.GroupBox();
            this.dgvPoints = new System.Windows.Forms.DataGridView();
            this.groupBoxProject = new System.Windows.Forms.GroupBox();
            this.btnSaveProject = new System.Windows.Forms.Button();
            this.btnOpenProject = new System.Windows.Forms.Button();
            this.btnNewProject = new System.Windows.Forms.Button();
            this.groupBoxPlc = new System.Windows.Forms.GroupBox();
            this.chkDebugMode = new System.Windows.Forms.CheckBox();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbInterval = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageMonitor = new System.Windows.Forms.TabPage();
            this.dgvMonitor = new System.Windows.Forms.DataGridView();
            this.panelMonitorControl = new System.Windows.Forms.Panel();
            this.rbUnitByte = new System.Windows.Forms.RadioButton();
            this.rbUnitWord = new System.Windows.Forms.RadioButton();
            this.cmbMonitorPoint = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPageHistory = new System.Windows.Forms.TabPage();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.panelHistoryControl = new System.Windows.Forms.Panel();
            this.lblHistoryTime = new System.Windows.Forms.Label();
            this.cmbHistorySpeed = new System.Windows.Forms.ComboBox();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.btnPrevFrame = new System.Windows.Forms.Button();
            this.btnPauseHistory = new System.Windows.Forms.Button();
            this.btnPlayHistory = new System.Windows.Forms.Button();
            this.trackHistory = new System.Windows.Forms.TrackBar();
            this.cmbHistoryPoint = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnOpenHistoryFile = new System.Windows.Forms.Button();
            this.txtHistoryFile = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageConfig.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.groupBoxControl.SuspendLayout();
            this.groupBoxPoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).BeginInit();
            this.groupBoxProject.SuspendLayout();
            this.groupBoxPlc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.tabPageMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonitor)).BeginInit();
            this.panelMonitorControl.SuspendLayout();
            this.tabPageHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.panelHistoryControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageConfig);
            this.tabControl1.Controls.Add(this.tabPageMonitor);
            this.tabControl1.Controls.Add(this.tabPageHistory);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(984, 661);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.Controls.Add(this.groupBoxLog);
            this.tabPageConfig.Controls.Add(this.groupBoxControl);
            this.tabPageConfig.Controls.Add(this.groupBoxPoints);
            this.tabPageConfig.Controls.Add(this.groupBoxProject);
            this.tabPageConfig.Controls.Add(this.groupBoxPlc);
            this.tabPageConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfig.Size = new System.Drawing.Size(976, 635);
            this.tabPageConfig.TabIndex = 0;
            this.tabPageConfig.Text = "設定・制御";
            this.tabPageConfig.UseVisualStyleBackColor = true;
            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLog.Controls.Add(this.txtLog);
            this.groupBoxLog.Location = new System.Drawing.Point(8, 480);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(960, 147);
            this.groupBoxLog.TabIndex = 4;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "通信・システムログ";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 15);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(954, 129);
            this.txtLog.TabIndex = 0;
            // 
            // groupBoxControl
            // 
            this.groupBoxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxControl.Controls.Add(this.lblStatus);
            this.groupBoxControl.Controls.Add(this.btnStop);
            this.groupBoxControl.Controls.Add(this.btnStart);
            this.groupBoxControl.Location = new System.Drawing.Point(748, 120);
            this.groupBoxControl.Name = "groupBoxControl";
            this.groupBoxControl.Size = new System.Drawing.Size(220, 110);
            this.groupBoxControl.TabIndex = 3;
            this.groupBoxControl.TabStop = false;
            this.groupBoxControl.Text = "データ収集制御";
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.LightGray;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStatus.Location = new System.Drawing.Point(15, 20);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(190, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "停止中";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(115, 65);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 35);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "収集停止";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(15, 65);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(90, 35);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "収集開始";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // groupBoxPoints
            // 
            this.groupBoxPoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPoints.Controls.Add(this.dgvPoints);
            this.groupBoxPoints.Location = new System.Drawing.Point(8, 236);
            this.groupBoxPoints.Name = "groupBoxPoints";
            this.groupBoxPoints.Size = new System.Drawing.Size(960, 238);
            this.groupBoxPoints.TabIndex = 2;
            this.groupBoxPoints.TabStop = false;
            this.groupBoxPoints.Text = "読出領域設定 (最大10ポイント)";
            // 
            // dgvPoints
            // 
            this.dgvPoints.AllowUserToAddRows = false;
            this.dgvPoints.AllowUserToDeleteRows = false;
            this.dgvPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPoints.Location = new System.Drawing.Point(3, 15);
            this.dgvPoints.Name = "dgvPoints";
            this.dgvPoints.RowTemplate.Height = 21;
            this.dgvPoints.Size = new System.Drawing.Size(954, 220);
            this.dgvPoints.TabIndex = 0;
            // 
            // groupBoxProject
            // 
            this.groupBoxProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProject.Controls.Add(this.btnSaveProject);
            this.groupBoxProject.Controls.Add(this.btnOpenProject);
            this.groupBoxProject.Controls.Add(this.btnNewProject);
            this.groupBoxProject.Location = new System.Drawing.Point(748, 6);
            this.groupBoxProject.Name = "groupBoxProject";
            this.groupBoxProject.Size = new System.Drawing.Size(220, 110);
            this.groupBoxProject.TabIndex = 1;
            this.groupBoxProject.TabStop = false;
            this.groupBoxProject.Text = "プロジェクト管理";
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.Location = new System.Drawing.Point(15, 75);
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(190, 25);
            this.btnSaveProject.TabIndex = 2;
            this.btnSaveProject.Text = "プロジェクトを保存";
            this.btnSaveProject.UseVisualStyleBackColor = true;
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Location = new System.Drawing.Point(15, 45);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(190, 25);
            this.btnOpenProject.TabIndex = 1;
            this.btnOpenProject.Text = "プロジェクトを開く...";
            this.btnOpenProject.UseVisualStyleBackColor = true;
            // 
            // btnNewProject
            // 
            this.btnNewProject.Location = new System.Drawing.Point(15, 15);
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(190, 25);
            this.btnNewProject.TabIndex = 0;
            this.btnNewProject.Text = "新規プロジェクト";
            this.btnNewProject.UseVisualStyleBackColor = true;
            // 
            // groupBoxPlc
            // 
            this.groupBoxPlc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPlc.Controls.Add(this.chkDebugMode);
            this.groupBoxPlc.Controls.Add(this.btnBrowseFolder);
            this.groupBoxPlc.Controls.Add(this.txtOutputFolder);
            this.groupBoxPlc.Controls.Add(this.label5);
            this.groupBoxPlc.Controls.Add(this.cmbInterval);
            this.groupBoxPlc.Controls.Add(this.label4);
            this.groupBoxPlc.Controls.Add(this.numTimeout);
            this.groupBoxPlc.Controls.Add(this.label3);
            this.groupBoxPlc.Controls.Add(this.numPort);
            this.groupBoxPlc.Controls.Add(this.label2);
            this.groupBoxPlc.Controls.Add(this.txtIpAddress);
            this.groupBoxPlc.Controls.Add(this.label1);
            this.groupBoxPlc.Location = new System.Drawing.Point(8, 6);
            this.groupBoxPlc.Name = "groupBoxPlc";
            this.groupBoxPlc.Size = new System.Drawing.Size(734, 224);
            this.groupBoxPlc.TabIndex = 0;
            this.groupBoxPlc.TabStop = false;
            this.groupBoxPlc.Text = "通信・保存設定";
            // 
            // chkDebugMode
            // 
            this.chkDebugMode.AutoSize = true;
            this.chkDebugMode.Location = new System.Drawing.Point(110, 190);
            this.chkDebugMode.Name = "chkDebugMode";
            this.chkDebugMode.Size = new System.Drawing.Size(200, 16);
            this.chkDebugMode.TabIndex = 11;
            this.chkDebugMode.Text = "デバッグモード有効 (logフォルダ保存)";
            this.chkDebugMode.UseVisualStyleBackColor = true;
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(420, 153);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFolder.TabIndex = 10;
            this.btnBrowseFolder.Text = "選択...";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(110, 155);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(300, 19);
            this.txtOutputFolder.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "データ保存フォルダ:";
            // 
            // cmbInterval
            // 
            this.cmbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterval.FormattingEnabled = true;
            this.cmbInterval.Location = new System.Drawing.Point(110, 120);
            this.cmbInterval.Name = "cmbInterval";
            this.cmbInterval.Size = new System.Drawing.Size(120, 20);
            this.cmbInterval.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "データ取得間隔:";
            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(110, 85);
            this.numTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(120, 19);
            this.numTimeout.TabIndex = 5;
            this.numTimeout.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "タイムアウト (ms):";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(110, 52);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(120, 19);
            this.numPort.TabIndex = 3;
            this.numPort.Value = new decimal(new int[] {
            5007,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ポート番号:";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(110, 22);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(200, 19);
            this.txtIpAddress.TabIndex = 1;
            this.txtIpAddress.Text = "192.168.3.39";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "PLC IPアドレス:";
            // 
            // tabPageMonitor
            // 
            this.tabPageMonitor.Controls.Add(this.dgvMonitor);
            this.tabPageMonitor.Controls.Add(this.panelMonitorControl);
            this.tabPageMonitor.Location = new System.Drawing.Point(4, 22);
            this.tabPageMonitor.Name = "tabPageMonitor";
            this.tabPageMonitor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMonitor.Size = new System.Drawing.Size(976, 635);
            this.tabPageMonitor.TabIndex = 1;
            this.tabPageMonitor.Text = "リアルタイムモニタ";
            this.tabPageMonitor.UseVisualStyleBackColor = true;
            // 
            // dgvMonitor
            // 
            this.dgvMonitor.AllowUserToAddRows = false;
            this.dgvMonitor.AllowUserToDeleteRows = false;
            this.dgvMonitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMonitor.Location = new System.Drawing.Point(3, 48);
            this.dgvMonitor.Name = "dgvMonitor";
            this.dgvMonitor.ReadOnly = true;
            this.dgvMonitor.RowTemplate.Height = 21;
            this.dgvMonitor.Size = new System.Drawing.Size(970, 584);
            this.dgvMonitor.TabIndex = 1;
            // 
            // panelMonitorControl
            // 
            this.panelMonitorControl.Controls.Add(this.rbUnitByte);
            this.panelMonitorControl.Controls.Add(this.rbUnitWord);
            this.panelMonitorControl.Controls.Add(this.cmbMonitorPoint);
            this.panelMonitorControl.Controls.Add(this.label6);
            this.panelMonitorControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMonitorControl.Location = new System.Drawing.Point(3, 3);
            this.panelMonitorControl.Name = "panelMonitorControl";
            this.panelMonitorControl.Size = new System.Drawing.Size(970, 45);
            this.panelMonitorControl.TabIndex = 0;
            // 
            // rbUnitByte
            // 
            this.rbUnitByte.AutoSize = true;
            this.rbUnitByte.Location = new System.Drawing.Point(380, 16);
            this.rbUnitByte.Name = "rbUnitByte";
            this.rbUnitByte.Size = new System.Drawing.Size(94, 16);
            this.rbUnitByte.TabIndex = 3;
            this.rbUnitByte.Text = "バイト単位(8bit)";
            this.rbUnitByte.UseVisualStyleBackColor = true;
            // 
            // rbUnitWord
            // 
            this.rbUnitWord.AutoSize = true;
            this.rbUnitWord.Checked = true;
            this.rbUnitWord.Location = new System.Drawing.Point(265, 16);
            this.rbUnitWord.Name = "rbUnitWord";
            this.rbUnitWord.Size = new System.Drawing.Size(97, 16);
            this.rbUnitWord.TabIndex = 2;
            this.rbUnitWord.TabStop = true;
            this.rbUnitWord.Text = "ワード単位(16bit)";
            this.rbUnitWord.UseVisualStyleBackColor = true;
            // 
            // cmbMonitorPoint
            // 
            this.cmbMonitorPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonitorPoint.FormattingEnabled = true;
            this.cmbMonitorPoint.Location = new System.Drawing.Point(95, 15);
            this.cmbMonitorPoint.Name = "cmbMonitorPoint";
            this.cmbMonitorPoint.Size = new System.Drawing.Size(150, 20);
            this.cmbMonitorPoint.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "モニタポイント:";
            // 
            // tabPageHistory
            // 
            this.tabPageHistory.Controls.Add(this.dgvHistory);
            this.tabPageHistory.Controls.Add(this.panelHistoryControl);
            this.tabPageHistory.Location = new System.Drawing.Point(4, 22);
            this.tabPageHistory.Name = "tabPageHistory";
            this.tabPageHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHistory.Size = new System.Drawing.Size(976, 635);
            this.tabPageHistory.TabIndex = 2;
            this.tabPageHistory.Text = "履歴再生ビューア";
            this.tabPageHistory.UseVisualStyleBackColor = true;
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.Location = new System.Drawing.Point(3, 118);
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowTemplate.Height = 21;
            this.dgvHistory.Size = new System.Drawing.Size(970, 514);
            this.dgvHistory.TabIndex = 2;
            // 
            // panelHistoryControl
            // 
            this.panelHistoryControl.Controls.Add(this.lblHistoryTime);
            this.panelHistoryControl.Controls.Add(this.cmbHistorySpeed);
            this.panelHistoryControl.Controls.Add(this.btnNextFrame);
            this.panelHistoryControl.Controls.Add(this.btnPrevFrame);
            this.panelHistoryControl.Controls.Add(this.btnPauseHistory);
            this.panelHistoryControl.Controls.Add(this.btnPlayHistory);
            this.panelHistoryControl.Controls.Add(this.trackHistory);
            this.panelHistoryControl.Controls.Add(this.cmbHistoryPoint);
            this.panelHistoryControl.Controls.Add(this.label8);
            this.panelHistoryControl.Controls.Add(this.btnOpenHistoryFile);
            this.panelHistoryControl.Controls.Add(this.txtHistoryFile);
            this.panelHistoryControl.Controls.Add(this.label7);
            this.panelHistoryControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHistoryControl.Location = new System.Drawing.Point(3, 3);
            this.panelHistoryControl.Name = "panelHistoryControl";
            this.panelHistoryControl.Size = new System.Drawing.Size(970, 115);
            this.panelHistoryControl.TabIndex = 1;
            // 
            // lblHistoryTime
            // 
            this.lblHistoryTime.AutoSize = true;
            this.lblHistoryTime.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblHistoryTime.Location = new System.Drawing.Point(620, 83);
            this.lblHistoryTime.Name = "lblHistoryTime";
            this.lblHistoryTime.Size = new System.Drawing.Size(132, 13);
            this.lblHistoryTime.TabIndex = 11;
            this.lblHistoryTime.Text = "再生日時: ----/--/-- --:--:--";
            // 
            // cmbHistorySpeed
            // 
            this.cmbHistorySpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHistorySpeed.FormattingEnabled = true;
            this.cmbHistorySpeed.Location = new System.Drawing.Point(530, 80);
            this.cmbHistorySpeed.Name = "cmbHistorySpeed";
            this.cmbHistorySpeed.Size = new System.Drawing.Size(80, 20);
            this.cmbHistorySpeed.TabIndex = 10;
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.Location = new System.Drawing.Point(475, 78);
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(40, 23);
            this.btnNextFrame.TabIndex = 9;
            this.btnNextFrame.Text = ">|";
            this.btnNextFrame.UseVisualStyleBackColor = true;
            // 
            // btnPrevFrame
            // 
            this.btnPrevFrame.Location = new System.Drawing.Point(355, 78);
            this.btnPrevFrame.Name = "btnPrevFrame";
            this.btnPrevFrame.Size = new System.Drawing.Size(40, 23);
            this.btnPrevFrame.TabIndex = 8;
            this.btnPrevFrame.Text = "|<";
            this.btnPrevFrame.UseVisualStyleBackColor = true;
            // 
            // btnPauseHistory
            // 
            this.btnPauseHistory.Location = new System.Drawing.Point(435, 78);
            this.btnPauseHistory.Name = "btnPauseHistory";
            this.btnPauseHistory.Size = new System.Drawing.Size(35, 23);
            this.btnPauseHistory.TabIndex = 7;
            this.btnPauseHistory.Text = "||";
            this.btnPauseHistory.UseVisualStyleBackColor = true;
            // 
            // btnPlayHistory
            // 
            this.btnPlayHistory.Location = new System.Drawing.Point(395, 78);
            this.btnPlayHistory.Name = "btnPlayHistory";
            this.btnPlayHistory.Size = new System.Drawing.Size(35, 23);
            this.btnPlayHistory.TabIndex = 6;
            this.btnPlayHistory.Text = "▶";
            this.btnPlayHistory.UseVisualStyleBackColor = true;
            // 
            // trackHistory
            // 
            this.trackHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackHistory.LargeChange = 10;
            this.trackHistory.Location = new System.Drawing.Point(260, 36);
            this.trackHistory.Maximum = 100;
            this.trackHistory.Name = "trackHistory";
            this.trackHistory.Size = new System.Drawing.Size(695, 45);
            this.trackHistory.TabIndex = 5;
            this.trackHistory.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // cmbHistoryPoint
            // 
            this.cmbHistoryPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHistoryPoint.FormattingEnabled = true;
            this.cmbHistoryPoint.Location = new System.Drawing.Point(95, 80);
            this.cmbHistoryPoint.Name = "cmbHistoryPoint";
            this.cmbHistoryPoint.Size = new System.Drawing.Size(150, 20);
            this.cmbHistoryPoint.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "表示ポイント:";
            // 
            // btnOpenHistoryFile
            // 
            this.btnOpenHistoryFile.Location = new System.Drawing.Point(405, 10);
            this.btnOpenHistoryFile.Name = "btnOpenHistoryFile";
            this.btnOpenHistoryFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenHistoryFile.TabIndex = 2;
            this.btnOpenHistoryFile.Text = "選択...";
            this.btnOpenHistoryFile.UseVisualStyleBackColor = true;
            // 
            // txtHistoryFile
            // 
            this.txtHistoryFile.Location = new System.Drawing.Point(95, 12);
            this.txtHistoryFile.Name = "txtHistoryFile";
            this.txtHistoryFile.ReadOnly = true;
            this.txtHistoryFile.Size = new System.Drawing.Size(300, 19);
            this.txtHistoryFile.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "履歴ファイル:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.Text = "plcView - PLC Data Collector & Viewer";
            this.tabControl1.ResumeLayout(false);
            this.tabPageConfig.ResumeLayout(false);
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.groupBoxControl.ResumeLayout(false);
            this.groupBoxPoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).EndInit();
            this.groupBoxProject.ResumeLayout(false);
            this.groupBoxPlc.ResumeLayout(false);
            this.groupBoxPlc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.tabPageMonitor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonitor)).EndInit();
            this.panelMonitorControl.ResumeLayout(false);
            this.panelMonitorControl.PerformLayout();
            this.tabPageHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.panelHistoryControl.ResumeLayout(false);
            this.panelHistoryControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageConfig;
        private System.Windows.Forms.TabPage tabPageMonitor;
        private System.Windows.Forms.TabPage tabPageHistory;
        private System.Windows.Forms.GroupBox groupBoxPlc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.CheckBox chkDebugMode;
        private System.Windows.Forms.GroupBox groupBoxProject;
        private System.Windows.Forms.Button btnNewProject;
        private System.Windows.Forms.Button btnOpenProject;
        private System.Windows.Forms.Button btnSaveProject;
        private System.Windows.Forms.GroupBox groupBoxPoints;
        private System.Windows.Forms.DataGridView dgvPoints;
        private System.Windows.Forms.GroupBox groupBoxControl;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel panelMonitorControl;
        private System.Windows.Forms.ComboBox cmbMonitorPoint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbUnitByte;
        private System.Windows.Forms.RadioButton rbUnitWord;
        private System.Windows.Forms.DataGridView dgvMonitor;
        private System.Windows.Forms.Panel panelHistoryControl;
        private System.Windows.Forms.TextBox txtHistoryFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOpenHistoryFile;
        private System.Windows.Forms.ComboBox cmbHistoryPoint;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar trackHistory;
        private System.Windows.Forms.Button btnPlayHistory;
        private System.Windows.Forms.Button btnPauseHistory;
        private System.Windows.Forms.Button btnPrevFrame;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.ComboBox cmbHistorySpeed;
        private System.Windows.Forms.Label lblHistoryTime;
        private System.Windows.Forms.DataGridView dgvHistory;
    }
}
