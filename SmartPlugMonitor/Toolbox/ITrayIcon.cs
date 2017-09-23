using System;
using System.Drawing;
using System.Collections.Generic;

namespace SmartPlugMonitor.Toolbox
{
    public interface ITrayIcon : IDisposable
    {
        Boolean Visible { get; set; }

        string ToolTipText { set; }

        Bitmap Icon { set; }

        IDictionary<string, EventHandler> ContextMenu { set; }

        EventHandler OnActivate { get; set; }
    }
}
