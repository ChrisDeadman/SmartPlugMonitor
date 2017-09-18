using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using log4net;

namespace SmartPlugMonitor.Workers
{
    public class SensorWorkerRunner : IDisposable
    {
        public class SensorResultsChangedEventArgs : EventArgs
        {
            public ICollection<SensorResult> SensorResults { get; private set; }

            public SensorResultsChangedEventArgs (ICollection<SensorResult> sensorResults)
            {
                this.SensorResults = sensorResults;
            }
        }

        private static readonly ILog Log = LogManager.GetLogger (typeof(SensorWorkerRunner));

        private const int POLLING_INTERVAL = 2000;

        private readonly Timer pollingTimer;
        private readonly SynchronizedCollection<ISensorWorker> workers = new SynchronizedCollection<ISensorWorker> ();

        public SensorWorkerRunner ()
        {
            this.pollingTimer = new Timer {
                Interval = POLLING_INTERVAL
            };
            pollingTimer.Tick += new EventHandler (OnPollSensors);
        }

        public ICollection<SensorResult> SensorResults { get; private set; } = new SensorResult[] {};

        public event EventHandler<SensorResultsChangedEventArgs> SensorResultsChanged;

        private void OnPollSensors (object sender, EventArgs args)
        {
            SensorResults = workers.SelectMany (w => w.Results).ToList ();

            foreach (var result in SensorResults) {
                Log.Info (result);
            }

            if (SensorResultsChanged != null) {
                SensorResultsChanged.Invoke (this, new SensorResultsChangedEventArgs (SensorResults));
            }
        }

        public void Start (IEnumerable<ISensorWorkerBuilder> builders)
        {
            if (pollingTimer.Enabled) {
                throw new InvalidOperationException ();
            }

            builders.AsParallel ().ForAll (builder => {
                if (builder.isConfigComplete) {
                    var worker = builder.build ();
                    worker.Start ();
                    workers.Add (worker);
                }
            });

            pollingTimer.Start ();
        }

        public void Stop ()
        {
            if (pollingTimer.Enabled) {
                pollingTimer.Stop ();
            }

            workers.AsParallel ().ForAll (worker => {
                worker.Stop ();
                worker.Dispose ();
            });
            workers.Clear ();
        }

        public void Dispose ()
        {
            Stop ();
        }
    }
}
