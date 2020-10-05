 using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using gui = UnityEngine.GUILayout;
using ueGUI = UnityEngine.GUI;
using MSCLoader;
using HutongGames.PlayMaker;
using TommoJProductions.MoControls.MouseEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using XInputDotNetPure;

namespace TommoJProductions.MoControls.GUI
{
    /// <summary>
    /// Represents the GUI for the mod, <see cref="MoControlsMod"/>.
    /// </summary>
    public class MoControlsGUI : MonoBehaviour
    {
        // Written, 22.08.2018

        #region Fields

        #region GUI

        /// <summary>
        /// Represents the scrool position of the scroll bar.
        /// </summary>
        private Vector2 mainGUIScrollPosition;
        /// <summary>
        /// Represents the main gui's width.
        /// </summary>
        private readonly float mainGuiWidth = Screen.width - (mainGuiLeft * 2);
        /// <summary>
        /// Represents the main gui's left.
        /// </summary>
        private static readonly float mainGuiLeft = Screen.currentResolution.width / 10.8f;
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
        private const float MENU_GUI_HEIGHT = 30f;
        /// <summary>
        /// Represents the scroll bar's offset.
        /// </summary>
        private const float SCROLL_BAR_OFFSET = 25f;

        #endregion

        #region GUI Colors

        private Color32 backgroundColor = new Color32(112, 112, 255, 255); // | Light-ist Blue
        private Color32 primaryItemColor = new Color32(133, 162, 250, 255); // | Light-Vague Blue
        private Color32 secondaryItemColor = new Color32(69, 114, 247, 255); // | Light Blue
        private Color32 unselectedMenuButtonColor = new Color32(133, 162, 250, 255); // | Light-Vague Blue
        private Color32 selectedMenuButtonColor = new Color32(80, 237, 90, 255); // Green
        private Color32 defaultContentColor = new Color32(255, 255, 255, 255); // | White
        private Color xboxButtonPressedColor = Color.gray;
        private Color32 moduleBackgroundColor = new Color32(85, 125, 170, 255); // | Soild Light-tone blue

        #endregion

        #region KeyBind fields

        /// <summary>
        /// Represents whether the instance has calculated the amount of keybinds yet.
        /// </summary>
        private bool hasCountedModKeybinds = false;
        /// <summary>
        /// Represents the amount of keybinds that have been loaded via the mods.
        /// </summary>
        private int modKeybindCount = 0;
        /// <summary>
        /// Represents a keybind to open/close the gui for the mod.
        /// </summary>
        public readonly Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.Home);

        #endregion

        #region Relevant controls 

