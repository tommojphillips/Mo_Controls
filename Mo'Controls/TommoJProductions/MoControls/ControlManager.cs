using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Linq;
using TommoJProductions.Debugging;
using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents the control manager for mo Controls.
    /// </summary>
    public class ControlManager : MonoBehaviour
    {
        // Written, 22.08.2018

        #region Fields

        /// <summary>
        /// Represents the handmode gameobject 
        /// </summary>
        private const string handModeLocation = "PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble";
        /// <summary>
        /// Represents the tool mode gameobject
        /// </summary>
        private const string toolModeLocation = "PLAYER/Pivot/AnimPivot/Camera/FPSCamera/2Spanner";
        /// <summary>
        /// Represents logic to enable scroll to a connected xbox controllers' triggers. enabled when player is in tool mode.
        /// </summary>
        private GuiNav toolModeScroll;
        /// <summary>
        /// Represents the current car dynamics. used for forcefeedback.
        /// </summary>
        internal CarDynamics carDynamics;
        /// <summary>
        /// Reprsents the current player mode.
        /// </summary>
        private PlayerModeEnum? currentPlayerMode;
        /// <summary>
        /// Represents the drivetrain gamobject. used for ffb.
        /// </summary>
        internal Drivetrain drivetrain;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the satsuma gameobject. used for forcefeedback.
        /// </summary>
        internal GameObject vehicle { get; private set; }
        /// <summary>
        /// gets or sets the currently selected controls
        /// </summary>
        private string[,] currentControls
        {
            get
            {
                return currentPlayerMode == PlayerModeEnum.Driving ? this.drivingControls : currentPlayerMode == PlayerModeEnum.OnFoot ? this.footControls : this.blankControls;
            }
            set
            {
                if (this.currentPlayerMode == PlayerModeEnum.Driving)
                    this.drivingControls = value;
                else if (this.currentPlayerMode == PlayerModeEnum.OnFoot)
                    this.footControls = value;
            }
        }
        /// <summary>        
        /// Gets all functional game-inputs for the game (MSC).        
        /// </summary>
        private static string[] inputNames
        {
            get
            {
                return new string[]
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
                    "Watch",
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
            }
        }
        /// <summary>
        /// Represents the current player mode. either in menu, on foot, or driving.
        /// </summary>
        internal static PlayerModeEnum getCurrentPlayerMode
        {
            get
            {
                if (MoControlsGO.moControlsGui.controlsGuiOpened)
                    return PlayerModeEnum.InMenu;
                PlayerModeEnum pme;
                switch (getCurrentVehicle)
                {
                    case "":
                        pme = PlayerModeEnum.OnFoot;
                        break;
                    default:
                        pme = PlayerModeEnum.Driving;
                        break;
                }
                return pme;
            }
        }
        /// <summary>
        /// Represents the current vehicle name (global playmakerFSM)
        /// </summary>
        internal static string currentVehicleName 
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the current vehicle the player is in.
        /// </summary>
        internal static string getCurrentVehicle
        {
            get { return FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value; }
        }
        /// <summary>
        /// Represents the change input result for the mod.
        /// </summary>
        internal ChangeInput changeInputResult
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the current foot controls.
        /// </summary>
        internal string[,] footControls
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the current driving controls.
        /// </summary>
        internal string[,] drivingControls
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents blank controls.
        /// </summary>
        private string[,] blankControls
        {
            get
            {
                return new string[,]
                {
                       { "Left", "None", "None", },
                       { "Right", "None", "None", },
                       { "Throttle", "None", "None", },
                       { "Brake", "None", "None", },
                       { "Clutch", "None", "None", },
                       { "ShiftUp", "None", "None", },
                       { "ShiftDown", "None", "None", },
                       { "IndicatorLeft", "None", "None", },
                       { "IndicatorRight", "None", "None", },
                       { "Range", "None", "None", },
                       { "HighBeam", "None", "None", },
                       { "Wipers", "None", "None", },
                       { "Boost", "None", "None", },
                       { "Handbrake", "None", "None", },
                       { "DrivingMode", "Return", "None", },
                       { "PlayerLeft", "None", "None", },
                       { "PlayerRight", "None", "None", },
                       { "PlayerUp", "None", "None", },
                       { "PlayerDown", "None", "None", },
                       { "Jump", "None", "None", },
                       { "Run", "None", "None", },
                       { "Zoom", "None", "None", },
                       { "Use", "None", "None", },
                       { "Crouch", "None", "None", },
                       { "Watch", "None", "None"},
                       { "ReachLeft", "None", "None", },
                       { "ReachRight", "None", "None", },
                       { "Hitchhike", "None", "None", },
                       { "Swear", "None", "None", },
                       { "Hit", "None", "None", },
                       { "Push", "None", "None", },
                       { "Finger", "None", "None", },
                       { "Urinate", "None", "None", },
                       { "Drunk", "None", "None", },
                       { "Smoking", "None", "None", },
                       { "reverse", "None", "None", },
                       { "first", "None", "None", },
                       { "second", "None", "None", },
                       { "third", "None", "None", },
                       { "fourth", "None", "None", },
                       { "fifth", "None", "None", },
                       { "sixth", "None", "None", },
                };
            }
        }
        /// <summary>
        /// Represents the hand mode gameobject.
        /// </summary>
        private static GameObject handModeGameObject
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the tool mode gameobject.
        /// </summary>
        private static GameObject toolModeGameObject
        {
            get;
            set;
        }
        /// <summary>
        /// Returns true if player is in tool mode.
        /// </summary>
        internal static bool isInToolMode
        {
            get
            {
                return toolModeGameObject.activeSelf && !handModeGameObject.activeSelf;
            }
        }
        /// <summary>
        /// Represents the last rumble pow.
        /// </summary>
        internal Vector2 lastRumblePow
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the last rumble pow sent to the controller.
        /// </summary>
        internal Vector2 lastRumbePowSent
        {
            get;
            private set;
        }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ControlManager()
        {
            // Written, 22.08.2018

            this.currentPlayerMode = null;
            this.setChangeInput();
            cInput.OnKeyChanged -= this.cInput_OnKeyChanged;
            cInput.OnKeyChanged += this.cInput_OnKeyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            MoControlsGO.controlManager.setControls(MoControlsSaveData.loadedSaveData.footControls, MoControlsSaveData.loadedSaveData.drivingControls);
            // Setting up toolmode stuff
            toolModeGameObject = GameObject.Find(toolModeLocation);
            handModeGameObject = GameObject.Find(handModeLocation);
            // Tool mode hold button set up
            HoldInputMono him = this.gameObject.AddComponent<HoldInputMono>();
            him.setData("Toggle Tool Mode", XboxButtonEnum.Start, 0.3f, this.requestedModeChange);
            // Setting up guiNav "toolMode MouseScroll".
            this.toolModeScroll = this.gameObject.AddComponent<GuiNav>();
            this.toolModeScroll.setControls(XboxAxisEnum.rightTrigger, XboxButtonEnum.NULL,
                XboxAxisEnum.leftTrigger, XboxButtonEnum.NULL,
                XboxAxisEnum.NULL, XboxButtonEnum.RB,
                XboxAxisEnum.NULL, XboxButtonEnum.LB);
            MoControlsMod.print(nameof(ControlManager) + ": Started", DebugTypeEnum.full);
        }
        /// <summary>
        /// Occurs on update.
        /// </summary>
        private void Update()
        {
            // Written, 31.08.2018

            if (this.currentPlayerMode != getCurrentPlayerMode)
            {
                this.currentPlayerMode = getCurrentPlayerMode;
                this.loadControlModeToCInput(this.currentPlayerMode, this.currentControls);
                MoControlsMod.print("Control Mode changed: " + this.currentPlayerMode, DebugTypeEnum.full);
            }
            // Enable scroll only if player is on foot and in tool mode (2) OR when player is holding an item while on foot and in hand mode.
            this.toolModeScroll.enabled = this.currentPlayerMode == PlayerModeEnum.OnFoot && (isInToolMode || (!isInToolMode && !isPlayerHandEmpty()));
            // Handling xbox controller force feedback rumble events.
            if (this.setFfbForVehicle())
                this.handleFfbOnXboxController();
        }
        /// <summary>
        /// Loads provided control list to cInput.
        /// </summary>
        /// <param name="inControlMode">The control mode.</param>
        private void loadControlModeToCInput(PlayerModeEnum? inPlayerMode, string[,] inControlMode)
        {
            // Written, 31.08.2018

            string controlListName = "game control";
            try
            {
                controlListName = inPlayerMode.Equals(PlayerModeEnum.OnFoot) ? "foot control" : "driving control";
                for (int i = 0; i < inControlMode.GetLength(0); i++)
                {
                    cInput.ChangeKey(inControlMode[i, 0], inControlMode[i, 1], inControlMode[i, 2]);
                }
                MoControlsMod.print(String.Format("<b><color=green>Successfully</color></b> loaded {0} inputs to cInput.", controlListName), DebugTypeEnum.full);
            }
            catch (NullReferenceException)
            {
                MoControlsMod.print(String.Format("control inputs was null; setting {0} inputs to current control settings.", controlListName), DebugTypeEnum.full);
                if (inPlayerMode == PlayerModeEnum.Driving)
                    this.drivingControls = loadControlInputsFromCInput();
                else
                    this.footControls = loadControlInputsFromCInput();
            }
            catch
            {
                MoControlsMod.print(String.Format("<b><color=red>Unsuccessfully</color></b> loaded {0} inputs to cInput.", controlListName), DebugTypeEnum.full);
                throw;
            }
        }
        /// <summary>
        /// Occurs when cinput keys are changed externally, (the game gui controls).
        /// </summary>
        private void cInput_OnKeyChanged()
        {
            // Written, 09.07.2018

            this.currentControls = loadControlInputsFromCInput();
        }
        /// <summary>
        /// Loads control inputs (defined in <see cref="inputNames"/>) from the class, <see cref="cInput"/> and adds each one to <see cref="controlInputs"/> with it's primary
        /// and secondary input.
        /// </summary>
        internal static string[,] loadControlInputsFromCInput()
        {
            // Written, 31.08.2018

            try
            {
                string[,] controls = new string[inputNames.Length, 3];
                for (int i = 0; i < inputNames.Length; i++)
                {
                    controls[i, 0] = cInput.GetText(inputNames[i], 0);
                    controls[i, 1] = cInput.GetText(inputNames[i], 1);
                    controls[i, 2] = cInput.GetText(inputNames[i], 2);
                }
                MoControlsMod.print("<b><color=green>Successfully</color></b> loaded game control inputs from cInput.", DebugTypeEnum.full);
                return controls;
            }
            catch (Exception ex)
            {
                MoControlsMod.print("<b><color=red>Unsuccessfully</color></b> loaded game control inputs from cInput.", DebugTypeEnum.full);
                ModConsole.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Assigns the control groups.
        /// </summary>
        /// <param name="inFootControls">The group of foot controls to assign.</param>
        /// <param name="inDrivingControls">The group of driving controls to assign.</param>
        internal void setControls(string[,] inFootControls, string[,] inDrivingControls)
        {
            // Written, 22.08.2018

            this.footControls = inFootControls;
            this.drivingControls = inDrivingControls;
        }
        /// <summary>
        /// Sets the provided game control in provided mode.
        /// </summary>
        /// <param name="inMode"></param>
        private void setGameControl(PlayerModeEnum inMode, string inControlName, int inIndex, string inInput)
        {
            // Written, 02.09.2018

            if (inIndex != 1 && inIndex != 2)
            {
                MoControlsMod.print("<b>C285 PControlManager</b>\r\nIndex out of range for game control editing...", DebugTypeEnum.full);
                throw new IndexOutOfRangeException();
            }
            switch (inMode)
            {
                case PlayerModeEnum.Driving:
                    for (int i = 0; i < this.drivingControls.GetLength(0); i++)
                    {
                        string _controlName = this.drivingControls[i, 0];
                        if (inControlName == _controlName)
                        {
                            this.drivingControls[i, inIndex] = inInput;
                            break;
                        }
                    }
                    if (this.currentPlayerMode == inMode)
                        this.loadControlModeToCInput(inMode, this.drivingControls);
                    break;
                case PlayerModeEnum.OnFoot:
                    for (int i = 0; i < this.footControls.GetLength(0); i++)
                    {
                        string _controlName = this.footControls[i, 0];
                        if (inControlName == _controlName)
                        {
                            this.footControls[i, inIndex] = inInput;
                            break;
                        }
                    }
                    if (this.currentPlayerMode == inMode)
                        this.loadControlModeToCInput(inMode, this.footControls);
                    break;
            }
        }
        /// <summary>
        /// Changes the input for a control defined in <see cref="changeInputResult"/> to the provided input string, <paramref name="inInput"/>.
        /// </summary>
        /// <param name="inInput">The input to assign.</param>
        internal void changeInput(string inInput)
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
                            cInput.ChangeKey(this.changeInputResult.controlName, inInput, cInput.GetText(this.changeInputResult.controlName, 2));
                        else
                            cInput.ChangeKey(this.changeInputResult.controlName, cInput.GetText(this.changeInputResult.controlName, 1), inInput);
                        this.currentControls = loadControlInputsFromCInput();
                    }
                    MoControlsMod.print("Player mode wasa null while attempting to change input..", DebugTypeEnum.full);
                }
                else
                {
                    this.setGameControl((PlayerModeEnum)playerMode, this.changeInputResult.controlName, this.changeInputResult.index, inInput);
                    MoControlsMod.print("Player mode was equal to <b>" + this.changeInputResult.mode + "</b> whiling setting '" + this.changeInputResult.controlName + "' to '" + inInput + "'.", DebugTypeEnum.full);
                    MoControlsSaveData.loadedSaveData.saveSettings();
                }
            }
            else
            {
                // Treat as a mod keybind.

                Keybind modKeybind = Keybind.Get(this.changeInputResult.mod).Where(kb => kb.ID == this.changeInputResult.controlName).First();
                if (this.changeInputResult.index == 1)
                {
                    modKeybind.Modifier = (KeyCode)Enum.Parse(typeof(KeyCode), inInput);
                }
                else
                {
                    modKeybind.Key = (KeyCode)Enum.Parse(typeof(KeyCode), inInput);
                }
                ModSettings_menu.SaveModBinds(this.changeInputResult.mod);
                MoControlsMod.print("saved <i>" + modKeybind.Mod.Name + "</i> mod keybinds.", DebugTypeEnum.full);
            }
            this.setChangeInput();
        }
        /// <summary>
        /// Toggles toolmode eg. tool=>hand | hand=>tool
        /// </summary>
        internal void toggleToolMode()
        {
            // Written, 08.10.2020

            if (isPlayerHandEmpty())
            {
                VirtualKeyShort? wVk = null;

                if (isInToolMode)
                    wVk = VirtualKeyShort.KEY_1;
                else if (isPlayerHandEmpty())
                    wVk = VirtualKeyShort.KEY_2;
                StartCoroutine(KeyboardEmulator.simulateKeyPressCoroutine((VirtualKeyShort)wVk));
            }
        }
        /// <summary>
        /// changes to request mode.
        /// </summary>
        private void requestedModeChange()
        {
            // Written, 07.10.2020

           MoControlsGO.xboxController._requestedModeChange = true;
        }
        /// <summary>
        /// Returns whether the players hand is empty (not holding anything).
        /// </summary>
        /// <returns></returns>
        private static bool isPlayerHandEmpty()
        {
            // Written, 06.10.2020

            GameObject gm = GameObject.Find(handModeLocation + "/Hand");
            PlayMakerFSM pm = gm.GetComponents<PlayMakerFSM>().First(_pm => _pm.FsmName == "PickUp");
            return pm.FsmVariables.FindFsmBool("HandEmpty").Value;
        }
        /// <summary>
        /// Handles ffb on the xbox controller.
        /// </summary>
        private void handleFfbOnXboxController()
        {
            // Written, 16.10.2020

            if (MoControlsSaveData.loadedSaveData.ffbOnXboxController && this.vehicle != null)
            {
                Vector2 rumblePow = this.getFfbSetOpt();
                MoControlsGO.xboxController.setRumble(rumblePow);
            }
        }
        /// <summary>
        /// Gets set options for ffb and returns the result as a vector2.
        /// </summary>
        internal Vector2 getFfbSetOpt()
        {
            // Written, 18.10.2020

            float xMotor = 0;
            float yMotor = 0;
            if (MoControlsSaveData.loadedSaveData.ffbOption_default)
            {
                return this.floatToVector(this.defaultFfb());
            }
            else
            {
                if (MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip)
                {
                    float wheelSlip = Mathf.Clamp(this.wheelSlipBasedFfb(), -1, 1);
                    if (wheelSlip > 0)
                        xMotor = wheelSlip;
                    else
                        yMotor = Mathf.Abs(wheelSlip);
                } if (MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter)
                    xMotor += this.rpmLimiterBasedFfb();
                if (MoControlsSaveData.loadedSaveData.ffbOption_gearChange)
                    yMotor += this.gearChangeBasedFfb();
            }
            return new Vector2(Mathf.Clamp(xMotor, 0, 1), Mathf.Clamp(yMotor, 0, 1));
        }
        private float gearChangeBasedFfb()
        {
            // Written, 23.10.2020

            if (this.drivetrain.changingGear)
                return this.drivetrain.rpm / this.drivetrain.maxRPM;
            return 0;
        }
        private float rpmLimiterBasedFfb()
        {
            // Written, 18.10.2020

            int engageAt = 1500;
            float _rpm = this.drivetrain.rpm;
            if (this.drivetrain.revLimiterTriggered || _rpm > (int)this.drivetrain.maxRPM - engageAt) // > 6500 RPM => ffb)
                return _rpm / this.drivetrain.maxRPM;
            return 0;
        }
        private float wheelSlipBasedFfb()
        {
            // Written, 18.10.2020

            return this.drivetrain.poweredWheels.Max(_wheel => _wheel.longitudinalSlip); // based on car roll, facing forward
        }
        /// <summary>
        /// Represents the default ffb (designed for a wheel) doesnt work properly
        /// </summary>
        private float defaultFfb()
        {
            // Written, 18.10.2020

            return this.carDynamics.forceFeedback;
        }
        /// <summary>
        /// Converts ffb float value to a vector 2 for xbox rumble events.
        /// </summary>
        /// <param name="inValue">The force feed back value.</param>
        private Vector2 floatToVector(float inValue)
        {
            // Written, 16.10.2020

            if (inValue > 0)
                return new Vector2(Mathf.Abs(inValue), 0);
            else if (inValue < 0)
                return new Vector2(0, Mathf.Abs(inValue));
            else
                return Vector2.zero;
        }
        /// <summary>
        /// Sets the change input result to the provided parameter. if <paramref name="inChangeInput"/> is null (no parameter), sets changeInputResult to a new instance.
        /// </summary>
        /// <param name="inChangeInput">The change input instance to set.</param>
        internal void setChangeInput(ChangeInput inChangeInput = null)
        {
            // Written, 17.10.2020

            this.changeInputResult = inChangeInput ?? new ChangeInput();
        }
        /// <summary>
        /// Sets all ffb to the current vehicle.
        /// </summary>
        private bool setFfbForVehicle() 
        {
            // Written, 23.10.2020

            string _currentVehicleName = getCurrentVehicle;
            bool isSet = (_currentVehicleName ?? "") == currentVehicleName;
            if (!isSet)
            {
                switch (_currentVehicleName)
                {
                    case "Satsuma":
                        this.vehicle = GameObject.Find("SATSUMA(557kg, 248)");
                        this.carDynamics = this.vehicle.GetComponent<CarDynamics>();
                        this.drivetrain = this.vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Jonnez":
                        this.vehicle = GameObject.Find("JONNEZ ES(Clone)");
                        this.carDynamics = this.vehicle.GetComponent<CarDynamics>();
                        this.drivetrain = this.vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Kekmet":
                        this.vehicle = GameObject.Find("KEKMET(350-400psi)");
                        this.carDynamics = this.vehicle.GetComponent<CarDynamics>();
                        this.drivetrain = this.vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Hayosiko":
                        this.vehicle = GameObject.Find("HAYOSIKO(1500kg, 250)");
                        this.carDynamics = this.vehicle.GetComponent<CarDynamics>();
                        this.drivetrain = this.vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Gifu":
                        this.vehicle = GameObject.Find("GIFU(750/450psi)");
                        this.carDynamics = this.vehicle.GetComponent<CarDynamics>();
                        this.drivetrain = this.vehicle.GetComponent<Drivetrain>();
                        break;
                    default:
                        this.vehicle = null;
                        break;
                }
                currentVehicleName = _currentVehicleName;
            }
            return this.vehicle != null;
        }

        #endregion
    }
}
