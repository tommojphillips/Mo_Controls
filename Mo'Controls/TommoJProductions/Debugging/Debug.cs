using System;
using System.IO;
using TommoJProductions.MoControls;

namespace TommoJProductions.Debugging
{
    /// <summary>
    /// Represents <see langword="static"/> debug methods for debugging/testing mod software.
    /// </summary>
    internal static class Debug
    {
        // Written, 07.01.2019

        /// <summary>
        /// Represents the external inputs output path.
        /// </summary>
        internal const string CINPUT_EXTERN_INPT_OUTPUT_PATH = "cInput_ExternalInputs.txt";

        /// <summary>
        /// Writes all external inputs from cInput to a file.
        /// </summary>
        internal static void writeCinputExternInputs()
        {
            // Written, 07.01.2019

            try
            {
                File.WriteAllText(CINPUT_EXTERN_INPT_OUTPUT_PATH, cInput.externalInputs);
                MoControlsMod.print("Successfully wrote cInput external inputs to a file, " + CINPUT_EXTERN_INPT_OUTPUT_PATH, DebugTypeEnum.none);
            }
            catch (Exception ex)
            {
                MoControlsMod.print("An error occured while attempting to write cinput extern inputs to a file, '" + CINPUT_EXTERN_INPT_OUTPUT_PATH + "'.\r\nError: " + ex.ToString(), DebugTypeEnum.none);
            }
        }
    }
}
