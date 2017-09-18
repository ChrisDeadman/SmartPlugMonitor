using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace SmartPlugMonitor.Workers
{
    public class SensorWorkerRunner : IDisposable
    {
        private const int POLLING_INTERVAL = 2000;

        private readonly Timer pollingTimer;
        private readonly List<ISensorWorker> workers = new List<ISensorWorker> ();

        public SensorWorkerRunner ()
        {
            this.pollingTimer = new Timer {
                Interval = POLLING_INTERVAL
            };
            pollingTimer.Tick += new EventHandler (OnPollSensors);
        }

        public IReadOnlyDictionary<ISensorWorker, IReadOnlyDictionary<string, string>> Values { get; private set; }

        public event EventHandler<ValuesChangedEventArgs> ValuesChanged;

        private void OnPollSensors (object sender, EventArgs args)
        {
            var sensorSummary = new Dictionary<ISensorWorker, IReadOnlyDictionary<string, string>> ();

            foreach (var worker in workers) {
                if (worker.Values.Count > 0) {
                    sensorSummary[worker] = worker.Values;
                }
            }

            if (ValuesChanged != null) {
                ValuesChanged.Invoke (this, new ValuesChangedEventArgs { SensorSummary = sensorSummary });
            }
        }

        public void Start (IEnumerable<ISensorWorkerBuilder> builders)
        {
            if (pollingTimer.Enabled) {
                throw new InvalidOperationException ();
            }

            foreach (var builder in builders) {
                if (builder.isConfigComplete) {
                    var worker = builder.build ();
                    worker.Start ();
                    workers.Add (worker);
                }
            }

            pollingTimer.Start ();
        }

        public void Stop ()
        {
            if (pollingTimer.Enabled) {
                pollingTimer.Stop ();
            }

            foreach (var worker in workers) {
                worker.Stop ();
                worker.Dispose ();
            }

            workers.Clear ();
        }

        public void Dispose ()
        {
            Stop ();
        }

        public class ValuesChangedEventArgs : EventArgs
        {
            public IReadOnlyDictionary<ISensorWorker, IReadOnlyDictionary<string, string>> SensorSummary { get; set; }
        }
    }
}
