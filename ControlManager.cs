using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HutongGames.PlayMaker;
using Harmony;
using MSCLoader;
using TommoJProductions.MoControlsV2.XInput;
using TommoJProductions.MoControlsV2.MouseInput;

namespace TommoJProductions.MoControlsV2 {
    public enum PLAYER_MODE {
        FOOT_MODE,
        DRIVING_MODE,
    }
    public struct Control_Input {
        public string name;
        public XINPUT_GAMEPAD_INPUT modifier;
        public XINPUT_GAMEPAD_INPUT input;

        public void set_name(string n) {
            name = n;
        }
        public void set_modifier(XINPUT_GAMEPAD_INPUT c) {
            modifier = c;
        }
        public void set_input(XINPUT_GAMEPAD_INPUT c) {
            input = c;
        }
    }
    public class Control_Manager : MonoBehaviour {        
        public static Control_Input[] foot_controls;
        public static Control_Input[] driving_controls;
        public static Control_Input[] current_controls {
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

        public static readonly string[] foot_controls_blacklist = new string[] {
            "Left", "Right", "ThrottleOn", "ThrottleOff", "BrakeOn", "BrakeOff", "ClutchOn", "ClutchOff", "StartEngine", "ShiftUp",
            "ShiftDown", "reverse", "neutral", "first", "second", "third", "fourth", "fifth", "sixth", "Handbrake1", "Handbrake",
            "IndicatorLeft", "IndicatorRight", "Range", "HighBeam", "Wipers", "Boost", "PlayerLeft", "PlayerRight", "PlayerUp",
            "PlayerDown", "ReachRight", "Horizontal", "Throttle", "Brake", "Clutch", "Vertical", "Up", "Down",

            /* MWC only */
            "ACC", "Ignition", "Lift1Up", "Lift1Down", "Lift2Up", "Lift2Down", "LightsOff", "LightsPark", "LightsDrive", "HiBeamToggle",
            "HiBeamFlash", "Washer", "IndicatorReturn", 
        };

        public static readonly string[] driving_controls_blacklist = new string[] {
            "ThrottleOn", "ThrottleOff", "BrakeOn", "BrakeOff", "ClutchOn", "ClutchOff", "StartEngine", "Handbrake1",
            "PlayerLeft", "PlayerRight", "PlayerUp", "PlayerDown", "Jump", "Run", "Crouch", "Push", "PlayerVertical",
            "PlayerHorizontal", "Vertical", "Up", "Down", "Left", "Right", "Urinate", "reverse", "neutral", "first",
            "second", "third", "fourth", "fifth", "sixth"
        };

        private static GameObject m_player;
        private static FsmBool m_player_in_menu;
        private static FsmString m_player_vehicle;
        private static PlayMakerFSM m_pick_up;
        private static PlayMakerFSM m_select_item;
        private static FsmBool m_hand_empty;
        private static XInput_Gamepad m_controller;
        private static Mouse_Emulator m_mouse_emulator;
        private static List<string> m_control_names;
        private static PLAYER_MODE m_player_mode;
        private static Camera_Manager m_camera_manager;
        private static PlayMakerFSM m_running;
        private static FsmBool m_run;

        public static XInput_Gamepad controller => m_controller;
        public static PLAYER_MODE player_mode => m_player_mode;
        public static string player_vehicle => m_player_vehicle.Value;
        public static bool hand_empty => m_hand_empty.Value;
        public static bool hand_mode => m_pick_up.Active;
        public static List<string> control_names => m_control_names;
        public static Mouse_Emulator mouse_emulator => m_mouse_emulator;
        public static Camera_Manager camera_manager => m_camera_manager;

        public static bool player_in_menu { 
            get => m_player_in_menu.Value; 
            set => m_player_in_menu.Value = value;
        }

        public static Dictionary<XINPUT_GAMEPAD_INPUT, XINPUT_GAMEPAD_INPUT[]> context_dic = new Dictionary<XINPUT_GAMEPAD_INPUT, XINPUT_GAMEPAD_INPUT[]>() {
            { XINPUT_GAMEPAD_INPUT.LS_X, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LS_LEFT, XINPUT_GAMEPAD_INPUT.LS_RIGHT } },
            { XINPUT_GAMEPAD_INPUT.LS_LEFT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LS_X } },
            { XINPUT_GAMEPAD_INPUT.LS_RIGHT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LS_X } },

