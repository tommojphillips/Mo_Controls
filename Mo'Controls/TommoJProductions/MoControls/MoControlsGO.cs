using TommoJProductions.MoControls.GUI;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
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

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when game starts.
        /// </summary>
        private void Awake()
        {
            // Written, 18.10.2020

            gameObject.AddComponent<ControlManager>();
            gameObject.AddComponent<XboxControllerManager>();

            gameObject.AddComponent<MouseEmulator>();
            gameObject.AddComponent<MoControlsGUI>();
            MoControlsMod.print(nameof(MoControlsGO) + ": Started", Debugging.DebugTypeEnum.full);
        }

        #endregion
    }
}
