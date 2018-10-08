using System.Threading;
using MSCLoader;
using UnityEngine;

namespace TommoJProdutions.MoControls.XInputInterpreter.Monitoring
{
    /// <summary>
    /// Represents monitoring methods for monitoring controller connections.
    /// </summary>
    public class MonitorControllerConnections : MonoBehaviour
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
                    ModConsole.Print("<color=green>>></color> Started Monitoring for controller connections...");
                }
                else
                {
                    ModConsole.Print("<color=green>>></color> Stopped Monitoring for controller connections...");
                }
                this.monitor = value;
            }
        }        
        /// <summary>
        /// Represents a List of xbox controllers to monitor.
        /// </summary>
        private XboxController[] xboxControllers
        {
            get
            {
                return this.gameObject.GetComponent<XboxControllerManager>().controllers;
            }
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MonitorControllerConnections"/>.
        /// </summary>
        /// <param name="numOfSupportedControllers">Number of supported controllers.</param>
        /// <param name="inMonitor">Monitor connection stautus?</param>
        /// <param name="xboxControllers">An array of xbox controllers.</param>
        public MonitorControllerConnections()
        {
            // Written, 16.07.2018
            
            this.controllerConnections = new ControllerConnection[XboxControllerManager.numOfControllersSupported];
            for (int i = 0; i < XboxControllerManager.numOfControllersSupported; i++)
            {
                XboxController xboxController = xboxControllers[i];
                if (xboxController != null)
                {
                    this.xboxControllers[i] = xboxController;
                    this.controllerConnections[i] = new ControllerConnection() { currentConnectionStatus = xboxController.isConnected, previousConnectionStatus = null };
                }
            }
        }

        #endregion

        #region Methods

        private void Update()
        {
            // Written, 16.07.2018

            if (this.Monitor)
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
                                XboxControllerManager.onControllerConnected(new ControllerConnectionEventArgs(xboxController));
                            }
                            else
                            {
                                if (controllerConnection.currentConnectionStatus == false)
                                {
                                    XboxControllerManager.onControllerDisconnected(new ControllerConnectionEventArgs(xboxController));
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
