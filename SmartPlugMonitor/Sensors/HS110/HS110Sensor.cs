using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using log4net;

namespace SmartPlugMonitor.Sensors.HS110
{
    public static class HS110Sensor
    {
        public class RealTimeData
        {
            public double Wattage { get; private set; }

            public double Voltage { get; private set; }

            public double Current { get; private set; }

            public string Error { get; private set; }

            public RealTimeData(double wattage, double voltage, double current, string error)
            {
                this.Wattage = wattage;
                this.Voltage = voltage;
                this.Current = current;
                this.Error = error;
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(HS110Sensor));

        private static readonly Regex WattageRegexV1 = new Regex("\"power\":([0-9]+)");
        private static readonly Regex VoltageRegexV1 = new Regex("\"voltage\":([0-9]+)");
        private static readonly Regex CurrentRegexV1 = new Regex("\"current\":([0-9]*[.][0-9]+)");

        private static readonly Regex WattageRegexV2 = new Regex("\"power_mw\":([0-9]+)");
        private static readonly Regex VoltageRegexV2 = new Regex("\"voltage_mv\":([0-9]+)");
        private static readonly Regex CurrentRegexV2 = new Regex("\"current_ma\":([0-9]+)");

        public static RealTimeData ReadRealtimeData(Func<byte[], byte[]> cbSendReceive)
        {
            var message_enc = Encrypt("{\"emeter\":{\"get_realtime\":{}}}");
            var message_len = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((ushort)message_enc.Length));
            var message = message_len.Concat(message_enc).ToArray(); // message is prefixed by 4 length bytes

            var response = cbSendReceive(message);
            if (response == null || response.Length <= 0) {
                return new RealTimeData(0, 0, 0, Globals.STATUS_NOT_CONNECTED);
            }

            var response_dec = Decrypt(response.Skip(4).ToArray()); // skip the 4 length bytes at the beginning and decrypt

            String error = "";

            var wattage = ParseValue(response_dec, WattageRegexV1, WattageRegexV2);
            if (wattage == null) {
                error += "Could not parse wattage;";
            }

            var voltage = ParseValue(response_dec, VoltageRegexV1, VoltageRegexV2);
            if (voltage == null) {
                error += "Could not parse voltage;";
            }

            var current = ParseValue(response_dec, CurrentRegexV1, CurrentRegexV2);
            if (current == null) {
                error += "Could not parse current;";
            }

            if (!String.IsNullOrEmpty(error)) {
                Log.Error(error);
            }

            return new RealTimeData(wattage ?? 0, voltage ?? 0, current ?? 0, !String.IsNullOrEmpty(error) ? Globals.STATUS_NOT_AVAILABLE : null);
        }

        private static double? ParseValue(String message, Regex v1Regex, Regex v2Regex)
        {
            foreach (var regex in new[] { v1Regex, v2Regex }) {
                var matches = regex.Match(message);
                foreach (Group match in matches.Groups) {
                    if (match.Success) {
                        if (double.TryParse(match.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result)) {
                            if (regex == v2Regex) {
                                return result / 1000;
                            } else {
                                return result;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static byte[] Encrypt(string message)
        {
            Log.Debug($"Encrypt -> {message}");
            var key = 171;
            var idx = 0;
            var result = new byte[message.Length];

            foreach (var chr in message) {
                var next = key ^ (byte)chr;
                key = next;
                result[idx++] = (byte)next;
            }
            return result;
        }

        private static string Decrypt(byte[] message)
        {
            var key = 171;
            var result = "";
            foreach (var chr in message) {
                var next = key ^ chr;
                key = chr;
                result += Convert.ToString((char)next);
            }
            Log.Debug($"Decrypt <- {result}");
            return result;
        }
    }
}
