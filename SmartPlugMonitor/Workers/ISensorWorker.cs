﻿using System;
using System.Collections.Generic;

namespace SmartPlugMonitor.Workers
{
    public interface ISensorWorker
    {
        string DisplayName { get; }

        bool IsConfigComplete { get; }

        ICollection<SensorWorkerResult> Results { get; }

        void Start ();

        void Stop ();
    }
}
