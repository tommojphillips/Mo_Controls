using System;

namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents event data for the <see cref="XboxControllerManager.ControllerDisconnected"/> event.
    /// </summary>
    public class ControllerDisconnectedEventArgs : EventArgs
    {
        // Written, 16.07.2018

        /// <summary>
        /// Represents the xbox controller that was disconnect.
        /// </summary>
        public XboxController xboxController
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ControllerDisconnectedEventArgs"/> and assingns, <see cref="xboxController"/> to the parameter, <paramref name="inXboxController"/>.
        /// </summary>
        public ControllerDisconnectedEventArgs(XboxController inXboxController)
        {
            // Written, 16.07.2018

            this.xboxController = inXboxController;
        }
    }
}
