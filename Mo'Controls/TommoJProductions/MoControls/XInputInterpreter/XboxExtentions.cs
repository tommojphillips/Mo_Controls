using System;
using System.Collections.Generic;
using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents extention methods for <see cref="XboxButtonEnum"/>.
    /// </summary>
    public static class XboxExtentions
    {
        // Written, 09.10.2018

        public static void forEach(this XboxControl[] inXboxControl, Func<XboxControl, bool> inFunc)
        {
            // Written, 09.10.2018
            try
            {
                foreach (XboxControl xc in inXboxControl)
                    inFunc?.Invoke(xc);                
            }
            catch (NullReferenceException ex)
            {
                if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.full))
                    MoControlsMod.print(String.Format("<i>[XboxExtentions.forEach(this XboxControl, Action)]</i> - <color=red>Action was null\r\n<b>StackTrace:</b> {0}</color>",
                        ex.StackTrace));
                return;
            }
        }
        /// <summary>
        /// Returns the string representation of the provided <see cref="XboxButtonEnum"/>.
        /// </summary>
        /// <param name="xboxButtonEnum">The xbox button to convert to string.</param>
        public static string toString(this XboxButtonEnum xboxButtonEnum)
        {
            // Written, 17.07.2018

            string value = null;
            switch (xboxButtonEnum)
            {
                case XboxButtonEnum.A:
                    value = "A";
                    break;
                case XboxButtonEnum.B:
                    value = "B";
                    break;
                case XboxButtonEnum.X:
                    value = "X";
                    break;
                case XboxButtonEnum.Y:
                    value = "Y";
                    break;
                case XboxButtonEnum.Back:
                    value = "Back";
                    break;
                case XboxButtonEnum.Start:
                    value = "Start";
                    break;
                case XboxButtonEnum.DPad_Up:
                    value = "D-Pad Up";
                    break;
                case XboxButtonEnum.DPad_Down:
                    value = "D-Pad Down";
                    break;
                case XboxButtonEnum.DPad_Left:
                    value = "D-Pad Left";
                    break;
                case XboxButtonEnum.DPad_Right:
                    value = "D-Pad Right";
                    break;
                case XboxButtonEnum.LS:
                    value = "LS";
                    break;
                case XboxButtonEnum.RS:
                    value = "RS";
                    break;
                case XboxButtonEnum.LB:
                    value = "LB";
                    break;
                case XboxButtonEnum.RB:
                    value = "RB";
                    break;
                case XboxButtonEnum.NULL:
                    value = null;
                    break;
            }
            return value;
        }
        /// <summary>
        /// Performs a deadzone check.
        /// </summary>
        /// <param name="stickValue">The values to perform the check on.</param>
        public static Vector2 doDeadzoneCheck(this Vector2 stickValue, float deadzoneThreshhold, DeadzoneTypeEnum deadzoneType)
        {
            // Written, 02.08.2018

            switch (deadzoneType)
            {
                case DeadzoneTypeEnum.Radial:
                    if (stickValue.magnitude < deadzoneThreshhold)
                        stickValue = Vector2.zero;
                    break;
                case DeadzoneTypeEnum.ScaledRadial:
                    if (stickValue.magnitude < deadzoneThreshhold)
                        stickValue = Vector2.zero;
                    stickValue = stickValue.normalized * ((stickValue.magnitude - deadzoneThreshhold) / (1 - deadzoneThreshhold));
                    break;
            }
            return stickValue;
        }
        /// <summary>
        /// Performs sensitivity operation.
        /// </summary>
        /// <param name="stickValue">inValue.</param>
        public static Vector2 doSensitivityOperation(this Vector2 stickValue, float sensitivityThreshhold)
        {
            // Written, 07.08.2018

            stickValue.Set(stickValue.x *= sensitivityThreshhold, stickValue.y *= sensitivityThreshhold);
            return stickValue;
        }
        /// <summary>
        /// Gets the xbox control in the collection by name.
        /// </summary>
        /// <param name="inXboxControls">The xbox control collection.</param>
        /// <param name="inName">The name of the xbox control to get.</param>
        public static XboxControl getXboxControl(this IEnumerable<XboxControl> inXboxControls, string inName)
        {
            // Written, 15.10.2018

            foreach (XboxControl xc in inXboxControls)
            {
                if (xc.name == inName)
                    return xc;
            }
            return null;
        }
    }
}
