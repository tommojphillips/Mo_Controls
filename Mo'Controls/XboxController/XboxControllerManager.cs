using System;
using Mo_Controls.XboxController.Monitoring;

namespace Mo_Controls.XboxController
{
    public class XboxControllerManager
    {
        // Written, 16.07.2018

        #region Fields

        public static event EventHandler ControllerConnected;
        public static event EventHandler ControllerDisconnected;

        #endregion

        #region Properties

        public int numOfControllersSupported
        {
            get;
            private set;
        }
        private XboxController[] controllers
        {
            get;
            set;
        }
        public MonitorControllerConnections monitorControllerConnections
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public XboxControllerManager(int numOfControllersToSupport)
        {
            // Written, 16.07.2018

            this.preInitialize(numOfControllersToSupport);
            this.postInitialize();
        }
        public XboxControllerManager(int numOfControllersToSupport, params XboxController[] controllers)
        {
            // Written, 16.07.2018

            if (controllers.Length <= numOfControllersToSupport)
            {
                this.preInitialize(numOfControllersToSupport);
                foreach (XboxController controller in controllers)
                {
                    this.addController(controller);
                }
                this.postInitialize();
            }
            else
                throw new Exception("The provided controllers were not in the correct range of first argument, numOfControllersToSupport. Specify between 1-numOfControllersToSupport value.");
        }

        #endregion

        #region Methods

        private void preInitialize(int numOfControllersToSupport)
        {
            // Written, 16.07.2018

            if (numOfControllersToSupport <= 4 && numOfControllersToSupport > 0)
                this.numOfControllersSupported = numOfControllersToSupport;
            else
                throw new Exception("Invaild amount of controllers to support. MIN = 1, MAX = 4.");
            this.controllers = new XboxController[this.numOfControllersSupported];
        }
        private void postInitialize()
        {
            // Written, 16.07.2018

            this.monitorControllerConnections = new MonitorControllerConnections(this.numOfControllersSupported, true, this.controllers);
        }
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
                bool isMonitoring = this.monitorControllerConnections.Monitor;
                this.monitorControllerConnections = new MonitorControllerConnections(this.numOfControllersSupported, isMonitoring, this.controllers);
            }
        }
        public void update()
        {
            // Written, 16.07.2018

            foreach (XboxController controller in this.controllers)
            {
                controller.update();
            }
        }
        public void refresh()
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
