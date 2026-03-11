using UnityEngine;

namespace TommoJProductions.MoControlsV2.MouseInput {

    public class Mouse_Emulator {
        public float sensitivity_scroll;        
        
        private float scroll_delta;
        private INPUT[] inputs = new INPUT[1];
        private INPUT input = new INPUT();

        public void update_mouse_buttons() {
            if (Control_Manager.get_input_down("MouseButton0")) {
                simulate_mouse_button(MOUSEEVENTF.LEFTDOWN);
            }
            else if (Control_Manager.get_input_up("MouseButton0")) {
                simulate_mouse_button(MOUSEEVENTF.LEFTUP);
            }

            if (Control_Manager.get_input_down("MouseButton1")) {
                simulate_mouse_button(MOUSEEVENTF.RIGHTDOWN);
            }
            else if (Control_Manager.get_input_up("MouseButton1")) {
                simulate_mouse_button(MOUSEEVENTF.RIGHTUP);
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
            input.type = INPUTTYPE.MOUSE;
            input.u.mi.dx = p.x;
            input.u.mi.dy = p.y;
            input.u.mi.mouseData = 0;
            input.u.mi.dwFlags = MOUSEEVENTF.MOVE;
            inputs[0] = input;
            User32_Imports.SendInput(1, inputs, INPUT.Size);
        }
        public void simulate_mouse_button(MOUSEEVENTF e) {
            input.type = INPUTTYPE.MOUSE;
            input.u.mi.dx = 0;
            input.u.mi.dy = 0;
            input.u.mi.mouseData = 0;
            input.u.mi.dwFlags = e;
            inputs[0] = input;
            User32_Imports.SendInput(1, inputs, INPUT.Size);
        }
        public void simulate_mouse_scroll(int direction) {
            input.type = INPUTTYPE.MOUSE;
            input.u.mi.dx = 0;
            input.u.mi.dy = 0;
            input.u.mi.mouseData = (uint)direction;
            input.u.mi.dwFlags = MOUSEEVENTF.WHEEL;
            inputs[0] = input;
            User32_Imports.SendInput(1, inputs, INPUT.Size);
        }
    }
}
