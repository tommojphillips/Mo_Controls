namespace Mo_Controls.McGUI
{
    public static class GUIMenuEnumExtentions
    {
        public static string toString(this MainGUIMenuEnum mainGUIMenu)
        {
            // Written, 22.08.2018

            switch (mainGUIMenu)
            {
                case MainGUIMenuEnum.DrivingControls:
                    return "Driving Controls";
                case MainGUIMenuEnum.FootControls:
                    return "Foot Controls";
                default:
                    return mainGUIMenu.ToString();
            }
        }
        public static string toString(this SettingsMenuEnum settingsGUIMenu)
        {
            // Written, 22.08.2018

            switch (settingsGUIMenu)
            {
                case SettingsMenuEnum.MouseEmulation:
                    return "Mouse Emulation";
                default:
                    return settingsGUIMenu.ToString();
            }
        }
    }
}
