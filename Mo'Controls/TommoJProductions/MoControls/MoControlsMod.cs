using MSCLoader;
using System;
using System.Collections.Generic;
using System.Collections;
using TommoJProductions.Debugging;
//using TommoJProductions.MoControls.InputEmulation;
using UnityEngine;
using System.IO;
using System.Reflection;
using TommoJProductions.MoControls.InputEmulation;

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
        public Keybind openControlsGui;
        /// <summary>
        /// Represents the primary input for LMB
        /// </summary>
        public Keybind lmbPrimaryInput;
        /// <summary>
        /// Represents the secondary input for LMB
        /// </summary>
        public Keybind lmbSecondaryInput;
        /// <summary>
        /// Represents the primary input for RMB
        /// </summary>
        public Keybind rmbPrimaryInput;
        /// <summary>
        /// Represents the secondary input for RMB
        /// </summary>
        public Keybind rmbSecondaryInput;

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
        private static FieldInfo _modsFolderFieldInfo;

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
        /// <summary>
        /// Gets modloaer mods folder field info.
        /// </summary>
        private static FieldInfo getModsFolderFi
        {
            // Written, 02.07.2022

            get
            {
                if (_modsFolderFieldInfo == null)
                    _modsFolderFieldInfo = typeof(ModLoader).GetField("ModsFolder", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
                return _modsFolderFieldInfo;
            }
        }
        /// <summary>
        /// Gets the currently used mods folder path.
        /// </summary>
        private static string getModsFolder => (string)getModsFolderFi.GetValue(null);

        #endregion

        #region Constructors

        static MoControlsMod() 
        {
            // Written, 12.08.2022

            checkModInstalledCorrectly();
            logRunningEnviroment();
        }

        public MoControlsMod()
        {
            // Written, 20.08.2018, modified, 12.08.2022
            
            if (!installedCorrectly)
                throw new FileNotFoundException("Mo'Controls is not installed correctly.");

            instance = this;
                        
            openControlsGui = new Keybind("OpenControls", "Open Controls GUI", KeyCode.Home);
            lmbPrimaryInput = new Keybind(InputEmulation.MouseEmulator.LMB_INPUT_NAME + "1", "LMB Primary Input", KeyCode.JoystickButton0);
            lmbSecondaryInput = new Keybind(InputEmulation.MouseEmulator.LMB_INPUT_NAME + "2", "LMB Secondary", KeyCode.Keypad1);
            rmbPrimaryInput = new Keybind(InputEmulation.MouseEmulator.RMB_INPUT_NAME + "1", "RMB Primary Input", KeyCode.JoystickButton1);
            rmbSecondaryInput = new Keybind(InputEmulation.MouseEmulator.RMB_INPUT_NAME + "2", "RMB Secondary", KeyCode.Keypad2);

            description = "Implements full xbox controller support eg => rumble/ffb support, separate mode for controls (Foot, Driving), autodetect scrollable.\n" +
            $"Latest Release Date: {VersionInfo.lastestRelease} | Key to toggle the GUI is {openControlsGui.Key} or <b>hold down the back button on a connected xbox controller for (> 0.3sec)</b>";

            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> {2} ready</color>", Name, Version, versionInfo));
        }

        #endregion

        #region Methods

        private static void logRunningEnviroment() 
        {
            // Written, 12.08.2022

            UnityEngine.Debug.Log("Running Enviroment:\n-----------------");
            UnityEngine.Debug.Log($"OS: {Application.platform}");
            UnityEngine.Debug.Log($"Mo'Controls installed correctly: {installedCorrectly}");
            UnityEngine.Debug.Log($"Game directory: {gameDirectory}");
            UnityEngine.Debug.Log($"Mods folder: {getModsFolder}");
            UnityEngine.Debug.Log("-----------------");
        }

        private static bool installedCorrectly = false;
        private static List<string> filesNotInstalledCorrectly;
        private static string gameDirectory;

        private static void checkModInstalledCorrectly() 
        {
            // Written, 12.08.2022

            installedCorrectly = true;
            filesNotInstalledCorrectly = new List<string>();
            gameDirectory = Path.GetFullPath(".");
            string modsFolder = getModsFolder;
            string xInputInterfaceFileName = "XInputInterface.dll";
            string xInputInterfacePath = gameDirectory + "/";
            string XInputPureFileName = "XInputDotNetPure.dll";
            string xInputPurePathPrefered = modsFolder + "/References/";
            string xInputPurePathAlt = gameDirectory + "/mysummercar_Data/Managed/";
            string assetBundleFileName = "mo_controls.unity3d";
            string assetBundlePath = modsFolder + "/Assets/Mo_Controls/";

            if (!File.Exists(xInputInterfacePath + xInputInterfaceFileName))
            {
                installedCorrectly = false;
                filesNotInstalledCorrectly.Add(xInputInterfaceFileName);
                filesNotInstalledCorrectly.Add($" should be here: '<b><color=yellow>{xInputInterfacePath}</color></b>' \n");
            }

            if (!File.Exists(xInputPurePathPrefered + XInputPureFileName) && !File.Exists(xInputPurePathAlt + XInputPureFileName))
            {
                installedCorrectly = false;
                filesNotInstalledCorrectly.Add(XInputPureFileName);
                filesNotInstalledCorrectly.Add($" should be here: '<b><color=yellow>{xInputPurePathPrefered}</color></b>' \n");
                //filesNotInstalledCorrectly.Add($" or here: '<b><color=yellow>{xInputPurePathAlt}</color></b>' \n");
            }

            if (!File.Exists( assetBundlePath + assetBundleFileName))
            {
                installedCorrectly = false;
                filesNotInstalledCorrectly.Add(assetBundleFileName);
                filesNotInstalledCorrectly.Add($" should be here: '<b><color=yellow>{assetBundlePath}</color></b>' \n");
            }

            if (!installedCorrectly)
            {
                string error = $"Mod is <b>not installed correctly</b>, thus Mo'Controls wont function correctly!\n" +
                    $"Files that aren't installed correctly:\n{string.Join("- ", filesNotInstalledCorrectly.ToArray())}";
                MSCLoader.ModUI.ShowMessage(error, "Mo'Controls: ERROR");
                error = error
                    .Replace("<b>", "")
                    .Replace("</b>", "")
                    .Replace("<color=yellow>", "")
                    .Replace("</color>", "");
                UnityEngine.Debug.LogError(error);
            }
        }

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
                print("<color=red>asset bundle threw error: " + ex.ToString() + ".</color>", DebugTypeEnum.none);
                assetsLoaded = false;
            }
        }

        #endregion

        #region Override Methods

        public override void ModSetup() 
        {
            // Written, 12.08.2022

            SetupFunction(Setup.OnLoad, onLoad);
        }

        public void onLoad()
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
