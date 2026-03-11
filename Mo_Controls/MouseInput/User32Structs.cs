using System;
using System.Runtime.InteropServices;

namespace TommoJProductions.MoControlsV2.MouseInput {

    public enum INPUTTYPE : uint {
        MOUSE    = 0,
        KEYBOARD = 1,
        HARDWARE = 2,
    }

    public enum MOUSEEVENTF : uint {
        MOVE       = 0x00000001,
        LEFTDOWN   = 0x00000002,
        LEFTUP     = 0x00000004,
        RIGHTDOWN  = 0x00000008,
        RIGHTUP    = 0x00000010,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP   = 0x00000040,
        XDOWN      = 0x00000080,
        XUP        = 0x00000100,
        WHEEL      = 0x00000800,
        HWHEEL     = 0x00001000,
        ABSOLUTE   = 0x00008000,
    }

    public enum KEYEVENTF : uint {
        KEYEVENTF_EXTENDEDKEY = 0x00000001,
        KEYEVENTF_KEYUP       = 0x00000002,
        KEYEVENTF_SCANCODE    = 0x00000004,
        KEYEVENTF_UNICODE     = 0x00000008,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT {
        public int dx;
        public int dy;
        public uint mouseData;
        public MOUSEEVENTF dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT {
        public ushort wVk;
        public ushort wScan;
        public KEYEVENTF dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion {
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT {
        public INPUTTYPE type;
        public InputUnion u;

        public static readonly int Size = Marshal.SizeOf(typeof(INPUT));
    }
}
