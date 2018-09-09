using System;
using System.Linq;
using UnityEngine;
using gui = UnityEngine.GUILayout;
using MSCLoader;
using HutongGames.PlayMaker;
using Mo_Controls.MouseEmulation;
using Mo_Controls.XboxController;
using xController = Mo_Controls.XboxController.XboxController;
using XInputDotNetPure;

namespace Mo_Controls.McGUI
{
    /// <summary>
    /// Represents the GUI for the mod, <see cref="Mo_Controls"/>.
    /// </summary>
    public class MoControlsGUI
    {
        // Written, 22.08.2018

        #region Fields

        #region GUI
        
        /// <summary>
        /// Represents the scrool position of the scroll bar.
        /// </summary>
        private Vector2 mainGUIScrollPosition;
        /// <summary>
        /// Represents whether the gui should be opened or closed.
        /// </summary>
        private bool controlsGuiOpened = false;
        /// <summary>
        /// Represents the main gui's width.
        /// </summary>
        private readonly float mainGuiWidth = (Screen.width - (MAIN_GUI_LEFT * 2));
        /// <summary>
        /// Represents the main gui's left.
        /// </summary>
        private const float MAIN_GUI_LEFT = 100f;
        /// <summary>
        /// Represents the main gui's top.
        /// </summary>
        private const float MAIN_GUI_TOP = 20f;
        /// <summary>
        /// Represents the menu gui's top.
        /// </summary>
        private const float MENU_GUI_TOP = 20f;
        /// <summary>
        /// Represents the menu gui's height.
        /// </summary>
        private const float MENU_GUI_HEIGHT = 22f;
        /// <summary>
        /// Represents the scroll bar's offset.
        /// </summary>
        private const float SCROLL_BAR_OFFSET = 25f;

        #endregion

        /// <summary>
        /// Represents the default color.
        /// </summary>
        private Color defaultColor = new Color(1, 1, 1);
        /// <summary>
        /// Represents the background color.
        /// </summary>
        private Color backGroundColor = new Color(0, 0, 0, 0.6f);
        /// <summary>
        /// Represents the amount of keybinds that have been loaded via the mods.
        /// </summary>
        private int modKeybindCount = 0;
        /// <summary>
        /// Represents the currently selected setting menu item.
        /// </summary>
        private SettingsMenuEnum settingsMenu;
        /// <summary>
        /// Represents the currently selected main menu item.
        /// </summary>
        private MainGUIMenuEnum mainGUIMenu;
        /// <summary>
        /// Represents the mod.
        /// </summary>
        private Mo_Controls mod;
        /// <summary>
        /// Represent whether the mod should display the virtual gui.
        /// </summary>
        public bool showVirtualGui;
        /// <summary>
        /// Represents a keybind to open/close the gui for the mod.
        /// </summary>
        public readonly Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.F12);

        #endregion

        #region Properties

