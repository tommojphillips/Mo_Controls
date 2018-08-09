using System;
using System.Linq;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using Mo_Controls.MouseEmulation;
using Mo_Controls.XboxController;
using xController = Mo_Controls.XboxController.XboxController;

namespace Mo_Controls
{
    // XInputDotNetPure.dll - in folder: "My Summer Car/mysummercar_data/Managed"
    // XInputInterface.dll - in folder: "My Summer Car"
    public class Mo_Controls : Mod
    {
        // Written, 06.07.2018        

        #region Mod Fields

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Author => "tommojphillips";
        public override string Version => "1.0.3";
        public override bool UseAssetsFolder => true;

        #endregion

        #region Fields / Properties

        #region Xbox Related

        /// <summary>
        /// Represents an emulator for the mouse.
        /// </summary>
        private MouseEmulator mouseEmulator
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the xbox controller manager.
        /// </summary>
        private XboxControllerManager xboxControllerManager
        {
            get;
            set;
        }
        /// <summary>
        /// Represents an xbox controller.
        /// </summary>
        public xController xboxController
        {
            get;
            private set;
        }

        #endregion

        #region Controls/Input Related

        /// <summary>
        /// Represents all functional inputs for the game.
        /// </summary>
        private string[] inputNames = new string[]
        {
            "Left",
            "Right",
            "Throttle",
            "Brake",
            "Clutch",
            "ShiftUp",
            "ShiftDown",
            "IndicatorLeft",
            "IndicatorRight",
            "Range",
            "HighBeam",
            "Wipers",
            "Boost",
            "Handbrake",
            "DrivingMode",
            "PlayerLeft",
            "PlayerRight",
            "PlayerUp",
            "PlayerDown",
            "Jump",
            "Run",
            "Zoom",
            "Use",
            "Crouch",
            "ReachLeft",
            "ReachRight",
            "Hitchhike",
            "Swear",
            "Hit",
            "Push",
            "Finger",
            "Urinate",
            "Drunk",
            "Smoking",
            "reverse",
            "first",
            "second",
            "third",
            "fourth",
            "fifth",
            "sixth",
        };
        /// <summary>
        /// Represents currently assigned control inputs for the game.
        /// </summary>
        private string[,] controlInputs;
        /// <summary>
        /// Represents the change input result for the mod.
        /// </summary>
        private ChangeInput changeInputResult
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the amount of keybinds that have been loaded via the mods.
        /// </summary>
        private int modKeybindCount = 0;

        #endregion

        #region Mod Loader Settings

        /// <summary>
        /// Represents a keybind to open/close the gui for the mod.
        /// </summary>
        public readonly Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.F12);
        /// <summary>
        /// Represents whether the mod should display debug info or not.
        /// </summary>
        private Settings showDebugGui = new Settings("showDebugGui", "Show Debug GUI", true);
        /// <summary>
        /// Represent whether the mod should display the virtual gui.
        /// </summary>
        private Settings showVirtualGui = new Settings("showVirtualGui", "Show Virtual Axes GUI", false);
        /// <summary>
        /// Represents whether the mod should monitor for xbox controller connections?
        /// </summary>
        private Settings monitiorXboxControllerConnectionStatus = new Settings("monitorConnectionStatus", "Monitor Controller status", true);

        #endregion

        #region GUI Fields

        /// <summary>
        /// Represents whether the gui should display the mod controls.
        /// </summary>
        private bool showModControls = false;
        /// <summary>
        /// Represents whether the gui should display the game controls.
        /// </summary>
        private bool showGameControls = true;
        /// <summary>
        /// Represents whether the gui should be opened or closed.
        /// </summary>
        private bool controlsGuiOpened = false;
        /// <summary>
        /// Represents whether the gui should display the settings.
        /// </summary>
        private bool showSettings = false;
        /// <summary>
        /// Represents the scroll positions of the scroll bar for the gui.
        /// </summary>
        private Vector2 controlsGuiScrollPosition;
        /// <summary>
        /// Represents the scroll position of the scroll bar for the virtual xbox axis gui.
        /// </summary>
        private Vector2 virtualAxisGuiScrollPosition;
        /// <summary>
        /// Represents the scroll position of the scroll bar for the xbox controller debug gui.
        /// </summary>
        private Vector2 xboxControllerDebugGuiScrollPosition;
        /// <summary>
        /// Represents the scroll position of the scroll bar for the mouse emulator gui.
        /// </summary>
        private Vector2 mouseEmulatorGuiScrollPosition;

