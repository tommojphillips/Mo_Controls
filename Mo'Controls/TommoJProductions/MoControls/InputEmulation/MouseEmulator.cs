using MSCLoader;
using System;
using System.Collections;
using System.Drawing;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;
using XInputDotNetPure;
using NM = TommoJProductions.MoControls.InputEmulation.NativeMethods;

namespace TommoJProductions.MoControls.InputEmulation
{
    public class MouseEmulator : MonoBehaviour
    {
        // Written, 01.08.2018

        /// <summary>
        /// Represents the primary input for LMB
        /// </summary>
        public Keybind lmbPrimaryInput = new Keybind(LMB_INPUT_NAME + "1", "LMB Primary Input", KeyCode.JoystickButton0);
        /// <summary>
        /// Represents the secondary input for LMB
        /// </summary>
        public Keybind lmbSecondaryInput = new Keybind(LMB_INPUT_NAME + "2", "LMB Secondary", KeyCode.Keypad1);
        /// <summary>
        /// Represents the primary input for RMB
        /// </summary>
        public Keybind rmbPrimaryInput = new Keybind(RMB_INPUT_NAME + "1", "RMB Primary Input", KeyCode.JoystickButton1);
        /// <summary>
        /// Represents the secondary input for RMB
        /// </summary>
        public Keybind rmbSecondaryInput = new Keybind(RMB_INPUT_NAME + "2", "RMB Secondary", KeyCode.Keypad2);

        #region Properties / Fields

        /// <summary>
        /// Returns the current position of the cursor.
        /// </summary>
        public static Point getCursorPosition
        {
            get
            {
                NM.GetCursorPos(out Point point);
                return point;
            }
        }

        // Mouse Constants
        /// <summary>
        /// Represents the default deadzone for the emulated mouse.
        /// </summary>
        public const float DEFAULT_DEADZONE = 0;
        /// <summary>
        /// Represents the default sensitivity for the emulated mouse.
        /// </summary>
        public const float DEFAULT_SENSITIVITY = 30.0f;
        /// <summary>
        /// Represents the min deadzone.
        /// </summary>
        public const float MIN_DEADZONE = 0;
        /// <summary>
        /// Represents the max deadzone.
        /// </summary>
        public const float MAX_DEADZONE = 0.99f;
        /// <summary>
        /// Represents the min sensitivity.
        /// </summary>
        public const float MIN_SENSITIVITY = 1f;
        /// <summary>
        /// Represents the max sensitivity.
        /// </summary>
        public const float MAX_SENSITIVITY = 100f;
        /// <summary>
        /// Represents LMB input name.
        /// </summary>
        public const string LMB_INPUT_NAME = "mocontrolsLMB";
        /// <summary>
        /// Represents RMB input name.
        /// </summary>
        public const string RMB_INPUT_NAME = "mocontrolsRMB";
        /// <summary>
        /// Represents a single mouse scroll value.
        /// </summary>
        public const int MOUSE_SCROLL_VALUE = 120;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MouseEmulator"/>.
        /// </summary>
        /// <param name="inputType">The input type to control the mouse.</param>
        public MouseEmulator()
        {
            // Written, 01.08.2018

            Keybind.Add(MoControlsMod.instance, this.lmbPrimaryInput);
            Keybind.Add(MoControlsMod.instance, this.lmbSecondaryInput);
            Keybind.Add(MoControlsMod.instance, this.rmbPrimaryInput);
            Keybind.Add(MoControlsMod.instance, this.rmbSecondaryInput);
        }

        #endregion

        #region Methods

