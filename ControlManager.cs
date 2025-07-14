using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HutongGames.PlayMaker;
using Harmony;
using MSCLoader;
using TommoJProductions.MoControlsV2.XInput;
using TommoJProductions.MoControlsV2.MouseInput;

using static TommoJProductions.MoControlsV2.ControlManager;

namespace TommoJProductions.MoControlsV2 {
    public enum PLAYER_MODE {
        FOOT_MODE,
        DRIVING_MODE,
    }

    public class ControlManager : MonoBehaviour {

        public static Dictionary<string, XInputGamepad.INPUT> foot_controls;
        public static Dictionary<string, XInputGamepad.INPUT> driving_controls;
        public static Dictionary<string, XInputGamepad.INPUT> current_controls {
            get {
                switch (player_mode) {
                    case PLAYER_MODE.FOOT_MODE:
                        return foot_controls;
                    case PLAYER_MODE.DRIVING_MODE:
                        return driving_controls;
                    default:
                        return null;
                }
            }
        }

        public static readonly List<string> foot_controls_blacklist = new List<string> {
            "Left", "Right", "ThrottleOn", "ThrottleOff", "BrakeOn", "BrakeOff", "ClutchOn", "ClutchOff", "StartEngine", "ShiftUp",
            "ShiftDown", "reverse", "neutral", "first", "second", "third", "fourth", "fifth", "sixth", "Handbrake1", "Handbrake",
            "IndicatorLeft", "IndicatorRight", "Range", "HighBeam", "Wipers", "Boost", "PlayerLeft", "PlayerRight", "PlayerUp",
            "PlayerDown", "ReachRight", "Horizontal", "Throttle", "Brake", "Clutch", "Vertical", "Up", "Down"
        };

        public static readonly List<string> driving_controls_blacklist = new List<string> {
            "ThrottleOn", "ThrottleOff", "BrakeOn", "BrakeOff", "ClutchOn", "ClutchOff", "StartEngine", "Handbrake1",
            "PlayerLeft", "PlayerRight", "PlayerUp", "PlayerDown", "Jump", "Run", "Crouch", "Push", "PlayerVertical",
            "PlayerHorizontal", "Vertical", "Up", "Down", "Left", "Right"
        };

        private static FsmBool m_player_in_menu;
        private static FsmBool m_player_stop;
        private static FsmString m_player_vehicle;
        private static PlayMakerFSM m_pick_up;
        private static PlayMakerFSM m_select_item;
        private static FsmBool m_hand_empty;
        private static XInputGamepad m_controller;
        private static MouseEmulator m_mouse_emulator;
        private static List<string> m_control_names;
        private static PLAYER_MODE m_player_mode;
        private static CameraManager m_camera_manager;

        public static XInputGamepad controller => m_controller;
        public static PLAYER_MODE player_mode => m_player_mode;
        public static string player_vehicle => m_player_vehicle.Value;
        public static bool hand_empty => m_hand_empty.Value;
        public static bool hand_mode => m_pick_up.Active;
        public static List<string> control_names => m_control_names;
        public static MouseEmulator mouse_emulator => m_mouse_emulator;
        public static CameraManager camera_manager => m_camera_manager;

        public static bool player_in_menu { 
            get => m_player_in_menu.Value; 
            set => m_player_in_menu.Value = value;
        }

        private void Update() {
            m_controller.get_state();
            m_mouse_emulator.update_mouse_buttons();
            m_mouse_emulator.update_mouse_scroll();
            update_tool_mode_button();
            update_player_mode();
            update_in_menu();
        }

