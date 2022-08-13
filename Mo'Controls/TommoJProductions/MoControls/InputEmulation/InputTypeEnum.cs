using System.ComponentModel;

namespace TommoJProductions.MoControls.InputEmulation
{
    public enum InputTypeEnum
    {
        /// <summary>
        /// Represents an input type of left thumbstick.
        /// </summary>
        [Description("Left stick")]
        LS,
        /// <summary>
        /// Represents an input type of right thumbstick.
        /// </summary>
        [Description("Right stick")]
        RS,
        /// <summary>
        /// Represents an input type of D-Pad.
        /// </summary>
        [Description("D-Pad")]
        DPad,
    }
}
