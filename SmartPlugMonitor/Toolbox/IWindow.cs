using System;
using System.Drawing;

namespace SmartPlugMonitor.Toolbox
{
    public interface IWindow
    {
        event EventHandler OnClose;

        int Width { get; }

        int Height { get; }

        Point Position { get; set; }

        void Show (Point position);

        void Close ();
    }
}
