using MSCLoader;
using System;
using System.IO;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using XInputDotNetPure;

namespace TommoJProductions.MoControls
{
    public class MoControlsSaveData
    {
        // Written, 20.08.2018

        #region Constants / Properties

        /// <summary>
        /// Represents the file name for the save data file.
        /// </summary>
        public const string fileName = "Mo_ControlsSaveData.txt";
        /// <summary>
        /// Represents the default save for the save file.
        /// </summary>
        public static MoControlsSaveData defaultSave
        {
            get
            {
                return new MoControlsSaveData()
                {
                    moControlsVersion = MoControlsMod.instance.Version,
                    debugMode = Debugging.DebugTypeEnum.none,
                    emulateMouse = true,
                    mouseInputType = InputTypeEnum.RS,
                    mouseDeadzoneType = DeadzoneTypeEnum.ScaledRadial,
                    mouseDeadzone = MouseEmulator.DEFAULT_DEADZONE,
                    mouseSensitivity = MouseEmulator.DEFAULT_SENSITIVITY,
                    displayCurrentPlayerModeOverlay = true,
                    footControls = defaultFootControls,
                    drivingControls = defaultDrivingControls,
                    displayFfbOverlay = false,
                    ffbOnXboxController = false,
                    ffbOption_default = false,
                    ffbOption_rpmLimiter = false,
                    ffbOption_wheelSlip = true,
                    displayVehicleInfoOverlay = false,
                    ffbOption_gearChange = true,
                    combinedTriggerAxis = false,
                    deviceDetection = UsingDeviceEnum.Auto,
                    playerMove = InputTypeEnum.LS,
                    playerJump = XboxButtonEnum.Y,
                    usePlayerMoveAsInput = true,
                    xboxControllerDeadzoneType = GamePadDeadZone.IndependentAxes                    
                };
            }
        }
        /// <summary>
        /// returns new defualt array of driving controls
        /// </summary>
        public static string[,] defaultDrivingControls 
        {
            get
            {
                return new string[,]
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
                };
            }
        }
        /// <summary>
        /// returns new defualt array of foot controls
        /// </summary>
        public static string[,] defaultFootControls
        {
            get
            {
                return new string[,]
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
                        { "PlayerLeft", "A", "" },
                        { "PlayerRight", "D", "" },
                        { "PlayerUp", "W", "" },
                        { "PlayerDown", "S", "" },
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
                };
            }
        }
        /// <summary>
        /// represents current used save data in the system.
        /// </summary>
        public static MoControlsSaveData loadedSaveData { get; private set; }

        #endregion

        #region save data fields

        public string moControlsVersion;
        public Debugging.DebugTypeEnum debugMode;
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
        public UsingDeviceEnum deviceDetection;
        public InputTypeEnum playerMove;
        public XboxButtonEnum playerJump;
        public bool usePlayerMoveAsInput;
        public GamePadDeadZone xboxControllerDeadzoneType;

        #endregion

        #region Methods

        internal static void resetLoadedSettings()
        {
            // Written, 04.08.2022

            loadedSaveData = defaultSave;
            loadedSaveData.saveSettings();
        }
        internal void saveSettings()
        {
            // Written, 17.10.2020

            SaveLoad.SerializeSaveFile(MoControlsMod.instance, this, fileName);
            MoControlsMod.print("saved mo'controls data.", Debugging.DebugTypeEnum.full);
        }
        internal static MoControlsSaveData loadSettings()
        {
            // Written, 17.10.2020

            bool createNewSaveFile = false;
            MoControlsSaveData mcsd = null;
            if (File.Exists(Path.Combine(ModLoader.GetModSettingsFolder(MoControlsMod.instance), fileName)))
            {
                mcsd = SaveLoad.DeserializeSaveFile<MoControlsSaveData>(MoControlsMod.instance, fileName);
            }
            else
            {
                createNewSaveFile = true;
                MoControlsMod.print("Save file does not exist. creating new save file.", Debugging.DebugTypeEnum.none);
            }
            if (createNewSaveFile)
            {
                resetLoadedSettings();
            }
            else
            {
                loadedSaveData = mcsd;
            }
            MoControlsMod.print("loaded mo'controls data.", Debugging.DebugTypeEnum.full);
            return mcsd;
        }

        #endregion
    }
}
