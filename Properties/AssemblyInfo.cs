// VERSION 1.2


using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("MoControlsV2")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("tommojphillips")]
[assembly: AssemblyProduct("MoControlsV2")]
[assembly: AssemblyCopyright("tommojphillips Copyright © 2026")]
[assembly: NeutralResourcesLanguage("en-AU")]

// Version information
[assembly: AssemblyVersion("2.0.1.4")]
[assembly: AssemblyFileVersion("2.0.1.4")]

namespace TommoJProductions.MoControlsV2 {

    public class VersionInfo {
	    public const string lastestRelease = "17.01.2026 09:00 PM";
	    public const string version = "2.0.1";
	    public const string full_version = "2.0.1.4";
	    public const string build = "4";

#if X64
        internal const bool IS_X64 = true;
#else
        internal const bool IS_X64 = false;
#endif

#if DEBUG
        internal const bool IS_DEBUG = true;
#else
        internal const bool IS_DEBUG = false;
#endif
    }
}

