namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents current and previous trigger states.
    /// </summary>
    public class XboxTriggerState : XboxControl
    {
        // Written, 16.07.2018

        #region Properties

        /// <summary>
        /// Represents the previous state of the trigger.
        /// </summary>
        public float previousState
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the current state of the trigger.
        /// </summary>
        public float state
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="XboxTriggerState"/>.
        /// </summary>
        /// <param name="inputName">The input for the trigger.</param>
        /// <param name="triggerName">The triggers name.</param>
        public XboxTriggerState(string inputName, string triggerName) : base (inputName, triggerName, XboxControlTypeEnum.Axis)
        {
            // Written, 20.07.2018
        }

        #endregion
    }
}