        public void load() {
            add_cinput_inputs();
            get_cinput_control_list();
            hook_cinput();

            foot_controls = new Dictionary<string, XInputGamepad.INPUT>();
            driving_controls = new Dictionary<string, XInputGamepad.INPUT>();

            m_controller = new XInputGamepad(GAMEPAD_INDEX.ONE);
            m_mouse_emulator = new MouseEmulator();
            m_camera_manager = new CameraManager();
            m_camera_manager.load();
            m_player_vehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle");
            m_player_stop = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerStop");
            m_pick_up = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").gameObject.GetPlayMaker("PickUp");
            m_hand_empty = m_pick_up.FsmVariables.FindFsmBool("HandEmpty");
            m_select_item = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/SelectItem").gameObject.GetPlayMaker("Selection");
            m_player_in_menu = FsmVariables.GlobalVariables.GetFsmBool("PlayerInMenu");

            set_default_controls();
            set_default_deadzones();
            set_default_sensitivity();
        }

        public static void set_control(PLAYER_MODE mode, string key, XInputGamepad.INPUT c) {
            switch (mode) {
                case PLAYER_MODE.FOOT_MODE:
                    if (ControlManager.foot_controls.ContainsKey(key)) {
                        ControlManager.foot_controls[key] = c;
                    }
                    else {
                        ControlManager.foot_controls.Add(key, c);
                    }
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    if (ControlManager.driving_controls.ContainsKey(key)) {
                        ControlManager.driving_controls[key] = c;
                    }
                    else {
                        ControlManager.driving_controls.Add(key, c);
                    }
                    break;
            }
        }

        public static void get_control(PLAYER_MODE mode, string key, out XInputGamepad.INPUT c) {
            c = XInputGamepad.INPUT.NONE;
            switch (mode) {
                case PLAYER_MODE.FOOT_MODE:
                    ControlManager.foot_controls.TryGetValue(key, out c);
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    ControlManager.driving_controls.TryGetValue(key, out c);
                    break;
            }
        }

        private void update_player_mode() {
            PLAYER_MODE new_player_mode = m_player_vehicle.Value == "" ? PLAYER_MODE.FOOT_MODE : PLAYER_MODE.DRIVING_MODE;
            if (m_player_mode != new_player_mode) {
                m_player_mode = new_player_mode;
                on_player_mode_changed();
            }
        }
        private void update_in_menu() {
            if (m_player_in_menu.Value) {
                m_mouse_emulator.update_mouse_postion();
            }
        }
        public void update_tool_mode_button() {
            if (m_controller.state.is_connected) {
                if (ControlManager.get_input_down("ToolMode")) {
                    if (hand_mode) {
                        m_select_item.SendEvent("ITEM2");
                    }
                    else {
                        m_select_item.SendEvent("ITEM1");
                    }
                }
            }
        }

        private void get_cinput_control_list() {
            FieldInfo _inputName_fi = typeof(cInput).GetField("_inputName", (BindingFlags.Static | BindingFlags.NonPublic));
            FieldInfo _axisName_fi = typeof(cInput).GetField("_axisName", (BindingFlags.Static | BindingFlags.NonPublic));

            string[] _inputName = (string[])_inputName_fi.GetValue(null);
            string[] _axisName = (string[])_axisName_fi.GetValue(null);

            m_control_names = new List<string>();

            for (int i = 0; i < 250; ++i) {
                if (_inputName[i] != null && _inputName[i] != "null" && !m_control_names.Contains(_inputName[i])) {
                    m_control_names.Add(_inputName[i]);
                }
                if (_axisName[i] != null && _axisName[i] != "null" && !m_control_names.Contains(_axisName[i])) {
                    m_control_names.Add(_axisName[i]);
                }
            }
        }
        private void hook_cinput() {
            MoControlsV2Mod.log("Patching cInput");
            HarmonyInstance harmony = HarmonyInstance.Create("mo_controls.input");
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetKey", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetKey"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetKeyDown", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetKeyDown"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetKeyUp", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetKeyUp"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetButton", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetKey"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetButtonDown", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetKeyDown"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetButtonUp", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetKeyUp"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetAxis", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetAxis"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetAxisRaw", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(ControlManagerHooks), "cinput_GetAxis"));
        }

