using UnityEngine;

namespace TommoJProductions.MoControlsV2.MouseInput {

    public class Mouse_Emulator {
        private float scroll_delta;
        public float sensitivity_scroll;        

        public void update_mouse_buttons() {
            if (Control_Manager.get_input_down("MouseButton0")) {
                simulate_mouse_button(MOUSE_EVENT.LEFTDOWN);
            }
            else if (Control_Manager.get_input_up("MouseButton0")) {
                simulate_mouse_button(MOUSE_EVENT.LEFTUP);
            }

            if (Control_Manager.get_input_down("MouseButton1")) {
                simulate_mouse_button(MOUSE_EVENT.RIGHTDOWN);
            }
            else if (Control_Manager.get_input_up("MouseButton1")) {
                simulate_mouse_button(MOUSE_EVENT.RIGHTUP);
            }
        }

        public void update_mouse_scroll() {
            scroll_delta -= Control_Manager.get_axis("MouseScroll-") * sensitivity_scroll * Time.deltaTime;
            scroll_delta += Control_Manager.get_axis("MouseScroll+") * sensitivity_scroll * Time.deltaTime;            
            scroll_delta += Control_Manager.get_axis("MouseScroll") * sensitivity_scroll * Time.deltaTime;
           
            if (scroll_delta < -1) {
                scroll_delta += 1;
                simulate_mouse_scroll(-1);
            }
            else if (scroll_delta > 1) {
                scroll_delta -= 1;
                simulate_mouse_scroll(1);
            }
        }

        public void simulate_mouse_move(Point p) {
            send(p, (uint)MOUSE_EVENT_BUTTON.NULL, (uint)MOUSE_EVENT.MOVE);
        }
        public void simulate_mouse_button(MOUSE_EVENT e) {
            User32_Imports.GetCursorPos(out Point p);
            send(p, (uint)MOUSE_EVENT_BUTTON.XBUTTON1, (uint)e);
        }
        public void simulate_mouse_scroll(int direction) {
            User32_Imports.GetCursorPos(out Point p);
            send(p, (uint)direction, (uint)MOUSE_EVENT.WHEEL);
        }

        private void send(Point p, uint data, uint flags) {
            Input input = new Input() {
                type = 0,
                mi = new Mouse_Input() {
                    x = p.x,
                    y = p.y,
                    data = data,
                    flags = flags,
                }
            };

            User32_Imports.SendInput(1, input, Input.size);
        }
    }
}