        private IEnumerator updateCoroutine()
        {
            // Written, 16.10.2020

            if (MoControlsSaveData.loadedSaveData.emulateMouse)
            {
                XboxController xboxController = MoControlsGO.xboxController;

                simulateLeftClick();
                simulateRightClick();
                if (xboxController.isConnected)
                {
                    GamePadThumbSticks.StickValue stickValue_temp = default;
                    Vector2 stickValue = Vector2.zero;
                    int moveX;
                    int moveY;

                    switch (MoControlsSaveData.loadedSaveData.mouseInputType)
                    {
                        case InputTypeEnum.LS:
                            stickValue_temp = xboxController.getLeftStick();
                            break;
                        case InputTypeEnum.RS:
                            stickValue_temp = xboxController.getRightStick();
                            break;
                        case InputTypeEnum.DPad:
                            if (xboxController.dPadLeft.state == ButtonState.Pressed)
                            {
                                stickValue.x = -1;
                            }
                            if (xboxController.dPadRight.state == ButtonState.Pressed)
                            {
                                stickValue.x = 1;
                            }
                            if (xboxController.dPadUp.state == ButtonState.Pressed)
                            {
                                stickValue.y = 1;
                            }
                            if (xboxController.dPadDown.state == ButtonState.Pressed)
                            {
                                stickValue.y = -1;
                            }
                            break;
                    }
                    if (MoControlsSaveData.loadedSaveData.mouseInputType != InputTypeEnum.DPad)
                    {
                        stickValue.x = stickValue_temp.X;
                        stickValue.y = stickValue_temp.Y;
                    }
                    if (stickValue != Vector2.zero)
                    {
                        // Deadzone
                        stickValue = stickValue.doDeadzoneCheck(MoControlsSaveData.loadedSaveData.mouseDeadzone, DeadzoneTypeEnum.ScaledRadial);
                        // Sensitivity
                        stickValue = stickValue.doSensitivityOperation(MoControlsSaveData.loadedSaveData.mouseSensitivity);

                        moveX = (int)stickValue.x;
                        moveY = (int)stickValue.y * -1; // '* -1' xbox controller y axis is naturally inverted. so changing the that..;
                        simulateMouseMove(moveX, moveY);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }
        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            MoControlsMod.print(nameof(MouseEmulator) + ": Started", Debugging.DebugTypeEnum.full);
        }
        /// <summary>
        /// Occurs every frame
        /// </summary>
        private void Update()
        {
            // Written, 01.08.2018

            StartCoroutine(this.updateCoroutine());
        }
        /// <summary>
        /// Creates required stuff to simulate mouse movement.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="data">Data to pass.</param>
        /// <param name="time">The time of event.</param>
        /// <param name="flag">flags to pass.</param>
        private static InputUnion createMouseInput(int x, int y, uint data, uint time, MouseEventF flag)
        {
            // Written, 06.10.2020

            InputUnion result = new InputUnion
            {
                mi = new MouseInput()
                {
                    dx = x,
                    dy = y,
                    mouseData = data,
                    time = time,
                    dwFlags = flag
                }
            };
            return result;
        }
        /// <summary>
        /// Simulates mouse movement.
        /// </summary>
        /// <param name="x">The X.</param>
        /// <param name="y">The Y.</param>
        private static void simulateMouseMove(int x, int y)
        {
            /*Input[] MouseEvent = new Input[1];
            MouseEvent[0].type = 0;
            MouseEvent[0].U = createMouseInput(x, y, 0, 0, MouseEventF.MOVE);
            NM.SendInput((uint)MouseEvent.Length, MouseEvent, Input.Size);*/

            // Written, 08.10.2020

            send(new Point(x, y), (uint)MouseEventDataXButtons.Nothing, MouseEventF.MOVE);
        }
        /// <summary>
        /// Simulates a left mouse button click.
        /// </summary>
        private void simulateLeftClick()
        {
            // Written, 08.10.2020

            if (this.lmbPrimaryInput.GetKeybindDown() || this.lmbSecondaryInput.GetKeybindDown())
            {
                MoControlsMod.print("LMB Down", Debugging.DebugTypeEnum.full);
                send(getCursorPosition, (uint)MouseEventDataXButtons.XBUTTON1, MouseEventF.LEFTDOWN);
            }
            if (this.lmbPrimaryInput.GetKeybindUp() || this.lmbSecondaryInput.GetKeybindUp())
            {
                MoControlsMod.print("LMB Up", Debugging.DebugTypeEnum.full);
                send(getCursorPosition, (uint)MouseEventDataXButtons.XBUTTON1, MouseEventF.LEFTUP);
            }
        }
        /// <summary>
        /// Simulates a right mouse button click
        /// </summary>
        private void simulateRightClick()
        {
            // Written, 08.10.2020

            if (this.rmbPrimaryInput.GetKeybindDown() || this.rmbSecondaryInput.GetKeybindDown())
            {
                MoControlsMod.print("RMB Down", Debugging.DebugTypeEnum.full);
                send(getCursorPosition, (uint)MouseEventDataXButtons.XBUTTON2, MouseEventF.RIGHTDOWN);
            }
            if (this.rmbPrimaryInput.GetKeybindUp() || this.rmbSecondaryInput.GetKeybindUp())
            {
                MoControlsMod.print("RMB Up", Debugging.DebugTypeEnum.full);
                send(getCursorPosition, (uint)MouseEventDataXButtons.XBUTTON2, MouseEventF.RIGHTUP);
            }
        }
        /// <summary>
        /// Simulates scrolling of the mouse wheel. (One mouse click = 120). Positive = Scroll up | Negitive = Scroll doen.
        /// </summary>
        /// <param name="inScrollAmount">The amount to scroll. default 120.</param>
        internal static void simulateScroll(int inScrollAmount)
        {
            // Written, 08.10.2020

            MoControlsMod.print(String.Format("Scroll {0} ({1})", inScrollAmount > 0 ? "Up" : "Down", inScrollAmount), Debugging.DebugTypeEnum.full);
            send(getCursorPosition, (uint)inScrollAmount, MouseEventF.WHEEL);
        }
        /// <summary>
        /// Sends mouse data to sendinput
        /// </summary>
        /// <param name="point">The position of the mouse.</param>
        /// <param name="wData">mouse data</param>
        /// <param name="eventF">mouse event flags.</param>
        private static void send(Point point, uint wData, MouseEventF eventF)
        {
            // Written, 08.10.2020

            Input[] inputs = new Input[1];
            inputs[0].type = 0;
            inputs[0].U = createMouseInput(point.X, point.Y, wData, 0, eventF);
            NM.SendInput((uint)inputs.Length, inputs, Input.Size);
        }

        #endregion
    }
}
