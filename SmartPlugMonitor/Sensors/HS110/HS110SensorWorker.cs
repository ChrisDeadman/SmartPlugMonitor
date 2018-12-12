using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;

using SmartPlugMonitor.Config;
using SmartPlugMonitor.Workers;

namespace SmartPlugMonitor.Sensors.HS110
{
    public class HS110SensorWorker : ISensorWorker
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HS110SensorWorker));

        private const int RX_TIMEOUT_MS = 1000;

        private const string SENSOR_ID_ERR = "error";
        private const string SENSOR_ID_WATTAGE = "wattage";
        private const string SENSOR_ID_VOLTAGE = "voltage";
        private const string SENSOR_ID_CURRENT = "current";

        private readonly HS110Config config;

        private readonly Object lockObject = new Object();

        private TcpClient tcpClient;

        public HS110SensorWorker(HS110Config config)
        {
            this.config = config;
        }

        public string DisplayName { get { return $"HS110@{config.IpAddress}:{config.Port}"; } }

        public bool IsConfigComplete { get { return Uri.CheckHostName(config.IpAddress) != UriHostNameType.Unknown; } }

        public ICollection<SensorWorkerResult> Results {
            get {
                try {
                    if (!Connect()) {
                        return new[] { new SensorWorkerResult(DisplayName, SENSOR_ID_ERR, Globals.STATUS_NOT_CONNECTED) };
                    }

                    var data = HS110Sensor.ReadRealtimeData(SendReceive);

                    Disconnect();

                    if (data.Error != null) {
                        return new[] { new SensorWorkerResult(DisplayName, SENSOR_ID_ERR, data.Error) };
                    }

                    var results = new List<SensorWorkerResult>();

                    if (config.MonitorWattage) {
                        results.Add(new SensorWorkerResult(DisplayName, SENSOR_ID_WATTAGE, data.Wattage.ToString("N0", CultureInfo.InvariantCulture)));
                    }
                    if (config.MonitorVoltage) {
                        results.Add(new SensorWorkerResult(DisplayName, SENSOR_ID_VOLTAGE, data.Voltage.ToString("N0", CultureInfo.InvariantCulture)));
                    }
                    if (config.MonitorCurrent) {
                        results.Add(new SensorWorkerResult(DisplayName, SENSOR_ID_CURRENT, data.Current.ToString("N2", CultureInfo.InvariantCulture).TrimStart('0')));
                    }

                    return results;
                } catch (SocketException e) {
                    Log.Error(e.Message);
                    return new[] { new SensorWorkerResult(DisplayName, SENSOR_ID_ERR, Globals.STATUS_NOT_CONNECTED) };
                } catch (IOException e) {
                    Log.Error(e.Message);
                    return new[] { new SensorWorkerResult(DisplayName, SENSOR_ID_ERR, Globals.STATUS_NOT_CONNECTED) };
                } catch (ThreadAbortException) {
                    throw;
                } catch (Exception e) {
                    Log.Error(e.ToString());
                    return new[] { new SensorWorkerResult(DisplayName, SENSOR_ID_ERR, Globals.STATUS_NOT_AVAILABLE) };
                }
            }
        }

        public void Start()
        {
            // connection is not kept open
        }

        public void Stop()
        {
            // connection is not kept open
        }

        private bool Connect()
        {
            lock (lockObject) {
                if (tcpClient == null || !tcpClient.Connected) {
                    Log.Debug("Connecting to sensor...");
                    tcpClient = new TcpClient {
                        ReceiveTimeout = RX_TIMEOUT_MS
                    };
                    var connectionResult = tcpClient.BeginConnect(config.IpAddress, config.Port, null, null);
                    var connectionSuccessful = connectionResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1000));
                    if (connectionSuccessful) {
                        tcpClient.EndConnect(connectionResult);
                    } else {
                        Log.Error("Failed to establish connection, check IP Address and connectivity!");
                    }
                    return connectionSuccessful;
                }
            }
            return true;
        }

        private void Disconnect()
        {
            lock (lockObject) {
                if (tcpClient != null) {
                    Log.Debug("Disconnecting from sensor.");
                    tcpClient.Close();
                    tcpClient = null;
                }
            }
        }

        private byte[] SendReceive(byte[] message)
        {
            lock (lockObject) {
                if (!tcpClient.Connected) {
                    return new byte[0];
                }

                Log.Debug($"SEND -> {BitConverter.ToString(message).Replace("-", "")}");
                tcpClient.Client.Send(message);

                var response = new byte[2048];
                var bytesReceived = tcpClient.Client.Receive(response, 0, response.Length, SocketFlags.None);
                Array.Resize(ref response, bytesReceived);
                Log.Debug($"RECV <- {BitConverter.ToString(response).Replace("-", "")}");

                return response;
            }
        }
    }
}
