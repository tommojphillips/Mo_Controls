using System;
using System.Runtime.InteropServices;

namespace TommoJProductions.MoControlsV2.MouseInput {
    internal class User32_Imports {
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point pos);
        
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] Input pInputs, int cbSize);
    }
}
