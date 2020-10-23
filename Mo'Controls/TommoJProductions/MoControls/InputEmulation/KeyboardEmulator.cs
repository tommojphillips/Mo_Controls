using System;
using System.Collections;
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

        /// <summary>
        /// Sends the provided key.
        /// </summary>
        /// <param name="key">The key to send</param>
        private static void send(VirtualKeyShort wVk, KeyEventF eventF)
        {
            // Written, 06.10.2020

            Input[] inputs = new Input[1];
            inputs[0].type = 1; // 1 = Keyboard Input
            inputs[0].U = createKeyEvent(wVk, eventF, 0, UIntPtr.Zero);
            if (NativeMethods.SendInput((uint)inputs.Length, inputs, Input.Size) == 0)
                MoControlsMod.print(string.Format("Error: {0}", new Win32Exception(Marshal.GetLastWin32Error()).Message), Debugging.DebugTypeEnum.full);
        }
        /// <summary>
        /// Simulates a key press via User32.dll=>SendInput
        /// </summary>
        /// <param name="key">The key to send.</param>
        public static IEnumerator simulateKeyPressCoroutine(VirtualKeyShort wVk)
        {
            // Written, 08.10.2020

            send(wVk, KeyEventF.KEYDOWN);
            yield return new WaitForSeconds(0.3f);
            send(wVk, KeyEventF.KEYUP);
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
    }
}
