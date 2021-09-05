using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;
using XInputDotNetPure;
using gui = UnityEngine.GUILayout;
using ueGUI = UnityEngine.GUI;

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

        private string mouseSettingsValue = "";
        private string mouseSettingsValue1 = "";
        private string pHorzGrav = "";
        private string pHorzSens = "";
        private string pHorzDead = "";
        private string pVertGrav = "";
        private string pVertSens = "";
        private string pVertDead = "";
        private string horzGrav = "";
        private string horzSens = "";
        private string horzDead = "";
        private string vertGrav = "";
        private string vertSens = "";
        private string vertDead = "";
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

        #endregion

        #region Relevant controls 

        /// <summary>
        /// Represents all relevant controls when in "on foot mode".
        /// </summary>
        private readonly List<GameControlsEnum> relevantFootControls = new List<GameControlsEnum>()
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
        private readonly List<GameControlsEnum> relevantDrivingControls = new List<GameControlsEnum>()
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
                this.controlManager.setChangeInput(value);
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
        /// Represents whether the gui should be opened or closed.
        /// </summary>
        internal bool controlsGuiOpened
        {
            get;
            private set;
        } = false;
        /// <summary>
        /// Represents the gui navigation system for the GUI.
        /// </summary>
        internal GuiNav guiNav
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes boolean to open/close the gui. 
        /// </summary>
        private void toggleGui()
        {
            // Written, 18.12.2018 | Modified, 09.10.2020

            this.controlsGuiOpened = !this.controlsGuiOpened;
                FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = this.controlsGuiOpened;
            this.guiNav.enabled = this.controlsGuiOpened;
        }
        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            //  mousesettings setup
            // GUINav Set up
            this.guiNav = this.gameObject.AddComponent<GuiNav>();
            this.guiNav.setControls(
                XboxAxisEnum.rightTrigger, XboxButtonEnum.NULL,
                XboxAxisEnum.leftTrigger, XboxButtonEnum.NULL,
                XboxAxisEnum.NULL, XboxButtonEnum.RB,
                XboxAxisEnum.NULL, XboxButtonEnum.LB);
            this.guiNav.enabled = false;
            // Gui hold button set up
            HoldInputMono him = this.gameObject.AddComponent<HoldInputMono>();
            him.setData("Open Mod GUI",
                XboxButtonEnum.Back,
                0.3f,
                this.toggleGui);
            // printing gui mono start to modconsole.
            MoControlsMod.print(nameof(MoControlsGUI) + ": Started", Debugging.DebugTypeEnum.full);
        }
        /// <summary>
        /// on Update.
        /// </summary>
        private void Update()
        {
            // Written, 22.08.2018

            if (MoControlsMod.instance.openControlsGui.GetKeybindDown())
                this.toggleGui();
            if (this.controlsGuiOpened)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                {
                    this.controlsGuiOpened = false;
                    this.guiNav.enabled = false;
                }
                if (this.changeInputResult.reassignKey)
                {
                    this.guiNav.enabled = false;
                    MonitorInputData mid = Input.monitorForInput();
                    if (mid.foundInput)
                    {
                        this.controlManager.changeInput(mid.input);
                        this.guiNav.enabled = true;
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

                    if (MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay)
                        this.drawPlayerModeOverlayGUI();
                    if (MoControlsSaveData.loadedSaveData.displayFfbOverlay)
                        this.drawForceFeedBackOverlayGUI();
                    if (MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay)
                        this.drawDrivetrainOverlay();
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
                gui.Label(String.Format("<b>{0} v{1} by {2}</b>",
                    this.mod.Name,
                    this.mod.Version,
                    this.mod.Author));
                if (this.mainGUIMenu != MainGUIMenuEnum.About)
                    gui.Label(String.Format("<b>{0}</b> GUI key bind." +
                        "\r\n<b>{1}</b> Sets as None." +
                        "\r\n<b>LMB</b> Selects." +
                        "\r\n<b>RMB</b> Cancels.",
                        MoControlsMod.instance.openControlsGui.Key,
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
                "on foot and when in driving mode! So you can get more out of your controller! Key to toggle the GUI is " + MoControlsMod.instance.openControlsGui.Key +
                " or <b>hold down the back button on a connected xbox controller for (> 0.3sec)</b>";
            string[] features = new string[]
            {
                "Xbox controller support",
                "Assign two inputs to each game control",
                "Mouse emulation; use your Xbox controller to control the mouse",
                "Split control modes for driving & walking",
                "Toggle tool/hand mode by holding down the <i>Start</i> button on a connected xbox controller for > 0.3sec",
                "Controller gui navigation",
                "Controller vibration/rumble effects; rumble options based on default (toplessgun), rpm, wheel-slip, etc."
            };
            string footerMessage = "Developed by <b>Tommo J. Armytage. | Latest release: " + MoControlsMod.LATEST_RELEASE_DATE + "</b>";
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

            gui.Label(string.Format("<b>Xbox Controller</b>: {0}", this.xboxController.isConnected ? "<color=green>connected</color>" : "<color=red>disconnected</color>"));
            gui.Space(5f);
            using (new gui.VerticalScope())
            {
                ueGUI.backgroundColor = this.moduleBackgroundColor;
                this.drawControllerFFBContent();
                this.drawControllerInputContent();
                this.drawCombinedTriggerAxisContent();
                ueGUI.backgroundColor = this.backgroundColor;
            }
        }
        /// <summary>
        /// Draws combined trigger axis setting content.
        /// </summary>
        private void drawCombinedTriggerAxisContent()
        {
            // Written, 24.10.2020

            using (new gui.HorizontalScope("box"))
            {
                bool combineTriggerAxis = MoControlsSaveData.loadedSaveData.combinedTriggerAxis;
                if (gui.Toggle(combineTriggerAxis, String.Format("Combine trigger axis: {0}", combineTriggerAxis ? "<color=green>On</color>" : "<color=red>Off</color>")) != combineTriggerAxis)
                {
                    MoControlsSaveData.loadedSaveData.combinedTriggerAxis = !combineTriggerAxis;
                    this.xboxController.updateTriggerAxis();
                    MoControlsSaveData.loadedSaveData.saveSettings();
                }
                gui.Label("NOTE: after changing this value you will need to reassign desired gameContol inputs to triggers.");
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
                            if (i >= xboxControls.Length)
                                break;
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
                                    if (this.xboxController.lS == xboxControls[i])
                                    {
                                        GamePadThumbSticks.StickValue ls = this.xboxController.getLeftStick();
                                        gui.Label(xboxControls[i].texture, style: new GUIStyle() { contentOffset = new Vector2(ls.X * 3, -(ls.Y * 3)) });
                                    }
                                    else if (this.xboxController.rS == xboxControls[i])
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
        /// Draws controller force feedback content.
        /// </summary>
        private void drawControllerFFBContent()
        {
            // Written, 18.10.2020

            bool _saveSettings = false;
            bool ffbOn = MoControlsSaveData.loadedSaveData.ffbOnXboxController;
            bool ffbOptDefault = MoControlsSaveData.loadedSaveData.ffbOption_default;
            bool ffbOptRpm = MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter;
            bool ffbOptWheelSlip = MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip;
            bool ffbOptGearChange = MoControlsSaveData.loadedSaveData.ffbOption_gearChange;
            if (gui.Toggle(ffbOn, String.Format("FFB: {0}", ffbOn ? "<color=green>On</color>" : "<color=red>Off</color>")) != ffbOn)
            {
                MoControlsSaveData.loadedSaveData.ffbOnXboxController = !ffbOn;
                _saveSettings = true;
            }
            using (new gui.HorizontalScope("box"))
            {
                gui.Label("<i><b>FFB Options</b></i>");
                if (gui.Toggle(ffbOptDefault, String.Format("Default ffb: {0}", ffbOptDefault ? "<color=green>On</color>" : "<color=red>Off</color>"))
                    != ffbOptDefault)
                {
                    // only default opt allowed if enabled
                    MoControlsSaveData.loadedSaveData.ffbOption_default = !ffbOptDefault;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                    {
                        if (ffbOptRpm)
                            MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter = false;
                        if (ffbOptWheelSlip)
                            MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip = false;
                        if (ffbOptGearChange)
                            MoControlsSaveData.loadedSaveData.ffbOption_gearChange = false;
                    }
                    _saveSettings = true;
                }
                if (gui.Toggle(ffbOptRpm, String.Format("RPM limiter ffb: {0}", ffbOptRpm ? "<color=green>On</color>" : "<color=red>Off</color>"))
                   != ffbOptRpm)
                {
                    MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter = !ffbOptRpm;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter)
                        if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                            MoControlsSaveData.loadedSaveData.ffbOption_default = false;
                    _saveSettings = true;
                }
                if (gui.Toggle(ffbOptWheelSlip, String.Format("Wheel slip ffb: {0}", ffbOptWheelSlip ? "<color=green>On</color>" : "<color=red>Off</color>"))
                   != ffbOptWheelSlip)
                {
                    MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip = !ffbOptWheelSlip;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip)
                        if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                            MoControlsSaveData.loadedSaveData.ffbOption_default = false;
                    _saveSettings = true;
                }
                if (gui.Toggle(ffbOptGearChange, String.Format("Gear change ffb: {0}", ffbOptGearChange ? "<color=green>On</color>" : "<color=red>Off</color>"))
                  != ffbOptGearChange)
                {
                    MoControlsSaveData.loadedSaveData.ffbOption_gearChange = !ffbOptGearChange;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_gearChange)
                        if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                            MoControlsSaveData.loadedSaveData.ffbOption_default = false;
                    _saveSettings = true;
                }
            }
            if (_saveSettings)
                MoControlsSaveData.loadedSaveData.saveSettings();
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
                gui.Label(MoControlsSaveData.loadedSaveData.emulateMouse ? "<color=green><b>ON</b></color>" : "<color=red><b>OFF</b></color>");
                if (gui.Toggle(MoControlsSaveData.loadedSaveData.emulateMouse, String.Format("<b>Emulate mouse for controller:</b> Using {0}", MoControlsSaveData.loadedSaveData.mouseInputType)) != MoControlsSaveData.loadedSaveData.emulateMouse)
                {
                    MoControlsSaveData.loadedSaveData.emulateMouse = !MoControlsSaveData.loadedSaveData.emulateMouse;
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
                    _asInput = MoControlsSaveData.loadedSaveData.mouseInputType == InputTypeEnum.LS;
                    if (gui.Toggle(_asInput, String.Format("<b>Left Stick:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (!_asInput)
                        {
                            MoControlsSaveData.loadedSaveData.mouseInputType = InputTypeEnum.LS;
                            saveSettings = true;
                        }
                    }
                    _asInput = MoControlsSaveData.loadedSaveData.mouseInputType == InputTypeEnum.RS;
                    if (gui.Toggle(_asInput, String.Format("<b>Right Stick:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (_asInput)
                        {
                            MoControlsSaveData.loadedSaveData.mouseInputType = InputTypeEnum.RS;
                            saveSettings = true;
                        }
                    }
                    _asInput = MoControlsSaveData.loadedSaveData.mouseInputType == InputTypeEnum.DPad;
                    if (gui.Toggle(_asInput, String.Format("<b>Directional Pad:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (!_asInput)
                        {
                            MoControlsSaveData.loadedSaveData.mouseInputType = InputTypeEnum.DPad;
                            saveSettings = true;
                        }
                    }
                }
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label("<i><b>Deadzone type:</b></i>");
                    _asInput = MoControlsSaveData.loadedSaveData.mouseDeadzoneType == DeadzoneTypeEnum.Radial;
                    if (gui.Toggle(_asInput, String.Format("<b>Radial:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (!_asInput)
                        {
                            MoControlsSaveData.loadedSaveData.mouseDeadzoneType = DeadzoneTypeEnum.Radial;
                            saveSettings = true;
                        }
                    }
                    _asInput = MoControlsSaveData.loadedSaveData.mouseDeadzoneType == DeadzoneTypeEnum.ScaledRadial;
                    if (gui.Toggle(_asInput, String.Format("<b>Scaled Radial:</b> {0}", _asInput ? "<color=green>ON</color>" : "")) != _asInput)
                    {
                        if (!_asInput)
                        {
                            MoControlsSaveData.loadedSaveData.mouseDeadzoneType = DeadzoneTypeEnum.ScaledRadial;
                            saveSettings = true;
                        }
                    }
                }
            }
            using (new gui.HorizontalScope())
            {
                using (new gui.HorizontalScope("box")) 
                {
                    gui.Label(String.Format("<b>Mouse Deadzone:</b> {0}", MoControlsSaveData.loadedSaveData.mouseDeadzone));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(mouseSettingsValue))
                            mouseSettingsValue = MoControlsSaveData.loadedSaveData.mouseDeadzone.ToString();
                        mouseSettingsValue = gui.TextField(mouseSettingsValue);
                        if (gui.Button("Set Mouse Deadzone"))
                        {
                            if (float.TryParse(mouseSettingsValue, out tempValue))
                            {
                                MoControlsSaveData.loadedSaveData.mouseDeadzone = tempValue;
                                saveSettings = true;
                            }
                        }
                    }
                }
                using (new gui.HorizontalScope("box")) 
                {
                    gui.Label(String.Format("<b>Mouse Sensitivity:</b> {0}", MoControlsSaveData.loadedSaveData.mouseSensitivity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(mouseSettingsValue1))
                            mouseSettingsValue1 = MoControlsSaveData.loadedSaveData.mouseSensitivity.ToString();
                        mouseSettingsValue1 = gui.TextField(mouseSettingsValue1);
                        if (gui.Button("Set Mouse Sensitivity"))
                        {
                            if (float.TryParse(mouseSettingsValue1, out tempValue))
                            {
                                MoControlsSaveData.loadedSaveData.mouseSensitivity = tempValue;
                                saveSettings = true;
                            }
                        }
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
                        gui.Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.lmbPrimaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", MoControlsMod.instance.lmbPrimaryInput.ID, MoControlsMod.instance.lmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", MoControlsMod.instance.lmbPrimaryInput.ID, MoControlsMod.instance.lmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.lmbSecondaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", MoControlsMod.instance.lmbSecondaryInput.ID, MoControlsMod.instance.lmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", MoControlsMod.instance.lmbSecondaryInput.ID, MoControlsMod.instance.lmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                }
                ueGUI.backgroundColor = this.primaryItemColor;
                using (new gui.HorizontalScope())
                {
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.rmbPrimaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", MoControlsMod.instance.rmbPrimaryInput.ID, MoControlsMod.instance.rmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", MoControlsMod.instance.rmbPrimaryInput.ID, MoControlsMod.instance.rmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.rmbSecondaryInput.Name));
                        using (new gui.HorizontalScope())
                        {
                            this.drawCommonControl("Modifier", MoControlsMod.instance.rmbSecondaryInput.ID, MoControlsMod.instance.rmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                            this.drawCommonControl("Input", MoControlsMod.instance.rmbSecondaryInput.ID, MoControlsMod.instance.rmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                        }
                    }
                }
            }
            ueGUI.backgroundColor = this.backgroundColor;
            gui.Space(5f);
            if (saveSettings)
            {
                MoControlsSaveData.loadedSaveData.saveSettings();
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
                if (gui.Toggle(MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay, "Display current player mode overlay") != MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay)
                {
                    MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay = !MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay;
                    _saveSettings = true;
                }
                if (gui.Toggle(MoControlsSaveData.loadedSaveData.displayFfbOverlay, "Display force feedback overlay") != MoControlsSaveData.loadedSaveData.displayFfbOverlay)
                {
                    MoControlsSaveData.loadedSaveData.displayFfbOverlay = !MoControlsSaveData.loadedSaveData.displayFfbOverlay;
                    _saveSettings = true;
                }
                if (gui.Toggle(MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay, "Display vehicle info overlay") != MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay)
                {
                    MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay = !MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay;
                    _saveSettings = true;
                }
            }
            drawPlayerMovementAxisSettings();
            
            using (new gui.HorizontalScope("box"))
            {
                gui.Label("<i><b><color=blue>C</color><color=yellow>o</color>lo<color=red>r</color> <color=purple>M</color>ix<color=green>e</color>r</b></i>");
                gui.Space(3f);
            }
            ueGUI.backgroundColor = this.backgroundColor;
            if (_saveSettings)
            {
                MoControlsSaveData.loadedSaveData.saveSettings();
            }
        }
        /// <summary>
        /// Draws the player mode overlay.
        /// </summary>
        private void drawPlayerModeOverlayGUI()
        {
            // Written, 22.08.2018

            using (new gui.AreaScope(new Rect(Screen.width / 2, 1, 180, 20)))
            {
                gui.Label(String.Format("{0} ({1} mode)", ControlManager.getCurrentPlayerMode.ToString(), ControlManager.isInToolMode ? "Tool" : "Hand"));
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
                                    if (modKeybinds[i].ID != MoControlsMod.instance.openControlsGui.ID)
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
        /// <summary>
        /// Draws force feedback overlay
        /// </summary>
        private void drawForceFeedBackOverlayGUI()
        {
            // Written, 16.10.2020

            using (new gui.AreaScope(new Rect(Screen.width / 2, 20, 200, 200)))
            {
                if (this.controlManager.vehicle != null)
                {
                    gui.Label(String.Format("FFB: {0}\nLast Rumble Sent: {1}",
                        this.controlManager.getFfbSetOpt().ToString("0.###"),
                        this.xboxController.prevRumblePow.magnitude.ToString("0.###")));
                }
                else
                {
                    gui.Label("No supported vehicle found.\nCurrent vehicle: " + ControlManager.getCurrentVehicle);
                }
            }
        }
        /// <summary>
        /// Draws drivetrain related info as a overlay.
        /// </summary>
        private void drawDrivetrainOverlay()
        {
            // Written, 18.10.2020

            using (new gui.AreaScope(new Rect(200 + Screen.width / 2, 22, 200, 300)))
            {
                if (this.controlManager.vehicle != null)
                {
                    Drivetrain dt = this.controlManager.drivetrain;
                    CarDynamics cd = this.controlManager.carDynamics;
                    gui.Label(new StringBuilder().AppendFormat("{9} drivetrain:\nRpm: {2}\nRange: {0}-{1}\nRev Limiter: {3} ({4})\nShift Triggered: {5} ({6})\nLongitudeSlip: {7}\nLateralSlip: {8}",
                        dt.minRPM.ToString("0"),
                        dt.maxRPM,
                        dt.rpm.ToString("0"),
                        dt.revLimiterTriggered,
                        dt.revLimiterTime,
                        dt.shiftTriggered,
                        dt.shiftTime,
                        dt.poweredWheels.Max(_wheel => _wheel.longitunalSlipVelo).ToString("0.###"),
                        dt.poweredWheels.Max(_wheel => _wheel.lateralSlipVelo).ToString("0.###"),
                        String.IsNullOrEmpty(ControlManager.currentVehicleName) ? "NaN" : ControlManager.currentVehicleName).ToString());;

                }
                else
                {
                    gui.Label("No supported vehicle found.\nCurrent vehicle: " + ControlManager.getCurrentVehicle);
                }
            }
        }

        private void drawPlayerMovementAxisSettings() 
        {
            // Player movement: gravity, sensitivity and deadzone settings 
            bool saveSettings = false;
            float _value;

            #region player horz

            gui.Label("<b>PlayerHorizontal</b>");
            using (new gui.HorizontalScope())
            {
                // Gravity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Gravity</b> {0}", MoControlsSaveData.loadedSaveData.playerHorzGravity));
                    using (new gui.VerticalScope("box"))
                    {                        
                        if (string.IsNullOrEmpty(pHorzGrav))
                            pHorzGrav = MoControlsSaveData.loadedSaveData.playerHorzGravity.ToString();;
                        pHorzGrav = gui.TextField(pHorzGrav);
                        if (gui.Button("Set Gravity"))
                        {
                            if (float.TryParse(pHorzGrav, out _value))
                            {
                                cInput.SetAxisGravity("PlayerHorizontal", _value);
                                MoControlsSaveData.loadedSaveData.playerHorzGravity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Sensitivity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Sensitivity</b> {0}", MoControlsSaveData.loadedSaveData.playerHorzSensitivity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(pHorzSens))
                            pHorzSens = MoControlsSaveData.loadedSaveData.playerHorzSensitivity.ToString();
                        pHorzSens = gui.TextField(pHorzSens);
                        if (gui.Button("Set Sensitivity"))
                        {
                            if (float.TryParse(pHorzSens, out _value))
                            {
                                cInput.SetAxisSensitivity("PlayerHorizontal", _value);
                                MoControlsSaveData.loadedSaveData.playerHorzSensitivity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Deadzone:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Deadzone</b> {0}", MoControlsSaveData.loadedSaveData.playerHorzDeadzone));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(pHorzDead))
                            pHorzDead = MoControlsSaveData.loadedSaveData.playerHorzDeadzone.ToString();
                        pHorzDead = gui.TextField(pHorzDead);
                        if (gui.Button("Set Deadzone"))
                        {
                            if (float.TryParse(pHorzDead, out _value))
                            {
                                cInput.SetAxisDeadzone("PlayerHorizontal", _value);
                                MoControlsSaveData.loadedSaveData.playerHorzDeadzone = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
            }

            #endregion

            #region player vert

            gui.Label("<b>PlayerVertical</b>");
            using (new gui.HorizontalScope())
            {
                // Gravity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Gravity</b> {0}", MoControlsSaveData.loadedSaveData.playerVertGravity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(pVertGrav))
                            pVertGrav = MoControlsSaveData.loadedSaveData.playerVertGravity.ToString();
                        pVertGrav = gui.TextField(pVertGrav);
                        if (gui.Button("Set Gravity"))
                        {
                            if (float.TryParse(pVertGrav, out _value))
                            {
                                cInput.SetAxisGravity("PlayerVertical", _value);
                                MoControlsSaveData.loadedSaveData.playerVertGravity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Sensitivity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Sensitivity</b> {0}", MoControlsSaveData.loadedSaveData.playerVertSensitivity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(pVertSens))
                            pVertSens = MoControlsSaveData.loadedSaveData.playerVertSensitivity.ToString();
                        pVertSens = gui.TextField(pVertSens);
                        if (gui.Button("Set Sensitivity"))
                        {
                            if (float.TryParse(pVertSens, out _value))
                            {
                                cInput.SetAxisSensitivity("PlayerVertical", _value);
                                MoControlsSaveData.loadedSaveData.playerVertSensitivity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Deadzone:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Deadzone</b> {0}", MoControlsSaveData.loadedSaveData.playerVertDeadzone));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(pVertDead))
                            pVertDead = MoControlsSaveData.loadedSaveData.playerVertDeadzone.ToString();
                        pVertDead = gui.TextField(pVertDead);
                        if (gui.Button("Set Deadzone"))
                        {
                            if (float.TryParse(pVertDead, out _value))
                            {
                                cInput.SetAxisDeadzone("PlayerVertical", _value);
                                MoControlsSaveData.loadedSaveData.playerVertDeadzone = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
            }

            #endregion

            #region horz

            gui.Label("<b>Horizontal</b>");
            using (new gui.HorizontalScope())
            {
                // Gravity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Gravity</b> {0}", MoControlsSaveData.loadedSaveData.horzGravity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(horzGrav))
                            horzGrav = MoControlsSaveData.loadedSaveData.horzGravity.ToString();
                            horzGrav = gui.TextField(horzGrav);
                        if (gui.Button("Set Gravity"))
                        {
                            if (float.TryParse(horzGrav, out _value))
                            {
                                cInput.SetAxisGravity("Horizontal", _value);
                                MoControlsSaveData.loadedSaveData.horzGravity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Sensitivity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Sensitivity</b> {0}", MoControlsSaveData.loadedSaveData.horzSensitivity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(horzSens))
                            horzSens = MoControlsSaveData.loadedSaveData.horzSensitivity.ToString();
                        horzSens = gui.TextField(horzSens);
                        if (gui.Button("Set Sensitivity"))
                        {
                            if (float.TryParse(horzSens, out _value))
                            {
                                cInput.SetAxisSensitivity("Horizontal", _value);
                                MoControlsSaveData.loadedSaveData.horzSensitivity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Deadzone:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Deadzone</b> {0}", MoControlsSaveData.loadedSaveData.horzDeadzone));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(horzDead))
                            horzDead = MoControlsSaveData.loadedSaveData.horzDeadzone.ToString();
                        horzDead = gui.TextField(horzDead);
                        if (gui.Button("Set Deadzone"))
                        {
                            if (float.TryParse(horzDead, out _value))
                            {
                                cInput.SetAxisDeadzone("Horizontal", _value);
                                MoControlsSaveData.loadedSaveData.horzDeadzone = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
            }

            #endregion

            #region vert

            gui.Label("<b>Vertical</b>");
            using (new gui.HorizontalScope())
            {
                // Gravity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Gravity</b> {0}", MoControlsSaveData.loadedSaveData.vertGravity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(vertGrav))
                            vertGrav = MoControlsSaveData.loadedSaveData.vertGravity.ToString();
                        vertGrav = gui.TextField(vertGrav);
                        if (gui.Button("Set Gravity"))
                        {
                            if (float.TryParse(vertGrav, out _value))
                            {
                                cInput.SetAxisGravity("Vertical", _value);
                                MoControlsSaveData.loadedSaveData.vertGravity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Sensitivity:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Sensitivity</b> {0}", MoControlsSaveData.loadedSaveData.vertSensitivity));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(vertSens))
                            vertSens = MoControlsSaveData.loadedSaveData.vertSensitivity.ToString();
                        vertSens = gui.TextField(vertSens);
                        if (gui.Button("Set Sensitivity"))
                        {
                            if (float.TryParse(vertSens, out _value))
                            {
                                cInput.SetAxisSensitivity("Vertical", _value);
                                MoControlsSaveData.loadedSaveData.vertSensitivity = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
                // Deadzone:
                using (new gui.HorizontalScope("box"))
                {
                    gui.Label(String.Format("<b>Deadzone</b> {0}", MoControlsSaveData.loadedSaveData.vertDeadzone));
                    using (new gui.VerticalScope("box"))
                    {
                        if (string.IsNullOrEmpty(vertDead))
                            vertDead = MoControlsSaveData.loadedSaveData.vertDeadzone.ToString();
                        vertDead = gui.TextField(vertDead);
                        if (gui.Button("Set Deadzone"))
                        {
                            if (float.TryParse(vertDead, out _value))
                            {
                                cInput.SetAxisDeadzone("Vertical", _value);
                                MoControlsSaveData.loadedSaveData.vertDeadzone = _value;
                                saveSettings = true;
                            }
                        }
                    }
                }
            }

            #endregion

            // save settings
            if (saveSettings)
            {
                MoControlsSaveData.loadedSaveData.saveSettings();
            }
        }

        #endregion
    }
}
