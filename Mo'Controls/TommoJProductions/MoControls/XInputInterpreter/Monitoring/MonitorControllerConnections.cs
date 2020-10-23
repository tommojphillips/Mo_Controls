using System.Collections;
using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter.Monitoring
{
    /// <summary>
    /// Represents monitoring methods for monitoring controller connections.
    /// </summary>
    public class MonitorControllerConnections : MonoBehaviour
    {
        // Written, 16.07.2018

        #region Properties / Fields

        private XboxControllerManager xboxControllerManager;
        /// <summary>
        /// Represents a List of xbox controllers to monitor.
        /// </summary>
        private XboxController[] xboxControllers
        {
            get
            {
                return this.xboxControllerManager.controllers;
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

        #endregion

        #region Methods

        private void Start() 
        {
            // Written, 17.10.2020

            this.xboxControllerManager = GetComponent<XboxControllerManager>(); 
            this.controllerConnections = new ControllerConnection[] { new ControllerConnection() /*{ currentConnectionStatus = xboxControllers[0].isConnected, previousConnectionStatus = null }*/ };
            //Only supports 1 controller currently.. no point enumating.
        }
        private IEnumerator updateCoroutine()
        {
            // Written, 16.07.2018

            if (MoControlsSaveData.loadedSaveData.monitiorXboxControllerConnectionStatus)
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
                yield return null;
            }
        }
        private void Update()
        {
            // Written, 16.10.2020

            StartCoroutine(updateCoroutine());
        }

        #endregion
    }
}
