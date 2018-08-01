using System;
using System.Linq;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using XInputDotNetPure;
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
        public override string Version => "1.0.2";
        public override bool UseAssetsFolder => true;

        #endregion

        #region Fields / Properties

        #region Xbox Related

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
        private xController xboxController
        {
            get;
            set;
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
        private ChangeInputResult changeInputResult
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
        private readonly Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.F12);
        /// <summary>
        /// Represents whether the mod should display debug info or not.
        /// </summary>
        private Settings showDebugGui = new Settings("showDebug", "Show debug?", true);
        /// <summary>
        /// Represent whether the mod should display the virtual gui.
        /// </summary>
        private Settings showVirtualGui = new Settings("", "Show xbox controller virtual gui?", false);

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

        // Constants
        
        /// <summary>
        /// Represents the width for the main (game/mod controls/keybinds) gui.
        /// </summary>
        private const float CONTROLS_GUI_WIDTH = 400f;
        /// <summary>
        /// Represents the height of the info gui.
        /// </summary>
        private const float INFO_GUI_HEIGHT = 200f;
        /// <summary>
        /// Represents the width for the virtual axes gui.
        /// </summary>
        private const float VIRTUAL_XBOX_AXES_GUI_WIDTH = 180f;
        /// <summary>
        /// Represents the height for the virtual axes gui.
        /// </summary>
        private const float VIRTUAL_XBOX_AXES_GUI_HEIGHT = 600f;
        /// <summary>
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
        private const float SETTINGS_HEIGHT = 70f;

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
        /// Monitors for input.
        /// </summary>
        private void monitorForInput()
        {
            // Written, 09.07.2018

            if (Input.anyKeyDown)
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                    {
                        if (kcode != selectKey) //Select key
                        {
                            if (kcode == noneKey) // Set as none key
                            {
                                this.changeInput(KeyCode.None.ToString());
                            }
                            else
                            {
                                if (kcode != this.openControlsGui.Key && kcode != cancelKey) // not allowed
                                {
                                    this.changeInput(kcode.ToString());
                                }
                                else
                                {
                                    this.changeInputResult = new ChangeInputResult();
                                }
                            }
                            return;
                        }
                        else
                            return;
                    }
                }
            }
            else
            {
                // Check xbox controller for input.

                if (this.xboxController.getRightTrigger() > 0.5f)
                {
                    this.changeInput(this.xboxController.RT.inputName);
                }
                else
                {
                    if (this.xboxController.getLeftTrigger() > 0.5f)
                    {
                        this.changeInput(this.xboxController.LT.inputName);
                    }
                    else
                    {
                        if (this.xboxController.DPadUp.state == ButtonState.Pressed)
                        {
                            this.changeInput(this.xboxController.DPadUp.inputName);
                        }
                        else
                        {
                            if (this.xboxController.DPadDown.state == ButtonState.Pressed)
                            {
                                this.changeInput(this.xboxController.DPadDown.inputName);
                            }
                            else
                            {
                                if (this.xboxController.DPadLeft.state == ButtonState.Pressed)
                                {
                                    this.changeInput(this.xboxController.DPadLeft.inputName);
                                }
                                else
                                {
                                    if (this.xboxController.DPadRight.state == ButtonState.Pressed)
                                    {
                                        this.changeInput(this.xboxController.DPadRight.inputName);
                                    }
                                    else
                                    {
                                        if (this.xboxController.getLeftStick().X > 0.0f)
                                        {
                                            this.changeInput(this.xboxController.leftThumbstick.right.inputName);
                                        }
                                        else
                                        {
                                            if (this.xboxController.getLeftStick().X < 0.0f)
                                            {
                                                this.changeInput(this.xboxController.leftThumbstick.left.inputName);
                                            }
                                            else
                                            {
                                                if (this.xboxController.getLeftStick().Y > 0.0f)
                                                {
                                                    this.changeInput(this.xboxController.leftThumbstick.up.inputName);
                                                }
                                                else
                                                {
                                                    if (this.xboxController.getLeftStick().Y < 0.0f)
                                                    {
                                                        this.changeInput(this.xboxController.leftThumbstick.down.inputName);
                                                    }
                                                    else
                                                    {
                                                        if (this.xboxController.getRightStick().X > 0.0f)
                                                        {
                                                            this.changeInput(this.xboxController.rightThumbstick.right.inputName);
                                                        }
                                                        else
                                                        {
                                                            if (this.xboxController.getRightStick().X < 0.0f)
                                                            {
                                                                this.changeInput(this.xboxController.rightThumbstick.left.inputName);
                                                            }
                                                            else
                                                            {
                                                                if (this.xboxController.getRightStick().Y > 0.0f)
                                                                {
                                                                    this.changeInput(this.xboxController.rightThumbstick.up.inputName);
                                                                }
                                                                else
                                                                {
                                                                    if (this.xboxController.getRightStick().Y < 0.0f)
                                                                    {
                                                                        this.changeInput(this.xboxController.rightThumbstick.down.inputName);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
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
            this.changeInputResult = new ChangeInputResult();
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
                if (GUILayout.Toggle((bool)this.showDebugGui.Value, "Show Debug") != (bool)this.showDebugGui.Value)
                {
                    this.showDebugGui.Value = !(bool)this.showDebugGui.Value;
                    saveSettings = true;
                }
                if (GUILayout.Toggle((bool)this.showVirtualGui.Value, "Show Virtual Axes GUI") != (bool)this.showVirtualGui.Value)
                {
                    this.showVirtualGui.Value = !(bool)this.showVirtualGui.Value;
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

            GUILayout.BeginArea(new Rect(GUI_SPACE, INFO_GUI_HEIGHT + GUI_SPACE - (this.showSettings ? 0 : SETTINGS_HEIGHT), CONTROLS_GUI_WIDTH, this.evaluateGameControlsGuiHeight())/*new Rect(GUI_SPACE, (INFO_GUI_HEIGHT + GUI_SPACE - (this.showSettings ? 0 : SETTINGS_HEIGHT)), CONTROLS_GUI_WIDTH, Screen.height - INFO_GUI_HEIGHT - (GUI_SPACE * 2))*/);
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(CONTROLS_GUI_WIDTH) });
            this.controlsGuiScrollPosition = GUILayout.BeginScrollView(this.controlsGuiScrollPosition, false, true, new GUILayoutOption[1] { GUILayout.Width(CONTROLS_GUI_WIDTH - 7) });

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

            GUILayout.BeginArea(new Rect(CONTROLS_GUI_WIDTH + (GUI_SPACE * 2), GUI_SPACE, VIRTUAL_XBOX_AXES_GUI_WIDTH, VIRTUAL_XBOX_AXES_GUI_HEIGHT));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH) });
            GUILayout.Label("Virtual xbox controller axes");
            this.virtualAxisGuiScrollPosition = GUILayout.BeginScrollView(this.virtualAxisGuiScrollPosition, false, true, new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH) });
            foreach (XboxControl xboxControl in this.xboxController.getXboxControls())
            {
                if (xboxControl.controlType == XboxControlTypeEnum.Axis)
                {
                    GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH - 35) });
                    if (GUILayout.Button(xboxControl.texture, new GUILayoutOption[1] { GUILayout.Width(VIRTUAL_XBOX_AXES_GUI_WIDTH - 35) }))
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
            GUILayout.BeginArea(new Rect(left, GUI_SPACE, XBOX_CONTROLLER_DEBUG_GUI_WIDTH, XBOX_CONTROLLER_DEBUG_GUI_HEIGHT));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH) });   
            GUILayout.Label("Xbox Controller Debug");
            GUILayout.Space(5f);
            GUILayout.Label("Xbox Controller Status: " + (this.xboxController.isConnected ? "Connected" : "Disconnected"));
            if (this.xboxController.isConnected)
            {                
                // Triggers
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - GUI_SPACE) });
                GUILayout.Label("Triggers:");
                GUILayout.Label(String.Format("Left: {0}\r\nRight: {1}", Math.Round(this.xboxController.getLeftTrigger(), 2), Math.Round(this.xboxController.getRightTrigger(), 2)));
                GUILayout.EndVertical();
                // Bumpers
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - GUI_SPACE) });
                GUILayout.Label("Bumpers:");
                GUILayout.Label(String.Format("Left: {0}\r\nRight: {1}", this.xboxController.LB.state, this.xboxController.RB.state));
                GUILayout.EndVertical();
                // Left Thumbsick
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - GUI_SPACE) });
                GUILayout.Label("Left Thumbstick:");
                GUILayout.Label(String.Format("X: {0}\r\nY: {1}\r\nLS: {2}", Math.Round(this.xboxController.getLeftStick().X, 2), Math.Round(this.xboxController.getLeftStick().Y, 2), this.xboxController.LS.state));
                GUILayout.EndVertical();
                // Right Thumbstick
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - GUI_SPACE) });
                GUILayout.Label("Right Thumbstick:");
                GUILayout.Label(String.Format("X: {0}\r\nY: {1}\r\nRS: {2}", Math.Round(this.xboxController.getRightStick().X, 2), Math.Round(this.xboxController.getRightStick().Y, 2), this.xboxController.RS.state));
                GUILayout.EndVertical();
                // Buttons
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - GUI_SPACE) });
                GUILayout.Label("Buttons");
                GUILayout.Label(String.Format("A: {0}\r\nB: {1}\r\nX: {2}\r\nY: {3}\r\nStart: {4}\r\nBack: {5}",
                    this.xboxController.A.state, this.xboxController.B.state, this.xboxController.X.state, this.xboxController.Y.state, this.xboxController.Start.state, this.xboxController.Back.state));
                GUILayout.EndVertical();
                // D-Pad
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(XBOX_CONTROLLER_DEBUG_GUI_WIDTH - GUI_SPACE) });
                GUILayout.Label("D-Pad");
                GUILayout.Label(
                    String.Format("Up: {0}\r\nDown: {1}\r\nLeft: {2}\r\nRight: {3}", this.xboxController.DPadUp.state, this.xboxController.DPadDown.state, this.xboxController.DPadLeft.state, this.xboxController.DPadRight.state));
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
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
                height = ((Screen.height - INFO_GUI_HEIGHT) + (this.showSettings ? 0 : SETTINGS_HEIGHT)) - GUI_SPACE * 2;
            return height;
        }

        #endregion

        #region Override Methods

        public override void ModSettings()
        {
            // 23.07.2018

            Settings.AddCheckBox(this, showDebugGui);
            Settings.AddCheckBox(this, this.showVirtualGui);
        }
        public override void OnLoad()
        {
            // Written, 06.07.2018    

            // Getting the amount of keybinds within the current game instance. Doing that here as mods cannot be added while the game is running (currently).
            foreach (Mod mod in ModLoader.LoadedMods)
                this.modKeybindCount += Keybind.Get(mod).Count;

            instance = this;
            this.changeInputResult = new ChangeInputResult();
            Keybind.Add(this, this.openControlsGui);
            this.controlInputs = new string[this.inputNames.Length, 3];
            this.loadControlInputsFromCInput();
            cInput.OnKeyChanged += this.CInput_OnKeyChanged;
            this.xboxControllerManager = new XboxControllerManager(1);
            this.xboxController = this.xboxControllerManager.controllers[0];
            XboxControllerManager.ControllerConnected += this.XboxControllerManager_ControllerConnected;
            XboxControllerManager.ControllerDisconnected += this.XboxControllerManager_ControllerDisconnected;
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
            }
            
        }
        public override void Update()
        {
            // Update is called once per frame

            this.xboxControllerManager.update();

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
                this.monitorForInput();
            }

            XboxButtonEnum xboxButtonDown = this.xboxController.getAnyButtonPressed();
            if (xboxButtonDown != XboxButtonEnum.NULL)
            {
                ModConsole.Print(xboxButtonDown.toString());
            }

            this.xboxControllerManager.refresh();
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
