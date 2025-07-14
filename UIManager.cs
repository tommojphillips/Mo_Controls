using MSCLoader;
using System;
using TommoJProductions.MoControlsV2.MouseInput;
using TommoJProductions.MoControlsV2.XInput;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TommoJProductions.MoControlsV2 {
    public class UIManager : MonoBehaviour {

        private MoControlsV2Assets assets;
        private ChangeInput change_input;
        private static MENU_ITEMS selected_tab;
        private GameObject ui_go;
        private GameObject foot_controls_tab;
        private GameObject driving_controls_tab;
        private GameObject settings_tab;
        private GameObject context_settings;

        struct slider_struct {
            public Transform item;
            public Text text;
            public Slider slider;

            public slider_struct(Transform transform) {
                item = null;
                text = null;
                slider = null;
                load(transform);
            }

            public void load(Transform transform) {
                item = transform;
                text = item.Find("Text").GetComponent<Text>();
                slider = item.Find("Slider").GetComponent<Slider>();
            }

            public void enable(bool enable) {
                item.gameObject.SetActive(enable);
            }

            public void set_range(float min, float max, bool whole_numbers) {
                slider.minValue = min;
                slider.maxValue = max;
                slider.wholeNumbers = whole_numbers;
            }

            public void add_listener(UnityAction<float> hook) {
                slider.onValueChanged.AddListener(hook);
            }
        }

        struct button_struct {
            public Transform item;
            public Text text;
            public Button button;

            public button_struct(Transform transform) {
                item = null;
                text = null;
                button = null;
                load(transform);
            }

            public void load(Transform transform) {
                item = transform;
                text = item.Find("Button/Text").GetComponent<Text>();
                button = item.Find("Button").GetComponent<Button>();
            }

            public void add_listener(UnityAction hook) {
                button.onClick.AddListener(hook);
            }

            public void enable(bool enable) {
                item.gameObject.SetActive(enable);
            }

            public void set_text(string str) {
                text.text = str;
            }
        }

        struct control_struct {
            public Transform item;
            public Text text;
            public Button button;
            public Image image;

            public control_struct(Transform transform) {
                item = null;
                text = null;
                button = null;
                image = null;
                load(transform);
            }

            public void load(Transform transform) {
                item = transform;
                text = item.Find("Button/Text").GetComponent<Text>();
                button = item.Find("Button").GetComponent<Button>();
                button = item.Find("Button").GetComponent<Button>();
                image = item.Find("Image").GetComponent<Image>();
            }

            public void add_listener(UnityAction hook) {
                button.onClick.AddListener(hook);
            }

            public void enable(bool enable) {
                item.gameObject.SetActive(enable);
            }

            public void set_text(string str) {
                text.text = str;
            }

            public void set_sprite(Sprite sprite) {
                image.sprite = sprite;
            }
        }

        private slider_struct deadzone_ls;
        private slider_struct deadzone_rs;
        private slider_struct deadzone_lt;
        private slider_struct deadzone_rt;
        private slider_struct sensitivity_mouse_look_x;
        private slider_struct sensitivity_mouse_look_y;
        private slider_struct sensitivity_mouse_move_x;
        private slider_struct sensitivity_mouse_move_y;
        private slider_struct sensitivity_mouse_scroll;
        private button_struct reset_controls;
        private button_struct reset_deadzones;
        private button_struct reset_sensitivity;

        public void Update() {
            change_input.update();
            if (UnityEngine.Input.GetKeyDown(KeyCode.F7)) {
                toggle_UI();
            }
        }

        public void load() {
            change_input = new ChangeInput(on_control_button_reassigned);
            load_asset_bundle();
            create_ui();
            set_settings();
        }

        enum MENU_ITEMS {
            FOOT_CONTROLS,
            DRIVING_CONTROLS,
            SETTINGS,
            COUNT,
        };

        private void create_ui() {
            ui_go = GameObject.Instantiate(assets.UI_prefab);

            foot_controls_tab = ui_go.transform.Find($"UI/Foot_Controls_Tab").gameObject;
            driving_controls_tab = ui_go.transform.Find($"UI/Driving_Controls_Tab").gameObject;
            settings_tab = ui_go.transform.Find($"UI/Settings_Tab").gameObject;
            context_settings = ui_go.transform.Find($"UI/Context_Settings").gameObject;

            Button button;
            button_struct button_struct = new button_struct();

            /* Menu Items */
            for (int i = 0; i < (int)MENU_ITEMS.COUNT; i++) {
                int c_index = i;
                button_struct.load(ui_go.transform.Find($"UI/Menu/Item {i}"));
                button_struct.enable(true);
                button_struct.set_text(((MENU_ITEMS)i).ToString().Replace("_", " "));
                button_struct.add_listener(delegate () { on_menu_button_pressed(c_index); });
            }
            
            /* Foot Controls */
            for (int i = 0; i < ControlManager.control_names.Count; i++) {
                                
                if (ControlManager.foot_controls_blacklist.Contains(ControlManager.control_names[i])) {
                    continue;
                }

                int c_index = i;
                button_struct.load(foot_controls_tab.transform.Find($"Content/Item {i}"));
                button_struct.enable(true);
                button_struct.add_listener(delegate () { on_control_button_pressed(c_index); });

                /* Hook setting button listener */
                button = button_struct.item.Find("Settings_Button").GetComponent<Button>();
                button.gameObject.SetActive(false); /* just disable for now */
                //button.onClick.AddListener(delegate () { on_control_settings_button_pressed(captured_index); });
            }

            /* Driving Controls */
            for (int i = 0; i < ControlManager.control_names.Count; i++) {
                
                if (ControlManager.driving_controls_blacklist.Contains(ControlManager.control_names[i])) {
                    continue;
                }

                int c_index = i;
                button_struct.load(driving_controls_tab.transform.Find($"Content/Item {i}"));
                button_struct.enable(true);
                button_struct.add_listener(delegate () { on_control_button_pressed(c_index); });

                /* Hook settings button listener */
                button = button_struct.item.Find("Settings_Button").GetComponent<Button>();
                button.gameObject.SetActive(false); /* just disable for now */
                //button.onClick.AddListener(delegate () { on_control_settings_button_pressed(captured_index); });                
            }

            /* settings_tab */

            /* LS Deadzone Slider */
            deadzone_ls = new slider_struct(settings_tab.transform.Find($"Content/Slider 0"));
            deadzone_ls.enable(true);
            deadzone_ls.set_range(0, 100, true);
            deadzone_ls.add_listener(on_deadzone_changed_ls);
            on_deadzone_changed_ls(ControlManager.controller.deadzone.ls);

            /* RS Deadzone Slider */
            deadzone_rs = new slider_struct(settings_tab.transform.Find($"Content/Slider 1"));
            deadzone_rs.enable(true);
            deadzone_rs.set_range(0, 100, true);
            deadzone_rs.add_listener(on_deadzone_changed_rs);
            on_deadzone_changed_rs(ControlManager.controller.deadzone.rs);

            /* LT Deadzone Slider */
            deadzone_lt = new slider_struct(settings_tab.transform.Find($"Content/Slider 2"));
            deadzone_lt.enable(true);
            deadzone_lt.set_range(0, 100, true);
            deadzone_lt.add_listener(on_deadzone_changed_lt);
            on_deadzone_changed_lt(ControlManager.controller.deadzone.lt);

            /* RT Deadzone Slider */
            deadzone_rt = new slider_struct(settings_tab.transform.Find($"Content/Slider 3"));
            deadzone_rt.enable(true);
            deadzone_rt.set_range(0, 100, true);
            deadzone_rt.add_listener(on_deadzone_changed_rt);
            on_deadzone_changed_rt(ControlManager.controller.deadzone.rt);

            /* Mouse Look X Sensitivity Slider */
            sensitivity_mouse_look_x = new slider_struct(settings_tab.transform.Find($"Content/Slider 4"));
            sensitivity_mouse_look_x.enable(true);
            sensitivity_mouse_look_x.set_range(0, 100, true);
            sensitivity_mouse_look_x.add_listener(on_sensitivity_changed_mouse_look_x);
            on_sensitivity_changed_mouse_look_x(ControlManager.camera_manager.controller_look_x.sensitivity);

            /* Mouse Look Y Sensitivity Slider */
            sensitivity_mouse_look_y = new slider_struct(settings_tab.transform.Find($"Content/Slider 5"));
            sensitivity_mouse_look_y.enable(true);
            sensitivity_mouse_look_y.set_range(0, 100, true);
            sensitivity_mouse_look_y.add_listener(on_sensitivity_changed_mouse_look_y);
            on_sensitivity_changed_mouse_look_y(ControlManager.camera_manager.controller_look_y.sensitivity);

            /* Mouse Move X Sensitivity Slider */
            sensitivity_mouse_move_x = new slider_struct(settings_tab.transform.Find($"Content/Slider 6"));
            sensitivity_mouse_move_x.enable(true);
            sensitivity_mouse_move_x.set_range(0, 100, true);
            sensitivity_mouse_move_x.add_listener(on_sensitivity_changed_mouse_move_x);
            on_sensitivity_changed_mouse_move_x(ControlManager.mouse_emulator.sensitivity_x);

            /* Mouse Move Y Sensitivity Slider */
            sensitivity_mouse_move_y = new slider_struct(settings_tab.transform.Find($"Content/Slider 7"));
            sensitivity_mouse_move_y.enable(true);
            sensitivity_mouse_move_y.set_range(0, 100, true);
            sensitivity_mouse_move_y.add_listener(on_sensitivity_changed_mouse_move_y);
            on_sensitivity_changed_mouse_move_y(ControlManager.mouse_emulator.sensitivity_y);

            /* Mouse Scroll Sensitivity Slider */
            sensitivity_mouse_scroll = new slider_struct(settings_tab.transform.Find($"Content/Slider 8"));
            sensitivity_mouse_scroll.enable(true);
            sensitivity_mouse_scroll.set_range(0, 100, true);
            sensitivity_mouse_scroll.add_listener(on_sensitivity_changed_mouse_scroll);
            on_sensitivity_changed_mouse_scroll(ControlManager.mouse_emulator.sensitivity_scroll);

            /* Reset Controls Button */
            reset_controls = new button_struct(settings_tab.transform.Find($"Content/Button 0"));
            reset_controls.enable(true);
            reset_controls.set_text("Reset Controls");
            reset_controls.add_listener(on_reset_settings);

            /* Reset Deadzones Button */
            reset_deadzones = new button_struct(settings_tab.transform.Find($"Content/Button 1"));
            reset_deadzones.enable(true);
            reset_deadzones.set_text("Reset Deadzones");
            reset_deadzones.add_listener(on_reset_deadzones);

            /* Reset Deadzones Button */
            reset_sensitivity = new button_struct(settings_tab.transform.Find($"Content/Button 2"));
            reset_sensitivity.enable(true);
            reset_sensitivity.set_text("Reset Sensitivity");
            reset_sensitivity.add_listener(on_reset_sensitivity);

            /* Header/Version */
            ui_go.transform.Find("UI/Header/Text").GetComponent<Text>().text = $"Mo`Controls v{VersionInfo.version}";
            ui_go.transform.Find("UI/Header/Shadow").GetComponent<Text>().text = $"Mo`Controls v{VersionInfo.version}";
        }

        private void set_settings() {

            on_deadzone_changed_ls(ControlManager.controller.deadzone.ls);
            on_deadzone_changed_rs(ControlManager.controller.deadzone.rs);
            on_deadzone_changed_lt(ControlManager.controller.deadzone.lt);
            on_deadzone_changed_rt(ControlManager.controller.deadzone.rt);
            on_sensitivity_changed_mouse_look_x(ControlManager.camera_manager.controller_look_x.sensitivity);
            on_sensitivity_changed_mouse_look_y(ControlManager.camera_manager.controller_look_y.sensitivity);
            on_sensitivity_changed_mouse_move_x(ControlManager.mouse_emulator.sensitivity_x);
            on_sensitivity_changed_mouse_move_y(ControlManager.mouse_emulator.sensitivity_y);
            on_sensitivity_changed_mouse_scroll(ControlManager.mouse_emulator.sensitivity_scroll);

            control_struct button = new control_struct();
            for (int i = 0; i < ControlManager.control_names.Count; i++) {
                if (ControlManager.foot_controls_blacklist.Contains(ControlManager.control_names[i])) {
                    continue;
                }

                ControlManager.get_control(PLAYER_MODE.FOOT_MODE, ControlManager.control_names[i], out XInputGamepad.INPUT c);
                button.load(foot_controls_tab.transform.Find($"Content/Item {i}"));
                button.set_text(ControlManager.control_names[i]);
                button.set_sprite(assets.spirtes[(int)c]);
            }

            for (int i = 0; i < ControlManager.control_names.Count; i++) {
                if (ControlManager.driving_controls_blacklist.Contains(ControlManager.control_names[i])) {
                    continue;
                }

                ControlManager.get_control(PLAYER_MODE.DRIVING_MODE, ControlManager.control_names[i], out XInputGamepad.INPUT c);                
                button.load(driving_controls_tab.transform.Find($"Content/Item {i}"));
                button.set_text(ControlManager.control_names[i]);
                button.set_sprite(assets.spirtes[(int)c]);                
            }
        }

        private void toggle_UI() {
            ui_go.SetActive(!ui_go.activeInHierarchy);
            ControlManager.player_in_menu = ui_go.activeInHierarchy;
        }

        private void set_control_value(string key, XInputGamepad.INPUT input) {
            /* Save Gamepad.Input as an INT so that it is modifable in the MSCEditor. */
            ControlManager.set_control(change_input.player_mode, key, input);
            switch (change_input.player_mode) {
                case PLAYER_MODE.FOOT_MODE:
                    MoControlsV2Mod.save_setting($"foot_{key}", (int)input);
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    MoControlsV2Mod.save_setting($"driving_{key}", (int)input);
                    break;
            }
        }

        private void set_tab(MENU_ITEMS i) {
            switch (i) {
                case MENU_ITEMS.FOOT_CONTROLS:
                    foot_controls_tab.SetActive(true);
                    driving_controls_tab.SetActive(false);
                    settings_tab.SetActive(false);
                    selected_tab = MENU_ITEMS.FOOT_CONTROLS;
                    break;
                case MENU_ITEMS.DRIVING_CONTROLS:
                    foot_controls_tab.SetActive(false);
                    driving_controls_tab.SetActive(true);
                    settings_tab.SetActive(false);
                    selected_tab = MENU_ITEMS.DRIVING_CONTROLS;
                    break;
                case MENU_ITEMS.SETTINGS:
                    foot_controls_tab.SetActive(false);
                    driving_controls_tab.SetActive(false);
                    settings_tab.SetActive(true);
                    selected_tab = MENU_ITEMS.SETTINGS;
                    break;
            }
        }
        
        private Sprite create_sprite(AssetBundle ab, string asset_name) {
            try {
                return Sprite.Create(ab.LoadAsset<Texture2D>(asset_name), Rect.MinMaxRect(0, 0, 100, 100), Vector2.zero);
            }
            catch (Exception e) {
                MoControlsV2Mod.error($"Failed to create sprite for asset: {asset_name}. Error: {e.StackTrace}");
                return null;
            }
        }

        private void load_asset_bundle() {
            assets = new MoControlsV2Assets((int)XInputGamepad.INPUT.COUNT);

            MoControlsV2Mod.log($"Loading asset bundle");
            AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(Properties.Resources.assetbundle);

            MoControlsV2Mod.log($"Loading ui prefabs");
            if (!assets.UI_prefab) {
                /* load ui prefab */
                assets.UI_prefab = ab.LoadAsset<GameObject>("Mo_Controls_UI.prefab");
                assets.UI_prefab.SetActive(false);
            }

            assets.spirtes[0] = create_sprite(ab, "xc_blank.png");
            assets.spirtes[1] = create_sprite(ab, "xc_a.png");
            assets.spirtes[2] = create_sprite(ab, "xc_b.png");
            assets.spirtes[3] = create_sprite(ab, "xc_x.png");
            assets.spirtes[4] = create_sprite(ab, "xc_y.png");
            assets.spirtes[5] = create_sprite(ab, "xc_start.png");
            assets.spirtes[6] = create_sprite(ab, "xc_back.png");            
            assets.spirtes[7] = create_sprite(ab, "xc_lb.png");
            assets.spirtes[8] = create_sprite(ab, "xc_rb.png");
            assets.spirtes[9] = create_sprite(ab, "xc_lt.png");
            assets.spirtes[10] = create_sprite(ab, "xc_rt.png");
            assets.spirtes[11] = null;//create_sprite(ab, "trigger_axis.png");
            assets.spirtes[12] = create_sprite(ab, "xc_ls_press.png");
            assets.spirtes[13] = create_sprite(ab, "xc_lsx.png");
            assets.spirtes[14] = create_sprite(ab, "xc_lsy.png");
            assets.spirtes[15] = create_sprite(ab, "xc_lsl.png");
            assets.spirtes[16] = create_sprite(ab, "xc_lsr.png");
            assets.spirtes[17] = create_sprite(ab, "xc_lsu.png");
            assets.spirtes[18] = create_sprite(ab, "xc_lsd.png");
            assets.spirtes[19] = create_sprite(ab, "xc_rs_press.png");
            assets.spirtes[20] = create_sprite(ab, "xc_rsx.png");
            assets.spirtes[21] = create_sprite(ab, "xc_rsy.png");
            assets.spirtes[22] = create_sprite(ab, "xc_rsl.png");
            assets.spirtes[23] = create_sprite(ab, "xc_rsr.png");
            assets.spirtes[24] = create_sprite(ab, "xc_rsu.png");
            assets.spirtes[25] = create_sprite(ab, "xc_rsd.png");
            assets.spirtes[26] = create_sprite(ab, "xc_dpl.png");
            assets.spirtes[27] = create_sprite(ab, "xc_dpr.png");
            assets.spirtes[28] = create_sprite(ab, "xc_dpu.png");
            assets.spirtes[29] = create_sprite(ab, "xc_dpd.png");

            ab.Unload(false);
        }

        /* Callbacks/Events */
        private void on_control_button_reassigned(XInputGamepad.INPUT i, bool canceled) {
            control_struct control;
            switch (change_input.player_mode) {
                case PLAYER_MODE.FOOT_MODE:
                    control = new control_struct(foot_controls_tab.transform.Find($"Content/Item {change_input.index}"));
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    control = new control_struct(driving_controls_tab.transform.Find($"Content/Item {change_input.index}"));
                    break;
                default:
                    return;
            }

            /* Reset Text */
            control.set_text(change_input.control_name);

            /* If the reassignment has been canceled. 
             * Don't update the texture or set the value */
            if (canceled) {
                return;
            }

            /* Set Texture */
            control.set_sprite(assets.spirtes[(int)i]);

            /* Set control value */
            set_control_value(change_input.control_name, (XInputGamepad.INPUT)i);

            /* If MouseButton0 is being reassigned and mouse click is down. Send a mouse click up event. */
            if (change_input.control_name == "MouseButton0" && ControlManager.mouse_emulator.mouse0_down) {
                ControlManager.mouse_emulator.simulate_mouse0_click(MOUSE_EVENT_CLICK_BUTTON0.UP);
            }

            /* If MouseButton1 is being reassigned and mouse click is down. Send a mouse click up event. */
            if (change_input.control_name == "MouseButton1" && ControlManager.mouse_emulator.mouse1_down) {
                ControlManager.mouse_emulator.simulate_mouse1_click(MOUSE_EVENT_CLICK_BUTTON1.UP);
            }
        }
        
        private void on_control_button_pressed(int i) {
            MoControlsV2Mod.log($"button pressed {ControlManager.control_names[i]}");

            if (!change_input.reassign_key) {
                control_struct control;
                PLAYER_MODE player_mode;
                switch (selected_tab) {
                    case MENU_ITEMS.FOOT_CONTROLS:
                        control = new control_struct(foot_controls_tab.transform.Find($"Content/Item {i}"));
                        player_mode = PLAYER_MODE.FOOT_MODE;
                        break;
                    case MENU_ITEMS.DRIVING_CONTROLS:
                        control = new control_struct(driving_controls_tab.transform.Find($"Content/Item {i}"));
                        player_mode = PLAYER_MODE.DRIVING_MODE;
                        break;
                    default:
                        return;
                }

                control.set_text("key assigning");
                change_input.to_polling_state(ControlManager.control_names[i], i, player_mode);
            }
        }

        private void on_control_settings_button_pressed(int i) {
            MoControlsV2Mod.log($"settings button pressed {ControlManager.control_names[i]}");
        }

        private void on_menu_button_pressed(int i) {
            MoControlsV2Mod.log($"button pressed {((MENU_ITEMS)i).ToString().Replace("_", " ")}");
            set_tab((MENU_ITEMS)i);
        }

        private void on_deadzone_changed_ls(float v) {
            deadzone_ls.text.text = $"LS Deadzone: {v}";
            deadzone_ls.slider.value = v;
            ControlManager.controller.deadzone.ls = v;
            MoControlsV2Mod.save_setting($"ls_deadzone", v);
        }

        private void on_deadzone_changed_rs(float v) {
            deadzone_rs.text.text = $"RS Deadzone: {v}";
            deadzone_rs.slider.value = v;
            ControlManager.controller.deadzone.rs = v;
            MoControlsV2Mod.save_setting($"rs_deadzone", v);
        }

        private void on_deadzone_changed_lt(float v) {
            deadzone_lt.text.text = $"LT Deadzone: {v}";
            deadzone_lt.slider.value = v;
            ControlManager.controller.deadzone.lt = v;
            MoControlsV2Mod.save_setting($"lt_deadzone", v);
        }

        private void on_deadzone_changed_rt(float v) {
            deadzone_rt.text.text = $"RT Deadzone: {v}";
            deadzone_rt.slider.value = v;
            ControlManager.controller.deadzone.rt = v;
            MoControlsV2Mod.save_setting($"rt_deadzone", v);
        }

        private void on_sensitivity_changed_mouse_look_x(float v) {
            sensitivity_mouse_look_x.text.text = $"Mouse Look X Sen: {v}";
            sensitivity_mouse_look_x.slider.value = v;
            ControlManager.camera_manager.controller_look_x.sensitivity = v;
            MoControlsV2Mod.save_setting($"mouse_look_x_sensitivity", v);
        }

        private void on_sensitivity_changed_mouse_look_y(float v) {
            sensitivity_mouse_look_y.text.text = $"Mouse Look Y Sen: {v}";
            sensitivity_mouse_look_y.slider.value = v;
            ControlManager.camera_manager.controller_look_y.sensitivity = v;
            MoControlsV2Mod.save_setting($"mouse_look_y_sensitivity", v);
        }

        private void on_sensitivity_changed_mouse_move_x(float v) {
            sensitivity_mouse_move_x.text.text = $"Mouse Move X Sen: {v}";
            sensitivity_mouse_move_x.slider.value = v;
            ControlManager.mouse_emulator.sensitivity_x = v;
            MoControlsV2Mod.save_setting($"mouse_move_x_sensitivity", v);
        }

        private void on_sensitivity_changed_mouse_move_y(float v) {
            sensitivity_mouse_move_y.text.text = $"Mouse Move Y Sen: {v}";
            sensitivity_mouse_move_y.slider.value = v;
            ControlManager.mouse_emulator.sensitivity_y = v;
            MoControlsV2Mod.save_setting($"mouse_move_y_sensitivity", v);
        }

        private void on_sensitivity_changed_mouse_scroll(float v) {
            sensitivity_mouse_scroll.text.text = $"Mouse Scroll Sen: {v}";
            sensitivity_mouse_scroll.slider.value = v;
            ControlManager.mouse_emulator.sensitivity_scroll = v;
            MoControlsV2Mod.save_setting($"mouse_scroll_sensitivity", v);
        }

        private void on_reset_settings() {
            for (int i = 0; i < ControlManager.control_names.Count; i++) {
                SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"foot_{ControlManager.control_names[i]}");
                SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"driving_{ControlManager.control_names[i]}");
            }

            ControlManager.set_default_controls();
            set_settings();
        }

        private void on_reset_deadzones() {
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"ls_deadzone");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"rs_deadzone");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"lt_deadzone");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"rt_deadzone");

            ControlManager.set_default_deadzones();
            set_settings();
        }

        private void on_reset_sensitivity() {
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"mouse_look_x_sensitivity");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"mouse_look_y_sensitivity");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"mouse_move_x_sensitivity");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"mouse_move_y_sensitivity");

            ControlManager.set_default_sensitivity();
            set_settings();
        }
    }
}
