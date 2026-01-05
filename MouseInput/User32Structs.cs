using System;
using System.Runtime.InteropServices;

namespace TommoJProductions.MoControlsV2.MouseInput {

    public enum MOUSE_EVENT : uint {
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

    public enum MOUSE_EVENT_BUTTON : uint {
        NULL     = 0x00000000,
        XBUTTON1 = 0x00000001,
        XBUTTON2 = 0x00000002
    }

    public struct Point {
        public int x;
        public int y;
    }

    public struct PointF {
        public float x;
        public float y;
        public float sqr_mag => x * x + y * y;
    }

    public struct MouseInput {
        public int x;
        public int y;
        public uint data;
        public uint flags;
        public uint time;
        public IntPtr extra_info;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Input {
        public uint type;
        public MouseInput mi;
        public static int size => Marshal.SizeOf(typeof(Input));
    }
}
