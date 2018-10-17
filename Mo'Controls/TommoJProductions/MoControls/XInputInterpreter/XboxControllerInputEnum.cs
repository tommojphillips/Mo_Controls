namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents types of xbox controller input namings. (axis + buttons).
    /// </summary>
    public enum XboxControllerInputMapEnum
    {
        /// <summary>
        /// Represents normal xbox one controller axis and button input naming. (from developer).
        /// </summary>
        Norm,
        /// <summary>
        /// Represents alt xbox controller axis and button input naming. (from UnityEngine.Input).
        /// </summary>
        Alt,
        /// <summary>
        /// Represents custom xbox controller axis and button input naming. (edited by end user).
        /// </summary>
        Custom
    }
}
