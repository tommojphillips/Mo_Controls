﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using static UnityEngine.GUILayout;

namespace TommoJProductions.MoControls.GUI
{
    public static class GUIExtentions
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
                case SettingsMenuEnum.Axis:
                default:
                    return inSettingsGUIMenu.ToString();
            }
        }
        private static StringBuilder builder;
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
                        alias = "<color=green>Handbrake Mod<b> v" + mod.Version + "</b></color>";
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
                    string gameControlName = inGameControl.ToString();
                    if (gameControlName == GameControlsEnum.Null.ToString())
                        return string.Empty;
                    builder = new StringBuilder(gameControlName.Length * 2);
                    builder.Append(gameControlName[0]);
                    for (int i = 1; i < gameControlName.Length; i++)
                    {
                        if (char.IsUpper(gameControlName[i]))
                            if ((gameControlName[i - 1] != ' ' && !char.IsUpper(gameControlName[i - 1])) ||
                                (inPreserveAcronyms && char.IsUpper(gameControlName[i - 1]) &&
                                 i < gameControlName.Length - 1 && !char.IsUpper(gameControlName[i + 1])))
                                builder.Append(' ');
                        builder.Append(gameControlName[i]);
                    }
                    alias = builder.ToString();
                    break;
            }
            return alias;
        }

        private static Dictionary<string, string> descriptionCache = new Dictionary<string, string>();
        private static DescriptionAttribute[] temp_descriptionArray;
        private static string temp_descriptionString;
        private static MemberInfo temp_descriptionMember;
        private static string temp_descriptionKey;
        /// <summary>
        /// Gets <see cref="DescriptionAttribute.Description"/> on provided object type. if attribute doesn't exist, returns <see cref="MemberInfo.Name"/>
        /// </summary>
        /// <param name="mi">the member info to get info from.</param>
        public static string getDescription(this MemberInfo mi)
        {
            // Written, 06.08.2022

            temp_descriptionKey = mi.DeclaringType?.Name + mi.Name;
            if (descriptionCache.ContainsKey(temp_descriptionKey))
            {
                return descriptionCache[temp_descriptionKey] ?? mi.Name;
            }
            addDesCache(temp_descriptionKey, mi);
            return temp_descriptionString;
        }
        public static string getDescription(this Type type, string memberName)
        {
            // Written, 06.08.2022

            temp_descriptionKey = type.Name + memberName;
            if (descriptionCache.ContainsKey(temp_descriptionKey))
            {
                return descriptionCache[temp_descriptionKey] ?? memberName;
            }
            temp_descriptionMember = type.GetMember(memberName)?[0];
            addDesCache(temp_descriptionKey, temp_descriptionMember);
            return temp_descriptionString;
        }
        private static void addDesCache(string key, MemberInfo mi) 
        {
            // Written, 06.08.2022

            temp_descriptionArray = mi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            if (temp_descriptionArray.Length > 0)
            {
                temp_descriptionString = temp_descriptionArray[0].Description;
            }
            else
            {
                temp_descriptionString = null;
            }
            descriptionCache.Add(key, temp_descriptionString);
        }
        /// <summary>
        /// [GUI] draws an enum that can be edited as a list of toggles.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="e">Reference enum (selected)</param>
        public static void drawPropertyEnum<T>(ref T e, ref bool saveSettings) where T : Enum
        {
            // Written, 25.05.2022
            
            Type t = e.GetType();
            string[] enumNames = Enum.GetNames(t);
            string n;
            bool isSelected;
            for (int i = 0; i < enumNames.Length; i++)
            {
                n = e.ToString();
                isSelected = n == enumNames[i];
                if (Toggle(isSelected, t.GetField(enumNames[i]).getDescription(), MaxWidth((10 * enumNames[i].Length) + 10)) && !isSelected)
                {
                    e = (T)Enum.Parse(t, enumNames[i]);
                    saveSettings = true;
                }
            }
        }
    }
}
