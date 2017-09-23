using System;
using System.Collections.Generic;

namespace SmartPlugMonitor.Ui
{
    public interface IConfigController
    {
        string Title { get; }

        IEnumerable<UiItem> UiItems { get; }

        void Save ();
    }
}
