using System;
using System.Linq;
using MSCLoader;
using UnityEngine;
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
        public override string Version => "1.0.5";
        public override bool UseAssetsFolder => true;

        #endregion

        #region Fields

        /// <summary>
        /// Represents the supported/compatible version of mod loader.
        /// </summary>
        public const string SUPPORTED_MODLOADER_VERSION = "0.4.4";

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
        /// Gets infomation about a cInput input based on the input namem, <paramref name="inputName"/>. Returns an array with 3 items within it. In this order:
        /// 1. The name of the control, 2. The Primary input for the control, 3. The Secondary input for the control.
        /// </summary>
        /// <param name="inputName">The name of the input.</param>
        private string[] getInputNameControl(string inputName)
        {
            // Written, 06.07.2018

            string[] returnValues = new string[3];
            for (int i = 0; i < 3; i++)
                returnValues[i] = cInput.GetText(inputName, i);
            return returnValues;
        }
        /// <summary>
        /// Loads control inputs (defined in <see cref="inputNames"/>) from the class, <see cref="cInput"/> and adds each one to <see cref="controlInputs"/> with it's primary
        /// and secondary input.
        /// </summary>
        public void loadControlInputsFromCInput()
        {
            // Written, 06.07.2018

            for (int i = 0; i < ControlManager.inputNames.Length; i++)
            {
                string[] inputName = this.getInputNameControl(ControlManager.inputNames[i]);
                for (int j = 0; j < inputName.Length; j++)
                {
                    //ModConsole.Print("[loadControlInputsFromCInput] - Not Working, please fix me.");
                }
            }
            ModConsole.Print("[loadControlInputsFromCInput] - Not Working, please fix me.");
        }
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
                    "modloader version is <color=orange>v{2}</color>\r\nFeel free to use the mod but keep in mind that it might not work as expected.",
                    this.Name, this.Version, ModLoader.Version), "ModLoader Version not supported.");
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
            if (startUp)
                ModConsole.Print("<color=green>>></color> Settings Loaded");
        }

        #endregion

        #region Override Methods

        public override void OnLoad()
        {
            // Written, 06.07.2018    

            this.controlManager = new ControlManager();
            this.moControlsGui = new MoControlsGUI(this);
            ModConsole.Print(String.Format("\r\n<color=green>{0} <b>v{1}</b> Loading</color>", this.Name, this.Version));
            this.loadControlInputsFromCInput();
            cInput.OnKeyChanged += null;
            cInput.OnKeyChanged += this.CInput_OnKeyChanged;
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

            this.moControlsGui.onUpdate();
            this.xboxControllerManager.onUpdate();
            this.mouseEmulator.onUpdate();       
            this.xboxControllerManager.onRefresh();
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when cinput keys are changed externally, (the game gui controls).
        /// </summary>
        private void CInput_OnKeyChanged()
        {
            // Written, 09.07.2018

            ModConsole.Print("[CInput.dll] - 'OnKeyChanged' occured.");
            this.loadControlInputsFromCInput();
        }
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
