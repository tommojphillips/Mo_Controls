using XInputDotNetPure;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents an xbox controller button.
    /// </summary>
    public class XboxButton : XboxControl
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

        #region Constructors

        public XboxButton(string inInputName, string inXboxButtonName, XboxControlTypeEnum inType) : base(inInputName, inXboxButtonName, inType)
        {
            // Written, 20.07.2018
        }

        #endregion
    }
}
