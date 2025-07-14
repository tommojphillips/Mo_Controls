using UnityEngine;
namespace TommoJProductions.MoControlsV2.XInput {
    
    public class XInputGamepad {
        public enum INPUT {
            NONE,
            A,
            B,
            X,
            Y,
            START,
            BACK,
            LB,
            RB,
            LT,
            RT,
            Trigger_Axis,
            LS,
            LS_X,
            LS_Y,
            LS_LEFT,
            LS_RIGHT,
            LS_UP,
            LS_DOWN,
            RS,
            RS_X,
            RS_Y,
            RS_LEFT,
            RS_RIGHT,
            RS_UP,
            RS_DOWN,
            DPAD_LEFT,
            DPAD_RIGHT,
            DPAD_UP,
            DPAD_DOWN,
            COUNT,
        }
        public Gamepad_Raw_State raw_state;
        public Gamepad_State state;
        public Gamepad_State prev_state;
        public Gamepad_Deadzone deadzone;
        public readonly GAMEPAD_INDEX player_index;
        
        public XInputGamepad(GAMEPAD_INDEX player_index) {
            this.player_index = player_index;
            raw_state = default;
            state = default;
            prev_state = default;
            deadzone = default;
        }

        public Gamepad_State get_state() {
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

        public void set_state(Gamepad_Vibration vibration) => XINPUT_IMPORTS.xinput_set_state(player_index, vibration);

        public void update_state(Gamepad_Raw_State raw_state, Gamepad_Deadzone deadzone) {
            state.is_connected = true;
            state.packet_number = raw_state.packet_number;
            state.buttons.start = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.START) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.back = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.BACK) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.ls = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.LS) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.rs = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.RS) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.lb = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.LB) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.rb = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.RB) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.a = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.A) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.b = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.B) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.x = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.X) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.buttons.y = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.Y) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.dpad.up = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.DPAD_UP) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.dpad.down = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.DPAD_DOWN) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.dpad.left = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.DPAD_LEFT) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.dpad.right = (raw_state.gamepad.buttons & (ushort)BUTTON_MASKS.DPAD_RIGHT) != 0 ? BUTTON_STATE.PRESSED : BUTTON_STATE.RELEASED;
            state.thumbsticks.left = DeadzoneUtils.apply_stick_deadzone(raw_state.gamepad.ls_x, raw_state.gamepad.ls_y, deadzone.ls);
            state.thumbsticks.right = DeadzoneUtils.apply_stick_deadzone(raw_state.gamepad.rs_x, raw_state.gamepad.rs_y, deadzone.rs);
            state.triggers.left = DeadzoneUtils.apply_trigger_deadzone(raw_state.gamepad.lt, deadzone.lt);
            state.triggers.right = DeadzoneUtils.apply_trigger_deadzone(raw_state.gamepad.rt, deadzone.rt);
        }

        public void reset_state() {
            state.is_connected = false;
            state.packet_number = 0;
            state.buttons.start = BUTTON_STATE.RELEASED;
            state.buttons.back = BUTTON_STATE.RELEASED;
            state.buttons.ls = BUTTON_STATE.RELEASED;
            state.buttons.rs = BUTTON_STATE.RELEASED;
            state.buttons.lb = BUTTON_STATE.RELEASED;
            state.buttons.rb = BUTTON_STATE.RELEASED;
            state.buttons.a = BUTTON_STATE.RELEASED;
            state.buttons.b = BUTTON_STATE.RELEASED;
            state.buttons.x = BUTTON_STATE.RELEASED;
            state.buttons.y = BUTTON_STATE.RELEASED;
            state.dpad.up = BUTTON_STATE.RELEASED;
            state.dpad.down = BUTTON_STATE.RELEASED;
            state.dpad.left = BUTTON_STATE.RELEASED;
            state.dpad.right = BUTTON_STATE.RELEASED;
            state.thumbsticks.left = Vector2.zero;
            state.thumbsticks.right = Vector2.zero;
            state.triggers.left = 0;
            state.triggers.right = 0;
        }
        
        
        public float get_input(INPUT input) {
            return get_input_state(state, input);
        }

        public float get_prev_input(INPUT input) {
            return get_input_state(prev_state, input);
        }

        public bool get_input_down(INPUT input) {

            if (get_prev_input(input) == 0 && get_input(input) != 0) {
                return true;
            }
            return false;
        }

        public bool get_input_up(INPUT input) {

            if (get_prev_input(input) != 0 && get_input(input) == 0) {
                return true;
            }
            return false;
        }

        public bool get_lt_tap() {
            if (get_prev_input(INPUT.LT) != 0 && get_input(INPUT.LT) >= 0.1f) {
                return true;
            }
            return false;
        }

        public bool get_rt_tap() {
            if (get_prev_input(INPUT.RT) != 0 && get_input(INPUT.RT) >= 0.1f) {
                return true;
            }
            return false;
        }

        private float get_input_state(Gamepad_State state, INPUT input) {

            switch (input) {

                case INPUT.A:
                    return (int)state.buttons.a;

                case INPUT.B:
                    return (int)state.buttons.b;

                case INPUT.X:
                    return (int)state.buttons.x;

                case INPUT.Y:
                    return (int)state.buttons.y;

                case INPUT.START:
                    return (int)state.buttons.start;

                case INPUT.BACK:
                    return (int)state.buttons.back;

                case INPUT.LB:
                    return (int)state.buttons.lb;

                case INPUT.RB:
                    return (int)state.buttons.rb;

                case INPUT.LS:
                    return (int)state.buttons.ls;

                case INPUT.LS_X:
                    return state.thumbsticks.left.x;

                case INPUT.LS_Y:
                    return state.thumbsticks.left.y;

                case INPUT.LS_LEFT:
                    if (state.thumbsticks.left.x < 0) {
                        return -state.thumbsticks.left.x;
                    }
                    return 0;

                case INPUT.LS_RIGHT:
                    if (state.thumbsticks.left.x > 0) {
                        return state.thumbsticks.left.x;
                    }
                    return 0;

                case INPUT.LS_UP:
                    if (state.thumbsticks.left.y < 0) {
                        return -state.thumbsticks.left.y;
                    }
                    return 0;

                case INPUT.LS_DOWN:
                    if (state.thumbsticks.left.y > 0) {
                        return state.thumbsticks.left.y;
                    }
                    return 0;

                case INPUT.RS:
                    return (int)state.buttons.rs;

                case INPUT.RS_X:
                    return state.thumbsticks.right.x;

                case INPUT.RS_Y:
                    return state.thumbsticks.right.y;

                case INPUT.RS_LEFT:
                    if (state.thumbsticks.right.x < 0) {
                        return -state.thumbsticks.right.x;
                    }
                    return 0;

                case INPUT.RS_RIGHT:
                    if (state.thumbsticks.right.x > 0) {
                        return state.thumbsticks.right.x;
                    }
                    return 0;

                case INPUT.RS_UP:
                    if (state.thumbsticks.right.y < 0) {
                        return -state.thumbsticks.right.y;
                    }
                    return 0;

                case INPUT.RS_DOWN:
                    if (state.thumbsticks.right.y > 0) {
                        return state.thumbsticks.right.y;
                    }
                    return 0;

                case INPUT.LT:
                    return state.triggers.left;

                case INPUT.RT:
                    return state.triggers.right;

                case INPUT.Trigger_Axis:
                    if (state.triggers.left > 0) {
                        return -state.triggers.left;
                    }
                    else {
                        return state.triggers.right;
                    }

                case INPUT.DPAD_LEFT:
                    return (int)state.dpad.left;

                case INPUT.DPAD_RIGHT:
                    return (int)state.dpad.right;

                case INPUT.DPAD_UP:
                    return (int)state.dpad.up;

                case INPUT.DPAD_DOWN:
                    return (int)state.dpad.down;
            }
            return 0;
        }
    }
}
