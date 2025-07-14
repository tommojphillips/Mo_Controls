using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace TommoJProductions.MoControlsV2.XInput {

    public enum GAMEPAD_INDEX : uint {
        ONE   = 0,
        TWO   = 1,
        THREE = 2,
        FOUR  = 3
    }

    public enum BUTTON_STATE : uint {
        RELEASED = 0,
        PRESSED  = 1,
    }
    
    public enum BUTTON_MASKS : ushort {
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

    public struct Gamepad_Buttons {
        public BUTTON_STATE start;
        public BUTTON_STATE back;
        public BUTTON_STATE ls;
        public BUTTON_STATE rs;
        public BUTTON_STATE lb;
        public BUTTON_STATE rb;
        public BUTTON_STATE a;
        public BUTTON_STATE b;
        public BUTTON_STATE x;
        public BUTTON_STATE y;
    }

    public struct Gamepad_DPad {
        public BUTTON_STATE up;
        public BUTTON_STATE down;
        public BUTTON_STATE left;
        public BUTTON_STATE right;
    }

    public struct Gamepad_Thumbsticks {
        public Vector2 left;
        public Vector2 right;
    }

    public struct Gamepad_Triggers {
        public float left;
        public float right;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Gamepad_Raw_State {
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

    public struct Gamepad_Deadzone {
        public float ls;
        public float rs;
        public float lt;
        public float rt;
    }

    public struct Gamepad_State {
        public bool is_connected;
        public uint packet_number;
        public Gamepad_Buttons buttons;
        public Gamepad_DPad dpad;
        public Gamepad_Thumbsticks thumbsticks;
        public Gamepad_Triggers triggers;
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
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Gamepad_Vibration {
        ushort left;
        ushort right;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInput_Capabilities {
        XINPUT_DEVTYPE type;
        XINPUT_DEVSUBTYPE sub_type;
        XINPUT_CAPS flags;
        Gamepad_Raw_State raw_state;
        Gamepad_Vibration vibration;
    }
}
