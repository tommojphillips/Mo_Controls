using System;
using UnityEngine;
using TommoJProductions.MoControls.XInputInterpreter;

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
        /// 
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
        private XboxButtonEnum keyName = XboxButtonEnum.NULL;
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
        /// 
        /// </summary>
        private void Update()
        {
            // Written, 18.12.2018

            XboxController xc = MoControlsGO.xboxController;

            if (xc.isConnected)
            {
                if (xc.getButtonDown(keyName))
                {
                    startTime = Time.time;
                    timer = startTime;
                }
                if (xc.getButtonPressed(keyName) && held == false)
                {
                    timer += Time.deltaTime;
                    if (timer > (startTime + holdTime))
                    {
                        held = true;
                        this.action?.Invoke();
                    }
                }
                if (xc.getButtonUp(keyName))
                {
                    held = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="inKeyName"></param>
        /// <param name="inHoldTime"></param>
        /// <param name="inAction"></param>
        internal void setData(string inName, XboxButtonEnum inKeyName, float inHoldTime, Action inAction)
        {
            // Written, 19.12.2018

            this.holdTime = inHoldTime;
            this.keyName = inKeyName;
            this.action = inAction;
            this.hiName = inName;
            this.id = ("hld_inpt_"+ this.hiName.ToLower().Replace(" ", "") +"_K" + inKeyName).ToLower().Trim();

            if (MoControlsMod.debugTypeEquals(Debugging.DebugTypeEnum.full))
                MoControlsMod.print("hld_inpt: " + this.hiName + " initiliazed as '" + this.id + "'.");
        }
        
        #endregion
    }
}
