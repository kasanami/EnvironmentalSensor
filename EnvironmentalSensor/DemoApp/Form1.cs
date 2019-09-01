﻿using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace DemoApp
{
    public partial class Form1 : Form
    {

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
            UpdateUI();
        }

        void UpdateUI()
        {
            if (serialPort.IsOpen)
            {
                stateLlabel.Text = "接続中";
                connectButton.Enabled = false;
                disconnectButton.Enabled = true;
                latestDataLongGetButton.Enabled = true;
            }
            else
            {
                stateLlabel.Text = "非接続";
                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
                latestDataLongGetButton.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort.Close();
        }

        private void PortsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPort = (string)portsComboBox.SelectedItem;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen == false && selectedPort != null)
            {
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

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            serialPort.Close();
            UpdateUI();
        }

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
                    }
                    Console.WriteLine(frame.Payload.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void LatestDataLongGetButton_Click(object sender, EventArgs e)
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
    }
}
