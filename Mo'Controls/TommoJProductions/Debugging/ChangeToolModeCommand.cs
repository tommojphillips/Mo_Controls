using MSCLoader;
using TommoJProductions.MoControls;
using TommoJProductions.MoControls.InputEmulation;

namespace TommoJProductions.Debugging
{
    class ChangeToolModeCommand : ConsoleCommand
    {
        public override string Name => "mctool";

        public override string Help => "Changes the tool mode based on argument, expecting either, 'hand' or 'tool'";

        public override void Run(string[] args)
        {
            // Written, 06.10.2020

            string errorMess = null;
            if (args.Length == 1)
            {
                if (args[0].ToLower() == "hand") // Hand mode 
                    KeyboardEmulator.Send(ScanCodeShort.KEY_1);
                else if (args[0].ToLower() == "tool") // Tool mode
                    KeyboardEmulator.Send(ScanCodeShort.KEY_2);
                else
                    errorMess = "'" + args[0] + "' is not a vaild argument. Command expects either, 'hand' or 'tool'.";
            }
            else
                errorMess = "Command expects 1 argument!";

            if (errorMess != null)
                MoControlsMod.print(errorMess, DebugTypeEnum.none);
            else
                MoControlsMod.print("Command executed successfully", DebugTypeEnum.none);
        }
    }
}
