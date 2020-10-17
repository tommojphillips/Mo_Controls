﻿using MSCLoader;
using System;
using TommoJProductions.Debugging;
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
        // Written, 06.07.2018        

        /// <summary>
        /// Represents whether this is a release version
        /// </summary>
        internal static bool isReleaseVersion => false;

        #region Mod Fields

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Author => "tommojphillips";
        public override string Version => "1.1.2";
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
        public const string SUPPORTED_MODLOADER_VERSION = "1.1.7";

        #endregion

        #region Properties

        internal bool loaded { get; private set; } = false;
        /// <summary>
        /// Represents the release version name.
        /// </summary>
        internal string releaseVersionName
        {
            get
            {
                return "<color=" + (isReleaseVersion ? "blue>Release" : "red>Pre-Release") + "</color>";
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
        internal string gameObjectName => String.Format("{0} v{1}", this.ID, this.Version);
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
            this.performDebugCheck();
            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> ({2}) ready</color>", this.Name, this.Version, this.releaseVersionName));
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

            if (MoControlsSaveData.loadedSaveData.debugMode == DebugTypeEnum.full)
                return true;
            if (MoControlsSaveData.loadedSaveData.debugMode == DebugTypeEnum.partial)
                if (inDebugType == DebugTypeEnum.none)
                    return true;
            if (MoControlsSaveData.loadedSaveData.debugMode == inDebugType)
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
                        this.Name, this.Version, ModLoader.MSCLoader_Ver, SUPPORTED_MODLOADER_VERSION), "ModLoader Version not supported.");
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

            this.loadControllerAssets();
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

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Written, 06.07.2018    

            MoControlsSaveData.loadSettings();
            moControlsGameObject = new GameObject(gameObjectName);
            moControlsGO = moControlsGameObject.AddComponent<MoControlsGO>();
            this.initialize();
            this.performModLoaderVersionCheck();
            print(this.Name + " v" + this.Version + ": Loaded.", DebugTypeEnum.none);
        }
        public override void SecondPassOnLoad()
        {
            // Written, 17.10.2020

            MoControlsGO.controlManager.setControls(MoControlsSaveData.loadedSaveData.footControls, MoControlsSaveData.loadedSaveData.drivingControls);
            MoControlsGO.xboxController.loadControllerAssets();
            print("Assets returned: " + assets.result, DebugTypeEnum.full);
            this.loaded = true;
        }

        #endregion

    }
}