        /// <summary>
        /// Represents the mouse emulator. references <see cref="Mo_Controls.mouseEmulator"/>.
        /// </summary>
        private MouseEmulator mouseEmulator
        {
            get
            {
                return this.mod.mouseEmulator;
            }
        }
        /// <summary>
        /// Represents the change input result. references <see cref="Mo_Controls.changeInputResult"/>.
        /// </summary>
        private ChangeInput changeInputResult
        {
            get
            {
                return this.mod.controlManager.changeInputResult;
            }
            set
            {
                this.mod.controlManager.changeInputResult = value;
            }
        }
        /// <summary>
        /// Represents the xbox controller. references <see cref="Mo_Controls.xboxController"/>.
        /// </summary>
        private xController xboxController
        {
            get
            {
                return this.mod.xboxController;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initliazes a new instance of <see cref="MoControlsGUI"/>.
        /// </summary>
        /// <param name="inMod">The mod.</param>
        public MoControlsGUI(Mo_Controls inMod)
        {
            // Written, 22.08.2018

            this.mod = inMod;
            Keybind.Add(this.mod, this.openControlsGui);
            // Getting the amount of keybinds within the current game instance. Doing that here as mods cannot be added while the game is running (currently).
            foreach (Mod mod in ModLoader.LoadedMods.Where(lm => Keybind.Get(mod).Count() > 0))
                this.modKeybindCount += Keybind.Get(mod).Count();
        }

        #endregion

        #region Methods

        /// <summary>
        /// on Update.
        /// </summary>
        public void onUpdate()
        {
            // Written, 22.08.2018

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
        }
        /// <summary>
        /// on GUI.
        /// </summary>
        public void onGUI()
        {
            // Written, 22.08.2018

            try
            {
                if (this.controlsGuiOpened)
                {
                    this.drawMainMenuGUI();
                    this.drawMainGUI();
                }
                else
                {
                    if (this.mod.controlManager.displayCurrentPlayerModeOverlay)
                    {
                        this.drawPlayerModeOverlayGUI();
                    }
                }
            }
            catch (Exception ex)
            {
                ModConsole.Error(ex.StackTrace);
                this.controlsGuiOpened = false;
            }
        }
        /// <summary>
        /// Draws the main menu for the main gui.
        /// </summary>
        private void drawMainMenuGUI()
        {
            // Written, 22.08.2018

            using (new gui.AreaScope(new Rect(MAIN_GUI_LEFT + 5f, MENU_GUI_TOP, this.mainGuiWidth - SCROLL_BAR_OFFSET, MENU_GUI_HEIGHT)))
            using (new gui.HorizontalScope())
            {
                foreach (MainGUIMenuEnum mainMenuItem in Enum.GetValues(typeof(MainGUIMenuEnum)))
                {
                    bool isSelected = (this.mainGUIMenu == mainMenuItem);
                    if (isSelected)
                        GUI.contentColor = Color.green;
                    if (gui.Button(mainMenuItem.toString()) && !isSelected)
                    {
                        this.mainGUIMenu = mainMenuItem;
                    }
                    if (isSelected)
                        GUI.contentColor = Color.white;
                }
            }
        }
        /// <summary>
        /// Draws the main gui and it's children.
        /// </summary>
        private void drawMainGUI()
        {
            // Written, 22.08.2018

            using (new gui.AreaScope(new Rect(MAIN_GUI_LEFT, (MENU_GUI_TOP + MENU_GUI_HEIGHT), this.mainGuiWidth, (Screen.height - (MAIN_GUI_TOP + MENU_GUI_TOP + MENU_GUI_HEIGHT)))))
            using (gui.ScrollViewScope scrollViewScope = new gui.ScrollViewScope(this.mainGUIScrollPosition, new GUILayoutOption[] { gui.Width(this.mainGuiWidth) }))
            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width(this.mainGuiWidth - SCROLL_BAR_OFFSET) }))
            {
                this.mainGUIScrollPosition = scrollViewScope.scrollPosition;
                gui.Label(String.Format("<b>{0} v{1} by {2}</b>", this.mod.Name, this.mod.Version, this.mod.Author));
                if (this.mainGUIMenu != MainGUIMenuEnum.About)
                    gui.Label(String.Format("<b>{0}</b> is a forbidden key (GUI key bind).\r\n<b>{1}</b> Sets as None.\r\n<b>LMB</b> Selects.\r\n<b>RMB</b> Cancels.", this.openControlsGui.Key, Input.noneKey));
                gui.Space(3.0f);
                switch (this.mainGUIMenu)
                {
                    case MainGUIMenuEnum.About:
                        this.drawAboutContent();
                        break;
                    case MainGUIMenuEnum.DrivingControls:
                        this.drawDrivingControlsContent();
                        break;
                    case MainGUIMenuEnum.FootControls:
                        this.drawFootControlsContent();
                        break;
                    case MainGUIMenuEnum.Settings:
                        this.drawSettingsContent();
                        break;
                    case MainGUIMenuEnum.ModControls:
                        this.drawModContent();
                        break;
                }
            }
        }
        /// <summary>
        /// Draws about content to the main gui.
        /// </summary>
        private void drawAboutContent()
        {
            // Written, 20.08.2018

            string aboutMessage = "<b>Mo'Controls</b>, assign your driving controls to both, the keyboard and an Xbox controller at the same time! Control/Move the players " +
                "camera/head around with an Xbox Controller!";
            string[] features = new string[] 
            {
                "Xbox Controller Support",
                "Assign two inputs to each game control",
                "Mouse Emulation; Use your Xbox Controller to control the mouse",                
                "Split control modes for driving & walking",
            };
            string footerMessage = "Developed by <b>Tommo J. Armytage</b>";
            string joinPrefix = "\r\n# ";

            gui.Space(5f);
            gui.Label(String.Format("<b>About:</b>\r\n\r\n{0}", aboutMessage));
            gui.Space(5f);
            gui.Label(String.Format("<b>Features:</b>\r\n{0}{1}", joinPrefix, String.Join(joinPrefix, features)));
            gui.Space(10f);
            gui.Label(footerMessage);
        }
        /// <summary>
        /// Draws foot controls content to the main gui.
        /// </summary>
        private void drawFootControlsContent()
        {
            // Written, 02.09.2018

            this.drawControlModeContent("Foot Controls", this.mod.controlManager.footControls);
        }
        /// <summary>
        /// Draws driving controls content to the main gui.
        /// </summary>
        private void drawDrivingControlsContent()
        {
            // Written, 02.09.2018

            this.drawControlModeContent("Driving Controls", this.mod.controlManager.drivingControls);
        }
        /// <summary>
        /// Draws settings content to the main gui.
        /// </summary>
        private void drawSettingsContent()
        {
            // Written, 20.08.2018

            this.drawSettingsMenu();

            switch (this.settingsMenu)
            {
                case SettingsMenuEnum.MouseEmulation:
                    this.drawMouseEmulationContent();
                    break;
                case SettingsMenuEnum.Debug:
                    this.drawDebugContent();
                    break;
                case SettingsMenuEnum.Display:
                    this.drawDisplayContent();
                    break;
            }
        }
        /// <summary>
        /// Draws the settings menu bar on the settings tab.
        /// </summary>
        private void drawSettingsMenu()
        {
            // Written, 20.08.2018

            using (new gui.HorizontalScope("box"))
            {
                foreach (SettingsMenuEnum settingsMenuItem in Enum.GetValues(typeof(SettingsMenuEnum)))
                {
                    bool isSelected = (this.settingsMenu == settingsMenuItem);
                    if (isSelected)
                        GUI.contentColor = Color.green;
                    if (gui.Button(settingsMenuItem.toString()) && !isSelected)
                    {
                        this.settingsMenu = settingsMenuItem;
                    }
                    if (isSelected)
                        GUI.contentColor = Color.white;
                }
            }
        }
        /// <summary>
        /// Draws debug content to the main gui.
        /// </summary>
        private void drawDebugContent()
        {
            // Written, 20.08.2018

            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width(250f) }))
            {
                gui.Label("Xbox Controller Debug");
                gui.Space(5f);
                gui.Label("Xbox Controller Status: " + (this.xboxController.isConnected ? "Connected" : "Disconnected"));
                if (this.xboxController.isConnected)
                {
                    // Triggers
                    using (new gui.VerticalScope("box"))
                    {
                        double leftRounded = Math.Round(this.xboxController.getLeftTrigger(), 2);
                        double rightRounded = Math.Round(this.xboxController.getRightTrigger(), 2);
                        gui.Label("Triggers:");
                        gui.Label(String.Format("Left: {0}\r\nRight: {1}", 
                            leftRounded > 0.0f || leftRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", leftRounded) : leftRounded.ToString(),
                            rightRounded > 0.0f || rightRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", rightRounded) : rightRounded.ToString()));
                    }
                    // Bumpers
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState lb = this.xboxController.LB.state;
                        ButtonState rb = this.xboxController.RB.state;
                        gui.Label("Bumpers:");
                        gui.Label(String.Format("Left: {0}\r\nRight: {1}",
                            lb == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", lb) : lb.ToString(),
                            rb == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", rb) : rb.ToString()));
                    }
                    // Left Thumbsick
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        double xRounded = Math.Round(this.xboxController.getLeftStick().X, 2);
                        double yRounded = Math.Round(this.xboxController.getLeftStick().Y, 2);
                        ButtonState ls = this.xboxController.LS.state;
                        gui.Label("Left Thumbstick:");
                        gui.Label(String.Format("X: {0}\r\nY: {1}\r\nLS: {2}",
                            xRounded > 0.0f || xRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", xRounded) : xRounded.ToString(),
                            yRounded > 0.0f || yRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", yRounded) : yRounded.ToString(),
                            ls == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", ls) : ls.ToString()));
                    }
                    // Right Thumbstick
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        double xRounded = Math.Round(this.xboxController.getRightStick().X, 2);
                        double yRounded = Math.Round(this.xboxController.getRightStick().Y, 2);
                        ButtonState rs = this.xboxController.RS.state;
                        gui.Label("Right Thumbstick:");
                        gui.Label(String.Format("X: {0}\r\nY: {1}\r\nRS: {2}",
                            xRounded > 0.0f || xRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", xRounded) : xRounded.ToString(),
                            yRounded > 0.0f || yRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", yRounded) : yRounded.ToString(),
                            rs == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", rs) : rs.ToString()));
                    }
                    // Buttons
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState a = this.xboxController.A.state;
                        ButtonState b = this.xboxController.B.state;
                        ButtonState x = this.xboxController.X.state;
                        ButtonState y = this.xboxController.Y.state;
                        ButtonState start = this.xboxController.Start.state;
                        ButtonState back = this.xboxController.Back.state;
                        gui.Label("Buttons");
                        gui.Label(String.Format("A: {0}\r\nB: {1}\r\nX: {2}\r\nY: {3}\r\nStart: {4}\r\nBack: {5}",
                            a == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", a) : a.ToString(),
                            b == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", b) : b.ToString(),
                            x == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", x) : x.ToString(),
                            y == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", y) : y.ToString(),
                            start == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", start) : start.ToString(),
                            back == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", back) : back.ToString()));
                    }
                    // D-Pad
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState u = this.xboxController.DPadUp.state;
                        ButtonState d = this.xboxController.DPadDown.state;
                        ButtonState l = this.xboxController.DPadLeft.state;
                        ButtonState r = this.xboxController.DPadRight.state;
                        gui.Label("D-Pad");
                        gui.Label(String.Format("Up: {0}\r\nDown: {1}\r\nLeft: {2}\r\nRight: {3}",
                            u == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", u) : u.ToString(),
                            d == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", d) : d.ToString(),
                            l == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", l) : l.ToString(),
                            r == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", r) : r.ToString()));
                    }
                }
            }
        }
        /// <summary>
        /// Draws mouse emulation content to the main gui.
        /// </summary>
        private void drawMouseEmulationContent()
        {
            // Written, 22.08.2018

            float tempValue;
            bool saveSettings = false;

            gui.Space(5f);
            gui.Label(String.Format("Mouse Deadzone: {0}", this.mouseEmulator.deadzone));
            tempValue = gui.HorizontalSlider(this.mouseEmulator.deadzone, MouseEmulator.MIN_DEADZONE, MouseEmulator.MAX_DEADZONE);
            if (tempValue != this.mouseEmulator.deadzone) // Value Changed.
            {
                this.mouseEmulator.deadzone = tempValue;
                saveSettings = true;
            }
            gui.Space(5f);
            gui.Label(String.Format("Mouse Sensitivity: {0}", this.mouseEmulator.sensitivity));
            tempValue = gui.HorizontalSlider(this.mouseEmulator.sensitivity, MouseEmulator.MIN_SENSITIVITY, MouseEmulator.MAX_SENSITIVITY);
            if (tempValue != this.mouseEmulator.sensitivity) // Value Changed.
            {
                this.mouseEmulator.sensitivity = tempValue;
                saveSettings = true;
            }
            gui.Space(5f);
            if (gui.Toggle(this.mouseEmulator.Emulating, String.Format("{0}: Using {1}", "Emulate mouse for controller", this.mouseEmulator.inputType)) != this.mouseEmulator.Emulating)
            {
                this.mouseEmulator.Emulating = !this.mouseEmulator.Emulating;
                saveSettings = true;
            }
            if (this.mouseEmulator.Emulating)
            {
                // As left + right thumb stick settings are grouped; need to manually change other value..
                bool _asInput = (this.mouseEmulator.inputType == InputTypeEnum.LS);
                if (gui.Toggle(_asInput, String.Format("Use LS: {0}", _asInput ? "ON" : "")) != _asInput)
                {
                    if (this.mouseEmulator.inputType != InputTypeEnum.LS)
                    {
                        this.mouseEmulator.inputType = InputTypeEnum.LS;
                        saveSettings = true;
                    }
                }
                _asInput = (this.mouseEmulator.inputType == InputTypeEnum.RS);
                if (gui.Toggle(_asInput, String.Format("Use RS: {0}", _asInput ? "ON" : "")) != _asInput)
                {
                    if (this.mouseEmulator.inputType != InputTypeEnum.RS)
                    {
                        this.mouseEmulator.inputType = InputTypeEnum.RS;
                        saveSettings = true;
                    }
                }
                _asInput = (this.mouseEmulator.inputType == InputTypeEnum.DPad);
                if (gui.Toggle(_asInput, String.Format("Use D-Pad: {0}", _asInput ? "ON" : "")) != _asInput)
                {
                    if (this.mouseEmulator.inputType != InputTypeEnum.DPad)
                    {
                        this.mouseEmulator.inputType = InputTypeEnum.DPad;
                        saveSettings = true;
                    }
                }
            }
            gui.Space(5f);
            gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbPrimaryInput.Name));
            using (new gui.HorizontalScope("box"))
            {
                this.drawCommonControl("Modifier", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                this.drawCommonControl("Input", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
            }
            gui.Space(3f);
            gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbSecondaryInput.Name));
            using (new gui.HorizontalScope("box"))
            {
                this.drawCommonControl("Modifier", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                this.drawCommonControl("Input", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
            }
            gui.Space(3f);
            gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.rmbPrimaryInput.Name));
            using (new gui.HorizontalScope("box"))
            {
                this.drawCommonControl("Modifier", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                this.drawCommonControl("Input", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
            }
            gui.Space(3f);
            gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.rmbSecondaryInput.Name));
            using (new gui.HorizontalScope("box"))
            {
                this.drawCommonControl("Modifier", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                this.drawCommonControl("Input", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
            }
            gui.Space(5f);
            if (saveSettings)
            {
                MoControlsSaveData.saveSettings(this.mod);
            }
        }
        /// <summary>
        /// Draws the virtual xbox controller axes gui.
        /// </summary>
        private void drawVirtualXboxControllerAxesContent()
        {
            // Written, 10.07.2018

            gui.Label("Virtual xbox controller axes");
            foreach (XboxControl xboxControl in this.xboxController.getXboxControls())
            {
                if (xboxControl.controlType == XboxControlTypeEnum.Axis)
                {
                    using (new gui.VerticalScope("box"))
                    {
                        if (gui.Button(xboxControl.texture))
                        {
                            if (this.changeInputResult.reassignKey)
                            {
                                this.changeInput(xboxControl.inputName);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Draws display settings content to the main gui.
        /// </summary>
        private void drawDisplayContent()
        {
            // Written, 22.08.2018

            bool _saveSettings = false;

            if (gui.Toggle(this.mod.controlManager.displayCurrentPlayerModeOverlay, "Display current player mode overlay") != this.mod.controlManager.displayCurrentPlayerModeOverlay)
            {
                this.mod.controlManager.displayCurrentPlayerModeOverlay = !this.mod.controlManager.displayCurrentPlayerModeOverlay;
                _saveSettings = true;
            }

            if (_saveSettings)
            {
                MoControlsSaveData.saveSettings(this.mod);
            }
        }
        /// <summary>
        /// Draws the player mode overlay.
        /// </summary>
        private void drawPlayerModeOverlayGUI()
        {
            // Written, 22.08.2018

            using (new gui.AreaScope(new Rect((Screen.width / 2), 1, 50, 20)))
            {
                gui.Label(ControlManager.playerMode.ToString());
            }
        }
        /// <summary>
        /// Draws a common control for the gui.
        /// </summary>
        private void drawCommonControl(string inTitle, string inControlName, string inInputName, int inIndex, PlayerModeEnum? inMode = null, Mod inMod = null)
        {
            // Written, 01.08.2018
            
            string reassignMessage =
                this.changeInputResult.controlName == inControlName
                && this.changeInputResult.index == inIndex
                && this.changeInputResult.mod == inMod 
                && this.changeInputResult.mode == inMode ? "<b>Awaiting key input</b>" : null;            
            XboxControl xboxControl = this.xboxController.getXboxControlByInputName(inInputName);
            bool buttonClicked = false;
            if (xboxControl != null && reassignMessage == null)
            {
                if (gui.Button(xboxControl.texture))
                {
                    buttonClicked = true;
                }
            }
            else
            {
                if (gui.Button(reassignMessage ?? inInputName))
                {
                    buttonClicked = true;
                }
            }
            if (buttonClicked)
            {
                this.changeInputResult.changeToPollingState(inControlName, inIndex, inMode, inMod);
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

                PlayerModeEnum? playerMode = this.changeInputResult.mode;

                if (playerMode == null)
                {
                    bool mistake = true;
                    ModUI.ShowYesNoMessage("Player Mode was null, is that right?", "Mistake?", delegate ()
                    {
                        mistake = false;
                    });
                    if (!mistake)
                    {

                        if (this.changeInputResult.index == 1)
                        {
                            cInput.ChangeKey(this.changeInputResult.controlName, input, cInput.GetText(this.changeInputResult.controlName, 2));
                        }
                        else
                        {
                            cInput.ChangeKey(this.changeInputResult.controlName, cInput.GetText(this.changeInputResult.controlName, 1), input);
                        }
                        this.mod.controlManager.currentControls = ControlManager.loadControlInputsFromCInput();
                    }
                }
                else
                {
                    this.mod.controlManager.setGameControl((PlayerModeEnum)playerMode, this.changeInputResult.controlName, this.changeInputResult.index, input);
                }
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
        /// draws the control input list.
        /// </summary>
        /// <param name="inControlInputs">The list to draw.</param>
        private void drawControlModeContent(string inTitle, string[,] inControlInputs)
        {
            // Written, 02.09.2018

            gui.Space(3f);
            gui.Label(String.Format("<b>{0}</b>", inTitle));
            gui.Space(5f);
            for (int i = 0; i < inControlInputs.GetLength(0); i++)
            {
                string controlName = inControlInputs[i, 0];
                gui.Space(3f);
                GUI.backgroundColor = this.backGroundColor;
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>{0}:</b>", controlName.gameControlToString(true)));
                    GUI.backgroundColor = this.defaultColor;
                    using (new gui.HorizontalScope())
                    {
                        bool isControls = (this.mainGUIMenu == MainGUIMenuEnum.FootControls);
                        PlayerModeEnum? playerMode;
                        if (isControls)
                            playerMode = PlayerModeEnum.OnFoot;
                        else
                        {
                            isControls = (this.mainGUIMenu == MainGUIMenuEnum.DrivingControls);
                            if (isControls)
                                playerMode = PlayerModeEnum.Driving;
                            else
                                playerMode = null;
                        }

                        this.drawCommonControl("Primary Input", controlName, inControlInputs[i, 1], 1, playerMode);
                        this.drawCommonControl("Secondary Input", controlName, inControlInputs[i, 2], 2, playerMode);
                    }
                }                
            }
            gui.Space(3f);
        }
        /// <summary>
        /// Draws all mod keybinds.
        /// </summary>
        private void drawModContent()
        {
            // Written, 09.09.2018

            gui.Space(3f);
            gui.Label(String.Format("<b>Total Mod Keybinds: {0}</b>", modKeybindCount));
            foreach (Mod _mod in ModLoader.LoadedMods)
            {
                Keybind[] modKeybinds = Keybind.Get(_mod).ToArray();

                if (modKeybinds.Count() > 0)
                {
                    gui.Space(3f);
                    GUI.backgroundColor = this.backGroundColor;
                    using (new gui.HorizontalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}</b>, by <b>{1}</b>:", _mod.Name, _mod.Author));
                        GUI.backgroundColor = this.defaultColor;
                        using (new gui.VerticalScope("box"))
                        {                            
                            for (int i = 0; i < modKeybinds.Length; i++)
                            {
                                gui.Label(String.Format("<b>{0}:</b>", modKeybinds[i].Name));
                                using (new gui.HorizontalScope())
                                {
                                    this.drawCommonControl("Modifier", modKeybinds[i].ID, modKeybinds[i].Modifier.ToString(), 1, inMod: _mod);
                                    this.drawCommonControl("Input", modKeybinds[i].ID, modKeybinds[i].Key.ToString(), 2, inMod: _mod);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
