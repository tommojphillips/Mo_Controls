using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents an xbox controller.
    /// </summary>
    public class XboxController
    {
        // Written, 16.07.2018

        #region Properties

        public bool isConnected
        {
            get
            {
                return this.state.IsConnected;
            }
        }
        public int index
        {
            get
            {
                return this.gamePadIndex;
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
            get;
            private set;
        }
        /// <summary>
        /// Represents the B button on the xbox controller.
        /// </summary>
        public XboxButton B
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the X button on the xbox controller.
        /// </summary>
        public XboxButton X
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Y button on the xbox controller.
        /// </summary>
        public XboxButton Y
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the D-Pad Up button on the xbox controller.
        /// </summary>
        public XboxButton DPadUp
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the D-Pad Down button on the xbox controller.
        /// </summary>
        public XboxButton DPadDown
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the D-Pad Left button on the xbox controller.
        /// </summary>
        public XboxButton DPadLeft
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the D-Pad Right button on the xbox controller.
        /// </summary>
        public XboxButton DPadRight
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Start button on the xbox controller.
        /// </summary>
        public XboxButton Start
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Back button on the xbox controller.
        /// </summary>
        public XboxButton Back
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Left-Stick button on the xbox controller.
        /// </summary>
        public XboxButton LS
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Right-Stick button on the xbox controller.
        /// </summary>
        public XboxButton RS
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Left-Bumper button on the xbox controller.
        /// </summary>
        public XboxButton LB
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Right-Bumper button on the xbox controller.
        /// </summary>
        public XboxButton RB
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Left Trigger on the xbox controller
        /// </summary>
        public XboxTriggerState LT
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the Right Trigger on the xbox controller.
        /// </summary>
        public XboxTriggerState RT
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public XboxController(int index)
        {
            // Written, 16.07.2018

            this.gamePadIndex = index - 1;
            this.playerIndex = (PlayerIndex)this.gamePadIndex;
            this.xboxRumbleEvents = new List<XboxRumble>();
            this.inputMap = new Dictionary<string, XboxButton>();

            #region Initilize Xbox Buttons

            // Face Buttons
            this.A = new XboxButton();
            this.B = new XboxButton();
            this.X = new XboxButton();
            this.Y = new XboxButton();
            this.Start = new XboxButton();
            this.Back = new XboxButton();
            // D-Pad
            this.DPadUp = new XboxButton();
            this.DPadDown = new XboxButton();
            this.DPadLeft = new XboxButton();
            this.DPadRight = new XboxButton();
            // Other
            this.LB = new XboxButton();
            this.RB = new XboxButton();
            this.LS = new XboxButton();
            this.RS = new XboxButton();
            // Triggers
            this.LT = new XboxTriggerState();
            this.RT = new XboxTriggerState();

            #endregion
        }

        #endregion

        #region Methods

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
        public bool getButtonPressed(string xboxButton)
        {
            // Written, 16.07.2018

            if (this.inputMap[xboxButton].state == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        public bool getButtonDown(string xboxButton)
        {
            // Written, 16.07.2018

            XboxButton button = this.inputMap[xboxButton];
            if (button.previousState == ButtonState.Released && button.state == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
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
        public void addRumble(XboxRumble xboxRumble)
        {
            // Written, 16.07.2018

            this.xboxRumbleEvents.Add(xboxRumble);
        }
        private void updateInputMap()
        {
            // Written, 16.07.2018

            inputMap["A"] = A;
            inputMap["B"] = B;
            inputMap["X"] = X;
            inputMap["Y"] = Y;
            inputMap["D-Pad Up"] = DPadUp;
            inputMap["D-Pad Down"] = DPadDown;
            inputMap["D-Pad Left"] = DPadLeft;
            inputMap["D-Pad Right"] = DPadRight;
            inputMap["Back"] = Back;
            inputMap["Start"] = Start;
            inputMap["LS"] = LS;
            inputMap["RS"] = RS;
            inputMap["LB"] = LB;
            inputMap["RB"] = RB;
        }
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

        #endregion
    }
}
