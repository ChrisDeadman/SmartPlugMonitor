using System.Reflection;
using System.Runtime.CompilerServices;

using SmartPlugMonitor;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.

[assembly: AssemblyTitle (Globals.ApplicationName)]
[assembly: AssemblyDescription ("Tray-based power monitoring software for smart plugs")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("Deads Inc.")]
[assembly: AssemblyProduct (Globals.ApplicationName)]
[assembly: AssemblyCopyright ("Copyright © 2017")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion ("0.2.*")]

// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator]
// This will cause log4net to look for a configuration file
// called SmartPlugMonitor.exe.config in the application base
// directory (i.e. the directory containing TestApp.exe)
