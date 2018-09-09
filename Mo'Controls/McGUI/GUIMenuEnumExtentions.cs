using System;
using System.Text;

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
                case MainGUIMenuEnum.ModControls:
                    return "Mod Controls";
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
        public static string gameControlToString(this string gameControlName, bool preserveAcronyms)
        {
            // Written, 09.09.2018

            if (String.IsNullOrEmpty(gameControlName))
                return string.Empty;
            StringBuilder newText = new StringBuilder(gameControlName.Length * 2);
            newText.Append(gameControlName[0]);
            for (int i = 1; i < gameControlName.Length; i++)
            {
                if (char.IsUpper(gameControlName[i]))
                    if ((gameControlName[i - 1] != ' ' && !char.IsUpper(gameControlName[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(gameControlName[i - 1]) &&
                         i < gameControlName.Length - 1 && !char.IsUpper(gameControlName[i + 1])))
                        newText.Append(' ');
                newText.Append(gameControlName[i]);
            }
            return newText.ToString();
        }
    }
}
