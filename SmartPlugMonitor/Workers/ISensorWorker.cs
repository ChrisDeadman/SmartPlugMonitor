using System;
using System.Collections.Generic;

namespace SmartPlugMonitor.Workers
{
    public interface ISensorWorker : IDisposable
    {
        string DisplayName { get; }

        ICollection<string> Values { get; }

        void Start ();

        void Stop ();
    }
}
