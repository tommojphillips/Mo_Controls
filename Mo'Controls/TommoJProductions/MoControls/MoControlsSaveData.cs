using MSCLoader;
using System;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;

namespace TommoJProductions.MoControls
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
                return new MoControlsSaveData()
                {
                    monitiorXboxControllerConnectionStatus = true,
                    emulateMouse = true,
                    mouseDeadzone = MouseEmulator.DEFAULT_DEADZONE,
                    mouseSensitivity = MouseEmulator.DEFAULT_SENSITIVITY,
                    mouseDeadzoneType = DeadzoneTypeEnum.ScaledRadial,
                    mouseInputType = InputTypeEnum.RS,
                    displayCurrentPlayerModeOverlay = true,
                    footControls = new string[,]
                    {
                        { "Left", "None", "None" },
                        { "Right", "None", "None" },
                        { "Throttle", "None", "None" },
                        { "Brake", "None", "None" },
                        { "Clutch", "None", "None" },
                        { "ShiftUp", "None", "None" },
                        { "ShiftDown", "None", "None" },
                        { "IndicatorLeft", "None", "None" },
                        { "IndicatorRight", "None", "None" },
                        { "Range", "None", "None" },
                        { "HighBeam", "None", "None" },
                        { "Wipers", "None", "None" },
                        { "Boost", "None", "None" },
                        { "Handbrake", "None", "None" },
                        { "DrivingMode", "Return", "JoystickButton6" },
                        { "PlayerLeft", "A", "Joy Axis 1-" },
                        { "PlayerRight", "D", "Joy Axis 1+" },
                        { "PlayerUp", "W", "Joy Axis 2-" },
                        { "PlayerDown", "S", "Joy Axis 2+" },
                        { "Jump", "Space", "JoystickButton3" },
                        { "Run", "LeftShift", "JoystickButton8" },
                        { "Zoom", "LeftControl", "Joy Axis 7+" },
                        { "Use", "F", "JoystickButton2" },
                        { "Crouch", "C", "JoystickButton9" },
                        { "Watch", "U", "None" },
                        { "ReachLeft", "Q", "JoystickButton4" },
                        { "ReachRight", "E", "None" },
                        { "Hitchhike", "O", "None" },
                        { "Swear", "N", "None" },
                        { "Hit", "H", "JoystickButton5" },
                        { "Push", "J", "Joy Axis 7-" },
                        { "Finger", "M", "Joy Axis 6-" },
                        { "Urinate", "P", "None" },
                        { "Drunk", "K", "None" },
                        { "Smoking", "I", "Joy Axis 6+" },
                        { "reverse", "None", "None" },
                        { "first", "None", "None" },
                        { "second", "None", "None" },
                        { "third", "None", "None" },
                        { "fourth", "None", "None" },
                        { "fifth", "None", "None" },
                        { "sixth", "None", "None" }
                    },
                    drivingControls = new string[,]
                    {
                        { "Left", "A", "Joy Axis 1-", },
                        { "Right", "D", "Joy Axis 1+", },
                        { "Throttle", "W", "Joy Axis 10+", },
                        { "Brake", "S", "Joy Axis 9+", },
                        { "Clutch", "X", "Joy Axis 2-", },
                        { "ShiftUp", "G", "JoystickButton5", },
                        { "ShiftDown", "B", "JoystickButton4", },
                        { "IndicatorLeft", "Semicolon", "None", },
                        { "IndicatorRight", "Quote", "None", },
                        { "Range", "R", "JoystickButton9", },
                        { "HighBeam", "L", "Joy Axis 7-", },
                        { "Wipers", "K", "None", },
                        { "Boost", "T", "None", },
                        { "Handbrake", "Z", "JoystickButton3", },
                        { "DrivingMode", "Return", "JoystickButton6", },
                        { "PlayerLeft", "None", "None", },
                        { "PlayerRight", "None", "None", },
                        { "PlayerUp", "None", "None", },
                        { "PlayerDown", "None", "None", },
                        { "Jump", "None", "None", },
                        { "Run", "None", "None", },
                        { "Zoom", "LeftControl", "Joy Axis 7+", },
                        { "Use", "F", "JoystickButton2", },
                        { "Crouch", "None", "None", },
                        { "Watch", "U", "None" },
                        { "ReachLeft", "Q", "Joy Axis 6-", },
                        { "ReachRight", "E", "Joy Axis 6+", },
                        { "Hitchhike", "O", "None", },
                        { "Swear", "N", "None", },
                        { "Hit", "H", "None", },
                        { "Push", "None", "None", },
                        { "Finger", "M", "None", },
                        { "Urinate", "None", "None", },
                        { "Drunk", "J", "None", },
                        { "Smoking", "I", "None", },
                        { "reverse", "None", "None", },
                        { "first", "None", "None", },
                        { "second", "None", "None", },
                        { "third", "None", "None", },
                        { "fourth", "None", "None", },
                        { "fifth", "None", "None", },
                        { "sixth", "None", "None", },
                    },
                    debugMode = Debugging.DebugTypeEnum.none,
                    playerSeenMscLoaderVersionError = false,
                    displayFfbOverlay = false,
                    ffbOnXboxController = false,
                    moControlsVersion = MoControlsMod.VERSION,
                    ffbOption_default = false,
                    ffbOption_rpmLimiter = false,
                    ffbOption_wheelSlip = true,
                    displayVehicleInfoOverlay = false,
                    ffbOption_gearChange = true,
                    combinedTriggerAxis = false,
                    horzDeadzone = 0.001f,
                    horzGravity = 3,
                    horzSensitivity = 3,
                    vertDeadzone = 0.001f,
                    vertGravity = 3,
                    vertSensitivity = 3,
                    playerHorzDeadzone = 0.05f,
                    playerHorzGravity = 3,
                    playerHorzSensitivity = 3,
                    playerVertDeadzone = 0.05f,
                    playerVertGravity = 3,
                    playerVertSensitivity = 3,
                    oldNewVersionAccepted = null,
                };
            }
        }   

        #endregion

        #region Properties

        public string moControlsVersion;
        public static MoControlsSaveData loadedSaveData;
        public bool playerSeenMscLoaderVersionError;
        public Debugging.DebugTypeEnum debugMode;
        public bool monitiorXboxControllerConnectionStatus;
        public bool emulateMouse;
        public InputTypeEnum mouseInputType;
        public DeadzoneTypeEnum mouseDeadzoneType;
        public float mouseDeadzone;
        public float mouseSensitivity;
        public bool displayCurrentPlayerModeOverlay;
        public string[,] footControls;
        public string[,] drivingControls;
        public bool ffbOnXboxController;
        public bool displayFfbOverlay;
        public bool ffbOption_default;
        public bool ffbOption_rpmLimiter;
        public bool ffbOption_wheelSlip;
        public bool displayVehicleInfoOverlay;
        public bool ffbOption_gearChange;
        public bool combinedTriggerAxis;
        public float playerHorzDeadzone;
        public float playerHorzGravity;
        public float playerHorzSensitivity;
        public float playerVertDeadzone;
        public float playerVertGravity;
        public float playerVertSensitivity;
        public float horzDeadzone;
        public float horzGravity;
        public float horzSensitivity;
        public float vertDeadzone;
        public float vertGravity;
        public float vertSensitivity;
        public string oldNewVersionAccepted;

        #endregion

        #region Fields

        private static OldSaveDataException oldSaveException;

        #endregion

        #region Methods

        internal void saveSettings()
        {
            // Written, 17.10.2020

            SaveLoad.SerializeSaveFile(MoControlsMod.instance, this, fileName + fileExtention);
            MoControlsMod.print("saved mo'controls data.", Debugging.DebugTypeEnum.full);
        }
        internal static MoControlsSaveData loadSettings()
        {
            // Written, 17.10.2020

            bool createNewSaveFile = false;
            MoControlsSaveData mcsd = null;
            try
            {
                mcsd = SaveLoad.DeserializeSaveFile<MoControlsSaveData>(MoControlsMod.instance, fileName + fileExtention);
                if (mcsd == null)
                    throw new NullReferenceException();

                if (mcsd.moControlsVersion != MoControlsMod.instance.Version)
                {
                    if (mcsd.oldNewVersionAccepted != mcsd.moControlsVersion)
                    {
                        oldSaveException = new OldSaveDataException(mcsd);
                        throw oldSaveException;
                    }
                }
            }
            catch (NullReferenceException)
            {
                createNewSaveFile = true;
                MoControlsMod.print("Save file does not exist, make any change to a setting to save.", Debugging.DebugTypeEnum.none);
            }
            catch (OldSaveDataException e)
            {
                    createNewSaveFile = true;
                    ModUI.ShowYesNoMessage(string.Format("A{1} save detected (v{0}).. Would you like to still load this save file? NOTE: you may need to change various settings",
                        e.oldSaveData.moControlsVersion,
                        MoControlsMod.determineIsVersionOldCurrentOrNew(mcsd.moControlsVersion) < 0 ? "n Older" : " Newer"),
                        "[Mo'Controls] - Different save version detected", keepSaveFile);
            }
            catch (Exception e)
            {
                createNewSaveFile = true;
                MoControlsMod.print("An error occured while loading the save file..make any change to a setting to save.\nERROR: " + e.Message, Debugging.DebugTypeEnum.none);
            }
            if (createNewSaveFile)
            {
                mcsd = defaultSave;
                mcsd.saveSettings();
            }
            loadedSaveData = mcsd;
            MoControlsMod.print("loaded mo'controls data.", Debugging.DebugTypeEnum.full);
            return mcsd;
        }

        private static void keepSaveFile()
        {
            loadedSaveData = oldSaveException.oldSaveData;
            loadedSaveData.oldNewVersionAccepted = loadedSaveData.moControlsVersion;
            loadedSaveData.saveSettings();
        }

        #endregion
    }
}
