using MSCLoader;
using TommoJProductions.MoControls;

namespace TommoJProductions.Debugging
{
    /// <summary>
    /// Represents the debug command for <see cref="MoControlsMod"/>.
    /// </summary>
    internal class DebugConsoleCommand : ConsoleCommand
    {
        // Written, 08.10.2018

        public override string Name => "mcdebug";

        public override string Help => "Enables/Disables debug mode for the mod, Mo'Controls. <color=grey>1.) <i>none or '0'</i>, 2.) <i>partial or '1'</i>, 3.) <i>full or '2'</i></color>";

        public override void Run(string[] inArgs)
        {
            // Written, 08.10.2018

            if (inArgs.Length == 1)
            {
                switch (inArgs[0])
                {
                    case "0":
                        MoControlsSaveData.loadedSaveData.debugMode = DebugTypeEnum.none;
                        break;
                    case "1":
                        MoControlsSaveData.loadedSaveData.debugMode = DebugTypeEnum.partial;
                        break;
                    case "2":
                        MoControlsSaveData.loadedSaveData.debugMode = DebugTypeEnum.full;
                        break;
                    case "none":
                        MoControlsSaveData.loadedSaveData.debugMode = DebugTypeEnum.none;
                        break;
                    case "partial":
                        MoControlsSaveData.loadedSaveData.debugMode = DebugTypeEnum.partial;
                        break;
                    case "full":
                        MoControlsSaveData.loadedSaveData.debugMode = DebugTypeEnum.full;
                        break;
                    default:
                        MoControlsMod.print("debug mode does not exist.. Use:\r\n1.) none or '0'\r\n2.) partial or '1'\r\n3.) full or '2'", DebugTypeEnum.none);
                        break;
                }
                MoControlsSaveData.loadedSaveData.saveSettings();
                MoControlsMod.print("Debug Mode set to <b>" + MoControlsSaveData.loadedSaveData.debugMode + "</b>.", DebugTypeEnum.none);
            }
        }
    }
}
