using System;
using TommoJProductions.MoControls.XInputInterpreter;

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
        private MoControlsGUI moControlsGUI;

        protected XboxControlGroup menuUp { get; private set; } = new XboxControlGroup() { buttonInput = XboxButtonEnum.RB };
        protected XboxControlGroup menuDown { get; private set; } = new XboxControlGroup() { buttonInput = XboxButtonEnum.LB };


        private void Start() 
        {
            base.Start();
            moControlsGUI = MoControlsGUI.instance;
        }

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

            if (controller.isConnected)
            {
                int mainMenuItemCount = Enum.GetNames(typeof(MainGUIMenuEnum)).Length - 1;
                int settingsMenuItemCount = Enum.GetNames(typeof(SettingsMenuEnum)).Length - 1;

                if (hasInputFromAxisOrButton(menuDown) > 0)
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
                else if (hasInputFromAxisOrButton(menuUp) > 0)
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
        internal void setControls(XboxControlGroup scrollDown, XboxControlGroup scrollUp, XboxControlGroup menuDown, XboxControlGroup menuUp)
        {
            // Written, 31.07.2022

            setControls(scrollDown, scrollUp);

            this.menuDown = menuDown;
            this.menuUp = menuUp;
        }

        #endregion
    }
}
