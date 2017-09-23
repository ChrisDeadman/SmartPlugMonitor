using System;
using System.Linq;
using System.Collections.Generic;

using Gtk;
using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Ui
{
    public class GtkConfigWindow : GtkWindow
    {
        private readonly IList<GtkConfigPage> configPages;

        public GtkConfigWindow (IEnumerable<GtkConfigPage> configPages) : base ("Configure")
        {
            this.configPages = configPages.ToList ();
            Initialize ();
        }

        private void Initialize ()
        {
            this.Icon = Globals.ApplicationIcon.ToBitmap ().ToPixbuf (); 

            var notebook = new Notebook ();

            foreach (var page in configPages) {
                notebook.AppendPage (page, new Label (page.Title));
            }

            this.Add (notebook);
            this.ShowAll ();
        }
    }
}
