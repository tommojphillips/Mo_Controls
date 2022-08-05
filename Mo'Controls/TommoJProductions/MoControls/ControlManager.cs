using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MSCLoader;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class ControlManager : MonoBehaviour
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

        public event Action onPlayerModeChanged = delegate { };

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
        /// Represents the satsuma gameobject. used for forcefeedback.
        /// </summary>
        internal GameObject vehicle;
        /// <summary>
        /// Reprsents the current player mode. (cache)
        /// </summary>
        private PlayerModeEnum currentPlayerMode; 
        /// <summary>
        /// the previous player mode. (cache)
        /// </summary>
        private PlayerModeEnum previousPlayerMode;

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

        #region raycast scrollables fields.

        private PlayMakerFSM[] raycastedFsms;
        private FsmState activeState;

        #endregion

        private FsmBool handEmpty;
        private PlayMakerFSM pickUp;

        private FsmBool _playerInMenu;
        /// <summary>
        /// Reprsents the current player mode fsm string. used to cached reference in <see cref="getCurrentPlayerMode"/>.
        /// </summary>
        private static FsmString _playerCurrentVehicle;

        private int raycastScrollableMask;

        internal Coroutine scrollCoroutine;
        private Coroutine toggleToolModeRoutine;

        private ScrollParameters footControlsScrollParameters = new ScrollParameters();
        private ScrollParameters drivingControlsScrollParameters = new ScrollParameters();
        
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
                    drivingControls = value;
                else if (currentPlayerMode == PlayerModeEnum.OnFoot)
                    footControls = value;
            }
        }
        /// <summary>
        /// Represents the current player mode. either in menu, on foot, or driving.
        /// </summary>
        internal PlayerModeEnum getCurrentPlayerMode
        {
            get
            {
                if (MoControlsGO.moControlsGui.controlsGuiOpened || playerInMenu)
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
        /// Represents the player current vehicle state. (PlayMakerGlobals)
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
        internal string[,] blankControls => _blankControls;

        /// <summary>
        /// Returns true if in hand mode. determines state by <see cref="getHandPickUpFsm"/>.Active.
        /// </summary>
        internal bool isInHandMode => pickUp.Active;

        /// <summary>
        /// Represents the xbox controller manager.
        /// </summary>
        internal XboxControllerManager xboxControllerManager
        {
            get;
            set;
        }
        /// <summary>
        /// Represents an xbox controller.
        /// </summary>
        internal XboxController xboxController
        {
            get
            {
                return xboxControllerManager.controller;
            }
        }

        internal bool playerInMenu 
        {
            get 
            {
                if (_playerInMenu == null)
                    _playerInMenu = FsmVariables.GlobalVariables.GetFsmBool("PlayerInMenu");
                return _playerInMenu.Value;
            }
            set
            {
                if (_playerInMenu == null)
                    _playerInMenu = FsmVariables.GlobalVariables.GetFsmBool("PlayerInMenu");
                _playerInMenu.Value = value;

            }
        }

        #endregion

        #region unity runtime

        private void OnEnable()
        {
            // Written, 05.08.2022

            onPlayerModeChanged += onPlayerModeChanged_deviceCheck;
            onPlayerModeChanged += onPlayerModeChanged_loadControlsToCInput;
        }

        private void OnDisable()
        {
            // Written, 05.08.2022

            onPlayerModeChanged -= onPlayerModeChanged_deviceCheck;
            onPlayerModeChanged -= onPlayerModeChanged_loadControlsToCInput;
        }
        private void OnApplicationQuit()
        {
            // Written, 30.06.2022

            scrollPassedReset();
        }
        private void Awake() 
        {
            // Written, 31.07.2022

            changeInputResult = new ChangeInput();
        }
        private void Start()
        {
            // Written, 08.10.2018 | Modifed, 07.2022

            GameObject player = Camera.main.transform.root.gameObject;
            pickUp = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
            handEmpty = pickUp.FsmVariables.FindFsmBool("HandEmpty");
            setControls(MoControlsSaveData.loadedSaveData.footControls, MoControlsSaveData.loadedSaveData.drivingControls);
            HoldInputMono him = gameObject.AddComponent<HoldInputMono>();
            him.setData("Toggle Tool Mode", XboxButtonEnum.Start, 0.3f, toggleToolMode);
            toolModeScroll = gameObject.AddComponent<XboxControllerScroll>();
            toolModeScroll.setControls(scrollUpA: XboxAxisEnum.RT, scrollDownA: XboxAxisEnum.LT);
            raycastScrollableMask = LayerMask.GetMask("Parts", "Dashboard", "Bolts", "DontCollide");

            controllerLookX = player.AddComponent<ControllerLook>();
            mouseLookX = player.GetComponent<MouseLook>();
            controllerLookX.axis = MouseLook.RotationAxes.MouseX;
            controllerLookX.mouseLook = mouseLookX;
            mouseLookX.enabled = false;
            controllerLookX.enabled = false;

            controllerLookY = Camera.main.transform.parent.gameObject.AddComponent<ControllerLook>();
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
        }
        private void Update()
        {
            // Written, 31.08.2018 | Updated, 30.05.2022

            playerModeCheck();
            handleFfbOnXboxController();
            mouseControllerSwitch();
            if (usingController)
            {
                MoControlsGO.mouseEmulator.simulateLeftClick();
                MoControlsGO.mouseEmulator.simulateRightClick();
            }
        }

        #endregion

        #region IEnumerators

        internal IEnumerator scrollFunction()
        {
            // Written, 04.06.2022

            while (xboxController.isConnected)
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
                            if (footControls[i, 1] == xboxController.lT.inputName)
                            {
                                footControlsScrollParameters.ltInput = footControls[i, 1];
                                footControlsScrollParameters.ltIndex = i;
                                footControlsScrollParameters.ltIndex2 = 1;
                                footControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (footControls[i, 2] == xboxController.lT.inputName)
                            {
                                footControlsScrollParameters.ltInput = footControls[i, 2];
                                footControlsScrollParameters.ltIndex = i;
                                footControlsScrollParameters.ltIndex2 = 2;
                                footControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (drivingControls[i, 1] == xboxController.lT.inputName)
                            { 
                                drivingControlsScrollParameters.ltInput = drivingControls[i, 1];
                                drivingControlsScrollParameters.ltIndex = i;
                                drivingControlsScrollParameters.ltIndex2 = 1;
                                drivingControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (drivingControls[i, 2] == xboxController.lT.inputName)
                            {
                                drivingControlsScrollParameters.ltInput = drivingControls[i, 2];
                                drivingControlsScrollParameters.ltIndex = i;
                                drivingControlsScrollParameters.ltIndex2 = 2;
                                drivingControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (footControls[i, 1] == xboxController.rT.inputName)
                            {
                                footControlsScrollParameters.rtInput = footControls[i, 1];
                                footControlsScrollParameters.rtIndex = i;
                                footControlsScrollParameters.rtIndex2 = 1;
                                footControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (footControls[i, 2] == xboxController.rT.inputName)
                            {
                                footControlsScrollParameters.rtInput = footControls[i, 2];
                                footControlsScrollParameters.rtIndex = i;
                                footControlsScrollParameters.rtIndex2 = 2;
                                footControls[i, 2] = "None";

                                scrollSet = true;
                            }

                            if (drivingControls[i, 1] == xboxController.rT.inputName)
                            {
                                drivingControlsScrollParameters.rtInput = drivingControls[i, 1];
                                drivingControlsScrollParameters.rtIndex = i;
                                drivingControlsScrollParameters.rtIndex2 = 1;
                                drivingControls[i, 1] = "None";

                                scrollSet = true;
                            }
                            else if (drivingControls[i, 2] == xboxController.rT.inputName)
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
                    continue;
                }
            }
            scrollCoroutine = null;
        }

        #endregion

        #region Methods

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
                if (KeyboardEmulator.anyInput() || (!MoControlsGO.mouseEmulator.enabled && ((uInput.GetAxisRaw("Mouse X") != 0) || (uInput.GetAxisRaw("Mouse Y") != 0))))
                {
                    usingController = false;
                    deviceChanged = true;
                }
            }
            else if (xboxController.anyInput)
            {
                usingController = true;
                deviceChanged = true;
            }

            if (deviceChanged && !playerInMenu)
            {
                deviceChanged = false;
                setDevice();
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
                mouseLookX.enabled = false;
                mouseLookY.enabled = false;
                controllerLookY.rotationY = (float)rotationYFieldInfo.GetValue(mouseLookY);
                controllerLookX.enabled = true;
                controllerLookY.enabled = true;
                if (!playerStopped)
                {
                    fpsMove.enabled = false;
                    controllerMove.enabled = true;
                }
            }
            else
            {
                rotationYFieldInfo.SetValue(controllerLookY.mouseLook, controllerLookY.rotationY);
                mouseLookX.enabled = true;
                mouseLookY.enabled = true;
                controllerLookX.enabled = false;
                controllerLookY.enabled = false;
                if (!playerStopped)
                {
                    fpsMove.enabled = true;
                    controllerMove.enabled = false;
                }
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
                previousPlayerMode = currentPlayerMode;                                
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

            raycastedFsms = raycastForScrollable()?.GetComponents<PlayMakerFSM>();

            if (raycastedFsms != null)
            {
                for (int i = 0; i < raycastedFsms.Length; i++)
                {
                    for (int j = 0; j < raycastedFsms[i].FsmStates.Length; j++)
                    {
                        activeState = raycastedFsms[i].FsmStates[j];
                        for (int n = 0; n < activeState.Actions.Length; n++)
                        {
                            if (activeState != null && activeState.Actions[n] is GetAxis)
                            {
                                if ((activeState.Actions[n] as GetAxis).axisName.Value == "Mouse ScrollWheel")
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Raycast for gameobjects on the included layers.
        /// </summary>
        private GameObject raycastForScrollable()
        {
            // Written, 31.05.2022

            if (Physics.Raycast(Camera.main.ScreenPointToRay(uInput.mousePosition), out RaycastHit hit, 1, raycastScrollableMask))
            {
                return hit.collider.gameObject;
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
        private void loadCurrentControlsToCInput()
        {
            // Written, 31.08.2018

            loadControlsToCInput(currentControls);
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
                        loadCurrentControlsToCInput();
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
                        loadCurrentControlsToCInput();
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
           
            setGameControl((PlayerModeEnum)changeInputResult.mode, changeInputResult.controlName, changeInputResult.index, inInput);
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
        /// Handles ffb on the xbox controller.
        /// </summary>
        private void handleFfbOnXboxController()
        {
            // Written, 16.10.2020

            if (MoControlsSaveData.loadedSaveData.ffbOnXboxController)
            {
                if (setFfbForVehicle())
                {
                    if (vehicle)
                    {
                        xboxController.setRumble(getFfbSetOpt());
                    }
                }
                else if (xboxController.prevRumblePow != XboxController.vector2Zero)
                {
                    xboxController.setRumble(XboxController.vector2Zero);
                }
            }
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
        private Vector2 wheelSlipBasedFfb()
        {
            // Written, 18.10.2020

            return new Vector2(drivetrain.poweredWheels[0].longitudinalSlip, drivetrain.poweredWheels[1].longitudinalSlip);
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
                ffbCurrentSupportedVehicleName = playerCurrentVehicle;
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
                if (vehicle != null)
                {
                    carDynamics = vehicle.GetComponent<CarDynamics>();
                    drivetrain = vehicle.GetComponent<Drivetrain>();
                }
            }
            return vehicle != null;
        }

        #endregion

        #region Event Handlers

        private void onPlayerModeChanged_loadControlsToCInput()
        {
            // Written, 05.08.2022

            loadCurrentControlsToCInput();
        }

        private void onPlayerModeChanged_deviceCheck()
        {
            // Written, 05.08.2022
            
            if (previousPlayerMode == PlayerModeEnum.InMenu)
            {
                setDevice();
            }
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
            // Written, 23.07.2022

            if (usingController)
            {
                controllerLookX.enabled = true;
                controllerLookY.enabled = true;
                if (!playerStopped)
                {
                    controllerMove.enabled = true;
                }
            }
            else
            {
                mouseLookX.enabled = true;
                mouseLookY.enabled = true;
            }
            MoControlsGO.mouseEmulator.enabled = false;
        }
        private void inMenu()
        {
            // Written, 23.07.2022

            mouseLookX.enabled = false;
            mouseLookY.enabled = false;
            controllerLookX.enabled = false;
            controllerLookY.enabled = false;
            controllerMove.enabled = false;

            MoControlsGO.mouseEmulator.enabled = true;
        }

        #endregion
    }
}
