using EnvironmentalSensor.Ipc;
using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        Client IpcClient = new Client();

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
            InitializeLatestListData();
            UpdateUI();
        }

        /// <summary>
        /// UI更新
        /// </summary>
        void UpdateUI()
        {
            if (dataFromSensorRadioButton.Checked)
            {
                groupBox1.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = false;
            }
            if (serialPort.IsOpen)
            {
                stateLlabel.Text = "接続中";
                groupBox4.Enabled = false;// 接続中は変更不可
                connectButton.Enabled = false;
                disconnectButton.Enabled = true;
                memoryDataLongGetButton.Enabled = false;
                memoryIndexGetButton.Enabled = false;
                measurementCheckBox.Enabled = true;
            }
            else
            {
                stateLlabel.Text = "非接続";
                groupBox4.Enabled = true;
                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
                memoryDataLongGetButton.Enabled = false;
                memoryIndexGetButton.Enabled = false;
                measurementCheckBox.Enabled = false;
                measurementCheckBox.Checked = false;
            }
        }

        #region センサー関係
        long lastTimeStamp = long.MinValue;
        /// <summary>
        /// 最新データを取得
        /// </summary>
        void LatestDataLongGet()
        {
            if (dataFromSensorRadioButton.Checked)
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
            else
            {
                // TODO:System.Runtime.Remoting.RemotingException対応
                var remoteObject = IpcClient.GetRemoteObject();
                Console.WriteLine($"{nameof(remoteObject.TimeStamp)}={remoteObject.TimeStamp.ToString()}");
                Console.WriteLine($"{nameof(remoteObject.Payloads)}={remoteObject.Payloads.Count}");
                Console.WriteLine($"{nameof(remoteObject.UpdateCompleted)}={remoteObject.UpdateCompleted}");
                // 最後の取得より以降なら描画更新
                if (lastTimeStamp < remoteObject.TimeStampBinary)
                {
                    // 最後の取得時刻から最新時刻までを、グラフに表示
                    foreach (var item in remoteObject.Payloads)
                    {
                        if (item.Key > lastTimeStamp)
                        {
                            var payload = item.Value;
                            if (payload is LatestDataLongResponsePayload)
                            {
                                AddChartData(payload as LatestDataLongResponsePayload);
                            }
                        }
                    }
                    // 表は最新時刻のみ表示
                    {
                        var payload = remoteObject.Payloads[remoteObject.TimeStampBinary];
                        if (payload is LatestDataLongResponsePayload)
                        {
                            SetLatestListData(payload as LatestDataLongResponsePayload);
                        }
                    }
                    lastTimeStamp = remoteObject.TimeStampBinary;
                }
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

        private void DataFromSensorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void DataFromServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
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
            // TODO:System.UnauthorizedAccessExceptionの対処
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
                        SetLatestListData(payload);
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

        #region データ表示
        enum DataId : int
        {
            SequenceNumber,
            Temperature,
            RelativeHumidity,
            AmbientLight,
            BarometricPressure,
            SoundNoise,
            eTVOC,
            eCO2,
            DiscomfortIndex,
            HeatStroke,
            VibrationInformation,
            SIValue,
            PGA,
            SeismicIntensity,
        }
        Dictionary<DataId, string> DataNames = new Dictionary<DataId, string>()
        {
            {DataId.SequenceNumber, "シーケンス番号[x0.1]"},
            {DataId.Temperature, "温度[℃]"},
            {DataId.RelativeHumidity, "相対湿度[％]"},
            {DataId.AmbientLight, "環境光[ルクス]"},
            {DataId.BarometricPressure, "気圧[hPa]"},
            {DataId.SoundNoise, "雑音[dBx100]"},
            {DataId.eTVOC, "総揮発性有機化学物量相当値[ppd x0.01]"},
            {DataId.eCO2, "二酸化炭素換算の数値[ppm x0.1]"},
            {DataId.DiscomfortIndex, "不快指数"},
            {DataId.HeatStroke, "熱中症警戒度[℃]"},
            {DataId.VibrationInformation, "振動情報[x0.1]"},
            {DataId.SIValue, "スペクトル強度[kine]"},
            {DataId.PGA, "PGA"},
            {DataId.SeismicIntensity, "SeismicIntensity"},
        };
        Dictionary<DataId, Color> DataColors = new Dictionary<DataId, Color>()
        {
            {DataId.SequenceNumber, Color.Black},
            {DataId.Temperature, Color.Red},
            {DataId.RelativeHumidity, Color.OrangeRed},
            {DataId.AmbientLight, Color.Yellow},
            {DataId.BarometricPressure, Color.GreenYellow},
            {DataId.SoundNoise, Color.Green},
            {DataId.eTVOC, Color.Cyan},
            {DataId.eCO2, Color.DarkCyan},
            {DataId.DiscomfortIndex, Color.DarkRed},
            {DataId.HeatStroke, Color.PaleVioletRed},
            {DataId.VibrationInformation, Color.Blue},
            {DataId.SIValue, Color.Blue},
            {DataId.PGA, Color.Blue},
            {DataId.SeismicIntensity, Color.Blue},
        };
        /// <summary>
        /// グラフの基準日時
        /// </summary>
        DateTime StandardDateTime = DateTime.Now;
        Dictionary<DataId, Series> dataSeries = new Dictionary<DataId, Series>();
        const int ChartPointsMaxCount = 20;
        void InitializeChartData()
        {
            dataChart.Series.Clear();
            foreach (var dataId in (DataId[])Enum.GetValues(typeof(DataId)))
            {
                var dataName = DataNames[dataId];
                var series = new Series(dataName);
                series.ChartType = SeriesChartType.Line;
                if (dataId == DataId.SequenceNumber)
                {
                    series.BorderWidth = 1;
                }
                else
                {
                    series.BorderWidth = 2;
                }
                series.Color = DataColors[dataId];
                dataChart.Series.Add(series);

                dataSeries.Add(dataId, series);
            }
            dataChart.Font = new Font(dataChart.Font.Name, 18);
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
                var x = Math.Round(now.TotalSeconds);
                dataChart.ChartAreas[0].AxisX.ScaleView.MinSize = 100;
                foreach (var dataId in (DataId[])Enum.GetValues(typeof(DataId)))
                {
                    var points = dataSeries[dataId].Points;
                    points.AddXY(x, GetDataFromId(payload, dataId));
                }
            }
        }
        delegate void AddChartDataDelegate(LatestDataLongResponsePayload payload);

        static double GetDataFromId(LatestDataLongResponsePayload payload, DataId dataId)
        {
            if (dataId == DataId.SequenceNumber) return payload.SequenceNumber * 0.1;
            if (dataId == DataId.Temperature) return payload.Temperature * payload.TemperatureUnit;
            if (dataId == DataId.RelativeHumidity) return payload.RelativeHumidity * payload.RelativeHumidityUnit;
            if (dataId == DataId.AmbientLight) return payload.AmbientLight;
            if (dataId == DataId.BarometricPressure) return payload.BarometricPressure * payload.BarometricPressureUnit;
            if (dataId == DataId.SoundNoise) return payload.SoundNoise * payload.SoundNoiseUnit * 100;
            if (dataId == DataId.eTVOC) return payload.eTVOC * 0.01;
            if (dataId == DataId.eCO2) return payload.eCO2 * 0.1;
            if (dataId == DataId.DiscomfortIndex) return payload.DiscomfortIndex * payload.DiscomfortIndexUnit;
            if (dataId == DataId.HeatStroke) return payload.HeatStroke * payload.HeatStrokeUnit;
            if (dataId == DataId.VibrationInformation) return payload.VibrationInformation * 0.1;
            if (dataId == DataId.SIValue) return payload.SIValue * payload.SIValueUnit;
            if (dataId == DataId.PGA) return payload.PGA * payload.PGAUnit;
            if (dataId == DataId.SeismicIntensity) return payload.SeismicIntensity * payload.SeismicIntensityUnit;
            throw new NotSupportedException($"{nameof(dataId)}={dataId}");
        }

        void InitializeLatestListData()
        {
            latestDataGridView.Columns.Clear();
            latestDataGridView.Columns.Add("Name", "名前");
            latestDataGridView.Columns.Add("Value", "値");
            foreach (var dataId in (DataId[])Enum.GetValues(typeof(DataId)))
            {
                latestDataGridView.Rows.Add(DataNames[dataId], "");
            }
        }
        void SetLatestListData(LatestDataLongResponsePayload payload)
        {
            foreach (var dataId in (DataId[])Enum.GetValues(typeof(DataId)))
            {
                latestDataGridView[1, (int)dataId].Value = GetDataFromId(payload, dataId);
            }
        }
        #endregion データ表示
    }
}
