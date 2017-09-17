﻿using System.Windows.Forms;
using SmartPlugMonitor.Config;
using System.Drawing;
using System.Reflection;

namespace SmartPlugMonitor
{
    public static class Globals
    {
        public const string ApplicationName = "Smartplug Monitor";

        public static readonly string ApplicationPath = Application.StartupPath + @"/";

        public static readonly Icon ApplicationIcon = new Icon (Assembly.GetExecutingAssembly ().GetManifestResourceStream ("icon.ico"));

        public static readonly string ConfigFilePath = ApplicationPath + "SmartPlugMonitor.exe.config";

        public static readonly ConfigFile ConfigFile = ConfigFile.Load (ConfigFilePath);

        public static void SaveConfigFile ()
        {
            ConfigFile.Save (ConfigFilePath);
        }
    }
}
