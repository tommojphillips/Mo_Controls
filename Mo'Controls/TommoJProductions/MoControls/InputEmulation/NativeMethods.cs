using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace TommoJProductions.MoControls.InputEmulation
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point pos);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] Input[] pInputs, int cbSize);
        
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PeekMessage(out NativeMessage message, IntPtr handle, uint filterMin, uint filterMax, uint flags);
    }
}
