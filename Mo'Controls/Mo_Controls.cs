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
        /// <summary>
        /// Represents all functional inputs for the game.
        /// </summary>
        private readonly string[] inputNames = new string[]
        {
            "Throttle",
            "Brake",
            "Clutch",
            "Handbrake",
            "Left",
            "Right",
            "ShiftUp",
            "ShiftDown",
            "reverse",
            "first",
            "second",
            "third",
            "fourth",
            "fifth",
            "sixth",
            "IndicatorLeft",
            "IndicatorRight",
            "Range",
            "HighBeam",
            "Wipers",
            "Boost",
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
            "DrivingMode",
            "Smoking",
        };
        /// <summary>
        /// Represents currently assigned control inputs for the game.
        /// </summary>
        private string[,] controlInputs;
        /// <summary>
        /// Represents a keybind to open/close the gui for the mod.
        /// </summary>
        private readonly Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.F12);
        /// <summary>
        /// Represents whether the gui should be opened or closed.
        /// </summary>
        private bool controlsGuiOpened = false;
        /// <summary>
        /// Represents the scroll positions of the scroll bar for the gui.
        /// </summary>
        private Vector2 controlsGuiScrollPosition;
        /// <summary>
        /// Represents the scroll position of the scroll bar for the virtual xbox axis gui.
        /// </summary>
        private Vector2 virtualAxisGuiScrollPosition;
        /// <summary>
        /// Represents the width for the main gui.
        /// </summary>
        private readonly float controlsGuiWidth = 400f;
        private readonly float controlsMainHeight = 170f;
        /// <summary>
        /// Represents the width for the virtual axes gui.
        /// </summary>
        private readonly float virtualXboxAxesGuiWidth = 180f;
        /// <summary>
        /// Represents the width for the xbox controller debug gui.
        /// </summary>
        private readonly float xboxControllerDebugGuiWidth = 250f;
        /// <summary>
        /// Represents the change input result for the mod.
        /// </summary>
        private ChangeInputResult changeInputResult = new ChangeInputResult();
        /// <summary>
        /// Represents whether the mod should display debug info or not.
        /// </summary>
        private const bool DEBUG = true;
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
                        if (kcode != KeyCode.Mouse0) //Select key
                        {
                            if (kcode == KeyCode.Escape) // Set as none key
                            {
                                this.changeInput(KeyCode.None.ToString());
                            }
                            else
                            {
                                if (kcode != this.openControlsGui.Key && kcode != KeyCode.Mouse1) // not allowed
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

            if (this.changeInputResult.index == 1)
            {
                cInput.ChangeKey(this.changeInputResult.inputName, input, cInput.GetText(this.changeInputResult.inputName, 2));
            }
            else
            {
                cInput.ChangeKey(this.changeInputResult.inputName, cInput.GetText(this.changeInputResult.inputName, 1), input);
            }
            this.changeInputResult = new ChangeInputResult();
            this.loadControlInputsFromCInput();
        }
        /// <summary>
        /// Draws the main gui.
        /// </summary>
        private void drawControlsGui()
        {
            // Written, 10.07.2018

            GUILayoutOption[] layoutOption = new GUILayoutOption[1] 
            {
                GUILayout.Width(this.controlsGuiWidth - 25)
            };
            GUILayout.BeginArea(new Rect(20, 20, this.controlsGuiWidth, this.controlsMainHeight));
            GUILayout.BeginVertical("box");
            GUILayout.Label(String.Format("{0} v{1} by {2}", this.Name, this.Version, this.Author));
            GUILayout.Label(String.Format("{0} is a forbidden key (GUI key bind).\r\nESC Sets as None.\r\nLMB Selects.\r\nRMB Cancels.", this.openControlsGui.Key));
            int numJoysticks = Input.GetJoystickNames().Count();
            if (numJoysticks > 0)
            {
                GUILayout.BeginVertical("box", layoutOption);
                GUILayout.Label(String.Format("Detected Joysticks: {0}", numJoysticks));
                foreach (string joystick in Input.GetJoystickNames())
                {
                    GUILayout.Label(String.Format("{0}", joystick));
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
            // Game Controls
            GUILayout.BeginArea(new Rect(20, this.controlsMainHeight + 10, this.controlsGuiWidth, Screen.height - this.controlsMainHeight - 40));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(this.controlsGuiWidth) });
            this.controlsGuiScrollPosition = GUILayout.BeginScrollView(this.controlsGuiScrollPosition, false, true, new GUILayoutOption[1] { GUILayout.Width(this.controlsGuiWidth) });
            GUILayout.Label(String.Format("Game Controls ({0})", this.controlInputs.GetLength(0)));
            for (int i = 0; i < this.controlInputs.GetLength(0); i++)
            {
                string controlName = this.controlInputs[i, 0];
                string primaryInput = this.controlInputs[i, 1];
                string secondaryInput = this.controlInputs[i, 2];
                XboxControl xboxControl;

                GUILayout.BeginVertical("box", layoutOption);
                GUILayout.Label(String.Format("{0}: {1}", controlName, this.changeInputResult.inputName == controlName ? "Awaiting key input" : ""));
                GUILayout.Label("Primary input:");
                xboxControl = this.xboxController.getXboxControlByInputName(primaryInput);
                if (xboxControl != null)
                {
                    if (GUILayout.Button(xboxControl.texture))
                    {
                        this.changeToPollingState(controlName, 1);
                    }
                }
                else
                {
                    if (GUILayout.Button(primaryInput))
                    {
                        this.changeToPollingState(controlName, 1);
                    }                    
                }
                GUILayout.Label("Secondary input:");
                xboxControl = this.xboxController.getXboxControlByInputName(secondaryInput);
                if (xboxControl != null)
                {
                    if (GUILayout.Button(xboxControl.texture))
                    {
                        this.changeToPollingState(controlName, 2);
                    }
                }
                else
                {
                    if (GUILayout.Button(secondaryInput))
                    {
                        this.changeToPollingState(controlName, 2);
                    }
                }
                GUILayout.EndVertical();
            }
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

            GUILayout.BeginArea(new Rect(this.controlsGuiWidth + 40, 20, this.virtualXboxAxesGuiWidth, 600));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(this.virtualXboxAxesGuiWidth) });
            GUILayout.Label("Virtual xbox controller axes");
            this.virtualAxisGuiScrollPosition = GUILayout.BeginScrollView(this.virtualAxisGuiScrollPosition, false, true, new GUILayoutOption[1] { GUILayout.Width(this.virtualXboxAxesGuiWidth) });
            foreach (XboxControl xboxControl in this.xboxController.getXboxControls())
            {
                if (xboxControl.controlType == XboxControlTypeEnum.Axis)
                {
                    GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.virtualXboxAxesGuiWidth - 35) });
                    if (GUILayout.Button(xboxControl.texture, new GUILayoutOption[1] { GUILayout.Width(this.virtualXboxAxesGuiWidth - 35) }))
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

            float left = this.changeInputResult.reassignKey ? (this.controlsGuiWidth + this.virtualXboxAxesGuiWidth + 60) : (this.controlsGuiWidth + 40);
            GUILayout.BeginArea(new Rect(left, 20, this.xboxControllerDebugGuiWidth, 650));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth) });   
            GUILayout.Label("Xbox Controller Debug");
            GUILayout.Space(5f);
            if (this.xboxController.isConnected)
            {
                // Triggers
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth - 10) });
                GUILayout.Label("Triggers:");
                GUILayout.Label(String.Format("Left: {0}\r\nRight: {1}", Math.Round(this.xboxController.getLeftTrigger(), 2), Math.Round(this.xboxController.getRightTrigger(), 2)));
                GUILayout.EndVertical();
                // Bumpers
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth - 10) });
                GUILayout.Label("Bumpers:");
                GUILayout.Label(String.Format("Left: {0}\r\nRight: {1}", this.xboxController.LB.state, this.xboxController.RB.state));
                GUILayout.EndVertical();
                // Left Thumbsick
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth - 10) });
                GUILayout.Label("Left Thumbstick:");
                GUILayout.Label(String.Format("X: {0}\r\nY: {1}\r\nLS: {2}", Math.Round(this.xboxController.getLeftStick().X, 2), Math.Round(this.xboxController.getLeftStick().Y, 2), this.xboxController.LS.state));
                GUILayout.EndVertical();
                // Right Thumbstick
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth - 10) });
                GUILayout.Label("Right Thumbstick:");
                GUILayout.Label(String.Format("X: {0}\r\nY: {1}\r\nRS: {2}", Math.Round(this.xboxController.getRightStick().X, 2), Math.Round(this.xboxController.getRightStick().Y, 2), this.xboxController.RS.state));
                GUILayout.EndVertical();
                // Buttons
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth - 10) });
                GUILayout.Label("Buttons");
                GUILayout.Label(String.Format("A: {0}\r\nB: {1}\r\nX: {2}\r\nY: {3}\r\nStart: {4}\r\nBack: {5}",
                    this.xboxController.A.state, this.xboxController.B.state, this.xboxController.X.state, this.xboxController.Y.state, this.xboxController.Start.state, this.xboxController.Back.state));
                GUILayout.EndVertical();
                // D-Pad
                GUILayout.Space(5f);
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.xboxControllerDebugGuiWidth - 10) });
                GUILayout.Label("D-Pad");
                GUILayout.Label(
                    String.Format("Up: {0}\r\nDown: {1}\r\nLeft: {2}\r\nRight: {3}", this.xboxController.DPadUp.state, this.xboxController.DPadDown.state, this.xboxController.DPadLeft.state, this.xboxController.DPadRight.state));
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("Xbox Controller not connected/detected.");
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        /// <summary>
        /// Changes the <see cref="changeInputResult"/> to it's "polling" state, with the values provided, <paramref name="inControlName"/>, + <paramref name="inIndex"/>. Which inturn lets <see cref="Update"/> branch to <see cref="monitorForInput"/>.
        /// </summary>
        /// <param name="inControlName">The game control to change.</param>
        /// <param name="inIndex">The index to change, Primary = 1, Secondary = 2.</param>
        private void changeToPollingState(string inControlName, int inIndex)
        {
            // Written, 20.07.2018

            if (!this.changeInputResult.reassignKey)
            {
                this.changeInputResult.reassignKey = true;
                this.changeInputResult.inputName = inControlName;
                this.changeInputResult.index = inIndex;
            }
        }

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Written, 06.07.2018    

            Keybind.Add(this, this.openControlsGui);
            this.controlInputs = new string[this.inputNames.Length, 3];
            this.loadControlInputsFromCInput();
            cInput.OnKeyChanged += this.CInput_OnKeyChanged;
            this.xboxController = new xController(1);
            this.xboxControllerManager = new XboxControllerManager(1, this.xboxController);
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
                    this.drawVirtualXboxControllerAxesGui();
                if (DEBUG)
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
        /// Is called when cinput keys are changed externally, (the game gui controls).
        /// </summary>
        private void CInput_OnKeyChanged()
        {
            // Written, 09.07.2018

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
