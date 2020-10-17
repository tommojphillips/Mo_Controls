using System;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls.GUI
{
    /// <summary>
    /// Represents xbox controller navigation in the User-Interface.
    /// </summary>
    internal class GuiNav : MonoBehaviour
    {
        // Written, 28.12.2018

        #region Properties

        /// <summary>
        /// Represents the GUI for the mod.
        /// </summary>
        private MoControlsGUI moControlsGUI
        {
            get
            {
                return MoControlsGO.moControlsGui;
            }
        }
        /// <summary>
        /// Represents the xbox controller.
        /// </summary>
        private XboxController xboxController
        {
            get
            {
                return MoControlsGO.xboxController;
            }
        }

        internal XboxButtonEnum scrollDownB { get; private set; } = XboxButtonEnum.NULL;
        internal XboxButtonEnum scrollUpB { get; private set; } = XboxButtonEnum.NULL;
        internal XboxButtonEnum menuDownB { get; private set; } = XboxButtonEnum.LB;
        internal XboxButtonEnum menuUpB { get; private set; } = XboxButtonEnum.RB;
        internal XboxAxisEnum scrollDownA { get; private set; } = XboxAxisEnum.leftTrigger;
        internal XboxAxisEnum scrollUpA { get; private set; } = XboxAxisEnum.rightTrigger;
        internal XboxAxisEnum menuDownA { get; private set; } = XboxAxisEnum.NULL;
        internal XboxAxisEnum menuUpA { get; private set; } = XboxAxisEnum.NULL;

        #endregion

        #region Methods

        /// <summary>
        /// Occurs every frame (Unity Method)
        /// </summary>
        private void Update()
        {
            // Written, 04.01.2019

            if (this.moControlsGUI.controlsGuiOpened)
                this.controllerMenuChange();
            this.controllerScroll();
        }
        /// <summary>
        /// Allows the xbox controller to scroll up/down in the User Interface.
        /// </summary>
        private void controllerScroll()
        {
            // Written, 09.10.2020

            if (this.xboxController.isConnected)
            {
                float input = this.hasInputFromAxisOrButton(this.scrollDownA, this.scrollDownB);
                if (input > 0)
                    MouseEmulator.simulateScroll((int)(-input * MouseEmulator.MOUSE_SCROLL_VALUE));
                input = this.hasInputFromAxisOrButton(this.scrollUpA, this.scrollUpB);
                if (input > 0)
                    MouseEmulator.simulateScroll((int)(input * MouseEmulator.MOUSE_SCROLL_VALUE));
            }
        }
        private float hasInputFromAxisOrButton(XboxAxisEnum xboxAxis, XboxButtonEnum xboxButton)
        {
            // Written, 09.10.2020

            float valueF = 0.0f;
            if (xboxAxis != XboxAxisEnum.NULL) //  is an axis.
            {
                switch (xboxAxis)
                {
                    case XboxAxisEnum.leftTrigger:
                        valueF = this.xboxController.getLeftTrigger();
                        break;
                    case XboxAxisEnum.rightTrigger:
                        valueF = this.xboxController.getRightTrigger();
                        break;
                }
            }
            else if (xboxButton != XboxButtonEnum.NULL) // is a button.
            {
                valueF = this.xboxController.getButtonDown(xboxButton) ? 1f : 0.0f;
            }
            return valueF;
        }
        /// <summary>
        /// Changes to the next/previous menu.
        /// </summary>
        private void controllerMenuChange()
        {
            // Written, 04.01.2019

            if (this.xboxController.isConnected)
            {
                int mainMenuItemCount = Enum.GetNames(typeof(MainGUIMenuEnum)).Length - 1;
                int settingsMenuItemCount = Enum.GetNames(typeof(SettingsMenuEnum)).Length - 1;


                if (this.hasInputFromAxisOrButton(menuDownA, menuDownB) > 0)
                {
                    if (this.moControlsGUI.mainGUIMenu == 0)
                    {
                        this.moControlsGUI.mainGUIMenu = (MainGUIMenuEnum)mainMenuItemCount;
                    }
                    else
                    {
                        if (this.moControlsGUI.mainGUIMenu == MainGUIMenuEnum.Settings)
                        {
                            if (this.moControlsGUI.settingsMenu > 0)
                            {
                                this.moControlsGUI.settingsMenu--;
                                return;
                            }
                        }
                        this.moControlsGUI.mainGUIMenu--;
                    }
                }
                else if (this.hasInputFromAxisOrButton(menuUpA, menuUpB) > 0)
                {
                    if (this.moControlsGUI.mainGUIMenu == (MainGUIMenuEnum)mainMenuItemCount)
                    {
                        if (this.moControlsGUI.settingsMenu < (SettingsMenuEnum)settingsMenuItemCount)
                        {
                            this.moControlsGUI.settingsMenu++;
                        }
                        else
                        {
                            this.moControlsGUI.mainGUIMenu = 0;
                        }
                    }
                    else
                    {
                        this.moControlsGUI.mainGUIMenu++;
                    }
                }
            }
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
            {
                this.scrollDownA = scrollDownA;
                this.scrollDownB = XboxButtonEnum.NULL;
            }
            if (scrollDownB != XboxButtonEnum.NULL)
            {
                this.scrollDownB = scrollDownB;
                this.scrollDownA = XboxAxisEnum.NULL;
            }
            // Scroll Up
            if (scrollUpA != XboxAxisEnum.NULL)
            {
                this.scrollUpA = scrollUpA;
                this.scrollUpB = XboxButtonEnum.NULL;
            }
            if (scrollUpB != XboxButtonEnum.NULL)
            {
                this.scrollUpB = scrollUpB;
                this.scrollUpA = XboxAxisEnum.NULL;
            }
            // Menu Down
            if (menuDownA != XboxAxisEnum.NULL)
            {
                this.menuDownA = menuDownA;
                this.menuDownB = XboxButtonEnum.NULL;
            }
            if (menuDownB != XboxButtonEnum.NULL)
            {
                this.menuDownB = menuDownB;
                this.menuDownA = XboxAxisEnum.NULL;
            }
            // Menu Up
            if (menuUpA != XboxAxisEnum.NULL)
            {
                this.menuUpA = menuUpA;
                this.menuUpB = XboxButtonEnum.NULL;
            }
            if (menuUpB != XboxButtonEnum.NULL)
            {
                this.menuUpB = menuUpB;
                this.menuUpA = XboxAxisEnum.NULL;
            }
        }
        #endregion
    }
}
