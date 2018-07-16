using System.Threading;

namespace Mo_Controls.XboxController.Monitoring
{
    public class MonitorControllerConnections
    {
        // Written, 16.07.2018

        #region Properties / Fields

        public bool Monitor
        {
            get
            {
                return this.monitor;
            }
            set
            {
                if (value)
                {
                    if (!this.monitoringThread.IsAlive)
                        this.monitoringThread.Start();
                }
                this.monitor = value;
            }
        }        
        public XboxController[] xboxControllers
        {
            get;
            set;
        }
        public ControllerConnection[] controllerConnections
        {
            get;
            set;
        }
        private bool monitor;
        private Thread monitoringThread;

        #endregion

        #region Constructors

        public MonitorControllerConnections(int numOfSupportedControllers, bool inMonitor = false, params XboxController[] xboxControllers)
        {
            // Written, 16.07.2018

            this.monitoringThread = new Thread(this.monitorControllerConnections);
            this.controllerConnections = new ControllerConnection[numOfSupportedControllers];
            this.xboxControllers = new XboxController[numOfSupportedControllers];
            for (int i = 0; i < numOfSupportedControllers; i++)
            {
                XboxController xboxController = xboxControllers[i];
                if (xboxController != null)
                {
                    this.xboxControllers[i] = xboxController;
                    this.controllerConnections[i] = new ControllerConnection() { currentConnectionStatus = xboxController.isConnected, previousConnectionStatus = null };
                }
            }
            this.Monitor = inMonitor;
        }

        #endregion

        #region Methods

        private void monitorControllerConnections()
        {
            // Written, 16.07.2018

            while (this.Monitor)
            {
               for (int i = 0; i < this.xboxControllers.Length; i++)
                {
                    ControllerConnection controllerConnection = this.controllerConnections[i];
                    XboxController xboxController = xboxControllers[i];
                    if (xboxController != null)
                    {
                        if (xboxController.isConnected != controllerConnection.currentConnectionStatus)
                        {
                            controllerConnection.previousConnectionStatus = controllerConnection.currentConnectionStatus;
                            controllerConnection.currentConnectionStatus = xboxController.isConnected;

                            if (controllerConnection.currentConnectionStatus == true)
                            {
                                XboxControllerManager.onControllerConnected(new ControllerConnectedEventArgs(xboxController));
                            }
                            else
                            {
                                if (controllerConnection.currentConnectionStatus == false)
                                {
                                    XboxControllerManager.onControllerDisconnected(new ControllerDisconnectedEventArgs(xboxController));
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
