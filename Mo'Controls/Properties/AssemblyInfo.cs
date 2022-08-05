using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("Mo_Controls")]
[assembly: AssemblyProduct("Mo'Controls")]
[assembly: AssemblyDescription("A mod for my summer car that implements full xbox controller support eg rumble support")]
[assembly: AssemblyCompany("Tommo J. Productions")]
[assembly: AssemblyCopyright("Copyright © Tommo J. Productions 2022")]
[assembly: AssemblyTrademark("Azine")]
[assembly: NeutralResourcesLanguage("en-AU")]
[assembly: AssemblyConfiguration("")]

// Version information
[assembly: AssemblyVersion("1.1.216.5")]
//[assembly: AssemblyFileVersion("1.1.216.5")]

public class VersionInfo
{
	public const string lastestRelease = "05.08.2022 02:15 PM";
	public const string version = "1.1.216.5";

    /// <summary>
    /// Represents if the mod has been complied for x64
    /// </summary>
    #if x64
        internal const bool IS_64_BIT = true;
    #else
        internal const bool IS_64_BIT = false;
    #endif
    #if DEBUG
        internal const bool IS_DEBUG_CONFIG = true;
    #else
        internal const bool IS_DEBUG_CONFIG = false;
    #endif
}

