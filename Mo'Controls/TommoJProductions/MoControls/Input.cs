using System;
using UnityEngine;
using uInput = UnityEngine.Input;
using XInputDotNetPure;
using TommoJProdutions.MoControls.XInputInterpreter;

namespace TommoJProdutions.MoControls
{
    public class Input
    {
        #region Keys

        /// <summary>
        /// Represents the select keycode.
        /// </summary>
        public const KeyCode selectKey = KeyCode.Mouse0; // LMB
        /// <summary>
        /// Represents the cancel keycode.
        /// </summary>
        public const KeyCode cancelKey = KeyCode.Mouse1; // RMB
        /// <summary>
        /// Represents the none keycode.
        /// </summary>
        public const KeyCode noneKey = KeyCode.Delete;

        #endregion

        #region Methods

        /// <summary>
        /// Monitors for input from both an xbox controller, the keyboard and the mouse.
        /// </summary>
        public static MonitorInputData monitorForInput()
        {
            // Written, 09.07.2018

            MonitorInputData monitorInputData = new MonitorInputData()
            {
                foundInput = false,
                input = null,
            };
            XboxController xboxController = MoControlsGO.xboxController;
            // Check xbox controller for input.
            if (xboxController.isConnected)
            {
                if (xboxController.getRightTrigger() > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xboxController.RT.inputName;
                }
                else
                {
                    if (xboxController.getLeftTrigger() > 0.5f)
                    {
                        monitorInputData.foundInput = true;
                        monitorInputData.input = xboxController.LT.inputName;
                    }
                    else
                    {
                        if (xboxController.DPadUp.state == ButtonState.Pressed)
                        {
                            monitorInputData.foundInput = true;
                            monitorInputData.input = xboxController.DPadUp.inputName;
                        }
                        else
                        {
                            if (xboxController.DPadDown.state == ButtonState.Pressed)
                            {
                                monitorInputData.foundInput = true;
                                monitorInputData.input = xboxController.DPadDown.inputName;
                            }
                            else
                            {
                                if (xboxController.DPadLeft.state == ButtonState.Pressed)
                                {
                                    monitorInputData.foundInput = true;
                                    monitorInputData.input = xboxController.DPadLeft.inputName;
                                }
                                else
                                {
                                    if (xboxController.DPadRight.state == ButtonState.Pressed)
                                    {
                                        monitorInputData.foundInput = true;
                                        monitorInputData.input = xboxController.DPadRight.inputName;
                                    }
                                    else
                                    {
                                        if (xboxController.getLeftStick().X > 0.0f)
                                        {
                                            monitorInputData.foundInput = true;
                                            monitorInputData.input = xboxController.leftThumbstick.right.inputName;
                                        }
                                        else
                                        {
                                            if (xboxController.getLeftStick().X < 0.0f)
                                            {
                                                monitorInputData.foundInput = true;
                                                monitorInputData.input = xboxController.leftThumbstick.left.inputName;
                                            }
                                            else
                                            {
                                                if (xboxController.getLeftStick().Y > 0.0f)
                                                {
                                                    monitorInputData.foundInput = true;
                                                    monitorInputData.input = xboxController.leftThumbstick.up.inputName;
                                                }
                                                else
                                                {
                                                    if (xboxController.getLeftStick().Y < 0.0f)
                                                    {
                                                        monitorInputData.foundInput = true;
                                                        monitorInputData.input = xboxController.leftThumbstick.down.inputName;
                                                    }
                                                    else
                                                    {
                                                        if (xboxController.getRightStick().X > 0.0f)
                                                        {
                                                            monitorInputData.foundInput = true;
                                                            monitorInputData.input = xboxController.rightThumbstick.right.inputName;
                                                        }
                                                        else
                                                        {
                                                            if (xboxController.getRightStick().X < 0.0f)
                                                            {
                                                                monitorInputData.foundInput = true;
                                                                monitorInputData.input = xboxController.rightThumbstick.left.inputName;
                                                            }
                                                            else
                                                            {
                                                                if (xboxController.getRightStick().Y > 0.0f)
                                                                {
                                                                    monitorInputData.foundInput = true;
                                                                    monitorInputData.input = xboxController.rightThumbstick.up.inputName;
                                                                }
                                                                else
                                                                {
                                                                    if (xboxController.getRightStick().Y < 0.0f)
                                                                    {
                                                                        monitorInputData.foundInput = true;
                                                                        monitorInputData.input = xboxController.rightThumbstick.down.inputName;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!monitorInputData.foundInput)
            {
                if (uInput.anyKeyDown)
                {
                    foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (uInput.GetKeyDown(kcode))
                        {
                            if (kcode != selectKey) //Select key
                            {
                                if (kcode == noneKey) // Set as none key
                                {
                                    monitorInputData = new MonitorInputData()
                                    {
                                        foundInput = true,
                                        input = KeyCode.None.ToString(),
                                    };
                                }
                                else
                                {
                                    if (kcode != MoControlsGO.moControlsGui.openControlsGui.Key && kcode != cancelKey) // not allowed
                                    {
                                        monitorInputData = new MonitorInputData()
                                        {
                                            foundInput = true,
                                            input = kcode.ToString(),
                                        };
                                    }
                                    else
                                    {
                                        MoControlsGO.controlManager.changeInputResult = new ChangeInput();
                                    }
                                }
                                break;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            return monitorInputData;
        }

        #endregion
    }
}
