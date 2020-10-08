using System;
using UnityEngine;
using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;

namespace TommoJProductions.MoControls
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
        internal static MoControlsGUI moControlsGui
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the control manager.
        /// </summary>
        internal static ControlManager controlManager
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents an emulator for the mouse.
        /// </summary>
        internal static MouseEmulator mouseEmulator
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the xbox controller manager.
        /// </summary>
        internal static XboxControllerManager xboxControllerManager
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents an xbox controller.
        /// </summary>
        internal static XboxController xboxController
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
            MoControlsMod.print(nameof(MoControlsGO) + ": Started", Debugging.DebugTypeEnum.full);
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
                    this.setLoadedSettings(MoControlsSaveData.loadSettings(), inStartUp: true);
                    xboxController.loadControllerAssets();
                    this.settingsLoaded = true;
                    MoControlsMod.print("All Modules have <b><color=green>successfully</color></b> been initilized.", Debugging.DebugTypeEnum.partial);
                }
            }
            else
            {
                if (!this.moduleError)
                {
                    this.moduleError = true;
                    MoControlsMod.print("Some modules have <b>not</b> been initilized.", Debugging.DebugTypeEnum.partial);
                }
            }
        }
        /// <summary>
        /// sets the settings.
        /// </summary>
        /// <param name="inSaveData">The save data to set as loaded.</param>
        /// <param name="inStartUp">the</param>
        internal void setLoadedSettings(MoControlsSaveData inSaveData, bool inStartUp = false, bool inPreload = false)
        {
            // Written, 22.08.2018

            if (inPreload)
            {
                MoControlsMod.debug = inSaveData.debugMode;
                MoControlsMod.instance.playerSeenMscLoaderVersionError = inSaveData.playerSeenMscLoaderVersionError;
                MoControlsMod.print("Pre-Settings loaded", Debugging.DebugTypeEnum.full);
            }
            else
            {
                try
                {
                    xboxControllerManager.monitorControllerConnections.Monitor = inSaveData.monitiorXboxControllerConnectionStatus;
                    mouseEmulator.deadzone = inSaveData.mouseDeadzone;
                    mouseEmulator.sensitivity = inSaveData.mouseSensitivity;
                    mouseEmulator.inputType = inSaveData.mouseInputType;
                    mouseEmulator.Emulating = inSaveData.emulateMouse;
                    MoControlsGUI.displayCurrentPlayerModeOverlay = inSaveData.displayCurrentPlayerModeOverlay;
                    controlManager.setControls(inSaveData.footControls, inSaveData.drivingControls);

                    if (inStartUp)
                    {
                        MoControlsMod.print("Start-Up settings loaded", Debugging.DebugTypeEnum.partial);
                    }

                }
                catch (NullReferenceException ex)
                {
                    MoControlsMod.print("[MoControlsGO.setLoadedSettings] - " + ex.StackTrace, Debugging.DebugTypeEnum.full);
                }
            }
        }

        #endregion

        #region Event handlers

        private void XboxControllerManager_ControllerDisconnected(object sender, ControllerConnectionEventArgs e)
        {
            // Written, 08.10.2018

            MoControlsMod.print("<color=grey><i>Controller: " + e.xboxController.index + "</i></color> <color=red>Disconnected</color>", Debugging.DebugTypeEnum.none);
        }

        private void XboxControllerManager_ControllerConnected(object inSender, ControllerConnectionEventArgs inE)
        {
            // Written, 08.10.2018

            MoControlsMod.print("<color=grey><i>Controller " + inE.xboxController.index + "</i></color> <color=green>Connected</color>", Debugging.DebugTypeEnum.none);
        }

        #endregion
    }
}