            { XINPUT_GAMEPAD_INPUT.LS_Y, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LS_UP, XINPUT_GAMEPAD_INPUT.LS_DOWN } },
            { XINPUT_GAMEPAD_INPUT.LS_UP, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LS_Y } },
            { XINPUT_GAMEPAD_INPUT.LS_DOWN, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LS_Y } },

            { XINPUT_GAMEPAD_INPUT.RS_X, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.RS_LEFT, XINPUT_GAMEPAD_INPUT.RS_RIGHT } },
            { XINPUT_GAMEPAD_INPUT.RS_LEFT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.RS_X } },
            { XINPUT_GAMEPAD_INPUT.RS_RIGHT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.RS_X } },

            { XINPUT_GAMEPAD_INPUT.RS_Y, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.RS_UP, XINPUT_GAMEPAD_INPUT.RS_DOWN } },
            { XINPUT_GAMEPAD_INPUT.RS_UP, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.RS_Y } },
            { XINPUT_GAMEPAD_INPUT.RS_DOWN, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.RS_Y } },

            { XINPUT_GAMEPAD_INPUT.DPAD_X, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.DPAD_LEFT, XINPUT_GAMEPAD_INPUT.DPAD_RIGHT } },
            { XINPUT_GAMEPAD_INPUT.DPAD_LEFT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.DPAD_X } },
            { XINPUT_GAMEPAD_INPUT.DPAD_RIGHT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.DPAD_X } },

            { XINPUT_GAMEPAD_INPUT.DPAD_Y, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.DPAD_UP, XINPUT_GAMEPAD_INPUT.DPAD_DOWN } },
            { XINPUT_GAMEPAD_INPUT.DPAD_UP, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.DPAD_Y } },
            { XINPUT_GAMEPAD_INPUT.DPAD_DOWN, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.DPAD_Y } },

            { XINPUT_GAMEPAD_INPUT.Trigger_Axis, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.LT, XINPUT_GAMEPAD_INPUT.RT } },
            { XINPUT_GAMEPAD_INPUT.LT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.Trigger_Axis } },
            { XINPUT_GAMEPAD_INPUT.RT, new XINPUT_GAMEPAD_INPUT[] { XINPUT_GAMEPAD_INPUT.Trigger_Axis } },
        };
#if FFB
        public static bool ffb;
        public static string ffb_vehicle;
        public static Drivetrain ffb_drivetrain;
        public static CarDynamics ffb_dynamics;
        public static Axles ffb_axles;

        public static bool ffb_opt_default;
        public static bool ffb_opt_gear_change;
        public static bool ffb_opt_wheel_slip;
        public static bool ffb_opt_wheel_spin;
        public static bool ffb_opt_rpm_limiter;
        public static bool ffb_opt_shiver;
