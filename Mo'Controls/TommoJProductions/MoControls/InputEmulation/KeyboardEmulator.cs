namespace TommoJProductions.MoControls.InputEmulation
{
    public static class KeyboardEmulator
    {
        // Written, 06.10.2020

        /// <summary>
        /// Sends the provided key.
        /// </summary>
        /// <param name="key">The key to send</param>
        public static void Send(ScanCodeShort key)
        {
            // Written, 06.10.2020

            Input[] inputs = new Input[1];
            Input input = new Input();
            input.type = 1; // 1 = Keyboard Input
            input.U.ki.wScan = key;
            input.U.ki.dwFlags = KeyEventF.SCANCODE;
            inputs[0] = input;
            NativeMethods.SendInput(1, inputs, Input.Size);
        }
    }
}
