// VERSION 1.1


using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("MoControlsV2")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tommo J. Productions")]
[assembly: AssemblyProduct("MoControlsV2")]
[assembly: AssemblyCopyright("Tommo J. Productions Copyright © 2026")]
[assembly: NeutralResourcesLanguage("en-AU")]

// Version information
[assembly: AssemblyVersion("2.0.0.188")]
[assembly: AssemblyFileVersion("2.0.0.188")]

namespace TommoJProductions.MoControlsV2 {

    public class VersionInfo {
	    public const string lastestRelease = "06.01.2026 12:17 AM";
	    public const string version = "2.0.0.188";

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

