using log4net;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SmartPlugMonitor.Sensors
{
    public class TpLinkSensor
    {
        public class RealTimeData
        {
            public double Wattage { get; private set; }

            public double Voltage { get; private set; }

            public double Current { get; private set; }

            public RealTimeData (double wattage, double voltage, double current)
            {
                this.Wattage = wattage;
                this.Voltage = voltage;
                this.Current = current;
            }
        }

        private static readonly ILog Log = LogManager.GetLogger (typeof(TpLinkSensor));

        private static readonly Regex WattageRegex = new Regex ("\"power\":([0-9]*)");
        private static readonly Regex VoltageRegex = new Regex ("\"voltage\":([0-9]*)");
        private static readonly Regex CurrentRegex = new Regex ("\"current\":([0-9]*[.][0-9]{2})");

        public static RealTimeData getRealtimeData (Stream stream)
        {
            var response = SendReceive (stream, "{\"emeter\":{\"get_realtime\":{}}}");
            var wattage = 0.0;
            var voltage = 0.0;
            var current = 0.0;

            var matches = WattageRegex.Match (response);
            foreach (Group match in matches.Groups) {
                if (match.Success) {
                    double.TryParse (match.Value, out wattage);
                } else {
                    return null;
                }
            }

            matches = VoltageRegex.Match (response);
            foreach (Group match in matches.Groups) {
                if (match.Success) {
                    double.TryParse (match.Value, out voltage);
                } else {
                    return null;
                }
            }

            matches = CurrentRegex.Match (response);
            foreach (Group match in matches.Groups) {
                if (match.Success) {
                    double.TryParse (match.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out current);
                } else {
                    return null;
                }
            }

            return new RealTimeData (wattage, voltage, current);
        }

        private static String SendReceive (Stream stream, String message)
        {
            using (var writer = new BinaryWriter (stream, Encoding.UTF8))
            using (var reader = new BinaryReader (stream)) {
                Log.Debug ($"SEND -> {message}");
                writer.Write (Encrypt (message));

                var response = Decrypt (reader.ReadBytes (1024));
                Log.Debug ($"RECV <- {response}");

                return response;
            }
        }

        private static byte[] Encrypt (String message)
        {
            var key = 171;
            var idx = 0;
            var result = new byte[4 + message.Length];
            result [idx++] = 0;
            result [idx++] = 0;
            result [idx++] = 0;
            result [idx++] = 0;

            foreach (var chr in message) {
                var next = key ^ (byte)chr;
                key = next;
                result [idx++] = (byte)next;
            }
            return result;
        }

        private static String Decrypt (byte[] message)
        {
            var key = 171;
            var result = "";
            foreach (var chr in message.Skip(4)) { 
                var next = key ^ (byte)chr;
                key = (byte)chr;
                result += Convert.ToString ((char)next);
            }
            return result;
        }
    }
}
