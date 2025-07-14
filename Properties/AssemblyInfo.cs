// VERSION 1.1


using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("MoControlsV2")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tommo J. Productions")]
[assembly: AssemblyProduct("MoControlsV2")]
[assembly: AssemblyCopyright("Tommo J. Productions Copyright © 2025")]
[assembly: NeutralResourcesLanguage("en-AU")]

// Version information
[assembly: AssemblyVersion("2.0.0.4")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace TommoJProductions.MoControlsV2 {

    public class VersionInfo {
	    public const string lastestRelease = "15.07.2025 12:34 AM";
	    public const string version = "2.0.0.4";

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

