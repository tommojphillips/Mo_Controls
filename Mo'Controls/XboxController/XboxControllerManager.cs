using System;
using Mo_Controls.XboxController.Monitoring;

namespace Mo_Controls.XboxController
{
    /// <summary>
    /// Represents an xbox controller manager, that manages xbox controllers that are added to it.
    /// </summary>
    public class XboxControllerManager
    {
        // Written, 16.07.2018

        #region Fields
        
        /// <summary>
        /// Represents an event that occurs when an xbox controller is connected.
        /// </summary>
        public static event EventHandler ControllerConnected;
        /// <summary>
        /// Represents an event that occurs when an xbox controller is disconnected.
        /// </summary>
        public static event EventHandler ControllerDisconnected;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the amount of supported controllers for the application.
        /// </summary>
        public int numOfControllersSupported
        {
            get;
            private set;
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
        public XboxControllerManager(int numOfControllersToSupport)
        {
            // Written, 16.07.2018

            if (numOfControllersToSupport <= 4 && numOfControllersToSupport > 0)
                this.numOfControllersSupported = numOfControllersToSupport;
            else
                throw new Exception("Invaild amount of controllers to support. MIN = 1, MAX = 4.");
            this.controllers = new XboxController[this.numOfControllersSupported];
            for (int i = 1; i <= numOfControllersToSupport; i++)
            {
                this.addController(new XboxController(i));
            }
            this.monitorControllerConnections = new MonitorControllerConnections(this.numOfControllersSupported, true, this.controllers);
        }

        #endregion

        #region Methods

         /// <summary>
        /// Adds the specified xbox controller to the manager.
        /// </summary>
        /// <param name="xboxController">The controller to add.</param>
        public void addController(XboxController xboxController)
        {
            // Written, 16.07.2018

            bool isControllerAssigned = false;
            for (int i = 0; i < this.numOfControllersSupported; i++)
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
                throw new Exception(String.Format("This instance of {2} only supports {0} controller{1}. You have attempted to add another controller which cannot be executed.",
                    this.numOfControllersSupported, (this.numOfControllersSupported > 1 ? "s" : ""), nameof(XboxControllerManager)));
            }
            else
            {
                if (this.monitorControllerConnections != null)
                {
                    bool isMonitoring = this.monitorControllerConnections.Monitor;
                    this.monitorControllerConnections = new MonitorControllerConnections(this.numOfControllersSupported, isMonitoring, this.controllers);
                }
            }
        }
        /// <summary>
        /// Performs updates for every controller within the manager. Updates <see cref="monitorControllerConnections"/>.
        /// </summary>
        public void onUpdate()
        {
            // Written, 16.07.2018

            foreach (XboxController controller in this.controllers)
            {
                controller.update();
            }
            this.monitorControllerConnections.onUpdate();
        }
        /// <summary>
        /// Performs refreshes for every controller within the manager.
        /// </summary>
        public void onRefresh()
        {
            // Written, 16.07.2018

            foreach (XboxController controller in this.controllers)
            {
                controller.refresh();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the <see cref="ControllerConnected"/> event.
        /// </summary>
        /// <param name="eventArgs">The event data.</param>
        internal static void onControllerConnected(ControllerConnectedEventArgs eventArgs)
        {
            // Written, 16.07.2018

            if (ControllerConnected != null)
                ControllerConnected.Invoke(null, eventArgs);
        }
        /// <summary>
        /// Raises the <see cref="ControllerDisconnected"/> event.
        /// </summary>
        /// <param name="eventArgs">The event data.</param>
        internal static void onControllerDisconnected(ControllerDisconnectedEventArgs eventArgs)
        {
            // Written, 16.07.2018

            if (ControllerDisconnected != null)
                ControllerDisconnected.Invoke(null, eventArgs);
        }

        #endregion
    }
}
