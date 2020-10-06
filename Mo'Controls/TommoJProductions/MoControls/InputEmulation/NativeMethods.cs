using System.Drawing;
using System.Runtime.InteropServices;

namespace TommoJProductions.MoControls.InputEmulation
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point pos);
        [DllImport("user32.dll")]
        internal static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] Input[] pInputs, int cbSize);
    }
}
