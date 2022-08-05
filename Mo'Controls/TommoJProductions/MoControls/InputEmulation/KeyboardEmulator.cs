using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TommoJProductions.MoControls.InputEmulation
{
    /// <summary>
    /// Represents a CSharp wrapper for simulating a keyboard press.
    /// </summary>
    public static class KeyboardEmulator
    {
        // Written, 08.10.2020

        public const uint PM_NOREMOVE = 0x0000;
        public const uint PM_REMOVE = 0x0001;
        public const uint PM_NOYIELD = 0x0002;

        public const uint PM_QS_KEY = 0x0001 << 16;
        public const uint PM_QS_RAWINPUT = 0x0400 << 16;
        public const uint PM_QS_MOUSEMOVE = 0x0002 << 16;

        public static VirtualKeyShort lastKeyPressed;
        public static VirtualKeyShort currentKeyPressed;

        private static List<VirtualKeyShort> keyboardKeys;
        private static NativeMessage msg;
        private static bool hasInput = false;

        private static bool simulatingKeyPress = false;

        private static WaitForSeconds waitFor;

        static KeyboardEmulator() 
        {
            // Written, 03.08.2022

            keyboardKeys = new List<VirtualKeyShort>();
            Type virtualKeyType = typeof(VirtualKeyShort);
            foreach (var key in Enum.GetNames(virtualKeyType))
            {
                keyboardKeys.Add((VirtualKeyShort)Enum.Parse(virtualKeyType, key));
            }
            waitFor = new WaitForSeconds(0.3f);
        }


        /// <summary>
        /// Simulates a key press via User32.dll=>SendInput
        /// </summary>
        /// <param name="key">The key to send.</param>
        public static IEnumerator simulateKeyPressCoroutine(VirtualKeyShort wVk)
        {
            // Written, 08.10.2020

            simulatingKeyPress = true;
            send(wVk, KeyEventF.KEYDOWN);
            yield return waitFor;
            send(wVk, KeyEventF.KEYUP);
            yield return waitFor;
            simulatingKeyPress = false;
        }

        /// <summary>
        /// Sends the provided key.
        /// </summary>
        /// <param name="key">The key to send</param>
        private static void send(VirtualKeyShort wVk, KeyEventF eventF)
        {
            // Written, 06.10.2020

            Input[] inputs = new Input[1];
            inputs[0].type = 1; // 1 = Keyboard Input | 0 = Mouse input
            inputs[0].U = createKeyEvent(wVk, eventF, 0, UIntPtr.Zero);
            if (NativeMethods.SendInput((uint)inputs.Length, inputs, Input.Size) == 0)
                MoControlsMod.print(string.Format("Error: {0}", new Win32Exception(Marshal.GetLastWin32Error()).Message), Debugging.DebugTypeEnum.none);
        }
        /// <summary>
        /// Creates a key event wrapped in a inputUnion structure. Passed to: <see cref="NativeMethods.SendInput(uint, Input[], int)"/>
        /// </summary>
        /// <param name="wScan">The scan keycode</param>
        /// <param name="dwFlags">dword flags</param>
        /// <param name="time">the time?</param>
        /// <param name="dwExtraInfo">dword extra info</param>
        /// <returns></returns>
        private static InputUnion createKeyEvent(VirtualKeyShort wVk, KeyEventF dwFlags, int time, UIntPtr dwExtraInfo)
        {
            // Written, 08.10.2020

            InputUnion result = new InputUnion
            {
                ki = new KeyboardInput()
                {
                    dwExtraInfo = dwExtraInfo,
                    dwFlags = dwFlags,
                    time = time,
                    wVk = wVk,
                }
            };
            return result;
        }

        public static bool anyInput()
        {
            // Written, 04.08.2022
                        
            hasInput = NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_QS_KEY | PM_NOREMOVE | PM_NOYIELD);

            if (hasInput)
            {
                currentKeyPressed = (VirtualKeyShort)msg.wParam;

                if (keyboardKeys.Contains(currentKeyPressed))
                {
                    lastKeyPressed = currentKeyPressed;
                    return !simulatingKeyPress;
                }
            }
            return false;
        }
    }
}
