using MSCLoader;

namespace TommoJProductions.MoControls.Debugging
{
    /// <summary>
    /// Represents the debug command for <see cref="MoControlsMod"/>.
    /// </summary>
    internal class DebugConsoleCommand : ConsoleCommand
    {
        public override string Name => "mcdebug";

        public override string Help => "Enables/Disables debug mode for the mod, Mo'Controls.\r\n<color=grey>1.) <i>none or '0'</i>\r\n2.) <i>partial or '1'</i>\r\n3.) <i>full or '2'</i></color>";

        public override void Run(string[] args)
        {
            // Written, 08.10.2018

            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "0":
                        MoControlsMod.debug = DebugTypeEnum.none;
                        break;
                    case "1":
                        MoControlsMod.debug = DebugTypeEnum.partial;
                        break;
                    case "2":
                        MoControlsMod.debug = DebugTypeEnum.full;
                        break;
                    case "none":
                        MoControlsMod.debug = DebugTypeEnum.none;
                        break;
                    case "partial":
                        MoControlsMod.debug = DebugTypeEnum.partial;
                        break;
                    case "full":
                        MoControlsMod.debug = DebugTypeEnum.full;
                        break;
                    default:
                        MoControlsMod.print("debug mode does not exist.. Use:\r\n1.) none or '0'\r\n2.) partial or '1'\r\n3.) full or '2'");
                        break;
                }
                //MoControlsMod.debug = !MoControlsMod.debug;
                MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
                MoControlsMod.print("Debug Mode set to <b>" + MoControlsMod.debug + "</b>.");
            }
        }
    }
}
