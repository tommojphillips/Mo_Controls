using System;
using System.Linq;
using UnityEngine;
using gui = UnityEngine.GUILayout;
using MSCLoader;
using HutongGames.PlayMaker;
using TommoJProdutions.MoControls.MouseEmulation;
using TommoJProdutions.MoControls.XInputInterpreter;
using XInputDotNetPure;

namespace TommoJProdutions.MoControls.GUI
{
    /// <summary>
    /// Represents xbox extention methods.
    /// </summary>
    public static class XboxExtentions
    {
        // Written, 09.10.2018

        public static void forEach(this XboxControl[] inXboxControl, Func<Action<XboxControl>> inFunc)
        {
            // Written, 09.10.2018
            try
            {

                if (inFunc is null)
                    throw new NullReferenceException();
                inFunc?.Invoke();
            }
            catch (NullReferenceException ex)
            {
                if (MoControlsMod.debug)
                    MoControlsMod.print(String.Format("<i>[XboxExtentions.forEach(this XboxControl, Action)]</i> - <color=red>Action was null\r\n<b>StackTrace:</b> {0}</color>",
                        ex.StackTrace));
                return;
            }
        }
    }

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

        /// <summary>
        /// Represents the color of the button's text when selected (menu item).
        /// </summary>
        private Color buttonTextSelectedColor = new Color32(135, 135, 135, 255); // Light Gray
        /// <summary>
        /// Represents the color of the button's text when not selected (menu item).
        /// </summary>
        private Color buttonTextNotSelectedColor = new Color32(118, 159, 222, 255); // Light Blue
        /// <summary>
        /// Represents the color of the button's background when selected (menu item).
        /// </summary>
        private Color buttonSelectedColor = new Color32(118, 159, 222, 255); // Light Blue
        /// <summary>
        /// Represents the color of the button's background when not selected (menu item).
        /// </summary>
        private Color buttonNotSelectedColor = new Color32(135, 135, 135, 255); // Light Gray
        /// <summary>
        /// Represents the default color.
        /// </summary>
        private Color defaultColor => primaryBackgroundColor;//new Color(1, 1, 1, 1); // Some sort of dark-gray : black ratio
        /// <summary>
        /// Represents the primary background color.
        /// </summary>
        private Color primaryBackgroundColor = new Color(1, 1, 1, 0.868f); // transparent gray.
        /// <summary>
        /// Represents the background color.
        /// </summary>
        private Color secondaryBackgroundColor = new Color32(118, 159, 222, 226); // Light Blue
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
        /// Represents the option of input for the xbox controller*.
        /// </summary>
        private XboxControllerInputMapEnum xboxControllerInputOption;
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
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            if (MoControlsMod.debug)
                MoControlsMod.print(nameof(MoControlsGUI) + ": Started");
        }
        /// <summary>
        /// on Update.
        /// </summary>
        private void Update()
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
            if (this.controlsGuiOpened)
            {
                if (this.changeInputResult.reassignKey)
                {
                    MonitorInputData mid = Input.monitorForInput();
                    if (mid.foundInput)
                        this.changeInput(mid.input);
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
                UnityEngine.GUI.skin = ModLoader.guiskin;
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

            using (new gui.HorizontalScope("box"))
                gui.Label(String.Format("<b>About:</b>\r\n\r\n{0}", aboutMessage));
            gui.Space(5f);
            using (new gui.HorizontalScope("box"))
                gui.Label(String.Format("<b>Features:</b>\r\n{0}{1}", joinPrefix, String.Join(joinPrefix, features)));
            gui.Space(10f);
            using (new gui.HorizontalScope("box"))
                gui.Label(footerMessage);
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
                this.drawXboxControllerDebugContent();
                this.drawControllerInputContent();
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
                gui.Label("<b>Xbox Controller Input Editor.</b>");
                this.drawConInputMenu();
                XboxControl[] xboxControls = null;

                switch (this.xboxControllerInputOption)
                {
                    case XboxControllerInputMapEnum.Norm:
                        xboxControls = XboxController.normalXboxControls;
                        break;
                    case XboxControllerInputMapEnum.Alt:
                        xboxControls = XboxController.altXboxControls;
                        break;
                    case XboxControllerInputMapEnum.Custom:
                        xboxControls = xboxController.customXboxControls;
                        break;
                }
                gui.Label((this.xboxControllerInputOption as XboxControllerInputMapEnum?).toString());
                int j = 0;
                foreach (XboxControl xCon in xboxControls)
                {
                    j++;
                    if (j == 2)
                    {
                        j = 0;
                        UnityEngine.GUI.backgroundColor = this.secondaryBackgroundColor;
                    }
                    else
                        UnityEngine.GUI.backgroundColor = this.defaultColor;
                    this.drawCommonControllerInputControl(xCon, _readonly: (this.xboxControllerInputOption != XboxControllerInputMapEnum.Custom));
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
                        gui.Label(String.Format("Left: {0}, <b>{2}</b>\r\nRight: {1}, <b>{3}</b>",
                            leftRounded > 0.0f || leftRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", leftRounded) : leftRounded.ToString(),
                            rightRounded > 0.0f || rightRounded < 0.0f ? String.Format("<color=yellow>{0}</color>", rightRounded) : rightRounded.ToString(),
                            this.xboxController.LT.inputName,
                            this.xboxController.RT.inputName));
                    }
                    // Bumpers
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState lb = this.xboxController.LB.state;
                        ButtonState rb = this.xboxController.RB.state;
                        gui.Label("Bumpers:");
                        gui.Label(String.Format("Left: {0}, <b>{2}</b>\r\nRight: {1}, <b>{3}</b>",
                            lb == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", lb) : lb.ToString(),
                            rb == ButtonState.Pressed ? String.Format("<color=yellow>{0}</color>", rb) : rb.ToString(),
                            this.xboxController.LB.inputName,
                            this.xboxController.RB.inputName));
                    }
                    // Left Thumbsick
                    gui.Space(5f);
                    using (new gui.VerticalScope("box"))
                    {
                        double xRounded = Math.Round(this.xboxController.getLeftStick().X, 2);
                        double yRounded = Math.Round(this.xboxController.getLeftStick().Y, 2);
                        ButtonState ls = this.xboxController.LS.state;
                        gui.Label("Left Thumbstick:");
                        gui.Label(String.Format("X: {0}, <b>{3}/{4}</b>\r\nY: {1}, <b>{5}/{6}</b>\r\nLS: {2}, <b>{7}</b>",
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
                    using (new gui.VerticalScope("box"))
                    {
                        double xRounded = Math.Round(this.xboxController.getRightStick().X, 2);
                        double yRounded = Math.Round(this.xboxController.getRightStick().Y, 2);
                        ButtonState rs = this.xboxController.RS.state;
                        gui.Label("Right Thumbstick:");
                        gui.Label(String.Format("X: {0}, <b>{3}/{4}</b>\r\nY: {1}, <b>{5}/{6}</b>\r\nRS: {2}, <b>{7}</b>",
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
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState a = this.xboxController.A.state;
                        ButtonState b = this.xboxController.B.state;
                        ButtonState x = this.xboxController.X.state;
                        ButtonState y = this.xboxController.Y.state;
                        ButtonState start = this.xboxController.Start.state;
                        ButtonState back = this.xboxController.Back.state;
                        gui.Label("Buttons");
                        gui.Label(String.Format("A: {0}, <b>{6}</b>\r\nB: {1}, <b>{7}</b>\r\nX: {2}, <b>{8}</b>\r\nY: {3}, <b>{9}</b>\r\nStart: {4}, <b>{10}</b>\r\nBack: {5}, <b>{11}</b>",
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
                    using (new gui.VerticalScope("box"))
                    {
                        ButtonState u = this.xboxController.DPadUp.state;
                        ButtonState d = this.xboxController.DPadDown.state;
                        ButtonState l = this.xboxController.DPadLeft.state;
                        ButtonState r = this.xboxController.DPadRight.state;
                        gui.Label("D-Pad");
                        gui.Label(String.Format("Up: {0}, <b>{4}</b>\r\nDown: {1}, <b>{5}</b>\r\nLeft: {2}, <b>{6}</b>\r\nRight: {3}, <b>{7}</b>",
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
        }
        /// <summary>
        /// Draws mouse emulation content to the main gui.
        /// </summary>
        private void drawMouseEmulationContent()
        {
            // Written, 22.08.2018

            float tempValue;
            bool saveSettings = false;

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
            gui.Label("<i><b>Mouse inputs:</b></i>");
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
            using (new gui.VerticalScope("box"))
            {
                gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.lmbSecondaryInput.Name));
                using (new gui.HorizontalScope())
                {
                    this.drawCommonControl("Modifier", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                    this.drawCommonControl("Input", this.mouseEmulator.lmbSecondaryInput.ID, this.mouseEmulator.lmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                }
            }
            gui.Space(3f);
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
            using (new gui.VerticalScope("box"))
            {
                gui.Label(String.Format("<b>{0}:</b>", this.mouseEmulator.rmbSecondaryInput.Name));
                using (new gui.HorizontalScope())
                {
                    this.drawCommonControl("Modifier", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Modifier.ToString(), 1, inMod: this.mod);
                    this.drawCommonControl("Input", this.mouseEmulator.rmbSecondaryInput.ID, this.mouseEmulator.rmbSecondaryInput.Key.ToString(), 2, inMod: this.mod);
                }
            }

            gui.Space(5f);
            if (saveSettings)
            {
                MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
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

            if (gui.Toggle(this.controlManager.displayCurrentPlayerModeOverlay, "Display current player mode overlay") != this.controlManager.displayCurrentPlayerModeOverlay)
            {
                this.controlManager.displayCurrentPlayerModeOverlay = !this.controlManager.displayCurrentPlayerModeOverlay;
                _saveSettings = true;
            }

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
            Color prevColor = UnityEngine.GUI.backgroundColor;
            UnityEngine.GUI.backgroundColor = this.defaultColor;
            if (xboxControl != null && reassignMessage == null && this.xboxController.assetsLoaded)
            {
                if (gui.Button(xboxControl.texture))
                {
                    buttonClicked = true;
                }
            }
            else
            {
                if (gui.Button(reassignMessage ?? (!this.xboxController.assetsLoaded ? "Asset not loaded" : "") + inInputName))
                {
                    buttonClicked = true;
                }
            }
            UnityEngine.GUI.backgroundColor = prevColor;
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

                PlayerModeEnum? playerMode = this.changeInputResult?.mode;

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
                        this.controlManager.currentControls = ControlManager.loadControlInputsFromCInput();
                    }
                }
                else
                {
                    this.controlManager.setGameControl((PlayerModeEnum)playerMode, this.changeInputResult.controlName, this.changeInputResult.index, input);
                    MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
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
                if (MoControlsMod.debug)
                    MoControlsMod.print("saved mo'controls MSCLoader mod keybinds.");

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
            int j = 0;
            for (int i = 0; i < inControlInputs.GetLength(0); i++)
            {
                j++;
                if (j == 2)
                {
                    j = 0;
                    UnityEngine.GUI.backgroundColor = this.secondaryBackgroundColor;
                }
                else
                    UnityEngine.GUI.backgroundColor = this.defaultColor;
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
            gui.Label(String.Format("<b>Total Mod Keybinds: {0}</b>", modKeybindCount));
            foreach (Mod _mod in ModLoader.LoadedMods)
            {
                Keybind[] modKeybinds = Keybind.Get(_mod).ToArray();

                if (modKeybinds.Count() > 0)
                {
                    gui.Space(3f);
                    UnityEngine.GUI.backgroundColor = this.secondaryBackgroundColor;
                    using (new gui.HorizontalScope("box"))
                    {
                        gui.Label(String.Format("<b>{0}</b>, by <b>{1}</b>:", _mod.Name, _mod.Author));
                        using (new gui.VerticalScope())
                        {
                            for (int i = 0; i < modKeybinds.Length; i++)
                            {
                                j++;
                                if (j == 2)
                                {
                                    j = 0;
                                    UnityEngine.GUI.backgroundColor = this.secondaryBackgroundColor;
                                }
                                else
                                    UnityEngine.GUI.backgroundColor = this.defaultColor;
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
            UnityEngine.GUI.backgroundColor = this.defaultColor;
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
                    {
                        UnityEngine.GUI.contentColor = this.buttonTextSelectedColor;
                        UnityEngine.GUI.backgroundColor = this.buttonSelectedColor;
                    }
                    else
                    {
                        UnityEngine.GUI.contentColor = this.buttonTextNotSelectedColor;
                        UnityEngine.GUI.backgroundColor = this.buttonNotSelectedColor;
                    }

                    if (gui.Button(title ?? _enum.ToString()) && !isSelected)
                    {
                        changedTo = _enum;
                        return true;
                    }
                }
                UnityEngine.GUI.contentColor = this.defaultColor;
                UnityEngine.GUI.backgroundColor = this.defaultColor;
                changedTo = inSelected;
                return false;
            }
        }
        /// <summary>
        /// Represents common controller input control.
        /// </summary>
        /// <param name="inXboxControl"></param>
        private void drawCommonControllerInputControl(XboxControl inXboxControl, bool _readonly)
        {
            // Written, 09.10.2018

            using (new gui.VerticalScope("box"))
            {
                XboxControl xC = this.xboxController.getXboxControlByInputName(inXboxControl.inputName);
                using (new gui.HorizontalScope())
                {
                    gui.Label(xC.texture);
                    if (_readonly)
                        gui.Label(xC.inputName);
                    else
                        inXboxControl.setInputName(gui.TextField(xC.inputName));
                }
            }
        }
        /// <summary>
        /// Draws menu for controller input.
        /// </summary>
        private void drawConInputMenu()
        {
            // Written, 09.10.2018

            if (this.drawGeneralMenu(this.xboxControllerInputOption, out XboxControllerInputMapEnum changedTo))
            {
                if (changedTo != XboxControllerInputMapEnum.Custom) // not implemented yet.
                    this.xboxControllerInputOption = changedTo;
            }
        }

        #endregion
    }
}
