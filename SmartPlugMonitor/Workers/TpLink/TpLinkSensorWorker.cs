using log4net;
using System;
using System.Net.Sockets;
using System.Globalization;
using System.Collections.Generic;

using SmartPlugMonitor.Config;
using SmartPlugMonitor.Sensors;

namespace SmartPlugMonitor.Workers.TpLink
{
    public class TpLinkSensorWorker : ISensorWorker
    {
        private static readonly ILog Log = LogManager.GetLogger (typeof(TpLinkSensorWorker));

        private const string SENSOR_ID_ERR = "error";
        private const string SENSOR_ID_WATTAGE = "wattage";
        private const string SENSOR_ID_VOLTAGE = "voltage";
        private const string SENSOR_ID_CURRENT = "current";
       
        private const string ERROR_NOT_CONNECTED = "D/C";
        private const string ERROR_NOT_AVAILABLE = "N/A";

        private readonly TpLinkConfig config;

        public TpLinkSensorWorker (TpLinkConfig config)
        {
            this.config = config;
        }

        public string DisplayName { get { return $"tp-link@{config.IpAddress}:{config.PortNumber}"; } } 

        public bool IsConfigComplete { get { return Uri.CheckHostName( config.IpAddress ) != UriHostNameType.Unknown; } }

        public ICollection<SensorWorkerResult> Results {
            get {
                try {
                    using (var tcpClient = new TcpClient ()) {
                        var connectionResult = tcpClient.BeginConnect (config.IpAddress, config.PortNumber, null, null);
                        var connectionSuccess = connectionResult.AsyncWaitHandle.WaitOne (TimeSpan.FromMilliseconds (100));
                        if (!connectionSuccess) {
                            Log.Error ("ERROR_NOT_CONNECTED");
                            return new [] { new SensorWorkerResult (DisplayName, SENSOR_ID_ERR, ERROR_NOT_CONNECTED) };
                        }
                        tcpClient.EndConnect (connectionResult);

                        var data = TpLinkSensor.getRealtimeData (tcpClient.GetStream ());
                        if (data == null) {
                            Log.Error ("ERROR_NOT_AVAILABLE");
                            return new [] { new SensorWorkerResult (DisplayName, SENSOR_ID_ERR, ERROR_NOT_AVAILABLE) };
                        }

                        var results = new List<SensorWorkerResult> ();

                        if (config.MonitorWattage) {
                            results.Add (new SensorWorkerResult (DisplayName, SENSOR_ID_WATTAGE, data.Wattage.ToString ("N0", CultureInfo.InvariantCulture)));
                        }
                        if (config.MonitorVoltage) {
                            results.Add (new SensorWorkerResult (DisplayName, SENSOR_ID_VOLTAGE, data.Voltage.ToString ("N0", CultureInfo.InvariantCulture)));
                        }
                        if (config.MonitorCurrent) {
                            results.Add (new SensorWorkerResult (DisplayName, SENSOR_ID_CURRENT, data.Current.ToString ("N2", CultureInfo.InvariantCulture).TrimStart ('0')));
                        }

                        return results;
                    }
                } catch (SocketException e) {
                    Log.Error (e.Message);
                    return new [] { new SensorWorkerResult (DisplayName, SENSOR_ID_ERR, ERROR_NOT_CONNECTED) };
                } catch (Exception e) {
                    Log.Error (e.ToString ());
                    return new [] { new SensorWorkerResult (DisplayName, SENSOR_ID_ERR, ERROR_NOT_AVAILABLE) };
                }
            }
        }

        public void Start ()
        {
        }

        public void Stop ()
        {
        }

        public void Dispose ()
        {
            Stop ();
        }
    }
}
