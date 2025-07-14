using System;

using MSCLoader;

using TommoJProductions.MoControlsV2.XInput;
using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    
    public class ChangeInput {

        #region Properties

        public bool reassign_key;
        public string control_name;
        public int index;
        public PLAYER_MODE player_mode;
        public Action<XInputGamepad.INPUT, bool> on_reassign_key;

        #endregion

        #region Constructors

        public ChangeInput(Action<XInputGamepad.INPUT, bool> on_reassign_key) {
            this.on_reassign_key = on_reassign_key;
            reassign_key = false;
            control_name = null;
            index = 0;
        }

        #endregion

        #region Methods

        public void to_polling_state(string control, int index, PLAYER_MODE player_mode) {
            if (!reassign_key) {
                reassign_key = true;
                this.control_name = control;
                this.index = index;
                this.player_mode = player_mode;
            }
        }

        private bool get_control_input(string key, PLAYER_MODE player_mode) {
            XInputGamepad.INPUT input;
            switch(player_mode) {
                case PLAYER_MODE.FOOT_MODE:
                    if (ControlManager.foot_controls.TryGetValue(key, out input)) {
                        return ControlManager.controller.get_input(input) != 0;
                    }
                    break;
                case PLAYER_MODE.DRIVING_MODE:
                    if (ControlManager.driving_controls.TryGetValue(key, out input)) {
                        return ControlManager.controller.get_input(input) != 0;
                    }
                    break;
            }
            return false;
        }

        public void update() {
            if (reassign_key) {

                if (Input.GetMouseButton(0) || get_control_input("MouseButton0", player_mode)) {
                    reassign_key = false;
                    on_reassign_key?.Invoke(XInputGamepad.INPUT.NONE, true); /* Cancel */
                    return;
                }

                if (Input.GetMouseButton(1) || get_control_input("MouseButton1", player_mode)) {
                    reassign_key = false;
                    on_reassign_key?.Invoke(XInputGamepad.INPUT.NONE, false); /* Set to None */
                    return;
                }

                for (int i = 0; i < (int)XInputGamepad.INPUT.COUNT; ++i) {
                    float input = ControlManager.controller.get_input((XInputGamepad.INPUT)i);                                        
                    if (input > 0.25f || input < -0.25f) {
                        reassign_key = false;
                        on_reassign_key?.Invoke((XInputGamepad.INPUT)i, false); /* Set Input */
                        return;
                    }
                }
            }
        }

        #endregion
    }
}
