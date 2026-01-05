using System;

using MSCLoader;

using TommoJProductions.MoControlsV2.XInput;
using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    
    public class Change_Input {
        public bool reassign_key;
        public string control_name;
        public int index;
        public PLAYER_MODE player_mode;
        public Action<XINPUT_GAMEPAD_INPUT, bool> on_reassign_key;
        public Control_Struct control;

        public Change_Input(Action<XINPUT_GAMEPAD_INPUT, bool> func) {
            on_reassign_key = func;
            reassign_key = false;
            control_name = null;
            index = 0;
            control = default;
        }

        public void to_polling_state(string name, int i, PLAYER_MODE mode, Transform t) {
            if (!reassign_key) {
                reassign_key = true;
                control_name = name;
                index = i;
                player_mode = mode;
                control = new Control_Struct(t);
            }
        }

        public void update() {
            if (reassign_key) {
                if (Input.GetMouseButton(0)) {
                    reassign_key = false;
                    on_reassign_key?.Invoke(XINPUT_GAMEPAD_INPUT.NONE, true); /* Cancel */
                    return;
                }
                if (Input.GetMouseButton(1)) {
                    reassign_key = false;
                    on_reassign_key?.Invoke(XINPUT_GAMEPAD_INPUT.NONE, false); /* Set to None */
                    return;
                }

                for (XINPUT_GAMEPAD_INPUT i = 0; i < XINPUT_GAMEPAD_INPUT.COUNT; ++i) {
                    float input = ControlManager.controller.get_input(i);                                        
                    if (input > 0.25f || input < -0.25f) {
                        reassign_key = false;
                        on_reassign_key?.Invoke(i, false); /* Set Input */
                        return;
                    }
                }
            }
        }
    }

    public class Context_Input {
        public bool reassign_key;
        public string control_name;
        public int index;
        public PLAYER_MODE player_mode;
        public Action on_reassign_key;
        public Control_Struct control;
        public XINPUT_GAMEPAD_INPUT[] context_inputs;

        public Context_Input(Action func) {
            on_reassign_key = func;
            reset();
        }

        public void to_polling_state(string name, int i, PLAYER_MODE mode, Transform t, XINPUT_GAMEPAD_INPUT[] inputs) {
            if (!reassign_key) {
                reassign_key = true;
                control_name = name;
                index = i;
                player_mode = mode;
                control = new Control_Struct(t);
                context_inputs = inputs;
            }
        }

        public void reset() {
            reassign_key = false;
            control_name = null;
            index = 0;
            player_mode = PLAYER_MODE.FOOT_MODE;
            control = default;
            context_inputs = null;
        }

        public void update() {
            if (reassign_key) {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                    on_reassign_key?.Invoke();
                }
            }
        }
    }
}
