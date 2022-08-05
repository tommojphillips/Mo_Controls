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

        /// <summary>
        /// Returns the string representation of the provided <see cref="XboxButtonEnum"/>.
        /// </summary>
        /// <param name="button">The xbox button to convert to string.</param>
        public static string toString(this XboxButtonEnum button)
        {
            // Written, 17.07.2018

            return button.ToString().Replace("_", " ");
        }
        /// <summary>
        /// Returns the string representation of the provided <see cref="XboxAxisEnum"/>.
        /// </summary>
        /// <param name="button">The xbox axis to convert to string.</param>
        public static string toString(this XboxAxisEnum axis)
        {
            // Written, 24.07.2022

            return axis.ToString().Replace("_", " ");
        }

        /// <summary>
        /// Performs a deadzone check.
        /// </summary>
        /// <param name="v2">The values to perform the check on.</param>
        public static Vector2 doDeadzoneCheck(this Vector2 v2, DeadzoneTypeEnum deadzoneType, float deadzoneValue)
        {
            // Written, 02.08.2018

            switch (deadzoneType)
            {
                case DeadzoneTypeEnum.Radial:
                    if (v2.magnitude < deadzoneValue)
                        v2 = XboxController.vector2Zero;
                    break;
                case DeadzoneTypeEnum.ScaledRadial:
                    if (v2.magnitude < deadzoneValue)
                        v2 = XboxController.vector2Zero;
                    else
                        v2 = v2.normalized * ((v2.magnitude - deadzoneValue) / (1 - deadzoneValue));
                    break;
            }
            return v2;
        }
        /// <summary>
        /// Performs sensitivity operation.
        /// </summary>
        /// <param name="v2">inValue.</param>
        public static Vector2 doSensitivityOperation(this Vector2 v2, float sensitivity)
        {
            // Written, 07.08.2018
                       
            return v2 * sensitivity;
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
