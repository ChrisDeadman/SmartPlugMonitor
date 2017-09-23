using System;
using System.Threading;
using System.Collections.Generic;

namespace SmartPlugMonitor.Toolbox
{
    public interface ITrayIconStrip : IDisposable
    {
        void Update (IEnumerable<Action<ITrayIcon>> iconInitializers);
    }

    public class TrayIconStrip<TIcon> : ITrayIconStrip
        where TIcon : ITrayIcon
    {
        private readonly List<ITrayIcon> trayIcons = new List<ITrayIcon> ();

        public void Update (IEnumerable<Action<ITrayIcon>> iconInitializers)
        {
            var iconIdx = 0;

            foreach (var initializer in iconInitializers) {
                if (trayIcons.Count <= iconIdx) {
                    var newIcon = (TIcon)Activator.CreateInstance (typeof(TIcon));
                    trayIcons.Add (newIcon);
                    newIcon.Visible = true;
                    Thread.Sleep (100); // Ensure correct tray icon order
                }

                var icon = trayIcons [iconIdx];
                initializer (icon);
                iconIdx++;
            }

            while (iconIdx < trayIcons.Count) {
                var icon = trayIcons [iconIdx];
                icon.Visible = false;
                icon.Dispose ();
                trayIcons.Remove (icon);
            }
        }

        public void Dispose ()
        {
            trayIcons.ForEach (icon => {
                icon.Visible = false;
                icon.Dispose ();
            });
            trayIcons.Clear ();
        }
    }
}
