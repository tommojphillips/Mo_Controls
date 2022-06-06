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


        #endregion

        #region Methods

        /// <summary>
        /// Occurs every frame (Unity Method)
        /// </summary>
        private void Update()
        {
            // Written, 04.01.2019

            if (moControlsGUI.controlsGuiOpened)
                controllerMenuChange();
            controllerScroll();
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
       
        #endregion
    }
}
