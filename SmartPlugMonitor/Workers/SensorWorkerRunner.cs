using log4net;
using System;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using System.Collections.Generic;

using SmartPlugMonitor.Config;

namespace SmartPlugMonitor.Workers
{
    public class SensorWorkerRunner : IDisposable
    {
        public class SensorResultsChangedEventArgs : EventArgs
        {
            public ICollection<SensorWorkerResult> SensorResults { get; private set; }

            public SensorResultsChangedEventArgs (ICollection<SensorWorkerResult> sensorResults)
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

        public ICollection<SensorWorkerResult> SensorResults { get; private set; } = new SensorWorkerResult[] {};

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

        public void Start (IEnumerable<ISensorWorker> sensorWorkers)
        {
            if (pollingTimer.Enabled) {
                throw new InvalidOperationException ();
            }

            sensorWorkers.AsParallel ().ForAll (worker => {
                if (worker.IsConfigComplete) {
                    workers.Add (worker);
                    worker.Start ();
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
