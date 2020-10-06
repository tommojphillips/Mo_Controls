using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSCLoader;
using TommoJProductions.MoControls;
using TommoJProductions.MoControls.InputEmulation;

namespace TommoJProductions.Debugging
{
    class GetToolModeCommand : ConsoleCommand
    {
        public override string Name => "mctool";

        public override string Help => "writes what tool and toolmode the player is in to the mod console.";

        public override void Run(string[] args)
        {
            // Written, 06.10.2020

            bool error = false;
            if (args.Length == 1)
            {
                if (Int32.TryParse(args[0], out int intValue))
                    if (intValue == 1) // Hand mode 
                        KeyboardEmulator.Send(ScanCodeShort.KEY_1);
                    else if (intValue == 2) // Tool mode
                        KeyboardEmulator.Send(ScanCodeShort.KEY_2); 
                    else
                        error = true;
                else
                    error = true;
            }
            else
                error = true;

            if (error)
                MoControlsMod.print("the command did not execute correctly.. command expects 1 arg of type intger. check help command for more info", DebugTypeEnum.none);
        }
    }
}
