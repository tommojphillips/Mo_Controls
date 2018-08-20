using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mo_Controls.MouseEmulation;

namespace Mo_Controls
{
    public class MoControlsSaveData
    {
        // Written, 20.08.2018

        #region Constants

        /// <summary>
        /// Represents the file extention for the save data file.
        /// </summary>
        public const string fileExtention = ".MCSaveData";
        /// <summary>
        /// Represents the file name for the save data file.
        /// </summary>
        public const string fileName = "MoControlsSaveData";
        /// <summary>
        /// Represents the default save for the save file.
        /// </summary>
        public static MoControlsSaveData defaultSave
        {
            get
            {
                return new MoControlsSaveData()
                {
                    showXboxDebugGui = false,
                    showXboxVirtualAxesGui = false,
                    monitiorXboxControllerConnectionStatus = true,
                    emulateMouse = false,
                    mouseDeadzone = MouseEmulator.DEFAULT_DEADZONE,
                    mouseSensitivity = MouseEmulator.DEFAULT_SENSITIVITY,
                    mouseInputType = InputTypeEnum.RS,
                };
            }
        }

        #endregion

        #region Fields

        public bool showXboxDebugGui
        {
            get;
            set;
        }
        public bool showXboxVirtualAxesGui
        {
            get;
            set;
        }
        public bool monitiorXboxControllerConnectionStatus
        {
            get;
            set;
        }
        public bool emulateMouse
        {
            get;
            set;
        }
        public InputTypeEnum mouseInputType
        {
            get;
            set;
        }
        public float mouseDeadzone
        {
            get;
            set;
        }
        public float mouseSensitivity
        {
            get;
            set;
        }
        public string lmbPrimaryInput
        {
            get;
            set;
        }
        public string lmbSecondaryInput
        {
            get;
            set;
        }
        public string rmbPrimaryInput
        {
            get;
            set;
        }
        public string rmbSecondaryInput
        {
            get;
            set;
        }

        #endregion
    }
}