#endif
        private void Update() {
            m_controller.get_state();
            update_tool_mode_button();
            update_player_mode();
            update_in_game();
#if FFB
            update_force_feedback();
#endif
        }
        
        public void load() {
            add_cinput_inputs();
            get_cinput_control_list();
            hook_cinput();

            foot_controls = new Control_Input[m_control_names.Count];
            driving_controls = new Control_Input[m_control_names.Count];

            for (int i = 0; i < m_control_names.Count; ++i) {
                foot_controls[i].name = m_control_names[i];
                driving_controls[i].name = m_control_names[i];
            }

            m_controller = new XInput_Gamepad(XINPUT_GAMEPAD_INDEX.ONE);
            m_mouse_emulator = new Mouse_Emulator();
            m_camera_manager = new Camera_Manager();
            m_camera_manager.load();

            m_player_vehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle");
            if (m_player_vehicle == null) {
                MoControlsV2Mod.error("PlayerCurrentVehicle not found!");
                return;
            }

            m_player_in_menu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
            if (m_player_in_menu == null) {
                MoControlsV2Mod.error("PlayerInMenu not found!");
                return;
            }

            m_player = GameObject.Find("PLAYER");
            if (m_player == null) {
                MoControlsV2Mod.error("PLAYER not found!");
                return;
            }

            Transform t = m_player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand");
            if (t == null) {
                MoControlsV2Mod.error("1Hand_Assemble/Hand not found!");
                return;
            }

            m_pick_up = t.GetPlayMaker("PickUp");
            if (m_pick_up == null) {
                MoControlsV2Mod.error("Pickup FSM not found!");
                return;
            }

            m_hand_empty = m_pick_up.FsmVariables.FindFsmBool("HandEmpty");
            if (m_hand_empty == null) {
                MoControlsV2Mod.error("HandEmpty not found!");
                return;
            }

            t = m_player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/SelectItem");
            if (t == null) {
                MoControlsV2Mod.error("SelectItem not found!");
                return;
            }

            m_select_item = t.GetPlayMaker("Selection");
            if (m_select_item == null) {
                MoControlsV2Mod.error("Selection FSM not found!");
                return;
            }

            hook_toggle_run();

            set_default_controls();
            set_default_deadzones();
            set_default_sensitivity();
#if FFB
            set_default_force_feedback();
#endif
        }

        public static void set_control(PLAYER_MODE mode, string key, XINPUT_GAMEPAD_INPUT? c = null, XINPUT_GAMEPAD_INPUT? m = null) {
            switch (mode) {
                case PLAYER_MODE.FOOT_MODE:
                    for (int i = 0; i < foot_controls.Length; ++i) {
                        if (foot_controls[i].name == key) {
                            if (c != null) {
                                foot_controls[i].set_input(c.Value);
                            }
                            if (m != null) {
                                foot_controls[i].set_modifier(m.Value);
                            }
                            return;
                        }
                    }

                    MoControlsV2Mod.error($"Error could not find control: {key}");
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    for (int i = 0; i < driving_controls.Length; ++i) {
                        if (driving_controls[i].name == key) {
                            if (c != null) {
                                driving_controls[i].set_input(c.Value);
                            }
                            if (m != null) {
                                driving_controls[i].set_modifier(m.Value);
                            }
                            return;
                        }
                    }

                    MoControlsV2Mod.error($"Error could not find control: {key}");
                    break;
            }
        }
        public static void get_control(PLAYER_MODE mode, string key, out Control_Input c) {
            switch (mode) {
                case PLAYER_MODE.FOOT_MODE:
                    for (int i = 0; i < foot_controls.Length; ++i) {
                        if (foot_controls[i].name == key) {
                            c = foot_controls[i];
                            return;
                        }
                    }
                    c = default;
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    for (int i = 0; i < driving_controls.Length; ++i) {
                        if (driving_controls[i].name == key) {
                            c = driving_controls[i];
                            return;
                        }
                    }
                    break;
            }
            c = default;
        }

        private void update_player_mode() {
            PLAYER_MODE new_player_mode = m_player_vehicle.Value == "" ? PLAYER_MODE.FOOT_MODE : PLAYER_MODE.DRIVING_MODE;
            if (m_player_mode != new_player_mode) {
                m_player_mode = new_player_mode;
            }
        }
        private void update_in_game() {
            if (!m_player_in_menu.Value) {
                m_mouse_emulator.update_mouse_buttons();
                m_mouse_emulator.update_mouse_scroll();
            }
        }
        private void update_tool_mode_button() {
            if (m_controller.state.is_connected) {
                if (Control_Manager.get_input_down("ToolMode")) {
                    if (hand_mode) {
                        m_select_item.SendEvent("ITEM2");
                    }
                    else {
                        m_select_item.SendEvent("ITEM1");
                    }
                }
            }
        }
