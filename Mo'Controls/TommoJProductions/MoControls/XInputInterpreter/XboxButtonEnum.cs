﻿namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents all xbox buttons. NOTE cannot change these values. as logic replaces _ with a space " ". (toString())
    /// </summary>
    public enum XboxButtonEnum
    {
        /// <summary>
        /// Represents null.
        /// </summary>
        None,
        /// <summary>
        /// Represents the A button.
        /// </summary>
        A,
        /// <summary>
        /// Represents the B button.
        /// </summary>
        B,
        /// <summary>
        /// Represents the X button.
        /// </summary>
        X,
        /// <summary>
        /// Represents the Y button.
        /// </summary>
        Y,
        /// <summary>
        /// Represents the Back button.
        /// </summary>
        Back,
        /// <summary>
        /// Represents the Start button.
        /// </summary>
        Start,
        /// <summary>
        /// Represents the D-Pad Up button.
        /// </summary>
        D_Pad_Up,
        /// <summary>
        /// Represents the D-Pad Down button.
        /// </summary>
        D_Pad_Down,
        /// <summary>
        /// Represents the D-Pad Left button.
        /// </summary>
        D_Pad_Left,
        /// <summary>
        /// Represents the D-Pad Right button.
        /// </summary>
        D_Pad_Right,
        /// <summary>
        /// Represents the Left Stick button.
        /// </summary>
        LS,
        /// <summary>
        /// Represents the Right Stick button.
        /// </summary>
        RS,
        /// <summary>
        /// Represents the Left Bumper button.
        /// </summary>
        LB,
        /// <summary>
        /// Represents the Right Bumper button.
        /// </summary>
        RB,
        /*/// <summary>
        /// Represents the Xbox logo/Guide button.
        /// </summary>
        Guide,*/
    }
}
