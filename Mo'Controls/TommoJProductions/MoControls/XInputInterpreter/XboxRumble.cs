using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents an xbox controller rumble.
    /// </summary>
    public class XboxRumble
    {
        // Written, 16.07.2018

        #region Properties

        /// <summary>
        /// Represents the amount of time to rumble rumble
        /// </summary>
        public float timer
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the fade-out start time in seconds.
        /// </summary>
        public float duration
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the intensity of the rumble. (between 0.0f and 1.0f).
        /// </summary>
        public Vector2 power
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the timer.
        /// </summary>
        public void update()
        {
            // Written, 16.07.2018

            timer -= Time.deltaTime;
        }

        #endregion
    }
}