#if FFB
        private XInput_Gamepad_Vibration ffb_state;

        private void update_force_feedback() {
            if (ffb && m_controller.state.is_connected) {

                switch (m_player_mode) {
                    case PLAYER_MODE.FOOT_MODE:
                        if (ffb_vehicle != null) {
                            ffb_vehicle = null;
                            MoControlsV2Mod.log($"FFB Vehicle: {ffb_vehicle} unset");

                            ffb_state.left = 0;
                            ffb_state.right = 0;
                            m_controller.set_state(ffb_state);
                        }
                        break;
                    case PLAYER_MODE.DRIVING_MODE:
                        if (ffb_vehicle != m_player_vehicle.Value) {

                            /* Find car dynamics */
                            ffb_dynamics = m_player.transform.GetComponentInParent<CarDynamics>();
                            if (ffb_dynamics == null) {
                                MoControlsV2Mod.error($"{ffb_vehicle}: CarDynamics not found!");
                                break;
                            }

                            /* Find drivetrain */
                            ffb_drivetrain = m_player.transform.GetComponentInParent<Drivetrain>();
                            if (ffb_drivetrain == null) {
                                MoControlsV2Mod.error($"{ffb_vehicle}: Drivetrain not found!");
                                break;
                            }

                            /* Find axles */
                            ffb_axles = m_player.transform.GetComponentInParent<Axles>();
                            if (ffb_axles == null) {
                                MoControlsV2Mod.error($"{ffb_vehicle}: Axles not found!");
                                break;
                            }

                            ffb_vehicle = m_player_vehicle.Value;
                            MoControlsV2Mod.log($"FFB Vehicle: {ffb_vehicle} set");
                        }

                        if (ffb_vehicle != null) {
                            ffb_state.left = 0;
                            ffb_state.right = 0;

                            if (ffb_opt_gear_change) {
                                float_to_rumble(ffb_gear_change(), ref ffb_state.left);
                            }
                            if (ffb_opt_rpm_limiter) {
                                float_to_rumble(ffb_rpm_limiter(), ref ffb_state.right);
                            }
                            if (ffb_opt_wheel_slip) {
                                vector2_to_rumble(ffb_wheel_slip(), ref ffb_state);
                            }
                            if (ffb_opt_wheel_spin) {
                                vector2_to_rumble(ffb_wheel_spin(), ref ffb_state);
                            }

                            m_controller.set_state(ffb_state);
                        }

                        break;
                }
            }
        }

        private void float_to_rumble(float value, ref ushort rumble) {
            rumble += (ushort)((float)ushort.MaxValue * value);
        }
        private void vector2_to_rumble(Vector2 value, ref XInput_Gamepad_Vibration rumble) {
            float_to_rumble(value.x, ref rumble.left);
            float_to_rumble(value.y, ref rumble.right);
        }

        private Vector2 ffb_wheel_slip() {
            Vector2 v2 = default;
            for (int i = 0; i < ffb_axles.allWheels.Length; ++i) {
                WheelPos pos = ffb_axles.allWheels[i].wheelPos;
                switch (pos) {
                    case WheelPos.FRONT_LEFT:
                        v2.x += ffb_axles.allWheels[i].longitudinalSlip;
                        break;
                    case WheelPos.FRONT_RIGHT:
                        v2.y += ffb_axles.allWheels[i].longitudinalSlip;
                        break;
                }
            }
            return v2;
        }
        private Vector2 ffb_wheel_spin() {
            Vector2 v2 = default;
            for (int i = 0; i < ffb_drivetrain.poweredWheels.Length; ++i) {
                switch (ffb_drivetrain.poweredWheels[i].wheelPos) {
                    case WheelPos.FRONT_LEFT:
                    case WheelPos.REAR_LEFT:
                        v2.x += ffb_axles.allWheels[i].longitudinalSlip;
                        break;
                    case WheelPos.FRONT_RIGHT:
                    case WheelPos.REAR_RIGHT:
                        v2.y += ffb_axles.allWheels[i].longitudinalSlip;
                        break;
                }
            }
            return v2;
        }
        private float ffb_gear_change() {
            if (ffb_drivetrain.changingGear) {
                return ffb_drivetrain.rpm / ffb_drivetrain.maxRPM;
            }
            return 0;
        }
        private float ffb_rpm_limiter() {
            if (ffb_drivetrain.revLimiterTriggered) {
                return ffb_drivetrain.rpm / ffb_drivetrain.maxRPM;
            }
            return 0;
        }
        private float ffb_shiver() {
            return 0;
        }
