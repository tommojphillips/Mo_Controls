using MSCLoader;
using System;
using TommoJProductions.MoControls;

namespace TommoJProductions.Debugging
{
    /// <summary>
    /// Represents a command that lists all debug stats.
    /// </summary>
    internal class DebugStatsCommand : ConsoleCommand
    {
        // Written, 28.12.2018

        public override string Name => "mcdebugstats";

        public override string Help => "Displays mo'controls debug Configuration";

        public override void Run(string[] inArgs)
        {
            // Written, 28.12.2018

            MoControlsMod.print(String.Format("<b>MC Debug Stats:</b>" +
                "\r\nmc_DebugMode=<b>{1}</b>" +
                "\r\nDEBUG Config=<b>{2}</b>",
                MoControlsSaveData.loadedSaveData.debugMode,
                VersionInfo.IS_DEBUG_CONFIG), DebugTypeEnum.none);
        }
    }
}
