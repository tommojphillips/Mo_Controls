using System;
using TommoJProductions.MoControls.XInputInterpreter.Monitoring;
using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents an xbox controller manager, that manages xbox controllers that are added to it.
    /// </summary>
    public class XboxControllerManager : MonoBehaviour
    {
        // Written, 16.07.2018

        #region Fields

        /// <summary>
        /// Represents the amount of supported controllers for the application.
        /// </summary>
        public const int numOfControllersSupported = 1;
        /// <summary>
        /// Represents an event that occurs when an xbox controller is connected.
        /// </summary>
        public static event EventHandler<ControllerConnectionEventArgs> ControllerConnected;
        /// <summary>
        /// Represents an event that occurs when an xbox controller is disconnected.
        /// </summary>
        public static event EventHandler<ControllerConnectionEventArgs> ControllerDisconnected;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the controllers.
        /// </summary>
        internal XboxController controller
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the monitoring controller connections system.
        /// </summary>
        internal MonitorControllerConnections monitorControllerConnections
        {
            get;
            set;
        }

        #endregion

        #region Methods

        private void Awake()
        {
            MoControlsMod.print(nameof(XboxControllerManager) + ": Initialized", Debugging.DebugTypeEnum.full);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the <see cref="ControllerConnected"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        internal void onControllerConnected(ControllerConnectionEventArgs e)
        {
            // Written, 16.07.2018

            if (ControllerConnected != null)
                ControllerConnected.Invoke(null, e);
            if (MoControlsGO.controlManager.scrollCoroutine == null)
                MoControlsGO.controlManager.scrollCoroutine = StartCoroutine(MoControlsGO.controlManager.scrollFunction());
            MoControlsMod.print("<color=grey><i>Controller " + e.xboxController.index + "</i></color> <color=green>Connected</color>", Debugging.DebugTypeEnum.none);
        }
        /// <summary>
        /// Raises the <see cref="ControllerDisconnected"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        internal void onControllerDisconnected(ControllerConnectionEventArgs e)
        {
            // Written, 16.07.2018

            if (ControllerDisconnected != null)
                ControllerDisconnected.Invoke(null, e);
            if (MoControlsGO.controlManager.scrollCoroutine != null)
                StopCoroutine(MoControlsGO.controlManager.scrollFunction());
            MoControlsMod.print("<color=grey><i>Controller: " + e.xboxController.index + "</i></color> <color=red>Disconnected</color>", Debugging.DebugTypeEnum.none);
        }

        #endregion
    }
}
