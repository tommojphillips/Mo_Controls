using System;
using System.Collections;
using System.Collections.Generic;
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

        #region Fields

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
        /// <summary>
        /// Represents the max number of events that can be processed per frame.
        /// </summary>
        private const int MAX_RUMBLE_EVENTS_PER_FRAME = 5;

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
        public bool isConnected
        {
            get
            {
                return this.state.IsConnected;
            }
        }
        /// <summary>
        /// Represents the index of the controller (1 - 4).
        /// </summary>
        public int index
        {
            get
            {
                return this.gamePadIndex + 1;
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
                return (XboxBoolState)this.xboxControls[0];
            }
        }
        /// <summary>
        /// Represents the B button on the xbox controller.
        /// </summary>
        public XboxBoolState b
        {
            get
            {
                return (XboxBoolState)this.xboxControls[1];
            }
        }
        /// <summary>
        /// Represents the X button on the xbox controller.
        /// </summary>
        public XboxBoolState x
        {
            get
            {
                return (XboxBoolState)this.xboxControls[2];
            }
        }
        /// <summary>
        /// Represents the Y button on the xbox controller.
        /// </summary>
        public XboxBoolState y
        {
            get
            {
                return (XboxBoolState)this.xboxControls[3];
            }
        }
        /// <summary>
        /// Represents the D-Pad Up button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadUp
        {
            get
            {
                return (XboxBoolState)this.xboxControls[10];
            }
        }
        /// <summary>
        /// Represents the D-Pad Down button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadDown
        {
            get
            {
                return (XboxBoolState)this.xboxControls[11];
            }
        }
        /// <summary>
        /// Represents the D-Pad Left button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadLeft
        {
            get
            {
                return (XboxBoolState)this.xboxControls[12];
            }
        }
        /// <summary>
        /// Represents the D-Pad Right button on the xbox controller.
        /// </summary>
        public XboxBoolState dPadRight
        {
            get
            {
                return (XboxBoolState)this.xboxControls[13];
            }
        }
        /// <summary>
        /// Represents the Start button on the xbox controller.
        /// </summary>
        public XboxBoolState start
        {
            get
            {
                return (XboxBoolState)this.xboxControls[7];
            }
        }
        /// <summary>
        /// Represents the Back button on the xbox controller.
        /// </summary>
        public XboxBoolState back
        {
            get
            {
                return (XboxBoolState)this.xboxControls[6];
            }
        }
        /// <summary>
        /// Represents the Left-Stick button on the xbox controller.
        /// </summary>
        public XboxBoolState lS
        {
            get
            {
                return (XboxBoolState)this.xboxControls[8];
            }
        }
        /// <summary>
        /// Represents the Right-Stick button on the xbox controller.
        /// </summary>
        public XboxBoolState rS
        {
            get
            {
                return (XboxBoolState)this.xboxControls[9];
            }
        }
        /// <summary>
        /// Represents the Left-Bumper button on the xbox controller.
        /// </summary>
        public XboxBoolState lB
        {
            get
            {
                return (XboxBoolState)this.xboxControls[4];
            }
        }
        /// <summary>
        /// Represents the Right-Bumper button on the xbox controller.
        /// </summary>
        public XboxBoolState rB
        {
            get
            {
                return (XboxBoolState)this.xboxControls[5];
            }
        }
        /// <summary>
        /// Represents the Left Trigger on the xbox controller
        /// </summary>
        public XboxFloatState lT
        {
            get
            {
                return (XboxFloatState)this.xboxControls[14];
            }
        }
        /// <summary>
        /// Represents the Right Trigger on the xbox controller.
        /// </summary>
        public XboxFloatState rT
        {
            get
            {
                return (XboxFloatState)this.xboxControls[15];
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
        public XboxController(int inIndex)
        {
            // Written, 16.07.2018

            this.gamePadIndex = inIndex - 1;
            this.playerIndex = (PlayerIndex)this.gamePadIndex;
            this.xboxRumbleEvents = new List<XboxRumble>();
            this.inputMap = new Dictionary<string, XboxControl>();

            #region Initilize Xbox Buttons

            this.xboxControls = normalXboxControls;
            // Thumbsticks
            this.leftThumbstick = new XboxJoystick(
                this.xboxControls[16] as XboxFloatState,
                this.xboxControls[17] as XboxFloatState,
                this.xboxControls[18] as XboxFloatState,
                this.xboxControls[19] as XboxFloatState);
            this.rightThumbstick = new XboxJoystick(
                this.xboxControls[20] as XboxFloatState,
                this.xboxControls[21] as XboxFloatState,
                this.xboxControls[22] as XboxFloatState,
                this.xboxControls[23] as XboxFloatState);

            #endregion            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the controllers state as with the input map, and handles current rumbles.
        /// </summary>
        internal IEnumerator updateStateCoroutine()
        {
            // Written, 16.10.2020

            this.state = GamePad.GetState(this.playerIndex);

            if (this.state.IsConnected)
            {
                // Face Buttons
                this.a.state = this.state.Buttons.A;
                this.b.state = this.state.Buttons.B;
                this.x.state = this.state.Buttons.X;
                this.y.state = this.state.Buttons.Y;
                this.start.state = this.state.Buttons.Start;
                this.back.state = this.state.Buttons.Back;
                // D-Pad
                this.dPadUp.state = this.state.DPad.Up;
                this.dPadDown.state = this.state.DPad.Down;
                this.dPadLeft.state = this.state.DPad.Left;
                this.dPadRight.state = this.state.DPad.Right;
                // Other
                this.lB.state = this.state.Buttons.LeftShoulder;
                this.rB.state = this.state.Buttons.RightShoulder;
                this.lS.state = this.state.Buttons.LeftStick;
                this.rS.state = this.state.Buttons.RightStick;
                // Triggers
                this.lT.state = this.state.Triggers.Left;
                this.rT.state = this.state.Triggers.Right;
                // Sticks
                this.leftThumbstick.right.state = this.state.ThumbSticks.Left.X > 0 ? this.state.ThumbSticks.Left.X : 0;
                this.leftThumbstick.left.state = this.state.ThumbSticks.Left.X < 0 ? this.state.ThumbSticks.Left.X : 0;
                this.leftThumbstick.up.state = this.state.ThumbSticks.Left.Y > 0 ? this.state.ThumbSticks.Left.Y : 0;
                this.leftThumbstick.down.state = this.state.ThumbSticks.Left.Y < 0 ? this.state.ThumbSticks.Left.Y : 0;
                // Sticks
                this.rightThumbstick.right.state = this.state.ThumbSticks.Right.X > 0 ? this.state.ThumbSticks.Right.X : 0;
                this.rightThumbstick.left.state = this.state.ThumbSticks.Right.X < 0 ? this.state.ThumbSticks.Right.X : 0;
                this.rightThumbstick.up.state = this.state.ThumbSticks.Right.Y > 0 ? this.state.ThumbSticks.Right.Y : 0;
                this.rightThumbstick.down.state = this.state.ThumbSticks.Right.Y < 0 ? this.state.ThumbSticks.Right.Y : 0;

                this.updateInputMap();
            }
            yield return null;
        }
        /// <summary>
        /// Refreshes the controllers previous state as with the input map.
        /// </summary>
        public void refresh()
        {
            // Written, 16.07.2018 | Modified, 09.10.2020

            this.previousState = this.state;

            if (this.state.IsConnected)
            {
                // Face Buttons
                this.a.previousState = this.previousState.Buttons.A;
                this.b.previousState = this.previousState.Buttons.B;
                this.x.previousState = this.previousState.Buttons.X;
                this.y.previousState = this.previousState.Buttons.Y;
                this.start.previousState = this.previousState.Buttons.Start;
                this.back.previousState = this.previousState.Buttons.Back;
                // D-Pad
                this.dPadUp.previousState = this.previousState.DPad.Up;
                this.dPadDown.previousState = this.previousState.DPad.Down;
                this.dPadLeft.previousState = this.previousState.DPad.Left;
                this.dPadRight.previousState = this.previousState.DPad.Right;
                // Other
                this.lB.previousState = this.previousState.Buttons.LeftShoulder;
                this.rB.previousState = this.previousState.Buttons.RightShoulder;
                this.lS.previousState = this.previousState.Buttons.LeftStick;
                this.rS.previousState = this.previousState.Buttons.RightStick;
                // Triggers
                this.lT.previousState = this.previousState.Triggers.Left;
                this.rT.previousState = this.previousState.Triggers.Right;
                // Sticks
                this.leftThumbstick.right.previousState = this.previousState.ThumbSticks.Left.X > 0 ? this.previousState.ThumbSticks.Left.X : 0;
                this.leftThumbstick.left.previousState = this.previousState.ThumbSticks.Left.X < 0 ? this.previousState.ThumbSticks.Left.X : 0;
                this.leftThumbstick.up.previousState = this.previousState.ThumbSticks.Left.Y > 0 ? this.previousState.ThumbSticks.Left.Y : 0;
                this.leftThumbstick.down.previousState = this.previousState.ThumbSticks.Left.Y < 0 ? this.previousState.ThumbSticks.Left.Y : 0;
                // Sticks
                this.rightThumbstick.right.previousState = this.previousState.ThumbSticks.Right.X > 0 ? this.previousState.ThumbSticks.Right.X : 0;
                this.rightThumbstick.left.previousState = this.previousState.ThumbSticks.Right.X < 0 ? this.previousState.ThumbSticks.Right.X : 0;
                this.rightThumbstick.up.previousState = this.previousState.ThumbSticks.Right.Y > 0 ? this.previousState.ThumbSticks.Right.Y : 0;
                this.rightThumbstick.down.previousState = this.previousState.ThumbSticks.Right.Y < 0 ? this.previousState.ThumbSticks.Right.Y : 0;

                this.updateInputMap();
            }
        }
        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed.
        /// </summary>
        /// <param name="inXboxButton">The button to check</param>
        /// <returns></returns>
        public bool getButtonPressed(XboxButtonEnum inXboxButton)
        {
            // Written, 16.07.2018

            if ((this.inputMap[inXboxButton.toString()] as XboxBoolState).state == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed and held down.
        /// </summary>
        /// <param name="inXboxButton">The button to check.</param>
        public bool getButtonDown(XboxButtonEnum inXboxButton)
        {
            // Written, 16.07.2018

            XboxBoolState button = this.inputMap[inXboxButton.toString()] as XboxBoolState;
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
        public bool getButtonUp(XboxButtonEnum inXboxButton)
        {
            // Written, 20.12.2018

            XboxBoolState button = this.inputMap[inXboxButton.toString()] as XboxBoolState;
            if (button.previousState == ButtonState.Pressed && button.state == ButtonState.Released)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// If xbox button has been pressed; returns that xbox button, else returns <see cref="XboxButtonEnum.NULL"/>.
        /// </summary>
        public XboxButtonEnum getAnyButtonPressed()
        {
            // Written, 18.07.2018

            foreach (XboxButtonEnum xButton in Enum.GetValues(typeof(XboxButtonEnum)))
            {
                if (xButton != XboxButtonEnum.NULL)
                {
                    if (this.getButtonPressed(xButton))
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
        public XboxButtonEnum getAnyButtonDown()
        {
            // Written, 18.07.2018

            foreach (XboxButtonEnum xButton in Enum.GetValues(typeof(XboxButtonEnum)))
            {
                if (xButton != XboxButtonEnum.NULL)
                {
                    if (this.getButtonDown(xButton))
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
        public GamePadThumbSticks.StickValue getLeftStick()
        {
            // Written, 18.07.2018

            return this.state.ThumbSticks.Left;
        }
        /// <summary>
        /// Gets the right thumb sticks values.
        /// </summary>
        public GamePadThumbSticks.StickValue getRightStick()
        {
            // Written, 18.07.2018

            return this.state.ThumbSticks.Right;
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
        public float getRightTrigger()
        {
            // Written, 18.07.2018

            return state.Triggers.Right;
        }
        /// <summary>
        /// Returns true if the left trigger was tapped.
        /// </summary>
        public bool getLeftTriggerTap()
        {
            // Written, 18.07.2018

            if (this.lT.previousState == 0f && this.lT.state >= 0.1f)
                return true;
            return false;
        }
        /// <summary>
        /// Returns true of the right trigger was tapped.
        /// </summary>
        public bool getRightTriggerTap()
        {
            // Written, 18.07.2018

            if (this.rT.previousState == 0f && this.rT.state >= 0.1f)
                return true;
            return false;
        }
        /// <summary>
        /// Adds a rumble to the xbox controller.
        /// </summary>
        /// <param name="inTimer">The time in seconds to rumble for.</param>
        /// <param name="inPower">The power of the rumble.</param>
        /// <param name="inDuration">The duration until the fade-time comes.</param>
        public void addRumble(float inTimer, Vector2 inPower, float inDuration)
        {
            // Written, 16.07.2018

            XboxRumble xboxRumble = new XboxRumble()
            {
                timer = inTimer,
                power = inPower,
                duration = inDuration,
            };
            this.addRumble(xboxRumble);
        }
        /// <summary>
        /// Adds the provided rumble to the xbox controller.
        /// </summary>
        /// <param name="inXboxRumble">The rumble to add.</param>
        public void addRumble(XboxRumble inXboxRumble)
        {
            // Written, 16.07.2018

            this.xboxRumbleEvents.Add(inXboxRumble);
        }
        /// <summary>
        /// Updates the input map.
        /// </summary>
        private void updateInputMap()
        {
            // Written, 16.07.2018 | Modified, 09.10.2020

            this.inputMap[a.name] = a;
            this.inputMap[b.name] = b;
            this.inputMap[x.name] = x;
            this.inputMap[y.name] = y;
            this.inputMap[dPadUp.name] = dPadUp;
            this.inputMap[dPadDown.name] = dPadDown;
            this.inputMap[dPadLeft.name] = dPadLeft;
            this.inputMap[dPadRight.name] = dPadRight;
            this.inputMap[back.name] = back;
            this.inputMap[start.name] = start;
            this.inputMap[lS.name] = lS;
            this.inputMap[rS.name] = rS;
            this.inputMap[lB.name] = lB;
            this.inputMap[rB.name] = rB;
            this.inputMap[leftThumbstick.up.name] = this.leftThumbstick.up;
            this.inputMap[leftThumbstick.down.name] = this.leftThumbstick.down;
            this.inputMap[leftThumbstick.left.name] = this.leftThumbstick.left;
            this.inputMap[leftThumbstick.right.name] = this.leftThumbstick.right;
            this.inputMap[rightThumbstick.up.name] = this.rightThumbstick.up;
            this.inputMap[rightThumbstick.down.name] = this.rightThumbstick.down;
            this.inputMap[rightThumbstick.left.name] = this.rightThumbstick.left;
            this.inputMap[rightThumbstick.right.name] = this.rightThumbstick.right;
        }
        /// <summary>
        /// Handles all current rumbles.
        /// </summary>
        internal IEnumerator handleRumbleCoroutine()
        {
            // Written, 16.07.2018

            if (this.xboxRumbleEvents.Count > 0)
            {
                Vector2 currentPower = new Vector2(0.0f, 0.0f);
                for (int i = 0; i < this.xboxRumbleEvents.Count; i++)
                {
                    if (this.xboxRumbleEvents.Count >= MAX_RUMBLE_EVENTS_PER_FRAME)
                        if (i == MAX_RUMBLE_EVENTS_PER_FRAME)
                            yield return null;
                    XboxRumble currentXboxRumble = this.xboxRumbleEvents[i];
                    float timeLeft = Mathf.Clamp(currentXboxRumble.timer / currentXboxRumble.duration, 0f, 1f);
                    if (timeLeft > 0)
                    {
                        currentPower = new Vector2(Mathf.Max(currentXboxRumble.power.x * timeLeft, currentPower.x), Mathf.Max(currentXboxRumble.power.y * timeLeft, currentPower.y));
                        currentXboxRumble.update();
                    }
                    else
                    {
                        this.xboxRumbleEvents.Remove(currentXboxRumble);
                    }
                    GamePad.SetVibration(this.playerIndex, currentPower.x, currentPower.y);
                }
            }
        }
        /// <summary>
        /// Loads control asset textures.
        /// </summary>
        public void loadControllerAssets()
        {
            // Written, 20.07.2018

            try
            {
                if (MoControlsMod.assetsLoaded)
                {
                    this.a.texture = MoControlsMod.assets.a;
                    this.b.texture = MoControlsMod.assets.b;
                    this.x.texture = MoControlsMod.assets.x;
                    this.y.texture = MoControlsMod.assets.y;
                    this.start.texture = MoControlsMod.assets.start;
                    this.back.texture = MoControlsMod.assets.back;

                    this.lS.texture = MoControlsMod.assets.ls;
                    this.rS.texture = MoControlsMod.assets.rs;

                    this.lT.texture = MoControlsMod.assets.lt;
                    this.rT.texture = MoControlsMod.assets.rt;

                    this.rB.texture = MoControlsMod.assets.rb;
                    this.lB.texture = MoControlsMod.assets.lb;

                    this.dPadUp.texture = MoControlsMod.assets.dpup;
                    this.dPadDown.texture = MoControlsMod.assets.dpdown;
                    this.dPadLeft.texture = MoControlsMod.assets.dpleft;
                    this.dPadRight.texture = MoControlsMod.assets.dpright;

                    this.xboxControls[16].texture = MoControlsMod.assets.lsleft;
                    this.xboxControls[17].texture = MoControlsMod.assets.lsright;
                    this.xboxControls[18].texture = MoControlsMod.assets.lsup;
                    this.xboxControls[19].texture = MoControlsMod.assets.lsdown;

                    this.xboxControls[20].texture = MoControlsMod.assets.rsleft;
                    this.xboxControls[21].texture = MoControlsMod.assets.rsright;
                    this.xboxControls[22].texture = MoControlsMod.assets.rsup;
                    this.xboxControls[23].texture = MoControlsMod.assets.rsdown;
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
        public XboxControl getXboxControlByInputName(string inInputName)
        {
            // Written, 20.07.2018
            if (inInputName != KeyCode.None.ToString()) // no point enumerating..
            {
                foreach (XboxControl xboxControl in this.xboxControls)
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
        public XboxControl[] getXboxControls()
        {
            // Written, 22.07.2018

            return this.xboxControls;
        }

        #endregion
    }
}
