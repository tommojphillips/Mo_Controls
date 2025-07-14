using UnityEngine;

namespace TommoJProductions.MoControlsV2.MouseInput {

    public class MouseEmulator {

        private PointF cursor_posF;
        private Point cursor_pos;
        private Input input;
        public float sensitivity_x;
        public float sensitivity_y;
        public float sensitivity_scroll;
        public bool m_mouse0_down;
        public bool m_mouse1_down;
        public bool mouse0_down => m_mouse0_down;
        public bool mouse1_down => m_mouse1_down;

        public MouseEmulator() {
            cursor_posF = default;
            cursor_pos = default;
            m_mouse0_down = false;
            m_mouse1_down = false;
        }

        public void update_mouse_buttons() {
            if (ControlManager.get_input_down("MouseButton0")) {
                simulate_mouse0_click(MOUSE_EVENT_CLICK_BUTTON0.DOWN);
            }
            else if (ControlManager.get_input_up("MouseButton0")) {
                simulate_mouse0_click(MOUSE_EVENT_CLICK_BUTTON0.UP);
            }

            if (ControlManager.get_input_down("MouseButton1")) {
                simulate_mouse1_click(MOUSE_EVENT_CLICK_BUTTON1.DOWN);
            }
            else if (ControlManager.get_input_up("MouseButton1")) {
                simulate_mouse1_click(MOUSE_EVENT_CLICK_BUTTON1.UP);
            }
        }

        private float scroll_delta;
        public void update_mouse_scroll() {
            scroll_delta -= ControlManager.get_axis("MouseScroll-") * (sensitivity_scroll / 100);
            scroll_delta += ControlManager.get_axis("MouseScroll+") * (sensitivity_scroll / 100);
           
            if (scroll_delta < -1) {
                scroll_delta += 1;
                simulate_scroll(-1);
            }
            else if (scroll_delta > 1) {
                scroll_delta -= 1;
                simulate_scroll(1);
            }
        }

        public void update_mouse_postion() {
            cursor_posF.x = ControlManager.get_axis("MouseMoveX") * sensitivity_x;
            cursor_posF.y = ControlManager.get_axis("MouseMoveY") * sensitivity_y;
            if (cursor_posF.sqr_mag > 0) {
                cursor_pos.x = (int)cursor_posF.x;
                cursor_pos.y = (int)cursor_posF.y;
                simulate_mouse_move(cursor_pos);
            }
        }

        public void simulate_mouse_move(Point p) {
            send(p, (uint)MOUSE_EVENT_BUTTON.NULL, (uint)MOUSE_EVENT.MOVE);
        }
        public void simulate_mouse0_click(MOUSE_EVENT_CLICK_BUTTON0 click) {
            User32Imports.GetCursorPos(out cursor_pos);
            send(cursor_pos, (uint)MOUSE_EVENT_BUTTON.XBUTTON1, (uint)click);
            m_mouse0_down = click == MOUSE_EVENT_CLICK_BUTTON0.DOWN;
        }
        public void simulate_mouse1_click(MOUSE_EVENT_CLICK_BUTTON1 click) {
            User32Imports.GetCursorPos(out cursor_pos);
            send(cursor_pos, (uint)MOUSE_EVENT_BUTTON.XBUTTON1, (uint)click);
            m_mouse1_down = click == MOUSE_EVENT_CLICK_BUTTON1.DOWN;
        }
        public void simulate_scroll(int scroll) {
            User32Imports.GetCursorPos(out cursor_pos);
            send(cursor_pos, (uint)scroll, (uint)MOUSE_EVENT.WHEEL);
        }

        private void update_input_state(int x, int y, uint data, uint flags) {
            input.type = 0;
            input.mi.x = x;
            input.mi.y = y;
            input.mi.data = data;
            input.mi.flags = flags;
        }

        private void send(Point p, uint data, uint flags) {
            update_input_state(p.x, p.y, data, flags);      
            User32Imports.SendInput(1, input, Input.Size);
        }
    }
}
