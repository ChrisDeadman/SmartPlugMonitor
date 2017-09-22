using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartPlugMonitor.Ui
{
    public class TrayIconStrip : IDisposable
    {
        private readonly List<NotifyIcon> trayIcons = new List<NotifyIcon> ();

        public void Update (IEnumerable<Action<NotifyIcon>> iconInitializers)
        {
            var iconIdx = 0;

            foreach (var initializer in iconInitializers.Reverse()) {
                if (trayIcons.Count <= iconIdx) {
                    trayIcons.Add (new NotifyIcon ());
                    Thread.Sleep (50); // Ensure correct tray icon order
                }

                var icon = trayIcons [iconIdx];
                RemoveLinkedHandlers (icon);
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

        private void RemoveLinkedHandlers (Component component)
        {
            var eventListProperty = component.GetType ().GetProperty ("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            var eventList = (EventHandlerList)eventListProperty.GetValue (component, null);

            foreach (var eventKeyInfo in component.GetType ().GetFields (BindingFlags.Static | BindingFlags.NonPublic)) {
                var eventKey = eventKeyInfo.GetValue (component);
                eventList.RemoveHandler (eventKey, eventList [eventKey]);
            }
        }
    }
}
