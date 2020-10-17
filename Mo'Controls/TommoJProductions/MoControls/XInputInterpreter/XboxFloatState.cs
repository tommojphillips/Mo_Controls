using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents current and previous trigger states.
    /// </summary>
    public class XboxFloatState : XboxControl
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
        /// Initializes a new instance of <see cref="XboxFloatState"/>.
        /// </summary>
        /// <param name="inputName">The input for the trigger.</param>
        /// <param name="triggerName">The triggers name.</param>
        public XboxFloatState(string inputName, string triggerName) : base(inputName, triggerName, XboxControlTypeEnum.Axis)
        {
            // Written, 20.07.2018
        }

        #endregion

        /// <summary>
        /// Returns the current state rounded to 2 decimal places. Colored yellow if absolute value is greater than 0
        /// </summary>
        public override string ToString()
        {
            return string.Format("<color={0}><b>{1}</b></color>", Mathf.Abs(this.state) > 0 ? "yellow" : "white", this.state.ToString("0.00"));
        }
    }
}
