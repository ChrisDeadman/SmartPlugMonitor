using System;

namespace SmartPlugMonitor
{
    public class SensorResult
    {
        public string SensorName { get; private set; }

        public string ValueName { get; private set; }

        public string Value { get; private set; }

        public SensorResult (string sensorName, string valueName, string value)
        {
            this.Value = value;
            this.ValueName = valueName;
            this.SensorName = sensorName;
        }

        public override string ToString ()
        {
            return string.Format ("[SensorResult: SensorName={0}, ValueName={1}, Value={2}]", SensorName, ValueName, Value);
        }
    }
}
