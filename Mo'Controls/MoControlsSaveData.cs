using System;
using Mo_Controls.MouseEmulation;
using MSCLoader;

namespace Mo_Controls
{
    public class MoControlsSaveData
    {
        // Written, 20.08.2018

        #region Constants

        /// <summary>
        /// Represents the file extention for the save data file.
        /// </summary>
        public const string fileExtention = ".txt";
        /// <summary>
        /// Represents the file name for the save data file.
        /// </summary>
        public const string fileName = "Mo_ControlsSaveData";
        /// <summary>
        /// Represents the default save for the save file.
        /// </summary>
        public static MoControlsSaveData defaultSave
        {
            get
            {
                string[,] currentControlInputs = ControlManager.loadControlInputsFromCInput();
                return new MoControlsSaveData()
                {
                    showXboxDebugGui = false,
                    showXboxVirtualAxesGui = false,
                    monitiorXboxControllerConnectionStatus = true,
                    emulateMouse = false,
                    mouseDeadzone = MouseEmulator.DEFAULT_DEADZONE,
                    mouseSensitivity = MouseEmulator.DEFAULT_SENSITIVITY,
                    mouseInputType = InputTypeEnum.RS,
                    displayCurrentPlayerModeOverlay = true,
                    footControls = currentControlInputs,
                    drivingControls = currentControlInputs,
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
        public bool displayCurrentPlayerModeOverlay
        {
            get;
            set;
        }
        public string[,] footControls
        {
            get;
            set;
        }
        public string[,] drivingControls
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public static void saveSettings(Mo_Controls mo_Controls)
        {
            // Written, 20.08.2018

            MoControlsSaveData mcsd = new MoControlsSaveData()
            {
                showXboxVirtualAxesGui = mo_Controls.moControlsGui.showVirtualGui,
                monitiorXboxControllerConnectionStatus = mo_Controls.xboxControllerManager.monitorControllerConnections.Monitor,
                emulateMouse = mo_Controls.mouseEmulator.Emulating,
                mouseDeadzone = mo_Controls.mouseEmulator.deadzone,
                mouseSensitivity = mo_Controls.mouseEmulator.sensitivity,
                mouseInputType = mo_Controls.mouseEmulator.inputType,
                displayCurrentPlayerModeOverlay = mo_Controls.controlManager.displayCurrentPlayerModeOverlay,
                footControls = mo_Controls.controlManager.footControls,
                drivingControls = mo_Controls.controlManager.drivingControls,
            };
            SaveLoad.SerializeSaveFile(mo_Controls, mcsd, fileName + fileExtention);
        }
        /// <summary>
        /// Saves the settings.
        /// </summary>
        public static void saveSettings(Mo_Controls mo_Controls, MoControlsSaveData mcsd)
        {
            // Written, 22.08.2018

            SaveLoad.SerializeSaveFile(mo_Controls, mcsd, fileName + fileExtention);
        }
        /// <summary>
        /// Loads the settings.
        /// </summary>
        public static MoControlsSaveData loadSettings(Mo_Controls mo_Controls)
        {
            // Written, 20.08.2018

            MoControlsSaveData mcsd;
            try
            {
                mcsd = SaveLoad.DeserializeSaveFile<MoControlsSaveData>(mo_Controls, fileName + fileExtention);
                if (mcsd == null)
                    throw new NullReferenceException();
            }
            catch (NullReferenceException)
            {
                mcsd = defaultSave;
            }
            return mcsd;            
        }

        #endregion
    }
}
