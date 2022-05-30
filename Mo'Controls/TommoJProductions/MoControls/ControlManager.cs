using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Collections;
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
        /// Represents the drivetrain gamobject. used for ffb.
        /// </summary>
        internal Drivetrain drivetrain;
        /// <summary>
        /// Reprsents the current player mode.
        /// </summary>
        private PlayerModeEnum _cpm;
        #endregion

        #region Properties

        /// <summary>
        /// Reprsents the current player mode.
        /// </summary>
        private PlayerModeEnum currentPlayerMode 
        {
            get
            {
                if (_cpm != getCurrentPlayerMode)
                {
                    currentPlayerMode = getCurrentPlayerMode;
                }
                return _cpm;
            }

            set
            {
                _cpm = value;
                loadControlModeToCInput(_cpm, currentControls);
                MoControlsMod.print("Control Mode changed: " + _cpm, DebugTypeEnum.full);
            }
        }
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
                return currentPlayerMode == PlayerModeEnum.Driving ? drivingControls : currentPlayerMode == PlayerModeEnum.OnFoot ? footControls : blankControls;
            }
            set
            {
                if (currentPlayerMode == PlayerModeEnum.Driving)
                    drivingControls = value;
                else if (currentPlayerMode == PlayerModeEnum.OnFoot)
                    footControls = value;
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

        private FsmBool handEmpty;

        #endregion

        #region Constuctors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ControlManager()
        {
            // Written, 22.08.2018

            setChangeInput();
            cInput.OnKeyChanged -= cInput_OnKeyChanged;
            cInput.OnKeyChanged += cInput_OnKeyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018


            GameObject gm = GameObject.Find(handModeLocation + "/Hand");
            handEmpty = gm.GetPlayMaker("PickUp").FsmVariables.FindFsmBool("HandEmpty").Value;

            MoControlsGO.controlManager.setControls(MoControlsSaveData.loadedSaveData.footControls, MoControlsSaveData.loadedSaveData.drivingControls);
            // Setting up toolmode stuff
            toolModeGameObject = GameObject.Find(toolModeLocation);
            handModeGameObject = GameObject.Find(handModeLocation);
            // Tool mode hold button set up
            HoldInputMono him = gameObject.AddComponent<HoldInputMono>();
            him.setData("Toggle Tool Mode", XboxButtonEnum.Start, 0.3f, requestedModeChange);
            // Setting up guiNav "toolMode MouseScroll".
            toolModeScroll = gameObject.AddComponent<GuiNav>();
            toolModeScroll.setControls(XboxAxisEnum.rightTrigger, XboxButtonEnum.NULL,
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
            // Written, 31.08.2018 | Updated, 30.05.2022
                       

            // Enable scroll only if player is on foot and in tool mode (2) OR when player is holding an item while on foot and in hand mode.
            switch (currentPlayerMode)
            {
                case PlayerModeEnum.OnFoot:
                    toolModeScroll.enabled = isInToolMode ? true : !isPlayerHandEmpty();
                    break;
                default:
                    toolModeScroll.enabled = false;
                    break;
            }

            // Handling xbox controller force feedback rumble events.
            if (setFfbForVehicle())
                handleFfbOnXboxController();
            else if (MoControlsGO.xboxController.prevRumblePow.magnitude > 0)
            {
                lastRumblePow = Vector2.zero;
                MoControlsGO.xboxController.setRumble(Vector2.zero);
            }
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
                    drivingControls = loadControlInputsFromCInput();
                else
                    footControls = loadControlInputsFromCInput();
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

            currentControls = loadControlInputsFromCInput();
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

            footControls = inFootControls;
            drivingControls = inDrivingControls;
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
                    for (int i = 0; i < drivingControls.GetLength(0); i++)
                    {
                        string _controlName = drivingControls[i, 0];
                        if (inControlName == _controlName)
                        {
                            drivingControls[i, inIndex] = inInput;
                            break;
                        }
                    }
                    if (currentPlayerMode == inMode)
                        loadControlModeToCInput(inMode, drivingControls);
                    break;
                case PlayerModeEnum.OnFoot:
                    for (int i = 0; i < footControls.GetLength(0); i++)
                    {
                        string _controlName = footControls[i, 0];
                        if (inControlName == _controlName)
                        {
                            footControls[i, inIndex] = inInput;
                            break;
                        }
                    }
                    if (currentPlayerMode == inMode)
                        loadControlModeToCInput(inMode, footControls);
                    break;
            }
        }
        /// <summary>
        /// Changes the input for a control defined in <see cref="changeInputResult"/> to the provided input string, <paramref name="inInput"/>.
        /// </summary>
        /// <param name="inInput">The input to assign.</param>
        internal void changeInput(string inInput)
        {
            // Written, 09.07.2018 | Updated, 30.05.2022

            // Treat as a game control.            
            setGameControl((PlayerModeEnum)changeInputResult.mode, changeInputResult.controlName, changeInputResult.index, inInput);
            MoControlsMod.print("Player mode was equal to <b>" + changeInputResult.mode + "</b> whiling setting '" + changeInputResult.controlName + "' to '" + inInput + "'.", DebugTypeEnum.full);
            MoControlsSaveData.loadedSaveData.saveSettings();

            setChangeInput();
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
        private bool isPlayerHandEmpty()
        {
            // Written, 27.02.2022

            return handEmpty.Value;
        }
        /// <summary>
        /// Handles ffb on the xbox controller.
        /// </summary>
        private void handleFfbOnXboxController()
        {
            // Written, 16.10.2020

            if (MoControlsSaveData.loadedSaveData.ffbOnXboxController && vehicle != null)
            {
                lastRumblePow = getFfbSetOpt();
                MoControlsGO.xboxController.setRumble(lastRumblePow);
            }
            else if (lastRumblePow.magnitude != 0)
            {
                lastRumblePow = Vector2.zero;
                MoControlsGO.xboxController.setRumble(lastRumblePow);
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
                return floatToVector(defaultFfb());
            }
            else
            {
                if (MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip)
                {
                    float wheelSlip = Mathf.Clamp(wheelSlipBasedFfb(), -1, 1);
                    if (wheelSlip > 0)
                        xMotor = wheelSlip;
                    else
                        yMotor = Mathf.Abs(wheelSlip);
                } if (MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter)
                    xMotor += rpmLimiterBasedFfb();
                if (MoControlsSaveData.loadedSaveData.ffbOption_gearChange)
                    yMotor += gearChangeBasedFfb();
            }
            return new Vector2(Mathf.Clamp(xMotor, 0, 1), Mathf.Clamp(yMotor, 0, 1));
        }
        private float gearChangeBasedFfb()
        {
            // Written, 23.10.2020

            if (drivetrain.changingGear)
                return drivetrain.rpm / drivetrain.maxRPM;
            return 0;
        }
        private float rpmLimiterBasedFfb()
        {
            // Written, 18.10.2020 || updated, 28.09.2021
            
            if (drivetrain.revLimiterTriggered)
                return drivetrain.rpm / drivetrain.maxRPM;
            return 0;
        }
        private float wheelSlipBasedFfb()
        {
            // Written, 18.10.2020

            return drivetrain.poweredWheels.Max(_wheel => Mathf.Abs(_wheel.longitudinalSlip)); // based on car roll, facing forward
        }
        /// <summary>
        /// Represents the default ffb (designed for a wheel) doesnt work properly
        /// </summary>
        private float defaultFfb()
        {
            // Written, 18.10.2020

            return carDynamics.forceFeedback / 100;
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

            changeInputResult = inChangeInput ?? new ChangeInput();
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
                        vehicle = GameObject.Find("SATSUMA(557kg, 248)");
                        carDynamics = vehicle.GetComponent<CarDynamics>();
                        drivetrain = vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Jonnez":
                        vehicle = GameObject.Find("JONNEZ ES(Clone)");
                        carDynamics = vehicle.GetComponent<CarDynamics>();
                        drivetrain = vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Kekmet":
                        vehicle = GameObject.Find("KEKMET(350-400psi)");
                        carDynamics = vehicle.GetComponent<CarDynamics>();
                        drivetrain = vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Hayosiko":
                        vehicle = GameObject.Find("HAYOSIKO(1500kg, 250)");
                        carDynamics = vehicle.GetComponent<CarDynamics>();
                        drivetrain = vehicle.GetComponent<Drivetrain>();
                        break;
                    case "Gifu":
                        vehicle = GameObject.Find("GIFU(750/450psi)");
                        carDynamics = vehicle.GetComponent<CarDynamics>();
                        drivetrain = vehicle.GetComponent<Drivetrain>();
                        break;
                    default:
                        vehicle = null;
                        break;
                }
                currentVehicleName = _currentVehicleName;
            }
            return vehicle != null;
        }

        #endregion
    }
}
