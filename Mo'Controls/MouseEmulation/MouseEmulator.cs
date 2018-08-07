using System;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;
using XInputDotNetPure;
using MSCLoader;
using Mo_Controls.XboxController;
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
        public static Settings emulateMouse = new Settings("emulateMouse", "Emulate mouse for controller", false);
        /// <summary>
        /// Represents whether the mod should use the left thumbstick for mouse emulation.
        /// </summary>
        public static Settings emulateMouse_useLeftThumbstick = new Settings("emulateMouse_useLeftThumbstick", "Use left thumbstick for mouse emulation", false);
        /// <summary>
        /// Represents whether the mod should use the right thumbstick for mouse emulation.
        /// </summary>
        public static Settings emulateMouse_useRightThumbstick = new Settings("emulateMouse_useRightThumbstick", "Use right thumbstick for mouse emulation", true);
        /// <summary>
        /// Represents whether the mod should use the d-pad for mouse emulation.
        /// </summary>
        public static Settings emulateMouse_useDPad = new Settings("emulateMouse_useDPad", "Use D-Pad for mouse emulation", false);
        /// <summary>
        /// Represents the current sensitivity for the mouse.
        /// </summary>
        public static Settings mouseSensitivity = new Settings("MouseSensitivity", "Mouse Sensitivity", DEFAULT_SENSITIVITY);
        /// <summary>
        /// Represents the current deadzone for the mouse.
        /// </summary>
        public static Settings mouseDeadzone = new Settings("MouseDeadzone", "Mouse Deadzone", DEFAULT_DEADZONE);

        #endregion

        /// <summary>
        /// Represents LMB input name.
        /// </summary>
        public const string LMB_INPUT_NAME = "EmLMB";
        /// <summary>
        /// Represents RMB input name.
        /// </summary>
        public const string RMB_INPUT_NAME = "RmLMB";
        /// <summary>
        /// Represents the primary input for LMB
        /// </summary>
        public Keybind lmbPrimaryInput = new Keybind(LMB_INPUT_NAME + "1", "LMB Primary Input", KeyCode.None);
        /// <summary>
        /// Represents the secondary input for LMB
        /// </summary>
        public Keybind lmbSecondaryInput = new Keybind(LMB_INPUT_NAME + "2", "LMB Secondary", KeyCode.None);
        /// <summary>
        /// Represents the primary input for RMB
        /// </summary>
        public Keybind rmbPrimaryInput = new Keybind(RMB_INPUT_NAME + "1", "RMB Primary Input", KeyCode.None);
        /// <summary>
        /// Represents the secondary input for RMB
        /// </summary>
        public Keybind rmbSecondaryInput = new Keybind(RMB_INPUT_NAME + "2", "RMB Secondary", KeyCode.None);
        /// <summary>
        /// Returns the value of the setting, <see cref="emulateMouse_useLeftThumbstick"/>. if set to <see langword="true"/>; Changes, <see cref="inputType"/> to <see cref="InputTypeEnum.LS"/>.
        /// </summary>
        public bool useLeftThumbstick
        {
            get
            {
                return (bool)emulateMouse_useLeftThumbstick.Value;
            }
            set
            {
                emulateMouse_useLeftThumbstick.Value = value;
                if (value)
                    this.inputType = InputTypeEnum.LS;
            }
        }
        /// <summary>
        /// Returns the value of the setting, <see cref="emulateMouse_useRightThumbstick"/>. if set to <see langword="true"/>; Changes, <see cref="inputType"/> to <see cref="InputTypeEnum.RS"/>.
        /// </summary>
        public bool useRightThumbstick
        {
            get
            {
                return (bool)emulateMouse_useRightThumbstick.Value;
            }
            set
            {
                emulateMouse_useRightThumbstick.Value = value;
                if (value)
                    this.inputType = InputTypeEnum.RS;
            }
        }
        /// <summary>
        /// Returns the value of the setting, <see cref="emulateMouse_useDPad"/>. if set to <see langword="true"/>; Changes, <see cref="inputType"/> to <see cref="InputTypeEnum.DPad"/>.
        /// </summary>
        public bool useDPad
        {
            get
            {
                return (bool)emulateMouse_useDPad.Value;
            }
            set
            {
                emulateMouse_useDPad.Value = value;
                if (value)
                    this.inputType = InputTypeEnum.DPad;
            }
        }
        /// <summary>
        /// Returns the <see cref="Settings.Name"/> property of the setting, <see cref="emulateMouse_useDPad"/>.
        /// </summary>
        public string getUseDPadSettingName
        {
            get
            {
                return emulateMouse_useDPad.Name;
            }
        }
        /// <summary>
        /// Returns the <see cref="Settings.Name"/> property of the setting, <see cref="emulateMouse_useLeftThumbstick"/>.
        /// </summary>
        public string getUseLeftThumbstickSettingName
        {
            get
            {
                return emulateMouse_useLeftThumbstick.Name;
            }
        }
        /// <summary>
        /// Returns the <see cref="Settings.Name"/> property of the setting, <see cref="emulateMouse_useRightThumbstick"/>.
        /// </summary>
        public string getUseRightThumbstickSettingName
        {
            get
            {
                return emulateMouse_useRightThumbstick.Name;
            }
        }
        /// <summary>
        /// Returns the <see cref="Settings.Name"/> property of the setting, <see cref="emulateMouse"/> which is private.
        /// </summary>
        public string getEmulateMouseSettingName
        {
            get
            {
                return emulateMouse.Name;
            }
        }
        /// <summary>
        /// Represents whether the current instance is emulating mouse movement.
        /// </summary>
        public bool Emulating
        {
            get
            {
                return (bool)emulateMouse.Value;
            }
            set
            {
                if (value)
                {
                    ModConsole.Print(String.Format("Started Emulating mouse as {0}.",
                        this.inputType == InputTypeEnum.LS ? "Left Thumbstick" : "Right Thumbstick"));
                }
                else
                {
                    ModConsole.Print("Stopped Emulating mouse..");
                }
                emulateMouse.Value = value;
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
                GetCursorPos(out point);
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
                return float.Parse(mouseDeadzone.Value.ToString());
            }
            set
            {
                mouseDeadzone.Value = value;
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
                return float.Parse(mouseSensitivity.Value.ToString());
            }
            set
            {
                mouseSensitivity.Value = value;
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

            this.Emulating = this.Emulating;
            this.useLeftThumbstick = this.useLeftThumbstick;
            this.useRightThumbstick = this.useRightThumbstick;
            this.useDPad = this.useDPad;
            this.deadzoneType = deadzoneType;
            Keybind.Add(Mo_Controls.instance, this.lmbPrimaryInput);
            Keybind.Add(Mo_Controls.instance, this.lmbSecondaryInput);
            Keybind.Add(Mo_Controls.instance, this.rmbPrimaryInput);
            Keybind.Add(Mo_Controls.instance, this.rmbSecondaryInput);
        }

        #endregion

        #region Methods

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point pos);
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, InputData[] pInputs, int cbSize);

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
            // move mouse: Flags ABSOLUTE (whole screen) and MOVE (move)
            MouseEvent[0].data = createMouseInput(x, y, 0, 0, MOUSEEVENTF_MOVE);//MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE);
            SendInput((uint)MouseEvent.Length, MouseEvent, Marshal.SizeOf(MouseEvent[0].GetType()));
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
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
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
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        /// <summary>
        /// Should be called every frame; on <see cref="Mod.Update()"/>.
        /// </summary>
        public void onUpdate()
        {
            // Written, 01.08.2018

            if (this.Emulating)
            {
                Thread thread = new Thread(delegate ()
                {

                    GamePadThumbSticks.StickValue stickValue_temp = default(GamePadThumbSticks.StickValue);
                    Vector2 stickValue = Vector2.zero;
                    int moveX;
                    int moveY;

                    xController xboxController = Mo_Controls.instance.xboxController;
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

                        if (this.lmbPrimaryInput.IsDown() || this.lmbSecondaryInput.IsDown())
                        {
                            simulateLeftClick();
                        }
                        if (this.rmbPrimaryInput.IsDown() || this.rmbSecondaryInput.IsDown())
                        {
                            simulateRightClick();
                        }
                    }
                });
                thread.Start();
            }
        }
        /// <summary>
        /// Should be called on <see cref="Mod.ModSettings"/>.
        /// </summary>
        public static void onModSettings(Mo_Controls mo_Controls)
        {
            // Written, 03.08.2018

            Mo_Controls moC = mo_Controls;

            Settings.AddCheckBox(moC, emulateMouse);
            Settings.AddCheckBox(moC, emulateMouse_useLeftThumbstick, "Emulate Mouse Settings");
            Settings.AddCheckBox(moC, emulateMouse_useRightThumbstick, "Emulate Mouse Settings");
            Settings.AddCheckBox(moC, emulateMouse_useDPad, "Emulate Mouse Settings");
            Settings.AddSlider(moC, mouseDeadzone, MIN_DEADZONE, MAX_DEADZONE);
            Settings.AddSlider(moC, mouseSensitivity, MIN_SENSITIVITY, MAX_SENSITIVITY);
        }

        #endregion
    }
}
