using System;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

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

        #endregion

        #region Properties

        /// <summary>
        /// Represents the normal (go) xbox controls.
        /// </summary>
        public static XboxControl[] normalXboxControls => new XboxControl[]
        {
                // Buttons
                new XboxButton(String.Format("{0}0", buttonPrefix), "A", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}1", buttonPrefix), "B", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}2", buttonPrefix), "X", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}3", buttonPrefix), "Y", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}4", buttonPrefix), "LB", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}5", buttonPrefix), "RB", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}6", buttonPrefix), "Back", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}7", buttonPrefix), "Start", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}8", buttonPrefix), "LS", XboxControlTypeEnum.Button),
                new XboxButton(String.Format("{0}9", buttonPrefix), "RS", XboxControlTypeEnum.Button),
                // D-Pad
                new XboxButton(String.Format("{0}7+", axisPrefix), "D-Pad Up", XboxControlTypeEnum.Axis),
                new XboxButton(String.Format("{0}7-", axisPrefix), "D-Pad Down", XboxControlTypeEnum.Axis),
                new XboxButton(String.Format("{0}6-", axisPrefix), "D-Pad Left", XboxControlTypeEnum.Axis),
                new XboxButton(String.Format("{0}6+", axisPrefix), "D-Pad Right", XboxControlTypeEnum.Axis),
                // Triggers
                new XboxTriggerState(String.Format("{0}9+", axisPrefix), "LT"),
                new XboxTriggerState(String.Format("{0}10+", axisPrefix), "RT"),
                // LS
                new XboxControl(String.Format("{0}1-", axisPrefix), "LS-Left", XboxControlTypeEnum.Axis),
                new XboxControl(String.Format("{0}1+", axisPrefix), "LS-Right", XboxControlTypeEnum.Axis),
                new XboxControl(String.Format("{0}2-", axisPrefix), "LS-Up", XboxControlTypeEnum.Axis),
                new XboxControl(String.Format("{0}2+", axisPrefix), "LS-Down", XboxControlTypeEnum.Axis),
                // RS
                new XboxControl(String.Format("{0}4-", axisPrefix), "RS-Left", XboxControlTypeEnum.Axis),
                new XboxControl(String.Format("{0}4+", axisPrefix), "RS-Right", XboxControlTypeEnum.Axis),
                new XboxControl(String.Format("{0}5-", axisPrefix), "RS-Up", XboxControlTypeEnum.Axis),
                new XboxControl(String.Format("{0}5+", axisPrefix), "RS-Down", XboxControlTypeEnum.Axis)
        };
        /// <summary>
        /// Represents the alt Xbox Controls.
        /// </summary>
        public static XboxControl[] altXboxControls => new XboxControl[]
        {                 
                // Buttons
                new XboxButton(Keys.XboxA, "A", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxB, "B", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxX, "X", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxY, "Y", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxBumperLeft, "LB", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxBumperRight, "RB", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxBack, "Back", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxStart, "Start", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxLStickButton, "LS", XboxControlTypeEnum.Button),
                new XboxButton(Keys.XboxRStickButton, "RS", XboxControlTypeEnum.Button),
                // D-Pad
                new XboxButton(Keys.XboxDPadUp, "D-Pad Up", XboxControlTypeEnum.Axis),
                new XboxButton(Keys.XboxDPadDown, "D-Pad Down", XboxControlTypeEnum.Axis),
                new XboxButton(Keys.XboxDPadLeft, "D-Pad Left", XboxControlTypeEnum.Axis),
                new XboxButton(Keys.XboxDPadRight, "D-Pad Right", XboxControlTypeEnum.Axis),
                // Triggers
                new XboxTriggerState(Keys.XboxTriggerLeft, "LT"),
                new XboxTriggerState(Keys.XboxTriggerRight, "RT"),
                // LS
                new XboxControl(Keys.XboxLStickLeft, "LS-Left", XboxControlTypeEnum.Axis),
                new XboxControl(Keys.XboxLStickRight, "LS-Right", XboxControlTypeEnum.Axis),
                new XboxControl(Keys.XboxLStickUp, "LS-Up", XboxControlTypeEnum.Axis),
                new XboxControl(Keys.XboxLStickDown, "LS-Down", XboxControlTypeEnum.Axis),
                // RS
                new XboxControl(Keys.XboxRStickLeft, "RS-Left", XboxControlTypeEnum.Axis),
                new XboxControl(Keys.XboxRStickRight, "RS-Right", XboxControlTypeEnum.Axis),
                new XboxControl(Keys.XboxRStickUp, "RS-Up", XboxControlTypeEnum.Axis),
                new XboxControl(Keys.XboxRStickDown, "RS-Down", XboxControlTypeEnum.Axis)
        };
        /// <summary>
        /// Represents the type of input for the xbox controller.
        /// </summary>
        public XboxControllerInputMapEnum xboxControllerInput
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the custom xbox controls.
        /// </summary>
        public XboxControl[] customXboxControls
        {
            get;
            set;
        }
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
        private Dictionary<string, XboxButton> inputMap
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the A button on the xbox controller.
        /// </summary>
        public XboxButton A
        {
            get
            {
                return (XboxButton)this.xboxControls[0];
            }
        }
        /// <summary>
        /// Represents the B button on the xbox controller.
        /// </summary>
        public XboxButton B
        {
            get
            {
                return (XboxButton)this.xboxControls[1];
            }
        }
        /// <summary>
        /// Represents the X button on the xbox controller.
        /// </summary>
        public XboxButton X
        {
            get
            {
                return (XboxButton)this.xboxControls[2];
            }
        }
        /// <summary>
        /// Represents the Y button on the xbox controller.
        /// </summary>
        public XboxButton Y
        {
            get
            {
                return (XboxButton)this.xboxControls[3];
            }
        }
        /// <summary>
        /// Represents the D-Pad Up button on the xbox controller.
        /// </summary>
        public XboxButton DPadUp
        {
            get
            {
                return (XboxButton)this.xboxControls[10];
            }
        }
        /// <summary>
        /// Represents the D-Pad Down button on the xbox controller.
        /// </summary>
        public XboxButton DPadDown
        {
            get
            {
                return (XboxButton)this.xboxControls[11];
            }
        }
        /// <summary>
        /// Represents the D-Pad Left button on the xbox controller.
        /// </summary>
        public XboxButton DPadLeft
        {
            get
            {
                return (XboxButton)this.xboxControls[12];
            }
        }
        /// <summary>
        /// Represents the D-Pad Right button on the xbox controller.
        /// </summary>
        public XboxButton DPadRight
        {
            get
            {
                return (XboxButton)this.xboxControls[13];
            }
        }
        /// <summary>
        /// Represents the Start button on the xbox controller.
        /// </summary>
        public XboxButton Start
        {
            get
            {
                return (XboxButton)this.xboxControls[7];
            }
        }
        /// <summary>
        /// Represents the Back button on the xbox controller.
        /// </summary>
        public XboxButton Back
        {
            get
            {
                return (XboxButton)this.xboxControls[6];
            }
        }
        /// <summary>
        /// Represents the Left-Stick button on the xbox controller.
        /// </summary>
        public XboxButton LS
        {
            get
            {
                return (XboxButton)this.xboxControls[8];
            }
        }
        /// <summary>
        /// Represents the Right-Stick button on the xbox controller.
        /// </summary>
        public XboxButton RS
        {
            get
            {
                return (XboxButton)this.xboxControls[9];
            }
        }
        /// <summary>
        /// Represents the Left-Bumper button on the xbox controller.
        /// </summary>
        public XboxButton LB
        {
            get
            {
                return (XboxButton)this.xboxControls[4];
            }
        }
        /// <summary>
        /// Represents the Right-Bumper button on the xbox controller.
        /// </summary>
        public XboxButton RB
        {
            get
            {
                return (XboxButton)this.xboxControls[5];
            }
        }
        /// <summary>
        /// Represents the Left Trigger on the xbox controller
        /// </summary>
        public XboxTriggerState LT
        {
            get
            {
                return (XboxTriggerState)this.xboxControls[14];
            }
        }
        /// <summary>
        /// Represents the Right Trigger on the xbox controller.
        /// </summary>
        public XboxTriggerState RT
        {
            get
            {
                return (XboxTriggerState)this.xboxControls[15];
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
        /// Represents the d-pad on the xbox controller.
        /// </summary>
        public XboxJoystick DPad
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="XboxController"/> and sets the index of the controller (1 - 4).
        /// </summary>
        /// <param name="index">The controller index to set, 1 - 4.</param>
        public XboxController(int index)
        {
            // Written, 16.07.2018

            this.gamePadIndex = index - 1;
            this.playerIndex = (PlayerIndex)this.gamePadIndex;
            this.xboxRumbleEvents = new List<XboxRumble>();
            this.inputMap = new Dictionary<string, XboxButton>();

            #region Initilize Xbox Buttons

            this.xboxControls = normalXboxControls;
            // Thumbsticks
            this.leftThumbstick = new XboxJoystick(
                this.xboxControls[16],
                this.xboxControls[17],
                this.xboxControls[18],
                this.xboxControls[19]);
            this.rightThumbstick = new XboxJoystick(
                this.xboxControls[20],
                this.xboxControls[21],
                this.xboxControls[22],
                this.xboxControls[23]);
            this.DPad = new XboxJoystick(
                this.DPadLeft,
                this.DPadRight,
                this.DPadUp,
                this.DPadDown);

            #endregion            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the controllers state as with the input map, and handles current rumbles.
        /// </summary>
        public void update()
        {
            // Written, 16.07.2018

            this.state = GamePad.GetState(this.playerIndex);

            if (this.state.IsConnected)
            {
                // Face Buttons
                this.A.state = this.state.Buttons.A;
                this.B.state = this.state.Buttons.B;
                this.X.state = this.state.Buttons.X;
                this.Y.state = this.state.Buttons.Y;
                this.Start.state = this.state.Buttons.Start;
                this.Back.state = this.state.Buttons.Back;
                // D-Pad
                this.DPadUp.state = this.state.DPad.Up;
                this.DPadDown.state = this.state.DPad.Down;
                this.DPadLeft.state = this.state.DPad.Left;
                this.DPadRight.state = this.state.DPad.Right;
                // Other
                this.LB.state = this.state.Buttons.LeftShoulder;
                this.RB.state = this.state.Buttons.RightShoulder;
                this.LS.state = this.state.Buttons.LeftStick;
                this.RS.state = this.state.Buttons.RightStick;
                // Triggers
                this.LT.state = this.state.Triggers.Left;
                this.RT.state = this.state.Triggers.Right;

                this.updateInputMap();
                this.handleRumble();
            }
        }
        /// <summary>
        /// Refreshes the controllers previous state as with the input map.
        /// </summary>
        public void refresh()
        {
            // Written, 16.07.2018

            this.previousState = this.state;

            if (this.state.IsConnected)
            {
                // Face Buttons
                this.A.previousState = this.previousState.Buttons.A;
                this.B.previousState = this.previousState.Buttons.B;
                this.X.previousState = this.previousState.Buttons.X;
                this.Y.previousState = this.previousState.Buttons.Y;
                this.Start.previousState = this.previousState.Buttons.Start;
                this.Back.previousState = this.previousState.Buttons.Back;
                // D-Pad
                this.DPadUp.previousState = this.previousState.DPad.Up;
                this.DPadDown.previousState = this.previousState.DPad.Down;
                this.DPadLeft.previousState = this.previousState.DPad.Left;
                this.DPadRight.previousState = this.previousState.DPad.Right;
                // Other
                this.LB.previousState = this.previousState.Buttons.LeftShoulder;
                this.RB.previousState = this.previousState.Buttons.RightShoulder;
                this.LS.previousState = this.previousState.Buttons.LeftStick;
                this.RS.previousState = this.previousState.Buttons.RightStick;
                // Triggers
                this.LT.previousState = this.previousState.Triggers.Left;
                this.RT.previousState = this.previousState.Triggers.Right;

                this.updateInputMap();
            }
        }
        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed.
        /// </summary>
        /// <param name="xboxButton">The button to check</param>
        /// <returns></returns>
        public bool getButtonPressed(XboxButtonEnum xboxButton)
        {
            // Written, 16.07.2018

            if (this.inputMap[xboxButton.toString()].state == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns true if the provided <see cref="XboxButtonEnum"/> has been pressed and held down.
        /// </summary>
        /// <param name="xboxButton">The button to check.</param>
        public bool getButtonDown(XboxButtonEnum xboxButton)
        {
            // Written, 16.07.2018

            XboxButton button = this.inputMap[xboxButton.toString()];
            if (button.previousState == ButtonState.Released && button.state == ButtonState.Pressed)
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

            if (this.LT.previousState == 0f && this.LT.state >= 0.1f)
                return true;
            return false;
        }
        /// <summary>
        /// Returns true of the right trigger was tapped.
        /// </summary>
        public bool getRightTriggerTap()
        {
            // Written, 18.07.2018

            if (this.RT.previousState == 0f && this.RT.state >= 0.1f)
                return true;
            return false;
        }
        /// <summary>
        /// Adds a rumble to the xbox controller.
        /// </summary>
        /// <param name="timer">The time in seconds to rumble for.</param>
        /// <param name="power">The power of the rumble.</param>
        /// <param name="duration">The duration until the fade-time comes.</param>
        public void addRumble(float timer, Vector2 power, float duration)
        {
            // Written, 16.07.2018

            XboxRumble xboxRumble = new XboxRumble()
            {
                timer = timer,
                power = power,
                duration = duration,
            };
            this.addRumble(xboxRumble);
        }
        /// <summary>
        /// Adds the provided rumble to the xbox controller.
        /// </summary>
        /// <param name="xboxRumble">The rumble to add.</param>
        public void addRumble(XboxRumble xboxRumble)
        {
            // Written, 16.07.2018

            this.xboxRumbleEvents.Add(xboxRumble);
        }
        /// <summary>
        /// Updates the input map.
        /// </summary>
        private void updateInputMap()
        {
            // Written, 16.07.2018

            this.inputMap[A.name] = A;
            this.inputMap[B.name] = B;
            this.inputMap[X.name] = X;
            this.inputMap[Y.name] = Y;
            this.inputMap[DPadUp.name] = DPadUp;
            this.inputMap[DPadDown.name] = DPadDown;
            this.inputMap[DPadLeft.name] = DPadLeft;
            this.inputMap[DPadRight.name] = DPadRight;
            this.inputMap[Back.name] = Back;
            this.inputMap[Start.name] = Start;
            this.inputMap[LS.name] = LS;
            this.inputMap[RS.name] = RS;
            this.inputMap[LB.name] = LB;
            this.inputMap[RB.name] = RB;
        }
        /// <summary>
        /// Handles all current rumbles.
        /// </summary>
        private void handleRumble()
        {
            // Written, 16.07.2018

            if (this.xboxRumbleEvents.Count > 0)
            {
                Vector2 currentPower = new Vector2(0.0f, 0.0f);
                for (int i = 0; i < this.xboxRumbleEvents.Count; i++)
                {
                    XboxRumble currentXboxRumble = this.xboxRumbleEvents[i];
                    float timeLeft = Mathf.Clamp(currentXboxRumble.timer / currentXboxRumble.duration, 0f, 1f);
                    if (timeLeft > 0.0f)
                    {
                        currentPower = new Vector2(Mathf.Max(currentXboxRumble.power.x * timeLeft, currentPower.x), Mathf.Max(currentXboxRumble.power.y * timeLeft, currentPower.y));
                        GamePad.SetVibration(this.playerIndex, currentPower.x, currentPower.y);
                        currentXboxRumble.update();
                    }
                    else
                    {
                        this.xboxRumbleEvents.Remove(currentXboxRumble);
                    }
                }
            }
        }
        /// <summary>
        /// Loads control asset textures.
        /// </summary>
        public void loadControllerAssets()
        {
            // Written, 20.07.2018

            if (MoControlsMod.assetsLoaded)
            {
                this.A.texture = MoControlsMod.assets.a;
                this.B.texture = MoControlsMod.assets.b;
                this.X.texture = MoControlsMod.assets.x;
                this.Y.texture = MoControlsMod.assets.y;
                this.Start.texture = MoControlsMod.assets.start;
                this.Back.texture = MoControlsMod.assets.back;

                this.LS.texture = MoControlsMod.assets.ls;
                this.RS.texture = MoControlsMod.assets.rs;

                this.LT.texture = MoControlsMod.assets.lt;
                this.RT.texture = MoControlsMod.assets.rt;

                this.DPadUp.texture = MoControlsMod.assets.dpup;
                this.DPadDown.texture = MoControlsMod.assets.dpdown;
                this.DPadLeft.texture = MoControlsMod.assets.dpleft;
                this.DPadRight.texture = MoControlsMod.assets.dpright;

                this.leftThumbstick.up.texture = MoControlsMod.assets.lsup;
                this.leftThumbstick.down.texture = MoControlsMod.assets.lsdown;
                this.leftThumbstick.left.texture = MoControlsMod.assets.lsleft;
                this.leftThumbstick.right.texture = MoControlsMod.assets.lsright;

                this.rightThumbstick.up.texture = MoControlsMod.assets.rsup;
                this.rightThumbstick.down.texture = MoControlsMod.assets.rsdown;
                this.rightThumbstick.left.texture = MoControlsMod.assets.rsleft;
                this.rightThumbstick.right.texture = MoControlsMod.assets.rsright;
            }
            else
                MoControlsMod.print("Assets cannot be loaded to instance of xbox controller as assets ain't loaded.");
        }
        /// <summary>
        /// Gets an xbox control by input name.
        /// </summary>
        public XboxControl getXboxControlByInputName(string inputName)
        {
            // Written, 20.07.2018
            if (inputName != KeyCode.None.ToString()) // no point enumerating..
            {
                foreach (XboxControl xboxControl in this.xboxControls)
                {
                    if (xboxControl.inputName == inputName)
                    {
                        return xboxControl;
                    }
                }
                foreach (XboxControl xboxControl in this.leftThumbstick.controls)
                {
                    if (xboxControl.inputName == inputName)
                    {
                        return xboxControl;
                    }
                }
                foreach (XboxControl xboxControl in this.rightThumbstick.controls)
                {
                    if (xboxControl.inputName == inputName)
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
        /// <summary>
        /// Sets the input mode.
        /// </summary>
        /// <param name="xboxControllerInputEnum">The mode to set.</param>
        public void setControllerInputType(XboxControllerInputMapEnum xboxControllerInputEnum)
        {
            // Writte, 09.10.2018

            this.xboxControllerInput = xboxControllerInputEnum;
            switch (this.xboxControllerInput)
            {
                case XboxControllerInputMapEnum.Norm:
                    this.xboxControls = normalXboxControls;
                    break;
                case XboxControllerInputMapEnum.Alt:
                    this.xboxControls = altXboxControls;
                    break;
                case XboxControllerInputMapEnum.Custom:
                    this.xboxControls = this.customXboxControls;
                    break;
            }
        }

        #endregion
    }
}
