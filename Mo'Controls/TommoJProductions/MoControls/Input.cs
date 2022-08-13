using System;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;
using XInputDotNetPure;
using uInput = UnityEngine.Input;

namespace TommoJProductions.MoControls
{
    internal class Input
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
        internal static MonitorInputData monitorForInput()
        {
            // Written, 09.07.2018

            MonitorInputData monitorInputData = new MonitorInputData()
            {
                foundInput = false,
                input = null,
            };
            XboxController xc = XboxControllerManager.instance.controller;

            // Check xbox controller for input.
            if (xc.isConnected)
            {
                if (xc.rT.state > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.rT.inputName;
                }
                if (xc.lT.state > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.lT.inputName;
                }
                if (xc.dPadUp.state == ButtonState.Pressed)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.dPadUp.inputName;
                }
                if (xc.dPadDown.state == ButtonState.Pressed)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.dPadDown.inputName;
                }
                if (xc.dPadLeft.state == ButtonState.Pressed)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.dPadLeft.inputName;
                }
                if (xc.dPadRight.state == ButtonState.Pressed)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.dPadRight.inputName;
                }
                if (xc.getLeftStick().x > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.leftThumbstick.right.inputName;
                }
                if (xc.getLeftStick().x < -0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.leftThumbstick.left.inputName;
                }
                if (xc.getLeftStick().y > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.leftThumbstick.up.inputName;
                }
                if (xc.getLeftStick().y < -0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.leftThumbstick.down.inputName;
                }
                if (xc.getRightStick().x > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.rightThumbstick.right.inputName;
                }
                if (xc.getRightStick().x < -0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.rightThumbstick.left.inputName;
                }
                if (xc.getRightStick().y > 0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.rightThumbstick.up.inputName;
                }
                if (xc.getRightStick().y < -0.5f)
                {
                    monitorInputData.foundInput = true;
                    monitorInputData.input = xc.rightThumbstick.down.inputName;
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
                            if (kcode == selectKey) //Select key
                                break;

                            if (kcode == noneKey || kcode == KeyCode.Escape) // Set as none key
                            {
                                monitorInputData = new MonitorInputData()
                                {
                                    foundInput = true,
                                    input = KeyCode.None.ToString(),
                                };
                            }
                            else
                            {
                                if (kcode != MoControlsMod.instance.openControlsGui.Key && kcode != cancelKey) // not allowed
                                {
                                    monitorInputData = new MonitorInputData()
                                    {
                                        foundInput = true,
                                        input = kcode.ToString(),
                                    };
                                }
                                else
                                {
                                    ControlManager.instance.changeInputResult.reset();
                                }
                            }

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
