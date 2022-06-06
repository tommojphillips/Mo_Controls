using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MSCLoader;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private XboxControllerScroll toolModeScroll;
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
        private PlayerModeEnum currentPlayerMode;
        private FsmBool handEmpty;
        private PlayMakerFSM pickUp;
        private static FsmString _playerCurrentVehicle;

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
                switch (playerCurrentVehicle)
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
        /// Represents the player current vehicle state.
        /// </summary>
        internal static string playerCurrentVehicle
        {
            get
            {
                if (_playerCurrentVehicle == null)
                    _playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle");
                return _playerCurrentVehicle.Value;
            }
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
        /// Gets the pickup playmakerfsm from the hand gameobject.
        /// </summary>
        public PlayMakerFSM getHandPickUpFsm
        {
            get
            {
                if (pickUp == null)
                    pickUp = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
                return pickUp;
            }
        }
        /// <summary>
        /// Returns true if in hand mode. determines state by <see cref="getHandPickUpFsm"/>.Active.
        /// </summary>
        public bool isInHandMode => getHandPickUpFsm.Active;

        #endregion

        #region Constuctors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ControlManager()
        {
            // Written, 22.08.2018

            setChangeInput();
            //cInput.OnKeyChanged -= cInput_OnKeyChanged;
            //cInput.OnKeyChanged += cInput_OnKeyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            handEmpty = getHandPickUpFsm.FsmVariables.FindFsmBool("HandEmpty");
            MoControlsGO.controlManager.setControls(MoControlsSaveData.loadedSaveData.footControls, MoControlsSaveData.loadedSaveData.drivingControls);
            HoldInputMono him = gameObject.AddComponent<HoldInputMono>();
            him.setData("Toggle Tool Mode", XboxButtonEnum.Start, 0.3f, requestedModeChange);
            toolModeScroll = gameObject.AddComponent<XboxControllerScroll>();
            toolModeScroll.setControls(scrollUpA: XboxAxisEnum.rightTrigger, scrollDownA: XboxAxisEnum.leftTrigger, menuUpB: XboxButtonEnum.RB, menuDownB: XboxButtonEnum.LB);
            MoControlsMod.print(nameof(ControlManager) + ": Started", DebugTypeEnum.full);
        }
        /// <summary>
        /// Occurs on update.
        /// </summary>
        private void Update()
        {
            // Written, 31.08.2018 | Updated, 30.05.2022

            cInputLoad();
            handleFfbOnXboxController();
            if (scrollCoroutine == null)
            {
                scrollCoroutine = StartCoroutine(scrollFunction());
                MoControlsMod.print("Scroll coroutine started", DebugTypeEnum.full);
            }
        }
        private void cInputLoad()
        {
            if (currentPlayerMode != getCurrentPlayerMode)
            {
                currentPlayerMode = getCurrentPlayerMode;
                loadControlModeToCInput();
                MoControlsMod.print("Control Mode changed: " + currentPlayerMode, DebugTypeEnum.full);
            }
        }
        private readonly ScrollParameters f = new ScrollParameters(), d = new ScrollParameters();
        private Coroutine scrollCoroutine;
        private bool scrollSet = false, scrollPassed = false;
        public class ScrollParameters
        {
            public string ltInput;
            public int ltIndex;
            public int ltIndex2;
            public string rtInput;
            public int rtIndex;
            public int rtIndex2;
        }
        private IEnumerator scrollFunction()
        {
            // Written, 04.06.2022
            while (isActiveAndEnabled)
            {
                if (currentPlayerMode == PlayerModeEnum.InMenu)
                {
                    toolModeScroll.enabled = false;
                    yield return null;
                    continue;
                }
                if (isInHandMode && handEmpty.Value)
                {
                    yield return null;
                    toolModeScroll.enabled = detectScrollable();
                }
                else
                    toolModeScroll.enabled = currentPlayerMode == PlayerModeEnum.OnFoot;
                if (toolModeScroll.enabled)
                {
                    if (!scrollPassed)
                    {
                        scrollPassed = true;
                        for (int i = 0; i < currentControls.GetLength(0); i++)
                        {
                            if (footControls[i, 1] == MoControlsGO.xboxController.lT.inputName)
                            {
                                f.ltInput = footControls[i, 1];
                                f.ltIndex = i;
                                f.ltIndex2 = 1;
                                footControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            if (footControls[i, 2] == MoControlsGO.xboxController.lT.inputName)
                            {
                                f.ltInput = footControls[i, 2];
                                f.ltIndex = i;
                                f.ltIndex2 = 2;
                                footControls[i, 2] = "None";

                                scrollSet = true;
                            }
                            if (drivingControls[i, 1] == MoControlsGO.xboxController.lT.inputName)
                            { 
                                d.ltInput = drivingControls[i, 1];
                                d.ltIndex = i;
                                d.ltIndex2 = 1;
                                drivingControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            if (drivingControls[i, 2] == MoControlsGO.xboxController.lT.inputName)
                            {
                                d.ltInput = drivingControls[i, 2];
                                d.ltIndex = i;
                                d.ltIndex2 = 2;
                                drivingControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (footControls[i, 1] == MoControlsGO.xboxController.rT.inputName)
                            {
                                f.rtInput = footControls[i, 1];
                                f.rtIndex = i;
                                f.rtIndex2 = 1;
                                footControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            if (footControls[i, 2] == MoControlsGO.xboxController.rT.inputName)
                            {
                                f.rtInput = footControls[i, 2];
                                f.rtIndex = i;
                                f.rtIndex2 = 2;
                                footControls[i, 2] = "None";

                                scrollSet = true;
                            }
                            if (drivingControls[i, 1] == MoControlsGO.xboxController.rT.inputName)
                            {
                                d.rtInput = drivingControls[i, 1];
                                d.rtIndex = i;
                                d.rtIndex2 = 1;
                                drivingControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            if (drivingControls[i, 2] == MoControlsGO.xboxController.rT.inputName)
                            {
                                d.rtInput = drivingControls[i, 2];
                                d.rtIndex = i;
                                d.rtIndex2 = 2;
                                drivingControls[i, 2] = "None";

                                scrollSet = true;
                            }
                        }
                        if (scrollSet)
                        {
                            yield return null;
                            loadControlModeToCInput();
                            continue;
                        }
                    }
                    yield return null;
                    continue;
                }
                else if (scrollPassed)
                {
                    scrollSet = false;
                    scrollPassed = false;
                    if (f.rtInput != null)
                        footControls[f.rtIndex, f.rtIndex2] = f.rtInput;
                    if (d.rtInput != null)
                        drivingControls[d.rtIndex, d.rtIndex2] = d.rtInput;
                    if (f.ltInput != null)
                        footControls[f.ltIndex, f.ltIndex2] = f.ltInput;
                    if (d.ltInput != null)
                        drivingControls[d.ltIndex, d.ltIndex2] = d.ltInput;
                    f.rtInput = null;
                    f.ltInput = null;
                    d.rtInput = null;
                    d.ltInput = null;
                    yield return null;
                    loadControlModeToCInput();
                    continue;
                }
                yield return null;
            }
            scrollCoroutine = null;
            MoControlsMod.print("Scroll Routine ended", DebugTypeEnum.full);
        }
        /// <summary>
        /// Checks if the raycasted gameobject has a playmakerfsm component and if the fsm has a state by the name of any <see cref="scrollables"/>
        /// </summary>
        private bool detectScrollable() 
        {
            // Written, 31.05.2022

            return raycastForScrollable()?.GetComponents<PlayMakerFSM>()?.Any(fsm => fsm.FsmStates.Any(state => state.Actions.Any(action => action is GetAxis && (action as GetAxis)?.axisName.Value == "Mouse ScrollWheel"))) ?? false;
        }
        /// <summary>
        /// Raycast for gameobjects on the included layers.
        /// </summary>
        private GameObject raycastForScrollable() 
        {
            // Written, 31.05.2022

            if (Camera.main != null)
                if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hit, 1, LayerMask.GetMask("Parts", "Dashboard", "Bolts", "DontCollide")))
                {
                    return hit.collider?.gameObject;
                }
            return null;
        }
        private string[,] lastLoadedControls_cInput;
        /// <summary>
        /// Loads provided control list to cInput.
        /// </summary>
        /// <param name="inControlMode">The control mode.</param>
        private void loadControlModeToCInput()
        {
            // Written, 31.08.2018

            switch (currentPlayerMode)
            {
                case PlayerModeEnum.OnFoot:
                    lastLoadedControls_cInput = footControls;
                    break;
                case PlayerModeEnum.Driving:
                    lastLoadedControls_cInput = drivingControls;
                    break;
                default:
                    lastLoadedControls_cInput = blankControls;
                    break;
            }

            for (int i = 0; i < lastLoadedControls_cInput.GetLength(0); i++)
            {
                cInput.ChangeKey(lastLoadedControls_cInput[i, 0], lastLoadedControls_cInput[i, 1], lastLoadedControls_cInput[i, 2]);
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
                        loadControlModeToCInput();
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
                        loadControlModeToCInput();
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

            if (handEmpty.Value)
            {
                VirtualKeyShort wVk;

                if (!isInHandMode)
                    wVk = VirtualKeyShort.KEY_1;
                else
                    wVk = VirtualKeyShort.KEY_2;
                StartCoroutine(KeyboardEmulator.simulateKeyPressCoroutine(wVk));
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
        /// Handles ffb on the xbox controller.
        /// </summary>
        private void handleFfbOnXboxController()
        {
            // Written, 16.10.2020

            if (setFfbForVehicle())
            {
                if (MoControlsSaveData.loadedSaveData.ffbOnXboxController && vehicle != null)
                {
                    MoControlsGO.xboxController.setRumble(getFfbSetOpt());
                }
            }
            else if (MoControlsGO.xboxController.prevRumblePow != Vector2.zero)
            {
                MoControlsGO.xboxController.setRumble(Vector2.zero);
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
                } 
                if (MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter)
                    xMotor += rpmLimiterBasedFfb();
                if (MoControlsSaveData.loadedSaveData.ffbOption_gearChange)
                    yMotor += gearChangeBasedFfb();
            }
            return new Vector2(Mathf.Clamp(xMotor, 0, 1), Mathf.Clamp(yMotor, 0, 1));
        }
        /// <summary>
        /// Calulates force feed back based on gear change
        /// </summary>
        private float gearChangeBasedFfb()
        {
            // Written, 23.10.2020

            if (drivetrain.changingGear)
                return drivetrain.rpm / drivetrain.maxRPM;
            return 0;
        }
        /// <summary>
        /// Calulates force feed back based on rpm limiter
        /// </summary>
        private float rpmLimiterBasedFfb()
        {
            // Written, 18.10.2020 || updated, 28.09.2021
            
            if (drivetrain.revLimiterTriggered)
                return drivetrain.rpm / drivetrain.maxRPM;
            return 0;
        }
        /// <summary>
        /// Calulates force feed back based on wheel slip.
        /// </summary>
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
            // Written, 23.10.2020, Updated, 31.05.2022

            if (playerCurrentVehicle != currentVehicleName)
            {
                switch (playerCurrentVehicle)
                {
                    case "Satsuma":
                        vehicle = GameObject.Find("SATSUMA(557kg, 248)");
                        break;
                    case "Jonnez":
                        vehicle = GameObject.Find("JONNEZ ES(Clone)");
                        break;
                    case "Kekmet":
                        vehicle = GameObject.Find("KEKMET(350-400psi)");
                        break;
                    case "Hayosiko":
                        vehicle = GameObject.Find("HAYOSIKO(1500kg, 250)");
                        break;
                    case "Gifu":
                        vehicle = GameObject.Find("GIFU(750/450psi)");
                        break;
                    default:
                        vehicle = null;
                        break;
                }
                carDynamics = vehicle?.GetComponent<CarDynamics>();
                drivetrain = vehicle?.GetComponent<Drivetrain>();
                currentVehicleName = playerCurrentVehicle;
            }
            return vehicle != null;
        }

        #endregion
    }
}
