using System;

using MSCLoader;

using TommoJProductions.MoControls.XInputInterpreter.Monitoring;

using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    /// <summary>
    /// Represents an xbox controller manager, that manages xbox controllers that are added to it.
    /// </summary>
    public class XboxControllerManager : SingletonMonoBehaviour<XboxControllerManager>
    {
        // Written, 16.07.2018

        #region Fields

        /// <summary>
        /// Represents the amount of supported controllers for the application.
        /// </summary>
        public const int numOfControllersSupported = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the controllers.
        /// </summary>
        internal XboxController controller
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        public XboxControllerManager()
        {
            XboxController.connected += controllerConnected;
            XboxController.disconnected += controllerDisconnected;
            controller = new XboxController();
        }


        #endregion

        #region Unity runtime

        private void Update()
        {
            // Written, 23.10.2020

            controller.update();
        }
        private void LateUpdate()
        {
            controller.refresh();
        }

        #endregion

        private void controllerDisconnected(XboxController controller) 
        {
            if (ControlManager.instance.scrollCoroutine != null)
                StopCoroutine(ControlManager.instance.scrollFunction());
            MoControlsMod.print("<color=grey><i>Controller: " + controller.index + "</i></color> <color=red>Disconnected</color>", Debugging.DebugTypeEnum.none);
        }

        private void controllerConnected(XboxController controller) 
        {
            if (ControlManager.instance.scrollCoroutine == null)
                ControlManager.instance.scrollCoroutine = StartCoroutine(ControlManager.instance.scrollFunction());
            MoControlsMod.print("<color=grey><i>Controller " + controller.index + "</i></color> <color=green>Connected</color>", Debugging.DebugTypeEnum.none);
        }
    }
}
