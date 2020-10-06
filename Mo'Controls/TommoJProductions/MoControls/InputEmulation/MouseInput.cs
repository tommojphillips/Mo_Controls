using System;

namespace TommoJProductions.MoControls.InputEmulation
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
        internal int dx;
        /// <summary>
        /// Represents Y coordinate.
        /// </summary>
        internal int dy;
        /// <summary>
        /// Represents mouse data, e.g. for mouse wheel.
        /// </summary>
        internal MouseEventDataXButtons mouseData;
        /// <summary>
        /// Represents further mouse data, e.g. for mouse buttons.
        /// </summary>
        internal MouseEventF dwFlags;
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
