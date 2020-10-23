using System;

namespace TommoJProductions.MoControls.InputEmulation
{
    [Flags]
    public enum MouseEventButtons : uint
    {
        // Written, 06.10.2020

        Nothing = 0x00000000,
        XBUTTON1 = 0x00000001,
        XBUTTON2 = 0x00000002
    }
}
