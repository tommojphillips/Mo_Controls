using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                return MoControlsGO.xboxController;
            }
        }

        protected XboxButtonEnum scrollDownB { get; private set; } = XboxButtonEnum.NULL;
        protected XboxButtonEnum scrollUpB { get; private set; } = XboxButtonEnum.NULL;
        protected XboxButtonEnum menuDownB { get; private set; } = XboxButtonEnum.LB;
        protected XboxButtonEnum menuUpB { get; private set; } = XboxButtonEnum.RB;
        protected XboxAxisEnum scrollDownA { get; private set; } = XboxAxisEnum.leftTrigger;
        protected XboxAxisEnum scrollUpA { get; private set; } = XboxAxisEnum.rightTrigger;
        protected XboxAxisEnum menuDownA { get; private set; } = XboxAxisEnum.NULL;
        protected XboxAxisEnum menuUpA { get; private set; } = XboxAxisEnum.NULL;

        private void Update() 
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
                    MouseEmulator.simulateScroll((int)(-input * MouseEmulator.MOUSE_SCROLL_VALUE));
                input = hasInputFromAxisOrButton(scrollUpA, scrollUpB);
                if (input > 0)
                    MouseEmulator.simulateScroll((int)(input * MouseEmulator.MOUSE_SCROLL_VALUE));
            }
        }
        protected float hasInputFromAxisOrButton(XboxAxisEnum xboxAxis, XboxButtonEnum xboxButton)
        {
            // Written, 09.10.2020

            float valueF = 0f;
            if (xboxAxis != XboxAxisEnum.NULL) //  is an axis.
            {
                switch (xboxAxis)
                {
                    case XboxAxisEnum.leftTrigger:
                        valueF = xboxController.getLeftTrigger();
                        break;
                    case XboxAxisEnum.rightTrigger:
                        valueF = xboxController.getRightTrigger();
                        break;
                }
            }
            else if (xboxButton != XboxButtonEnum.NULL) // is a button.
            {
                valueF = xboxController.getButtonDown(xboxButton) ? 1f : 0f;
            }
            return valueF;
        }
        /// <summary>
        /// Sets all xbox controls. NOTE: only set either 'A' (axis) OR 'B' (button) variants of each control type, eg, scrollUpA & scrollUpB. otherwise will
        /// only detect axis input variant.
        /// </summary>
        internal void setControls(XboxAxisEnum scrollUpA = XboxAxisEnum.NULL, XboxButtonEnum scrollUpB = XboxButtonEnum.NULL,
            XboxAxisEnum scrollDownA = XboxAxisEnum.NULL, XboxButtonEnum scrollDownB = XboxButtonEnum.NULL,
            XboxAxisEnum menuUpA = XboxAxisEnum.NULL, XboxButtonEnum menuUpB = XboxButtonEnum.NULL,
            XboxAxisEnum menuDownA = XboxAxisEnum.NULL, XboxButtonEnum menuDownB = XboxButtonEnum.NULL)
        {
            // Written, 09.10.2020

            // Scroll Down
            if (scrollDownA != XboxAxisEnum.NULL)
                this.scrollDownA = scrollDownA;
            if (scrollDownB != XboxButtonEnum.NULL)
                this.scrollDownB = scrollDownB;
            // Scroll Up
            if (scrollUpA != XboxAxisEnum.NULL)
                this.scrollUpA = scrollUpA;
            if (scrollUpB != XboxButtonEnum.NULL)
                this.scrollUpB = scrollUpB;
            // Menu Down
            if (menuDownA != XboxAxisEnum.NULL)
                this.menuDownA = menuDownA;
            if (menuDownB != XboxButtonEnum.NULL)
                this.menuDownB = menuDownB;
            // Menu Up
            if (menuUpA != XboxAxisEnum.NULL)
                this.menuUpA = menuUpA;
            if (menuUpB != XboxButtonEnum.NULL)
                this.menuUpB = menuUpB;
        }
    }
}
