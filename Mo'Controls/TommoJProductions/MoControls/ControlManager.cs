using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using JetBrains.Annotations;

using MSCLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TommoJProductions.Debugging;
using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;
using uInput = UnityEngine.Input;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents the control manager for mo Controls.
    /// </summary>
    public class ControlManager : SingletonMonoBehaviour<ControlManager>
    {
        // Written, 22.08.2018

        public struct ScrollParameters
        {
            public string ltInput;
            public int ltIndex;
            public int ltIndex2;
            public string rtInput;
            public int rtIndex;
            public int rtIndex2;
        }
        public struct VehicleData 
        {
            // Written, 07.08.2022

            public AxisCarController controller;
            public Drivetrain drivetrain;
            public CarDynamics dynamics;
            public GameObject vehicle;

            public readonly static VehicleData defaultVehicleData = default(VehicleData);

            public VehicleData(AxisCarController controller)
            {
                this.controller = controller;
                drivetrain = controller.GetComponent<Drivetrain>();
                dynamics = controller.GetComponent<CarDynamics>();
                vehicle = controller.gameObject;
            }
        }

        internal Camera playerCamera;
        internal GameObject player;

        #region Events 

        public event Action onPlayerModeChanged = delegate { };

        #endregion

        #region Fields

        /// <summary>
        /// Represents logic to enable scroll to a connected xbox controllers' triggers. enabled when player is in tool mode.
        /// </summary>
        private XboxControllerScroll toolModeScroll;
        /// <summary>
        /// Represents the current car dynamics. used for forcefeedback.
        /// </summary>
        internal VehicleData ffbVehicle;
        /// <summary>
        /// Reprsents the current player mode. (cache)
        /// </summary>
        private PlayerModeEnum currentPlayerMode = PlayerModeEnum.InMenu; 

        private readonly string[,] _blankControls = new string[,]
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
        private string[,] _drivingControls;
        private string[,] _footControls;

        #region raycast scrollables fields.

        private PlayMakerFSM[] raycastedFsms;
        private FsmState activeState;

        #endregion

        private FsmBool handEmpty;
        private PlayMakerFSM pickUp;

        /// <summary>
        /// Reprsents the current player mode fsm string. used to cached reference in <see cref="getCurrentPlayerMode"/>.
        /// </summary>
        private static FsmString _playerCurrentVehicle;

        private int raycastScrollableMask;

        internal Coroutine scrollCoroutine;
        private Coroutine toggleToolModeRoutine;

        private ScrollParameters footControlsScrollParameters = new ScrollParameters();
        private ScrollParameters drivingControlsScrollParameters = new ScrollParameters();

        private XboxControllerScroll.XboxControlGroup scrollDown = new XboxControllerScroll.XboxControlGroup(XboxAxisEnum.LT);
        private XboxControllerScroll.XboxControlGroup scrollUp = new XboxControllerScroll.XboxControlGroup(XboxAxisEnum.RT);
        
        private ControllerLook controllerLookX;
        private ControllerLook controllerLookY;

        private MouseLook mouseLookX;
        private MouseLook mouseLookY;

        private ControllerMove controllerMove;
        private FPSInputController fpsMove;

        private FieldInfo rotationYFieldInfo;

        private bool scrollSet = false;
        private bool scrollPassed = false;
        internal bool usingController = false;
        private bool deviceChanged = false;
        private bool playerStopped = false;
        private bool updateCursorSet = false;

        internal Dictionary<string, VehicleData> vehicles = new Dictionary<string, VehicleData>();
        internal string[] vehicleNames;

        private GameObject raycastScrollableGameObject;
        private RaycastHit raycastScrollableHit;

        #endregion

        #region Properties

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
                    _drivingControls = value;
                else if (currentPlayerMode == PlayerModeEnum.OnFoot)
                    _footControls = value;
            }
        }
        /// <summary>
        /// Represents the current player mode. either in menu, on foot, or driving.
        /// </summary>
        internal PlayerModeEnum getCurrentPlayerMode
        {
            get
            {
                if (MoControlsGUI.instance.controlsGuiOpened || playerInMenu.Value)
                    return PlayerModeEnum.InMenu;
                switch (playerCurrentVehicle)
                {
                    case "":
                        return PlayerModeEnum.OnFoot;
                    default:
                        return PlayerModeEnum.Driving;
                }
            }
        }
        /// <summary>
        /// Represents the current vehicle name. used for setting ffb on all supported vehicles.
        /// </summary>
        internal static string ffbCurrentSupportedVehicleName 
        {
            get;
            private set;
        }
        /// <summary>
        /// player current vehicle string (cache => PlayMakerGlobals).
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
        internal string[,] footControls => _footControls;
        /// <summary>
        /// Represents the current driving controls.
        /// </summary>
        internal string[,] drivingControls => _drivingControls;
        /// <summary>
        /// Represents blank controls.
        /// </summary>
        internal string[,] blankControls => _blankControls;

        /// <summary>
        /// Returns true if in hand mode. determines state by <see cref="getHandPickUpFsm"/>.Active.
        /// </summary>
        internal bool isInHandMode => pickUp.Active;

        /// <summary>
        /// Represents an xbox controller.
        /// </summary>
        private XboxController controller;

        internal FsmBool playerInMenu { get; private set; }

        #endregion

        #region unity runtime

        private void OnEnable()
        {
            // Written, 05.08.2022

            onPlayerModeChanged += onPlayerModeChanged_loadControlsToCInput;
            onPlayerModeChanged += onPlayerModeChanged_handleFfbOnXboxController;
            //onPlayerModeChanged += changeDrivingControls;
        }

        private void OnDisable()
        {
            // Written, 05.08.2022

            onPlayerModeChanged -= onPlayerModeChanged_loadControlsToCInput;
            onPlayerModeChanged -= onPlayerModeChanged_handleFfbOnXboxController;
            //onPlayerModeChanged -= changeDrivingControls;
        }
        private void OnApplicationQuit()
        {
            // Written, 30.06.2022

            scrollPassedReset();
        }
        private void Awake() 
        {
            // Written, 31.07.2022

            playerInMenu = FsmVariables.GlobalVariables.GetFsmBool("PlayerInMenu");
            changeInputResult = new ChangeInput();
        }
        private void Start()
        {
            // Written, 08.10.2018 | Modifed, 07.2022

            GameObject pov = PlayMakerGlobals.Instance.Variables.FindFsmGameObject("POV").Value;
            playerCamera = pov.GetComponent<Camera>();  

            player = pov.transform.root.gameObject;
            pickUp = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
            handEmpty = pickUp.FsmVariables.FindFsmBool("HandEmpty");
            _drivingControls = MoControlsSaveData.loadedSaveData.drivingControls;
            _footControls = MoControlsSaveData.loadedSaveData.footControls;
            HoldInputMono him = gameObject.AddComponent<HoldInputMono>();
            him.setData("Toggle Tool Mode", XboxButtonEnum.Start, 0.3f, toggleToolMode);
            toolModeScroll = gameObject.AddComponent<XboxControllerScroll>();
            toolModeScroll.setControls(scrollDown, scrollUp);
            raycastScrollableMask = LayerMask.GetMask("Parts", "Dashboard", "Bolts", "DontCollide");
            controller = XboxControllerManager.instance.controller;

            controllerLookX = player.AddComponent<ControllerLook>();
            mouseLookX = player.GetComponent<MouseLook>();
            controllerLookX.axis = MouseLook.RotationAxes.MouseX;
            controllerLookX.mouseLook = mouseLookX;
            mouseLookX.enabled = false;
            controllerLookX.enabled = false;

            controllerLookY = pov.transform.parent.gameObject.AddComponent<ControllerLook>();
            mouseLookY = controllerLookY.GetComponent<MouseLook>();
            controllerLookY.axis = MouseLook.RotationAxes.MouseY;
            controllerLookY.mouseLook = mouseLookY;
            mouseLookY.enabled = false;
            controllerLookY.enabled = false;
            rotationYFieldInfo = typeof(MouseLook).GetField("rotationY", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            PlayMakerFSM cursor = player.GetPlayMaker("Update Cursor");
            FsmState uc = cursor.GetState("Update cursor");
            uc.RemoveAction(4);
            uc.RemoveAction(3);
            uc = cursor.GetState("In Menu");
            uc.RemoveAction(3);
            uc.RemoveAction(2);
            FsmHook.FsmInject(player, "Update cursor", updateCursor);
            FsmHook.FsmInject(player, "In Menu", inMenu);

            fpsMove = player.GetComponent<FPSInputController>();
            controllerMove = player.AddComponent<ControllerMove>();
            controllerMove.enabled = false;

            FsmHook.FsmInject(player, "Stop", playerStopping);
            FsmHook.FsmInject(player, "Move", playerMoving);

            getAllVehicleReferences();
            //setCarControllerReferences();

            ffbVehicle = VehicleData.defaultVehicleData;

            playerMoveAsInputEnableCheck();      
        }

        private void Update()
        {
            // Written, 31.08.2018 | Updated, 30.05.2022

            playerModeCheck();
            mouseControllerSwitch();
            if (usingController)
            {
                MouseEmulator.instance.simulateLeftClick();
                MouseEmulator.instance.simulateRightClick();
            }
        }

        #endregion

        #region IEnumerators

        internal IEnumerator scrollFunction()
        {
            // Written, 04.06.2022

            while (controller.isConnected)
            {
                yield return null;
                if (currentPlayerMode == PlayerModeEnum.InMenu)
                {
                    toolModeScroll.enabled = false;
                    continue;
                }
                if (isInHandMode && handEmpty.Value)
                {
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
                            if (footControls[i, 1] == controller.lT.inputName)
                            {
                                footControlsScrollParameters.ltInput = footControls[i, 1];
                                footControlsScrollParameters.ltIndex = i;
                                footControlsScrollParameters.ltIndex2 = 1;
                                footControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (footControls[i, 2] == controller.lT.inputName)
                            {
                                footControlsScrollParameters.ltInput = footControls[i, 2];
                                footControlsScrollParameters.ltIndex = i;
                                footControlsScrollParameters.ltIndex2 = 2;
                                footControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (drivingControls[i, 1] == controller.lT.inputName)
                            { 
                                drivingControlsScrollParameters.ltInput = drivingControls[i, 1];
                                drivingControlsScrollParameters.ltIndex = i;
                                drivingControlsScrollParameters.ltIndex2 = 1;
                                drivingControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (drivingControls[i, 2] == controller.lT.inputName)
                            {
                                drivingControlsScrollParameters.ltInput = drivingControls[i, 2];
                                drivingControlsScrollParameters.ltIndex = i;
                                drivingControlsScrollParameters.ltIndex2 = 2;
                                drivingControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (footControls[i, 1] == controller.rT.inputName)
                            {
                                footControlsScrollParameters.rtInput = footControls[i, 1];
                                footControlsScrollParameters.rtIndex = i;
                                footControlsScrollParameters.rtIndex2 = 1;
                                footControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (footControls[i, 2] == controller.rT.inputName)
                            {
                                footControlsScrollParameters.rtInput = footControls[i, 2];
                                footControlsScrollParameters.rtIndex = i;
                                footControlsScrollParameters.rtIndex2 = 2;
                                footControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (drivingControls[i, 1] == controller.rT.inputName)
                            {
                                drivingControlsScrollParameters.rtInput = drivingControls[i, 1];
                                drivingControlsScrollParameters.rtIndex = i;
                                drivingControlsScrollParameters.rtIndex2 = 1;
                                drivingControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (drivingControls[i, 2] == controller.rT.inputName)
                            {
                                drivingControlsScrollParameters.rtInput = drivingControls[i, 2];
                                drivingControlsScrollParameters.rtIndex = i;
                                drivingControlsScrollParameters.rtIndex2 = 2;
                                drivingControls[i, 2] = "None";

                                scrollSet = true;
                            }
                            yield return null;
                        }
                        if (scrollSet)
                        {
                            loadCurrentControlsToCInput();
                            continue;
                        }
                        yield return null;
                    }
                    yield return null;
                    continue;
                }
                else if (scrollPassed)
                {
                    scrollPassedReset();
                }
            }
            scrollCoroutine = null;
        }

        #endregion

        #region Methods

        internal void playerMoveAsInputEnableCheck() 
        {
            // Written, 09.08.2022

            for (int i = 15; i <= 19; i++) // player move and jump index range.
            {
                XboxControl xboxControl;
                for (int j = 0; j < controller.xboxControls.Length; j++)
                {
                    xboxControl = controller.xboxControls[j];
                                        
                    if (xboxControl.inputName == footControls[i, 1])
                        footControls[i, 1] = Keys.None;

                    if (xboxControl.inputName == footControls[i, 2])
                        footControls[i, 2] = Keys.None;

                    if (xboxControl.inputName == drivingControls[i, 1])
                        drivingControls[i, 1] = Keys.None;

                    if (xboxControl.inputName == drivingControls[i, 2])
                        drivingControls[i, 2] = Keys.None;
                }
            }
        }
        /// <summary>
        /// switches controls from keyboard/mouse to controller.
        /// </summary>
        private void mouseControllerSwitch()
        {
            // Written, 23.07.2022

            if (MoControlsSaveData.loadedSaveData.deviceDetection != UsingDeviceEnum.Auto)
                return;

            if (usingController)
            {
                if (KeyboardEmulator.anyInput() || (!MouseEmulator.instance.enabled && ((uInput.GetAxisRaw("Mouse X") != 0) || (uInput.GetAxisRaw("Mouse Y") != 0))))
                {
                    usingController = false;
                    deviceChanged = true;
                }
            }
            else if (controller.anyInput)
            {
                usingController = true;
                deviceChanged = true;
            }

            if (deviceChanged && !playerInMenu.Value)
            {
                setDevice();
                deviceChanged = false;
            }
        }
        /// <summary>
        /// Sets all behaviours and values for switching between keyboard/mouse and controller.
        /// </summary>
        private void setDevice()
        {
            // Written, 04.08.2022

            if (usingController)
            {
                if (deviceChanged)
                {
                    controllerLookY.rotationY = (float)rotationYFieldInfo.GetValue(mouseLookY);
                }

                mouseLookX.enabled = false;
                mouseLookY.enabled = false;
                controllerLookX.enabled = true;
                controllerLookY.enabled = true;
            }
            else
            {
                if (deviceChanged)
                {
                    rotationYFieldInfo.SetValue(controllerLookY.mouseLook, controllerLookY.rotationY);
                }

                mouseLookX.enabled = true;
                mouseLookY.enabled = true;
                controllerLookX.enabled = false;
                controllerLookY.enabled = false;
            }

            if (!playerStopped)
            {
                fpsMove.enabled = !usingController;
                controllerMove.enabled = usingController;
            }
        }
        /// <summary>
        /// checks if player mode has changed. => foot, driving or in menu. updates <see cref="currentPlayerMode"/> and <see cref="previousPlayerMode"/>. invokes event <see cref="onPlayerModeChanged"/> if mode has changed.
        /// </summary>
        private void playerModeCheck()
        {
            // Written, 05.08.2022

            if (currentPlayerMode != getCurrentPlayerMode)
            {                              
                currentPlayerMode = getCurrentPlayerMode;
                onPlayerModeChanged.Invoke();
                MoControlsMod.print("Control Mode changed: " + currentPlayerMode, DebugTypeEnum.full);
            }
        }        
        /// <summary>
        /// resets foot and driving controls trigger inputs.
        /// </summary>
        private void scrollPassedReset()
        {
            // Written, 30.06.2022

            scrollPassed = false;
            if (scrollSet)
            {
                scrollSet = false;
                if (footControlsScrollParameters.rtInput != null)
                    footControls[footControlsScrollParameters.rtIndex, footControlsScrollParameters.rtIndex2] = footControlsScrollParameters.rtInput;
                if (drivingControlsScrollParameters.rtInput != null)
                    drivingControls[drivingControlsScrollParameters.rtIndex, drivingControlsScrollParameters.rtIndex2] = drivingControlsScrollParameters.rtInput;
                if (footControlsScrollParameters.ltInput != null)
                    footControls[footControlsScrollParameters.ltIndex, footControlsScrollParameters.ltIndex2] = footControlsScrollParameters.ltInput;
                if (drivingControlsScrollParameters.ltInput != null)
                    drivingControls[drivingControlsScrollParameters.ltIndex, drivingControlsScrollParameters.ltIndex2] = drivingControlsScrollParameters.ltInput;
                footControlsScrollParameters.rtInput = null;
                footControlsScrollParameters.ltInput = null;
                drivingControlsScrollParameters.rtInput = null;
                drivingControlsScrollParameters.ltInput = null;
                loadCurrentControlsToCInput();
            }
        }
        /// <summary>
        /// Checks if the raycasted gameobject has a playmakerfsm component and if the fsm has a state by the name of any <see cref="scrollables"/>
        /// </summary>
        private bool detectScrollable() 
        {
            // Written, 23.07.2022

            raycastScrollableGameObject = raycastForScrollableMask();

            if (!raycastScrollableGameObject)
                return false;

            raycastedFsms = raycastScrollableGameObject.GetComponents<PlayMakerFSM>();
            if (raycastedFsms != null)
            {
                for (int fsm = 0; fsm < raycastedFsms.Length; fsm++)
                {
                    for (int state = 0; state < raycastedFsms[fsm].FsmStates.Length; state++)
                    {
                        activeState = raycastedFsms[fsm].FsmStates[state];
                        for (int action = 0; action < activeState.Actions.Length; action++)
                        {
                            if (activeState != null && activeState.Actions[action] is GetAxis)
                            {
                                if ((activeState.Actions[action] as GetAxis).axisName.Value == "Mouse ScrollWheel")
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Raycast for gameobjects on the <see cref="raycastScrollableMask"/>
        /// </summary>
        private GameObject raycastForScrollableMask()
        {
            // Written, 31.05.2022

            if (Physics.Raycast(playerCamera.ScreenPointToRay(uInput.mousePosition), out raycastScrollableHit, 1, raycastScrollableMask))
            {
                return raycastScrollableHit.collider.gameObject;
            }
            return null;
        }
        /// <summary>
        /// Loads provided control list to cInput.
        /// </summary>
        /// <param name="controls">The control list to load.</param>
        internal void loadControlsToCInput(string[,] controls) 
        {
            // Written, 31.08.2018 | Modified, 04.08.2022

            for (int i = 0; i < controls.GetLength(0); i++)
            {
                cInput.ChangeKey(controls[i, 0], controls[i, 1], controls[i, 2]);
            }
        }
        /// <summary>
        /// loads <see cref="currentControls"/> set to cinput.
        /// </summary>
        private void loadCurrentControlsToCInput() => loadControlsToCInput(currentControls);
        /// <summary>
        /// Sets the provided game control in provided mode.
        /// </summary>
        /// <param name="inMode"></param>
        private void setGameControl(string inputName)
        {
            // Written, 02.09.2018

            switch (changeInputResult.mode)
            {
                case PlayerModeEnum.Driving:
                    changeControl(inputName, ref _drivingControls);
                    break;
                case PlayerModeEnum.OnFoot:
                    changeControl(inputName, ref _footControls);
                    break;
            }
        }
        private void changeKeyBind() 
        {
            // Written, 08.08.2022


        }
        /// <summary>
        /// Changes a input on the provided control list <see cref="controls"/>. all values in <see cref="changeInputResult"/>.
        /// </summary>
        /// <param name="inputName">the input name to change to</param>
        /// <param name="controls">the control list to change a input name in</param>
        private void changeControl(string inputName, ref string[,] controls)
        {
            // Written, 07.08.2022

            for (int i = 0; i < controls.GetLength(0); i++)
            {
                if (changeInputResult.controlName == controls[i, 0])
                {
                    controls[i, changeInputResult.index] = inputName;
                    break;
                }
            }
        }
        /// <summary>
        /// Changes the input for a control defined in <see cref="changeInputResult"/> to the provided input string, <paramref name="inputName"/>.
        /// </summary>
        /// <param name="inputName">The input name to assign. eg => Joy Axis 3+</param>
        internal void changeInput(string inputName)
        {
            // Written, 09.07.2018 | Updated, 30.05.2022
           
            setGameControl(inputName);
            MoControlsSaveData.loadedSaveData.saveSettings();
            changeInputResult.reset();
        }
        /// <summary>
        /// Toggles toolmode eg. tool=>hand | hand=>tool
        /// </summary>
        internal void toggleToolMode()
        {
            // Written, 08.10.2020

            VirtualKeyShort wVk;

            if (!isInHandMode)
                wVk = VirtualKeyShort.KEY_1;
            else
                wVk = VirtualKeyShort.KEY_2;

            if (toggleToolModeRoutine != null)
                StopCoroutine(toggleToolModeRoutine);
            toggleToolModeRoutine = StartCoroutine(KeyboardEmulator.simulateKeyPressCoroutine(wVk));
        }        
        /// <summary>
        /// Gets set options for ffb and returns the result as a vector2.
        /// </summary>
        internal Vector2 getFfbSetOpt()
        {
            // Written, 18.10.2020

            Vector2 motor = new Vector2();
            if (MoControlsSaveData.loadedSaveData.ffbOption_default)
            {
                return floatToRumbleEffect(defaultFfb());
            }
            else
            {
                if (MoControlsSaveData.loadedSaveData.ffbOption_wheelSlip)
                {
                    motor = wheelSlipBasedFfb();
                } 
                if (MoControlsSaveData.loadedSaveData.ffbOption_rpmLimiter)
                    motor.x += rpmLimiterBasedFfb();
                if (MoControlsSaveData.loadedSaveData.ffbOption_gearChange)
                    motor.y += gearChangeBasedFfb();
            }
            return motor;
        }
        /// <summary>
        /// Calulates force feed back based on gear change
        /// </summary>
        private float gearChangeBasedFfb()
        {
            // Written, 23.10.2020

            if (ffbVehicle.drivetrain.changingGear)
                return ffbVehicle.drivetrain.rpm / ffbVehicle.drivetrain.maxRPM;
            return 0;
        }
        /// <summary>
        /// Calulates force feed back based on rpm limiter
        /// </summary>
        private float rpmLimiterBasedFfb()
        {
            // Written, 18.10.2020 || updated, 28.09.2021
            
            if (ffbVehicle.drivetrain.revLimiterTriggered)
                return ffbVehicle.drivetrain.rpm / ffbVehicle.drivetrain.maxRPM;
            return 0;
        }
        /// <summary>
        /// Calulates force feed back based on wheel slip.
        /// </summary>
        private Vector2 wheelSlipBasedFfb()
        {
            // Written, 18.10.2020

            return new Vector2(ffbVehicle.drivetrain.poweredWheels[0].longitudinalSlip, ffbVehicle.drivetrain.poweredWheels[1].longitudinalSlip);
        }
        /// <summary>
        /// Represents the default ffb (designed for a wheel) doesnt work properly
        /// </summary>
        private float defaultFfb()
        {
            // Written, 18.10.2020

            return ffbVehicle.dynamics.forceFeedback / 100;
        }
        /// <summary>
        /// Converts ffb float value to a vector 2 for xbox rumble events.
        /// </summary>
        /// <param name="inValue">The force feed back value.</param>
        private Vector2 floatToRumbleEffect(float inValue)
        {
            // Written, 16.10.2020

            if (inValue > 0)
                return new Vector2(inValue, 0);
            else if (inValue < 0)
                return new Vector2(0, inValue * -1);
            else
                return Vector2.zero;
        }
        /// <summary>
        /// Sets all ffb to the current vehicle.
        /// </summary>
        private bool setFfbForVehicle() 
        {
            // Written, 23.10.2020, Updated, 31.05.2022

            if (playerCurrentVehicle != ffbCurrentSupportedVehicleName)
            {
                string key;
                switch (playerCurrentVehicle)
                {
                    case "Satsuma":
                        key = "SATSUMA(557kg, 248)";
                        break;
                    case "Jonnez":
                        key = "JONNEZ ES(Clone)";
                        break;
                    case "Kekmet":
                        key = "KEKMET(350-400psi)";
                        break;
                    case "Hayosiko":
                        key = "HAYOSIKO(1500kg, 250)";
                        break;
                    case "Gifu":
                        key = "GIFU(750/450psi)";
                        break;
                    case "Ruscko":
                        key = "RCO_RUSCKO12(270)";
                        break;
                    case "Ferndale":
                        key = "FERNDALE(1630kg)";
                        break;
                    case "Combine":
                        key = "COMBINE(350-400psi)";
                        break;
                    default:
                    case "":
                        key = "";
                        break;
                }
                if (vehicles.ContainsKey(key))
                {
                    ffbVehicle = vehicles[key];
                    ffbCurrentSupportedVehicleName = key;
                }
                else
                {
                    ffbVehicle = VehicleData.defaultVehicleData;
                }
            }
            return ffbVehicle.vehicle != null;
        }

        internal void getAllVehicleReferences()
        {
            // Written, 07.08.2022

            AxisCarController[] carControllers = Resources.FindObjectsOfTypeAll<AxisCarController>();
            vehicleNames = new string[carControllers.Length];
            vehicles.Clear();
            for (int i = 0; i < carControllers.Length; i++)
            {
                vehicleNames[i] = carControllers[i].name;
                vehicles.Add(vehicleNames[i], new VehicleData(carControllers[i]));
            }
        }

        #endregion

        #region Event Handlers
        
        /// <summary>
        /// Handles ffb on the xbox controller.
        /// </summary>
        private void onPlayerModeChanged_handleFfbOnXboxController()
        {
            // Written, 16.10.2020

            if (MoControlsSaveData.loadedSaveData.ffbOnXboxController)
            {
                if (setFfbForVehicle())
                {
                    if (ffbVehicle.vehicle)
                    {
                        controller.setRumble(getFfbSetOpt());
                    }
                }
                else if (controller.prevRumblePow != XboxController.vector2Zero)
                {
                    controller.setRumble(XboxController.vector2Zero);
                }
            }
        }
        private void onPlayerModeChanged_loadControlsToCInput()
        {
            // Written, 05.08.2022

            loadCurrentControlsToCInput();
        }

        private void playerStopping()
        {
            // Written, 04.08.2022
            
            playerStopped = true;
            controllerMove.enabled = false;
        }
        private void playerMoving()
        {
            // Written, 04.08.2022

            playerStopped = false;

            if (usingController)
            {
                controllerMove.enabled = true;
            }
        }
        private void updateCursor()
        {
            // Written, 06.08.2022

            if (!updateCursorSet)
            {
                updateCursorSet = true;
                setDevice();
                MouseEmulator.instance.enabled = false;
            }
        }
        private void inMenu()
        {
            // Written, 23.07.2022

            updateCursorSet = false;
            mouseLookX.enabled = false;
            mouseLookY.enabled = false;
            controllerLookX.enabled = false;
            controllerLookY.enabled = false;
            controllerMove.enabled = false;
            fpsMove.enabled = false;
            MouseEmulator.instance.enabled = true;
        }

        #endregion
    }
}
