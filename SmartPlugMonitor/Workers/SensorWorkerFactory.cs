using System;
using System.Collections.Generic;

namespace SmartPlugMonitor.Workers
{
    public class SensorWorkerFactory
    {
        private readonly List<ISensorWorker> workers = new List<ISensorWorker> ();

        public IList<ISensorWorker> Workers { get { return workers.AsReadOnly (); } }

        public void build (IEnumerable<ISensorWorkerBuilder> builders)
        {
            Stop ();

            foreach (var builder in builders) {
                if (builder.isConfigComplete) {
                    workers.Add (builder.build ());
                }
            }
        }

        public void Start ()
        {
            foreach (var worker in workers) {
                worker.Start ();
            }
        }

        public void Stop ()
        {
            foreach (var worker in workers) {
                worker.Stop ();
                worker.Dispose ();
            }

            workers.Clear ();
        }
    }
}
