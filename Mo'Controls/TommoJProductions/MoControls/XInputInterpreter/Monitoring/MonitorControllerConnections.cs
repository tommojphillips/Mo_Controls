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

        #region fields

        private ControllerConnection controllerConnection = new ControllerConnection();
        internal Coroutine routine;

        #endregion

        #region Methods
                
        private void OnEnable()
        {
            // Written, 05.07.2022

            routine = StartCoroutine(updateCoroutine());
        }
        private IEnumerator updateCoroutine()
        {
            // Written, 16.07.2018

            while (isActiveAndEnabled)
            {
                if (MoControlsGO.controlManager.xboxController.isConnected != controllerConnection.currentConnectionStatus)
                {
                    controllerConnection.previousConnectionStatus = controllerConnection.currentConnectionStatus;
                    controllerConnection.currentConnectionStatus = MoControlsGO.controlManager.xboxController.isConnected;

                    if (controllerConnection.currentConnectionStatus == true)
                    {
                        MoControlsGO.controlManager.xboxControllerManager.onControllerConnected(new ControllerConnectionEventArgs(MoControlsGO.controlManager.xboxController));
                    }
                    else if (controllerConnection.currentConnectionStatus == false)
                    {
                        MoControlsGO.controlManager.xboxControllerManager.onControllerDisconnected(new ControllerConnectionEventArgs(MoControlsGO.controlManager.xboxController));
                    }
                }
                yield return null;
            }
            routine = null;
        }

        #endregion
    }
}
