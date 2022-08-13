using TommoJProductions.MoControls.InputEmulation;
using UnityEngine;

namespace TommoJProductions.MoControls.XInputInterpreter
{
    public class XboxControllerScroll : MonoBehaviour
    {
        public class XboxControlGroup
        {
            public XboxAxisEnum axisInput = XboxAxisEnum.None;
            public XboxButtonEnum buttonInput = XboxButtonEnum.None;

            public XboxControlGroup(XboxAxisEnum axisInput = XboxAxisEnum.None, XboxButtonEnum buttonInput = XboxButtonEnum.None)
            {
                this.axisInput = axisInput;
                this.buttonInput = buttonInput;
            }

            public XboxControlGroup()
            {
            }
        }

        /// <summary>
        /// Represents the xbox controller.
        /// </summary>
        protected XboxController controller;

        protected XboxControlGroup scrollUp { get; private set; } = new XboxControlGroup() { axisInput = XboxAxisEnum.RT };
        protected XboxControlGroup scrollDown { get; private set; } = new XboxControlGroup() { axisInput = XboxAxisEnum.LT };

        protected void Start()
        {
            controller = XboxControllerManager.instance.controller;
        }

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

            if (controller.isConnected)
            {
                float input = hasInputFromAxisOrButton(scrollDown);
                if (input > 0)
                {
                    MouseEmulator.instance.simulateScroll(-(int)(MouseEmulator.MOUSE_SCROLL_VALUE * input));
                }
                input = hasInputFromAxisOrButton(scrollUp);
                if (input > 0)
                {
                    MouseEmulator.instance.simulateScroll((int)(input * MouseEmulator.MOUSE_SCROLL_VALUE));
                }
            }
        }
        protected float hasInputFromAxisOrButton(XboxControlGroup controlGroup)
        {
            // Written, 09.10.2020

            float valueF = 0f;
            if (controlGroup.axisInput != XboxAxisEnum.None) //  is an axis.
            {
                switch (controlGroup.axisInput)
                {
                    case XboxAxisEnum.LT:
                        valueF = controller.lT.state;
                        break;
                    case XboxAxisEnum.RT:
                        valueF = controller.rT.state;
                        break;
                }
            }
            else if (controlGroup.buttonInput != XboxButtonEnum.None) // is a button.
            {
                valueF = controller.getButtonDown(controlGroup.buttonInput) ? 1f : 0f;
            }
            return valueF;
        }
        /// <summary>
        /// Sets all xbox controls. NOTE: only set either 'A' (axis) OR 'B' (button) variants of each control type, eg, scrollUpA & scrollUpB. otherwise will
        /// only detect axis input variant.
        /// </summary>
        internal void setControls(XboxControlGroup scrollDown, XboxControlGroup scrollUp)
        {
            // Written, 31.07.2022

            this.scrollDown = scrollDown;
            this.scrollUp = scrollUp;
        }
    }
}
