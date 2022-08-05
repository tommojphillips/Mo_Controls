using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using TommoJProductions.MoControls.XInputInterpreter.Monitoring;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents the Mo'Controls gameobject that holds references to each of the other <see cref="Component"/>s.
    /// </summary>
    public class MoControlsGO : MonoBehaviour
    {
        // Written, 08.10.2018

        #region Properties

        /// <summary>
        /// Represents the GUI for the mod.
        /// </summary>
        internal static MoControlsGUI moControlsGui
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the control manager.
        /// </summary>
        internal static ControlManager controlManager
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents an emulator for the mouse.
        /// </summary>
        internal static MouseEmulator mouseEmulator
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when game starts.
        /// </summary>
        private void Awake()
        {
            // Written, 18.10.2020

            controlManager = gameObject.AddComponent<ControlManager>();
            controlManager.xboxControllerManager = gameObject.AddComponent<XboxControllerManager>();
            controlManager.xboxControllerManager.controller = gameObject.AddComponent<XboxController>();
            controlManager.xboxControllerManager.monitorControllerConnections = gameObject.AddComponent<MonitorControllerConnections>();

            mouseEmulator = gameObject.AddComponent<MouseEmulator>();
            moControlsGui = gameObject.AddComponent<MoControlsGUI>();
            MoControlsMod.print(nameof(MoControlsGO) + ": Started", Debugging.DebugTypeEnum.full);
        }

        #endregion
    }
}
