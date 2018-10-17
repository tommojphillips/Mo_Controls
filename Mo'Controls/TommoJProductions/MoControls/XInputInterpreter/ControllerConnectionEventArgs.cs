using System;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents event data for the when the controllers connection changes.
    /// </summary>
    public class ControllerConnectionEventArgs : EventArgs
    {
        // Written, 16.07.2018

        /// <summary>
       /// Represents the xbox controller that was connected.
       /// </summary>
        public XboxController xboxController
        {
            get;
            private set;
        }

        /// <summary>
        /// Initalizes a new instance of, <see cref="ControllerConnectionEventArgs"/> and assigns <see cref="xboxController"/> to the parameter, <paramref name="inXboxController"/>.
        /// </summary>
        /// <param name="inXboxController">The xbox controller that was connected.</param>
        public ControllerConnectionEventArgs(XboxController inXboxController)
        {
            // Written, 16.07.2018

            this.xboxController = inXboxController;
        }
    }
}