        private void add_cinput_inputs() {
            cInput.SetKey("ToolMode", "None");
            cInput.SetKey("MouseButton0", "None");
            cInput.SetKey("MouseButton1", "None");
            cInput.SetKey("MouseScroll-", "None");
            cInput.SetKey("MouseScroll+", "None");
            cInput.SetKey("MouseMoveX", "None");
            cInput.SetKey("MouseMoveY", "None");
            cInput.SetKey("MouseLookX", "None");
            cInput.SetKey("MouseLookY", "None");
        }

        public static void set_default_controls() {

            foot_controls.Clear();
            set_control(PLAYER_MODE.FOOT_MODE, "DrivingMode", XInputGamepad.INPUT.BACK);
            set_control(PLAYER_MODE.FOOT_MODE, "ToolMode", XInputGamepad.INPUT.START);
            set_control(PLAYER_MODE.FOOT_MODE, "PlayerHorizontal", XInputGamepad.INPUT.LS_X);
            set_control(PLAYER_MODE.FOOT_MODE, "PlayerVertical", XInputGamepad.INPUT.LS_Y);
            set_control(PLAYER_MODE.FOOT_MODE, "Jump", XInputGamepad.INPUT.Y);
            set_control(PLAYER_MODE.FOOT_MODE, "Crouch", XInputGamepad.INPUT.RS);
            set_control(PLAYER_MODE.FOOT_MODE, "Run", XInputGamepad.INPUT.LS);
            set_control(PLAYER_MODE.FOOT_MODE, "Use", XInputGamepad.INPUT.X);
            set_control(PLAYER_MODE.FOOT_MODE, "Finger", XInputGamepad.INPUT.DPAD_LEFT);
            set_control(PLAYER_MODE.FOOT_MODE, "Smoking", XInputGamepad.INPUT.DPAD_RIGHT);
            set_control(PLAYER_MODE.FOOT_MODE, "Push", XInputGamepad.INPUT.DPAD_DOWN);
            set_control(PLAYER_MODE.FOOT_MODE, "Zoom", XInputGamepad.INPUT.DPAD_UP);
            set_control(PLAYER_MODE.FOOT_MODE, "ReachLeft", XInputGamepad.INPUT.LB);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseButton0", XInputGamepad.INPUT.A);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseButton1", XInputGamepad.INPUT.B);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseMoveX", XInputGamepad.INPUT.RS_X);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseMoveY", XInputGamepad.INPUT.RS_Y);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseLookX", XInputGamepad.INPUT.RS_X);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseLookY", XInputGamepad.INPUT.RS_Y);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseScroll-", XInputGamepad.INPUT.LT);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseScroll+", XInputGamepad.INPUT.RT);

            driving_controls.Clear();
            set_control(PLAYER_MODE.DRIVING_MODE, "DrivingMode", XInputGamepad.INPUT.BACK);
            set_control(PLAYER_MODE.DRIVING_MODE, "ToolMode", XInputGamepad.INPUT.START);
            set_control(PLAYER_MODE.DRIVING_MODE, "Throttle", XInputGamepad.INPUT.RT);
            set_control(PLAYER_MODE.DRIVING_MODE, "Brake", XInputGamepad.INPUT.LT);
            set_control(PLAYER_MODE.DRIVING_MODE, "Clutch", XInputGamepad.INPUT.LB);
            set_control(PLAYER_MODE.DRIVING_MODE, "Horizontal", XInputGamepad.INPUT.LS_X);
            set_control(PLAYER_MODE.DRIVING_MODE, "ShiftUp", XInputGamepad.INPUT.B);
            set_control(PLAYER_MODE.DRIVING_MODE, "ShiftDown", XInputGamepad.INPUT.X);
            set_control(PLAYER_MODE.DRIVING_MODE, "Zoom", XInputGamepad.INPUT.DPAD_UP);
            set_control(PLAYER_MODE.DRIVING_MODE, "ReachLeft", XInputGamepad.INPUT.DPAD_LEFT);
            set_control(PLAYER_MODE.DRIVING_MODE, "ReachRight", XInputGamepad.INPUT.DPAD_RIGHT);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseButton0", XInputGamepad.INPUT.A);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseButton1", XInputGamepad.INPUT.RB);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseMoveX", XInputGamepad.INPUT.RS_X);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseMoveY", XInputGamepad.INPUT.RS_Y);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseLookX", XInputGamepad.INPUT.RS_X);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseLookY", XInputGamepad.INPUT.RS_Y);
        }
        public static void set_default_deadzones() {
            controller.deadzone.ls = 15f;
            controller.deadzone.rs = 15f;
            controller.deadzone.lt = 10f;
            controller.deadzone.rt = 10f;
        }
        public static void set_default_sensitivity() {
            mouse_emulator.sensitivity_x = 15;
            mouse_emulator.sensitivity_y = 15;
            mouse_emulator.sensitivity_scroll = 30;
            camera_manager.controller_look_x.sensitivity = 65;
            camera_manager.controller_look_y.sensitivity = 65;
        }

        public static bool get_input(string description) {
            if (description != null) {
                if (current_controls.TryGetValue(description, out XInputGamepad.INPUT v)) {
                    return controller.get_input(v) != 0;                    
                }
            }
            return false;
        }
        public static float get_axis(string description) {
            if (description != null) {
                if (current_controls.TryGetValue(description, out XInputGamepad.INPUT c)) {
                    return controller.get_input(c);      
                }
            }
            return 0;
        }
        public static bool get_input_down(string description) {
            if (description != null) {
                if (current_controls.TryGetValue(description, out XInputGamepad.INPUT v)) {
                    return controller.get_input_down(v);                    
                }
            }
            return false;
        }
        public static bool get_input_up(string description) {
            if (description != null) {
                if (current_controls.TryGetValue(description, out XInputGamepad.INPUT v)) {
                    return controller.get_input_up(v);                    
                }
            }
            return false;
        }

        /* Callbacks/Events */
        private void on_player_mode_changed() {
            MoControlsV2Mod.log("Control Mode changed: " + m_player_mode);
        }
    }

    public static class ControlManagerHooks {

        private static bool get_key(string description, out bool result) {
            if (ControlManager.get_input(description)) {
                result = true;
                return false; /* skip orignal method */
            }
            else {
                result = false;
                return true; /* goto orignal method */
            }
        }
        private static bool get_key_down(string description, out bool result) {
            if (ControlManager.get_input_down(description)) {
                result = true;
                return false; /* skip orignal method */
            }
            else {
                result = false;
                return true; /* goto orignal method */
            }
        }
        private static bool get_key_up(string description, out bool result) {
            if (ControlManager.get_input_up(description)) {
                result = true;
                return false; /* skip orignal method */
            }
            else {
                result = false;
                return true; /* goto orignal method */
            }
        }
        private static bool get_axis(string description, out float result) {
            result = ControlManager.get_axis(description);
            if (result != 0.0f) {
                return false; /* skip orignal method */
            }
            else {
                return true; /* goto orignal method */
            }
        }

        /* Hooks */
        private static bool cinput_GetKey(string description, out bool __result) {
            /* bool cInput:GetKey(string description) */
            return get_key(description, out __result);
        }
        private static bool cinput_GetKeyDown(string description, out bool __result) {
            /* bool cInput:GetKeyDown(string description) */
            return get_key_down(description, out __result);
        }
        private static bool cinput_GetKeyUp(string description, out bool __result) {
            /* bool cInput:GetKeyUp(string description) */
            return get_key_up(description, out __result);
        }
        private static bool cinput_GetButton(string description, out bool __result) {
            /* bool cInput:GetButton(string description) */
            return get_key(description, out __result);
        }
        private static bool cinput_GetButtonDown(string description, out bool __result) {
            /* bool cInput:GetButtonDown(string description) */
            return get_key_down(description, out __result);
        }
        private static bool cinput_GetButtonUp(string description, out bool __result) {
            /* bool cInput:GetButtonUp(string description) */
            return get_key_up(description, out __result);
        }
        private static bool cinput_GetAxis(string description, out float __result) {
            /* float cInput:GetAxis(string description) */
            /* float cInput:GetAxisRaw(string description) */
            return get_axis(description, out __result);
        }
    }
}
