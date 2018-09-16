using System;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;
using XInputDotNetPure;
using MSCLoader;
using Mo_Controls.XboxController;
using NM = Mo_Controls.MouseEmulation.NativeMethods;
using xController = Mo_Controls.XboxController.XboxController;

namespace Mo_Controls.MouseEmulation
{
    public class MouseEmulator
    {
        // Written, 01.08.2018

        #region Properties / Fields

        #region MSCLoader Settings

        /// <summary>
        /// Represents whether the mod should emulate the mouse.
        /// </summary>
        public static bool emulateMouse;
        /// <summary>
        /// Represents whether the mod should use the left thumbstick for mouse emulation.
        /// </summary>
        public static bool emulateMouse_useLeftThumbstick;
        /// <summary>
        /// Represents whether the mod should use the right thumbstick for mouse emulation.
        /// </summary>
        public static bool emulateMouse_useRightThumbstick;
        /// <summary>
        /// Represents whether the mod should use the d-pad for mouse emulation.
        /// </summary>
        public static bool emulateMouse_useDPad;
        /// <summary>
        /// Represents the current sensitivity for the mouse.
        /// </summary>
        public static float mouseSensitivity;
        /// <summary>
        /// Represents the current deadzone for the mouse.
        /// </summary>
        public static float mouseDeadzone;

        #endregion

