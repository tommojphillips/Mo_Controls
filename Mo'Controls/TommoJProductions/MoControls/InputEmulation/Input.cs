using System.Runtime.InteropServices;

namespace TommoJProductions.MoControls.InputEmulation
{
    /// <summary>
    /// Represents keyboard input structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        // Written, 06.10.2020

        /// <summary>
        /// Represents the input type... 1 for keyboard
        /// </summary>
        public uint type;
        /// <summary>
        /// Represents the input union.
        /// </summary>
        public InputUnion U;
        /// <summary>
        /// Represents the size of the input
        /// </summary>
        public static int Size
        {
            get { return Marshal.SizeOf(typeof(Input)); }
        }
    }
}
