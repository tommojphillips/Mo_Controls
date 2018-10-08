using System;
using MSCLoader;
using UnityEngine;

namespace TommoJProdutions.MoControls
{
    // XInputDotNetPure.dll - in folder: "My Summer Car/mysummercar_data/Managed"
    // XInputInterface.dll - in folder: "My Summer Car"
    public class MoControlsMod : Mod
    {
        // Written, 06.07.2018        

        #region Mod Fields

        public override string ID => "Mo_Controls";
        public override string Name => "Mo'Controls";
        public override string Author => "tommojphillips";
        public override string Version => "1.0.7";
        public override bool UseAssetsFolder => true;

        #endregion

        #region Fields

        /// <summary>
        /// Represents whether or not to display debug data.
        /// </summary>
        public static bool debug;
        /// <summary>
        /// Represents the supported/compatible version of mod loader.
        /// </summary>
        public const string SUPPORTED_MODLOADER_VERSION = "0.4.6";
        /// <summary>
        /// Represents the moControls gameobject.
        /// </summary>
        private static GameObject moControlsGameObject;

        #endregion

        #region Properties

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
            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> Initialized</color>", this.Name, this.Version));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs a check to see if the mod loader version is the same as the currently supported version.
        /// </summary>
        private void performModLoaderVersionCheck()
        {
            // Written, 03.08.2018

            // Modloader supported version check.
            if (SUPPORTED_MODLOADER_VERSION != ModLoader.Version)
            {
                ModUI.ShowMessage(
                    String.Format("<b>[{0} <color=orange>v{1}]</color></b> - NOTE: modloader v{2} is <color=orange>not</color> currently <color=orange>supported</color> and/or may not be <color=orange>compatible</color>.\r\nSupported " +
                    "modloader version is <color=orange>v{3}</color>\r\nFeel free to use the mod but keep in mind that it might not work as expected.",
                    this.Name, this.Version, ModLoader.Version, SUPPORTED_MODLOADER_VERSION), "ModLoader Version not supported.");
            }
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

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Written, 06.07.2018    

            ConsoleCommand.Add(new DebugConsoleCommand());
            moControlsGameObject = new GameObject(gameObjectName);
            moControlsGO = moControlsGameObject.AddComponent<MoControlsGO>();
            this.performModLoaderVersionCheck();
        }

        #endregion        
    }
}   
