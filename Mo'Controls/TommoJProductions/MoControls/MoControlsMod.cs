using MSCLoader;
using System;
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

        /// <summary>
        /// Represents whether this is a release version
        /// </summary>
        internal static bool isReleaseVersion => true;
        /// <summary>
        /// Represents the current version.
        /// </summary>
        /// 
        internal const string VERSION = "1.1.6";
        /// <summary>
        /// Represents the latest release version.
        /// </summary>
        internal const string LATEST_RELEASE_DATE = "30.05.2022";


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
        public override string Author => "tommojphillips";
        public override string Version => VERSION;
        public override bool UseAssetsFolder => true;
        public override bool SecondPass => true;
        #endregion

        #region Fields

        /// <summary>
        /// Represents the moControls gameobject.
        /// </summary>
        private static GameObject moControlsGameObject;
        /// <summary>
        /// Represents the supported/compatible version of mod loader.
        /// </summary>
        public const string SUPPORTED_MODLOADER_VERSION = "1.1.16";

#if DEBUG
        private const bool IS_DEBUG_CONFIG = true;
#else
        private const bool IS_DEBUG_CONFIG = false;
#endif

#if x64
        private const bool IS_X64 = true;
#else
        private const bool IS_x64 = false;
#endif

        #endregion

        #region Properties

        /// <summary>
        /// Represents the release version name.
        /// </summary>
        internal string releaseVersionName
        {
            get
            {
                return String.Format("(<color={0}-{2}</color>) {1}", isReleaseVersion ? "blue>Release" : "red>Pre-Release", IS_X64 ? "x64" : "x86", IS_DEBUG_CONFIG ? "DEBUG" : "R");
            }
        }
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
            performDebugCheck();
            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> {2} ready</color>", Name, Version, releaseVersionName));
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
                assets = new AssetHolder(ab.LoadAllAssets<Texture2D>()) ?? throw new Exception("asset holder return an error");
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
        /// <summary>
        /// Performs a check to see if the mod loader version is the same as the currently supported version.
        /// </summary>
        private void performModLoaderVersionCheck()
        {
            // Written, 03.08.2018

            // Modloader supported version check.
            if (SUPPORTED_MODLOADER_VERSION != ModLoader.MSCLoader_Ver)
            {
                if (!MoControlsSaveData.loadedSaveData.playerSeenMscLoaderVersionError)
                {
                    ModUI.ShowMessage(
                        String.Format("<b>[{0} <color=orange>v{1}]</color></b> - NOTE: modloader v{2} may not be <color=orange>compatible</color>.\r\nSupported " +
                        "modloader version is <color=orange>v{3}</color>.",
                        Name, Version, ModLoader.MSCLoader_Ver, SUPPORTED_MODLOADER_VERSION), "ModLoader Version not supported.");
                    MoControlsSaveData.loadedSaveData.playerSeenMscLoaderVersionError = true;
                    MoControlsSaveData.loadedSaveData.saveSettings();
                }
                print("<color=orange>Warning</color> <color=grey>Supported modloader version is <b>v" + SUPPORTED_MODLOADER_VERSION + "</b>; you're running version <b>" + ModLoader.MSCLoader_Ver + "</b>. May not be compatible with current version.</color>.", DebugTypeEnum.partial);
            }
            else
            {
                print("<color=grey>Running supported modloader version, <color=green>" + SUPPORTED_MODLOADER_VERSION + "</color></color>", DebugTypeEnum.full);
                if (MoControlsSaveData.loadedSaveData.playerSeenMscLoaderVersionError)
                {
                    MoControlsSaveData.loadedSaveData.playerSeenMscLoaderVersionError = false;
                    MoControlsSaveData.loadedSaveData.saveSettings();
                }
            }
        }
        /// <summary>
        /// Initializes all mscmodloader commands and loads controller assets. (textures)
        /// </summary>
        private void initialize()
        {
            // Written, 17.10.2018

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

            loadControllerAssets();
        }
        /// <summary>
        /// Represents a debug/release mode check.
        /// </summary>
        private void performDebugCheck()
        {
            // Written, 28.12.2018

#if DEBUG
            if (isReleaseVersion)
                ModUI.ShowMessage("<color=orange>Warning</color>: Debug Configuration invaild. Was this intentional Tommo? (dev message)" +
                    "\r\n" +
                    "\r\nRelease version: " + isReleaseVersion +
                    "\r\nDebug Config: True",
                    "<color=orange>Warning</color>: <b>Incorrect Debug/Releaase Configuration</b>");
#else
            if (!isReleaseVersion)
                ModUI.ShowMessage("<color=orange>Warning</color>: Release Configuration invaild" +
                    "\r\nWas this intentional Tommo? (dev message)" +
                    "\r\n" +
                    "\r\nRelease version: " + isReleaseVersion +
                    "\r\nDebug Config: False",
                    "<color=orange>Warning</color>: <b>Incorrect Release/Debug Configuration</b>");
#endif
        }

        private void loadCInputAxisSettings()
        {
            // Written, 23.10.2020

            // setting cInput axis deadzones//
            /*external cinput control names = PlayerVertical*PlayerHorizontal*Horizontal*Vertical*/
            cInput.SetAxisDeadzone("PlayerHorizontal", MoControlsSaveData.loadedSaveData.playerHorzDeadzone);
            cInput.SetAxisSensitivity("PlayerHorizontal", MoControlsSaveData.loadedSaveData.playerHorzSensitivity);
            cInput.SetAxisGravity("PlayerHorizontal", MoControlsSaveData.loadedSaveData.playerHorzGravity);
            cInput.SetAxisDeadzone("PlayerVertical", MoControlsSaveData.loadedSaveData.playerVertDeadzone);
            cInput.SetAxisSensitivity("PlayerVertical", MoControlsSaveData.loadedSaveData.playerVertSensitivity);
            cInput.SetAxisGravity("PlayerVertical", MoControlsSaveData.loadedSaveData.playerVertGravity);
            cInput.SetAxisDeadzone("Horizontal", MoControlsSaveData.loadedSaveData.horzDeadzone);
            cInput.SetAxisSensitivity("Horizontal", MoControlsSaveData.loadedSaveData.horzSensitivity);
            cInput.SetAxisGravity("Horizontal", MoControlsSaveData.loadedSaveData.horzGravity);
            cInput.SetAxisDeadzone("Vertical", MoControlsSaveData.loadedSaveData.vertDeadzone);
            cInput.SetAxisSensitivity("Vertical", MoControlsSaveData.loadedSaveData.vertSensitivity);
            cInput.SetAxisGravity("Vertical", MoControlsSaveData.loadedSaveData.vertGravity);
            MoControlsMod.print("loaded cinput axis settings (grav, dead, sens).. hold <i>Left-Ctrl</i> through Mo'Controls' second pass loading sequ to disable", DebugTypeEnum.none);//.full);
        }

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Project start date, 06.07.2018 | Modified 28.10.2020   

            MoControlsSaveData.loadSettings();
            initialize();
            performModLoaderVersionCheck();
        }
        public static int determineIsVersionOldCurrentOrNew(string inVersion)
        {
            string[] versionNumbers = inVersion.Split('.');
            string[] currentVersionNumbers = VERSION.Split('.');
            int result;
            int value;
            if (versionNumbers.Length == currentVersionNumbers.Length)
            {
                for (int i = 0; i < versionNumbers.Length; i++)
                {
                    if (int.TryParse(versionNumbers[i], out result))
                        if (int.TryParse(currentVersionNumbers[i], out value))
                            if (result > value)
                                return 1;
                            else if (result < value)
                                return -1;
                }
            }
            return 0;
        }
        public override void SecondPassOnLoad()
        {
            // Written, 18.10.2020

            if (!UnityEngine.Input.GetKey(KeyCode.LeftControl))
                loadCInputAxisSettings();
            else
                print("skipped loading cinput axis settings (grav, dead, sens).. Cause: <i>Left-Ctrl held down through Mo'Controls' second pass loading sequ</i>", DebugTypeEnum.none);
            moControlsGameObject = new GameObject(gameObjectName);
            moControlsGO = moControlsGameObject.AddComponent<MoControlsGO>();
            print(Name + " v" + Version + ": Loaded.", DebugTypeEnum.none);
        }
        #endregion

    }
}
