using System;
using System.Runtime.InteropServices;
namespace TommoJProductions.MoControlsV2.MouseInput {

    public enum MOUSE_EVENT : uint {
        ABSOLUTE = 0x8000,
        HWHEEL = 0x01000,
        MOVE = 0x0001,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        WHEEL = 0x0800,
        XDOWN = 0x0080,
        XUP = 0x0100
    }

    public enum MOUSE_EVENT_CLICK_BUTTON0 : uint {
        DOWN = 0x0002,
        UP   = 0x0004
    }

    public enum MOUSE_EVENT_CLICK_BUTTON1 : uint {
        DOWN = 0x0008,
        UP   = 0x0010
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

        public static int Size => Marshal.SizeOf(typeof(Input));

    }
}
