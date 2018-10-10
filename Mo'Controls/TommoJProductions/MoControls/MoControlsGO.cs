using System;
using UnityEngine;
using TommoJProdutions.MoControls.GUI;
using TommoJProdutions.MoControls.MouseEmulation;
using TommoJProdutions.MoControls.XInputInterpreter;

namespace TommoJProdutions.MoControls
{
    /// <summary>
    /// Represents the Mo'Controls gameobject that holds references to each of the other <see cref="Component"/>s.
    /// </summary>
    public class MoControlsGO : MonoBehaviour
    {
        // Written, 08.10.2018

        #region Fields

        /// <summary>
        /// Represents whether or not the modules had an error.
        /// </summary>
        private bool moduleError;
        /// <summary>
        /// Represents whether or not the settings are loaded.
        /// </summary>
        private bool settingsLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// Represents whether or not all the modules have started.
        /// </summary>
        private bool modulesStarted
        {
            get
            {
                if (moControlsGui != null
                    && controlManager != null
                    && mouseEmulator != null
                    && xboxControllerManager != null
                    && xboxController != null)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Represents the GUI for the mod.
        /// </summary>
        public static MoControlsGUI moControlsGui
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the control manager.
        /// </summary>
        public static ControlManager controlManager
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents an emulator for the mouse.
        /// </summary>
        public static MouseEmulator mouseEmulator
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the xbox controller manager.
        /// </summary>
        public static XboxControllerManager xboxControllerManager
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents an xbox controller.
        /// </summary>
        public static XboxController xboxController
        {
            get
            {
                return xboxControllerManager.controllers[0];
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            controlManager = this.gameObject.AddComponent<ControlManager>();
            moControlsGui = this.gameObject.AddComponent<MoControlsGUI>();
            xboxControllerManager = this.gameObject.AddComponent<XboxControllerManager>();
            mouseEmulator = this.gameObject.AddComponent<MouseEmulator>();
            XboxControllerManager.ControllerConnected += this.XboxControllerManager_ControllerConnected;
            XboxControllerManager.ControllerDisconnected += this.XboxControllerManager_ControllerDisconnected;
            if (MoControlsMod.debug)
                MoControlsMod.print(nameof(MoControlsGO) + ": Started");
        }
        /// <summary>
        /// Occurs every frame.
        /// </summary>
        private void Update()
        {
            // Written, 08.10.2018

            if (this.modulesStarted)
            {
                if (!this.settingsLoaded && GameObject.Find(MoControlsMod.instance.gameObjectName) != null)
                {
                    this.setLoadedSettings(MoControlsSaveData.loadSettings(), true);
                    this.settingsLoaded = true;
                    if (MoControlsMod.debug)
                        MoControlsMod.print("All Modules have <b><color=green>successfully</color></b> been initilized.");
                }
            }
            else
            {
                if (!this.moduleError)
                {
                    this.moduleError = true;
                    MSCLoader.ModConsole.Error("Some modules have <b>not</b> been initilized.");
                }
            }
        }
        /// <summary>
        /// sets the settings.
        /// </summary>
        /// <param name="inSaveData"></param>
        /// <param name="startUp"></param>
        private void setLoadedSettings(MoControlsSaveData inSaveData, bool startUp = false)
        {
            // Written, 22.08.2018

            try
            {
                moControlsGui.showVirtualGui = inSaveData.showXboxVirtualAxesGui;
                xboxControllerManager.monitorControllerConnections.Monitor = inSaveData.monitiorXboxControllerConnectionStatus;
                mouseEmulator.deadzone = inSaveData.mouseDeadzone;
                mouseEmulator.sensitivity = inSaveData.mouseSensitivity;
                mouseEmulator.inputType = inSaveData.mouseInputType;
                mouseEmulator.Emulating = inSaveData.emulateMouse;
                controlManager.displayCurrentPlayerModeOverlay = inSaveData.displayCurrentPlayerModeOverlay;
                controlManager.setControls(inSaveData.footControls, inSaveData.drivingControls);
                MoControlsMod.debug = inSaveData.debugMode;
                xboxController.customXboxControls = inSaveData.customControllerInputControls;
                xboxController.setControllerInputType(inSaveData.xboxControllerInput);

                if (startUp)
                {
                    if (MoControlsMod.debug)
                        MoControlsMod.print("Settings Loaded");
                }
            }
            catch (NullReferenceException ex)
            {
                MSCLoader.ModConsole.Error("[MoControlsMod.setLoadedSettings] - " + ex.StackTrace);
            }
        }

        #endregion

        #region Event handlers

        private void XboxControllerManager_ControllerDisconnected(object sender, ControllerConnectionEventArgs e)
        {
            // Written, 08.10.2018

            MoControlsMod.print("Controller: " + e.xboxController.index + " <color=red>Disconnected</color>");
        }

        private void XboxControllerManager_ControllerConnected(object sender, ControllerConnectionEventArgs e)
        {
            // Written, 08.10.2018

            MoControlsMod.print("Controller " + e.xboxController.index + " <color=green>Connected</color>");
        }

        #endregion
    }
}
