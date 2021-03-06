﻿namespace DemoApp
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.stateLlabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.portsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.memoryIndexGetButton = new System.Windows.Forms.Button();
            this.memoryDataLongGetButton = new System.Windows.Forms.Button();
            this.latestDataLongGetButton = new System.Windows.Forms.Button();
            this.measurementCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dataChartAutoScrollCheckBox = new System.Windows.Forms.CheckBox();
            this.dataChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.latestDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.dataFromServerRadioButton = new System.Windows.Forms.RadioButton();
            this.dataFromSensorRadioButton = new System.Windows.Forms.RadioButton();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.measurementTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.latestDataGridView)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1355, 889);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(4, 78);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1347, 66);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "接続";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.disconnectButton, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.connectButton, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.stateLlabel, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.portsComboBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 19);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1339, 43);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // disconnectButton
            // 
            this.disconnectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disconnectButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.disconnectButton.Location = new System.Drawing.Point(669, 4);
            this.disconnectButton.Margin = new System.Windows.Forms.Padding(4);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(125, 42);
            this.disconnectButton.TabIndex = 6;
            this.disconnectButton.Text = "切断";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.connectButton.Location = new System.Drawing.Point(536, 4);
            this.connectButton.Margin = new System.Windows.Forms.Padding(4);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(125, 42);
            this.connectButton.TabIndex = 5;
            this.connectButton.Text = "接続";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // stateLlabel
            // 
            this.stateLlabel.AutoSize = true;
            this.stateLlabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateLlabel.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stateLlabel.Location = new System.Drawing.Point(403, 0);
            this.stateLlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stateLlabel.Name = "stateLlabel";
            this.stateLlabel.Size = new System.Drawing.Size(125, 50);
            this.stateLlabel.TabIndex = 4;
            this.stateLlabel.Text = "-";
            this.stateLlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(270, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 50);
            this.label2.TabIndex = 3;
            this.label2.Text = "状態";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // portsComboBox
            // 
            this.portsComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portsComboBox.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.portsComboBox.FormattingEnabled = true;
            this.portsComboBox.Location = new System.Drawing.Point(137, 4);
            this.portsComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.portsComboBox.Name = "portsComboBox";
            this.portsComboBox.Size = new System.Drawing.Size(125, 31);
            this.portsComboBox.TabIndex = 2;
            this.portsComboBox.SelectedIndexChanged += new System.EventHandler(this.PortsComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "ポート";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(4, 152);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(1347, 66);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "制御";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.memoryIndexGetButton, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.memoryDataLongGetButton, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.latestDataLongGetButton, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.measurementCheckBox, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 19);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1339, 43);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // memoryIndexGetButton
            // 
            this.memoryIndexGetButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryIndexGetButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.memoryIndexGetButton.Location = new System.Drawing.Point(1006, 4);
            this.memoryIndexGetButton.Margin = new System.Windows.Forms.Padding(4);
            this.memoryIndexGetButton.Name = "memoryIndexGetButton";
            this.memoryIndexGetButton.Size = new System.Drawing.Size(329, 42);
            this.memoryIndexGetButton.TabIndex = 6;
            this.memoryIndexGetButton.Text = "保存データ数取得";
            this.memoryIndexGetButton.UseVisualStyleBackColor = true;
            this.memoryIndexGetButton.Click += new System.EventHandler(this.MemoryIndexGetButton_Click);
            // 
            // memoryDataLongGetButton
            // 
            this.memoryDataLongGetButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryDataLongGetButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.memoryDataLongGetButton.Location = new System.Drawing.Point(672, 4);
            this.memoryDataLongGetButton.Margin = new System.Windows.Forms.Padding(4);
            this.memoryDataLongGetButton.Name = "memoryDataLongGetButton";
            this.memoryDataLongGetButton.Size = new System.Drawing.Size(326, 42);
            this.memoryDataLongGetButton.TabIndex = 5;
            this.memoryDataLongGetButton.Text = "保存データ取得";
            this.memoryDataLongGetButton.UseVisualStyleBackColor = true;
            this.memoryDataLongGetButton.Click += new System.EventHandler(this.MemoryDataLongGetButton_Click);
            // 
            // latestDataLongGetButton
            // 
            this.latestDataLongGetButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.latestDataLongGetButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.latestDataLongGetButton.Location = new System.Drawing.Point(4, 4);
            this.latestDataLongGetButton.Margin = new System.Windows.Forms.Padding(4);
            this.latestDataLongGetButton.Name = "latestDataLongGetButton";
            this.latestDataLongGetButton.Size = new System.Drawing.Size(326, 42);
            this.latestDataLongGetButton.TabIndex = 4;
            this.latestDataLongGetButton.Text = "最新データ取得";
            this.latestDataLongGetButton.UseVisualStyleBackColor = true;
            this.latestDataLongGetButton.Click += new System.EventHandler(this.LatestDataLongGetButton_Click);
            // 
            // measurementCheckBox
            // 
            this.measurementCheckBox.AutoSize = true;
            this.measurementCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.measurementCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.measurementCheckBox.Location = new System.Drawing.Point(338, 4);
            this.measurementCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.measurementCheckBox.Name = "measurementCheckBox";
            this.measurementCheckBox.Size = new System.Drawing.Size(326, 42);
            this.measurementCheckBox.TabIndex = 7;
            this.measurementCheckBox.Text = "継続測定";
            this.measurementCheckBox.UseVisualStyleBackColor = true;
            this.measurementCheckBox.CheckedChanged += new System.EventHandler(this.MeasurementCheckBox_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel4);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(4, 226);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(1347, 659);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "データ";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel4.Controls.Add(this.dataChartAutoScrollCheckBox, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.dataChart, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.latestDataGridView, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 19);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1339, 636);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // dataChartAutoScrollCheckBox
            // 
            this.dataChartAutoScrollCheckBox.AutoSize = true;
            this.dataChartAutoScrollCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataChartAutoScrollCheckBox.Location = new System.Drawing.Point(4, 4);
            this.dataChartAutoScrollCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.dataChartAutoScrollCheckBox.Name = "dataChartAutoScrollCheckBox";
            this.dataChartAutoScrollCheckBox.Size = new System.Drawing.Size(931, 42);
            this.dataChartAutoScrollCheckBox.TabIndex = 0;
            this.dataChartAutoScrollCheckBox.Text = "自動スクロール";
            this.dataChartAutoScrollCheckBox.UseVisualStyleBackColor = true;
            // 
            // dataChart
            // 
            chartArea1.Name = "ChartArea1";
            this.dataChart.ChartAreas.Add(chartArea1);
            this.dataChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.dataChart.Legends.Add(legend1);
            this.dataChart.Location = new System.Drawing.Point(4, 54);
            this.dataChart.Margin = new System.Windows.Forms.Padding(4);
            this.dataChart.Name = "dataChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.dataChart.Series.Add(series1);
            this.dataChart.Size = new System.Drawing.Size(931, 578);
            this.dataChart.TabIndex = 0;
            this.dataChart.Text = "chart1";
            // 
            // latestDataGridView
            // 
            this.latestDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.latestDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.latestDataGridView.Location = new System.Drawing.Point(943, 54);
            this.latestDataGridView.Margin = new System.Windows.Forms.Padding(4);
            this.latestDataGridView.Name = "latestDataGridView";
            this.latestDataGridView.RowHeadersWidth = 51;
            this.latestDataGridView.RowTemplate.Height = 21;
            this.latestDataGridView.Size = new System.Drawing.Size(392, 578);
            this.latestDataGridView.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel5);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(4, 4);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(1347, 66);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "データ取得方法";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.dataFromServerRadioButton, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.dataFromSensorRadioButton, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(4, 19);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1339, 43);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // dataFromServerRadioButton
            // 
            this.dataFromServerRadioButton.AutoSize = true;
            this.dataFromServerRadioButton.Checked = true;
            this.dataFromServerRadioButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataFromServerRadioButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dataFromServerRadioButton.Location = new System.Drawing.Point(673, 4);
            this.dataFromServerRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.dataFromServerRadioButton.Name = "dataFromServerRadioButton";
            this.dataFromServerRadioButton.Size = new System.Drawing.Size(662, 42);
            this.dataFromServerRadioButton.TabIndex = 1;
            this.dataFromServerRadioButton.TabStop = true;
            this.dataFromServerRadioButton.Text = "サーバーから";
            this.dataFromServerRadioButton.UseVisualStyleBackColor = true;
            this.dataFromServerRadioButton.CheckedChanged += new System.EventHandler(this.DataFromServerRadioButton_CheckedChanged);
            // 
            // dataFromSensorRadioButton
            // 
            this.dataFromSensorRadioButton.AutoSize = true;
            this.dataFromSensorRadioButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataFromSensorRadioButton.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dataFromSensorRadioButton.Location = new System.Drawing.Point(4, 4);
            this.dataFromSensorRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.dataFromSensorRadioButton.Name = "dataFromSensorRadioButton";
            this.dataFromSensorRadioButton.Size = new System.Drawing.Size(661, 42);
            this.dataFromSensorRadioButton.TabIndex = 0;
            this.dataFromSensorRadioButton.Text = "センサーから";
            this.dataFromSensorRadioButton.UseVisualStyleBackColor = true;
            this.dataFromSensorRadioButton.CheckedChanged += new System.EventHandler(this.DataFromSensorRadioButton_CheckedChanged);
            // 
            // serialPort
            // 
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.SerialPort_DataReceived);
            // 
            // measurementTimer
            // 
            this.measurementTimer.Interval = 1000;
            this.measurementTimer.Tick += new System.EventHandler(this.MeasurementTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1355, 889);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "DemoApp";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.latestDataGridView)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label stateLlabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox portsComboBox;
        private System.Windows.Forms.Label label1;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button latestDataLongGetButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataVisualization.Charting.Chart dataChart;
        private System.Windows.Forms.Button memoryDataLongGetButton;
        private System.Windows.Forms.Button memoryIndexGetButton;
        private System.Windows.Forms.Timer measurementTimer;
        private System.Windows.Forms.CheckBox measurementCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.DataGridView latestDataGridView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.RadioButton dataFromServerRadioButton;
        private System.Windows.Forms.RadioButton dataFromSensorRadioButton;
        private System.Windows.Forms.CheckBox dataChartAutoScrollCheckBox;
    }
}

