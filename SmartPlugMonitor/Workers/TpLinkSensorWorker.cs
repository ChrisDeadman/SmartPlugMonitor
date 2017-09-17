﻿using log4net;
using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SmartPlugMonitor.Config;
using SmartPlugMonitor.Sensors;
using System.Globalization;

namespace SmartPlugMonitor.Workers
{
    public class TpLinkSensorWorker : ISensorWorker
    {
        private static readonly ILog Log = LogManager.GetLogger (typeof(TpLinkSensorWorker));

        private static readonly string ERROR_NOT_CONNECTED = "D/C";
        private static readonly string ERROR_NOT_AVAILABLE = "N/A";
        private static readonly int PORT_NUMBER = 9999;

        private readonly String ipAddress;
        private readonly bool monitorWattage;
        private readonly bool monitorVoltage;
        private readonly bool monitorCurrent;

        public TpLinkSensorWorker (String ipAddress, bool monitorWattage, bool monitorVoltage, bool monitorCurrent)
        {
            this.ipAddress = ipAddress;
            this.monitorWattage = monitorWattage;
            this.monitorVoltage = monitorVoltage;
            this.monitorCurrent = monitorCurrent;
        }

        public string DisplayName { get { return "tp-link"; } }

        public ICollection<string> Values {
            get {
                try {
                    using (var tcpClient = new TcpClient ()) {
                        var result = tcpClient.BeginConnect (ipAddress, PORT_NUMBER, null, null);
                        var success = result.AsyncWaitHandle.WaitOne (TimeSpan.FromMilliseconds (100));
                        if (!success) {
                            Log.Error ("ERROR_NOT_CONNECTED");
                            return new[] { ERROR_NOT_CONNECTED };
                        }
                        tcpClient.EndConnect (result);

                        var data = TpLinkSensor.getRealtimeData (tcpClient.GetStream ());
                        if (data == null) {
                            Log.Error ("ERROR_NOT_AVAILABLE");
                            return new[] { ERROR_NOT_AVAILABLE };
                        }

                        var values = new List<String> ();
                        if (monitorWattage) {
                            values.Add (data.Wattage.ToString ("N0", CultureInfo.InvariantCulture));
                        }
                        if (monitorVoltage) {
                            values.Add (data.Voltage.ToString ("N0", CultureInfo.InvariantCulture));
                        }
                        if (monitorCurrent) {
                            values.Add (data.Current.ToString ("N2", CultureInfo.InvariantCulture).TrimStart ('0'));
                        }

                        if (values.Count > 0) {
                            Log.Info (String.Join ((", "), values));
                        }

                        return values;
                    }
                } catch (SocketException e) {
                    Log.Error (e.Message);
                    return new[] { ERROR_NOT_CONNECTED };
                } catch (Exception e) {
                    Log.Error (e.ToString ());
                    return new[] { ERROR_NOT_AVAILABLE };
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

        public class TpLinkSensorWorkerBuilder : ISensorWorkerBuilder
        {
            public bool isConfigComplete {
                get {
                    var config = Globals.ConfigFile.TpLinkConfig;
                    return !String.IsNullOrEmpty (config.IpAddress);
                }
            }

            public ISensorWorker build ()
            {
                var config = Globals.ConfigFile.TpLinkConfig;
                return new TpLinkSensorWorker (config.IpAddress, config.MonitorWattage, config.MonitorVoltage, config.MonitorCurrent);
            }
        }
    }
}
