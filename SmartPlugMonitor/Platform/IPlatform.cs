using System;
using System.Drawing;
using System.Collections.Generic;

using SmartPlugMonitor.Toolbox;
using SmartPlugMonitor.Workers;

namespace SmartPlugMonitor.Platform
{
    public interface IPlatform
    {
        void Init ();

        void ApplicationRun ();

        void ApplicationExit ();

        void ApplicationInvoke (Action action);

        Point GetCursorPosition (IWindow window);

        ITrayIconStrip CreateTrayIconStrip ();

        IWindow CreateConfigWindow ();

        IEnumerable<ISensorWorker> CreateSensorWorkers ();
    }
}
