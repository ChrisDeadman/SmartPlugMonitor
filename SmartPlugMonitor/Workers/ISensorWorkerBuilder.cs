using System;

namespace SmartPlugMonitor.Workers
{
    public interface ISensorWorkerBuilder
    {
        bool isConfigComplete { get; }

        ISensorWorker build ();
    }
}