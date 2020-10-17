namespace TommoJProductions.MoControls.XInputInterpreter
{
    public class XboxJoystick
    {
        // Written, 21.07.2018

        #region Properties

        /// <summary>
        /// Represents all the thumbsticks <see cref="XboxControl"/>s in order of; left, right, up, down.
        /// </summary>
        public XboxFloatState[] controls
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the left input for the thumbstick.
        /// </summary>
        public XboxFloatState left
        {
            get
            {
                return this.controls[0];
            }
        }
        /// <summary>
        /// Represents the right input for the thumbstick.
        /// </summary>
        public XboxFloatState right
        {
            get
            {
                return this.controls[1];
            }
        }
        /// <summary>
        /// Represents the up input for the thumbstick.
        /// </summary>
        public XboxFloatState up
        {
            get
            {
                return this.controls[2];
            }
        }
        /// <summary>
        /// Represents the down input for the thumbstick.
        /// </summary>
        public XboxFloatState down
        {
            get
            {
                return this.controls[3];
            }
        }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initializes a new instance of <see cref="XboxJoystick"/>.
        /// </summary>
        /// <param name="inLeft">The left input.</param>
        /// <param name="inRight">The right input.</param>
        /// <param name="inUp">The up input.</param>
        /// <param name="inDown">The down input.</param>
        public XboxJoystick(XboxFloatState inLeft, XboxFloatState inRight, XboxFloatState inUp, XboxFloatState inDown)
        {
            // Written, 21.07.2018

            this.controls = new XboxFloatState[]
            {
                inLeft,
                inRight,
                inUp,
                inDown,
            };
        }

        #endregion
    }
}
