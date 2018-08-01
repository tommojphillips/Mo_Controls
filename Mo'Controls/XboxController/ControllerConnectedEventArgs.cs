using System;

namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents event data for the <see cref="XboxControllerManager.ControllerConnected"/> event.
    /// </summary>
    public class ControllerConnectedEventArgs : EventArgs
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
        /// Initalizes a new instance of, <see cref="ControllerConnectedEventArgs"/> and assigns <see cref="xboxController"/> to the parameter, <paramref name="inXboxController"/>.
        /// </summary>
        /// <param name="inXboxController">The xbox controller that was connected.</param>
        public ControllerConnectedEventArgs(XboxController inXboxController)
        {
            // Written, 16.07.2018

            this.xboxController = inXboxController;
        }
    }
}
