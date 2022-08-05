using MSCLoader;
using System;
using System.Collections.Generic;
using System.Collections;
using TommoJProductions.Debugging;
using TommoJProductions.MoControls.InputEmulation;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    // Mod References
    // XInputDotNetPure.dll - in folder: "My Summer Car/mysummercar_data/Managed"
    // XInputInterface.dll - in folder: "My Summer Car"

    /// <summary>
    /// Represents the mo'controls mod.
    /// </summary>
    public class MoControlsMod : Mod
    {
        // Written, 06.07.2018  (Project start date)     

        #region Mod Keybinds

        /// <summary>
        /// Represents a keybind to open/close the gui for the mod.
        /// </summary>
        public Keybind openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.Home);
        /// <summary>
        /// Represents the primary input for LMB
        /// </summary>
        public Keybind lmbPrimaryInput = new Keybind(MouseEmulator.LMB_INPUT_NAME + "1", "LMB Primary Input", KeyCode.JoystickButton0);
        /// <summary>
        /// Represents the secondary input for LMB
        /// </summary>
        public Keybind lmbSecondaryInput = new Keybind(MouseEmulator.LMB_INPUT_NAME + "2", "LMB Secondary", KeyCode.Keypad1);
        /// <summary>
        /// Represents the primary input for RMB
        /// </summary>
        public Keybind rmbPrimaryInput = new Keybind(MouseEmulator.RMB_INPUT_NAME + "1", "RMB Primary Input", KeyCode.JoystickButton1);
        /// <summary>
        /// Represents the secondary input for RMB
        /// </summary>
        public Keybind rmbSecondaryInput = new Keybind(MouseEmulator.RMB_INPUT_NAME + "2", "RMB Secondary", KeyCode.Keypad2);

        #endregion

        #region Mod Fields

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Description => description;
        public override string Author => "tommojphillips";
        public override string Version => VersionInfo.version;
        public override bool UseAssetsFolder => true;

        #endregion

        #region Fields

        private readonly string versionInfo = String.Format("(<color={0}-{1}</color>)", VersionInfo.IS_DEBUG_CONFIG ? "red>Debug" : "blue>Release", VersionInfo.IS_64_BIT ? "x64" : "x86");
        private readonly string description;

        /// <summary>
        /// Represents the moControls gameobject.
        /// </summary>
        private static GameObject moControlsGameObject;

        #endregion

        #region Properties

        /// <summary>
        /// Represents whether or not the assets are loaded.
        /// </summary>
        internal static bool assetsLoaded
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents all assets for the mod.
        /// </summary>
        internal static AssetHolder assets
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the <see cref="moControlsGO"/> name.
        /// </summary>
        internal string gameObjectName => String.Format("{0} v{1}", ID, Version);
        /// <summary>
        /// Represents the current instance of <see cref="MoControlsMod"/>.
        /// </summary>
        internal static MoControlsMod instance
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the mo_controls game object instance.
        /// </summary>
        internal static MoControlsGO moControlsGO
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public MoControlsMod()
        {
            // Written, 20.08.2018

            instance = this;
            
            description = "Implements full xbox controller support eg => rumble/ffb support, separate mode for controls (Foot, Driving), autodetect scrollable.\n" +
            $"Latest Release Date: {VersionInfo.lastestRelease} | \n" +
            $"Key to toggle the GUI is {openControlsGui.Key} or <b>hold down the back button on a connected xbox controller for (> 0.3sec)</b>";
            
            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> {2} ready</color>", Name, Version, versionInfo));
        }

        #endregion

        #region Methods

        /// <summary>
        /// checks if type equals debug.
        /// </summary>
        /// <param name="inDebugType">the debug type to compare.</param>
        private static bool debugTypeEquals(DebugTypeEnum inDebugType)
        {
            // Written, 15.10.2018

            if (MoControlsSaveData.loadedSaveData != null)
            {
                if (MoControlsSaveData.loadedSaveData.debugMode == DebugTypeEnum.full)
                    return true;
                if (MoControlsSaveData.loadedSaveData.debugMode == DebugTypeEnum.partial)
                    if (inDebugType == DebugTypeEnum.none)
                        return true;
                if (MoControlsSaveData.loadedSaveData.debugMode == inDebugType)
                    return true;
            }
            else
                return true;
            return false;
        }
        /// <summary>
        /// Prints a message to the console with the prefix of the mod name.
        /// </summary>
        /// <param name="inMessage"></param>
        internal static void print(string inMessage, DebugTypeEnum inDebugType)
        {
            // Written, 08.10.2018

            if (debugTypeEquals(inDebugType))
            {
                string _msg = String.Format("<i><color=grey>[{0}]</color> <b><color=green>>></color></b></i> {1}", instance.Name, inMessage);
                ModConsole.Print(_msg);
            }
        }
        /// <summary>
        /// Loads control asset textures.
        /// </summary>
        private void loadControllerAssets()
        {
            // Written, 20.07.2018

            try
            {
                AssetBundle ab = LoadAssets.LoadBundle(this, "mo_controls.unity3d");
                print("Asset bundle loaded successfully.", DebugTypeEnum.partial);
                assets = new AssetHolder(ab.LoadAllAssets<Texture2D>()) ?? throw new Exception("asset holder returned null");
                ab.Unload(false);
                assetsLoaded = true;

                print("Asset bundle unloaded successfully.", DebugTypeEnum.full);
            }
            catch (Exception ex)
            {
                print("<color=red>asset bundle threw error: " + ex.ToString() + ".</color>", DebugTypeEnum.partial);
                assetsLoaded = false;
            }
        }
         
        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Project start date, 06.07.2018 | Modified 28.10.2020   

            ConsoleCommand.Add(new DebugConsoleCommand());
            ConsoleCommand.Add(new DebugStatsCommand());
            ConsoleCommand.Add(new ListLoadedAssembliesConsoleCommand());
            ConsoleCommand.Add(new WriteCinputExternInputsCommand());
            ConsoleCommand.Add(new ChangeToolModeCommand());

            Keybind.Add(this, openControlsGui);
            Keybind.Add(this, lmbPrimaryInput);
            Keybind.Add(this, lmbSecondaryInput);
            Keybind.Add(this, rmbPrimaryInput);
            Keybind.Add(this, rmbSecondaryInput);

            MoControlsSaveData.loadSettings();
            loadControllerAssets();
            moControlsGameObject = new GameObject(gameObjectName);
            moControlsGO = moControlsGameObject.AddComponent<MoControlsGO>();

            print($"{Name} v{Version}: Loaded", DebugTypeEnum.none);                        
        }

        #endregion

    }
}
