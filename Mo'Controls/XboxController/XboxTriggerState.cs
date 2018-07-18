namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents current and previous trigger states.
    /// </summary>
    public class XboxTriggerState
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
    }
}
