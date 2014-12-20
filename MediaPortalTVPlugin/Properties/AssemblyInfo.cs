using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MediaBrowser.Plugins.MediaPortal")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Media Browser Team")]
[assembly: AssemblyProduct("MediaBrowser.Plugins.MediaPortal")]
[assembly: AssemblyCopyright("Copyright © Media Browser Team 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2c6a0219-7621-4b06-8a64-da3f7038b649")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.*")]

//// BUild type
//#if DEBUG
//[assembly: AssemblyConfiguration("Debug")]
//#else
//[assembly: AssemblyConfiguration("Release")]
//#endif

// This reflects the API version, only update this if the API changes, as dependent applications need to be rebuild when
// the assembly is replaced with an assembly with a different AssemblyVersion attribute. 
// [assembly: AssemblyVersion("0.1.0.0")]

// This reflects the version of this unique build, and should be changed with each build. Sadly it's not easily possible
// to let this change with each build in Visual Studio, so we don't have that in our version numbers now and hope people
// don't mess around too much with our builds. For stable releases, this is of the format major.minor.bugfix.0. For new 
// minor versions we use major.prev-minor.99.x, with x incrementing with each alpha/beta/RC release. For test versions of
// bugfix releases this is major.minor.prev-bugfix.x, with x incrementing with each alpha/beta/RC release. This number
// is also used to check for new versions in the configurator, so please follow this scheme strictly. 
//[assembly: AssemblyFileVersion("0.1.0.*")]