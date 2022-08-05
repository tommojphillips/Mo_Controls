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
using static UnityEngine.GUILayout;
using static UnityEngine.GUI;
using ScrollViewScope = UnityEngine.GUILayout.ScrollViewScope;
using System.ComponentModel;
using System.Reflection;

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
        const string aboutMessage = "<b>Mo'Controls</b> allows the player to have a primary and secondary input for each in-game control," +
                " the player could set all primary inputs to the keyboard and all secondary inputs to an Xbox Controller to have a seamless" +
                " swap of the keyboard to Xbox Controller. Mo'Controls also allows the player to have different control profiles for when " +
                "on foot and when in driving mode! So you can get more out of your controller!";         
        string[] features = new string[]
            {
                "Xbox controller support",
                "Assign two inputs to each game control",
                "Mouse emulation; use your Xbox controller to control the mouse",
                "Split control modes for driving & walking",
                "Toggle tool/hand mode by holding down the <i>Start</i> button on a connected xbox controller for > 0.3sec",
                "Controller gui navigation",
                "FFB: Controller vibration/rumble effects; rumble options based on default (toplessgun) or rpm, wheel-slip, gear change. See (Settings => Xbox Controller",
                "\nAuto-Enables scroll on the controller triggers if player is looking at something that has a Scroll function."
            };          
        const string footerMessage = "Developed by <b>Tommo J. Armytage. | Latest release: " + VersionInfo.version + "</b>";        
        
        private string joinPrefix = "\r\n# ";
        
        private GUISkin defaultSkin;

        private GUIStyle marginStyle = new GUIStyle()
        {
            margin = new RectOffset() 
            {
                left = 100
            }
        };
        private int itemWidth = 160;
        private int marginOffset = 200;
        private int maxItemsPerRow; 
        private int drawCount;

        #endregion

        #region GUI Colors

        private Texture2D defaultBackgroundColor;
        private Texture2D defaultButtonColor;
        private Texture2D primaryItemColor;
        private Texture2D moduleBackgroundColor;
        private Texture2D unselectedMenuButtonColor;
        private Texture2D selectedMenuButtonColor;
        private Color32 defaultContentColor = new Color32(255, 255, 255, 255); // | White
        private Color xboxButtonPressedContentColor = Color.gray;
   

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

        

        #region drawToggleGroup Data

        private bool containsColorTag;
        private int indexOfColorTag;
        private string newName;
        private string colorTag;
        private bool predict;
        private object currentValue;
        private Array enumValues;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Represents the instance of the mod.
        /// </summary>
        private MoControlsMod mod => MoControlsMod.instance;
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
        /// Represents the xbox controller. references <see cref="MoControlsMod.xboxController"/>.
        /// </summary>
        private XboxController xboxController
        {
            get
            {
                return controlManager.xboxController;
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
        internal bool controlsGuiOpened { get; private set; } = false;
        /// <summary>
        /// Represents the gui navigation system for the 
        /// </summary>
        internal GuiNav guiNav
        {
            get;
            private set;
        }

        #endregion

        #region unity runtime

        private void Awake()
        {
            // Written, 08.10.2018

            setUpGuiNav();
            setUpOpenGuiHoldInput();
            setUpGui();
        }

        private void Update()
        {
            // Written, 22.08.2018 | Updated, 04.08.2022

            if (MoControlsMod.instance.openControlsGui.GetKeybindDown())
                toggleGui();
            if (controlsGuiOpened)
            {
                if (controlManager.changeInputResult.reassignKey)
                {
                    guiNav.enabled = false;
                    MonitorInputData mid = Input.monitorForInput();
                    if (mid.foundInput)
                    {
                        if (validateInput(mid))
                        {
                            controlManager.changeInput(mid.input);
                        }
                        else
                        {
                            controlManager.changeInputResult.reset();
                        }
                        guiNav.enabled = true;
                    }
                }
                else
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                    {
                        controlsGuiOpened = false;
                        guiNav.enabled = false;
                    }
                }
            }
        }
        
        private void OnGUI()
        {
            // Written, 22.08.2018

            skin.box.normal.background = defaultBackgroundColor;
            if (controlsGuiOpened)
            {
                drawMainMenuGUI();
                drawMainGUI();
            }
            else
            {
                drawOverlayGUI();
            }
            skin = defaultSkin;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draws all overlays in mo'controls.
        /// </summary>
        private void drawOverlayGUI() 
        {
            // Written, 05.08.2022

            using (new AreaScope(new Rect(Screen.width / 2, 5, 200, 200)))
            {
                if (MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay)
                    Label(String.Format("{0} ({1} mode)", MoControlsGO.controlManager.getCurrentPlayerMode, !controlManager.isInHandMode ? "Tool" : "Hand"));
                if (controlManager.vehicle != null)
                {
                    if (MoControlsSaveData.loadedSaveData.displayFfbOverlay)
                        drawForceFeedBackOverlayGUI();
                    if (MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay)
                        drawDrivetrainOverlay();
                }
                else
                {
                    Label("No supported vehicle found.\nCurrent vehicle: " + ControlManager.playerCurrentVehicle);
                }
            }
        }
        /// <summary>
        /// checks the validity of the provided data.
        /// </summary>
        /// <param name="data">the data to validate</param>
        /// <returns>returns <see langword="true"/> if <paramref name="data"/> is vaild</returns>
        private bool validateInput(MonitorInputData data)
        {
            // Written, 04.08.2022

            string cName = controlManager.changeInputResult.controlName;
            if (MoControlsSaveData.loadedSaveData.usePlayerMoveAsInput)
            {
                if (cName == "PlayerLeft" || cName == "PlayerRight" || cName == "PlayerUp" || cName == "PlayerDown")
                {
                    XboxControl[] controls;
                    switch (MoControlsSaveData.loadedSaveData.playerMove)
                    {
                        case InputTypeEnum.DPad:
                            controls = xboxController.dPadControls;
                            break;
                        case InputTypeEnum.RS:
                            controls = xboxController.rightThumbstick.controls;
                            break;
                        default:
                        case InputTypeEnum.LS:
                            controls = xboxController.leftThumbstick.controls;
                            break;

                    }
                    if (controls.Any(c => c.inputName == data.input))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// sets up gui references. eg, textures, default skin, maxitems in a row.
        /// </summary>
        private void setUpGui()
        {
            // Written, 04.08.2022

            defaultBackgroundColor = createTextureFromColor(10, 10, new Color32(125, 125, 125, 255));
            primaryItemColor = createTextureFromColor(10, 10, new Color32(85, 143, 255, 255));
            moduleBackgroundColor = createTextureFromColor(10, 10, new Color32(85, 125, 170, 255));
            unselectedMenuButtonColor = createTextureFromColor(10, 10, new Color32(133, 162, 250, 255));
            selectedMenuButtonColor = createTextureFromColor(10, 10, new Color32(80, 237, 90, 255));
            defaultButtonColor = createTextureFromColor(10, 10, new Color32(150, 150, 150, 255));

            defaultSkin = ScriptableObject.CreateInstance(typeof(GUISkin)) as GUISkin;

            maxItemsPerRow = (int)Math.Truncate((mainGuiWidth - marginOffset) / itemWidth);
        }
        /// <summary>
        /// sets up the hold input for opening the gui.
        /// </summary>
        private void setUpOpenGuiHoldInput()
        {
            // Written, 04.08.2022

            HoldInputMono him = gameObject.AddComponent<HoldInputMono>();
            him.setData("Open Mod GUI", XboxButtonEnum.Back, 0.3f, toggleGui);
        }
        /// <summary>
        /// sets up gui nav reference with controls. see: <see cref="guiNav"/>
        /// </summary>
        private void setUpGuiNav()
        {
            // Written, 04.08.2022

            guiNav = gameObject.AddComponent<GuiNav>();
            guiNav.setControls(XboxAxisEnum.RT, XboxButtonEnum.None, XboxAxisEnum.LT, XboxButtonEnum.None, XboxAxisEnum.None, XboxButtonEnum.RB, XboxAxisEnum.None, XboxButtonEnum.LB);
            guiNav.enabled = false;
        }
        /// <summary>
        /// creates a texure with the color provided.
        /// </summary>
        /// <param name="width">width of texture</param>
        /// <param name="height">height of texture</param>
        /// <param name="col">the color of the texture</param>
        /// <returns>a new texture instance of color param: <paramref name="col"/></returns>
        private Texture2D createTextureFromColor(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
        /// <summary>
        /// Draws a bool as a toggle.
        /// </summary>
        /// <param name="name">the name for the toggle.</param>
        /// <param name="value">the value.</param>
        /// <param name="saveSettings">reference to savesettings.</param>
        /// <returns>returns true if <paramref name="value"/> has changed</returns>
        private bool drawToggle(string name, ref bool value, ref bool saveSettings)
        {
            // Written, 25.07.2022

            if (Toggle(value, name) != value)
            {
                value = !value;
                saveSettings = true;
                return true;
            }
            return false;
        }        
        /// <summary>
        /// Draws all enums as toggles. in a group. only one can be selected.
        /// </summary>
        /// <typeparam name="T">the type enum this toggle group is drawing.</typeparam>
        /// <param name="name">the name of this toggle group.</param>
        /// <param name="reference">the current selected value.</param>
        /// <param name="saveSettings">reference to savesettings.</param>
        /// <returns>returns true if <paramref name="reference"/> has changed</returns>
        private bool drawToggleGroup<T>(string name, ref T reference, ref bool saveSettings) where T : Enum
        {
            // Written, 03.08.2022

            bool hasReferenceChanged = false;
            containsColorTag = name.Contains("<color=");
            if (containsColorTag)
            {
                indexOfColorTag = name.IndexOf("<color=");
                newName = name.Substring(0, indexOfColorTag);
                colorTag = name.Substring(indexOfColorTag);
            }
            else
            {
                newName = name;
            }
            enumValues = Enum.GetValues(typeof(T));
            Label(newName + ":");
            for (int i = 0; i < enumValues.Length; i++)
            {
                currentValue = enumValues.GetValue(i);
                predict = reference.Equals(currentValue);
                if (Toggle(predict, $"<b>{currentValue}:</b> {(containsColorTag ? colorTag : "")}{(predict ? "ON" : "")}{(containsColorTag ? "</color>" : "")}") != predict)
                {
                    reference = (T)currentValue;
                    saveSettings = true;
                    hasReferenceChanged = true;
                }
            }
            return hasReferenceChanged;
        }
        /// <summary>
        /// Changes boolean to open/close the  
        /// </summary>
        private void toggleGui()
        {
            // Written, 18.12.2018 | Modified, 09.10.2020

            controlsGuiOpened = !controlsGuiOpened;
            controlManager.playerInMenu = controlsGuiOpened;
            guiNav.enabled = controlsGuiOpened;
        }
        /// <summary>
        /// Draws the main menu for the main 
        /// </summary>
        private void drawMainMenuGUI()
        {
            // Written, 22.08.2018

            using (new AreaScope(new Rect(mainGuiLeft + 5f, MENU_GUI_TOP, mainGuiWidth - SCROLL_BAR_OFFSET, MENU_GUI_HEIGHT)))
            using (new HorizontalScope())
            {
                if (drawGeneralMenu(mainGUIMenu, out MainGUIMenuEnum changedTo))
                {
                    mainGUIMenu = changedTo;
                }
            }
        }
        /// <summary>
        /// Draws the main gui and it's children.
        /// </summary>
        private void drawMainGUI()
        {
            // Written, 22.08.2018

            bool saveSettings = false;
            using (new AreaScope(new Rect(mainGuiLeft, MENU_GUI_TOP + MENU_GUI_HEIGHT, mainGuiWidth, Screen.height - (MAIN_GUI_TOP + MENU_GUI_TOP + MENU_GUI_HEIGHT)), ""))
            using (ScrollViewScope scrollViewScope = new ScrollViewScope(mainGUIScrollPosition, Width(mainGuiWidth + SCROLL_BAR_OFFSET)))
            using (new VerticalScope("box", Width(mainGuiWidth)))
            {
                mainGUIScrollPosition = scrollViewScope.scrollPosition;
                Label($"<b>{mod.Name} v{mod.Version} by {mod.Author}</b>");
                if (mainGUIMenu != MainGUIMenuEnum.About)
                    Label($"<b>{MoControlsMod.instance.openControlsGui.Key}</b> GUI key bind.\n" +
                        $"<b>{Input.noneKey}</b> Sets as None.\n" +
                        $"<b>LMB</b> Selects.\n" +
                        $"<b>RMB</b> Cancels.");
                Space(3.0f);
                switch (mainGUIMenu)
                {
                    case MainGUIMenuEnum.About:
                        drawAboutContent();
                        break;
                    case MainGUIMenuEnum.DrivingControls:
                        drawDrivingControlContent();
                        break;
                    case MainGUIMenuEnum.FootControls:
                        drawFootControlContent(ref saveSettings);
                        break;
                    case MainGUIMenuEnum.Settings:
                        drawSettingsContent(ref saveSettings);
                        break;
                }
                skin.box.normal.background = moduleBackgroundColor;
                using (new VerticalScope(marginStyle, Width(mainGuiWidth - marginOffset)))
                using (new VerticalScope("box"))
                {
                    Label(footerMessage);
                    Space(10f);
                }
                skin.box.normal.background = defaultBackgroundColor;

                if (saveSettings)
                {
                    MoControlsSaveData.loadedSaveData.saveSettings();
                }
            }
        }        
        /// <summary>
        /// Draws about content to the main 
        /// </summary>
        private void drawAboutContent()
        {
            // Written, 20.08.2018
                    
            Space(5f);
            skin.box.normal.background = moduleBackgroundColor;
            using (new VerticalScope(marginStyle, Width(mainGuiWidth - marginOffset)))
            {
                using (new HorizontalScope("box"))
                    Label(String.Format("<b>About:</b>\r\n\r\n{0}", aboutMessage));
                Space(5f);
                using (new HorizontalScope("box"))
                    Label(String.Format("<b>Features:</b>\r\n{0}{1}", joinPrefix, String.Join(joinPrefix, features)));
            }
            skin.box.normal.background = defaultBackgroundColor;
        }
        /// <summary>
        /// Draws settings content to the main 
        /// </summary>
        private void drawSettingsContent(ref bool saveSettings)
        {
            // Written, 20.08.2018

            Space(3f);
            Label("<b>Settings</b>");
            Space(5f);
            drawSettingsMenu();

            switch (settingsMenu)
            {
                case SettingsMenuEnum.MouseEmulation:
                    drawMouseEmulationContent(ref saveSettings);
                    break;
                case SettingsMenuEnum.XboxController:
                    drawXboxControllerContent(ref saveSettings);
                    break;
                case SettingsMenuEnum.Axis:
                    drawDisplayContent(ref saveSettings);
                    break;
            }
        }
        /// <summary>
        /// Draws the settings menu bar on the settings tab.
        /// </summary>
        private void drawSettingsMenu()
        {
            // Written, 09.10.2018

            if (drawGeneralMenu(settingsMenu, out SettingsMenuEnum changedTo))
            {
                settingsMenu = changedTo;
            }
        }
        /// <summary>
        /// Draws xbox controller settings/content.
        /// </summary>
        private void drawXboxControllerContent(ref bool saveSettings)
        {
            // Written, 09.10.2018

            Label(string.Format("<b>Xbox Controller</b>: {0}", xboxController.isConnected ? "<color=green>connected</color>" : "<color=red>disconnected</color>"));
            Space(5f);
            skin.box.normal.background = moduleBackgroundColor;
            using (new VerticalScope(marginStyle))
            {
                using (new HorizontalScope("box", Width(mainGuiWidth - marginOffset)))
                {
                    string message;

                    using (new VerticalScope("box"))
                    {
                        Label($"using {(controlManager.usingController ? "controller" : "mouse/keyboard")}");
                        
                        Label($"unity: any key: {UnityEngine.Input.anyKey}");
                        Label($"keyboard: any key {KeyboardEmulator.anyInput()}");

                        Label($"Last keyboard input detected: {KeyboardEmulator.lastKeyPressed} ({KeyboardEmulator.currentKeyPressed})");
                    }
                    Space(5);
                    if (xboxController.isConnected)
                    {
                        using (new VerticalScope("box"))
                        {
                            Label("Xbox inputs");
                            Label($"any input: {xboxController.anyInput}");
                            Label($"any boolState input: {xboxController.anyButtonPressed}");
                            Label($"any floatState input: {xboxController.anyThumbstickInput}");

                            if (xboxController.lastControlInput != null)
                            {
                                message = $"Last xbox input detected: {xboxController.lastControlInput.name} | ";
                                
                                if (xboxController.lastControlInput is XboxBoolState)
                                {
                                    message += (xboxController.lastControlInput as XboxBoolState).state;
                                }
                                else
                                {
                                    message += (xboxController.lastControlInput as XboxFloatState).state.ToString("F2");
                                }
                                Label(message);
                            }
                        }
                    }
                }

                using (new VerticalScope(marginStyle))
                {
                    using (new HorizontalScope("box", Width(mainGuiWidth - marginOffset)))
                    {
                        drawToggleGroup("Controller deadzone type", ref MoControlsSaveData.loadedSaveData.xboxControllerDeadzoneType, ref saveSettings);
                    }
                }
                drawControllerFFBContent(ref saveSettings);
                drawControllerInputContent();
                drawCombinedTriggerAxisContent(ref saveSettings);
            }
            skin.box.normal.background = defaultBackgroundColor;
        }
        /// <summary>
        /// Draws combined trigger axis setting content.
        /// </summary>
        private void drawCombinedTriggerAxisContent(ref bool saveSettings)
        {
            // Written, 24.10.2020

            using (new HorizontalScope("box", Width(mainGuiWidth - marginOffset)))
            {
                bool combineTriggerAxis = MoControlsSaveData.loadedSaveData.combinedTriggerAxis;
                if (Toggle(combineTriggerAxis, String.Format("Combine trigger axis: {0}", combineTriggerAxis ? "<color=green>On</color>" : "<color=red>Off</color>")) != combineTriggerAxis)
                {
                    MoControlsSaveData.loadedSaveData.combinedTriggerAxis = !combineTriggerAxis;
                    xboxController.updateTriggerAxisInputName();
                    saveSettings = true;
                }
                Label("NOTE: after changing this value you will need to reassign desired gameContol inputs to triggers.");
            }
        }
        /// <summary>
        /// Draws controller input content.
        /// </summary>
        private void drawControllerInputContent()
        {
            // Written, 09.10.2018

            using (new VerticalScope("box", Width(mainGuiWidth - marginOffset)))
            {
                int colorIndex = 0;
                Vector2 stickMovement;
                bool isBoolState;
                GUIStyle stickMovementStyle = new GUIStyle();
                XboxControl[] controls = xboxController.xboxControls;

                for (int i = 1; i < controls.Length; i++)
                {
                    i--;
                    using (new HorizontalScope())
                    {
                        for (int n = 0; n < maxItemsPerRow; n++)
                        {
                            if (i >= controls.Length)
                                break;
                            colorIndexCheck(ref colorIndex);
                            using (new VerticalScope("box", new GUILayoutOption[] { Width(itemWidth), Height(32) }))
                            {
                                using (new HorizontalScope())
                                {
                                    isBoolState = controls[i] is XboxBoolState;
                                    if (isBoolState && xboxController.isConnected && (controls[i] as XboxBoolState).state == ButtonState.Pressed)
                                    {
                                        contentColor = xboxButtonPressedContentColor;
                                    }
                                    if (xboxController.lS == controls[i])
                                    {
                                        stickMovement = xboxController.getLeftStick();
                                        stickMovement.y *= -1;
                                        stickMovementStyle.contentOffset = stickMovement * 3;
                                        Label(controls[i].texture, style: stickMovementStyle);
                                    }
                                    else if (xboxController.rS == controls[i])
                                    {
                                        stickMovement = xboxController.getRightStick();
                                        stickMovement.y *= -1;
                                        stickMovementStyle.contentOffset = stickMovement * 3;
                                        Label(controls[i].texture, style: stickMovementStyle);
                                    }
                                    else
                                        Label(controls[i].texture);
                                    contentColor = defaultContentColor;
                                    using (new VerticalScope())
                                    {
                                        Label(controls[i].inputName);
                                        if (xboxController.isConnected)
                                        {
                                            Label(controls[i].ToString());
                                        }
                                    }
                                }
                            }
                            i++;
                        }
                        colorIndexCheck(ref colorIndex);
                    }
                }
            }
        }
        /// <summary>
        /// Draws controller force feedback content.
        /// </summary>
        private void drawControllerFFBContent(ref bool saveSettings)
        {
            // Written, 18.10.2020

            bool ffbOn = MoControlsSaveData.loadedSaveData.ffbOnXboxController;
            bool ffbOptDefault = MoControlsSaveData.loadedSaveData.ffbOption_default;
            bool ffbOptRpm = MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter;
            bool ffbOptWheelSlip = MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip;
            bool ffbOptGearChange = MoControlsSaveData.loadedSaveData.ffbOption_gearChange;
            using (new HorizontalScope("box", Width(mainGuiWidth - marginOffset)))
            {
                if (Toggle(ffbOn, String.Format("FFB: {0}", ffbOn ? "<color=green>On</color>" : "<color=red>Off</color>")) != ffbOn)
                {
                    MoControlsSaveData.loadedSaveData.ffbOnXboxController = !ffbOn;
                    xboxController.setRumble(XboxController.vector2Zero);
                    saveSettings = true;
                }
                Label("<i><b>FFB Options</b></i>");
                if (Toggle(ffbOptDefault, String.Format("Default ffb: {0}", ffbOptDefault ? "<color=green>On</color>" : "<color=red>Off</color>"))
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
                    saveSettings = true;
                }
                if (Toggle(ffbOptRpm, String.Format("RPM limiter ffb: {0}", ffbOptRpm ? "<color=green>On</color>" : "<color=red>Off</color>"))
                   != ffbOptRpm)
                {
                    MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter = !ffbOptRpm;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter)
                        if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                            MoControlsSaveData.loadedSaveData.ffbOption_default = false;
                    saveSettings = true;
                }
                if (Toggle(ffbOptWheelSlip, String.Format("Wheel slip ffb: {0}", ffbOptWheelSlip ? "<color=green>On</color>" : "<color=red>Off</color>"))
                   != ffbOptWheelSlip)
                {
                    MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip = !ffbOptWheelSlip;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip)
                        if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                            MoControlsSaveData.loadedSaveData.ffbOption_default = false;
                    saveSettings = true;
                }
                if (Toggle(ffbOptGearChange, String.Format("Gear change ffb: {0}", ffbOptGearChange ? "<color=green>On</color>" : "<color=red>Off</color>"))
                  != ffbOptGearChange)
                {
                    MoControlsSaveData.loadedSaveData.ffbOption_gearChange = !ffbOptGearChange;
                    if (MoControlsSaveData.loadedSaveData.ffbOption_gearChange)
                        if (MoControlsSaveData.loadedSaveData.ffbOption_default)
                            MoControlsSaveData.loadedSaveData.ffbOption_default = false;
                    saveSettings = true;
                }
            }
        }
        /// <summary>
        /// Draws mouse emulation content to the main 
        /// </summary>
        private void drawMouseEmulationContent(ref bool saveSettings)
        {
            // Written, 22.08.2018

            float tempValue;

            skin.box.normal.background = moduleBackgroundColor;
            Label("<b>Mouse Emulation</b>");
            Space(5f);
            using (new VerticalScope(marginStyle, Width(mainGuiWidth - marginOffset)))
            {
                using (new HorizontalScope("box"))
                {
                    Label("<b><color=" + (MoControlsSaveData.loadedSaveData.emulateMouse ? "green>ON" : "red>OFF") + "</color></b>");
                    drawToggle($"<b>Emulate mouse for controller:</b> Using {MoControlsSaveData.loadedSaveData.mouseInputType}", ref MoControlsSaveData.loadedSaveData.emulateMouse, ref saveSettings);
                }
                using (new HorizontalScope())
                {
                    using (new HorizontalScope("box"))
                    {
                        Label("<i><b>Emulate mouse on joystick:</b></i>");
                        GUIExtentions.drawPropertyEnum(ref MoControlsSaveData.loadedSaveData.mouseInputType, ref saveSettings);
                    }
                    using (new HorizontalScope("box"))
                    {
                        Label("<i><b>Deadzone type:</b></i>");
                        GUIExtentions.drawPropertyEnum(ref MoControlsSaveData.loadedSaveData.mouseDeadzoneType, ref saveSettings);
                    }
                }
                using (new HorizontalScope())
                {
                    using (new VerticalScope("box", Width(marginOffset)))
                    {
                        Label(String.Format("<b>Mouse Deadzone:</b> {0}", MoControlsSaveData.loadedSaveData.mouseDeadzone));

                        if (string.IsNullOrEmpty(mouseSettingsValue))
                            mouseSettingsValue = MoControlsSaveData.loadedSaveData.mouseDeadzone.ToString();
                        mouseSettingsValue = TextField(mouseSettingsValue);
                        if (Button("Set Mouse Deadzone"))
                        {
                            if (float.TryParse(mouseSettingsValue, out tempValue))
                            {
                                MoControlsSaveData.loadedSaveData.mouseDeadzone = tempValue;
                                saveSettings = true;
                            }
                        }
                    }
                    using (new VerticalScope("box", Width(marginOffset)))
                    {
                        Label(String.Format("<b>Mouse Sensitivity:</b> {0}", MoControlsSaveData.loadedSaveData.mouseSensitivity));

                        if (string.IsNullOrEmpty(mouseSettingsValue1))
                            mouseSettingsValue1 = MoControlsSaveData.loadedSaveData.mouseSensitivity.ToString();
                        mouseSettingsValue1 = TextField(mouseSettingsValue1);
                        if (Button("Set Mouse Sensitivity"))
                        {
                            if (float.TryParse(mouseSettingsValue1, out tempValue))
                            {
                                MoControlsSaveData.loadedSaveData.mouseSensitivity = tempValue;
                                saveSettings = true;
                            }
                        }
                    }
                }
                Space(5f);
                using (new VerticalScope())
                {
                    Label("<i><b>Mouse inputs:</b></i>");
                    using (new HorizontalScope(Width(itemWidth)))
                    {
                        skin.box.normal.background = primaryItemColor;
                        using (new VerticalScope("box"))
                        {

                            Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.lmbPrimaryInput.Name));
                            using (new HorizontalScope())
                            {
                                drawCommonControl(MoControlsMod.instance.lmbPrimaryInput.ID, MoControlsMod.instance.lmbPrimaryInput.Modifier.ToString(), 1);
                                drawCommonControl(MoControlsMod.instance.lmbPrimaryInput.ID, MoControlsMod.instance.lmbPrimaryInput.Key.ToString(), 2);
                            }
                        }
                        skin.box.normal.background = moduleBackgroundColor;
                        using (new VerticalScope("box"))
                        {
                            Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.lmbSecondaryInput.Name));
                            using (new HorizontalScope())
                            {
                                drawCommonControl(MoControlsMod.instance.lmbSecondaryInput.ID, MoControlsMod.instance.lmbSecondaryInput.Modifier.ToString(), 1);
                                drawCommonControl(MoControlsMod.instance.lmbSecondaryInput.ID, MoControlsMod.instance.lmbSecondaryInput.Key.ToString(), 2);
                            }
                        }
                    }
                    using (new HorizontalScope(Width(itemWidth)))
                    {
                        skin.box.normal.background = moduleBackgroundColor;
                        using (new VerticalScope("box"))
                        {
                            Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.rmbPrimaryInput.Name));
                            using (new HorizontalScope())
                            {
                                drawCommonControl(MoControlsMod.instance.rmbPrimaryInput.ID, MoControlsMod.instance.rmbPrimaryInput.Modifier.ToString(), 1);
                                drawCommonControl(MoControlsMod.instance.rmbPrimaryInput.ID, MoControlsMod.instance.rmbPrimaryInput.Key.ToString(), 2);
                            }
                        }
                        skin.box.normal.background = primaryItemColor;
                        using (new VerticalScope("box"))
                        {
                            Label(String.Format("<b>{0}:</b>", MoControlsMod.instance.rmbSecondaryInput.Name));
                            using (new HorizontalScope())
                            {
                                drawCommonControl(MoControlsMod.instance.rmbSecondaryInput.ID, MoControlsMod.instance.rmbSecondaryInput.Modifier.ToString(), 1);
                                drawCommonControl(MoControlsMod.instance.rmbSecondaryInput.ID, MoControlsMod.instance.rmbSecondaryInput.Key.ToString(), 2);
                            }
                        }
                    }
                }
            }
            skin.box.normal.background = defaultBackgroundColor;
        }
        /// <summary>
        /// Draws display settings content to the main 
        /// </summary>
        private void drawDisplayContent(ref bool saveSettings)
        {
            // Written, 22.08.2018

            skin.box.normal.background = moduleBackgroundColor;
            using (new VerticalScope(marginStyle, Width(mainGuiWidth - marginOffset)))
            {
                using (new HorizontalScope())
                {
                    using (new VerticalScope("box"))
                    {
                        Label("Overlays:");
                        drawToggle("Display current player mode overlay", ref MoControlsSaveData.loadedSaveData.displayCurrentPlayerModeOverlay, ref saveSettings);
                        drawToggle("Display force feedback overlay", ref MoControlsSaveData.loadedSaveData.displayFfbOverlay, ref saveSettings);
                        drawToggle("Display vehicle info overlay", ref MoControlsSaveData.loadedSaveData.displayVehicleInfoOverlay, ref saveSettings);
                    }
                    using (new VerticalScope("box"))
                    {
                        drawToggleGroup("Debug mode<color=green>", ref MoControlsSaveData.loadedSaveData.debugMode, ref saveSettings);
                    }
                }

                using (new VerticalScope("box"))
                {
                    if (Button("Reset Mo'Controls save data", Width(200)))
                    {
                        MoControlsSaveData.resetLoadedSettings();
                    }
                    using (new HorizontalScope("box"))
                    {
                        if (Button("Calibrate cInput", Width(200)))
                        {
                            controlManager.loadControlsToCInput(controlManager.footControls);
                            cInput.Calibrate();
                            controlManager.loadControlsToCInput(controlManager.drivingControls);
                            cInput.Calibrate();
                            controlManager.loadControlsToCInput(controlManager.blankControls);
                        }
                        Label("- calibrates analog (axis) inputs to their default/neutral position");
                    }
                }
                using (new VerticalScope("box"))
                { 
                    bool hasChanged = drawToggleGroup("Device detection<color=green>", ref MoControlsSaveData.loadedSaveData.deviceDetection, ref saveSettings);

                    if (hasChanged) 
                    {
                        if (MoControlsSaveData.loadedSaveData.deviceDetection != UsingDeviceEnum.Auto)
                        {
                            controlManager.usingController = MoControlsSaveData.loadedSaveData.deviceDetection == UsingDeviceEnum.xboxController;
                        }
                    }
                }

                using (new HorizontalScope("box"))
                {
                    Label("<i><b><color=blue>C</color><color=yellow>o</color>lo<color=red>r</color> <color=purple>M</color>ix<color=green>e</color>r</b></i>");
                }
            }
            skin.box.normal.background = defaultBackgroundColor;
        }
        /// <summary>
        /// Draws a common control for the 
        /// </summary>
        private void drawCommonControl(string inControlName, string inInputName, int inIndex, PlayerModeEnum? inMode = null)
        {
            // Written, 01.08.2018

            string reassignMessage = null;
            if (controlManager.changeInputResult.controlName == inControlName && controlManager.changeInputResult.index == inIndex && controlManager.changeInputResult.mode == inMode)
                reassignMessage = "<b>Awaiting key input</b>";
            XboxControl xboxControl;
            bool buttonClicked = false;
            skin.box.normal.background = unselectedMenuButtonColor;
            using (new HorizontalScope(Width(75)))
            {
                if (reassignMessage == null)
                {
                    xboxControl = xboxController.getXboxControlByInputName(inInputName);
                    if (xboxControl != null && MoControlsMod.assetsLoaded)
                    {
                        if (Button(xboxControl.texture))
                        {
                            buttonClicked = true;
                        }
                    }
                    else if (Button(inInputName))
                    {
                        buttonClicked = true;
                    }
                }
                else 
                {
                    Label(reassignMessage);
                }
            }
            skin.box.normal.background = defaultBackgroundColor;

            if (buttonClicked && !controlManager.changeInputResult.reassignKey)
            {
                controlManager.changeInputResult.changeToPollingState(inControlName, inIndex, inMode);
            }
        }
        /// <summary>
        /// draws the control input list.
        /// </summary>
        /// <param name="inControlInputs">The list to draw.</param>
        private void drawControlModeContent(string inTitle, string[,] inControlInputs)
        {
            // Written, 30.07.2022

            int length = inControlInputs.GetLength(0);
            int rowTotal = length / maxItemsPerRow;
            int rowStartindex = 0;
            int colorIndex = 0;

            Label($"<b>{inTitle}</b> {drawCount}");
            onlyShowRelevantControls = Toggle(onlyShowRelevantControls, "Show Relevant Controls");

            drawCount = 0;

            // control list
            for (int row = 0; row <= rowTotal; row++)
            {
                using (new HorizontalScope(Width(mainGuiWidth - marginOffset)))
                {
                    for (int i = rowStartindex; i < length; i++)
                    {
                        if (drawCount >= (row + 1) * maxItemsPerRow)
                            break;

                        rowStartindex++;
                        GameControlsEnum gameControl = inControlInputs[i, 0].getGameControl();
                        bool drawControl = false;
                        if (onlyShowRelevantControls)
                        {
                            if (mainGUIMenu == MainGUIMenuEnum.DrivingControls)
                            {
                                if (relevantDrivingControls.Contains(gameControl))
                                    drawControl = true;
                            }
                            else if (mainGUIMenu == MainGUIMenuEnum.FootControls)
                            {
                                if (relevantFootControls.Contains(gameControl))
                                    drawControl = true;
                            }
                        }
                        else
                            drawControl = true;

                        if (!drawControl)
                            continue;

                        drawCount++;

                        colorIndexCheck(ref colorIndex);

                        using (new VerticalScope("box", new GUILayoutOption[] { Width(itemWidth), Height(75) }))
                        {
                            Label(String.Format("<b>{0}:</b>", gameControl.getGameControlAlias(true)));

                            bool isControls = mainGUIMenu == MainGUIMenuEnum.FootControls;
                            PlayerModeEnum? playerMode;
                            if (isControls)
                                playerMode = PlayerModeEnum.OnFoot;
                            else
                            {
                                isControls = mainGUIMenu == MainGUIMenuEnum.DrivingControls;
                                if (isControls)
                                    playerMode = PlayerModeEnum.Driving;
                                else
                                    playerMode = null;
                            }
                            using (new HorizontalScope())
                            {
                                drawCommonControl(inControlInputs[i, 0], inControlInputs[i, 1], 1, playerMode);
                                drawCommonControl(inControlInputs[i, 0], inControlInputs[i, 2], 2, playerMode);
                            }
                        }
                    }
                    colorIndexCheck(ref colorIndex);
                    skin.box.normal.background = defaultBackgroundColor;
                }
            }
        }
        /// <summary>
        /// Draws a menu with the provided Enum, <typeparamref name="T"/>.
        /// </summary>
        private bool drawGeneralMenu<T>(T inSelected, out T inChangedTo) where T : Enum
        {
            // Written, 09.10.2018

            using (new HorizontalScope("box"))
            {
                foreach (T _enum in Enum.GetValues(typeof(T)))
                {
                    bool isSelected = _enum.Equals(inSelected);

                    if (isSelected)
                        skin.button.normal.background = selectedMenuButtonColor;
                    else
                        skin.button.normal.background = unselectedMenuButtonColor;

                    if (Button(_enum.ToString()) && !isSelected)
                    {
                        inChangedTo = _enum;
                        return true;
                    }
                }
            }
            skin.button.normal.background = defaultButtonColor;
            inChangedTo = inSelected;
            return false;
        }
        /// <summary>
        /// Draws force feedback overlay
        /// </summary>
        private void drawForceFeedBackOverlayGUI()
        {
            // Written, 16.10.2020

            Label($"FFB: {controlManager.getFfbSetOpt().ToString("0.###")}\nLast Rumble Sent: {xboxController.prevRumblePow.sqrMagnitude.ToString("0.###")}");
        }
        /// <summary>
        /// Draws drivetrain related info as a overlay.
        /// </summary>
        private void drawDrivetrainOverlay()
        {
            // Written, 18.10.2020

            Drivetrain dt = controlManager.drivetrain;
            Label(new StringBuilder().AppendFormat("drivetrain:\nRpm: {2}\nRange: {0}-{1}\nRev Limiter: {3} ({4})\nShift Triggered: {5} ({6})\nLongitudeSlip: {7}\nLateralSlip: {8}",
                dt.minRPM.ToString("0"),
                dt.maxRPM,
                dt.rpm.ToString("0"),
                dt.revLimiterTriggered,
                dt.revLimiterTime,
                dt.shiftTriggered,
                dt.shiftTime,
                dt.poweredWheels.Max(_wheel => _wheel.longitunalSlipVelo).ToString("0.###"),
                dt.poweredWheels.Max(_wheel => _wheel.lateralSlipVelo).ToString("0.###")).ToString());
        }
        private void drawFootControlContent(ref bool saveSettings)
        {
            // Written, 03.08.2022

            using (new VerticalScope(marginStyle))
            {
                using (new VerticalScope("box"))
                {
                    bool changed = drawToggle("Use player move as input", ref MoControlsSaveData.loadedSaveData.usePlayerMoveAsInput, ref saveSettings);

                    if (MoControlsSaveData.loadedSaveData.usePlayerMoveAsInput)
                    {
                        if (changed)
                        {
                            MoControlsSaveData.loadedSaveData.playerMove = InputTypeEnum.LS;
                        }
                        drawToggleGroup("Player Move", ref MoControlsSaveData.loadedSaveData.playerMove, ref saveSettings);
                    }
                    else if (changed)
                    {
                        MoControlsSaveData.loadedSaveData.playerMove = InputTypeEnum.None;
                    }
                }
                drawControlModeContent("Foot Controls", controlManager.footControls);
            }
        }
        private void drawDrivingControlContent()
        {
            // Written, 03.08.2022

            using (new VerticalScope(marginStyle))
            {
                drawControlModeContent("Driving Controls", controlManager.drivingControls);
            }
        }
        private void colorIndexCheck(ref int colorIndex)
        {
            // Written, 03.08.2022

            colorIndex++;
            if (colorIndex == 2)
            {
                colorIndex = 0;
                skin.box.normal.background = primaryItemColor;
            }
            else
            {
                skin.box.normal.background = moduleBackgroundColor;
            }
        }

        #endregion        
    }
}
