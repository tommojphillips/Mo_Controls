using System.Collections;
using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter.Monitoring
{
    /// <summary>
    /// Represents monitoring methods for monitoring controller connections.
    /// </summary>
    public class MonitorControllerConnections : SingletonMonoBehaviour<MonitorControllerConnections>
    {
        // Written, 16.07.2018

        #region fields / Properties

        private ControllerConnection controllerConnection;
        private ControllerConnectionEventArgs eventArgs;
        private XboxController controller;
        private WaitForSeconds step;

        internal Coroutine routine { get; private set; }

        #endregion

        #region Constructors

        public MonitorControllerConnections()
        {
            // Written, 05.08.2022

            controllerConnection = new ControllerConnection();
            eventArgs = new ControllerConnectionEventArgs();
        }

        #endregion

        #region unity runtime

        private void Start()
        {
            // Written, 05.08.2022

            controller = XboxControllerManager.instance.controller;
            setTimeStep();
            startMonitoring();
        }

        #endregion

        #region IEnumerators

        private IEnumerator updateCoroutine()
        {
            // Written, 16.07.2018 | Modified, 05.07.2022

            /*while (isActiveAndEnabled)
            {
                if (controller.isConnected != controllerConnection.currentConnectionStatus)
                {
                    controllerConnection.previousConnectionStatus = controllerConnection.currentConnectionStatus;
                    controllerConnection.currentConnectionStatus = controller.isConnected;

                    eventArgs.setData(controller);

                    if (controllerConnection.currentConnectionStatus == true)
                    {
                        XboxControllerManager.instance.onControllerConnected(eventArgs);
                    }
                    else if (controllerConnection.currentConnectionStatus == false)
                    {
                        XboxControllerManager.instance.onControllerDisconnected(eventArgs);
                    }
                }
                yield return step;
            }*/
            yield return null;
        }

        #endregion

        #region Methods

        internal void setTimeStep()
        {
            // Written, 05.08.2022
            
            step = new WaitForSeconds(MoControlsSaveData.loadedSaveData.monitorControllerConnectionsTimeStep);
        }
        internal void startMonitoring() 
        {
            // Written, 05.08.2022

            if (routine != null)
            {
                StopCoroutine(routine);
            }
            routine = StartCoroutine(updateCoroutine());
        }

        #endregion
    }
}
