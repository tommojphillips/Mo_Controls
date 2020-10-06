using System;
using System.Runtime.InteropServices;

namespace TommoJProductions.MoControls.InputEmulation
{
    /// <summary>
    /// Represents keyboard input
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        internal VirtualKeyShort wVk;
        internal ScanCodeShort wScan;
        internal KeyEventF dwFlags;
        internal int time;
        internal UIntPtr dwExtraInfo;
    }
}
