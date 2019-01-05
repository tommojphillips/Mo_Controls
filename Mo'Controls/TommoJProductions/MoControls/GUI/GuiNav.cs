using System;
using UnityEngine;
using TommoJProductions.MoControls.MouseEmulation;
using TommoJProductions.MoControls.XInputInterpreter;

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

        #endregion

        #region Methods

        /// <summary>
        /// Occurs every frame (Unity Method)
        /// </summary>
        private void Update()
        {
            // Written, 04.01.2019

            this.controllerMenuChange();
            this.controllerScroll();
        }
        /// <summary>
        /// Allows the xbox controller to scroll up/down in the User Interface.
        /// </summary>
        private void controllerScroll()
        {
            // Written, 04.01.2018

            if (this.xboxController.isConnected)
            {
                float triggerValue = this.xboxController.getLeftTrigger();
                if (triggerValue > 0)
                    MouseEmulator.simulateScroll((int)(triggerValue * -120));
                triggerValue = this.xboxController.getRightTrigger();
                if (triggerValue > 0)
                    MouseEmulator.simulateScroll((int)(triggerValue * 120));
            }
        }
        /// <summary>
        /// Changes to the next/previous menu.
        /// </summary>
        private void controllerMenuChange()
        {
            // Written, 04.01.2019

            int mainMenuItemCount = Enum.GetNames(typeof(MainGUIMenuEnum)).Length - 1;
            int settingsMenuItemCount = Enum.GetNames(typeof(SettingsMenuEnum)).Length - 1;

            if (this.xboxController.getButtonDown(XboxButtonEnum.LB))
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
            else
            {
                if (this.xboxController.getButtonDown(XboxButtonEnum.RB))
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

        #endregion
    }
}
