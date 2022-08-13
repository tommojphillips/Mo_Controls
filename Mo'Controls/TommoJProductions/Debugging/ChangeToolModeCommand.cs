using MSCLoader;
using TommoJProductions.MoControls;

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
                ControlManager.instance.toggleToolMode();
                MoControlsMod.print("Command executed successfully", DebugTypeEnum.none);
            }
            else
                MoControlsMod.print("Command did not execute successfully", DebugTypeEnum.none);
        }
    }
}
