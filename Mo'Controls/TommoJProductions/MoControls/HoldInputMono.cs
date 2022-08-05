using System;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents a way to do something after holding down an input for said amount of time.
    /// </summary>
    internal class HoldInputMono : MonoBehaviour
    {
        // Written, 18.12.2018

        #region Fields 

        /// <summary>
        /// The start time.
        /// </summary>
        private float startTime = 0f;
        /// <summary>
        /// 
        /// </summary>
        private float timer = 0f;
        /// <summary>
        /// 
        /// </summary>
        private bool held = false;
        /// <summary>
        /// 
        /// </summary>
        private float holdTime = -1f;
        /// <summary>
        /// 
        /// </summary>
        private XboxButtonEnum keyName = XboxButtonEnum.None;
        /// <summary>
        /// 
        /// </summary>
        private Action action = null;
        /// <summary>
        /// 
        /// </summary>
        private string id = "_id";
        /// <summary>
        /// 
        /// </summary>
        private string hiName = "hi_name";

        #endregion

        #region Methods

        /// <summary>
        /// Updates every frame.
        /// </summary>
        private void Update()
        {
            // Written, 18.12.2018

            if (MoControlsGO.controlManager.xboxController.isConnected)
            {
                if (MoControlsGO.controlManager.xboxController.getButtonDown(keyName))
                {
                    startTime = Time.time;
                    timer = startTime;
                }
                if (MoControlsGO.controlManager.xboxController.getButtonPressed(keyName) && held == false)
                {
                    timer += Time.deltaTime;
                    if (timer > (startTime + holdTime))
                    {
                        held = true;
                        action?.Invoke();
                    }
                }
                if (MoControlsGO.controlManager.xboxController.getButtonUp(keyName))
                {
                    held = false;
                }
            }
        }
        /// <summary>
        /// Sets and initializes hold input mono
        /// </summary>
        /// <param name="inName">The name to set the hold input</param>
        /// <param name="inKeyName">The Xbox Button to assign the hold</param>
        /// <param name="inHoldTime">How long the xbox button needs to be held down for before action execution.</param>
        /// <param name="inAction">The action to perform.</param>
        internal void setData(string inName, XboxButtonEnum inKeyName, float inHoldTime, Action inAction)
        {
            // Written, 19.12.2018

            holdTime = inHoldTime;
            keyName = inKeyName;
            action = inAction;
            hiName = inName;
            id = ("hld_inpt_" + hiName.ToLower().Replace(" ", "") + "_K" + inKeyName).ToLower().Trim();

            MoControlsMod.print("hld_inpt: " + hiName + " initiliazed as '" + id + "'.", Debugging.DebugTypeEnum.full);
        }

        #endregion
    }
}
