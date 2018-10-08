using System;
using System.Threading;
using MSCLoader;
using Mo_Controls.McGUI;
using Mo_Controls.MouseEmulation;
using Mo_Controls.XboxController;
using xController = Mo_Controls.XboxController.XboxController;

namespace Mo_Controls
{
    // XInputDotNetPure.dll - in folder: "My Summer Car/mysummercar_data/Managed"
    // XInputInterface.dll - in folder: "My Summer Car"
    public class Mo_Controls : Mod
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
        /// Represents the supported/compatible version of mod loader.
        /// </summary>
        public const string SUPPORTED_MODLOADER_VERSION = "0.4.6";
        /// <summary>
        /// Represents the update thread.
        /// </summary>
        private Thread updateThread;
        #endregion

        #region Properties

        /// <summary>
        /// Represents the GUI for the mod.
        /// </summary>
        public MoControlsGUI moControlsGui
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the control manager.
        /// </summary>
        public ControlManager controlManager
        {
            get;
            set;
        }
        /// <summary>
        /// Represents an emulator for the mouse.
        /// </summary>
        public MouseEmulator mouseEmulator
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the xbox controller manager.
        /// </summary>
        public XboxControllerManager xboxControllerManager
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents an xbox controller.
        /// </summary>
        public xController xboxController
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the current instance of <see cref="Mo_Controls"/>.
        /// </summary>
        public static Mo_Controls instance
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public Mo_Controls()
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
        /// 
        /// </summary>
        /// <param name="inSaveData"></param>
        /// <param name="startUp"></param>
        private void setLoadedSettings(MoControlsSaveData inSaveData, bool startUp = false)
        {
            // Written, 22.08.2018

            this.moControlsGui.showVirtualGui = inSaveData.showXboxVirtualAxesGui;
            this.xboxControllerManager.monitorControllerConnections.Monitor = inSaveData.monitiorXboxControllerConnectionStatus;
            this.mouseEmulator.deadzone = inSaveData.mouseDeadzone;
            this.mouseEmulator.sensitivity = inSaveData.mouseSensitivity;
            this.mouseEmulator.inputType = inSaveData.mouseInputType;
            this.mouseEmulator.Emulating = inSaveData.emulateMouse;
            this.controlManager.displayCurrentPlayerModeOverlay = inSaveData.displayCurrentPlayerModeOverlay;
            this.controlManager.setControls(inSaveData.footControls, inSaveData.drivingControls);
            if (startUp)
            {
                ModConsole.Print("<color=green>>></color> Settings Loaded");
            }
        }

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Written, 06.07.2018    

            ModConsole.Print(String.Format("\r\n<color=green>{0} <b>v{1}</b> Loading</color>", this.Name, this.Version));
            this.controlManager = new ControlManager();
            this.moControlsGui = new MoControlsGUI();
            this.xboxControllerManager = new XboxControllerManager(1);
            this.xboxController = this.xboxControllerManager.controllers[0];
            if (!this.xboxControllerManager.controllerConnectedEvent)
                XboxControllerManager.ControllerConnected += this.XboxControllerManager_ControllerConnected;
            if (!this.xboxControllerManager.controllerDisconnectedEvent)
                XboxControllerManager.ControllerDisconnected += this.XboxControllerManager_ControllerDisconnected;
            this.mouseEmulator = new MouseEmulator(DeadzoneTypeEnum.ScaledRadial);
            this.setLoadedSettings(MoControlsSaveData.loadSettings(this), true);            
            this.performModLoaderVersionCheck();
            ModConsole.Print(String.Format("<color=green>{0} <b>v{1}</b> Loaded</color>", this.Name, this.Version));
        }
        public override void OnGUI()
        {
            // Written, 10.07.2018

            this.moControlsGui.onGUI();            
        }
        public override void Update()
        {
            // Written, 22.08.2018
            // Update is called once per frame

            this.updateThread = new Thread(delegate ()
            {
                this.controlManager.onUpdate();
                this.moControlsGui.onUpdate();
                this.xboxControllerManager.onUpdate();
                this.mouseEmulator.onUpdate();
                this.xboxControllerManager.onRefresh();
            });
            this.updateThread.Start();
        }

        #endregion
        
        #region Events
        
        /// <summary>
        /// Occurs when a controller is disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XboxControllerManager_ControllerDisconnected(object sender, EventArgs e)
        {
            ModConsole.Print("Controller Disconnected");
        }
        /// <summary>
        /// Occurs when a controller is connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XboxControllerManager_ControllerConnected(object sender, EventArgs e)
        {
            ModConsole.Print("Controller Connected");
        }

        #endregion
    }
}   
