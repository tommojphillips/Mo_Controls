using UnityEngine;

namespace TommoJProdutions.MoControls.XInputInterpreter
{
    public class XboxControl
    {
        #region Properties

        /// <summary>
        /// Represents the input name of the control.
        /// </summary>
        public string inputName
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the xbox controls name (display name).
        /// </summary>
        public string name
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the image representation of the xbox control.
        /// </summary>
        public Texture2D texture
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the type of control.
        /// </summary>
        public XboxControlTypeEnum controlType
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="XboxControl"/>.
        /// </summary>
        /// <param name="inInputName">The input for the xbox control.</param>
        /// <param name="inName">The display name for the xbox control.</param>
        /// <param name="inType">The type of xbox control.</param>
        public XboxControl(string inInputName, string inName, XboxControlTypeEnum inType)
        {
            // Written, 20.07.2018

            this.inputName = inInputName;
            this.name = inName;
            this.controlType = inType;
        }

        #endregion
    }
}
