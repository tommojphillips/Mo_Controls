﻿using System;
using MSCLoader;
using TommoJProductions.MoControls;

namespace TommoJProductions.Debugging
{
    public class DebugStatsCommand : ConsoleCommand
    {
        // Written, 28.12.2018
        public override string Name => "mcdebugstats";

        public override string Help => "Displays mo'controls debug Configuration";

        public override void Run(string[] inArgs)
        {
            // Written, 28.12.2018

            string debugConStr;
#if DEBUG
            debugConStr = "True";
#else
            debugConStr = "False";
#endif
            MoControlsMod.print(String.Format("<b>MC Debug Stats:</b>" +
                "\r\nmc_ReleaseVersion=<b>{0}</b>" +
                "\r\nmc_DebugMode=<b>{1}</b>" +
                "\r\nDEBUG Config=<b>{2}</b>" +
                "\r\nMod API Info:" +
                "\r\nUser is " + (MoControlsMod.instance.modApiData.runningModApi ? "" : "not") + " running " +
                "<b><i>Mod API" + (MoControlsMod.instance.modApiData.runningModApi ? " v" + MoControlsMod.instance.modApiData.version + "</i></b>." : "."),
                MoControlsMod.isReleaseVersion,
                MoControlsMod.debug,
                debugConStr), DebugTypeEnum.none);
        }
    }
}
