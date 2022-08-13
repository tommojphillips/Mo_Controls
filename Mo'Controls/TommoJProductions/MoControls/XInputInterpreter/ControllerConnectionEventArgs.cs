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

        public void setData(XboxController inXboxController)
        {
            // Written, 05.08.2022

            xboxController = inXboxController;
        }
    }
}
