using EnvironmentalSensor.Ipc;
using EnvironmentalSensor.Usb;
using EnvironmentalSensor.Usb.Payloads;
using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static EnvironmentalSensor.Usb.Utility;

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
        /// <summary>
        /// 最後に受信した日時(サーバーとの同期用なのでUTCに合わせる)
        /// </summary>
        long lastTimeStampTicks = long.MinValue;
        /// <summary>
        /// 最新データを取得
        /// </summary>
        void LatestDataLongGet()
        {
            // センサーから値を取得
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
                // サーバーから値を取得
                LatestDataLongGet_FromIpcServer();
            }
        }
        /// <summary>
        /// LatestDataLongGetの処理の
        /// </summary>
        private void LatestDataLongGet_FromIpcServer()
        {
            // 受信データの一時保管
            var receivedData = new Dictionary<long, byte[]>();
            // TODO:System.Runtime.Remoting.RemotingException対応
            try
            {
                var remoteObject = IpcClient.GetRemoteObject();
                // サーバー等のRemoteObjectの更新を待つ
                IpcClient.Mutex.WaitOne();
                Console.WriteLine($"{nameof(remoteObject.TimeStamp)}={remoteObject.TimeStamp.ToString()}");
                Console.WriteLine($"{nameof(remoteObject.UpdateCompleted)}={remoteObject.UpdateCompleted}");
#if DEBUG
                // わざと遅延させてデータ更新中にアクセスしていないかチェックしやすくする
                Thread.Sleep(100);
#endif
                if (remoteObject.UpdateCompleted == false)
                {
                    throw new Exception($"排他制御が正常にできていない {nameof(remoteObject.UpdateCompleted)}==false");
                }
                // 前回の取得日時より後なら取得
                if (lastTimeStampTicks < remoteObject.TimeStampTicks &&
                    remoteObject.UpdateCompleted)
                {
                    foreach (var key in remoteObject.ReceivedDataHistory.Keys)
                    {
                        var data = remoteObject.GetReceivedData(key);
                        receivedData.Add(key, data);
                    }
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
                // 今回追加されたデータ
                var intermediateDataHistory = new Dictionary<DateTime, IntermediateData>();
                // 最後の取得時刻から最新時刻までを、中間データに追加
                // 念の為、時刻でソートもする
                foreach (var item in receivedData
                    .Where(item => item.Key > lastTimeStampTicks)
                    .OrderBy(item => item.Key))
                {
                    // item.Keyは、UTCなので、表示用にLocalTimeに変換
                    var timeStamp = new DateTime(item.Key, DateTimeKind.Utc).ToLocalTime();
#if DEBUG
                    //Console.WriteLine($"{nameof(timeStamp)}={timeStamp}");
#endif
                    var frame = new Frame(item.Value);
                    var payload = frame.Payload;
                    if (payload is LatestDataLongResponsePayload)
                    {
                        var intermediateData = new IntermediateData(payload as LatestDataLongResponsePayload);
                        intermediateDataHistory.Add(timeStamp, intermediateData);
                        IntermediateDataHistory.Add(timeStamp, intermediateData);
                    }
                }
                // 関連データ更新
                UpdateSmooth();
                // 表は最新時刻のみ表示
                if (IntermediateDataHistory.Keys.Count > 0)
                {
                    var latestTimeStamp = IntermediateDataHistory.Keys.Max();
                    SetLatestListData(IntermediateDataHistory[latestTimeStamp]);
                    // lastTimeStampTicksはUTCなので変換
                    lastTimeStampTicks = latestTimeStamp.ToUniversalTime().Ticks;
                }
                // グラフに反映
                AddChartData(intermediateDataHistory);
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
                        //AddChartData(payload);
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
        static readonly DataId[] DataIds = (DataId[])Enum.GetValues(typeof(DataId));
        /// <summary>
        /// データ表示用係数
        /// </summary>
        static readonly Dictionary<DataId, double> DataScales = new Dictionary<DataId, double>()
        {
            {DataId.SequenceNumber      , 0.1},
            {DataId.Temperature         , 1},
            {DataId.RelativeHumidity    , 1},
            {DataId.AmbientLight        , 1},
            {DataId.BarometricPressure  , 10},
            {DataId.SoundNoise          , 1},
            {DataId.eTVOC               , 0.1},
            {DataId.eCO2                , 0.1},
            {DataId.DiscomfortIndex     , 1},
            {DataId.HeatStroke          , 1},
            {DataId.VibrationInformation, 1},
            {DataId.SIValue             , 0.6},
            {DataId.PGA                 , 0.6},
            {DataId.SeismicIntensity    , 1},
        };
        /// <summary>
        /// データ表示用切片
        /// </summary>
        static readonly Dictionary<DataId, double> DataBiases = new Dictionary<DataId, double>()
        {
            {DataId.SequenceNumber      , 0},
            {DataId.Temperature         , 0},
            {DataId.RelativeHumidity    , 0},
            {DataId.AmbientLight        , 0},
            {DataId.BarometricPressure  , -10100},
            {DataId.SoundNoise          , 0},
            {DataId.eTVOC               , 0},
            {DataId.eCO2                , 0},
            {DataId.DiscomfortIndex     , 0},
            {DataId.HeatStroke          , 0},
            {DataId.VibrationInformation, 0},
            {DataId.SIValue             , 0},
            {DataId.PGA                 , 0},
            {DataId.SeismicIntensity    , 0},
        };
        static readonly Dictionary<DataId, string> DataNames = new Dictionary<DataId, string>()
        {
            {DataId.SequenceNumber      , "シーケンス番号"},
            {DataId.Temperature         , "温度[℃]"},
            {DataId.RelativeHumidity    , "相対湿度[％]"},
            {DataId.AmbientLight        , "環境光[ルクス]"},
            {DataId.BarometricPressure  , "気圧[hPa]"},
            {DataId.SoundNoise          , "雑音[dB]"},
            {DataId.eTVOC               , "総揮発性有機化学物量相当値[ppd]"},
            {DataId.eCO2                , "二酸化炭素換算の数値[ppm]"},
            {DataId.DiscomfortIndex     , "不快指数"},
            {DataId.HeatStroke          , "熱中症警戒度[℃]"},
            {DataId.VibrationInformation, "振動情報"},
            {DataId.SIValue             , "スペクトル強度[kine]"},
            {DataId.PGA                 , "PGA"},
            {DataId.SeismicIntensity    , "SeismicIntensity"},
        };
        static readonly Dictionary<DataId, Color> DataColors = new Dictionary<DataId, Color>()
        {
            {DataId.SequenceNumber      , Color.Black},
            {DataId.Temperature         , Color.Red},
            {DataId.RelativeHumidity    , Color.Blue},
            {DataId.AmbientLight        , Color.Yellow},
            {DataId.BarometricPressure  , Color.GreenYellow},
            {DataId.SoundNoise          , Color.FromArgb(128, Color.Green) },
            {DataId.eTVOC               , Color.Cyan},
            {DataId.eCO2                , Color.DarkCyan},
            {DataId.DiscomfortIndex     , Color.DarkRed},
            {DataId.HeatStroke          , Color.PaleVioletRed},
            {DataId.VibrationInformation, Color.Blue},
            {DataId.SIValue             , Color.Blue},
            {DataId.PGA                 , Color.Blue},
            {DataId.SeismicIntensity    , Color.Blue},
        };
        /// <summary>
        /// 表示用中間データ
        /// </summary>
        struct IntermediateData
        {
            public Dictionary<DataId, double> Values;
            public IntermediateData(double value)
            {
                Values = new Dictionary<DataId, double>();
                foreach (var dataId in DataIds)
                {
                    Values.Add(dataId, value);
                }
            }
            public IntermediateData(LatestDataLongResponsePayload payload)
            {
                Values = new Dictionary<DataId, double>();
                foreach (var dataId in DataIds)
                {
                    var value = GetDataFromId(payload, dataId);
                    Values.Add(dataId, value);
                }
            }
        }
        Dictionary<DateTime, IntermediateData> IntermediateDataHistory = new Dictionary<DateTime, IntermediateData>();
        IntermediateData SmoothIntermediateData = new IntermediateData(0);
        /// <summary>
        /// SmoothIntermediateDataをIntermediateDataHistoryから作成
        /// ※最新データから作成
        /// </summary>
        void UpdateSmooth()
        {
            const int Count = 10;
            var totalIntermediateData = new IntermediateData(0);
            foreach (var dataId in DataIds)
            {
                totalIntermediateData.Values[dataId] = 0;
            }
            foreach (var item in IntermediateDataHistory.Reverse().Take(Count))
            {
                foreach (var dataId in DataIds)
                {
                    totalIntermediateData.Values[dataId] += item.Value.Values[dataId];
                }
            }
            foreach (var dataId in DataIds)
            {
                SmoothIntermediateData.Values[dataId] = totalIntermediateData.Values[dataId] / Count;
            }
        }

        Dictionary<DataId, Series> dataSeries = new Dictionary<DataId, Series>();
        Dictionary<DataId, Series> smoothDataSeries = new Dictionary<DataId, Series>();
        const int ChartPointsMaxCount = 20;
        /// <summary>
        /// グラフのエリア
        /// </summary>
        ChartArea dataChartArea;
        /// <summary>
        /// 一画面に表示する時間
        /// </summary>
        static double DataChartArea_ViewSize = TimeSpan.FromMinutes(60).TotalDays;// 60分
        /// <summary>
        /// 日付型をグラフのX軸の値に変更
        /// </summary>
        static double DateTimeToX(DateTime dateTime)
        {
            return dateTime.ToOADate();
            //return dateTime.Ticks / 10000000;
        }
        /// <summary>
        /// グラフ初期化
        /// </summary>
        void InitializeChartData()
        {
            dataChart.Series.Clear();
            foreach (var dataId in DataIds)
            {
                var dataName = DataNames[dataId] + "x" + DataScales[dataId].ToString() + DataBiases[dataId].ToString("+0;-#;");
                var series = new Series(dataName);
                series.ChartType = SeriesChartType.Line;
                if (dataId == DataId.SequenceNumber || dataId == DataId.SoundNoise)
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

            {
                var dataId = DataId.SoundNoise;
                var dataName = DataNames[dataId] + "(平滑)" + "x" + DataScales[dataId].ToString();
                var series = new Series(dataName);
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 2;
                series.Color = Color.DarkGreen;
                dataChart.Series.Add(series);

                smoothDataSeries.Add(dataId, series);
            }

            dataChart.Font = new Font(dataChart.Font.Name, 18);
            // グラフにスクロールバー表示
            {
                var viewStart = 0.0;
                var viewSize = DataChartArea_ViewSize;
                var series = dataSeries[0];
                dataChartArea = dataChart.ChartAreas[series.ChartArea];
                dataChartArea.CursorX.AutoScroll = true;
                dataChartArea.AxisX.Minimum = DateTimeToX(DateTime.Now);
                dataChartArea.AxisX.Maximum = DateTimeToX(DateTime.Now);
                dataChartArea.AxisX.ScaleView.Zoom(viewStart, viewStart + viewSize);
                dataChartArea.AxisX.ScaleView.SmallScrollSize = viewSize / 100;
                dataChartArea.AxisX.ScaleView.SmallScrollMinSize = dataChartArea.AxisX.ScaleView.SmallScrollSize;
                dataChartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                // X軸を時間で表示する設定
                dataChartArea.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                dataChartArea.AxisX.Interval = 1;
                dataChartArea.AxisX.LabelStyle.IntervalType = dataChartArea.AxisX.IntervalType;
                dataChartArea.AxisX.LabelStyle.Interval = 5;
                dataChartArea.AxisX.LabelStyle.Format = "HH:mm";
                // Y軸の表示設定
                dataChartArea.AxisY.Minimum = 0;
                dataChartArea.AxisY.Maximum = 100;
                dataChartArea.AxisY.Interval = 10;
            }
        }

        void AddChartData(Dictionary<DateTime, IntermediateData> intermediateDataHistory)
        {
            if (dataChart.InvokeRequired)
            {
                var _delegate = new AddChartDataDelegate(AddChartData);
                Invoke(_delegate, new object[] { intermediateDataHistory });
            }
            else
            {
                double viewEnd = 0;
                foreach (var item2 in intermediateDataHistory)
                {
                    var dateTime = item2.Key;
                    var x = DateTimeToX(dateTime);
                    viewEnd = x;
                    // グラフの表示範囲の指定
                    if (dataChartArea.AxisX.Minimum > x)
                    {
                        dataChartArea.AxisX.Minimum = x;
                    }
                    if (dataChartArea.AxisX.Minimum < x)
                    {
                        dataChartArea.AxisX.Maximum = x;
                    }
                    var intermediateData = item2.Value;
                    foreach (var item in intermediateData.Values)
                    {
                        var dataId = item.Key;
                        var points = dataSeries[dataId].Points;
                        //if (dataId == DataId.SIValue ||
                        //    dataId == DataId.PGA ||
                        //    dataId == DataId.SeismicIntensity)
                        //{
                        //    // 表示しない
                        //    continue;
                        //}
                        var scale = DataScales[dataId];
                        var bias = DataBiases[dataId];
                        points.AddXY(x, item.Value * scale + bias);
                    }
                    // 平滑化SoundNoiseの追加
                    {
                        var dataId = DataId.SoundNoise;
                        var points = smoothDataSeries[dataId].Points;
                        var scale = DataScales[dataId];
                        var bias = DataBiases[dataId];
                        points.AddXY(x, SmoothIntermediateData.Values[dataId] * scale + bias);
                    }
                }
                // スクロール位置更新
                if (dataChartAutoScrollCheckBox.Checked && viewEnd > 0)
                {
                    dataChartArea.AxisX.ScaleView.Zoom(viewEnd - DataChartArea_ViewSize, viewEnd);
                }
            }
        }
        delegate void AddChartDataDelegate(Dictionary<DateTime, IntermediateData> intermediateDataHistory);

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

        enum LatestDataGridView_Columns
        {
            Name,
            Value,
            Visible,
        }

        void InitializeLatestListData()
        {
            latestDataGridView.Columns.Clear();
            latestDataGridView.Columns.Add(LatestDataGridView_Columns.Name.ToString(), "名前");
            latestDataGridView.Columns.Add(LatestDataGridView_Columns.Value.ToString(), "値");
            {
                var column = new DataGridViewCheckBoxColumn();
                column.Name = LatestDataGridView_Columns.Visible.ToString();
                column.HeaderText = "表示";
                column.Width = 50;
                latestDataGridView.Columns.Add(column);
            }
            foreach (var dataId in DataIds)
            {
                latestDataGridView.Rows.Add(DataNames[dataId], "");
            }
        }
        void SetLatestListData(LatestDataLongResponsePayload payload)
        {
            int column = (int)LatestDataGridView_Columns.Value;
            foreach (var dataId in DataIds)
            {
                latestDataGridView[column, (int)dataId].Value = GetDataFromId(payload, dataId);
            }
        }
        void SetLatestListData(IntermediateData data)
        {
            int column = (int)LatestDataGridView_Columns.Value;
            foreach (var item in data.Values)
            {
                var dataId = item.Key;
                latestDataGridView[column, (int)dataId].Value = item.Value;
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