        // Constants

        /// <summary>
        /// Represents the width for the main (game/mod controls/keybinds) gui.
        /// </summary>
        private const float CONTROLS_GUI_WIDTH = 400f;
        /// <summary>
        /// Represents the height of the info gui.
        /// </summary>
        private const float INFO_GUI_HEIGHT = 235f;//340f;
        /// <summary>
        /// Represents the width for the virtual axes gui.
        /// </summary>
        private const float VIRTUAL_XBOX_AXES_GUI_WIDTH = 180f;
        /// <summary>
        /// /// Represents the height for the virtual axes gui.
        /// </summary>
        private const float VIRTUAL_XBOX_AXES_GUI_HEIGHT = 600f;
        /// Represents the width for the xbox controller debug gui.
        /// </summary>
        private const float XBOX_CONTROLLER_DEBUG_GUI_WIDTH = 250f;
        /// <summary>
        /// Represents the height for the xbox controller debug gui.
        /// </summary>
        private const float XBOX_CONTROLLER_DEBUG_GUI_HEIGHT = 650f;
        /// <summary>
        /// Represents the space between the guis.
        /// </summary>
        private const float GUI_SPACE = 10f;
        /// <summary>
        /// Represents the height of the settings in the info gui.
        /// </summary>
        private const float SETTINGS_HEIGHT = 100f;
        /// <summary>
        /// Represents the width of the emulation settings gui.
        /// </summary>
        private const float MOUSE_EMULATION_GUI_WIDTH = 400f;
        /// <summary>
        /// Represents the height of the emulation settings gui.
        /// </summary>
        private const float MOUSE_EMULATION_GUI_HEIGHT = 845f;
        /// <summary>
        /// Represents the off set of the scroll bar.
        /// </summary>
        private const float SCROLLBAR_OFFSET = 7f;

        #endregion

        #region Keys

        /// <summary>
        /// Represents the select keycode.
        /// </summary>
        private const KeyCode selectKey = KeyCode.Mouse0; // LMB
        /// <summary>
        /// Represents the cancel keycode.
        /// </summary>
        private const KeyCode cancelKey = KeyCode.Mouse1; // RMB
        /// <summary>
        /// Represents the none keycode.
        /// </summary>
        private const KeyCode noneKey = KeyCode.Delete;        

        #endregion

        /// <summary>
        /// Represents the current instance of <see cref="Mo_Controls"/>.
        /// </summary>
        public static Mo_Controls instance
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the supported/compatible version of mod loader.
        /// </summary>
        public const string SUPPORTED_MODLOADER_VERSION = "0.4.4";

        #endregion

        #region Methods

