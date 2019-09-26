using EnvironmentalSensor.Ipc;
using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static EnvironmentalSensor.USB.Utility;

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
            }
            else
            {
                stateLlabel.Text = "非接続";
                groupBox4.Enabled = true;
                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
                memoryDataLongGetButton.Enabled = false;
                memoryIndexGetButton.Enabled = false;
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
                IntermediateDatas.Clear();
                // TODO:System.Runtime.Remoting.RemotingException対応
                try
                {
                    var remoteObject = IpcClient.GetRemoteObject();
                    IpcClient.Mutex.WaitOne();
                    Console.WriteLine($"{nameof(remoteObject.TimeStamp)}={remoteObject.TimeStamp.ToString()}");
                    Console.WriteLine($"{nameof(remoteObject.Payloads)}={remoteObject.Payloads.Count}");
                    Console.WriteLine($"{nameof(remoteObject.UpdateCompleted)}={remoteObject.UpdateCompleted}");
                    if (remoteObject.UpdateCompleted == false)
                    {
                        throw new Exception($"排他制御が正常にできていない {nameof(remoteObject.UpdateCompleted)}==false");
                    }
                    // 最後の取得より以降なら描画更新
                    if (lastTimeStamp < remoteObject.TimeStampBinary &&
                        remoteObject.UpdateCompleted)
                    {
                        // 最後の取得時刻から最新時刻までを、グラフに表示
                        // 時刻でソートする
                        foreach (var item in remoteObject.Payloads
                            .Where(item => item.Key > lastTimeStamp)
                            .OrderBy(item => item.Key))
                        {
                            var dateTime = DateTime.FromBinary(item.Key);
                            var payload = item.Value;
                            if (payload is LatestDataLongResponsePayload)
                            {
                                IntermediateDatas.Add(dateTime, new IntermediateData(payload as LatestDataLongResponsePayload));
                            }
                        }
                        lastTimeStamp = remoteObject.TimeStampBinary;
                    }
                }
                catch (Exception ex)
                {
                    measurementCheckBox.Checked = false;// 継続取得を中断
                    MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK);
                }
                finally
                {
                    IpcClient.Mutex.ReleaseMutex();
                }
                // UIに反映
                {
                    foreach (var item in IntermediateDatas)
                    {
                        AddChartData(item.Key, item.Value);
                    }
                    // 表は最新時刻のみ表示
                    {
                        var latestTimeStamp = IntermediateDatas.Keys.Max();
                        SetLatestListData(IntermediateDatas[latestTimeStamp]);
                    }
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
            measurementCheckBox.Checked = false;
        }

        private void DataFromServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
            measurementCheckBox.Checked = false;
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
                SettingSerialPort(serialPort);
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
            measurementCheckBox.Checked = false;
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
        static readonly Dictionary<DataId, double> DataScales = new Dictionary<DataId, double>()
        {
            {DataId.SequenceNumber      , 0.1},
            {DataId.Temperature         , 1},
            {DataId.RelativeHumidity    , 1},
            {DataId.AmbientLight        , 1},
            {DataId.BarometricPressure  , 0.1},
            {DataId.SoundNoise          , 1},
            {DataId.eTVOC               , 0.03},
            {DataId.eCO2                , 0.03},
            {DataId.DiscomfortIndex     , 1},
            {DataId.HeatStroke          , 1},
            {DataId.VibrationInformation, 1},
            {DataId.SIValue             , 0.6},
            {DataId.PGA                 , 0.6},
            {DataId.SeismicIntensity    , 1},
        };
        static readonly Dictionary<DataId, string> DataNames = new Dictionary<DataId, string>()
        {
            {DataId.SequenceNumber, "シーケンス番号"},
            {DataId.Temperature, "温度[℃]"},
            {DataId.RelativeHumidity, "相対湿度[％]"},
            {DataId.AmbientLight, "環境光[ルクス]"},
            {DataId.BarometricPressure, "気圧[hPa]"},
            {DataId.SoundNoise, "雑音[dB]"},
            {DataId.eTVOC, "総揮発性有機化学物量相当値[ppd]"},
            {DataId.eCO2, "二酸化炭素換算の数値[ppm]"},
            {DataId.DiscomfortIndex, "不快指数"},
            {DataId.HeatStroke, "熱中症警戒度[℃]"},
            {DataId.VibrationInformation, "振動情報"},
            {DataId.SIValue, "スペクトル強度[kine]"},
            {DataId.PGA, "PGA"},
            {DataId.SeismicIntensity, "SeismicIntensity"},
        };
        static readonly Dictionary<DataId, Color> DataColors = new Dictionary<DataId, Color>()
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
        /// 表示用中間データ
        /// </summary>
        struct IntermediateData
        {
            public Dictionary<DataId, double> Values;
            public IntermediateData(LatestDataLongResponsePayload payload)
            {
                Values = new Dictionary<DataId, double>();
                foreach (var dataId in (DataId[])Enum.GetValues(typeof(DataId)))
                {
                    var value = GetDataFromId(payload, dataId);
                    Values.Add(dataId, value);
                }
            }
        }
        Dictionary<DateTime, IntermediateData> IntermediateDatas = new Dictionary<DateTime, IntermediateData>();
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
                var dataName = DataNames[dataId] + "x" + DataScales[dataId].ToString();
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
            AddChartData(DateTime.Now, payload);
        }
        void AddChartData(DateTime dateTime, LatestDataLongResponsePayload payload)
        {
            DebugWriteLine($"{nameof(AddChartData)} {dateTime} {payload.SequenceNumber}");
            if (dataChart.InvokeRequired)
            {
                var _delegate = new AddChartDataDelegate(AddChartData);
                Invoke(_delegate, new object[] { dateTime, payload });
            }
            else
            {
                var now = dateTime - StandardDateTime;
                var x = Math.Round(now.TotalSeconds);
                dataChart.ChartAreas[0].AxisX.ScaleView.MinSize = 100;
                foreach (var dataId in (DataId[])Enum.GetValues(typeof(DataId)))
                {
                    var points = dataSeries[dataId].Points;
                    var scale = DataScales[dataId];
                    points.AddXY(x, GetDataFromId(payload, dataId) * scale);
                }
            }
        }
        void AddChartData(DateTime dateTime, IntermediateData data)
        {
            if (dataChart.InvokeRequired)
            {
                var _delegate = new AddChartDataDelegate2(AddChartData);
                Invoke(_delegate, new object[] { dateTime, data });
            }
            else
            {
                var now = dateTime - StandardDateTime;
                var x = Math.Round(now.TotalSeconds);
                dataChart.ChartAreas[0].AxisX.ScaleView.MinSize = 100;
                foreach (var item in data.Values)
                {
                    var dataId = item.Key;
                    var points = dataSeries[dataId].Points;
                    var scale = DataScales[dataId];
                    points.AddXY(x, item.Value * scale);
                }
            }
        }
        delegate void AddChartDataDelegate(DateTime dateTime, LatestDataLongResponsePayload payload);
        delegate void AddChartDataDelegate2(DateTime dateTime, IntermediateData payload);

        static double GetDataFromId(LatestDataLongResponsePayload payload, DataId dataId)
        {
            if (dataId == DataId.SequenceNumber) return payload.SequenceNumber;
            if (dataId == DataId.Temperature) return payload.Temperature.Value;
            if (dataId == DataId.RelativeHumidity) return payload.RelativeHumidity.Value;
            if (dataId == DataId.AmbientLight) return payload.AmbientLight.Value;
            if (dataId == DataId.BarometricPressure) return payload.BarometricPressure.Value;
            if (dataId == DataId.SoundNoise) return payload.SoundNoise.Value;
            if (dataId == DataId.eTVOC) return payload.eTVOC.Value;
            if (dataId == DataId.eCO2) return payload.eCO2.Value;
            if (dataId == DataId.DiscomfortIndex) return payload.DiscomfortIndex.Value;
            if (dataId == DataId.HeatStroke) return payload.HeatStroke.Value;
            if (dataId == DataId.VibrationInformation) return payload.VibrationInformation;
            if (dataId == DataId.SIValue) return payload.SIValue.Value;
            if (dataId == DataId.PGA) return payload.PGA.Value;
            if (dataId == DataId.SeismicIntensity) return payload.SeismicIntensity.Value;
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
        void SetLatestListData(IntermediateData data)
        {
            foreach (var item in data.Values)
            {
                var dataId = item.Key;
                latestDataGridView[1, (int)dataId].Value = item.Value;
            }
        }
        #endregion データ表示
        static void DebugWriteLine(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
