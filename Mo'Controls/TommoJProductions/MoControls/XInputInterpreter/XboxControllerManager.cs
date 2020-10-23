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
        /// Returns a bool depending if the event, <see cref="ControllerConnected"/> to equal to null.
        /// </summary>
        public bool controllerConnectedEvent
        {
            get
            {
                if (ControllerConnected == null)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// Returns a bool depending if the event, <see cref="ControllerDisconnected"/> to equal to null.
        /// </summary>
        public bool controllerDisconnectedEvent
        {
            get
            {
                if (ControllerDisconnected == null)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// Represents the controllers.
        /// </summary>
        internal XboxController controller
        {
            get { return this.gameObject.GetComponent<XboxController>(); }
        }
        /// <summary>
        /// Represents the monitoring controller connections system.
        /// </summary>
        public MonitorControllerConnections monitorControllerConnections
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="XboxControllerManager"/> and sets the provided integer to <see cref="numOfControllersSupported"/> and adds a controller to the manager for every <see cref="numOfControllersSupported"/>.
        /// </summary>
        /// <param name="numOfControllersToSupport">The amount of controllers to support.</param>
        public XboxControllerManager()
        {
            // Written, 16.07.2018

            ControllerConnected = null;
            ControllerDisconnected = null;
        }

        #endregion

        #region Methods

        private void Start()
        {
            try
            {
                this.gameObject.AddComponent<XboxController>();
                this.monitorControllerConnections = this.gameObject.AddComponent<MonitorControllerConnections>();
                MoControlsMod.print(nameof(XboxControllerManager) + ": Started", Debugging.DebugTypeEnum.full);
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.Error(ex.StackTrace);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the <see cref="ControllerConnected"/> event.
        /// </summary>
        /// <param name="inEventArgs">The event data.</param>
        internal static void onControllerConnected(ControllerConnectionEventArgs inEventArgs)
        {
            // Written, 16.07.2018

            if (ControllerConnected != null)
                ControllerConnected.Invoke(null, inEventArgs);
        }
        /// <summary>
        /// Raises the <see cref="ControllerDisconnected"/> event.
        /// </summary>
        /// <param name="inEventArgs">The event data.</param>
        internal static void onControllerDisconnected(ControllerConnectionEventArgs inEventArgs)
        {
            // Written, 16.07.2018

            if (ControllerDisconnected != null)
                ControllerDisconnected.Invoke(null, inEventArgs);
        }

        #endregion
    }
}