#endif
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
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetKey", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetKey"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetKeyDown", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetKeyDown"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetKeyUp", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetKeyUp"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetButton", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetKey"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetButtonDown", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetKeyDown"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetButtonUp", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetKeyUp"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetAxis", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetAxis"));
            harmony.Patch(AccessTools.Method(typeof(cInput), "GetAxisRaw", new[] { typeof(string) }), prefix: new HarmonyMethod(typeof(Control_Manager_Hooks), "cinput_GetAxis"));
        }
        private void add_cinput_inputs() {
            cInput.SetKey("ToggleRun", "None");
            cInput.SetKey("ToolMode", "None");
            cInput.SetKey("MouseButton0", "None");
            cInput.SetKey("MouseButton1", "None");
            cInput.SetKey("MouseScroll-", "None");
            cInput.SetKey("MouseScroll+", "None");
            cInput.SetKey("MouseLookX", "None");
            cInput.SetKey("MouseLookY", "None");
        }
        
        private void hook_toggle_run() {
            m_running = m_player.GetPlayMaker("Running");
            if (m_running != null) {
                m_run = m_running.GetVariable<FsmBool>("Run");
                FsmState state = m_running.GetState("No run");
                if (state != null) {
                    m_running.FsmInject("No run", check_no_running, true, state.Actions.Length - 1, true);
                }
                state = m_running.GetState("Run");
                if (state != null) {
                    m_running.FsmInject("Run", check_running, true, state.Actions.Length - 1, true);
                }
            }
        }

        public static void set_default_controls() {
            for (int i = 0; i < foot_controls.Length; ++i) {
                foot_controls[i].input = XINPUT_GAMEPAD_INPUT.NONE;
                foot_controls[i].modifier = XINPUT_GAMEPAD_INPUT.NONE;
            }
            set_control(PLAYER_MODE.FOOT_MODE, "DrivingMode", XINPUT_GAMEPAD_INPUT.BACK);
            set_control(PLAYER_MODE.FOOT_MODE, "ToolMode", XINPUT_GAMEPAD_INPUT.START);
            set_control(PLAYER_MODE.FOOT_MODE, "PlayerHorizontal", XINPUT_GAMEPAD_INPUT.LS_X);
            set_control(PLAYER_MODE.FOOT_MODE, "PlayerVertical", XINPUT_GAMEPAD_INPUT.LS_Y);
            set_control(PLAYER_MODE.FOOT_MODE, "Jump", XINPUT_GAMEPAD_INPUT.Y);
            set_control(PLAYER_MODE.FOOT_MODE, "Crouch", XINPUT_GAMEPAD_INPUT.RS);
            set_control(PLAYER_MODE.FOOT_MODE, "ToggleRun", XINPUT_GAMEPAD_INPUT.LS);
            set_control(PLAYER_MODE.FOOT_MODE, "Use", XINPUT_GAMEPAD_INPUT.X);
            set_control(PLAYER_MODE.FOOT_MODE, "Finger", XINPUT_GAMEPAD_INPUT.DPAD_LEFT);
            set_control(PLAYER_MODE.FOOT_MODE, "Smoking", XINPUT_GAMEPAD_INPUT.DPAD_RIGHT);
            set_control(PLAYER_MODE.FOOT_MODE, "Push", XINPUT_GAMEPAD_INPUT.DPAD_DOWN);
            set_control(PLAYER_MODE.FOOT_MODE, "Zoom", XINPUT_GAMEPAD_INPUT.DPAD_UP);
            set_control(PLAYER_MODE.FOOT_MODE, "Urinate", XINPUT_GAMEPAD_INPUT.DPAD_UP, XINPUT_GAMEPAD_INPUT.RB);
            set_control(PLAYER_MODE.FOOT_MODE, "Watch", XINPUT_GAMEPAD_INPUT.DPAD_DOWN, XINPUT_GAMEPAD_INPUT.RB);
            set_control(PLAYER_MODE.FOOT_MODE, "Swear", XINPUT_GAMEPAD_INPUT.DPAD_LEFT, XINPUT_GAMEPAD_INPUT.RB);
            set_control(PLAYER_MODE.FOOT_MODE, "Drunk", XINPUT_GAMEPAD_INPUT.DPAD_RIGHT, XINPUT_GAMEPAD_INPUT.RB);
            set_control(PLAYER_MODE.FOOT_MODE, "ReachLeft", XINPUT_GAMEPAD_INPUT.LB);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseButton0", XINPUT_GAMEPAD_INPUT.A);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseButton1", XINPUT_GAMEPAD_INPUT.B);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseLookX", XINPUT_GAMEPAD_INPUT.RS_X);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseLookY", XINPUT_GAMEPAD_INPUT.RS_Y);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseScroll-", XINPUT_GAMEPAD_INPUT.LT);
            set_control(PLAYER_MODE.FOOT_MODE, "MouseScroll+", XINPUT_GAMEPAD_INPUT.RT);

            for (int i = 0; i < driving_controls.Length; ++i) {
                driving_controls[i].input = XINPUT_GAMEPAD_INPUT.NONE;
                driving_controls[i].modifier = XINPUT_GAMEPAD_INPUT.NONE;
            }
            set_control(PLAYER_MODE.DRIVING_MODE, "DrivingMode", XINPUT_GAMEPAD_INPUT.BACK);
            set_control(PLAYER_MODE.DRIVING_MODE, "ToolMode", XINPUT_GAMEPAD_INPUT.START);
            set_control(PLAYER_MODE.DRIVING_MODE, "Throttle", XINPUT_GAMEPAD_INPUT.RT);
            set_control(PLAYER_MODE.DRIVING_MODE, "Brake", XINPUT_GAMEPAD_INPUT.LT);
            set_control(PLAYER_MODE.DRIVING_MODE, "Clutch", XINPUT_GAMEPAD_INPUT.LB);
            set_control(PLAYER_MODE.DRIVING_MODE, "Horizontal", XINPUT_GAMEPAD_INPUT.LS_X);
            set_control(PLAYER_MODE.DRIVING_MODE, "ShiftUp", XINPUT_GAMEPAD_INPUT.B);
            set_control(PLAYER_MODE.DRIVING_MODE, "ShiftDown", XINPUT_GAMEPAD_INPUT.X);
            set_control(PLAYER_MODE.DRIVING_MODE, "Zoom", XINPUT_GAMEPAD_INPUT.DPAD_UP);
            set_control(PLAYER_MODE.DRIVING_MODE, "ReachLeft", XINPUT_GAMEPAD_INPUT.DPAD_LEFT);
            set_control(PLAYER_MODE.DRIVING_MODE, "ReachRight", XINPUT_GAMEPAD_INPUT.DPAD_RIGHT);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseButton0", XINPUT_GAMEPAD_INPUT.A);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseButton1", XINPUT_GAMEPAD_INPUT.RB);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseLookX", XINPUT_GAMEPAD_INPUT.RS_X);
            set_control(PLAYER_MODE.DRIVING_MODE, "MouseLookY", XINPUT_GAMEPAD_INPUT.RS_Y);
        }
        public static void set_default_deadzones() {
            m_controller.deadzone.ls = 15f;
            m_controller.deadzone.rs = 15f;
            m_controller.deadzone.lt = 10f;
            m_controller.deadzone.rt = 10f;
        }
        public static void set_default_sensitivity() {
            m_mouse_emulator.sensitivity_scroll = 30;
            m_camera_manager.controller_look_x.sensitivity = 65;
            m_camera_manager.controller_look_y.sensitivity = 65;
        }
