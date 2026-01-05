using System.Runtime.InteropServices;

namespace TommoJProductions.MoControlsV2.XInput {
    public static class XINPUT_IMPORTS {

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        public static extern uint xinput_get_state(XINPUT_GAMEPAD_INDEX index, out XInput_Gamepad_Raw_State state);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")] 
        public static extern void xinput_set_state(XINPUT_GAMEPAD_INDEX index, XInput_Gamepad_Vibration vibration);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputEnable")]
        public static extern uint xinput_enable(bool enable);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetCapabilities")]
        public static extern uint xinput_get_capabilities(XINPUT_GAMEPAD_INDEX index, uint flags, out XInput_Capabilities capabilities);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetBatteryInformation")]
        public static extern uint xinput_get_battery_infomation(XINPUT_GAMEPAD_INDEX index, XINPUT_DEVTYPE dev_type, out XInput_Battery_Information battery_info);
    }
}
