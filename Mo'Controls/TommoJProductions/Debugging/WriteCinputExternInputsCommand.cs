using MSCLoader;

namespace TommoJProductions.Debugging
{
    // Written, 07.01.2019

    /// <summary>
    /// Represents a console command that calls <see cref="Debug.writeCinputExternInputs"/>.
    /// </summary>
    internal class WriteCinputExternInputsCommand : ConsoleCommand
    {
        public override string Name => "mcwritecinptextern";

        public override string Help => "Writes all cInput external inputs to a file, " + Debug.CINPUT_EXTERN_INPT_OUTPUT_PATH;

        public override void Run(string[] inArgs)
        {
            // Written, 07.01.2019

            Debug.writeCinputExternInputs();
        }
    }
}
