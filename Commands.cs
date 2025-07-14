using System;
using System.IO;
using System.Reflection;

using MSCLoader;

using TommoJProductions.MoControlsV2.XInput;

namespace TommoJProductions.MoControlsV2 {
    
    public class WriteCinputExternInputsCommand : ConsoleCommand {

        public override string Name => "cinputextern";
        public override string Help => $"Writes all cInput external inputs to {CINPUT_EXTERN_INPT_OUTPUT_PATH}";

        internal const string CINPUT_EXTERN_INPT_OUTPUT_PATH = "cInput_ExternalInputs.txt";

        public override void Run(string[] inArgs) {
            FieldInfo _inputName_fi = typeof(cInput).GetField("_inputName", (BindingFlags.Static | BindingFlags.NonPublic));
            FieldInfo _axisName_fi = typeof(cInput).GetField("_axisName", (BindingFlags.Static | BindingFlags.NonPublic));
            
            string[] _inputName = (string[])_inputName_fi.GetValue(null);
            string[] _axisName = (string[])_axisName_fi.GetValue(null);

            var writer = File.CreateText(CINPUT_EXTERN_INPT_OUTPUT_PATH);
            for (int i = 0; i < 250; ++i) {
                if (_inputName[i] != null && _inputName[i] != "" && _inputName[i] != "null") {
                    writer.WriteLine(_inputName[i]);
                }
            }

            for (int i = 0; i < 250; ++i) {
                if (_axisName[i] != null && _axisName[i] != "") {
                    writer.WriteLine(_axisName[i]);
                }
            }
            writer.Close();
        }        

        private void write_cinput_extern_inputs() {
            try {
                File.WriteAllText(CINPUT_EXTERN_INPT_OUTPUT_PATH, cInput.externalInputs);
                MoControlsV2Mod.log("Successfully wrote cInput external inputs to a file, " + CINPUT_EXTERN_INPT_OUTPUT_PATH);
            }
            catch (Exception ex) {
                MoControlsV2Mod.error("Error writing " + CINPUT_EXTERN_INPT_OUTPUT_PATH + "'.\r\nError: " + ex.ToString());
            }
        }
    }
}
