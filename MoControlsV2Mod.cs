﻿using System.IO;
using UnityEngine;
using MSCLoader;
using TommoJProductions.MoControlsV2.XInput;
using System;

namespace TommoJProductions.MoControlsV2 {
    public struct MoControlsV2Assets {
        public GameObject UI_prefab;
        public Sprite[] spirtes;

        public MoControlsV2Assets(int sprite_count) {
            UI_prefab = null;
            spirtes = new Sprite[sprite_count];
        }
    }

    public static class MoControlsV2Util {
        public static void load<T>(Mod mod, string key, ref T output, Func<T, bool> func) {
            if (SaveLoad.ValueExists(mod, key)) {
                T v = SaveLoad.ReadValue<T>(mod, key);
                if (func?.Invoke(v) ?? true) {
                    output = v;
                }
            }
        }
    }

    public class MoControlsV2Mod : Mod {
        // Project start, 11.03.2023; Comeback, 09.07.2025

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Author => "tommojphillips";
        public override string Version => VersionInfo.version;
        public override string Description => "Mo'Controls";

        private GameObject manager_go;
        private ControlManager control_manager;
        private UIManager ui_manager;

        public static Mod mod;

        public override void ModSetup() {
            mod = this;
            SetupFunction(Setup.OnLoad, on_load);
        }

        private void on_load() {
            log("Loading");
            load_commands();
            create_manager_gameobject();
            load_control_manager();
            load_settings();
            load_ui_manager();
            log("Loaded");
        }

        private void create_manager_gameobject() {
            manager_go = new GameObject("Mo_Controls_GO");
        }

        private void load_ui_manager() {
            ui_manager = manager_go.AddComponent<UIManager>();
            ui_manager.load();
        }

        private void load_control_manager() {
            control_manager = manager_go.AddComponent<ControlManager>();
            control_manager.load();
        }

        private void load_commands() {
            ConsoleCommand.Add(new WriteCinputExternInputsCommand());
        }

        private static void validate_controller_input(int index, out XInputGamepad.INPUT c) {
            if (index < (int)XInputGamepad.INPUT.COUNT && index >= 0) {
                c = (XInputGamepad.INPUT)index;
            }
            else {
                c = XInputGamepad.INPUT.NONE;
            }
        }

        public static void load_settings() {
            /* 
               We save Gamepad.Input as an INT so that it is modifable in the MSCEditor. 
               ES2 complains if we try to load a value as a different type. So we must load it back as an INT.
            */
            string key;
            for (int i = 0; i < ControlManager.control_names.Count; ++i) {

                /* Load foot controls */
                key = $"foot_{ControlManager.control_names[i]}";
                if (SaveLoad.ValueExists(mod, key)) {
                    int index = SaveLoad.ReadValue<int>(mod, key);
                    validate_controller_input(index, out XInputGamepad.INPUT c);
                    ControlManager.set_control(PLAYER_MODE.FOOT_MODE, ControlManager.control_names[i], c);
                }

                /* Load driving controls */
                key = $"driving_{ControlManager.control_names[i]}";
                if (SaveLoad.ValueExists(mod, key)) {
                    int index = SaveLoad.ReadValue<int>(mod, key);
                    validate_controller_input(index, out XInputGamepad.INPUT c);
                    ControlManager.set_control(PLAYER_MODE.DRIVING_MODE, ControlManager.control_names[i], c);
                }
            }

            /* Load Deadzones */
            MoControlsV2Util.load<float>(mod, "ls_deadzone", ref ControlManager.controller.deadzone.ls, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "rs_deadzone", ref ControlManager.controller.deadzone.rs, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "lt_deadzone", ref ControlManager.controller.deadzone.lt, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "rt_deadzone", ref ControlManager.controller.deadzone.rt, delegate (float v) { return v >= 0; });

            /* Load Sensitivity */
            MoControlsV2Util.load<float>(mod, "mouse_look_x_sensitivity", ref ControlManager.camera_manager.controller_look_x.sensitivity, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "mouse_look_y_sensitivity", ref ControlManager.camera_manager.controller_look_y.sensitivity, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "mouse_move_x_sensitivity", ref ControlManager.mouse_emulator.sensitivity_x, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "mouse_move_y_sensitivity", ref ControlManager.mouse_emulator.sensitivity_y, delegate (float v) { return v >= 0; });
            MoControlsV2Util.load<float>(mod, "mouse_scroll_sensitivity", ref ControlManager.mouse_emulator.sensitivity_scroll, delegate (float v) { return v >= 0; });
        }

        public static void save_setting<T>(string key, T v) {
            SaveLoad.WriteValue(mod, key, v);
        }
        
        private int compare_byte_array(byte[] a1, byte[] a2) {
            if (a1.Length == a2.Length) {
                for (int i = 0; i < a1.Length; ++i) {
                    if (a1[i] != a2[i]) {
                        return 1;
                    }
                }
            }
            else {
                return 1;
            }
            return 0;
        }

        internal static void log(string message) {
            ModConsole.Log("<b>Mo'Controls V2</b>: " + message);
        }

        internal static void error(string message) {
            ModConsole.Log("<b>Mo'Controls V2</b>: " + message);
        }
    }
}
