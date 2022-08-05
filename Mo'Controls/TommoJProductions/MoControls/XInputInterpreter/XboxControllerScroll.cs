using TommoJProductions.MoControls.InputEmulation;
using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    public class XboxControllerScroll : MonoBehaviour
    {

        /// <summary>
        /// Represents the xbox controller.
        /// </summary>
        protected XboxController xboxController
        {
            get
            {
                return MoControlsGO.controlManager.xboxController;
            }
        }

        protected XboxAxisEnum scrollDownA { get; private set; } = XboxAxisEnum.LT;
        protected XboxAxisEnum scrollUpA { get; private set; } = XboxAxisEnum.RT;
        protected XboxButtonEnum scrollDownB { get; private set; } = XboxButtonEnum.None;
        protected XboxButtonEnum scrollUpB { get; private set; } = XboxButtonEnum.None;

        protected void Update() 
        {
            controllerScroll();
        }

        /// <summary>
        /// Allows the xbox controller to scroll up/down in the User Interface.
        /// </summary>
        protected internal void controllerScroll()
        {
            // Written, 09.10.2020

            if (xboxController.isConnected)
            {
                float input = hasInputFromAxisOrButton(scrollDownA, scrollDownB);
                if (input > 0)
                {
                    MouseEmulator.simulateScroll((int)(-input * MouseEmulator.MOUSE_SCROLL_VALUE));
                }
                input = hasInputFromAxisOrButton(scrollUpA, scrollUpB);
                if (input > 0)
                    MouseEmulator.simulateScroll((int)(input * MouseEmulator.MOUSE_SCROLL_VALUE));
            }
        }
        protected float hasInputFromAxisOrButton(XboxAxisEnum xboxAxis, XboxButtonEnum xboxButton)
        {
            // Written, 09.10.2020

            float valueF = 0f;
            if (xboxAxis != XboxAxisEnum.None) //  is an axis.
            {
                switch (xboxAxis)
                {
                    case XboxAxisEnum.LT:
                        valueF = xboxController.getLeftTrigger();
                        break;
                    case XboxAxisEnum.RT:
                        valueF = xboxController.getRightTrigger();
                        break;
                }
            }
            else if (xboxButton != XboxButtonEnum.None) // is a button.
            {
                valueF = xboxController.getButtonDown(xboxButton) ? 1f : 0f;
            }
            return valueF;
        }
        /// <summary>
        /// Sets all xbox controls. NOTE: only set either 'A' (axis) OR 'B' (button) variants of each control type, eg, scrollUpA & scrollUpB. otherwise will
        /// only detect axis input variant.
        /// </summary>
        internal void setControls(XboxAxisEnum scrollUpA = XboxAxisEnum.None, XboxButtonEnum scrollUpB = XboxButtonEnum.None, XboxAxisEnum scrollDownA = XboxAxisEnum.None, XboxButtonEnum scrollDownB = XboxButtonEnum.None)
        {
            // Written, 31.07.2022

            // Scroll Down
            if (scrollDownA != XboxAxisEnum.None)
                this.scrollDownA = scrollDownA;
            if (scrollDownB != XboxButtonEnum.None)
                this.scrollDownB = scrollDownB;
            // Scroll Up
            if (scrollUpA != XboxAxisEnum.None)
                this.scrollUpA = scrollUpA;
            if (scrollUpB != XboxButtonEnum.None)
                this.scrollUpB = scrollUpB;
        }
    }
}