        /// <summary>
        /// Represents all relevant controls when in "on foot mode".
        /// </summary>
        private List<GameControlsEnum> relevantFootControls = new List<GameControlsEnum>()
        {
            GameControlsEnum.PlayerUp,
            GameControlsEnum.PlayerDown,
            GameControlsEnum.PlayerLeft,
            GameControlsEnum.PlayerRight,
            GameControlsEnum.DrivingMode,
            GameControlsEnum.Drunk,
            GameControlsEnum.Crouch,
            GameControlsEnum.Hit,
            GameControlsEnum.Hitchhike,
            GameControlsEnum.Jump,
            GameControlsEnum.Push,
            GameControlsEnum.Finger,
            GameControlsEnum.ReachLeft,
            GameControlsEnum.Run,
            GameControlsEnum.Smoke,
            GameControlsEnum.Swear,
            GameControlsEnum.Use,
            GameControlsEnum.Urinate,
            GameControlsEnum.WristWatch,
            GameControlsEnum.Zoom
        };
        /// <summary>
        /// Represents all relevant controls when in "driving mode".
        /// </summary>
        private List<GameControlsEnum> relevantDrivingControls = new List<GameControlsEnum>()
        {
            GameControlsEnum.Boost,
            GameControlsEnum.Brake,
            GameControlsEnum.Clutch,
            GameControlsEnum.DrivingMode,
            GameControlsEnum.Drunk,
            GameControlsEnum.Fifth,
            GameControlsEnum.Finger,
            GameControlsEnum.First,
            GameControlsEnum.Fourth,
            GameControlsEnum.Handbrake,
            GameControlsEnum.HighBeam,
            GameControlsEnum.IndicatorLeft,
            GameControlsEnum.IndicatorRight,
            GameControlsEnum.Left,
            GameControlsEnum.Range,
            GameControlsEnum.ReachLeft,
            GameControlsEnum.ReachRight,
            GameControlsEnum.Reverse,
            GameControlsEnum.Right,
            GameControlsEnum.Second,
            GameControlsEnum.ShiftDown,
            GameControlsEnum.ShiftUp,
            GameControlsEnum.Sixth,
            GameControlsEnum.Smoke,
            GameControlsEnum.Swear,
            GameControlsEnum.Third,
            GameControlsEnum.Throttle,
            GameControlsEnum.Use,
            GameControlsEnum.Wipers,
            GameControlsEnum.WristWatch,
            GameControlsEnum.Zoom
        };
        /// <summary>
        /// Represents whether to show relevant controls.
        /// </summary>
        private bool onlyShowRelevantControls = true;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Represents the instance of the mod.
        /// </summary>
        private MoControlsMod mod => MoControlsMod.instance;
        /// <summary>
        /// Represents the mouse emulator. references <see cref="MoControlsMod.mouseEmulator"/>.
        /// </summary>
        private MouseEmulator mouseEmulator
        {
            get
            {
                return MoControlsGO.mouseEmulator;
            }
        }
        /// <summary>
        /// Represents the control manager.
        /// </summary>
        private ControlManager controlManager
        {
            get
            {
                return MoControlsGO.controlManager;
            }
        }
        /// <summary>
        /// Represents the change input result. references <see cref="MoControlsMod.changeInputResult"/>.
        /// </summary>
        private ChangeInput changeInputResult
        {
            get
            {
                return this.controlManager.changeInputResult;
            }
            set
            {
                this.controlManager.changeInputResult = value;
            }
        }
        /// <summary>
        /// Represents the xbox controller. references <see cref="MoControlsMod.xboxController"/>.
        /// </summary>
        private XboxController xboxController
        {
            get
            {
                return MoControlsGO.xboxController;
            }
        }
        /// <summary>
        /// Represents the currently selected setting menu item.
        /// </summary>
        internal SettingsMenuEnum settingsMenu
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the currently selected main menu item.
        /// </summary>
        internal MainGUIMenuEnum mainGUIMenu
        {
            get;
            set;
        }
        /// <summary>
        /// Represents whether this should display the overlay.
        /// </summary>
        internal static bool displayCurrentPlayerModeOverlay
        {
            get;
            set;
        }
        /// <summary>
        /// Represents whether the gui should be opened or closed.
        /// </summary>
        internal bool controlsGuiOpened 
        {
            get;
            private set;
        } = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initliazes a new instance of <see cref="MoControlsGUI"/>.
        /// </summary>
        /// <param name="inMod">The mod.</param>
        public MoControlsGUI()
        {
            // Written, 22.08.2018

            Keybind.Add(this.mod, this.openControlsGui);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes boolean to open/close the gui. 
        /// </summary>
        private void toggleGui()
        {
            // Written, 18.12.2018

            this.controlsGuiOpened = !this.controlsGuiOpened;

            if (this.controlsGuiOpened)
            {
                FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = true;
                this.gameObject.GetComponent<GuiNav>().enabled = true;
            }
            else
            {
                FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = false;
                this.gameObject.GetComponent<GuiNav>().enabled = false;
            }
        }
        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {        
            // Written, 08.10.2018

            // Gui hold button set up
            HoldInputMono him = this.gameObject.AddComponent<HoldInputMono>();
            him.setData("Open Mod GUI",
                XboxButtonEnum.Back,
                0.3f,
                this.toggleGui);

            MoControlsMod.print(nameof(MoControlsGUI) + ": Started", Debugging.DebugTypeEnum.full);
        }
        /// <summary>
        /// on Update.
        /// </summary>
        private void Update()
        {
            // Written, 22.08.2018

            if (this.openControlsGui.GetKeybindDown())
                this.toggleGui();
            if (this.controlsGuiOpened)
            {
                if (this.changeInputResult.reassignKey)
                {
                    MoControlsGO.guiNav.enabled = false;
                    MonitorInputData mid = Input.monitorForInput();
                    if (mid.foundInput)
                    {
                        this.controlManager.changeInput(mid.input);
                        MoControlsGO.guiNav.enabled = true;
                    }
                }
            }
        }
        /// <summary>
        /// on GUI.
        /// </summary>
        private void OnGUI()
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
                    if (displayCurrentPlayerModeOverlay)
                    {
                        this.drawPlayerModeOverlayGUI();
                    }
                }
            }
            catch (Exception ex)
            {
                ModConsole.Error(ex.ToString());
                this.controlsGuiOpened = false;
            }
        }
        /// <summary>
        /// Draws the main menu for the main gui.
        /// </summary>
        private void drawMainMenuGUI()
        {
            // Written, 22.08.2018

            using (new gui.AreaScope(new Rect(mainGuiLeft + 5f, MENU_GUI_TOP, this.mainGuiWidth - SCROLL_BAR_OFFSET, MENU_GUI_HEIGHT)))
            using (new gui.HorizontalScope())
            {
                if (this.drawGeneralMenu(this.mainGUIMenu, out MainGUIMenuEnum changedTo))
                {
                    this.mainGUIMenu = changedTo;
                }
            }
        }
        /// <summary>
        /// Draws the main gui and it's children.
        /// </summary>
        private void drawMainGUI()
        {
            // Written, 22.08.2018

            ueGUI.contentColor = this.defaultContentColor;
            ueGUI.backgroundColor = this.backgroundColor;
            using (new gui.AreaScope(new Rect(mainGuiLeft, MENU_GUI_TOP + MENU_GUI_HEIGHT, this.mainGuiWidth, Screen.height - (MAIN_GUI_TOP + MENU_GUI_TOP + MENU_GUI_HEIGHT))))
            using (gui.ScrollViewScope scrollViewScope = new gui.ScrollViewScope(this.mainGUIScrollPosition, new GUILayoutOption[] { gui.Width(this.mainGuiWidth) }))
            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width(this.mainGuiWidth - SCROLL_BAR_OFFSET), gui.MaxWidth(this.mainGuiWidth - SCROLL_BAR_OFFSET) }))
            {
                this.mainGUIScrollPosition = scrollViewScope.scrollPosition;
                gui.Label(String.Format("<b>{0} v{1} ({3}) by {2}</b>", 
                    this.mod.Name, 
                    this.mod.Version, 
                    this.mod.Author,
                    MoControlsMod.instance.releaseVersionName));
                if (this.mainGUIMenu != MainGUIMenuEnum.About)
                    gui.Label(String.Format("<b>{0}</b> GUI key bind." +
                        "\r\n<b>{1}</b> Sets as None." +
                        "\r\n<b>LMB</b> Selects." +
                        "\r\n<b>RMB</b> Cancels.", 
                        this.openControlsGui.Key,
                        Input.noneKey));
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

            string aboutMessage = "<b>Mo'Controls</b> allows the player to have a primary and secondary input for each in-game control," +
                " the player could set all primary inputs to the keyboard and all secondary inputs to an Xbox Controller to have a seamless" +
                " swap of the keyboard to Xbox Controller. Mo'Controls also allows the player to have different control profiles for when " +
                "on foot and when in driving mode! So you can get more out of your controller! Default key to toggle the GUI is " + this.openControlsGui.Key +
                " or <b>hold down the back button on a connected xbox controller. (> 0.5sec)</b>";
            string[] features = new string[]
            {
                "Xbox Controller Support",
                "Assign two inputs to each game control",
                "Mouse Emulation; Use your Xbox Controller to control the mouse",
                "Split control modes for driving & walking",
                "Hold down the back button on an xbox controller to open the gui.",
                "Controller GUI Navigation"
            };
            string footerMessage = "Developed by <b>Tommo J. Armytage. | Latest release: 05.10.2020</b>";
            string joinPrefix = "\r\n# ";

            gui.Space(5f);

            ueGUI.backgroundColor = this.moduleBackgroundColor;
            using (new gui.HorizontalScope("box"))
                gui.Label(String.Format("<b>About:</b>\r\n\r\n{0}", aboutMessage));
            gui.Space(5f);
            using (new gui.HorizontalScope("box"))
                gui.Label(String.Format("<b>Features:</b>\r\n{0}{1}", joinPrefix, String.Join(joinPrefix, features)));
            gui.Space(10f);
            using (new gui.HorizontalScope("box"))
                gui.Label(footerMessage);
            ueGUI.backgroundColor = this.backgroundColor;
        }
        /// <summary>
        /// Draws foot controls content to the main gui.
        /// </summary>
        private void drawFootControlsContent()
        {
            // Written, 02.09.2018

            this.drawControlModeContent("Foot Controls", this.controlManager.footControls);
        }
        /// <summary>
        /// Draws driving controls content to the main gui.
        /// </summary>
        private void drawDrivingControlsContent()
        {
            // Written, 02.09.2018

            this.drawControlModeContent("Driving Controls", this.controlManager.drivingControls);
        }
        /// <summary>
        /// Draws settings content to the main gui.
        /// </summary>
        private void drawSettingsContent()
        {
            // Written, 20.08.2018

            gui.Space(3f);
            gui.Label("<b>Settings</b>");
            gui.Space(5f);
            this.drawSettingsMenu();

            switch (this.settingsMenu)
            {
                case SettingsMenuEnum.MouseEmulation:
                    this.drawMouseEmulationContent();
                    break;
                case SettingsMenuEnum.XboxController:
                    this.drawXboxControllerContent();
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
            // Written, 09.10.2018

            if (this.drawGeneralMenu(this.settingsMenu, out SettingsMenuEnum changedTo))
            {
                this.settingsMenu = changedTo;
            }
        }
        /// <summary>
        /// Draws xbox controller settings/content.
        /// </summary>
        private void drawXboxControllerContent()
        {
            // Written, 09.10.2018

            gui.Label("<b>Xbox Controller</b>");
            gui.Space(5f);
            using (new gui.HorizontalScope())
            {
                ueGUI.backgroundColor = this.moduleBackgroundColor;
                this.drawControllerInputContent();
                ueGUI.backgroundColor = this.backgroundColor;
            }
        }
        /// <summary>
        /// Draws controller input content.
        /// </summary>
        private void drawControllerInputContent()
        {
            // Written, 09.10.2018

            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width((this.mainGuiWidth) - SCROLL_BAR_OFFSET - 12.5f) }))
            {
                gui.Label("<b>Xbox Controller Input Viewer: " + (xboxController.isConnected ? "<color=green>Connected</color></b>" : "<color=red>Disconnected</color></b>"));               
                int j = 0;
                int itemWidth = 160;
                int maxItemsPerRow = (int)(mainGuiWidth / itemWidth);
                XboxControl[] xboxControls = this.xboxController.getXboxControls();

                for (int i = 1; i < xboxControls.Length; i++)
                {
                    i--;
                    using (new gui.HorizontalScope())
                    {
                        for (int n = 0; n < maxItemsPerRow; n++)
                        {
                            j++;
                            if (j == 2)
                            {
                                j = 0;
                                ueGUI.backgroundColor = this.primaryItemColor;
                            }
                            else
                                ueGUI.backgroundColor = this.secondaryItemColor;
                            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width(itemWidth), gui.Height(32) }))
                            {
                                using (new gui.HorizontalScope())
                                {
                                    bool isBoolState = xboxControls[i] is XboxBoolState;
                                    if (isBoolState && this.xboxController.isConnected && (xboxControls[i] as XboxBoolState).state == ButtonState.Pressed)
                                        ueGUI.contentColor = this.xboxButtonPressedColor;
                                    if (this.xboxController.LS == xboxControls[i])
                                    {
                                        GamePadThumbSticks.StickValue ls = this.xboxController.getLeftStick();
                                        gui.Label(xboxControls[i].texture, style: new GUIStyle() { contentOffset = new Vector2(ls.X * 3, -(ls.Y * 3)) });
                                    }
                                    else if (this.xboxController.RS == xboxControls[i])
                                    {
                                        GamePadThumbSticks.StickValue rs = this.xboxController.getRightStick();
                                        gui.Label(xboxControls[i].texture, style: new GUIStyle() { contentOffset = new Vector2(rs.X * 3, -(rs.Y * 3)) });
                                    }
                                    else
                                        gui.Label(xboxControls[i].texture);
                                    ueGUI.contentColor = this.defaultContentColor;
                                    using (new gui.VerticalScope())
                                    {
                                        gui.Label(xboxControls[i].inputName);
                                        if (this.xboxController.isConnected)
                                        {
                                            if (isBoolState)
                                                gui.Label((xboxControls[i] as XboxBoolState).ToString());
                                            else if (xboxControls[i] is XboxFloatState)
                                                gui.Label((xboxControls[i] as XboxFloatState).ToString());
                                        }
                                    }
                                }
                            }
                            i++;
                        }
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

            ueGUI.backgroundColor = this.moduleBackgroundColor;
            gui.Label("<b>Mouse Emulation</b>");
            gui.Space(5f);

            using (new gui.HorizontalScope("box"))
            {
                gui.Label(this.mouseEmulator.Emulating ? "<color=green><b>ON</b></color>" : "<color=red><b>OFF</b></color>");
                if (gui.Toggle(this.mouseEmulator.Emulating, String.Format("<b>Emulate mouse for controller:</b> Using {0}", this.mouseEmulator.inputType)) != this.mouseEmulator.Emulating)
                {
                    this.mouseEmulator.Emulating = !this.mouseEmulator.Emulating;
                    saveSettings = true;
                }
            }
            using (new gui.HorizontalScope())
            {
                bool _asInput;
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label("<i><b>Emulate mouse on joystick:</b></i>");
                    // As left + right thumb stick settings are grouped; need to manually change other value..
                    _asInput = this.mouseEmulator.inputType == InputTypeEnum.LS;
                    if (gui.Toggle(_asInput, String.Format("<b>Left Stick:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.inputType != InputTypeEnum.LS)
                        {
                            this.mouseEmulator.inputType = InputTypeEnum.LS;
                            saveSettings = true;
                        }
                    }
                    _asInput = this.mouseEmulator.inputType == InputTypeEnum.RS;
                    if (gui.Toggle(_asInput, String.Format("<b>Right Stick:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.inputType != InputTypeEnum.RS)
                        {
                            this.mouseEmulator.inputType = InputTypeEnum.RS;
                            saveSettings = true;
                        }
                    }
                    _asInput = this.mouseEmulator.inputType == InputTypeEnum.DPad;
                    if (gui.Toggle(_asInput, String.Format("<b>Directional Pad:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.inputType != InputTypeEnum.DPad)
                        {
                            this.mouseEmulator.inputType = InputTypeEnum.DPad;
                            saveSettings = true;
                        }
                    }
                }
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label("<i><b>Deadzone type:</b></i>");
                    _asInput = this.mouseEmulator.deadzoneType == DeadzoneTypeEnum.Radial;
                    if (gui.Toggle(_asInput, String.Format("<b>Radial:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.deadzoneType != DeadzoneTypeEnum.Radial)
                        {
                            this.mouseEmulator.deadzoneType = DeadzoneTypeEnum.Radial;
                            saveSettings = true;
                        }
                    }
                    _asInput = this.mouseEmulator.deadzoneType == DeadzoneTypeEnum.ScaledRadial;
                    if (gui.Toggle(_asInput, String.Format("<b>Scaled Radial:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.deadzoneType != DeadzoneTypeEnum.ScaledRadial)
                        {
                            this.mouseEmulator.deadzoneType = DeadzoneTypeEnum.ScaledRadial;
                            saveSettings = true;
                        }
                    }
                }
            }
            using (new gui.HorizontalScope())
            {
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>Mouse Deadzone:</b> {0}", this.mouseEmulator.deadzone));
                    tempValue = gui.HorizontalSlider(this.mouseEmulator.deadzone, MouseEmulator.MIN_DEADZONE, MouseEmulator.MAX_DEADZONE);
                    if (tempValue != this.mouseEmulator.deadzone) // Value Changed.
                    {
                        this.mouseEmulator.deadzone = tempValue;
                        saveSettings = true;
                    }
                }                
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>Mouse Sensitivity:</b> {0}", this.mouseEmulator.sensitivity));
                    tempValue = gui.HorizontalSlider(this.mouseEmulator.sensitivity, MouseEmulator.MIN_SENSITIVITY, MouseEmulator.MAX_SENSITIVITY);
                    if (tempValue != this.mouseEmulator.sensitivity) // Value Changed.
                    {
                        this.mouseEmulator.sensitivity = tempValue;
                        saveSettings = true;
                    }
                }
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>GUI Sensitivity:</b> {0}", this.mouseEmulator.sensitivity));
                    tempValue = gui.HorizontalSlider(this.mouseEmulator.sensitivity, MouseEmulator.MIN_SENSITIVITY, MouseEmulator.MAX_SENSITIVITY);
                    if (tempValue != this.mouseEmulator.sensitivity) // Value Changed.
                    {
                        this.mouseEmulator.sensitivity = tempValue;
                        saveSettings = true;
                    }
                }
            }
            gui.Space(5f);
            ueGUI.backgroundColor = this.moduleBackgroundColor;
            using (new gui.VerticalScope())
            {
                gui.Label("<i><b>Mouse inputs:</b></i>");
                ueGUI.backgroundColor = this.primaryItemColor;
                using (new gui.HorizontalScope())
                {
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbPrimaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbSecondaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                }
                ueGUI.backgroundColor = this.primaryItemColor;
                using (new gui.HorizontalScope())
                {
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.rmbPrimaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.rmbSecondaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                }
            }
            ueGUI.backgroundColor = this.backgroundColor;
            gui.Space(5f);
            if (saveSettings)
            {
                MoControlsSaveData.saveSettings();
            }
        }
        /// <summary>
        /// Draws display settings content to the main gui.
        /// </summary>
        private void drawDisplayContent()
        {
            // Written, 22.08.2018

            ueGUI.backgroundColor = this.moduleBackgroundColor;
            bool _saveSettings = false;

            using (new gui.HorizontalScope("box"))
            {
                if (gui.Toggle(displayCurrentPlayerModeOverlay, "Display current player mode overlay") != displayCurrentPlayerModeOverlay)
                {
                    displayCurrentPlayerModeOverlay = !displayCurrentPlayerModeOverlay;
                    _saveSettings = true;
                }
            }
            using (new gui.HorizontalScope("box"))
            {
                gui.Label("<i><b><color=blue>C</color><color=yellow>o</color>lo<color=red>r</color> <color=purple>M</color>ix<color=green>e</color>r</b></i>");
                gui.Space(3f);
            }
            ueGUI.backgroundColor = this.backgroundColor;
            if (_saveSettings)
            {
                MoControlsSaveData.saveSettings();
            }
        }
        /// <summary>
        /// Draws the player mode overlay.
        /// </summary>
        private void drawPlayerModeOverlayGUI()
        {
            // Written, 22.08.2018

            using (new gui.AreaScope(new Rect(Screen.width / 2, 1, 50, 20)))
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
            XboxControl xboxControl = this.xboxController?.getXboxControlByInputName(inInputName);
            bool buttonClicked = false;
            ueGUI.backgroundColor = this.unselectedMenuButtonColor;
            if (xboxControl?.texture != null && reassignMessage == null && MoControlsMod.assetsLoaded)
            {
                if (gui.Button(xboxControl.texture))
                {
                    buttonClicked = true;
                }
            }
            else
            {
                if (gui.Button(reassignMessage ?? (!MoControlsMod.assetsLoaded && xboxControl?.texture == null ? "<b><color=red>Asset not loaded</color></b> " : "") + inInputName))
                {
                    buttonClicked = true;
                }
            }
            ueGUI.backgroundColor = this.backgroundColor;
            if (buttonClicked)
            {
                this.changeInputResult.changeToPollingState(inControlName, inIndex, inMode, inMod);
            }
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
            this.onlyShowRelevantControls = gui.Toggle(this.onlyShowRelevantControls, "Show Relevant Controls");
            gui.Space(5f);
            int j = 0;
            for (int i = 0; i < inControlInputs.GetLength(0); i++)
            {
                string gameControlName = inControlInputs[i, 0];
                GameControlsEnum gameControl = gameControlName.getGameControl();
                bool drawControl = false;
                if (this.onlyShowRelevantControls)
                {
                    if (this.mainGUIMenu == MainGUIMenuEnum.DrivingControls)
                    {
                        if (this.relevantDrivingControls.Contains(gameControl))
                            drawControl = true;
                    }
                    else
                    {
                        if (this.mainGUIMenu == MainGUIMenuEnum.FootControls)
                        {
                            if (this.relevantFootControls.Contains(gameControl))
                                drawControl = true;
                        }
                    }
                }
                else
                    drawControl = true;
                if (drawControl)
                {
                    j++;
                    if (j == 2)
                    {
                        j = 0;
                        ueGUI.backgroundColor = this.primaryItemColor;
                    }
                    else
                        ueGUI.backgroundColor = this.secondaryItemColor;

                    gui.Space(3f);
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", gameControl.getGameControlAlias(true)));
                        using (new gui.HorizontalScope())
                        {
                            bool isControls = this.mainGUIMenu == MainGUIMenuEnum.FootControls;
                            PlayerModeEnum? playerMode;
                            if (isControls)
                                playerMode = PlayerModeEnum.OnFoot;
                            else
                            {
                                isControls = this.mainGUIMenu == MainGUIMenuEnum.DrivingControls;
                                if (isControls)
                                    playerMode = PlayerModeEnum.Driving;
                                else
                                    playerMode = null;
                            }

                            this.drawCommonControl("Primary Input", gameControlName, inControlInputs[i, 1], 1, playerMode);
                            this.drawCommonControl("Secondary Input", gameControlName, inControlInputs[i, 2], 2, playerMode);
                        }


                    }
                    ueGUI.backgroundColor = this.backgroundColor;
                    gui.Space(3f);
                }
            }
        }
        /// <summary>
        /// Draws all mod keybinds.
        /// </summary>
        private void drawModContent()
        {
            // Written, 09.09.2018

            int j = 0;
            if (!this.hasCountedModKeybinds)
            {
                this.hasCountedModKeybinds = true;
                foreach (Mod _mod in ModLoader.LoadedMods)
                    this.modKeybindCount += Keybind.Get(_mod).Count;
            }
            gui.Space(3f);
            gui.Label(String.Format("<b>Total Mod Keybinds: {0}</b>", modKeybindCount - 4 /* There are 4 hidden kb's for mouse buttons as of 13.12.2018, correcting stats with constant value. */));
            foreach (Mod _mod in ModLoader.LoadedMods)
            {
                Keybind[] modKeybinds = Keybind.Get(_mod).ToArray();

                if (modKeybinds.Count() > 0)
                {
                    gui.Space(3f);
                    ueGUI.backgroundColor = this.moduleBackgroundColor;
                    using (new gui.HorizontalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}</b>, by <b>{1}</b>:", _mod.Name, _mod.Author));
                        using (new gui.VerticalScope())
                        {
                            bool ignoreKeybind = false;
                            for (int i = 0; i < modKeybinds.Length; i++)
                            {

                                if (_mod is MoControlsMod)
                                {
                                    if (modKeybinds[i].ID != this.openControlsGui.ID)
                                        ignoreKeybind = true;
                                }
                                if (!ignoreKeybind)
                                {
                                    j++;
                                    if (j == 2)
                                    {
                                        j = 0;
                                        ueGUI.backgroundColor = this.primaryItemColor;
                                    }
                                    else
                                        ueGUI.backgroundColor = this.secondaryItemColor;
                                    using (new gui.VerticalScope("box"))
                                    {
                                        gui.Label(String.Format("<b>{0}:</b>", modKeybinds[i].Name));
                                        using (new gui.HorizontalScope())
                                        {
                                            this.drawCommonControl("Modifier", modKeybinds[i].ID, modKeybinds[i].Modifier.ToString(), 1, inMod: _mod);
                                            this.drawCommonControl("Input", modKeybinds[i].ID, modKeybinds[i].Key.ToString(), 2, inMod: _mod);
                                        }
                                    }
                                    gui.Space(3f);
                                }
                            }
                        }
                    }
                }
            }
            ueGUI.backgroundColor = this.backgroundColor;
        }
        /// <summary>
        /// Draws a menu with the provided Enum, <typeparamref name="T"/>.
        /// </summary>
        private bool drawGeneralMenu<T>(T inSelected, out T inChangedTo)
        {
            // Written, 09.10.2018

            using (new gui.HorizontalScope("box"))
            {
                foreach (T _enum in Enum.GetValues(typeof(T)))
                {
                    bool isSelected = _enum.Equals(inSelected);
                    string title = null;
                    if (_enum is XboxControllerInputMapEnum)
                        title = (_enum as XboxControllerInputMapEnum?).toString();
                    else
                        if (_enum is SettingsMenuEnum)
                        title = (_enum as SettingsMenuEnum?).toString();
                    else
                            if (_enum is MainGUIMenuEnum)
                        title = (_enum as MainGUIMenuEnum?).toString();

                    if (isSelected)
                        ueGUI.backgroundColor = this.selectedMenuButtonColor;
                    else
                        ueGUI.backgroundColor = this.unselectedMenuButtonColor;

                    if (gui.Button(title ?? _enum.ToString()) && !isSelected)
                    {
                        inChangedTo = _enum;
                        return true;
                    }
                }
                ueGUI.backgroundColor = this.backgroundColor;
                inChangedTo = inSelected;
                return false;
            }
        }
       
        #endregion
    }
}