#if FFB
        public static void set_default_force_feedback() {
            ffb = false;
            ffb_opt_default = false;
            ffb_opt_gear_change = true;
            ffb_opt_wheel_slip = true;
            ffb_opt_wheel_spin = true;
            ffb_opt_rpm_limiter = true;
        }
#endif

        public static bool is_modifier_down(Control_Input v) {
            if (v.modifier != XINPUT_GAMEPAD_INPUT.NONE) {
                /* Control has modifier */
                return m_controller.get_input_pressed(v.modifier);
            }
            else {
                /* Control has no modifier */
                for (int i = 0; i < current_controls.Length; ++i) {
                    /* Check all other controls with the same input for a modifier */
                    if (current_controls[i].input == v.input && m_controller.get_input_pressed(current_controls[i].modifier)) {
                        return false;
                    }
                    else {
                        if (context_dic.TryGetValue(v.input, out XINPUT_GAMEPAD_INPUT[] related_inputs)) {
                            for (int j = 0; j < related_inputs.Length; ++j) {
                                if (current_controls[i].input == related_inputs[j] && m_controller.get_input_pressed(current_controls[i].modifier)) {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }
        public static bool get_input(string description) {
            if (description != null) {
                for (int i = 0; i < current_controls.Length; ++i) {
                    if (current_controls[i].name == description) {
                        if (!is_modifier_down(current_controls[i])) {
                            return false;
                        }
                        return m_controller.get_input_pressed(current_controls[i].input);
                    }
                }
            }
            return false;
        }
        public static float get_axis(string description) {
            if (description != null) {
                for (int i = 0; i < current_controls.Length; ++i) {
                    if (current_controls[i].name == description) {
                        if (!is_modifier_down(current_controls[i])) {
                            return 0;
                        }
                        return m_controller.get_input_axis(current_controls[i].input);
                    }
                }
            }
            return 0;
        }
        public static bool get_input_down(string description) {
            if (description != null) {
                for (int i = 0; i < current_controls.Length; ++i) {
                    if (current_controls[i].name == description) {
                        if (!is_modifier_down(current_controls[i])) {
                            return false;
                        }
                        return m_controller.get_input_down(current_controls[i].input);
                    }
                }
            }
            return false;
        }
        public static bool get_input_up(string description) {
            if (description != null) {
                for (int i = 0; i < current_controls.Length; ++i) {
                    if (current_controls[i].name == description) {
                        return m_controller.get_input_up(current_controls[i].input);
                    }
                }
            }
            return false;
        }

        void check_no_running() {
            if (get_input_down("ToggleRun") || get_input("Run")) {
                m_running.SendEvent("RUN");
                m_run.Value = true;
            }
        }
        void check_running() {
            if (get_input_down("ToggleRun") || get_input_up("Run")) {
                m_running.SendEvent("WALK");
                m_run.Value = false;
            }
        }
    }

    public static class Control_Manager_Hooks {

        private static bool get_key(string description, out bool result) {
            if (Control_Manager.get_input(description)) {
                result = true;
                return false; /* skip orignal method */
            }
            else {
                result = false;
                return true; /* goto orignal method */
            }
        }
        private static bool get_key_down(string description, out bool result) {
            if (Control_Manager.get_input_down(description)) {
                result = true;
                return false; /* skip orignal method */
            }
            else {
                result = false;
                return true; /* goto orignal method */
            }
        }
        private static bool get_key_up(string description, out bool result) {
            if (Control_Manager.get_input_up(description)) {
                result = true;
                return false; /* skip orignal method */
            }
            else {
                result = false;
                return true; /* goto orignal method */
            }
        }
        private static bool get_axis(string description, out float result) {
            result = Control_Manager.get_axis(description);
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
