using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents the Mo'Controls gameobject that holds references to each of the other <see cref="Component"/>s.
    /// </summary>
    public class MoControlsGO : MonoBehaviour
    {
        // Written, 08.10.2018

        #region Properties

        /// <summary>
        /// Represents whether or not the modules had an error.
        /// </summary>
        internal bool moduleError { private get; set; }
        /// <summary>
        /// Represents whether or not the settings are loaded.
        /// </summary>
        internal bool settingsLoaded { get; private set; }
        /// <summary>
        /// Represents whether or not all the modules have started.
        /// </summary>
        internal bool modulesStarted
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
                return xboxControllerManager.controller;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when game starts.
        /// </summary>
        private void Awake()
        {
            // Written, 18.10.2020

            xboxControllerManager = this.gameObject.AddComponent<XboxControllerManager>();
            controlManager = this.gameObject.AddComponent<ControlManager>();
            mouseEmulator = this.gameObject.AddComponent<MouseEmulator>();
            moControlsGui = this.gameObject.AddComponent<MoControlsGUI>();
            XboxControllerManager.ControllerConnected += this.xboxControllerManager_ControllerConnected;
            XboxControllerManager.ControllerDisconnected += this.xboxControllerManager_ControllerDisconnected;
            MoControlsMod.print(nameof(MoControlsGO) + ": Started", Debugging.DebugTypeEnum.full);
        }

        #endregion

        #region Event handlers

        private void xboxControllerManager_ControllerDisconnected(object sender, ControllerConnectionEventArgs e)
        {
            // Written, 08.10.2018

            MoControlsMod.print("<color=grey><i>Controller: " + e.xboxController.index + "</i></color> <color=red>Disconnected</color>", Debugging.DebugTypeEnum.none);
        }

        private void xboxControllerManager_ControllerConnected(object inSender, ControllerConnectionEventArgs inE)
        {
            // Written, 08.10.2018

            MoControlsMod.print("<color=grey><i>Controller " + inE.xboxController.index + "</i></color> <color=green>Connected</color>", Debugging.DebugTypeEnum.none);
        }

        #endregion
    }
}
