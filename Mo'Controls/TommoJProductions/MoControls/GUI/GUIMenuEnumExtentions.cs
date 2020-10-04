using System;
using System.Text;
using System.Linq;

namespace TommoJProductions.MoControls.GUI
{
    public static class GUIMenuEnumExtentions
    {
        public static string toString(this MainGUIMenuEnum? inMainGUIMenu)
        {
            // Written, 22.08.2018

            switch (inMainGUIMenu)
            {
                case MainGUIMenuEnum.DrivingControls:
                    return "Driving Controls";
                case MainGUIMenuEnum.FootControls:
                    return "Foot Controls";
                case MainGUIMenuEnum.ModControls:
                    return "Mod Controls";
                default:
                    return inMainGUIMenu.ToString();
            }
        }
        public static string toString(this SettingsMenuEnum? inSettingsGUIMenu)
        {
            // Written, 22.08.2018

            switch (inSettingsGUIMenu)
            {
                case SettingsMenuEnum.MouseEmulation:
                    return "Mouse Emulation";
                case SettingsMenuEnum.XboxController:
                    return "Xbox Controller";
                default:
                    return inSettingsGUIMenu.ToString();
            }
        }
        public static string toString(this XInputInterpreter.XboxControllerInputMapEnum? inInputMenu)
        {
            // Written, 22.08.2018

            switch (inInputMenu)
            {
                case XInputInterpreter.XboxControllerInputMapEnum.Norm:
                    return "Normal input (dev)";
                case XInputInterpreter.XboxControllerInputMapEnum.Alt:
                    return "Alternative input (unity detection)";
                case XInputInterpreter.XboxControllerInputMapEnum.Custom:
                    return "Custom input";
                default:
                    return inInputMenu.ToString();
            }
        }
        internal static string getGameControlAlias(this GameControlsEnum inGameControl, bool inPreserveAcronyms)
        {
            // Written, 16.09.2018

            string alias = "";

            switch (inGameControl)
            {
                case GameControlsEnum.Left:
                    alias = "Turn Left";
                    break;
                case GameControlsEnum.Right:
                    alias = "Turn Right";
                    break;                
                case GameControlsEnum.Boost:
                    alias = "N2O";
                    break;
                case GameControlsEnum.Swear:
                    alias = "Curse";
                    break;
                case GameControlsEnum.Drunk:
                    alias = "Talk";
                    break;
                case GameControlsEnum.Smoke:
                    alias = "Smoke";
                    break;
                case GameControlsEnum.Handbrake:
                    try
                    {
                        MSCLoader.Mod mod = MSCLoader.ModLoader.LoadedMods.Where(_mod => _mod.ID == "handbrakemod").ToArray()[0];
                        alias = "Handbrake; <color=green>Using Handbrake Mod <b>v" + mod.Version + "</b></color>";
                    }
                    catch (IndexOutOfRangeException)
                    {
                        alias = "Handbrake";
                    }
                    break;
                case GameControlsEnum.WristWatch:
                    alias = "Wrist Watch";
                    break;
                default:
                    string gameControlName = inGameControl.getName();
                    if (String.IsNullOrEmpty(gameControlName))
                        return string.Empty;
                    StringBuilder newText = new StringBuilder(gameControlName.Length * 2);
                    newText.Append(gameControlName[0]);
                    for (int i = 1; i < gameControlName.Length; i++)
                    {
                        if (char.IsUpper(gameControlName[i]))
                            if ((gameControlName[i - 1] != ' ' && !char.IsUpper(gameControlName[i - 1])) ||
                                (inPreserveAcronyms && char.IsUpper(gameControlName[i - 1]) &&
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
