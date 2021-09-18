using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using XInputDotNetPure;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents an xbox controller.
    /// </summary>
    public class XboxController : MonoBehaviour
    {
        // Written, 16.07.2018

        #region Fields

        internal Vector2 prevRumblePow;
        /// <summary>
        /// Represents if the user has request toolmode change via, <see cref="HoldInputMono"/>. see:<see cref="toggleToolMode"/>.
        /// </summary>
        internal bool _requestedModeChange;
        /// <summary>
        /// Represents the button prefix.
        /// </summary>
        private static readonly string buttonPrefix = "JoystickButton";
        /// <summary>
        /// Represents the axis prefix.
        /// </summary>
        private static readonly string axisPrefix = "Joy Axis ";
        /// <summary>
        /// Represents the normal (go) xbox controls.
        /// </summary>
        internal static readonly XboxControl[] normalXboxControls = new XboxControl[]
        {
                // Buttons
                new XboxBoolState(String.Format("{0}0", buttonPrefix), "A", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}1", buttonPrefix), "B", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}2", buttonPrefix), "X", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}3", buttonPrefix), "Y", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}4", buttonPrefix), "LB", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}5", buttonPrefix), "RB", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}6", buttonPrefix), "Back", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}7", buttonPrefix), "Start", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}8", buttonPrefix), "LS", XboxControlTypeEnum.Button),
                new XboxBoolState(String.Format("{0}9", buttonPrefix), "RS", XboxControlTypeEnum.Button),
                // D-Pad
                new XboxBoolState(String.Format("{0}7+", axisPrefix), "D-Pad Up", XboxControlTypeEnum.Axis),
                new XboxBoolState(String.Format("{0}7-", axisPrefix), "D-Pad Down", XboxControlTypeEnum.Axis),
                new XboxBoolState(String.Format("{0}6-", axisPrefix), "D-Pad Left", XboxControlTypeEnum.Axis),
                new XboxBoolState(String.Format("{0}6+", axisPrefix), "D-Pad Right", XboxControlTypeEnum.Axis),
                // Triggers
                new XboxFloatState(String.Format("{0}9+", axisPrefix), "LT"),
                new XboxFloatState(String.Format("{0}10+", axisPrefix), "RT"),
                // LS
                new XboxFloatState(String.Format("{0}1-", axisPrefix), "LS-Left"),
                new XboxFloatState(String.Format("{0}1+", axisPrefix), "LS-Right"),
                new XboxFloatState(String.Format("{0}2-", axisPrefix), "LS-Up"),
                new XboxFloatState(String.Format("{0}2+", axisPrefix), "LS-Down"),
                // RS
                new XboxFloatState(String.Format("{0}4-", axisPrefix), "RS-Left"),
                new XboxFloatState(String.Format("{0}4+", axisPrefix), "RS-Right"),
                new XboxFloatState(String.Format("{0}5-", axisPrefix), "RS-Up"),
                new XboxFloatState(String.Format("{0}5+", axisPrefix), "RS-Down")
        };

        #endregion

        #region Properties

        /// <summary>
        /// Represents all the controls of an xbox controller.
        /// </summary>
        private XboxControl[] xboxControls
        {
            get;
            set;
        }
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
                return gamePadIndex + 1;
            }
        }
        /// <summary>
        /// Represents the previous <see cref="GamePadState"/> of the controller.
        /// </summary>
        private GamePadState previousState
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the current <see cref="GamePadState"/> of the controller.
        /// </summary>
        private GamePadState state
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the gamepad index. Allowed values; 1, 2, 3, 4.
        /// </summary>
        private int gamePadIndex
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the index of the gamepad.
        /// </summary>
        private PlayerIndex playerIndex
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the rumble events for the xbox controller.
        /// </summary>
        private List<XboxRumble> xboxRumbleEvents
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the input map for the controller.
        /// </summary>
        private Dictionary<string, XboxControl> inputMap
        {
            get;
            set;
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="XboxController"/> and sets the index of the controller (1 - 4).
        /// </summary>
        /// <param name="inIndex">The controller index to set, 1 - 4.</param>
        public XboxController()
        {
            // Written, 16.07.2018

            gamePadIndex = 0;
            playerIndex = (PlayerIndex)gamePadIndex;
            xboxRumbleEvents = new List<XboxRumble>();
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

            loadControllerAssets();
            updateTriggerAxis();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates and sets the inputName (combined: 3-/3+ | not: 9+/10+) for the triggers.
        /// </summary>
        internal void updateTriggerAxis()
        {
            // Written, 24.10.2020

            if (MoControlsSaveData.loadedSaveData.combinedTriggerAxis)
            {
                xboxControls[14].setInputName(String.Format("{0}3+", axisPrefix));
                xboxControls[15].setInputName(String.Format("{0}3-", axisPrefix));
            }
            else
            {
                xboxControls[14].setInputName(String.Format("{0}9+", axisPrefix));
                xboxControls[15].setInputName(String.Format("{0}10+", axisPrefix));
            }
        }
        private void Update()
        {
            // Written, 23.10.2020


            if (_requestedModeChange)
            {
                MoControlsMod.print("Identified changemode request.", Debugging.DebugTypeEnum.full);
                MoControlsGO.controlManager.toggleToolMode();
                _requestedModeChange = false;
            }
            update();
        }
        private void LateUpdate()
        {
            // Written, 23.10.2020

            refresh();
        }
        /// <summary>
        /// Updates the controllers state as with the input map, and handles current rumbles.
        /// </summary>
        private void update()
        {
            // Written, 16.10.2020

            state = GamePad.GetState(playerIndex);

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

                updateInputMap();
            }
        }
        /// <summary>
        /// Refreshes the controllers previous state as with the input map.
        /// </summary>
        private void refresh()
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
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed.
        /// </summary>
        /// <param name="inXboxButton">The button to check</param>
        /// <returns></returns>
        internal bool getButtonPressed(XboxButtonEnum inXboxButton)
        {
            // Written, 16.07.2018

            if ((inputMap[inXboxButton.toString()] as XboxBoolState).state == ButtonState.Pressed)
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

            XboxBoolState button = inputMap[inXboxButton.toString()] as XboxBoolState;
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

            XboxBoolState button = inputMap[inXboxButton.toString()] as XboxBoolState;
            if (button.previousState == ButtonState.Pressed && button.state == ButtonState.Released)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// If xbox button has been pressed; returns that xbox button, else returns <see cref="XboxButtonEnum.NULL"/>.
        /// </summary>
        internal XboxButtonEnum getAnyButtonPressed()
        {
            // Written, 18.07.2018

            foreach (XboxButtonEnum xButton in Enum.GetValues(typeof(XboxButtonEnum)))
            {
                if (xButton != XboxButtonEnum.NULL)
                {
                    if (getButtonPressed(xButton))
                    {
                        return xButton;
                    }
                }
            }
            return XboxButtonEnum.NULL;
        }
        /// <summary>
        /// If xbox button has been pressed down; returns that xbox button, else returns <see cref="XboxButtonEnum.NULL"/>.
        /// </summary>
        internal XboxButtonEnum getAnyButtonDown()
        {
            // Written, 18.07.2018

            foreach (XboxButtonEnum xButton in Enum.GetValues(typeof(XboxButtonEnum)))
            {
                if (xButton != XboxButtonEnum.NULL)
                {
                    if (getButtonDown(xButton))
                    {
                        return xButton;
                    }
                }
            }
            return XboxButtonEnum.NULL;
        }
        /// <summary>
        /// Gets the left thumb sticks values.
        /// </summary>
        internal GamePadThumbSticks.StickValue getLeftStick()
        {
            // Written, 18.07.2018

            return state.ThumbSticks.Left;
        }
        /// <summary>
        /// Gets the right thumb sticks values.
        /// </summary>
        internal GamePadThumbSticks.StickValue getRightStick()
        {
            // Written, 18.07.2018

            return state.ThumbSticks.Right;
        }
        /// <summary>
        /// Gets the left trigger. Ranges from 0.0f (not pressed) to 1.0f (fully pressed).
        /// </summary>
        public float getLeftTrigger()
        {
            // Written, 18.07.2018

            return state.Triggers.Left;
        }
        /// <summary>
        /// Gets the right trigger. Ranges from 0.0f (not pressed) to 1.0f (fully pressed).
        /// </summary>
        internal float getRightTrigger()
        {
            // Written, 18.07.2018

            return state.Triggers.Right;
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
        /// Updates the input map.
        /// </summary>
        private void updateInputMap()
        {
            // Written, 16.07.2018 | Modified, 09.10.2020

            inputMap[a.name] = a;
            inputMap[b.name] = b;
            inputMap[x.name] = x;
            inputMap[y.name] = y;
            inputMap[dPadUp.name] = dPadUp;
            inputMap[dPadDown.name] = dPadDown;
            inputMap[dPadLeft.name] = dPadLeft;
            inputMap[dPadRight.name] = dPadRight;
            inputMap[back.name] = back;
            inputMap[start.name] = start;
            inputMap[lS.name] = lS;
            inputMap[rS.name] = rS;
            inputMap[lB.name] = lB;
            inputMap[rB.name] = rB;
            inputMap[leftThumbstick.up.name] = leftThumbstick.up;
            inputMap[leftThumbstick.down.name] = leftThumbstick.down;
            inputMap[leftThumbstick.left.name] = leftThumbstick.left;
            inputMap[leftThumbstick.right.name] = leftThumbstick.right;
            inputMap[rightThumbstick.up.name] = rightThumbstick.up;
            inputMap[rightThumbstick.down.name] = rightThumbstick.down;
            inputMap[rightThumbstick.left.name] = rightThumbstick.left;
            inputMap[rightThumbstick.right.name] = rightThumbstick.right;
        }
        /// <summary>
        /// Handles all current rumbles.
        /// </summary>
        internal void setRumble(Vector2 rumblePow)
        {
            // Written, 23.10.2020

            if (rumblePow != prevRumblePow)
            {
                GamePad.SetVibration(PlayerIndex.One, rumblePow.x, rumblePow.y);
                prevRumblePow = rumblePow;
            }
        }
        /// <summary>
        /// Loads control asset textures.
        /// </summary>
        private void loadControllerAssets()
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
                    MoControlsMod.print("Assets cannot be loaded to instance of xbox controller as assets ain't loaded.", Debugging.DebugTypeEnum.full);
            }
            catch (Exception ex)
            {
                MoControlsMod.print("An error occured while trying to load xbox controller assets.. Stacktrace:\r\n " + ex.StackTrace, Debugging.DebugTypeEnum.full);
            }
        }
        /// <summary>
        /// Gets an xbox control by input name.
        /// </summary>
        internal XboxControl getXboxControlByInputName(string inInputName)
        {
            // Written, 20.07.2018
            if (inInputName != KeyCode.None.ToString()) // no point enumerating..
            {
                foreach (XboxControl xboxControl in xboxControls)
                {
                    if (xboxControl.inputName == inInputName)
                    {
                        return xboxControl;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Returns all the xbox controls.
        /// </summary>
        internal XboxControl[] getXboxControls()
        {
            // Written, 22.07.2018

            return xboxControls;
        }

        #endregion
    }
}
