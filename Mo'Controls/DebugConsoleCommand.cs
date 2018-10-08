using MSCLoader;

namespace TommoJProdutions.MoControls
{
    /// <summary>
    /// Represents the debug command for <see cref="MoControlsMod"/>.
    /// </summary>
    internal class DebugConsoleCommand : ConsoleCommand
    {
        public override string Name => "mcdebug";

        public override string Help => "Enables/Disables debug mode for the mod, Mo'Controls";

        public override void Run(string[] args)
        {
            // Written, 08.10.2018

            MoControlsMod.debug = !MoControlsMod.debug;
            MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
            MoControlsMod.print("Debug Mode set to <b>" + MoControlsMod.debug + "</b>.");
        }
    }
}
