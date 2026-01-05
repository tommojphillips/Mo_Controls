using UnityEngine;
namespace TommoJProductions.MoControlsV2.XInput {
    
    public class XInput_Gamepad {
        
        public XInput_Gamepad_Raw_State raw_state;
        public XInput_Gamepad_State state;
        public XInput_Gamepad_State prev_state;
        public XInput_Gamepad_Deadzone deadzone;
        public readonly XINPUT_GAMEPAD_INDEX player_index;
        
        public XInput_Gamepad(XINPUT_GAMEPAD_INDEX index) {
            player_index = index;
            raw_state = default;
            state = default;
            prev_state = default;
            deadzone = default;
        }

        public XInput_Gamepad_State get_state() {
            uint result = XINPUT_IMPORTS.xinput_get_state(player_index, out raw_state);
            if (result == 0) {
                prev_state = state;
                update_state(raw_state, deadzone);
            }
            else {
                reset_state();
            }
            return state;
        }

        public void set_state(XInput_Gamepad_Vibration vibration) => XINPUT_IMPORTS.xinput_set_state(player_index, vibration);

        public void update_state(XInput_Gamepad_Raw_State raw_state, XInput_Gamepad_Deadzone deadzone) {
            state.is_connected = true;
            state.packet_number = raw_state.packet_number;
            state.buttons.start = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.START) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.back = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.BACK) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.ls = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.LS) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.rs = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.RS) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.lb = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.LB) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.rb = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.RB) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.a = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.A) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.b = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.B) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.x = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.X) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.y = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.Y) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.up = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.DPAD_UP) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.down = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.DPAD_DOWN) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.left = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.DPAD_LEFT) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.right = (raw_state.gamepad.buttons & (ushort)XINPUT_BUTTON_MASKS.DPAD_RIGHT) != 0 ? XINPUT_BUTTON_STATE.PRESSED : XINPUT_BUTTON_STATE.RELEASED;
            state.thumbsticks.left = DeadzoneUtils.apply_stick_deadzone(raw_state.gamepad.ls_x, raw_state.gamepad.ls_y, deadzone.ls);
            state.thumbsticks.right = DeadzoneUtils.apply_stick_deadzone(raw_state.gamepad.rs_x, raw_state.gamepad.rs_y, deadzone.rs);
            state.triggers.left = DeadzoneUtils.apply_trigger_deadzone(raw_state.gamepad.lt, deadzone.lt);
            state.triggers.right = DeadzoneUtils.apply_trigger_deadzone(raw_state.gamepad.rt, deadzone.rt);
        }

        public void reset_state() {
            state.is_connected = false;
            state.packet_number = 0;
            state.buttons.start = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.back = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.ls = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.rs = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.lb = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.rb = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.a = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.b = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.x = XINPUT_BUTTON_STATE.RELEASED;
            state.buttons.y = XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.up = XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.down = XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.left = XINPUT_BUTTON_STATE.RELEASED;
            state.dpad.right = XINPUT_BUTTON_STATE.RELEASED;
            state.thumbsticks.left = Vector2.zero;
            state.thumbsticks.right = Vector2.zero;
            state.triggers.left = 0;
            state.triggers.right = 0;
        }

        /* Get any input in the current frame */
        public float get_input(XINPUT_GAMEPAD_INPUT input) {
            return get_input_state(state, input);
        }

        /* Get any input in the previous frame */
        public float get_prev_input(XINPUT_GAMEPAD_INPUT input) {
            return get_input_state(prev_state, input);
        }

        /* Get any input that has been pressed in the current frame */
        public bool get_input_down(XINPUT_GAMEPAD_INPUT input) {
            if (get_prev_input(input) == 0 && get_input(input) != 0) {
                return true;
            }
            return false;
        }

        /* Get any input that has been released in the current frame */
        public bool get_input_up(XINPUT_GAMEPAD_INPUT input) {
            if (get_prev_input(input) != 0 && get_input(input) == 0) {
                return true;
            }
            return false;
        }

        /* Get any input that has been tapped in the current frame */
        public bool get_input_tap(XINPUT_GAMEPAD_INPUT input) {
            if (get_prev_input(input) != 0 && get_input(input) >= 0.1f) {
                return true;
            }
            return false;
        }

        /* Get any input that has been doubled tapped in the current frame */
        public bool get_input_double_tap(XINPUT_GAMEPAD_INPUT input) {
            if (get_prev_input(input) != 0 && get_input(input) >= 0.1f) {
                return true;
            }
            return false;
        }

        private float get_input_state(XInput_Gamepad_State state, XINPUT_GAMEPAD_INPUT input) {
            switch (input) {
                case XINPUT_GAMEPAD_INPUT.A:
                    return (int)state.buttons.a;
                case XINPUT_GAMEPAD_INPUT.B:
                    return (int)state.buttons.b;
                case XINPUT_GAMEPAD_INPUT.X:
                    return (int)state.buttons.x;
                case XINPUT_GAMEPAD_INPUT.Y:
                    return (int)state.buttons.y;
                case XINPUT_GAMEPAD_INPUT.START:
                    return (int)state.buttons.start;
                case XINPUT_GAMEPAD_INPUT.BACK:
                    return (int)state.buttons.back;
                case XINPUT_GAMEPAD_INPUT.LB:
                    return (int)state.buttons.lb;
                case XINPUT_GAMEPAD_INPUT.RB:
                    return (int)state.buttons.rb;
                case XINPUT_GAMEPAD_INPUT.LS:
                    return (int)state.buttons.ls;
                case XINPUT_GAMEPAD_INPUT.LS_LEFT:
                    if (state.thumbsticks.left.x < 0) {
                        return -state.thumbsticks.left.x;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.LS_RIGHT:
                    if (state.thumbsticks.left.x > 0) {
                        return state.thumbsticks.left.x;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.LS_UP:
                    if (state.thumbsticks.left.y > 0) {
                        return -state.thumbsticks.left.y;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.LS_DOWN:
                    if (state.thumbsticks.left.y < 0) {
                        return state.thumbsticks.left.y;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.LS_X:
                    return state.thumbsticks.left.x;
                case XINPUT_GAMEPAD_INPUT.LS_Y:
                    return state.thumbsticks.left.y;
                case XINPUT_GAMEPAD_INPUT.RS:
                    return (int)state.buttons.rs;
                case XINPUT_GAMEPAD_INPUT.RS_LEFT:
                    if (state.thumbsticks.right.x < 0) {
                        return -state.thumbsticks.right.x;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.RS_RIGHT:
                    if (state.thumbsticks.right.x > 0) {
                        return state.thumbsticks.right.x;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.RS_UP:
                    if (state.thumbsticks.right.y > 0) {
                        return -state.thumbsticks.right.y;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.RS_DOWN:
                    if (state.thumbsticks.right.y < 0) {
                        return state.thumbsticks.right.y;
                    }
                    return 0;
                case XINPUT_GAMEPAD_INPUT.RS_X:
                    return state.thumbsticks.right.x;
                case XINPUT_GAMEPAD_INPUT.RS_Y:
                    return state.thumbsticks.right.y;
                case XINPUT_GAMEPAD_INPUT.LT:
                    return state.triggers.left;
                case XINPUT_GAMEPAD_INPUT.RT:
                    return state.triggers.right;
                case XINPUT_GAMEPAD_INPUT.Trigger_Axis:
                    if (state.triggers.left > 0) {
                        return -state.triggers.left;
                    }
                    else {
                        return state.triggers.right;
                    }
                case XINPUT_GAMEPAD_INPUT.DPAD_LEFT:
                    return (int)state.dpad.left;
                case XINPUT_GAMEPAD_INPUT.DPAD_RIGHT:
                    return (int)state.dpad.right;
                case XINPUT_GAMEPAD_INPUT.DPAD_UP:
                    return (int)state.dpad.up;
                case XINPUT_GAMEPAD_INPUT.DPAD_DOWN:
                    return (int)state.dpad.down;
                case XINPUT_GAMEPAD_INPUT.DPAD_X:
                    if (state.dpad.right > 0) {
                        return (int)state.dpad.right;
                    }
                    else {
                        return -(int)state.dpad.left;
                    }
                case XINPUT_GAMEPAD_INPUT.DPAD_Y:
                    if (state.dpad.up > 0) {
                        return (int)state.dpad.up;
                    }
                    else {
                        return -(int)state.dpad.down;
                    }
                default:
                    return 0;
            }
        }
    }
}