        /// <summary>
        /// Represents LMB input name.
        /// </summary>
        public const string LMB_INPUT_NAME = "LMB";
        /// <summary>
        /// Represents RMB input name.
        /// </summary>
        public const string RMB_INPUT_NAME = "RMB";
        /// <summary>
        /// Represents the primary input for LMB
        /// </summary>
        public Keybind lmbPrimaryInput = new Keybind(LMB_INPUT_NAME + "1", "LMB Primary Input", KeyCode.Joystick1Button0);
        /// <summary>
        /// Represents the secondary input for LMB
        /// </summary>
        public Keybind lmbSecondaryInput = new Keybind(LMB_INPUT_NAME + "2", "LMB Secondary", KeyCode.Keypad1);
        /// <summary>
        /// Represents the primary input for RMB
        /// </summary>
        public Keybind rmbPrimaryInput = new Keybind(RMB_INPUT_NAME + "1", "RMB Primary Input", KeyCode.Joystick1Button2);
        /// <summary>
        /// Represents the secondary input for RMB
        /// </summary>
        public Keybind rmbSecondaryInput = new Keybind(RMB_INPUT_NAME + "2", "RMB Secondary", KeyCode.Keypad2);
        /// <summary>
        /// Represents whether the current instance is emulating mouse movement.
        /// </summary>
        public bool Emulating
        {
            get
            {
                return emulateMouse;
            }
            set
            {
                if (value)
                {
                    ModConsole.Print(String.Format("<color=green>>></color> Started Emulating mouse as {0}.", this.inputType));
                }
                else
                {
                    ModConsole.Print("<color=green>>></color> Stopped Emulating mouse..");
                }
                emulateMouse = value;
            }
        }
        /// <summary>
        /// Returns the current position of the cursor.
        /// </summary>
        public static Point getCursorPosition
        {
            get
            {
                Point point = new Point();
                NativeMethods.GetCursorPos(out point);
                return point;
            }
        }
        /// <summary>
        /// Represents the input that will control the mouse.
        /// </summary>
        public InputTypeEnum inputType
        {
            get;
            set;
        }        
        /// <summary>
        /// Represents the deadzone for the mouse.
        /// </summary>
        public float deadzone
        {
            get
            {
                return mouseDeadzone;
            }
            set
            {
                mouseDeadzone = value;
            }
        }
        /// <summary>
        /// Represents the type of deadzone algorithm to use.
        /// </summary>
        public DeadzoneTypeEnum deadzoneType
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the sensitivity fpr the mouse.
        /// </summary>
        public float sensitivity
        {
            get
            {
                return mouseSensitivity;
            }
            set
            {
                mouseSensitivity = value;
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

        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MouseEmulator"/>.
        /// </summary>
        /// <param name="inputType">The input type to control the mouse.</param>
        public MouseEmulator(DeadzoneTypeEnum deadzoneType)
        {
            // Written, 01.08.2018

            this.deadzoneType = DeadzoneTypeEnum.ScaledRadial;

            Keybind.Add(Mo_Controls.instance, this.lmbPrimaryInput);
            Keybind.Add(Mo_Controls.instance, this.lmbSecondaryInput);
            Keybind.Add(Mo_Controls.instance, this.rmbPrimaryInput);
            Keybind.Add(Mo_Controls.instance, this.rmbSecondaryInput);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Creates required stuff to simulate mouse movement.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="data">Data to pass.</param>
        /// <param name="time">The time of event.</param>
        /// <param name="flag">flags to pass.</param>
        private static MouseInput createMouseInput(int x, int y, uint data, uint time, uint flag)
        {
            // create from the given data an object of the type MouseInput, which then can be send
            MouseInput Result = new MouseInput();
            Result.X = x;
            Result.Y = y;
            Result.mouseData = data;
            Result.time = time;
            Result.dwFlags = flag;
            return Result;
        }
        /// <summary>
        /// Simulates mouse movement.
        /// </summary>
        /// <param name="x">The X.</param>
        /// <param name="y">The Y.</param>
        private static void simulateMouseMove(int x, int y)
        {
            InputData[] MouseEvent = new InputData[1];
            MouseEvent[0].type = 0;
            MouseEvent[0].data = createMouseInput(x, y, 0, 0, MOUSEEVENTF_MOVE);
            NM.SendInput((uint)MouseEvent.Length, MouseEvent, Marshal.SizeOf(MouseEvent[0].GetType()));
        }
        /// <summary>
        /// Simulates a left mouse button click.
        /// </summary>
        private static void simulateLeftClick()
        {
            // Written, 04.08.2018

            Point tempCursPos = getCursorPosition;
            int X = tempCursPos.X;
            int Y = tempCursPos.Y;
            NM.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        /// <summary>
        /// Simulates a right mouse button click
        /// </summary>
        private static void simulateRightClick()
        {
            // Written, 04.08.2018

            Point tempCursPos = getCursorPosition;
            int X = tempCursPos.X;
            int Y = tempCursPos.Y;
            NM.mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        /// <summary>
        /// Should be called every frame; on <see cref="Mod.Update()"/>.
        /// </summary>
        public void onUpdate()
        {
            // Written, 01.08.2018

            Thread thread = new Thread(delegate ()
            {                
                if (this.Emulating)
                {
                    xController xboxController = Mo_Controls.instance.xboxController;

                    if (this.lmbPrimaryInput.IsDown() || this.lmbSecondaryInput.IsDown())
                    {
                        simulateLeftClick();
                    }
                    if (this.rmbPrimaryInput.IsDown() || this.rmbSecondaryInput.IsDown())
                    {
                        simulateRightClick();
                    }
                    if (xboxController.isConnected)
                    {
                        GamePadThumbSticks.StickValue stickValue_temp = default(GamePadThumbSticks.StickValue);
                        Vector2 stickValue = Vector2.zero;
                        int moveX;
                        int moveY;

                        switch (this.inputType)
                        {
                            case InputTypeEnum.LS:
                                stickValue_temp = xboxController.getLeftStick();
                                break;
                            case InputTypeEnum.RS:
                                stickValue_temp = xboxController.getRightStick();
                                break;
                            case InputTypeEnum.DPad:
                                if (xboxController.DPadLeft.state == ButtonState.Pressed)
                                {
                                    stickValue.x = -1;
                                }
                                if (xboxController.DPadRight.state == ButtonState.Pressed)
                                {
                                    stickValue.x = 1;
                                }
                                if (xboxController.DPadUp.state == ButtonState.Pressed)
                                {
                                    stickValue.y = 1;
                                }
                                if (xboxController.DPadDown.state == ButtonState.Pressed)
                                {
                                    stickValue.y = -1;
                                }
                                break;
                        }
                        if (this.inputType != InputTypeEnum.DPad)
                        {
                            stickValue.x = stickValue_temp.X;
                            stickValue.y = stickValue_temp.Y;
                        }
                        if (stickValue != Vector2.zero)
                        {
                            // Deadzone
                            stickValue = stickValue.doDeadzoneCheck(this.deadzone, this.deadzoneType);
                            // Sensitivity
                            stickValue = stickValue.doSensitivityOperation(this.sensitivity);

                            moveX = (int)stickValue.x;
                            moveY = (int)stickValue.y * -1; // '* -1' xbox controller y axis is naturally inverted. so changing the that..;
                            simulateMouseMove(moveX, moveY);
                        }
                    }                
                }
            });
            thread.Start();
        }

        #endregion
    }
}
