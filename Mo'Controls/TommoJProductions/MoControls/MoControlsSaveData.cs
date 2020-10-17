﻿using MSCLoader;
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
                        { "PlayerRight", "D", "None", },
                        { "PlayerUp", "W", "None", },
                        { "PlayerDown", "S", "None", },
                        { "Jump", "Space", "None", },
                        { "Run", "LeftShift", "None", },
                        { "Zoom", "None", "None", },
                        { "Use", "F", "None", },
                        { "Crouch", "C", "None", },
                        { "Watch", "U", "None" },
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
                        { "PlayerRight", "None", "None", },
                        { "PlayerUp", "None", "None", },
                        { "PlayerDown", "None", "None", },
                        { "Jump", "None", "None", },
                        { "Run", "None", "None", },
                        { "Zoom", "None", "None", },
                        { "Use", "F", "None", },
                        { "Crouch", "None", "None", },
                        { "Watch", "U", "None" },
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
                    playerSeenMscLoaderVersionError = false,
                    displayFfbOverlay = false,
                    ffbOnXboxController = false,
                    ffbHandledOnUpdateScheme = UnityRuntimeUpdateSchemesEnum.update,
                    moControlsVersion = MoControlsMod.instance.Version,
                    ffbOption_default = true,
                    ffbOption_rpm = false,
                    ffbOption_wheelSlip = false,
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
        public UnityRuntimeUpdateSchemesEnum ffbHandledOnUpdateScheme;
        public bool ffbOption_default;
        public bool ffbOption_rpm;
        public bool ffbOption_wheelSlip;
        public bool displayVehicalInfoOverlay;

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
                    throw new Exception("Old mocontrols save file.");
            }
            catch (NullReferenceException)
            {
                createNewSaveFile = true;
                MoControlsMod.print("Save file does not exist, creating save file.", Debugging.DebugTypeEnum.none);
            }
            catch (Exception e)
            {
                createNewSaveFile = true;
                MoControlsMod.print("An error occured while loading the save file.. overriding with new save file. Maybe mod updated?\nERROR: " + e.Message, Debugging.DebugTypeEnum.none);
            }
            finally
            {
                if (createNewSaveFile)
                {
                    defaultSave.saveSettings();
                }
            }
            loadedSaveData = mcsd;
            MoControlsMod.print("loaded mo'controls data.", Debugging.DebugTypeEnum.full);
            return mcsd;
        }

        #endregion
    }
}
