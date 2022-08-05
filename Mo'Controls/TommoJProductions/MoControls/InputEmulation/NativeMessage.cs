using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TommoJProductions.MoControls.InputEmulation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessage
    {
        public IntPtr handle;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point p;
    }
}
