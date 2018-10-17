namespace TommoJProductions.MoControls.MouseEmulation
{
    /// <summary>
    /// Represents a super structure for input data of the C++ function SendInput
    /// </summary>
    internal struct InputData
    {
        // Written, 01.08.2018

        /// <summary>
        /// Represents the type of the input, 0 for mouse.
        /// </summary>
        internal int type;
        /// <summary>
        /// Represents mouse data.
        /// </summary>
        internal MouseInput data;
    }
}
