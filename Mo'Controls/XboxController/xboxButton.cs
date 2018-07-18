using XInputDotNetPure;

namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents an xbox controller button.
    /// </summary>
    public class XboxButton
    {
        // Written, 16.07.2018

        #region Properties
        
        /// <summary>
        /// Represents the previous state for the xbox button.
        /// </summary>
        public ButtonState previousState
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the current state for the xbox button.
        /// </summary>
        public ButtonState state
        {
            get;
            set;
        }

        #endregion
    }
}
