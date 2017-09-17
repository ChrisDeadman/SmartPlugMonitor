using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace SmartPlugMonitor.Workers
{
    public class SensorSummaryWorker : ISensorWorker
    {
        private static readonly int POLLING_INTERVAL = 2000;

        private readonly Timer pollingTimer;
        private readonly SensorWorkerFactory sensorWorkerFactory;
        private readonly Action<IDictionary<ISensorWorker, ICollection<string>>> updateValues;

        public SensorSummaryWorker (SensorWorkerFactory sensorWorkerFactory, Action<IDictionary<ISensorWorker, ICollection<string>>> updateValues)
        {
            this.sensorWorkerFactory = sensorWorkerFactory;
            this.updateValues = updateValues;
            this.pollingTimer = new Timer {
                Interval = POLLING_INTERVAL
            };
            pollingTimer.Tick += new EventHandler (UpdateValues);
        }

        public string DisplayName { get { return GetType ().Name; } }

        public ICollection<string> Values { get; private set; } = new string[] {};

        private void UpdateValues (object sender, EventArgs args)
        {
            var sensorSummary = new Dictionary<ISensorWorker, ICollection<string>> ();

            foreach (var worker in sensorWorkerFactory.Workers) {
                if ((worker != this) && (worker.Values.Count > 0)) {
                    sensorSummary [worker] = worker.Values;
                }
            }

            Values = sensorSummary.SelectMany (e => e.Value).ToList ();
            updateValues (sensorSummary);
        }

        public void Start ()
        {
            if (pollingTimer.Enabled) {
                throw new InvalidOperationException ();
            }

            pollingTimer.Start ();
        }

        public void Stop ()
        {
            if (pollingTimer.Enabled) {
                pollingTimer.Stop ();
            }
        }

        public void Dispose ()
        {
            Stop ();
        }

        public class SensorSummaryWorkerBuilder : ISensorWorkerBuilder
        {
            private readonly SensorWorkerFactory sensorWorkerFactory;
            private readonly Action<IDictionary<ISensorWorker, ICollection<string>>> updateValues;

            public SensorSummaryWorkerBuilder (SensorWorkerFactory sensorWorkerFactory, Action<IDictionary<ISensorWorker, ICollection<string>>> updateValues)
            {
                this.sensorWorkerFactory = sensorWorkerFactory;
                this.updateValues = updateValues;
            }

            public bool isConfigComplete {
                get { return sensorWorkerFactory != null; }
            }

            public ISensorWorker build ()
            {
                return new SensorSummaryWorker (sensorWorkerFactory, updateValues);
            }
        }
    }
}
