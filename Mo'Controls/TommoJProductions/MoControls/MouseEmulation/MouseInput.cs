using System;

namespace TommoJProductions.MoControls.MouseEmulation
{
    /// <summary>
    /// structure for mouse data
    /// </summary>
    internal struct MouseInput
    {
        // Written, 01.08.2018

        /// <summary>
        /// Represents X coordinate.
        /// </summary>
        internal int X;
        /// <summary>
        /// Represents Y coordinate.
        /// </summary>
        internal int Y;
        /// <summary>
        /// Represents mouse data, e.g. for mouse wheel.
        /// </summary>
        internal uint mouseData;
        /// <summary>
        /// Represents further mouse data, e.g. for mouse buttons.
        /// </summary>
        internal uint dwFlags;
        /// <summary>
        /// Represents the time of the event.
        /// </summary>
        internal uint time;
        /// <summary>
        /// Represents further information.
        /// </summary>
        internal IntPtr dwExtraInfo;
    }
}
