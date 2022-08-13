using MSCLoader;
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
                        { "Jump", "Space", "" },
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

        public string moControlsVersion = MoControlsMod.instance.Version;
        public Debugging.DebugTypeEnum debugMode = Debugging.DebugTypeEnum.none;
        public bool displayCurrentPlayerModeOverlay = true;
        public string[,] footControls = defaultFootControls;
        public string[,] drivingControls = defaultDrivingControls;
        public bool ffbOnXboxController = false;
        public bool displayFfbOverlay = false;
        public bool ffbOption_default = false;
        public bool ffbOption_rpmLimiter = false;
        public bool ffbOption_wheelSlip = true;
        public bool displayVehicleInfoOverlay = false;
        public bool ffbOption_gearChange = true;
        public bool combinedTriggerAxis = false;
        public UsingDeviceEnum deviceDetection = UsingDeviceEnum.Auto;
        public GamePadDeadZone xboxControllerDeadzoneType = GamePadDeadZone.IndependentAxes;
        public bool showHPatternShifterGameControls = false;
        public float monitorControllerConnectionsTimeStep = 5;

        public bool usePlayerMoveAsInput = true;
        public bool playerMoveUseRawInput = true;
        public float playerMoveDeadzone = 0;
        public DeadzoneTypeEnum playerMoveDeadzoneType = DeadzoneTypeEnum.ScaledRadial;
        public InputTypeEnum playerMove = InputTypeEnum.LS;
        public XboxButtonEnum playerJump = XboxButtonEnum.Y;

        public InputTypeEnum playerLook = InputTypeEnum.RS;
        public bool playerLookUseRawInput = true;
        public float playerLookDeadzone = 0;
        public float playerLookSensitivityMultiplier = 1;
        public DeadzoneTypeEnum playerLookDeadzoneType = DeadzoneTypeEnum.ScaledRadial;

        public bool emulateMouse =  true;
        public bool mouseInputUseRawInput = true;
        public float mouseDeadzone = 0;
        public float mouseSensitivity = 10;
        public InputTypeEnum mouseInputType = InputTypeEnum.RS;
        public DeadzoneTypeEnum mouseDeadzoneType = DeadzoneTypeEnum.ScaledRadial;

        #endregion

        #region Methods

        internal static void resetLoadedSettings()
        {
            // Written, 04.08.2022

            loadedSaveData = new MoControlsSaveData();
            loadedSaveData.saveSettings();
        }
        /// <summary>
        /// Saves this instance to a file.
        /// </summary>
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
