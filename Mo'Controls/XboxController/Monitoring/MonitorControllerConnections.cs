using System.Threading;
using MSCLoader;

namespace Mo_Controls.XboxController.Monitoring
{
    /// <summary>
    /// Represents monitoring methods for monitoring controller connections.
    /// </summary>
    public class MonitorControllerConnections
    {
        // Written, 16.07.2018

        #region Properties / Fields
        
        /// <summary>
        /// Represents whether the current instance is monitoring for controller connections changing.
        /// </summary>
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
                    {
                        ModConsole.Print("Started Monitoring for controller connections...");
                        this.monitoringThread.Start();
                    }
                }
                else
                {
                    ModConsole.Print("Stopped Monitoring for controller connections...");
                }
                this.monitor = value;
            }
        }        
        /// <summary>
        /// Represents a List of xbox controllers to monitor.
        /// </summary>
        private XboxController[] xboxControllers
        {
            get;
            set;
        }
        /// <summary>
        /// Represents an array of connections.
        /// </summary>
        private ControllerConnection[] controllerConnections
        {
            get;
            set;
        }
        /// <summary>
        /// Represents whether the current instance is monitoring.
        /// </summary>
        private bool monitor;
        /// <summary>
        /// Represents the monitoring thread.
        /// </summary>
        private Thread monitoringThread;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MonitorControllerConnections"/>.
        /// </summary>
        /// <param name="numOfSupportedControllers">Number of supported controllers.</param>
        /// <param name="inMonitor">Monitor connection stautus?</param>
        /// <param name="xboxControllers">An array of xbox controllers.</param>
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

        /// <summary>
        /// Monitors for changing connections and sends events based on what it finds. 
        /// </summary>
        private void monitorControllerConnections()
        {
            // Written, 16.07.2018

            while (this.Monitor && !PlayMakerFSM.ApplicationIsQuitting)
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
