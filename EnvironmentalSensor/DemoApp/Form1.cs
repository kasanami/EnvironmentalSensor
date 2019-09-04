using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DemoApp
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 選択したポート
        /// </summary>
        string selectedPort = null;

        public Form1()
        {
            InitializeComponent();

            // すべてのシリアル・ポート名を取得する
            var ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                portsComboBox.Items.Add(port);
            }
            if (ports.Length > 0)
            {
                portsComboBox.SelectedIndex = 0;
                selectedPort = ports[0];
            }
            InitializeChartData();
            UpdateUI();
        }

        /// <summary>
        /// UI更新
        /// </summary>
        void UpdateUI()
        {
            if (serialPort.IsOpen)
            {
                stateLlabel.Text = "接続中";
                connectButton.Enabled = false;
                disconnectButton.Enabled = true;
                latestDataLongGetButton.Enabled = true;
                memoryDataLongGetButton.Enabled = false;
                memoryIndexGetButton.Enabled = false;
                measurementCheckBox.Enabled = true;
            }
            else
            {
                stateLlabel.Text = "非接続";
                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
                latestDataLongGetButton.Enabled = false;
                memoryDataLongGetButton.Enabled = false;
                memoryIndexGetButton.Enabled = false;
                measurementCheckBox.Enabled = false;
                measurementCheckBox.Checked = false;
            }
        }

        #region センサー関係
        /// <summary>
        /// 最新データを取得
        /// </summary>
        void LatestDataLongGet()
        {
            if (serialPort.IsOpen)
            {
                var payload = new LatestDataLongCommandPayload();
                var frame = new Frame(payload);
                var buffer = frame.ToBytes();
                Console.WriteLine(buffer.ToDebugString());
                serialPort.Write(buffer, 0, buffer.Length);
            }
            else
            {
                Console.WriteLine("閉じてる");
            }
        }

        #endregion センサー関係

        #region イベント

        /// <summary>
        /// フォームが閉じる前に発生
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort.Close();
        }

        /// <summary>
        /// ポート選択ドロップダウンリスト
        /// </summary>
        private void PortsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPort = (string)portsComboBox.SelectedItem;
        }

        /// <summary>
        /// 接続ボタン
        /// </summary>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen == false && selectedPort != null)
            {
                /// マニュアル:4.1. Communication specification
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.Handshake = Handshake.None;// フロー制御：None
                serialPort.PortName = selectedPort;
                serialPort.Open();
            }
            UpdateUI();
        }

        /// <summary>
        /// 切断ボタン
        /// </summary>
        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            serialPort.Close();
            UpdateUI();
        }

        /// <summary>
        /// データが受信された
        /// </summary>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[1024];
            var readSize = serialPort.Read(buffer, 0, buffer.Length);
            Console.WriteLine("DataReceived");
            Console.WriteLine($"readSize={readSize}");
            if (readSize > 0)
            {
                Console.WriteLine($"buffer={buffer.ToDebugString()}");
                try
                {
                    var frame = new Frame(buffer, 0, readSize);

                    if (frame.Payload is ErrorResponsePayload)
                    {
                        var payload = frame.Payload as ErrorResponsePayload;
                    }
                    else if (frame.Payload is MemoryDataLongResponsePayload)
                    {
                        var payload = frame.Payload as MemoryDataLongResponsePayload;
                    }
                    else if (frame.Payload is LatestDataLongResponsePayload)
                    {
                        var payload = frame.Payload as LatestDataLongResponsePayload;
                        AddChartData(payload);
                    }
                    Console.WriteLine(frame.Payload.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 最新データを取得ボタン
        /// </summary>
        private void LatestDataLongGetButton_Click(object sender, EventArgs e)
        {
            LatestDataLongGet();
        }

        /// <summary>
        /// 保存データを取得ボタン
        /// </summary>
        private void MemoryDataLongGetButton_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                var payload = new MemoryDataLongCommandPayload();
                payload.StartMemoryIndex = 0;
                payload.EndMemoryIndex = 0;
                var frame = new Frame(payload);
                var buffer = frame.ToBytes();
                Console.WriteLine(buffer.ToDebugString());
                serialPort.Write(buffer, 0, buffer.Length);
            }
            else
            {
                Console.WriteLine("閉じてる");
            }
        }

        private void MemoryIndexGetButton_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                var payload = new CommandPayload();
                payload.Command = FrameCommand.Read;
                payload.Address = FrameAddress.LatestMemoryInformation;
                var frame = new Frame(payload);
                var buffer = frame.ToBytes();
                serialPort.Write(buffer, 0, buffer.Length);
            }
            else
            {
                Console.WriteLine("閉じてる");
            }
        }
        /// <summary>
        /// 継続測定のタイマー
        /// </summary>
        private void MeasurementTimer_Tick(object sender, EventArgs e)
        {
            LatestDataLongGet();
        }
        /// <summary>
        /// 継続測定のチェックボックス
        /// </summary>
        private void MeasurementCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (measurementCheckBox.Checked)
            {
                measurementTimer.Start();
            }
            else
            {
                measurementTimer.Stop();
            }
        }
        #endregion イベント

        #region Chart
        /// <summary>
        /// グラフの基準日時
        /// </summary>
        DateTime StandardDateTime = DateTime.Now;
        Series temperatureSeries;
        Series relativeHumiditySeries;
        Series ambientLightSeries;
        void InitializeChartData()
        {
            temperatureSeries = new Series("温度[℃]");
            temperatureSeries.ChartType = SeriesChartType.Line;
            relativeHumiditySeries = new Series("相対湿度[％]");
            relativeHumiditySeries.ChartType = SeriesChartType.Line;
            ambientLightSeries = new Series("環境光[ルクス]");
            ambientLightSeries.ChartType = SeriesChartType.Line;
            dataChart.Series.Clear();
            dataChart.Series.Add(temperatureSeries);
            dataChart.Series.Add(relativeHumiditySeries);
            dataChart.Series.Add(ambientLightSeries);
        }
        void AddChartData(LatestDataLongResponsePayload payload)
        {
            Console.WriteLine(nameof(AddChartData));
            if (dataChart.InvokeRequired)
            {
                var _delegate = new AddChartDataDelegate(AddChartData);
                Invoke(_delegate, new object[] { payload });
            }
            else
            {
                var now = DateTime.Now - StandardDateTime;
                var x = now.TotalSeconds;
                temperatureSeries.Points.AddXY(x, payload.Temperature * 0.01);
                relativeHumiditySeries.Points.AddXY(x, payload.RelativeHumidity * 0.01);
                ambientLightSeries.Points.AddXY(x, payload.AmbientLight);
            }
        }
        delegate void AddChartDataDelegate(LatestDataLongResponsePayload payload);
        #endregion Chart
    }
}
