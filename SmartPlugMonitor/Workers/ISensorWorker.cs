using System;
using System.Collections.Generic;

namespace SmartPlugMonitor.Workers
{
    public interface ISensorWorker : IDisposable
    {
        string DisplayName { get; }

        IReadOnlyDictionary<string, string> Values { get; }

        void Start ();

        void Stop ();
    }
}
