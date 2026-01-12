using MSCLoader;
using System;
using System.Collections.Generic;
using TommoJProductions.MoControlsV2.MouseInput;
using TommoJProductions.MoControlsV2.XInput;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TommoJProductions.MoControlsV2 {
    public struct Slider_Struct {
        public Transform item;
        public Text text;
        public Slider slider;

        public Slider_Struct(Transform transform) {
            item = null;
            text = null;
            slider = null;
            load(transform);
        }

        public void load(Transform transform) {
            item = transform;
            text = item.Find("Text").GetComponent<Text>();
            slider = item.Find("Slider").GetComponent<Slider>();

            text.color = Color.black;
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
    public struct Button_Struct {
        public Transform item;
        public Text text;
        public Button button;
        public Image image;

        public Button_Struct(Transform transform, string name = "Button") {
            item = null;
            text = null;
            button = null;
            image = null;
            load(transform, name);
        }

        public void load(Transform transform, string name = "Button") {
            if (transform == null) {
                item = null;
                text = null;
                button = null;
                image = null;
                MoControlsV2Mod.error("button_struct: transform was null.");
                return;
            }

            item = transform;
            text = item.Find($"{name}/Text").GetComponent<Text>();
            button = item.Find(name).GetComponent<Button>();
            image = item.Find(name).GetComponent<Image>();

            text.color = Color.black;
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
    public struct Control_Struct {
        public Transform item;
        public Text text;
        public Button button;
        public Button button1;
        public Button button2;
        public Image image1;
        public Image image2;

        public Control_Struct(Transform transform) {
            item = null;
            text = null;
            button = null;
            button1 = null;
            button2 = null;
            image1 = null;
            image2 = null;
            load(transform);
        }

        public void load(Transform transform) {
            if (transform == null) {
                item = null;
                text = null;
                button = null;
                button1 = null;
                button2 = null;
                image1 = null;
                image2 = null;
                MoControlsV2Mod.error("control_struct: transform was null.");
                return;
            }

            item = transform;
            text = item.Find("Button/Text").GetComponent<Text>();
            button = item.Find("Button").GetComponent<Button>();
            button1 = item.Find("Image").GetComponent<Button>();
            button2 = item.Find("Image 1").GetComponent<Button>();
            image1 = item.Find("Image").GetComponent<Image>();
            image2 = item.Find("Image 1").GetComponent<Image>();

            text.color = Color.black;
        }

        public void add_listener_input(UnityAction hook) {
            button.onClick.AddListener(hook);
        }
        public void add_listener_modifier(UnityAction hook) {
            button2.onClick.AddListener(hook);
        }
        public void add_listener_context(UnityAction hook) {
            button1.onClick.AddListener(hook);
        }
        public void enable(bool enable) {
            item.gameObject.SetActive(enable);
        }
        public void set_text(string str) {
            text.text = str;
        }
        public void set_input_sprite(Sprite sprite) {
            image1.sprite = sprite;
        }
        public void set_modifier_sprite(Sprite sprite) {
            image2.sprite = sprite;
        }
    }
    public class Pointer_Handler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public bool hover;

        public void OnPointerEnter(PointerEventData eventData) {
            hover = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            hover = false;
        }

        public void reset() {
            hover = false;
        }
    }

    public class UI_Manager : MonoBehaviour {

        enum MENU_ITEMS {
            FOOT_CONTROLS,
            DRIVING_CONTROLS,
            SETTINGS,
            COUNT,
        };

        private Mo_Controls_V2_Assets assets;
        private Change_Input change_input;
        private Context_Input context_input;
        private MENU_ITEMS selected_tab;
        private GameObject ui_go;
        private GameObject foot_controls_tab;
        private GameObject driving_controls_tab;
        private GameObject settings_tab;
        private GameObject context_settings;
        
        private Slider_Struct deadzone_ls;
        private Slider_Struct deadzone_rs;
        private Slider_Struct deadzone_lt;
        private Slider_Struct deadzone_rt;
        private Slider_Struct sensitivity_mouse_look_x;
        private Slider_Struct sensitivity_mouse_look_y;
        private Slider_Struct sensitivity_mouse_scroll;
        private Button_Struct reset_controls;
        private Button_Struct reset_deadzones;
        private Button_Struct reset_sensitivity;

        private int foot_count;
        private int driving_count;
        private Pointer_Handler context_pointer_handler;

        private Color header_text_color;
        private Scrollbar scrollbar;

        Text header_text;
        Text header_shadow;

        public void Update() {
            if (ui_go.activeInHierarchy) {
                change_input.update();
                context_input.update();
                update_connection_status();
            }

            if (MoControlsV2Mod.ui_toggle_keybind.GetKeybindDown()) {
                toggle_UI();
            }
        }

        public void load() {
            change_input = new Change_Input(on_control_button_reassigned);
            context_input = new Context_Input(on_context_menu_click);
            load_asset_bundle();
            create_ui();
            set_settings();
        }
        private void load_asset_bundle() {
            assets = new Mo_Controls_V2_Assets((int)XINPUT_GAMEPAD_INPUT.COUNT);

            MoControlsV2Mod.log($"Loading asset bundle");
            AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(Properties.Resources.assetbundle);

            MoControlsV2Mod.log($"Loading ui prefabs");
            if (!assets.UI_prefab) {
                /* load ui prefab */
                assets.UI_prefab = ab.LoadAsset<GameObject>("Mo_Controls_UI.prefab");
                assets.UI_prefab.SetActive(false);
            }

            assets.sprites[0] = create_sprite(ab, "xc_blank.png");
            assets.sprites[1] = create_sprite(ab, "xc_a.png");
            assets.sprites[2] = create_sprite(ab, "xc_b.png");
            assets.sprites[3] = create_sprite(ab, "xc_x.png");
            assets.sprites[4] = create_sprite(ab, "xc_y.png");
            assets.sprites[5] = create_sprite(ab, "xc_start.png");
            assets.sprites[6] = create_sprite(ab, "xc_back.png");
            assets.sprites[7] = create_sprite(ab, "xc_lb.png");
            assets.sprites[8] = create_sprite(ab, "xc_rb.png");
            assets.sprites[9] = create_sprite(ab, "xc_lt.png");
            assets.sprites[10] = create_sprite(ab, "xc_rt.png");
            assets.sprites[11] = null;//create_sprite(ab, "trigger_axis.png");
            assets.sprites[12] = create_sprite(ab, "xc_ls_press.png");
            assets.sprites[13] = create_sprite(ab, "xc_lsl.png");
            assets.sprites[14] = create_sprite(ab, "xc_lsr.png");
            assets.sprites[15] = create_sprite(ab, "xc_lsu.png");
            assets.sprites[16] = create_sprite(ab, "xc_lsd.png");
            assets.sprites[17] = create_sprite(ab, "xc_lsx.png");
            assets.sprites[18] = create_sprite(ab, "xc_lsy.png");
            assets.sprites[19] = create_sprite(ab, "xc_rs_press.png");
            assets.sprites[20] = create_sprite(ab, "xc_rsl.png");
            assets.sprites[21] = create_sprite(ab, "xc_rsr.png");
            assets.sprites[22] = create_sprite(ab, "xc_rsu.png");
            assets.sprites[23] = create_sprite(ab, "xc_rsd.png");
            assets.sprites[24] = create_sprite(ab, "xc_rsx.png");
            assets.sprites[25] = create_sprite(ab, "xc_rsy.png");
            assets.sprites[26] = create_sprite(ab, "xc_dpl.png");
            assets.sprites[27] = create_sprite(ab, "xc_dpr.png");
            assets.sprites[28] = create_sprite(ab, "xc_dpu.png");
            assets.sprites[29] = create_sprite(ab, "xc_dpd.png");
            assets.sprites[30] = create_sprite(ab, "xc_dpx.png");
            assets.sprites[31] = create_sprite(ab, "xc_dpy.png");

            ab.Unload(false);
        }
        private void create_control(string[] blacklist, Transform tab_transform, int i, ref int count) {
            for (int j = 0; j < blacklist.Length; j++) {
                if (blacklist[j] == Control_Manager.control_names[i]) {
                    return;
                }
            }

            int c_index = i;
            Control_Struct control = new Control_Struct(tab_transform.Find($"Content/Item {i}"));
            control.enable(true);
            control.add_listener_input(delegate () { on_control_button_pressed(c_index); });
            control.add_listener_modifier(delegate () { on_control_modifier_pressed(c_index); });
            control.add_listener_context(delegate () { on_context_menu_open(c_index); });
            count++;
        }
        private void create_slider(out Slider_Struct slider, UnityAction<float> on_changed, Transform tab_transform, int i, float initial_value) {
            slider = new Slider_Struct(tab_transform.Find($"Content/Slider {i}"));
            slider.enable(true);
            slider.set_range(0, 100, true);
            slider.add_listener(on_changed);
            on_changed(initial_value);
        }
        private void create_button(out Button_Struct button, UnityAction on_click, Transform tab_transform, int i, string text) {
            button = new Button_Struct(tab_transform.Find($"Content/Button {i}"));
            button.enable(true);
            button.set_text(text);
            button.add_listener(on_click);
        }
        private void create_ui() {
            ui_go = Instantiate(assets.UI_prefab);

            foot_controls_tab = ui_go.transform.Find($"UI/Foot_Controls_Tab").gameObject;
            driving_controls_tab = ui_go.transform.Find($"UI/Driving_Controls_Tab").gameObject;
            settings_tab = ui_go.transform.Find($"UI/Settings_Tab").gameObject;
            context_settings = ui_go.transform.Find($"UI/Context_Settings").gameObject;
            scrollbar = ui_go.transform.Find("UI/Scrollbar").GetComponent<Scrollbar>();

            header_text = ui_go.transform.Find("UI/Connection/Text").GetComponent<Text>();
            header_shadow = ui_go.transform.Find("UI/Connection/Shadow").GetComponent<Text>();

            /* Header/Version */
            header_text_color = ui_go.transform.Find("UI/Header/Text").GetComponent<Text>().color;
            ui_go.transform.Find("UI/Header/Text").GetComponent<Text>().text = $"Mo`Controls v{VersionInfo.full_version}";
            ui_go.transform.Find("UI/Header/Shadow").GetComponent<Text>().text = $"Mo`Controls v{VersionInfo.full_version}";

            /* Menu Items */
            for (MENU_ITEMS i = 0; i < MENU_ITEMS.COUNT; i++) {
                MENU_ITEMS c_index = i;
                Button_Struct button_struct = new Button_Struct(ui_go.transform.Find($"UI/Menu/Item {(int)i}"));
                button_struct.enable(true);
                button_struct.set_text(get_menu_tab_text(i));
                button_struct.add_listener(delegate () { set_tab(c_index); });
            }

            /*Foot/Driving tab */
            foot_count = 0;
            driving_count = 0;
            for (int i = 0; i < Control_Manager.control_names.Count; i++) {
                create_control(Control_Manager.foot_controls_blacklist, foot_controls_tab.transform, i, ref foot_count);
                create_control(Control_Manager.driving_controls_blacklist, driving_controls_tab.transform, i, ref driving_count);
            }

            set_tab(0);

            /* settings_tab */
            create_slider(out deadzone_ls, on_deadzone_changed_ls, settings_tab.transform, 0, Control_Manager.controller.deadzone.ls);
            create_slider(out deadzone_rs, on_deadzone_changed_rs, settings_tab.transform, 1, Control_Manager.controller.deadzone.rs);
            create_slider(out deadzone_lt, on_deadzone_changed_lt, settings_tab.transform, 2, Control_Manager.controller.deadzone.lt);
            create_slider(out deadzone_rt, on_deadzone_changed_rt, settings_tab.transform, 3, Control_Manager.controller.deadzone.rt);
            create_slider(out sensitivity_mouse_look_x, on_sensitivity_changed_mouse_look_x, settings_tab.transform, 4, Control_Manager.camera_manager.controller_look_x.sensitivity);
            create_slider(out sensitivity_mouse_look_y, on_sensitivity_changed_mouse_look_y, settings_tab.transform, 5, Control_Manager.camera_manager.controller_look_y.sensitivity);
            create_slider(out sensitivity_mouse_scroll, on_sensitivity_changed_mouse_scroll, settings_tab.transform, 6, Control_Manager.mouse_emulator.sensitivity_scroll);

            create_button(out reset_controls, on_reset_settings, settings_tab.transform, 0, "Reset Controls");
            create_button(out reset_deadzones, on_reset_deadzones, settings_tab.transform, 1, "Reset Deadzones");
            create_button(out reset_sensitivity, on_reset_sensitivity, settings_tab.transform, 2, "Reset Sensitivity");

            /* Context menu */
            for (int i = 0; i < 4; i++) {
                int c_index = i;
                Button_Struct button_struct = new Button_Struct(context_settings.transform.Find($"Content/Item {i}"));
                button_struct.enable(true);
                button_struct.add_listener(delegate () { context_menu_reassign(c_index); });
            }

            context_pointer_handler = context_settings.AddComponent<Pointer_Handler>();
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
                
        private void update_connection_status() {
            string connection;
            if (Control_Manager.controller.state.is_connected) {
                connection = "Connected";
            }
            else {
                connection = "Disconnected";
            }
            header_text.text = connection;
            header_shadow.text = connection;
        }
        private string get_menu_tab_text(MENU_ITEMS i) {
            switch (i) {
                case MENU_ITEMS.FOOT_CONTROLS:
                    return "Foot";
                case MENU_ITEMS.DRIVING_CONTROLS:
                    return "Driving";
                case MENU_ITEMS.SETTINGS:
                    return "Settings";
                default:
                    return "Unk";
            }
        }
        private float cal_content_height(int count, int items_per_row, float item_height) {
            return ((count / items_per_row) + Mathf.Clamp01(count % items_per_row)) * item_height;
        }
        private void set_settings() {

            on_deadzone_changed_ls(Control_Manager.controller.deadzone.ls);
            on_deadzone_changed_rs(Control_Manager.controller.deadzone.rs);
            on_deadzone_changed_lt(Control_Manager.controller.deadzone.lt);
            on_deadzone_changed_rt(Control_Manager.controller.deadzone.rt);
            on_sensitivity_changed_mouse_look_x(Control_Manager.camera_manager.controller_look_x.sensitivity);
            on_sensitivity_changed_mouse_look_y(Control_Manager.camera_manager.controller_look_y.sensitivity);
            on_sensitivity_changed_mouse_scroll(Control_Manager.mouse_emulator.sensitivity_scroll);

            Control_Struct button = new Control_Struct();            
            for (int i = 0; i < Control_Manager.control_names.Count; i++) {
                bool blacklist = false;
                for (int j = 0; j < Control_Manager.foot_controls_blacklist.Length; j++) {
                    if (Control_Manager.foot_controls_blacklist[j] == Control_Manager.control_names[i]) {
                        blacklist = true;
                        break;
                    }
                }
                if (blacklist) {
                    continue;
                }

                Control_Manager.get_control(PLAYER_MODE.FOOT_MODE, Control_Manager.control_names[i], out Control_Input c);
                button.load(foot_controls_tab.transform.Find($"Content/Item {i}"));
                button.set_text(Control_Manager.control_names[i]);
                button.set_input_sprite(assets.sprites[(int)c.input]);
                button.set_modifier_sprite(assets.sprites[(int)c.modifier]);
            }

            for (int i = 0; i < Control_Manager.control_names.Count; i++) {
                bool blacklist = false; 
                for (int j = 0; j < Control_Manager.driving_controls_blacklist.Length; j++) {
                    if (Control_Manager.driving_controls_blacklist[j] == Control_Manager.control_names[i]) {
                        blacklist = true;
                        break;
                    }
                }
                if (blacklist) {
                    continue;
                }

                Control_Manager.get_control(PLAYER_MODE.DRIVING_MODE, Control_Manager.control_names[i], out Control_Input c);                
                button.load(driving_controls_tab.transform.Find($"Content/Item {i}"));
                button.set_text(Control_Manager.control_names[i]);
                button.set_input_sprite(assets.sprites[(int)c.input]);
                button.set_modifier_sprite(assets.sprites[(int)c.modifier]);
            }
        }
        private void toggle_UI() {
            ui_go.SetActive(!ui_go.activeInHierarchy);
            Control_Manager.player_in_menu = ui_go.activeInHierarchy;
        }
        private void set_control_input(PLAYER_MODE mode, string key, XINPUT_GAMEPAD_INPUT input) {
            Control_Manager.set_control(mode, key, input, null);
            switch (mode) {
                case PLAYER_MODE.FOOT_MODE:
                    MoControlsV2Mod.save_setting($"foot_input_{key}", (int)input);
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    MoControlsV2Mod.save_setting($"driving_input_{key}", (int)input);
                    break;
            }
        }
        private void set_control_modifier(PLAYER_MODE mode, string key, XINPUT_GAMEPAD_INPUT modifier) {
            Control_Manager.set_control(mode, key, null, modifier);
            switch (mode) {
                case PLAYER_MODE.FOOT_MODE:
                    MoControlsV2Mod.save_setting($"foot_modifier_{key}", (int)modifier);
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    MoControlsV2Mod.save_setting($"driving_modifier_{key}", (int)modifier);
                    break;
            }
        }
        private void set_tab(MENU_ITEMS i) {
            /* highlight selected tab button */
            for (MENU_ITEMS c = 0; c < MENU_ITEMS.COUNT; c++) {
                Button_Struct button_struct = new Button_Struct();
                button_struct.load(ui_go.transform.Find($"UI/Menu/Item {(int)c}"));
                if (c == i) {
                    button_struct.text.color = header_text_color;
                }
                else {
                    button_struct.text.color = Color.black;
                }
            }

            /* activate selected tab */
            RectTransform t;
            Transform t2;
            switch (i) {
                case MENU_ITEMS.FOOT_CONTROLS:
                    foot_controls_tab.SetActive(true);
                    driving_controls_tab.SetActive(false);
                    settings_tab.SetActive(false);
                    selected_tab = MENU_ITEMS.FOOT_CONTROLS;

                    t2 = foot_controls_tab.transform.Find("Content");
                    t = t2.GetComponent<RectTransform>();
                    t.sizeDelta = new Vector2(t.sizeDelta.x, cal_content_height(foot_count, 5, 116));
                    t.anchoredPosition = new Vector2(0, 0);
                    scrollbar.value = 1;
                    break;
                case MENU_ITEMS.DRIVING_CONTROLS:
                    foot_controls_tab.SetActive(false);
                    driving_controls_tab.SetActive(true);
                    settings_tab.SetActive(false);
                    selected_tab = MENU_ITEMS.DRIVING_CONTROLS;

                    t2 = driving_controls_tab.transform.Find("Content");
                    t = t2.GetComponent<RectTransform>();
                    t.sizeDelta = new Vector2(t.sizeDelta.x, cal_content_height(driving_count, 5, 116));
                    t.anchoredPosition = new Vector2(0, 0);
                    scrollbar.value = 1;
                    break;
                case MENU_ITEMS.SETTINGS:
                    foot_controls_tab.SetActive(false);
                    driving_controls_tab.SetActive(false);
                    settings_tab.SetActive(true);
                    selected_tab = MENU_ITEMS.SETTINGS;
                    break;
            }
        }
        private void get_ui_control(int i, out Transform t, out PLAYER_MODE mode) {
            switch (selected_tab) {
                case MENU_ITEMS.FOOT_CONTROLS:
                    t = foot_controls_tab.transform.Find($"Content/Item {i}");
                    mode = PLAYER_MODE.FOOT_MODE;
                    break;
                case MENU_ITEMS.DRIVING_CONTROLS:
                    t = driving_controls_tab.transform.Find($"Content/Item {i}");
                    mode = PLAYER_MODE.DRIVING_MODE;
                    break;
                default:
                    t = null;
                    mode = 0;
                    return;
            }
        }

        /* Callbacks/Events */
        private void on_control_button_reassigned(XINPUT_GAMEPAD_INPUT i, bool cancelled) {
            change_input.control.set_text(change_input.control_name);

            if (cancelled) {
                return;
            }

            change_input.control.set_input_sprite(assets.sprites[(int)i]);
            set_control_input(change_input.player_mode, change_input.control_name, i);
        }
        private void on_control_button_pressed(int i) {
            if (!change_input.reassign_key) {
                change_input.on_reassign_key = on_control_button_reassigned;
                get_ui_control(i, out Transform t, out PLAYER_MODE mode);
                change_input.to_polling_state(Control_Manager.control_names[i], i, mode, t);
                change_input.control.set_text("Key");
            }
        }
        
        private void on_control_modifier_reassigned(XINPUT_GAMEPAD_INPUT i, bool cancelled) {
            change_input.control.set_text(change_input.control_name);

            if (cancelled) {
                return;
            }

            change_input.control.set_modifier_sprite(assets.sprites[(int)i]);
            set_control_modifier(change_input.player_mode, change_input.control_name, i);
        }
        private void on_control_modifier_pressed(int i) {
            if (!change_input.reassign_key) {
                change_input.on_reassign_key = on_control_modifier_reassigned;
                get_ui_control(i, out Transform t, out PLAYER_MODE mode);
                change_input.to_polling_state(Control_Manager.control_names[i], i, mode, t);
                change_input.control.set_text("Modifier");
            }
        }

        private void on_context_menu_close() {
            context_settings.SetActive(false);
            context_input.reset();
            context_pointer_handler.reset();
        }
        private void on_context_menu_open(int i) {
            if (!context_input.reassign_key) {
                XINPUT_GAMEPAD_INPUT k;
                PLAYER_MODE player_mode;
                Transform t;
                switch (selected_tab) {
                    case MENU_ITEMS.FOOT_CONTROLS:
                        k = Control_Manager.foot_controls[i].input;
                        t = foot_controls_tab.transform.Find($"Content/Item {i}");
                        player_mode = PLAYER_MODE.FOOT_MODE;
                        break;
                    case MENU_ITEMS.DRIVING_CONTROLS:
                        k = Control_Manager.driving_controls[i].input;
                        t = driving_controls_tab.transform.Find($"Content/Item {i}");
                        player_mode = PLAYER_MODE.DRIVING_MODE;
                        break;
                    default:
                        return;
                }

                /* Check if this gamepad input has any context input */
                if (!Control_Manager.context_dic.TryGetValue(k, out XINPUT_GAMEPAD_INPUT[] inputs)) {
                    on_context_menu_close();
                    return;
                }

                context_settings.SetActive(true);

                /* Load context items */
                Button_Struct button = new Button_Struct();
                for (int j = 0; j < 4; j++) {
                    button.load(context_settings.transform.Find($"Content/Item {j}"));
                    if (j < inputs.Length) {
                        button.enable(true);
                        button.set_text("");
                        button.set_sprite(assets.sprites[(int)inputs[j]]);
                    }
                    else {
                        button.enable(false);
                    }
                }

                /* Set context menu position, width, height */
                GridLayoutGroup grid = context_settings.transform.Find("Content").GetComponent<GridLayoutGroup>();
                RectTransform t2 = context_settings.GetComponent<RectTransform>();
                t2.position = UnityEngine.Input.mousePosition;
                t2.sizeDelta = new Vector2(grid.cellSize.x, grid.cellSize.y * inputs.Length);

                context_input.to_polling_state(Control_Manager.control_names[i], i, player_mode, t, inputs);
            }
        }
        private void on_context_menu_click() {
            if (!context_pointer_handler.hover) {
                on_context_menu_close();
            }
        }
        private void context_menu_reassign(int i) {
            context_input.control.set_input_sprite(assets.sprites[(int)context_input.context_inputs[i]]);
            set_control_input(context_input.player_mode, context_input.control_name, context_input.context_inputs[i]);
            on_context_menu_close();
        }

        private void on_deadzone_changed_ls(float v) {
            deadzone_ls.text.text = $"LS Deadzone: {v}";
            deadzone_ls.slider.value = v;
            Control_Manager.controller.deadzone.ls = v;
            MoControlsV2Mod.save_setting($"ls_deadzone", v);
        }
        private void on_deadzone_changed_rs(float v) {
            deadzone_rs.text.text = $"RS Deadzone: {v}";
            deadzone_rs.slider.value = v;
            Control_Manager.controller.deadzone.rs = v;
            MoControlsV2Mod.save_setting($"rs_deadzone", v);
        }
        private void on_deadzone_changed_lt(float v) {
            deadzone_lt.text.text = $"LT Deadzone: {v}";
            deadzone_lt.slider.value = v;
            Control_Manager.controller.deadzone.lt = v;
            MoControlsV2Mod.save_setting($"lt_deadzone", v);
        }
        private void on_deadzone_changed_rt(float v) {
            deadzone_rt.text.text = $"RT Deadzone: {v}";
            deadzone_rt.slider.value = v;
            Control_Manager.controller.deadzone.rt = v;
            MoControlsV2Mod.save_setting($"rt_deadzone", v);
        }
        private void on_sensitivity_changed_mouse_look_x(float v) {
            sensitivity_mouse_look_x.text.text = $"Mouse Look X Sen: {v}";
            sensitivity_mouse_look_x.slider.value = v;
            Control_Manager.camera_manager.controller_look_x.sensitivity = v;
            MoControlsV2Mod.save_setting($"mouse_look_x_sensitivity", v);
        }
        private void on_sensitivity_changed_mouse_look_y(float v) {
            sensitivity_mouse_look_y.text.text = $"Mouse Look Y Sen: {v}";
            sensitivity_mouse_look_y.slider.value = v;
            Control_Manager.camera_manager.controller_look_y.sensitivity = v;
            MoControlsV2Mod.save_setting($"mouse_look_y_sensitivity", v);
        }
        private void on_sensitivity_changed_mouse_scroll(float v) {
            sensitivity_mouse_scroll.text.text = $"Mouse Scroll Sen: {v}";
            sensitivity_mouse_scroll.slider.value = v;
            Control_Manager.mouse_emulator.sensitivity_scroll = v;
            MoControlsV2Mod.save_setting($"mouse_scroll_sensitivity", v);
        }
        private void on_reset_settings() {
            for (int i = 0; i < Control_Manager.control_names.Count; i++) {
                SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"foot_input_{Control_Manager.control_names[i]}");
                SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"foot_modifier_{Control_Manager.control_names[i]}");
                SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"driving_input_{Control_Manager.control_names[i]}");
                SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"driving_modifier_{Control_Manager.control_names[i]}");
            }

            Control_Manager.set_default_controls();
            set_settings();
        }
        private void on_reset_deadzones() {
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"ls_deadzone");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"rs_deadzone");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"lt_deadzone");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"rt_deadzone");

            Control_Manager.set_default_deadzones();
            set_settings();
        }
        private void on_reset_sensitivity() {
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"mouse_look_x_sensitivity");
            SaveLoad.DeleteValue(MoControlsV2Mod.mod, $"mouse_look_y_sensitivity");
            Control_Manager.set_default_sensitivity();
            set_settings();
        }
    }
}
