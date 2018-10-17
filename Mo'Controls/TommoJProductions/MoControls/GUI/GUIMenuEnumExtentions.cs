using System;
using System.Text;
using System.Linq;

namespace TommoJProductions.MoControls.GUI
{
    public static class GUIMenuEnumExtentions
    {
        public static string toString(this MainGUIMenuEnum? mainGUIMenu)
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
        public static string toString(this SettingsMenuEnum? settingsGUIMenu)
        {
            // Written, 22.08.2018

            switch (settingsGUIMenu)
            {
                case SettingsMenuEnum.MouseEmulation:
                    return "Mouse Emulation";
                case SettingsMenuEnum.XboxController:
                    return "Xbox Controller";
                default:
                    return settingsGUIMenu.ToString();
            }
        }
        public static string toString(this XInputInterpreter.XboxControllerInputMapEnum? inputMenu)
        {
            // Written, 22.08.2018

            switch (inputMenu)
            {
                case XInputInterpreter.XboxControllerInputMapEnum.Norm:
                    return "Normal input (dev)";
                case XInputInterpreter.XboxControllerInputMapEnum.Alt:
                    return "Alternative input (unity detection)";
                case XInputInterpreter.XboxControllerInputMapEnum.Custom:
                    return "Custom input";
                default:
                    return inputMenu.ToString();
            }
        }
        public static string getGameControlAlias(this string gameControlName, bool preserveAcronyms)
        {
            // Written, 16.09.2018

            string alias = "";

            switch (gameControlName)
            {
                case "Left":
                    alias = "Turn Left";
                    break;
                case "Right":
                    alias = "Turn Right";
                    break;                
                case "Boost":
                    alias = "N2O";
                    break;
                case "Swear":
                    alias = "Curse";
                    break;
                case "Drunk":
                    alias = "Talk";
                    break;
                case "Smoking":
                    alias = "Smoke";
                    break;
                case "Handbrake":
                    MSCLoader.Mod mod = MSCLoader.ModLoader.LoadedMods.Where(_mod => _mod.ID == "handbrakemod").ToArray()?[0];
                        alias = mod is null ? "Handbrake" : "Handbrake; <color=green>Using Handbrake Mod v" + mod.Version + "</color>";
                    break;
                default:
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
                    alias = newText.ToString();
                    break;
            }
            return alias;           
        }
    }
}
