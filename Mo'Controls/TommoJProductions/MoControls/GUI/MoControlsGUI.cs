using System;
using System.Linq;
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
        private const float MENU_GUI_HEIGHT = 30f;
        /// <summary>
        /// Represents the scroll bar's offset.
        /// </summary>
        private const float SCROLL_BAR_OFFSET = 25f;

        #endregion

        private Color32 backgroundColor = new Color32(88, 92, 133, 230); // #585C85 | light navel blue
        private Color32 primaryItemColor = new Color32(113, 154, 195, 230); // #719AC3 | Light White Blue
        private Color32 secondaryItemColor = new Color32(200, 216, 211, 230); // Sand
        private Color32 unselectedMenuButtonColor = new Color32(113, 120, 216, 230); // #7178D8 | purple-blue
        private Color32 selectedMenuButtonColor = new Color32(113, 154, 195, 230); // #719AC3 | Light White Blue
        private Color32 defaultContentColor = new Color32(190, 194, 251, 230); // #BEC2FB | light purple-blue 
        private Color32 moduleBackgroundColor = new Color32(68, 154, 219, 230); // #449ADB | Light blue
        /// <summary>
        /// Represents whether the instance has calculated the amount of keybinds yet.
        /// </summary>
        private bool hasCountedModKeybinds = false;
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
        /// Represents a keybind to open/close the gui for the mod.
        /// </summary>
        public readonly Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.F12);

        #endregion

        #region Properties

        /// <summary>
        /// Shows/closes the msc menu.
        /// </summary>
        private bool mscMenu
        {
            get
            {
                return GameObject.Find("Systems/OptionsMenu").activeSelf;
            }
            set
            {
                GameObject.Find("Systems/OptionsMenu").SetActive(value);
            }
        }
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
        /// Changes boolean to open/close the msc menu gui. 
        /// </summary>
        private void toggleMscMenu()
        {
            // Written, 20.12.2018

            this.mscMenu = !this.mscMenu;

            if (this.mscMenu)
            {
                FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = true;
            }
            else
            {
                FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = false;
            }
        }
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
            }
            else
            {
                FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu").Value = false;
            }
        }
        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {        
            // Written, 08.10.2018

            HoldInputMono him = this.gameObject.AddComponent<HoldInputMono>();
            him.setData("Open Mod GUI",
                XboxButtonEnum.Back,
                0.5f,
                this.toggleGui);
            if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.full))
                MoControlsMod.print(nameof(MoControlsGUI) + ": Started");
        }
        /// <summary>
        /// on Update.
        /// </summary>
        private void Update()
        {
            // Written, 22.08.2018

            if (this.openControlsGui.IsDown())
                this.toggleGui();
            if (this.controlsGuiOpened)
            {
                if (this.changeInputResult.reassignKey)
                {
                    MonitorInputData mid = Input.monitorForInput();
                    if (mid.foundInput)
                        this.controlManager.changeInput(mid.input);
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
                ueGUI.skin = ModLoader.guiskin;
                if (this.controlsGuiOpened)
                {
                    this.drawMainMenuGUI();
                    this.drawMainGUI();
                }
                else
                {
                    if (this.controlManager.displayCurrentPlayerModeOverlay)
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

            using (new gui.AreaScope(new Rect(MAIN_GUI_LEFT + 5f, MENU_GUI_TOP, this.mainGuiWidth - SCROLL_BAR_OFFSET, MENU_GUI_HEIGHT)))
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
            using (new gui.AreaScope(new Rect(MAIN_GUI_LEFT, (MENU_GUI_TOP + MENU_GUI_HEIGHT), this.mainGuiWidth, (Screen.height - (MAIN_GUI_TOP + MENU_GUI_TOP + MENU_GUI_HEIGHT)))))
            using (gui.ScrollViewScope scrollViewScope = new gui.ScrollViewScope(this.mainGUIScrollPosition, new GUILayoutOption[] { gui.Width(this.mainGuiWidth) }))
            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width(this.mainGuiWidth - SCROLL_BAR_OFFSET), gui.MaxWidth(this.mainGuiWidth - SCROLL_BAR_OFFSET) }))
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

            using (new gui.HorizontalScope())
            {
                ueGUI.backgroundColor = this.moduleBackgroundColor;
                this.drawXboxControllerDebugContent();
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

            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width((this.mainGuiWidth - SCROLL_BAR_OFFSET - 12.5f) / 2) }))
            {
                gui.Label("<b>Xbox Controller Input Viewer.</b>");               
                int j = 0;
                foreach (XboxControl xCon in this.xboxController.getXboxControls())
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
                    using (new gui.HorizontalScope())
                    {
                        gui.Label(xCon.texture);
                        gui.Label(xCon.inputName);
                    }
                }
            }
        }
        /// <summary>
        /// Draws debug content to the main gui.
        /// </summary>
        private void drawXboxControllerDebugContent()
        {
            // Written, 20.08.2018

            using (new gui.VerticalScope("box", new GUILayoutOption[] { gui.Width((this.mainGuiWidth - SCROLL_BAR_OFFSET - 12.5f) / 2) }))
            {
                gui.Label("<b>Xbox Controller Debug</b>");
                gui.Space(5f);
                gui.Label("<i><b>Xbox Controller Status:</b> <color=" + (this.xboxController.isConnected ? "green>Connected" : "red>Disconnected") + "</color>.</i>");
                if (this.xboxController.isConnected)
                {
                    // Triggers
                    ueGUI.backgroundColor = this.primaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        double leftRounded = Math.Round(this.xboxController.getLeftTrigger(), 2);
                        double rightRounded = Math.Round(this.xboxController.getRightTrigger(), 2);
                        gui.Label("<b>Triggers:</b>");
                        gui.Label(String.Format("Left: {0}, <b><color=white>{2}</color></b>\r\nRight: {1}, <b><color=white>{3}</color></b>",
                            leftRounded > 0.0f || leftRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", leftRounded) : leftRounded.ToString(),
                            rightRounded > 0.0f || rightRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", rightRounded) : rightRounded.ToString(),
                            this.xboxController.LT.inputName,
                            this.xboxController.RT.inputName));
                    }
                    // Bumpers
                    gui.Space(5f);
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState lb = this.xboxController.LB.state;
                        ButtonState rb = this.xboxController.RB.state;
                        gui.Label("<b>Bumpers:</b>");
                        gui.Label(String.Format("Left: {0}, <b><color=white>{2}</color></b>\r\nRight: {1}, <b><color=white>{3}</color></b>",
                            lb == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", lb) : lb.ToString(),
                            rb == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", rb) : rb.ToString(),
                            this.xboxController.LB.inputName,
                            this.xboxController.RB.inputName));
                    }
                    // Left Thumbsick
                    gui.Space(5f);
                    ueGUI.backgroundColor = this.primaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        double xRounded = Math.Round(this.xboxController.getLeftStick().X, 2);
                        double yRounded = Math.Round(this.xboxController.getLeftStick().Y, 2);
                        ButtonState ls = this.xboxController.LS.state;
                        gui.Label("<b>Left Thumbstick:</b>");
                        gui.Label(String.Format("X: {0}, <b><color=white>{3}/{4}</color></b>\r\nY: {1}, <b><color=white>{5}/{6}</color></b>\r\nLS: {2}, <b><color=white>{7}</color></b>",
                            xRounded > 0.0f || xRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", xRounded) : xRounded.ToString(),
                            yRounded > 0.0f || yRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", yRounded) : yRounded.ToString(),
                            ls == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", ls) : ls.ToString(),
                            this.xboxController.leftThumbstick.left.inputName,
                            this.xboxController.leftThumbstick.right.inputName,
                            this.xboxController.leftThumbstick.up.inputName,
                            this.xboxController.leftThumbstick.down.inputName,
                            this.xboxController.LS.inputName));
                    }
                    // Right Thumbstick
                    gui.Space(5f);
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        double xRounded = Math.Round(this.xboxController.getRightStick().X, 2);
                        double yRounded = Math.Round(this.xboxController.getRightStick().Y, 2);
                        ButtonState rs = this.xboxController.RS.state;
                        gui.Label("<b>Right Thumbstick:</b>");
                        gui.Label(String.Format("X: {0}, <b><color=white>{3}/{4}</color></b>\r\nY: {1}, <b><color=white>{5}/{6}</color></b>\r\nRS: {2}, <b><color=white>{7}</color></b>",
                            xRounded > 0.0f || xRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", xRounded) : xRounded.ToString(),
                            yRounded > 0.0f || yRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", yRounded) : yRounded.ToString(),
                            rs == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", rs) : rs.ToString(),
                            this.xboxController.rightThumbstick.left.inputName,
                            this.xboxController.rightThumbstick.right.inputName,
                            this.xboxController.rightThumbstick.up.inputName,
                            this.xboxController.rightThumbstick.down.inputName,
                            this.xboxController.RS.inputName
                            ));
                    }
                    // Buttons
                    gui.Space(5f);
                    ueGUI.backgroundColor = this.primaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState a = this.xboxController.A.state;
                        ButtonState b = this.xboxController.B.state;
                        ButtonState x = this.xboxController.X.state;
                        ButtonState y = this.xboxController.Y.state;
                        ButtonState start = this.xboxController.Start.state;
                        ButtonState back = this.xboxController.Back.state;
                        gui.Label("<b>Buttons:</b>");
                        gui.Label(String.Format("A: {0}, <b><color=white>{6}</color></b>\r\nB: {1}, <b><color=white>{7}</color></b>\r\nX: {2}, <b><color=white>{8}</color></b>\r\nY: {3}, <b><color=white>{9}</color></b>\r\nStart: {4}, <b><color=white>{10}</color></b>\r\nBack: {5}, <b><color=white>{11}</color></b>",
                            a == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", a) : a.ToString(),
                            b == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", b) : b.ToString(),
                            x == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", x) : x.ToString(),
                            y == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", y) : y.ToString(),
                            start == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", start) : start.ToString(),
                            back == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", back) : back.ToString(),
                            this.xboxController.A.inputName,
                            this.xboxController.B.inputName,
                            this.xboxController.X.inputName,
                            this.xboxController.Y.inputName,
                            this.xboxController.Start.inputName,
                            this.xboxController.Back.inputName));
                    }
                    // D-Pad
                    gui.Space(5f);
                    ueGUI.backgroundColor = this.secondaryItemColor;
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState u = this.xboxController.DPadUp.state;
                        ButtonState d = this.xboxController.DPadDown.state;
                        ButtonState l = this.xboxController.DPadLeft.state;
                        ButtonState r = this.xboxController.DPadRight.state;
                        gui.Label("<b>D-Pad:</b>");
                        gui.Label(String.Format("Up: {0}, <b><color=white>{4}</color></b>\r\nDown: {1}, <b><color=white>{5}</color></b>\r\nLeft: {2}, <b><color=white>{6}</color></b>\r\nRight: {3}, <b><color=white>{7}</color></b>",
                            u == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", u) : u.ToString(),
                            d == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", d) : d.ToString(),
                            l == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", l) : l.ToString(),
                            r == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", r) : r.ToString(),
                            this.xboxController.DPad.up.inputName,
                            this.xboxController.DPad.down.inputName,
                            this.xboxController.DPad.left.inputName,
                            this.xboxController.DPad.right.inputName));
                    }
                }
            }
            ueGUI.backgroundColor = this.backgroundColor;
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
                if (gui.Toggle(this.mouseEmulator.Emulating, String.Format("{0}: Using {1}", "Emulate mouse for controller", this.mouseEmulator.inputType)) != this.mouseEmulator.Emulating)
                {
                    this.mouseEmulator.Emulating = !this.mouseEmulator.Emulating;
                    saveSettings = true;
                }
            }
            gui.Space(5f);
            using (new gui.HorizontalScope("box"))
            {
                using (new gui.VerticalScope())
                {
                    gui.Label(String.Format("Deadzone: {0}", this.mouseEmulator.deadzone));
                    tempValue = gui.HorizontalSlider(this.mouseEmulator.deadzone, MouseEmulator.MIN_DEADZONE, MouseEmulator.MAX_DEADZONE);
                    if (tempValue != this.mouseEmulator.deadzone) // Value Changed.
                    {
                        this.mouseEmulator.deadzone = tempValue;
                        saveSettings = true;
                    }
                }
            }
            gui.Space(5f);
            using (new gui.HorizontalScope("box"))
            {
                using (new gui.VerticalScope())
                {
                    gui.Label(String.Format("Sensitivity: {0}", this.mouseEmulator.sensitivity));
                    tempValue = gui.HorizontalSlider(this.mouseEmulator.sensitivity, MouseEmulator.MIN_SENSITIVITY, MouseEmulator.MAX_SENSITIVITY);
                    if (tempValue != this.mouseEmulator.sensitivity) // Value Changed.
                    {
                        this.mouseEmulator.sensitivity = tempValue;
                        saveSettings = true;
                    }
                }
            }
            gui.Space(5f);
            using (new gui.HorizontalScope("box"))
            {
                using (new gui.VerticalScope())
                {
                    gui.Label("<i><b>Emulate mouse on joystick:</b></i>");
                    // As left + right thumb stick settings are grouped; need to manually change other value..
                    bool _asInput = (this.mouseEmulator.inputType == InputTypeEnum.LS);
                    if (gui.Toggle(_asInput, String.Format("Left Stick: {0}", _asInput ? "ON" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.inputType != InputTypeEnum.LS)
                        {
                            this.mouseEmulator.inputType = InputTypeEnum.LS;
                            saveSettings = true;
                        }
                    }
                    _asInput = (this.mouseEmulator.inputType == InputTypeEnum.RS);
                    if (gui.Toggle(_asInput, String.Format("Right Stick: {0}", _asInput ? "ON" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.inputType != InputTypeEnum.RS)
                        {
                            this.mouseEmulator.inputType = InputTypeEnum.RS;
                            saveSettings = true;
                        }
                    }
                    _asInput = (this.mouseEmulator.inputType == InputTypeEnum.DPad);
                    if (gui.Toggle(_asInput, String.Format("Directional Pad: {0}", _asInput ? "ON" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.inputType != InputTypeEnum.DPad)
                        {
                            this.mouseEmulator.inputType = InputTypeEnum.DPad;
                            saveSettings = true;
                        }
                    }
                }
            }
            gui.Space(5f);
            using (new gui.HorizontalScope("box"))
            {
                using (new gui.VerticalScope())
                {
                    gui.Label("<i><b>Deadzone type:</b></i>");
                    bool _asInput = (this.mouseEmulator.deadzoneType == DeadzoneTypeEnum.Radial);
                    if (gui.Toggle(_asInput, String.Format("Radial: {0}", _asInput ? "ON" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.deadzoneType != DeadzoneTypeEnum.Radial)
                        {
                            this.mouseEmulator.deadzoneType = DeadzoneTypeEnum.Radial;
                            saveSettings = true;
                        }
                    }
                    _asInput = (this.mouseEmulator.deadzoneType == DeadzoneTypeEnum.ScaledRadial);
                    if (gui.Toggle(_asInput, String.Format("Scaled Radial: {0}", _asInput ? "ON" : "")) != _asInput)
                    {
                        if (this.mouseEmulator.deadzoneType != DeadzoneTypeEnum.ScaledRadial)
                        {
                            this.mouseEmulator.deadzoneType = DeadzoneTypeEnum.ScaledRadial;
                            saveSettings = true;
                        }
                    }
                }
            }
            gui.Space(5f);
            ueGUI.backgroundColor = this.moduleBackgroundColor;
            using (new gui.VerticalScope("box"))
            {
                gui.Label("<i><b>Mouse inputs:</b></i>");
                ueGUI.backgroundColor = this.primaryItemColor;
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbPrimaryInput.Name));
                    using (new gui.HorizontalScope())
                    {
                        this.drawCommonControl("Modifier", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                        this.drawCommonControl("Input", this.mouseEmulator.lmbPrimaryInput.ID, this.mouseEmulator.lmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
                    }
                }

                gui.Space(3f);
                ueGUI.backgroundColor = this.secondaryItemColor;
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbSecondaryInput.Name));
                    using (new gui.HorizontalScope())
                    {
                        this.drawCommonControl("Modifier", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                        this.drawCommonControl("Input", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                    }
                };
                gui.Space(3f);
                ueGUI.backgroundColor = this.primaryItemColor;
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.rmbPrimaryInput.Name));
                    using (new gui.HorizontalScope())
                    {
                        this.drawCommonControl("Modifier", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Modifier.ToString(), 1, inMod: this.mod);
                        this.drawCommonControl("Input", this.mouseEmulator.rmbPrimaryInput.ID, this.mouseEmulator.rmbPrimaryInput.Key.ToString(), 2, inMod: this.mod);
                    }
                }
                gui.Space(3f);
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
            ueGUI.backgroundColor = this.backgroundColor;
            gui.Space(5f);
            if (saveSettings)
            {
                MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
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
                if (gui.Toggle(this.controlManager.displayCurrentPlayerModeOverlay, "Display current player mode overlay") != this.controlManager.displayCurrentPlayerModeOverlay)
                {
                    this.controlManager.displayCurrentPlayerModeOverlay = !this.controlManager.displayCurrentPlayerModeOverlay;
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
                MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
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
            gui.Space(5f);
            int j = 0;
            for (int i = 0; i < inControlInputs.GetLength(0); i++)
            {
                j++;
                if (j == 2)
                {
                    j = 0;
                    ueGUI.backgroundColor = this.primaryItemColor;
                }
                else
                    ueGUI.backgroundColor = this.secondaryItemColor;
                string controlName = inControlInputs[i, 0];
                gui.Space(3f);
                using (new gui.VerticalScope("box"))
                {
                    gui.Label(String.Format("<b>{0}:</b>", controlName.getGameControlAlias(true)));
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
            ueGUI.backgroundColor = this.backgroundColor;
            gui.Space(3f);
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
        private bool drawGeneralMenu<T>(T inSelected, out T changedTo)
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
                        changedTo = _enum;
                        return true;
                    }
                }
                ueGUI.backgroundColor = this.backgroundColor;
                changedTo = inSelected;
                return false;
            }
        }
       
        #endregion
    }
}
