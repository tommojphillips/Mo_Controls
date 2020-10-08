﻿using MSCLoader;
using System;
using TommoJProductions.MoControls;
using TommoJProductions.MoControls.InputEmulation;

namespace TommoJProductions.Debugging
{
    class ChangeToolModeCommand : ConsoleCommand
    {
        public override string Name => "mctool";

        public override string Help => "Toggles the tool mode either, 'hand' or 'tool'";

        public override void Run(string[] args)
        {
            // Written, 06.10.2020

            if (args.Length == 0)
            {
                MoControlsGO.controlManager.toggleToolMode();
                MoControlsMod.print("Command executed successfully", DebugTypeEnum.none);
            }
            else
                MoControlsMod.print("Command did not execute successfully", DebugTypeEnum.none);
        }
    }
}
