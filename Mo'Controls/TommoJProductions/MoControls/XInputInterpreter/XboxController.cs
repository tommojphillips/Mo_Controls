using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TommoJProductions.MoControls.InputEmulation;
using UnityEngine;
using XInputDotNetPure;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents an xbox controller.
    /// </summary>
    public class XboxController
    {
        // Written, 16.07.2018

        #region constants / readonly fields

        /// <summary>
        /// Represents the button prefix.
        /// </summary>
        private const string BUTTON_PREFIX = "JoystickButton";
        /// <summary>
        /// Represents the axis prefix.
        /// </summary>
        private const string AXIS_PREFIX = "Joy Axis ";

        /// <summary>
        /// Represents the normal (go) xbox controls.
        /// </summary>
        private static readonly XboxControl[] normalXboxControls = new XboxControl[]
        {
                // Buttons
                new XboxBoolState(String.Format("{0}0", BUTTON_PREFIX), "A"),
                new XboxBoolState(String.Format("{0}1", BUTTON_PREFIX), "B"),
                new XboxBoolState(String.Format("{0}2", BUTTON_PREFIX), "X"),
                new XboxBoolState(String.Format("{0}3", BUTTON_PREFIX), "Y"),
                new XboxBoolState(String.Format("{0}4", BUTTON_PREFIX), "LB"),
                new XboxBoolState(String.Format("{0}5", BUTTON_PREFIX), "RB"),
                new XboxBoolState(String.Format("{0}6", BUTTON_PREFIX), "Back"),
                new XboxBoolState(String.Format("{0}7", BUTTON_PREFIX), "Start"),
                new XboxBoolState(String.Format("{0}8", BUTTON_PREFIX), "LS"),
                new XboxBoolState(String.Format("{0}9", BUTTON_PREFIX), "RS"),
                // D-Pad
                new XboxBoolState(String.Format("{0}7+", AXIS_PREFIX), "D Pad Up", XboxControlTypeEnum.Axis),
                new XboxBoolState(String.Format("{0}7-", AXIS_PREFIX), "D Pad Down", XboxControlTypeEnum.Axis),
                new XboxBoolState(String.Format("{0}6-", AXIS_PREFIX), "D Pad Left", XboxControlTypeEnum.Axis),
                new XboxBoolState(String.Format("{0}6+", AXIS_PREFIX), "D Pad Right", XboxControlTypeEnum.Axis),
                // Triggers
                new XboxFloatState(String.Format("{0}9+", AXIS_PREFIX), "LT"),
                new XboxFloatState(String.Format("{0}10+", AXIS_PREFIX), "RT"),
                // LS
                new XboxFloatState(String.Format("{0}1-", AXIS_PREFIX), "LS Left"),
                new XboxFloatState(String.Format("{0}1+", AXIS_PREFIX), "LS Right"),
                new XboxFloatState(String.Format("{0}2-", AXIS_PREFIX), "LS Up"),
                new XboxFloatState(String.Format("{0}2+", AXIS_PREFIX), "LS Down"),
                // RS
                new XboxFloatState(String.Format("{0}4-", AXIS_PREFIX), "RS Left"),
                new XboxFloatState(String.Format("{0}4+", AXIS_PREFIX), "RS Right"),
                new XboxFloatState(String.Format("{0}5-", AXIS_PREFIX), "RS Up"),
                new XboxFloatState(String.Format("{0}5+", AXIS_PREFIX), "RS Down")
        };
        /// <summary>
        /// used for indexing.. cache
        /// </summary>
        private readonly static XboxBoolState emptyBoolState;
        /// <summary>
        /// used for indexing.. cache
        /// </summary>
        private readonly static XboxFloatState emptyFloatState;
        /// <summary>
        /// Represents <see cref="Vector3.zero"/>.. cache
        /// </summary>
        internal readonly static Vector2 vector2Zero;

        #endregion

        /// <summary>
        /// Represents an event that occurs when an xbox controller is connected.
        /// </summary>
        public static event Action<XboxController> connected;
        /// <summary>
        /// Represents an event that occurs when an xbox controller is disconnected.
        /// </summary>
        public static event Action<XboxController> disconnected;

        #region Fields

        /// <summary>
        /// the previous (last) rumble sent to xbox controller.
        /// </summary>
        internal Vector2 prevRumblePow;
        /// <summary>
        /// Represents current left stick state.
        /// </summary>
        private Vector2 leftThumbStickState;
        /// <summary>
        /// Represents current right stick state.
        /// </summary>
        private Vector2 rightThumbStickState;
        /// <summary>
        /// used in <see cref="getInputFromTypeRaw(InputTypeEnum)"/>. cache.
        /// </summary>
        private Vector2 inputFromType;

        /// <summary>
        /// Represents the previous <see cref="GamePadState"/> of the controller.
        /// </summary>
        private GamePadState previousState;
        /// <summary>
        /// Represents the current <see cref="GamePadState"/> of the controller.
        /// </summary>
        private GamePadState state;

        /// <summary>
        /// Represents the index of the gamepad.
        /// </summary>
        private PlayerIndex playerIndex;

        /// <summary>
        /// Represents the input map for the controller.
        /// </summary>
        private Dictionary<string, XboxControl> inputMap;

        /// <summary>
        /// Represents current any input state.
        /// </summary>
        private bool _anyInput;
        /// <summary>
        /// Represents current any stick input state.
        /// </summary>
        private bool _anyThumbstickInput;
        /// <summary>
        /// Represents current any button pressed state.
        /// </summary>
        private bool _anyButtonPressed;
        /// <summary>
        /// Represents current any button down state.
        /// </summary>
        private bool _anyButtonDown;

        #endregion

        #region Properties

        /// <summary>
        /// Represents whether the controller is connect or not.
        /// </summary>
        internal bool isConnected
        {
            get
            {
                return state.IsConnected;
            }
        }
        /// <summary>
        /// Represents the index of the controller (1 - 4).
        /// </summary>
        internal int index
        {
            get
            {
                return (int)playerIndex + 1;
            }
        }
        /// <summary>
        /// Represents the A button on the xbox controller.
        /// </summary>
        public XboxBoolState a
        {
            get
            {
                return (XboxBoolState)xboxControls[0];
            }
        }
        /// <summary>
        /// Represents the B button on the xbox controller.
        /// </summary>
        public XboxBoolState b
        {
            get
            {
                return (XboxBoolState)xboxControls[1];
            }
        }
        /// <summary>
        /// Represents the X button on the xbox controller.
        /// </summary>
        public XboxBoolState x
        {
            get
            {
                return (XboxBoolState)xboxControls[2];
            }
        }
        /// <summary>
        /// Represents the Y button on the xbox controller.
        /// </summary>
        public XboxBoolState y
        {
            get
            {
                return (XboxBoolState)xboxControls[3];
            }
        }
        /// <summary>
        /// Represents the D-Pad Up button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadUp
        {
            get
            {
                return (XboxBoolState)xboxControls[10];
            }
        }
        /// <summary>
        /// Represents the D-Pad Down button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadDown
        {
            get
            {
                return (XboxBoolState)xboxControls[11];
            }
        }
        /// <summary>
        /// Represents the D-Pad Left button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadLeft
        {
            get
            {
                return (XboxBoolState)xboxControls[12];
            }
        }
        /// <summary>
        /// Represents the D-Pad Right button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadRight
        {
            get
            {
                return (XboxBoolState)xboxControls[13];
            }
        }
        /// <summary>
        /// Represents the Start button on the xbox controller.
        /// </summary>
        public XboxBoolState start
        {
            get
            {
                return (XboxBoolState)xboxControls[7];
            }
        }
        /// <summary>
        /// Represents the Back button on the xbox controller.
        /// </summary>
        public XboxBoolState back
        {
            get
            {
                return (XboxBoolState)xboxControls[6];
            }
        }
        /// <summary>
        /// Represents the Left-Stick button on the xbox controller.
        /// </summary>
        public XboxBoolState lS
        {
            get
            {
                return (XboxBoolState)xboxControls[8];
            }
        }
        /// <summary>
        /// Represents the Right-Stick button on the xbox controller.
        /// </summary>
        public XboxBoolState rS
        {
            get
            {
                return (XboxBoolState)xboxControls[9];
            }
        }
        /// <summary>
        /// Represents the Left-Bumper button on the xbox controller.
        /// </summary>
        public XboxBoolState lB
        {
            get
            {
                return (XboxBoolState)xboxControls[4];
            }
        }
        /// <summary>
        /// Represents the Right-Bumper button on the xbox controller.
        /// </summary>
        public XboxBoolState rB
        {
            get
            {
                return (XboxBoolState)xboxControls[5];
            }
        }
        /// <summary>
        /// Represents the Left Trigger on the xbox controller
        /// </summary>
        public XboxFloatState lT
        {
            get
            {
                return (XboxFloatState)xboxControls[14];
            }
        }
        /// <summary>
        /// Represents the Right Trigger on the xbox controller.
        /// </summary>
        public XboxFloatState rT
        {
            get
            {
                return (XboxFloatState)xboxControls[15];
            }
        }
        /// <summary>
        /// Represents the left thumbstick on the xbox controller.
        /// </summary>
        public XboxJoystick leftThumbstick
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the right thumbstick on the xbox controller.
        /// </summary>
        public XboxJoystick rightThumbstick
        {
            get;
            private set;
        }
        /// <summary>
        /// represents if controller has detected any input from this controller.
        /// </summary>
        public bool anyInput { get; private set; }
        /// <summary>
        /// represents if any thumbstick has input.
        /// </summary>
        public bool anyThumbstickInput { get; private set; }
        /// <summary>
        /// represents if any button has been pressed this frame.
        /// </summary>
        public bool anyButtonPressed { get; private set; }
        /// <summary>
        /// Represents if any button is down.
        /// </summary>
        public bool anyButtonDown { get; private set; }
        /// <summary>
        /// Represents the last detected input from this controller.
        /// </summary>
        internal XboxControl lastControlInput { get; private set; }
        /// <summary>
        /// Represents all dpad controls (left, right up and down).
        /// </summary>
        internal XboxBoolState[] dPadControls { get; private set; }
        /// <summary>
        /// Represents all the controls of an xbox controller.
        /// </summary>
        internal XboxControl[] xboxControls { get; private set; }

        #endregion

        #region Constructors

        static XboxController() 
        {
            // Written, 05.08.2022

            vector2Zero = Vector2.zero;
            emptyBoolState = new XboxBoolState(string.Empty, string.Empty);
            emptyFloatState = new XboxFloatState(string.Empty, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="XboxController"/> and sets the index of the controller.
        /// </summary>
        public XboxController()
        {
            // Written, 16.07.2018

            playerIndex = PlayerIndex.One;
            inputMap = new Dictionary<string, XboxControl>();

            #region Initilize Xbox Buttons

            xboxControls = normalXboxControls;
            // Thumbsticks
            leftThumbstick = new XboxJoystick(
                xboxControls[16] as XboxFloatState,
                xboxControls[17] as XboxFloatState,
                xboxControls[18] as XboxFloatState,
                xboxControls[19] as XboxFloatState);
            rightThumbstick = new XboxJoystick(
                xboxControls[20] as XboxFloatState,
                xboxControls[21] as XboxFloatState,
                xboxControls[22] as XboxFloatState,
                xboxControls[23] as XboxFloatState);

            #endregion
            
            loadAssetsToController();
            updateTriggerAxisInputName();

            leftThumbStickState = vector2Zero;
            rightThumbStickState = vector2Zero;

            dPadControls = new XboxBoolState[4]
            {
                dPadUp,
                dPadDown,
                dPadLeft,
                dPadRight
            };
        }

        [IndexerName("xboxControl")]
        public XboxBoolState this[XboxButtonEnum xboxButton]
        {
            get
            {
                string key = xboxButton.toString();
                if (inputMap.ContainsKey(key))
                    return (XboxBoolState)inputMap[key];
                return emptyBoolState;
            }
        }
        [IndexerName("xboxControl")]
        public XboxFloatState this[XboxAxisEnum xboxAxis]
        {
            get
            {
                string key = xboxAxis.toString();
                if (inputMap.ContainsKey(key))
                    return (XboxFloatState)inputMap[key];
                return emptyFloatState;
            }
        }

        #endregion

        #region Methods

        private bool prevConnection;
        private void connectionChangedCheck() 
        {
            // Written, 13.08.2022

            if (prevConnection != isConnected)
            {
                prevConnection = isConnected;

                if (prevConnection)
                {
                    connected?.Invoke(this);
                }
                else
                {
                    disconnected?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Updates the input map.
        /// </summary>
        private void updateInputMap()
        {
            // Written, 16.07.2018 | Modified, 09.10.2020 | Modified, 02.06.2022

            _anyInput = false;
            _anyButtonPressed = false;
            _anyButtonDown = false;
            _anyThumbstickInput = false;
            for (int i = 0; i < xboxControls.Length; i++)
            {
                inputMap[xboxControls[i].name] = xboxControls[i];

                if (xboxControls[i] is XboxBoolState)
                {
                    XboxBoolState bs = xboxControls[i] as XboxBoolState;
                    if (bs.state == ButtonState.Pressed)
                    {
                        _anyInput = true;
                        _anyButtonPressed = true;
                        if (bs.previousState == ButtonState.Released)
                            _anyButtonDown = true;
                        lastControlInput = xboxControls[i];
                        continue;
                    }
                }
                else if ((xboxControls[i] as XboxFloatState).state != 0)
                {
                    _anyInput = true;
                    _anyThumbstickInput = true;
                    lastControlInput = xboxControls[i];
                    continue;
                }
            }
            anyInput = _anyInput;
            anyButtonPressed = _anyButtonPressed;
            anyButtonDown = _anyButtonDown;
            anyThumbstickInput = _anyThumbstickInput;
        }
        /// <summary>
        /// Loads control asset textures.
        /// </summary>
        private void loadAssetsToController()
        {
            // Written, 20.07.2018

            try
            {
                if (MoControlsMod.assetsLoaded)
                {
                    a.texture = MoControlsMod.assets.a;
                    b.texture = MoControlsMod.assets.b;
                    x.texture = MoControlsMod.assets.x;
                    y.texture = MoControlsMod.assets.y;
                    start.texture = MoControlsMod.assets.start;
                    back.texture = MoControlsMod.assets.back;

                    lS.texture = MoControlsMod.assets.ls;
                    rS.texture = MoControlsMod.assets.rs;

                    lT.texture = MoControlsMod.assets.lt;
                    rT.texture = MoControlsMod.assets.rt;

                    rB.texture = MoControlsMod.assets.rb;
                    lB.texture = MoControlsMod.assets.lb;

                    dPadUp.texture = MoControlsMod.assets.dpup;
                    dPadDown.texture = MoControlsMod.assets.dpdown;
                    dPadLeft.texture = MoControlsMod.assets.dpleft;
                    dPadRight.texture = MoControlsMod.assets.dpright;

                    xboxControls[16].texture = MoControlsMod.assets.lsleft;
                    xboxControls[17].texture = MoControlsMod.assets.lsright;
                    xboxControls[18].texture = MoControlsMod.assets.lsup;
                    xboxControls[19].texture = MoControlsMod.assets.lsdown;

                    xboxControls[20].texture = MoControlsMod.assets.rsleft;
                    xboxControls[21].texture = MoControlsMod.assets.rsright;
                    xboxControls[22].texture = MoControlsMod.assets.rsup;
                    xboxControls[23].texture = MoControlsMod.assets.rsdown;
                }
                else
                    MoControlsMod.print("Assets arent loaded..", Debugging.DebugTypeEnum.full);
            }
            catch (Exception ex)
            {
                MoControlsMod.print("An error occured while trying to load xbox controller assets.. Stacktrace:\r\n " + ex.StackTrace, Debugging.DebugTypeEnum.full);
            }
        }

        /// <summary>
        /// Updates the controllers state as with the input map, and handles current rumbles.
        /// </summary>
        internal void update()
        {
            // Written, 16.10.2020

            state = GamePad.GetState(playerIndex, MoControlsSaveData.loadedSaveData.xboxControllerDeadzoneType);

            connectionChangedCheck();

            if (state.IsConnected)
            {
                // Face Buttons
                a.state = state.Buttons.A;
                b.state = state.Buttons.B;
                x.state = state.Buttons.X;
                y.state = state.Buttons.Y;
                start.state = state.Buttons.Start;
                back.state = state.Buttons.Back;
                // D-Pad
                dPadUp.state = state.DPad.Up;
                dPadDown.state = state.DPad.Down;
                dPadLeft.state = state.DPad.Left;
                dPadRight.state = state.DPad.Right;
                // Other
                lB.state = state.Buttons.LeftShoulder;
                rB.state = state.Buttons.RightShoulder;
                lS.state = state.Buttons.LeftStick;
                rS.state = state.Buttons.RightStick;
                // Triggers
                lT.state = state.Triggers.Left;
                rT.state = state.Triggers.Right;
                // Sticks
                leftThumbstick.right.state = state.ThumbSticks.Left.X > 0 ? state.ThumbSticks.Left.X : 0;
                leftThumbstick.left.state = state.ThumbSticks.Left.X < 0 ? state.ThumbSticks.Left.X : 0;
                leftThumbstick.up.state = state.ThumbSticks.Left.Y > 0 ? state.ThumbSticks.Left.Y : 0;
                leftThumbstick.down.state = state.ThumbSticks.Left.Y < 0 ? state.ThumbSticks.Left.Y : 0;
                // Sticks
                rightThumbstick.right.state = state.ThumbSticks.Right.X > 0 ? state.ThumbSticks.Right.X : 0;
                rightThumbstick.left.state = state.ThumbSticks.Right.X < 0 ? state.ThumbSticks.Right.X : 0;
                rightThumbstick.up.state = state.ThumbSticks.Right.Y > 0 ? state.ThumbSticks.Right.Y : 0;
                rightThumbstick.down.state = state.ThumbSticks.Right.Y < 0 ? state.ThumbSticks.Right.Y : 0;

                leftThumbStickState.x = state.ThumbSticks.Left.X;
                leftThumbStickState.y = state.ThumbSticks.Left.Y;

                rightThumbStickState.x = state.ThumbSticks.Right.X;
                rightThumbStickState.y = state.ThumbSticks.Right.Y;

                updateInputMap();
            }
            else if (anyInput)
            {
                anyInput = false;
                anyButtonDown = false;
                anyButtonPressed = false;
                anyThumbstickInput = false;
            }
        }
        /// <summary>
        /// Refreshes the controllers previous state as with the input map.
        /// </summary>
        internal void refresh()
        {
            // Written, 16.07.2018 | Modified, 09.10.2020

            previousState = state;

            if (state.IsConnected)
            {
                // Face Buttons
                a.previousState = previousState.Buttons.A;
                b.previousState = previousState.Buttons.B;
                x.previousState = previousState.Buttons.X;
                y.previousState = previousState.Buttons.Y;
                start.previousState = previousState.Buttons.Start;
                back.previousState = previousState.Buttons.Back;
                // D-Pad
                dPadUp.previousState = previousState.DPad.Up;
                dPadDown.previousState = previousState.DPad.Down;
                dPadLeft.previousState = previousState.DPad.Left;
                dPadRight.previousState = previousState.DPad.Right;
                // Other
                lB.previousState = previousState.Buttons.LeftShoulder;
                rB.previousState = previousState.Buttons.RightShoulder;
                lS.previousState = previousState.Buttons.LeftStick;
                rS.previousState = previousState.Buttons.RightStick;
                // Triggers
                lT.previousState = previousState.Triggers.Left;
                rT.previousState = previousState.Triggers.Right;
                // Sticks
                leftThumbstick.right.previousState = previousState.ThumbSticks.Left.X > 0 ? previousState.ThumbSticks.Left.X : 0;
                leftThumbstick.left.previousState = previousState.ThumbSticks.Left.X < 0 ? previousState.ThumbSticks.Left.X : 0;
                leftThumbstick.up.previousState = previousState.ThumbSticks.Left.Y > 0 ? previousState.ThumbSticks.Left.Y : 0;
                leftThumbstick.down.previousState = previousState.ThumbSticks.Left.Y < 0 ? previousState.ThumbSticks.Left.Y : 0;
                // Sticks
                rightThumbstick.right.previousState = previousState.ThumbSticks.Right.X > 0 ? previousState.ThumbSticks.Right.X : 0;
                rightThumbstick.left.previousState = previousState.ThumbSticks.Right.X < 0 ? previousState.ThumbSticks.Right.X : 0;
                rightThumbstick.up.previousState = previousState.ThumbSticks.Right.Y > 0 ? previousState.ThumbSticks.Right.Y : 0;
                rightThumbstick.down.previousState = previousState.ThumbSticks.Right.Y < 0 ? previousState.ThumbSticks.Right.Y : 0;

                updateInputMap();
            }
        }

        /// <summary>
        /// Updates and sets the inputName (combined: 3-/3+ | not: 9+/10+) for the triggers.
        /// </summary>
        internal void updateTriggerAxisInputName()
        {
            // Written, 24.10.2020

            if (MoControlsSaveData.loadedSaveData.combinedTriggerAxis)
            {
                xboxControls[14].setInputName(String.Format("{0}3+", AXIS_PREFIX));
                xboxControls[15].setInputName(String.Format("{0}3-", AXIS_PREFIX));
            }
            else
            {
                xboxControls[14].setInputName(String.Format("{0}9+", AXIS_PREFIX));
                xboxControls[15].setInputName(String.Format("{0}10+", AXIS_PREFIX));
            }
        }

        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed.
        /// </summary>
        /// <param name="inXboxButton">The button to check</param>
        /// <returns></returns>
        internal bool getButtonPressed(XboxButtonEnum inXboxButton)
        {
            // Written, 16.07.2018

            if (this[inXboxButton].state == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed and held down.
        /// </summary>
        /// <param name="inXboxButton">The button to check.</param>
        internal bool getButtonDown(XboxButtonEnum inXboxButton)
        {
            // Written, 16.07.2018

            XboxBoolState button = this[inXboxButton];
            if (button.previousState == ButtonState.Released && button.state == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed and released.
        /// </summary>
        /// <param name="inXboxButton">The button to check.</param>
        internal bool getButtonUp(XboxButtonEnum inXboxButton)
        {
            // Written, 20.12.2018

            XboxBoolState button = this[inXboxButton];
            if (button.previousState == ButtonState.Pressed && button.state == ButtonState.Released)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the left thumb sticks values.
        /// </summary>
        internal Vector2 getLeftStick()
        {
            // Written, 18.07.2018

            return leftThumbStickState;
        }
        /// <summary>
        /// Gets the right thumb sticks values.
        /// </summary>
        internal Vector2 getRightStick()
        {
            // Written, 18.07.2018

            return rightThumbStickState;
        }

        /// <summary>
        /// Returns true if the left trigger was tapped.
        /// </summary>
        internal bool getLeftTriggerTap()
        {
            // Written, 18.07.2018

            if (lT.previousState == 0f && lT.state >= 0.1f)
                return true;
            return false;
        }
        /// <summary>
        /// Returns true of the right trigger was tapped.
        /// </summary>
        internal bool getRightTriggerTap()
        {
            // Written, 18.07.2018

            if (rT.previousState == 0f && rT.state >= 0.1f)
                return true;
            return false;
        }        

        /// <summary>
        /// sends a rumble effect to the xbox controller. of <see cref="playerIndex"/> (0 - 3)
        /// </summary>
        internal void setRumble(Vector2 rumblePow)
        {
            // Written, 23.10.2020

            if (rumblePow != prevRumblePow)
            {
                GamePad.SetVibration(playerIndex, rumblePow.x, rumblePow.y);
                prevRumblePow = rumblePow;
            }
        }
        
        /// <summary>
        /// Gets an xbox control by input name.
        /// </summary>
        internal XboxControl getXboxControlByInputName(string inInputName)
        {
            // Written, 20.07.2018

            foreach (XboxControl xboxControl in xboxControls)
            {
                if (xboxControl.inputName == inInputName)
                {
                    return xboxControl;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets raw input from selected input type, ls, rs or dp.
        /// </summary>
        /// <param name="inputType">the input type to get.</param>
        internal Vector2 getInputFromTypeRaw(InputTypeEnum inputType)
        {
            // Written, 24.07.2022
                        
            switch (inputType)
            {
                case InputTypeEnum.LS:
                    inputFromType = getLeftStick();
                    break;
                case InputTypeEnum.RS:
                    inputFromType = getRightStick();
                    break;
                case InputTypeEnum.DPad:
                    inputFromType = vector2Zero;
                    if (dPadLeft.state == ButtonState.Pressed)
                    {
                        inputFromType.x = -1;
                    }
                    else if (dPadRight.state == ButtonState.Pressed)
                    {
                        inputFromType.x = 1;
                    }
                    if (dPadUp.state == ButtonState.Pressed)
                    {
                        inputFromType.y = 1;
                    }
                    else if (dPadDown.state == ButtonState.Pressed)
                    {
                        inputFromType.y = -1;
                    }
                    break;
                default:
                    inputFromType = vector2Zero;
                    break;
            }
            return inputFromType;
        }

        #endregion
    }
}
