using System;

namespace TommoJProductions.MoControls.InputEmulation
{
    /// <summary>
    /// Represents keyboard event flags
    /// </summary>
    [Flags]
    public enum KeyEventF : uint
    {
        KEYDOWN = 0x0100,
        EXTENDEDKEY = 0x0001,
        KEYUP = 0x0002,
        SCANCODE = 0x0008,
        UNICODE = 0x0004
    }
}
