﻿using System;
using System.Collections.Generic;

namespace SmartPlugMonitor.Workers
{
    public interface ISensorWorker : IDisposable
    {
        string DisplayName { get; }

        ICollection<SensorResult> Results { get; }

        void Start ();

        void Stop ();
    }
}
