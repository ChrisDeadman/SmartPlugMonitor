using log4net;
using System;
using System.Linq;
using System.Threading;
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

        private readonly SynchronizedCollection<ISensorWorker> workers = new SynchronizedCollection<ISensorWorker> ();

        private Thread pollingThread;

        public ICollection<SensorWorkerResult> SensorResults { get; private set; } = new SensorWorkerResult[] {};

        public event EventHandler<SensorResultsChangedEventArgs> SensorResultsChanged;

        public void Start (IEnumerable<ISensorWorker> sensorWorkers)
        {
            if (pollingThread != null) {
                throw new InvalidOperationException ();
            }

            sensorWorkers.AsParallel ().ForAll (worker => {
                if (worker.IsConfigComplete) {
                    workers.Add (worker);
                    worker.Start ();
                }
            });

            pollingThread = new Thread (() => {
                try {
                    while (true) {
                        QuerySensors ();
                        Thread.Sleep (POLLING_INTERVAL);
                    }
                } catch (ThreadAbortException) {
                }
            });
            pollingThread.Start ();
        }

        public void Stop ()
        {
            if (pollingThread != null) {
                pollingThread.Abort ();
                pollingThread.Join ();
                pollingThread = null;
            }

            workers.AsParallel ().ForAll (worker => {
                worker.Stop ();
            });
            workers.Clear ();
        }

        public void Dispose ()
        {
            Stop ();
        }

        private void QuerySensors ()
        {
            SensorResults = workers.AsParallel().SelectMany (w => w.Results).ToList ();

            foreach (var result in SensorResults) {
                Log.Info (result);
            }

            if (SensorResultsChanged != null) {
                SensorResultsChanged.Invoke (this, new SensorResultsChangedEventArgs (SensorResults));
            }
        }
    }
}
