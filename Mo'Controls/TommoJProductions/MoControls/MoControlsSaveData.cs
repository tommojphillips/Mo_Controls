using System;
using TommoJProductions.MoControls.MouseEmulation;
using MSCLoader;

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
                ControlManager.loadControlInputsFromCInput();
                return new MoControlsSaveData()
                {
                    monitiorXboxControllerConnectionStatus = true,
                    emulateMouse = false,
                    mouseDeadzone = MouseEmulator.DEFAULT_DEADZONE,
                    mouseSensitivity = MouseEmulator.DEFAULT_SENSITIVITY,
                    mouseInputType = InputTypeEnum.RS,
                    displayCurrentPlayerModeOverlay = true,
                    footControls = new string[,]
                    {
                        { "Left", "None", "None", },
                        { "Right", "None", "None", },
                        { "Throttle", "None", "None", },
                        { "Brake", "None", "None", },
                        { "Clutch", "None", "None", },
                        { "ShiftUp", "None", "None", },
                        { "ShiftDown", "None", "None", },
                        { "IndicatorLeft", "None", "None", },
                        { "IndicatorRight", "None", "None", },
                        { "Range", "None", "None", },
                        { "HighBeam", "None", "None", },
                        { "Wipers", "None", "None", },
                        { "Boost", "None", "None", },
                        { "Handbrake", "None", "None", },
                        { "DrivingMode", "Return", "None", },
                        { "PlayerLeft", "A", "None", },
                        {  "PlayerRight", "D", "None", },
                        { "PlayerUp", "W", "None", },
                        { "PlayerDown", "S", "None", },
                        { "Jump", "Space", "None", },
                        { "Run", "LeftShift", "None", },
                        { "Zoom", "None", "None", },
                        { "Use", "F", "None", },
                        { "Crouch", "C", "None", },
                        { "Watch", "U", "None"},
                        { "ReachLeft", "Q", "None", },
                        { "ReachRight", "E", "None", },
                        { "Hitchhike", "O", "None", },
                        { "Swear", "N", "None", },
                        { "Hit", "H", "None", },
                        { "Push", "J", "None", },
                        { "Finger", "M", "None", },
                        { "Urinate", "P", "None", },
                        { "Drunk", "K", "None", },
                        { "Smoking", "I", "None", },
                        { "reverse", "None", "None", },
                        { "first", "None", "None", },
                        { "second", "None", "None", },
                        { "third", "None", "None", },
                        { "fourth", "None", "None", },
                        { "fifth", "None", "None", },
                        { "sixth", "None", "None", },
                    },
                    drivingControls = new string[,]
                    {
                        { "Left", "A", "None", },
                        { "Right", "D", "None", },
                        { "Throttle", "W", "None", },
                        { "Brake", "S", "None", },
                        { "Clutch", "X", "None", },
                        { "ShiftUp", "G", "None", },
                        { "ShiftDown", "B", "None", },
                        { "IndicatorLeft", "Semicolon", "None", },
                        { "IndicatorRight", "Quote", "None", },
                        { "Range", "R", "None", },
                        { "HighBeam", "L", "None", },
                        { "Wipers", "K", "None", },
                        { "Boost", "T", "None", },
                        { "Handbrake", "Z", "None", },
                        { "DrivingMode", "Return", "None", },
                        { "PlayerLeft", "None", "None", },
                        {  "PlayerRight", "None", "None", },
                        { "PlayerUp", "None", "None", },
                        { "PlayerDown", "None", "None", },
                        { "Jump", "None", "None", },
                        { "Run", "None", "None", },
                        { "Zoom", "None", "None", },
                        { "Use", "F", "None", },
                        { "Crouch", "None", "None", },
                        { "Watch", "U", "None"},
                        { "ReachLeft", "Q", "None", },
                        { "ReachRight", "E", "None", },
                        { "Hitchhike", "None", "None", },
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
                    playerSeenMscLoaderVersionError = false
                };
            }
        }

        #endregion

        #region Properties

        public static MoControlsSaveData loadedSaveData
        {
            get;
            private set;
        }
        public bool playerSeenMscLoaderVersionError
        {
            get;
            set;
        }
        public Debugging.DebugTypeEnum debugMode
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
        public static void saveSettings(MoControlsGO inMo_Controls)
        {
            // Written, 20.08.2018

            MoControlsSaveData mcsd = new MoControlsSaveData()
            {
                monitiorXboxControllerConnectionStatus = MoControlsGO.xboxControllerManager.monitorControllerConnections.Monitor,
                emulateMouse = MoControlsGO.mouseEmulator.Emulating,
                mouseDeadzone = MoControlsGO.mouseEmulator.deadzone,
                mouseSensitivity = MoControlsGO.mouseEmulator.sensitivity,
                mouseInputType = MoControlsGO.mouseEmulator.inputType,
                displayCurrentPlayerModeOverlay = MoControlsGO.controlManager.displayCurrentPlayerModeOverlay,
                footControls = MoControlsGO.controlManager.footControls,
                drivingControls = MoControlsGO.controlManager.drivingControls,
                debugMode = MoControlsMod.debug,
                playerSeenMscLoaderVersionError = MoControlsMod.instance.playerSeenMscLoaderVersionError,
            };
            saveSettings(MoControlsMod.instance, mcsd);
        }
        /// <summary>
        /// Saves the settings.
        /// </summary>
        public static void saveSettings(MoControlsMod inMo_Controls, MoControlsSaveData inMcsd)
        {
            // Written, 22.08.2018

            SaveLoad.SerializeSaveFile(inMo_Controls, inMcsd, fileName + fileExtention);
            MoControlsMod.print("saved mo'controls data.", Debugging.DebugTypeEnum.full);
        }
        /// <summary>
        /// Loads the settings.
        /// </summary>
        public static MoControlsSaveData loadSettings()
        {
            // Written, 20.08.2018

            float time = UnityEngine.Time.time;

            if (loadedSaveData is null)
            {
                bool createNewSaveFile = false;
                MoControlsSaveData mcsd = null;
                try
                {
                    mcsd = SaveLoad.DeserializeSaveFile<MoControlsSaveData>(MoControlsMod.instance, fileName + fileExtention);
                    if (mcsd == null)
                        throw new NullReferenceException();
                }
                catch (NullReferenceException)
                {
                    createNewSaveFile = true;                    
                        MoControlsMod.print("Save file does not exist, creating save file.", Debugging.DebugTypeEnum.full);
                }
                catch (Exception)
                {
                    createNewSaveFile = true;
                    MoControlsMod.print("An error occured while loading the save file.. overriding with new save file. Maybe mod updated?", Debugging.DebugTypeEnum.none);
                }
                if (createNewSaveFile)
                {
                    mcsd = defaultSave;
                    saveSettings(MoControlsMod.instance, mcsd);
                }
                MoControlsMod.print("loaded mo'controls data in <b>" + (time - UnityEngine.Time.time) + "s</b>.", Debugging.DebugTypeEnum.full);
                return mcsd;
            }
            else
            {
                MoControlsMod.print("Mo'Controls save data was already loaded; passed the save data on.", Debugging.DebugTypeEnum.full);
                return loadedSaveData;
            }
        }

        #endregion
    }
}
