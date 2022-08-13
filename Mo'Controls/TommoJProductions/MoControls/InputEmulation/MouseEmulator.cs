using System.Collections;
using System.Drawing;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;
using XInputDotNetPure;
using NM = TommoJProductions.MoControls.InputEmulation.NativeMethods;

namespace TommoJProductions.MoControls.InputEmulation
{
    public class MouseEmulator : SingletonMonoBehaviour<MouseEmulator>
    {
        // Written, 01.08.2018

        #region Properties / Fields

        /// <summary>
        /// Returns the current position of the cursor.
        /// </summary>
        public Point getCursorPosition
        {
            get
            {
                if (NM.GetCursorPos(out cursorPos))
                    return cursorPos;
                return Point.Empty;
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
        public const float DEFAULT_SENSITIVITY = 15.0f;
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

        private XboxController controller;

        private Vector2 stickValue;
        private Point mouseMove;
        private Point cursorPos;

        private static InputUnion mouseData;

        #endregion

        #region Unity runtime

        private void Start()
        {
            // Written, 08.10.2018

            mouseMove = new Point();
            mouseData = new InputUnion();
            mouseData.mi = new MouseInput();
            mouseData.ki = new KeyboardInput();
            mouseData.hi = new HardwareInput();

            controller = XboxControllerManager.instance.controller;
        }
        private void Update()
        {
            // Written, 01.08.2018

            update();
        }

        #endregion

        #region Methods
         
        private void update()
        {
            // Written, 16.10.2020

            if (MoControlsSaveData.loadedSaveData.emulateMouse)
            {
                if (controller.isConnected)
                {
                    stickValue = controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.mouseInputType);
                    if (!MoControlsSaveData.loadedSaveData.mouseInputUseRawInput)
                        stickValue = controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.mouseInputType)
                            .doDeadzoneCheck(MoControlsSaveData.loadedSaveData.mouseDeadzoneType, MoControlsSaveData.loadedSaveData.mouseDeadzone)
                            .doSensitivityOperation(MoControlsSaveData.loadedSaveData.mouseSensitivity);

                    if (stickValue.sqrMagnitude > 0)
                    {
                        stickValue *= MoControlsSaveData.loadedSaveData.mouseSensitivity;
                        mouseMove.X = (int)stickValue.x;
                        mouseMove.Y = (int)stickValue.y * -1;
                        simulateMouseMove(mouseMove);
                    }
                }
            }
        }
        /// <summary>
        /// Creates required stuff to simulate mouse movement.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="data">Data to pass.</param>
        /// <param name="time">The time of event.</param>
        /// <param name="flag">flags to pass.</param>
        private static InputUnion reuseMouseInputData(int x, int y, uint data, uint time, MouseEventF flag)
        {
            // Written, 06.10.2020

            mouseData.mi.dx = x;
            mouseData.mi.dy = y;
            mouseData.mi.time = time;
            mouseData.mi.mouseData = data;
            mouseData.mi.dwFlags = flag;            
            return mouseData;
        }
        /// <summary>
        /// Simulates mouse movement.
        /// </summary>
        /// <param name="x">The MoControlsGO.controlManager.xboxController.</param>
        /// <param name="y">The Y.</param>
        private static void simulateMouseMove(Point p)
        {
            // Written, 08.10.2020

            send(p, (uint)MouseEventButtons.Nothing, MouseEventF.MOVE);
        }
        /// <summary>
        /// Simulates a left mouse button click.
        /// </summary>
        internal void simulateLeftClick()
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
        internal void simulateRightClick()
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
        internal void simulateScroll(int inScrollAmount)
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
            inputs[0].U = reuseMouseInputData(point.X, point.Y, wData, 0, eventF);
            NM.SendInput((uint)inputs.Length, inputs, Input.Size);
        }

        #endregion
    }
}
