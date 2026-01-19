using UnityEngine;
using Harmony;

namespace TommoJProductions.MoControlsV2 {
    public class Camera_Manager {

        public Controller_Look controller_look_x;
        public Controller_Look controller_look_y;
        public Mo_Mouse_Look mouse_look_x;
        public Mo_Mouse_Look mouse_look_y;
        private static HarmonyInstance harmony;

        public void load() {
            patch_mouse_look();

            GameObject player = GameObject.Find("PLAYER");
            controller_look_x = player.AddComponent<Controller_Look>();
            controller_look_x.use_raw_input = true;
            mouse_look_x = player.AddComponent<Mo_Mouse_Look>();
            mouse_look_x.use_raw_input = false;

            GameObject pov = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera");
            controller_look_y = pov.AddComponent<Controller_Look>();
            controller_look_y.use_raw_input = true;
            mouse_look_y = pov.AddComponent<Mo_Mouse_Look>();
            mouse_look_y.use_raw_input = false;
        }
        public void unload() {
            unpatch_mouse_look();

            if (controller_look_x != null) {
                Object.DestroyImmediate(controller_look_x);
                controller_look_x = null;
            }
            if (controller_look_y != null) {
                Object.DestroyImmediate(controller_look_y);
                controller_look_y = null;
            }
            
            if (mouse_look_x != null) {
                Object.DestroyImmediate(mouse_look_x);
                mouse_look_x = null;
            }
            if (mouse_look_y != null) {
                Object.DestroyImmediate(mouse_look_y);
                mouse_look_y = null;
            }
        }

        private void patch_mouse_look() {
            /* Patch out MouseLook::Update() */
            if (harmony == null) {
                MoControlsV2Mod.log("Patching MouseLook");
                harmony = HarmonyInstance.Create("mo_controls.camera");
                harmony.Patch(AccessTools.Method(typeof(MouseLook), "Update"), new HarmonyMethod(typeof(Camera_Manager), "MouseLook_Update"));
            }
        }
        private void unpatch_mouse_look() {
            if (harmony != null) {
                MoControlsV2Mod.log("Unpatching MouseLook");
                harmony.PatchAll();
                harmony = null;
            }
        }

        /* Hooks */
        public static bool MouseLook_Update() {
            return false; /* skip orignal method */
        }
    }
}
