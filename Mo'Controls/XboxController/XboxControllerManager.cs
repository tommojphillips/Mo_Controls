using System;
using TommoJProdutions.MoControls.XInputInterpreter.Monitoring;
using UnityEngine;

namespace TommoJProdutions.MoControls.XInputInterpreter
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
        public XboxController[] controllers
        {
            get;
            private set;
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
                this.controllers = new XboxController[numOfControllersSupported];
                for (int i = 1; i <= numOfControllersSupported; i++)
                {
                    this.addController(new XboxController(i));
                }
                this.monitorControllerConnections = this.gameObject.AddComponent<MonitorControllerConnections>();
                MSCLoader.ModConsole.Print(nameof(XboxControllerManager) + ": Loaded");
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.Error(ex.StackTrace);
            }
        }
        /// <summary>
        /// Performs updates for every controller within the manager. Updates <see cref="monitorControllerConnections"/>.
        /// </summary>
        private void Update()
        {
            // Written, 16.07.2018

            foreach (XboxController controller in this.controllers)
            {
                controller.update();
            }
        }
        /// <summary>
        /// Performs refreshes for every controller within the manager.
        /// </summary>
        private void LateUpdate()
        {
            // Written, 16.07.2018

            foreach (XboxController controller in this.controllers)
            {
                controller.refresh();
            }
        }
        /// <summary>
        /// Adds the specified xbox controller to the manager.
        /// </summary>
        /// <param name="xboxController">The controller to add.</param>
        public void addController(XboxController xboxController)
        {
            // Written, 16.07.2018

            bool isControllerAssigned = false;
            for (int i = 0; i < numOfControllersSupported; i++)
            {
                if (this.controllers[i] == null)
                {
                    this.controllers[i] = xboxController;
                    isControllerAssigned = true;
                    break;
                }
            }
            if (!isControllerAssigned)
            {
                string errorMessage = String.Format("This instance of {2} only supports {0} controller{1}. You have attempted to add another controller which cannot be executed.", numOfControllersSupported, (numOfControllersSupported > 1 ? "s" : ""), nameof(XboxControllerManager));
                MSCLoader.ModConsole.Error(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the <see cref="ControllerConnected"/> event.
        /// </summary>
        /// <param name="eventArgs">The event data.</param>
        internal static void onControllerConnected(ControllerConnectionEventArgs eventArgs)
        {
            // Written, 16.07.2018

            if (ControllerConnected != null)
                ControllerConnected.Invoke(null, eventArgs);
        }
        /// <summary>
        /// Raises the <see cref="ControllerDisconnected"/> event.
        /// </summary>
        /// <param name="eventArgs">The event data.</param>
        internal static void onControllerDisconnected(ControllerConnectionEventArgs eventArgs)
        {
            // Written, 16.07.2018

            if (ControllerDisconnected != null)
                ControllerDisconnected.Invoke(null, eventArgs);
        }

        #endregion
    }
}
