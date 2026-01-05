using System.Runtime.InteropServices;
using UnityEngine;

namespace TommoJProductions.MoControlsV2.XInput {

    public enum XINPUT_GAMEPAD_INDEX : uint {
        ONE   = 0,
        TWO   = 1,
        THREE = 2,
        FOUR  = 3
    }

    public enum XINPUT_BUTTON_STATE : uint {
        RELEASED = 0,
        PRESSED  = 1,
    }
    
    public enum XINPUT_BUTTON_MASKS : ushort {
        DPAD_UP    = 0x0001,
        DPAD_DOWN  = 0x0002,
        DPAD_LEFT  = 0x0004,
        DPAD_RIGHT = 0x0008,
        START      = 0x0010,
        BACK       = 0x0020,
        LS         = 0x0040,
        RS         = 0x0080,
        LB         = 0x0100,
        RB         = 0x0200,
        A          = 0x1000,
        B          = 0x2000,
        X          = 0x4000,
        Y          = 0x8000
    }
    
    public enum XINPUT_CAPS : ushort {
        FFB_SUPPORTED   = 0x0001,
        WIRELESS        = 0x0002,
        VOICE_SUPPORTED = 0x0004,
        PMD_SUPPORTED   = 0x0008,
        NO_NAVIGATION   = 0x0010
    }

    public enum XINPUT_DEVTYPE : byte {
        GAMEPAD = 0x01
    }

    public enum XINPUT_DEVSUBTYPE : byte {
        UNKNOWN      = 0x00,
        GAMEPAD      = 0x01,
        WHEEL        = 0x02,
        ARCADE_STICK = 0x03,
        FLIGHT_STICK = 0x04,
        ARCADE_PAD   = 0x13,
    }

    public enum XINPUT_BATTERY_TYPE : byte {
        DISCONNECTED = 0x00, /* The device is not connected. */
        WIRED        = 0x01, /* The device is a wired device and does not have a battery. */
        ALKALINE     = 0x02, /* The device has an alkaline battery. */
        NIMH         = 0x03, /* The device has a nickel metal hydride battery. */
        UNKNOWN      = 0xFF, /* The device has an unknown battery type. */
    }

    public enum XINPUT_BATTERY_LEVEL : byte {
        EMPTY  = 0x00,
        LOW    = 0x01,
        MEDIUM = 0x02,
        FULL   = 0x03,
    }

    public enum XINPUT_GAMEPAD_INPUT {
        NONE,
        A,
        B,
        X,
        Y,
        START,
        BACK,
        LB,
        RB,
        LT,
        RT,
        Trigger_Axis,
        LS,
        LS_LEFT,
        LS_RIGHT,
        LS_UP,
        LS_DOWN,
        LS_X,
        LS_Y,
        RS,
        RS_LEFT,
        RS_RIGHT,
        RS_UP,
        RS_DOWN,
        RS_X,
        RS_Y,
        DPAD_LEFT,
        DPAD_RIGHT,
        DPAD_UP,
        DPAD_DOWN,
        DPAD_X,
        DPAD_Y,
        COUNT,
    }

    public struct XInput_Gamepad_Buttons {
        public XINPUT_BUTTON_STATE start;
        public XINPUT_BUTTON_STATE back;
        public XINPUT_BUTTON_STATE ls;
        public XINPUT_BUTTON_STATE rs;
        public XINPUT_BUTTON_STATE lb;
        public XINPUT_BUTTON_STATE rb;
        public XINPUT_BUTTON_STATE a;
        public XINPUT_BUTTON_STATE b;
        public XINPUT_BUTTON_STATE x;
        public XINPUT_BUTTON_STATE y;
    }

    public struct XInput_Gamepad_DPad {
        public XINPUT_BUTTON_STATE up;
        public XINPUT_BUTTON_STATE down;
        public XINPUT_BUTTON_STATE left;
        public XINPUT_BUTTON_STATE right;
    }

    public struct XInput_Gamepad_Thumbsticks {
        public Vector2 left;
        public Vector2 right;
    }

    public struct XInput_Gamepad_Triggers {
        public float left;
        public float right;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInput_Gamepad_Raw_State {
        public uint packet_number;
        public Gamepad gamepad;

        [StructLayout(LayoutKind.Sequential)]
        public struct Gamepad {
            public ushort buttons;
            public byte lt;
            public byte rt;
            public short ls_x;
            public short ls_y;
            public short rs_x;
            public short rs_y;
        }
    }

    public struct XInput_Gamepad_Deadzone {
        public float ls;
        public float rs;
        public float lt;
        public float rt;
    }

    public struct XInput_Gamepad_State {
        public bool is_connected;
        public uint packet_number;
        public XInput_Gamepad_Buttons buttons;
        public XInput_Gamepad_DPad dpad;
        public XInput_Gamepad_Thumbsticks thumbsticks;
        public XInput_Gamepad_Triggers triggers;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInput_Gamepad_Vibration {
        public ushort left;
        public ushort right;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInput_Capabilities {
        public XINPUT_DEVTYPE type;
        public XINPUT_DEVSUBTYPE sub_type;
        public XINPUT_CAPS flags;
        public XInput_Gamepad_Raw_State raw_state;
        public XInput_Gamepad_Vibration vibration;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInput_Battery_Information {
        public XINPUT_BATTERY_TYPE type;
        public XINPUT_BATTERY_LEVEL level;
    }
}
