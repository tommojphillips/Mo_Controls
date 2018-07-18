namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents extention methods for <see cref="XboxButtonEnum"/>.
    /// </summary>
    public static class XboxButtonEnumExtentions
    {
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
    }
}
