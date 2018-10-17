using System;
using UnityEngine;
using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.MouseEmulation;
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
                if (!MoControlsMod.menuLoad)
                {

                    if (moControlsGui != null
                        && controlManager != null
                        && mouseEmulator != null
                        && xboxControllerManager != null
                        && xboxController != null)
                        return true;
                }
                else
                {
                    if (moControlsGui != null
                       && mouseEmulator != null
                       && xboxControllerManager != null
                       && xboxController != null)
                        return true;
                }
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

            if (!MoControlsMod.menuLoad)
                controlManager = this.gameObject.AddComponent<ControlManager>();
            moControlsGui = this.gameObject.AddComponent<MoControlsGUI>();
            xboxControllerManager = this.gameObject.AddComponent<XboxControllerManager>();
            mouseEmulator = this.gameObject.AddComponent<MouseEmulator>();
            XboxControllerManager.ControllerConnected += this.XboxControllerManager_ControllerConnected;
            XboxControllerManager.ControllerDisconnected += this.XboxControllerManager_ControllerDisconnected;
            if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.full))
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
                    this.setLoadedSettings(MoControlsSaveData.loadSettings(), startUp: true);
                    xboxController.loadControllerAssets();
                    this.settingsLoaded = true;
                    if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.partial))
                        MoControlsMod.print("All Modules have <b><color=green>successfully</color></b> been initilized.");
                }
            }
            else
            {
                if (!this.moduleError)
                {
                    this.moduleError = true;
                    if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.partial))
                        MoControlsMod.print("Some modules have <b>not</b> been initilized.");
                }
            }
        }
        /// <summary>
        /// sets the settings.
        /// </summary>
        /// <param name="inSaveData">The save data to set as loaded.</param>
        /// <param name="startUp">the</param>
        public void setLoadedSettings(MoControlsSaveData inSaveData, bool startUp = false, bool preload = false)
        {
            // Written, 22.08.2018

            if (preload)
            {
                MoControlsMod.debug = inSaveData.debugMode;
                MoControlsMod.instance.playerSeenMscLoaderVersionError = inSaveData.playerSeenMscLoaderVersionError;
                if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.full))
                    MoControlsMod.print("Pre-Settings Loaded");
            }
            else
            {
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
                    xboxController.customXboxControls = inSaveData.customControllerInputControls;
                    xboxController.setControllerInputType(inSaveData.xboxControllerInput);

                    if (startUp)
                    {
                        if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.partial))
                            MoControlsMod.print("Settings Loaded");
                    }

                }
                catch (NullReferenceException ex)
                {
                    if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.full))
                        MSCLoader.ModConsole.Error("[MoControlsMod.setLoadedSettings] - " + ex.StackTrace);
                }
            }
        }

        #endregion

        #region Event handlers

        private void XboxControllerManager_ControllerDisconnected(object sender, ControllerConnectionEventArgs e)
        {
            // Written, 08.10.2018

            MoControlsMod.print("<color=grey><i>Controller: " + e.xboxController.index + "</i></color> <color=red>Disconnected</color>");
        }

        private void XboxControllerManager_ControllerConnected(object sender, ControllerConnectionEventArgs e)
        {
            // Written, 08.10.2018

            MoControlsMod.print("<color=grey><i>Controller " + e.xboxController.index + "</i></color> <color=green>Connected</color>");
        }

        #endregion
    }
}
