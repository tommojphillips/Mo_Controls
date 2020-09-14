﻿using System;
using TommoJProductions.MoControls.Debugging;
using MSCLoader;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    // XInputDotNetPure.dll - in folder: "My Summer Car/mysummercar_data/Managed"
    // XInputInterface.dll - in folder: "My Summer Car"
    /// <summary>
    /// Represents the mo'controls mod.
    /// </summary>
    public class MoControlsMod : Mod
    {
        // Written, 06.07.2018        

        #region Mod Fields

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Author => "tommojphillips";
        public override string Version => "1.1";
        public override bool UseAssetsFolder => true;

        #endregion

        #region Fields

        /// <summary>
        /// Represents whether or not to display debug data.
        /// </summary>
        public static DebugTypeEnum debug;
        /// <summary>
        /// Represents the supported/compatible version of mod loader.
        /// </summary>
        public const string SUPPORTED_MODLOADER_VERSION = "1.1.7";
        /// <summary>
        /// Represents the moControls gameobject.
        /// </summary>
        private static GameObject moControlsGameObject;
        /// <summary>
        /// Player seen error message?
        /// </summary>
        public bool playerSeenMscLoaderVersionError
        {
            get;
            set;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Represents whether or not the assets are loaded.
        /// </summary>
        public static bool assetsLoaded
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents all assets for the mod.
        /// </summary>
        public static AssetHolder assets
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the <see cref="moControlsGO"/> name.
        /// </summary>
        public string gameObjectName => String.Format("{0} v{1}", this.ID, this.Version);
        /// <summary>
        /// Represents the current instance of <see cref="MoControlsMod"/>.
        /// </summary>
        public static MoControlsMod instance
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the mo_controls game object instance.
        /// </summary>
        public static MoControlsGO moControlsGO
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
            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> ready</color>", this.Name, this.Version));
        }

        #endregion

        #region Methods

        /// <summary>
        /// checks if type equals debug.
        /// </summary>
        /// <param name="inDebugType">the debug type to compare.</param>
        public static bool debugTypeEquals(DebugTypeEnum inDebugType)
        {
            // Written, 15.10.2018
            
            if (debug == DebugTypeEnum.full)
                return true;
            if (debug == DebugTypeEnum.partial)
                if (inDebugType == DebugTypeEnum.none)
                    return true;
            if (debug == inDebugType)
                return true;
            return false;
        }
        /// <summary>
        /// Prints a message to the console with the prefix of the mod name.
        /// </summary>
        /// <param name="inMessage"></param>
        public static void print(string inMessage)
        {
            // Written, 08.10.2018

            string _msg = String.Format("<i><color=grey>[{0}]</color> <b><color=green>>></color></b></i> {1}", instance.Name, inMessage);
            ModConsole.Print(_msg);
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
                if (debugTypeEquals(DebugTypeEnum.partial))
                    print("Asset bundle loaded successfully.");
                assets = new AssetHolder(ab.LoadAllAssets<Texture2D>()) ?? throw new Exception("asset holder return an error");
                ab.Unload(false);
                assetsLoaded = true;

                if (debugTypeEquals(DebugTypeEnum.full))
                    print("Asset bundle unloaded successfully.");
            }
            catch (Exception ex)
            {
                if (debugTypeEquals(DebugTypeEnum.partial))
                    print("<color=red>asset bundle threw error: " + ex.ToString() + ".</color>");
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
                if (!this.playerSeenMscLoaderVersionError)
                {
                    ModUI.ShowMessage(
                        String.Format("<b>[{0} <color=orange>v{1}]</color></b> - NOTE: modloader v{2} may not be <color=orange>compatible</color>.\r\nSupported " +
                        "modloader version is <color=orange>v{3}</color>.",
                        this.Name, this.Version, ModLoader.MSCLoader_Ver, SUPPORTED_MODLOADER_VERSION), "ModLoader Version not supported.");
                    this.playerSeenMscLoaderVersionError = true;
                    MoControlsSaveData.saveSettings();
                }
                if (debugTypeEquals(DebugTypeEnum.partial))
                    print("<color=orange>Warning</color> <color=grey>Supported modloader version is <b>v" + SUPPORTED_MODLOADER_VERSION + "</b>; you're running version <b>" + ModLoader.MSCLoader_Ver + "</b>. May not be compatible with current version.</color>.");
            }
            else
            {
                if (debugTypeEquals(DebugTypeEnum.full))
                    print("<color=grey>Running supported modloader version, <color=green>" + SUPPORTED_MODLOADER_VERSION + "</color></color>");
                this.playerSeenMscLoaderVersionError = false;
                MoControlsSaveData.saveSettings();
            }
        }
        /// <summary>
        /// Initializes.
        /// </summary>
        private void initialize()
        {
            // Written, 17.10.2018

            ConsoleCommand.Add(new DebugConsoleCommand());
            this.loadControllerAssets();
        }

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Written, 06.07.2018    
                        
            moControlsGameObject = new GameObject(gameObjectName);
            moControlsGO = moControlsGameObject.AddComponent<MoControlsGO>();
            moControlsGO.setLoadedSettings(MoControlsSaveData.loadSettings(), preload: true);
            this.initialize();
            this.performModLoaderVersionCheck();
            if (debugTypeEquals(DebugTypeEnum.full))
                print("Assets returned: " + assets.result);
            if (debugTypeEquals(DebugTypeEnum.none))
                print(this.Name + " v" + this.Version + ": Loaded.");
        }

        #endregion        
    }
}   
