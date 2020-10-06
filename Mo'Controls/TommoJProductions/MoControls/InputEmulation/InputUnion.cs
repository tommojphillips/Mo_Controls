using System.Runtime.InteropServices;

namespace TommoJProductions.MoControls.InputEmulation
{
    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        // Written, 06.10.2020

        [FieldOffset(0)]
        internal MouseInput mi;
        [FieldOffset(0)]
        internal KEYBDINPUT ki;
        [FieldOffset(0)]
        internal HardwareInput hi;
    }
}