        /// <summary>
        /// Gets infomation about a cInput input based on the input namem, <paramref name="inputName"/>. Returns an array with 3 items within it. In this order:
        /// 1. The name of the control, 2. The Primary input for the control, 3. The Secondary input for the control.
        /// </summary>
        /// <param name="inputName">The name of the input.</param>
        private string[] getInputNameControl(string inputName)
        {
            // Written, 06.07.2018

            string[] returnValues = new string[3];
            for (int i = 0; i < 3; i++)
                returnValues[i] = cInput.GetText(inputName, i);
            return returnValues;
        }
        /// <summary>
        /// Loads control inputs (defined in <see cref="inputNames"/>) from the class, <see cref="cInput"/> and adds each one to <see cref="controlInputs"/> with it's primary
        /// and secondary input.
        /// </summary>
        private void loadControlInputsFromCInput()
        {
            // Written, 06.07.2018

            for (int i = 0; i < inputNames.Length; i++)
            {
                string[] inputName = this.getInputNameControl(this.inputNames[i]);
                for (int j = 0; j < inputName.Length; j++)
                {
                    this.controlInputs[i, j] = inputName[j];
                }
            }
        }
        /// <summary>
        /// Changes the input for a control defined in <see cref="changeInputResult"/> to the provided input string, <paramref name="input"/>.
        /// </summary>
        /// <param name="input"></param>
        private void changeInput(string input)
        {
            // Written, 09.07.2018

            if (!this.changeInputResult.isModKeybind)
            {
                // Treat as a game control.

                if (this.changeInputResult.index == 1)
                {
                    cInput.ChangeKey(this.changeInputResult.controlName, input, cInput.GetText(this.changeInputResult.controlName, 2));
                }
                else
                {
                    cInput.ChangeKey(this.changeInputResult.controlName, cInput.GetText(this.changeInputResult.controlName, 1), input);
                }
                this.loadControlInputsFromCInput();
            }
            else
            {
                // Treat as a mod keybind.

                Keybind modKeybind = Keybind.Get(this.changeInputResult.mod).Where(kb => kb.ID == this.changeInputResult.controlName).First();
                if (this.changeInputResult.index == 1)
                {
                    modKeybind.Modifier = (KeyCode)Enum.Parse(typeof(KeyCode), input);
                }
                else
                {
                    modKeybind.Key = (KeyCode)Enum.Parse(typeof(KeyCode), input);
                }
                ModSettings_menu.SaveModBinds(this.changeInputResult.mod);

            }
            this.changeInputResult = new ChangeInput();
        }
        /// <summary>
        /// Draws the main gui.
        /// </summary>
        private void drawControlsGui()
        {
            // Written, 10.07.2018

            GUILayoutOption[] layoutOption = new GUILayoutOption[1]
            {
                GUILayout.Width(CONTROLS_GUI_WIDTH - 31)
            };

            #region Info Gui

            GUILayout.BeginArea(new Rect(GUI_SPACE, GUI_SPACE, CONTROLS_GUI_WIDTH, INFO_GUI_HEIGHT));
            GUILayout.BeginVertical("box");
            GUILayout.Label(String.Format("{0} v{1} by {2}", this.Name, this.Version, this.Author));
            GUILayout.Label(String.Format("{0} is a forbidden key (GUI key bind).\r\n{1} Sets as None.\r\nLMB Selects.\r\nRMB Cancels.", this.openControlsGui.Key, noneKey));

            #region Settings

            if (GUILayout.Button(this.showSettings ? "Close Settings" : "Open Settings"))
            {
                this.showSettings = !this.showSettings;
            }
            if (this.showSettings)
            {
                bool saveSettings = false;
                if (GUILayout.Toggle((bool)this.showDebugGui.Value, this.showDebugGui.Name) != (bool)this.showDebugGui.Value)
                {
                    this.showDebugGui.Value = !(bool)this.showDebugGui.Value;
                    saveSettings = true;
                }
                if (GUILayout.Toggle((bool)this.showVirtualGui.Value, this.showVirtualGui.Name) != (bool)this.showVirtualGui.Value)
                {
                    this.showVirtualGui.Value = !(bool)this.showVirtualGui.Value;
                    saveSettings = true;
                }
                if (GUILayout.Toggle((bool)this.monitiorXboxControllerConnectionStatus.Value, this.monitiorXboxControllerConnectionStatus.Name) != (bool)this.monitiorXboxControllerConnectionStatus.Value)
                {
                    this.monitiorXboxControllerConnectionStatus.Value = !(bool)this.monitiorXboxControllerConnectionStatus.Value;
                    saveSettings = true;
                }                
                if (saveSettings)
                {
                    ModSettings_menu.SaveSettings(this);
                }
            }

            #endregion

            GUILayout.EndVertical();
            GUILayout.EndArea();

            #endregion

            float top = INFO_GUI_HEIGHT + GUI_SPACE - (this.showSettings ? 0 : SETTINGS_HEIGHT);
            GUILayout.BeginArea(new Rect(GUI_SPACE, top, CONTROLS_GUI_WIDTH, this.evaluateGameControlsGuiHeight()));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(CONTROLS_GUI_WIDTH) });
            this.controlsGuiScrollPosition = GUILayout.BeginScrollView(this.controlsGuiScrollPosition, false, false, new GUILayoutOption[1] { GUILayout.Width(CONTROLS_GUI_WIDTH - SCROLLBAR_OFFSET) });

            #region Game Controls

            if (GUILayout.Button(String.Format("{1}, Game Controls ({0})", this.controlInputs.GetLength(0), this.showGameControls ? "Close" : "Open")))
            {
                this.showGameControls = !this.showGameControls;
            }
            if (this.showGameControls)
            {
                for (int i = 0; i < this.controlInputs.GetLength(0); i++)
                {
                    string controlName = this.controlInputs[i, 0];
                    string primaryInput = this.controlInputs[i, 1];
                    string secondaryInput = this.controlInputs[i, 2];

                    GUILayout.BeginVertical("box", layoutOption);
                    GUILayout.Label(String.Format("{0}:", controlName));
                    this.drawCommonControl("Primary input", controlName, primaryInput, 1);
                    this.drawCommonControl("Secondary input", controlName, secondaryInput, 2);
                    GUILayout.EndVertical();
                }
            }

            #endregion

            GUILayout.Space(3f);

            #region Mod Controls / Keybinds

            if (GUILayout.Button(String.Format("{0}, Mod Keybinds ({1})", this.showModControls ? "Close" : "Open", this.modKeybindCount)))
            {
                this.showModControls = !this.showModControls;
            }
            if (this.showModControls)
            {
                foreach (Mod mod in ModLoader.LoadedMods)
                {
                    GUILayout.BeginVertical("box", layoutOption);
                    GUILayout.Label(String.Format("{0} v{1}: {2}", mod.Name, mod.Version, mod.Author));
                    foreach (Keybind kb in Keybind.Get(mod))
                    {
                        GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(CONTROLS_GUI_WIDTH - 40) });
                        GUILayout.Label(String.Format("{0}:", kb.Name));
                        this.drawCommonControl("Modifier", kb.ID, kb.Modifier.ToString(), 1, mod);
                        this.drawCommonControl("Input", kb.ID, kb.Key.ToString(), 2, mod);
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
            }

            #endregion

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        /// <summary>
        /// Draws the virtual xbox controller axes gui.
        /// </summary>
        private void drawVirtualXboxControllerAxesGui()
        {
            // Written, 10.07.2018

            bool isGreaterThanScreen = VIRTUAL_XBOX_AXES_GUI_HEIGHT >= Screen.height;
            float offset = isGreaterThanScreen ? 35 : GUI_SPACE;
            GUILayout.BeginArea(new Rect(CONTROLS_GUI_WIDTH + (GUI_SPACE * 2), GUI_SPACE, VIRTUAL_XBOX_AXES_GUI_WIDTH, (isGreaterThanScreen ? (Screen.height - (GUI_SPACE * 2)) : (VIRTUAL_XBOX_AXES_GUI_HEIGHT))));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH) });
            GUILayout.Label("Virtual xbox controller axes");
            this.virtualAxisGuiScrollPosition = GUILayout.BeginScrollView(this.virtualAxisGuiScrollPosition, false, false, new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH - SCROLLBAR_OFFSET) });
            foreach (XboxControl xboxControl in this.xboxController.getXboxControls())
            {
                if (xboxControl.controlType == XboxControlTypeEnum.Axis)
                {
                    GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH - offset) });
                    if (GUILayout.Button(xboxControl.texture, new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH - offset) }))
                    {
                        if (this.changeInputResult.reassignKey)
                        {
                            this.changeInput(xboxControl.inputName);
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        /// <summary>
        /// Draws the xbox controller debug gui.
        /// </summary>
        private void drawXboxControllerDebugGui()
        {
            // Written, 19.07.2018

            float left = this.changeInputResult.reassignKey && (bool)this.showVirtualGui.Value ? (CONTROLS_GUI_WIDTH + VIRTUAL_XBOX_AXES_GUI_WIDTH + (GUI_SPACE * 3)) : (CONTROLS_GUI_WIDTH + (GUI_SPACE * 2));
            bool isGreaterThanScreen = XBOX_CONTROLLER_DEBUG_GUI_HEIGHT >= Screen.height;
            float offset = isGreaterThanScreen ? 31 : GUI_SPACE;
            GUILayout.BeginArea(new Rect(left, GUI_SPACE, XBOX_CONTROLLER_DEBUG_GUI_WIDTH, (isGreaterThanScreen ? (Screen.height - (GUI_SPACE * 2)) : (XBOX_CONTROLLER_DEBUG_GUI_HEIGHT))));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
            this.xboxControllerDebugGuiScrollPosition = GUILayout.BeginScrollView(this.xboxControllerDebugGuiScrollPosition, false, false, new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - SCROLLBAR_OFFSET) });
            GUILayout.Label("Xbox Controller Debug");
            GUILayout.Space(5f);
            GUILayout.Label("Xbox Controller Status: " + (this.xboxController.isConnected ? "Connected" : "Disconnected"));
            if (this.xboxController.isConnected)
            {                
                // Triggers
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
                GUILayout.Label("Triggers:");
                GUILayout.Label(String.Format("Left: {0}\r\nRight: {1}", Math.Round(this.xboxController.getLeftTrigger(), 2), Math.Round(this.xboxController.getRightTrigger(), 2)));
                GUILayout.EndVertical();
                // Bumpers
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
                GUILayout.Label("Bumpers:");
                GUILayout.Label(String.Format("Left: {0}\r\nRight: {1}", this.xboxController.LB.state, this.xboxController.RB.state));
                GUILayout.EndVertical();
                // Left Thumbsick
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
                GUILayout.Label("Left Thumbstick:");
                GUILayout.Label(String.Format("X: {0}\r\nY: {1}\r\nLS: {2}", Math.Round(this.xboxController.getLeftStick().X, 2), Math.Round(this.xboxController.getLeftStick().Y, 2), this.xboxController.LS.state));
                GUILayout.EndVertical();
                // Right Thumbstick
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
                GUILayout.Label("Right Thumbstick:");
                GUILayout.Label(String.Format("X: {0}\r\nY: {1}\r\nRS: {2}", Math.Round(this.xboxController.getRightStick().X, 2), Math.Round(this.xboxController.getRightStick().Y, 2), this.xboxController.RS.state));
                GUILayout.EndVertical();
                // Buttons
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
                GUILayout.Label("Buttons");
                GUILayout.Label(String.Format("A: {0}\r\nB: {1}\r\nX: {2}\r\nY: {3}\r\nStart: {4}\r\nBack: {5}",
                    this.xboxController.A.state, this.xboxController.B.state, this.xboxController.X.state, this.xboxController.Y.state, this.xboxController.Start.state, this.xboxController.Back.state));
                GUILayout.EndVertical();
                // D-Pad
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
                GUILayout.Label("D-Pad");
                GUILayout.Label(
                    String.Format("Up: {0}\r\nDown: {1}\r\nLeft: {2}\r\nRight: {3}", this.xboxController.DPadUp.state, this.xboxController.DPadDown.state, this.xboxController.DPadLeft.state, this.xboxController.DPadRight.state));
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        /// <summary>
        /// Draws the mouse emulation gui.
        /// </summary>
        private void drawMouseEmulationGui()
        {
            // Written 03.08.2018

            float tempValue;
            bool saveSettings = false;
            bool isGreaterThanScreen = MOUSE_EMULATION_GUI_HEIGHT >= Screen.height;
            float offset = isGreaterThanScreen ? 31 : GUI_SPACE;
            GUILayout.BeginArea(new Rect((Screen.width - MOUSE_EMULATION_GUI_WIDTH - GUI_SPACE), GUI_SPACE, MOUSE_EMULATION_GUI_WIDTH, (isGreaterThanScreen ? (Screen.height - (GUI_SPACE * 2)) : (MOUSE_EMULATION_GUI_HEIGHT))));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(MOUSE_EMULATION_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
            this.mouseEmulatorGuiScrollPosition = 
                GUILayout.BeginScrollView(this.mouseEmulatorGuiScrollPosition, false, false, new GUILayoutOption[1] { GUILayout.Width(MOUSE_EMULATION_GUI_WIDTH - SCROLLBAR_OFFSET) });
            GUILayout.Label("Mouse Emulation");
            GUILayout.Space(5f);
            GUILayout.Label(String.Format("Mouse Deadzone: {0}", this.mouseEmulator.deadzone));
            tempValue = GUILayout.HorizontalSlider(this.mouseEmulator.deadzone, MouseEmulator.MIN_DEADZONE, MouseEmulator.MAX_DEADZONE);
            if (tempValue != this.mouseEmulator.deadzone) // Value Changed.
            {
                this.mouseEmulator.deadzone = tempValue;
                saveSettings = true;
            }
            GUILayout.Space(5f);
            GUILayout.Label(String.Format("Mouse Sensitivity: {0}", this.mouseEmulator.sensitivity));
            tempValue = GUILayout.HorizontalSlider(this.mouseEmulator.sensitivity, MouseEmulator.MIN_SENSITIVITY, MouseEmulator.MAX_SENSITIVITY);
            if (tempValue != this.mouseEmulator.sensitivity) // Value Changed.
            {
                this.mouseEmulator.sensitivity = tempValue;
                saveSettings = true;
            }
            GUILayout.Space(5f);
            if (GUILayout.Toggle(this.mouseEmulator.Emulating, String.Format("{0}: Using {1}", this.mouseEmulator.getEmulateMouseSettingName, this.mouseEmulator.inputType)) != this.mouseEmulator.Emulating)
            {
                this.mouseEmulator.Emulating = !this.mouseEmulator.Emulating;
                saveSettings = true;
            }
            if (this.mouseEmulator.Emulating)
            {
                // As left + right thumb stick settings are grouped; need to manually change other value..
                if (GUILayout.Toggle(this.mouseEmulator.useLeftThumbstick, String.Format("{0}: {1}", this.mouseEmulator.getUseLeftThumbstickSettingName, this.mouseEmulator.useLeftThumbstick ? "ON" : "")) != this.mouseEmulator.useLeftThumbstick)
                {
                    if (this.mouseEmulator.inputType != InputTypeEnum.LS)
                    {
                        this.mouseEmulator.useLeftThumbstick = !this.mouseEmulator.useLeftThumbstick;
                        this.mouseEmulator.useRightThumbstick = !this.mouseEmulator.useLeftThumbstick;
                        this.mouseEmulator.useDPad = !this.mouseEmulator.useLeftThumbstick;
                        saveSettings = true;
                    }
                }
                if (GUILayout.Toggle(this.mouseEmulator.useRightThumbstick, String.Format("{0}: {1}", this.mouseEmulator.getUseRightThumbstickSettingName, this.mouseEmulator.useRightThumbstick ? "ON" : "")) != this.mouseEmulator.useRightThumbstick)
                {
                    if (this.mouseEmulator.inputType != InputTypeEnum.RS)
                    {
                        this.mouseEmulator.useRightThumbstick = !this.mouseEmulator.useRightThumbstick;
                        this.mouseEmulator.useLeftThumbstick = !this.mouseEmulator.useRightThumbstick;
                        this.mouseEmulator.useDPad = !this.mouseEmulator.useRightThumbstick;
                        saveSettings = true;
                    }
                }
                if (GUILayout.Toggle(this.mouseEmulator.useDPad, String.Format("{0}: {1}", this.mouseEmulator.getUseDPadSettingName, this.mouseEmulator.useDPad ? "ON" : "")) != this.mouseEmulator.useDPad)
                {
                    if (this.mouseEmulator.inputType != InputTypeEnum.DPad)
                    {
                        this.mouseEmulator.useDPad = !this.mouseEmulator.useDPad;
                        this.mouseEmulator.useLeftThumbstick = !this.mouseEmulator.useDPad;
                        this.mouseEmulator.useRightThumbstick = !this.mouseEmulator.useDPad;
                        saveSettings = true;
                    }
                }
            }
            GUILayout.Space(5f);
            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(MOUSE_EMULATION_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
            GUILayout.Label(String.Format("{0}:", this.mouseEmulator.lmbPrimaryInput.Name));
            this.drawCommonControl("Modifier", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Modifier.ToString(), 1, this);
            this.drawCommonControl("Input", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Key.ToString(), 2, this);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(MOUSE_EMULATION_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
            GUILayout.Label(String.Format("{0}:", this.mouseEmulator.lmbSecondaryInput.Name));
            this.drawCommonControl("Modifier", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Modifier.ToString(), 1, this);
            this.drawCommonControl("Input", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Key.ToString(), 2, this);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(MOUSE_EMULATION_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
            GUILayout.Label(String.Format("{0}:", this.mouseEmulator.rmbPrimaryInput.Name));
            this.drawCommonControl("Modifier", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Modifier.ToString(), 1, this);
            this.drawCommonControl("Input", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Key.ToString(), 2, this);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(MOUSE_EMULATION_GUI_WIDTH - offset - SCROLLBAR_OFFSET) });
            GUILayout.Label(String.Format("{0}:", this.mouseEmulator.rmbSecondaryInput.Name));
            this.drawCommonControl("Modifier", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Modifier.ToString(), 1, this);
            this.drawCommonControl("Input", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Key.ToString(), 2, this);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            if (saveSettings)
            {
                ModSettings_menu.SaveSettings(this);
            }
        }
        /// <summary>
        /// Draws a common control for the gui.
        /// </summary>
        private void drawCommonControl(string title, string controlName, string inputName, int index, Mod mod = null)
        {
            // Written, 01.08.2018

            string reassignMessage = 
                this.changeInputResult.controlName == controlName 
                && this.changeInputResult.index == index 
                && this.changeInputResult.mod == mod ? "Awaiting key input" : "";
            GUILayout.Label(String.Format("{0}: {1}", title, reassignMessage));
            XboxControl xboxControl = this.xboxController.getXboxControlByInputName(inputName);
            if (xboxControl != null)
            {
                if (GUILayout.Button(xboxControl.texture))
                {
                    this.changeInputResult.changeToPollingState(controlName, index, mod);
                }
            }
            else
            {
                if (GUILayout.Button(inputName))
                {
                    this.changeInputResult.changeToPollingState(controlName, index, mod);
                }
            }
        }
        /// <summary>
        /// Evaluates the correct height for the game controls gui.
        /// </summary>
        private float evaluateGameControlsGuiHeight()
        {
            // Written, 29.07.2018

            float height;

            if (!this.showGameControls && !this.showModControls)
                height = 65f;
            else
                height = ((Screen.height - INFO_GUI_HEIGHT) + (this.showSettings ? 0 : SETTINGS_HEIGHT)) - (GUI_SPACE * 2);
            return height;
        }
        /// <summary>
        /// Performs a check to see if the mod loader version is the same as the currently supported version.
        /// </summary>
        private void performModLoaderVersionCheck()
        {
            // Written, 03.08.2018

            // Modloader supported version check.
            if (SUPPORTED_MODLOADER_VERSION != ModLoader.Version)
            {
                ModUI.ShowMessage(
                    String.Format("<b>[{0} <color=orange>v{1}]</color></b> - NOTE: modloader v{2} is <color=orange>not</color> currently <color=orange>supported</color> and/or may not be <color=orange>compatible</color>.\r\nSupported " +
                    "modloader version is <color=orange>v{2}</color>\r\nFeel free to use the mod but keep in mind that it might not work as expected.",
                    this.Name, this.Version, ModLoader.Version), "ModLoader Version not supported.");
            }
        }

        #endregion

        #region Override Methods

        public override void ModSettings()
        {
            // 23.07.2018

            Settings.AddCheckBox(this, showDebugGui);
            Settings.AddCheckBox(this, this.showVirtualGui);
            Settings.AddCheckBox(this, this.monitiorXboxControllerConnectionStatus);
            MouseEmulator.onModSettings(this);

        }
        public override void OnLoad()
        {
            // Written, 06.07.2018    

            // Getting the amount of keybinds within the current game instance. Doing that here as mods cannot be added while the game is running (currently).
            foreach (Mod mod in ModLoader.LoadedMods)
                this.modKeybindCount += Keybind.Get(mod).Count;

            instance = this;
            this.changeInputResult = new ChangeInput();
            Keybind.Add(this, this.openControlsGui);
            this.controlInputs = new string[this.inputNames.Length, 3];
            this.loadControlInputsFromCInput();
            cInput.OnKeyChanged += this.CInput_OnKeyChanged;
            this.xboxControllerManager = new XboxControllerManager(1);
            this.xboxController = this.xboxControllerManager.controllers[0];
            XboxControllerManager.ControllerConnected += this.XboxControllerManager_ControllerConnected;
            XboxControllerManager.ControllerDisconnected += this.XboxControllerManager_ControllerDisconnected;
            this.mouseEmulator = new MouseEmulator(DeadzoneTypeEnum.ScaledRadial);
            this.performModLoaderVersionCheck();
            ModConsole.Print(String.Format("{0} v{1}: Loaded", this.Name, this.Version));
        }
        public override void OnGUI()
        {
            // Written, 10.07.2018

            if (this.controlsGuiOpened)
            {
                this.drawControlsGui();
                if (this.changeInputResult.reassignKey)
                    if ((bool)this.showVirtualGui.Value)
                    this.drawVirtualXboxControllerAxesGui();
                if ((bool)this.showDebugGui.Value)
                    this.drawXboxControllerDebugGui();
                this.drawMouseEmulationGui();
            }
            
        }
        public override void Update()
        {
            // Update is called once per frame

            this.xboxControllerManager.onUpdate();
            this.mouseEmulator.onUpdate();
            if (this.openControlsGui.IsDown())
            {
                this.controlsGuiOpened = !this.controlsGuiOpened;

                if (this.controlsGuiOpened)
                {
                    FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = true;
                }
                else
                {
                    FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = false;
                }
            }
            if (this.changeInputResult.reassignKey)
            {
                MonitorInputData mid = Input.monitorForInput();
                if (mid.foundInput)
                    this.changeInput(mid.input);
            }
            this.xboxControllerManager.onRefresh();
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when cinput keys are changed externally, (the game gui controls).
        /// </summary>
        private void CInput_OnKeyChanged()
        {
            // Written, 09.07.2018

            ModConsole.Print("[CInput.dll] - 'OnKeyChanged' occured.");
            this.loadControlInputsFromCInput();
        }
        /// <summary>
        /// Occurs when a controller is disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XboxControllerManager_ControllerDisconnected(object sender, EventArgs e)
        {
            ModConsole.Print("Controller Disconnected");
        }
        /// <summary>
        /// Occurs when a controller is connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XboxControllerManager_ControllerConnected(object sender, EventArgs e)
        {
            ModConsole.Print("Controller Connected");
        }

        #endregion
    }
}   
