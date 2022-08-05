using System;
using TommoJProductions.MoControls.InputEmulation;
using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls.GUI
{
    /// <summary>
    /// Represents xbox controller navigation in the User-Interface.
    /// </summary>
    internal class GuiNav : XboxControllerScroll
    {
        // Written, 28.12.2018


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

        protected XboxAxisEnum menuDownA { get; private set; } = XboxAxisEnum.None;
        protected XboxAxisEnum menuUpA { get; private set; } = XboxAxisEnum.None;
        protected XboxButtonEnum menuDownB { get; private set; } = XboxButtonEnum.LB;
        protected XboxButtonEnum menuUpB { get; private set; } = XboxButtonEnum.RB;

        #region Methods

        /// <summary>
        /// Occurs every frame (Unity Method)
        /// </summary>
        private new void Update()
        {
            // Written, 04.01.2019

            base.Update();
            if (moControlsGUI.controlsGuiOpened)
                controllerMenuChange();
        }  
        /// <summary>
        /// Changes to the next/previous menu.
        /// </summary>
        private void controllerMenuChange()
        {
            // Written, 04.01.2019

            if (xboxController.isConnected)
            {
                int mainMenuItemCount = Enum.GetNames(typeof(MainGUIMenuEnum)).Length - 1;
                int settingsMenuItemCount = Enum.GetNames(typeof(SettingsMenuEnum)).Length - 1;

                if (hasInputFromAxisOrButton(menuDownA, menuDownB) > 0)
                {
                    if (moControlsGUI.mainGUIMenu == 0)
                    {
                        moControlsGUI.mainGUIMenu = (MainGUIMenuEnum)mainMenuItemCount;
                    }
                    else
                    {
                        if (moControlsGUI.mainGUIMenu == MainGUIMenuEnum.Settings)
                        {
                            if (moControlsGUI.settingsMenu > 0)
                            {
                                moControlsGUI.settingsMenu--;
                                return;
                            }
                        }
                        moControlsGUI.mainGUIMenu--;
                    }
                }
                else if (hasInputFromAxisOrButton(menuUpA, menuUpB) > 0)
                {
                    if (moControlsGUI.mainGUIMenu == (MainGUIMenuEnum)mainMenuItemCount)
                    {
                        if (moControlsGUI.settingsMenu < (SettingsMenuEnum)settingsMenuItemCount)
                        {
                            moControlsGUI.settingsMenu++;
                        }
                        else
                        {
                            moControlsGUI.mainGUIMenu = 0;
                        }
                    }
                    else
                    {
                        moControlsGUI.mainGUIMenu++;
                    }
                }
            }
        }

        /// <summary>
        /// Sets all xbox controls. NOTE: only set either 'A' (axis) OR 'B' (button) variants of each control type, eg, scrollUpA & scrollUpB. otherwise will
        /// only detect axis input variant.
        /// </summary>
        internal void setControls(XboxAxisEnum scrollUpA = XboxAxisEnum.None, XboxButtonEnum scrollUpB = XboxButtonEnum.None,
            XboxAxisEnum scrollDownA = XboxAxisEnum.None, XboxButtonEnum scrollDownB = XboxButtonEnum.None,
            XboxAxisEnum menuUpA = XboxAxisEnum.None, XboxButtonEnum menuUpB = XboxButtonEnum.None,
            XboxAxisEnum menuDownA = XboxAxisEnum.None, XboxButtonEnum menuDownB = XboxButtonEnum.None)
        {
            // Written, 31.07.2022

            base.setControls(scrollUpA, scrollUpB, scrollDownA, scrollDownB);

            // Menu Down
            if (menuDownA != XboxAxisEnum.None)
                this.menuDownA = menuDownA;
            if (menuDownB != XboxButtonEnum.None)
                this.menuDownB = menuDownB;
            // Menu Up
            if (menuUpA != XboxAxisEnum.None)
                this.menuUpA = menuUpA;
            if (menuUpB != XboxButtonEnum.None)
                this.menuUpB = menuUpB;
        }
        #endregion
    }
}
