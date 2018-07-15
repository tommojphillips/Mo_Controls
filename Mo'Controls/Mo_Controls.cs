using System;
using System.Linq;
using MSCLoader;
using UnityEngine;

namespace Mo_Controls
{
    public class Mo_Controls : Mod
    {
        // Written, 06.07.2018

        #region Mod Fields

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Author => "tommojphillips";
        public override string Version => "1.0.2";
        public override bool UseAssetsFolder => false;

        #endregion

        #region Fields

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
        /// Represents all xbox one controller axes.
        /// </summary>
        private readonly string[,] xboxControllerAxes = new string[,]
        {
            // D-Pad
            { "D-Pad Left", Keys.XboxDPadLeft },
            { "D-Pad Right", Keys.XboxDPadRight },
            { "D-Pad Up", Keys.XboxDPadUp },
            { "D-Pad Down", Keys.XboxDPadDown },
            // Left Stick
            { "Left-Stick Left", Keys.Xbox1LStickLeft },
            { "Left-Stick Right", Keys.Xbox1LStickRight },
            { "Left-Stick Up", Keys.Xbox1LStickUp },
            { "Left-Stick Down", Keys.Xbox1LStickDown },
            // Right Stick
            { "Right-Stick Left", Keys.Xbox1RStickLeft},
            { "Right-Stick Right", Keys.Xbox1RStickRight },
            { "Right-Stick Up", Keys.Xbox1RStickUp },
            { "Right-Stick Down", Keys.Xbox1RStickDown },
            // Triggers
            { "Trigger Left", Keys.Xbox1TriggerLeft },
            { "Trigger Right", Keys.Xbox1TriggerRight },
        };
        /// <summary>
        /// Represents all xbox one controller buttons.
        /// </summary>
        private readonly string[,] xboxControllerButtons = new string[,]
        {
            { "Xbox A", Keys.XboxA },
            { "Xbox B", Keys.XboxB },
            { "Xbox X", Keys.XboxX },
            { "Xbox Y", Keys.XboxY },
            { "Xbox LB", Keys.Xbox1BumperLeft },
            { "Xbox RB", Keys.Xbox1BumperRight },
            { "Xbox Back", Keys.XboxBack },
            { "Xbox Start", Keys.XboxStart },
            { "Xbox LS", Keys.XboxLStickButton },
            { "Xbox RS", Keys.XboxRStickButton },
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
        /// Represents the width for the main gui.
        /// </summary>
        private readonly float controlsGuiWidth = 600f;
        /// <summary>
        /// Represents the width for the virtual axes gui.
        /// </summary>
        private readonly float virtualXboxAxesGuiWidth = 300f;
        /// <summary>
        /// Represents the change input result for the mod.
        /// </summary>
        private ChangeInputResult changeInputResult = new ChangeInputResult();

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
        /// Changes the input for a control defined in <see cref="changeInputResult"/>.
        /// </summary>
        private void changeInput()
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
                            }
                            return;
                        }
                        else
                            return;
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

            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(20, 20, this.controlsGuiWidth, Screen.height - 40));
            GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(this.controlsGuiWidth) });
            this.controlsGuiScrollPosition = GUILayout.BeginScrollView(controlsGuiScrollPosition, false, true, new GUILayoutOption[1] { GUILayout.Width(this.controlsGuiWidth) });
            GUILayout.Label(String.Format("{0} v{1} by {2}", this.Name, this.Version, this.Author));
            GUILayout.Label(String.Format("RMB & {0} are forbidden.\r\nESC cancels.\r\nLMB Selects.", this.openControlsGui.Key));
            int numJoysticks = Input.GetJoystickNames().Count();
            if (numJoysticks > 0)
            {

                GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(this.controlsGuiWidth - 10) });
                GUILayout.Label(String.Format("Detected Joysticks: {0}", numJoysticks));
                foreach (string joystick in Input.GetJoystickNames())
                {
                    GUILayout.Label(String.Format("{0}", joystick));
                }
                GUILayout.EndVertical();
            }
            GUILayout.Label(String.Format("Game Controls ({0})", this.controlInputs.GetLength(0)));
            for (int input = 0; input < this.controlInputs.GetLength(0); input++)
            {
                string controlName = this.controlInputs[input, 0];
                string primaryInput = this.checkIfInputIsXboxInput(this.controlInputs[input, 1]);
                string secondaryInput = this.checkIfInputIsXboxInput(this.controlInputs[input, 2]);

                GUILayout.BeginVertical("box", new GUILayoutOption[1] { GUILayout.Width(this.controlsGuiWidth - 10) });
                GUILayout.Label(String.Format("{0}: {1}", controlName, this.changeInputResult.inputName == controlName ? "Awaiting key input" : ""));
                GUILayout.Label("Primary input:");
                if (GUILayout.Button(primaryInput))
                {
                    if (!this.changeInputResult.reassignKey)
                    {
                        this.changeInputResult.reassignKey = true;
                        this.changeInputResult.inputName = controlName;
                        this.changeInputResult.index = 1;
                    }
                }
                GUILayout.Label("Secondary input:");
                if (GUILayout.Button(secondaryInput))
                {
                    if (!this.changeInputResult.reassignKey)
                    {
                        this.changeInputResult.reassignKey = true;
                        this.changeInputResult.inputName = controlName;
                        this.changeInputResult.index = 2;
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
            for (int i = 0; i < this.xboxControllerAxes.GetLength(0); i++)
            {
                string axisName = this.xboxControllerAxes[i, 0];
                string axisInput = this.xboxControllerAxes[i, 1];
                GUILayout.BeginVertical("Box", new GUILayoutOption[1] { GUILayout.Width(this.virtualXboxAxesGuiWidth / 2) });
                if (GUILayout.Button(axisName, new GUILayoutOption[1] { GUILayout.Width(this.virtualXboxAxesGuiWidth / 2) }))
                {
                    if (this.changeInputResult.reassignKey)
                    {
                        this.changeInput(axisInput);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        /// <summary>
        /// Checks if the provided input is an xbox controller input then returns a more user-friendly name for it eg, "Joy1 Axis 1-" would be "Left-Stick Left".
        /// </summary>
        /// <param name="input">The input to check.</param>
        private string checkIfInputIsXboxInput(string input)
        {
            // Written, 10.07.2018

            for (int i = 0; i < this.xboxControllerAxes.GetLength(0); i++)
            {
                if (xboxControllerAxes[i, 1] == input)
                {
                    return xboxControllerAxes[i, 0];
                }
            }
            for (int i = 0; i < this.xboxControllerButtons.GetLength(0); i++)
            {
                if (xboxControllerButtons[i, 1] == input)
                {
                    return xboxControllerButtons[i, 0];
                }
            }
            return input;
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
            ModConsole.Print(String.Format("{0} v{1}: Loaded", this.Name, this.Version));
        }
        public override void OnGUI()
        {
            // Written, 10.07.2018

            if (this.controlsGuiOpened)
            {
                try
                {                    
                    this.drawControlsGui();
                    if (this.changeInputResult.reassignKey)
                        this.drawVirtualXboxControllerAxesGui();
                }
                catch (Exception ex)
                {
                    ModConsole.Print(ex);
                }
            }
        }
        public override void Update()
        {
            // Update is called once per frame

            if (this.openControlsGui.IsDown())
            {
                this.controlsGuiOpened = !this.controlsGuiOpened;
            }
            if (this.changeInputResult.reassignKey)
            {
                this.changeInput();
            }
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

        #endregion
    }
}   
