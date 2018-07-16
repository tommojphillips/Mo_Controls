using XInputDotNetPure;

namespace Mo_Controls.XboxController
{
    public class XboxButton
    {
        // Written, 16.07.2018

        #region Properties

        public ButtonState previousState
        {
            get;
            set;
        }
        public ButtonState state
        {
            get;
            set;
        }

        #endregion
    }
}
