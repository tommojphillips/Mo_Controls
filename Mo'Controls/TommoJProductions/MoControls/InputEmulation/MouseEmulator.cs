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
                        stickValue = stickValue.doDeadzoneCheck(MoControlsSaveData.loadedSaveData.mouseDeadzone, MoControlsSaveData.loadedSaveData.mouseDeadzoneType);
                        // Sensitivity
                        stickValue = stickValue.doSensitivityOperation(MoControlsSaveData.loadedSaveData.mouseSensitivity);

                        moveX = (int)stickValue.x;
                        moveY = (int)stickValue.y * -1; // '* -1' xbox controller y axis is naturally inverted. so changing the that..;
                        simulateMouseMove(moveX, moveY);
                        yield return null;//new WaitForEndOfFrame();
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

            StartCoroutine(updateCoroutine());
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
            // Written, 08.10.2020

            send(new Point(x, y), (uint)MouseEventButtons.Nothing, MouseEventF.MOVE);
        }
        /// <summary>
        /// Simulates a left mouse button click.
        /// </summary>
        private void simulateLeftClick()
        {
            // Written, 08.10.2020

            if (MoControlsMod.instance.lmbPrimaryInput.GetKeybindDown() || MoControlsMod.instance.lmbSecondaryInput.GetKeybindDown())
            {
                send(getCursorPosition, (uint)MouseEventButtons.XBUTTON1, MouseEventF.LEFTDOWN);
            }
            if (MoControlsMod.instance.lmbPrimaryInput.GetKeybindUp() || MoControlsMod.instance.lmbSecondaryInput.GetKeybindUp())
            {
                send(getCursorPosition, (uint)MouseEventButtons.XBUTTON1, MouseEventF.LEFTUP);
            }
        }
        /// <summary>
        /// Simulates a right mouse button click
        /// </summary>
        private void simulateRightClick()
        {
            // Written, 08.10.2020

            if (MoControlsMod.instance.rmbPrimaryInput.GetKeybindDown() || MoControlsMod.instance.rmbSecondaryInput.GetKeybindDown())
            {
                send(getCursorPosition, (uint)MouseEventButtons.XBUTTON2, MouseEventF.RIGHTDOWN);
            }
            if (MoControlsMod.instance.rmbPrimaryInput.GetKeybindUp() || MoControlsMod.instance.rmbSecondaryInput.GetKeybindUp())
            {
                send(getCursorPosition, (uint)MouseEventButtons.XBUTTON2, MouseEventF.RIGHTUP);
            }
        }
        /// <summary>
        /// Simulates scrolling of the mouse wheel. (One mouse click = 120). Positive = Scroll up | Negitive = Scroll doen.
        /// </summary>
        /// <param name="inScrollAmount">The amount to scroll. default 120.</param>
        internal static void simulateScroll(int inScrollAmount)
        {
            // Written, 08.10.2020

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
