using UnityEngine;
using Harmony;

namespace TommoJProductions.MoControlsV2 {
    public class CameraManager {

        public ControllerLook controller_look_x;
        public ControllerLook controller_look_y;
        public MoMouseLook mouse_look_x;
        public MoMouseLook mouse_look_y;

        public void load() {
            hook_mouse_look();

            GameObject player = GameObject.Find("PLAYER");
            controller_look_x = player.AddComponent<ControllerLook>();
            controller_look_x.use_raw_input = true;
            mouse_look_x = player.AddComponent<MoMouseLook>();
            mouse_look_x.use_raw_input = false;

            GameObject pov = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera");
            controller_look_y = pov.AddComponent<ControllerLook>();
            controller_look_y.use_raw_input = true;
            mouse_look_y = pov.AddComponent<MoMouseLook>();
            mouse_look_y.use_raw_input = false;
        }

        private void hook_mouse_look() {
            /* Patch out MouseLook::Update() */
            MoControlsV2Mod.log("Patching MouseLook");
            HarmonyInstance harmony = HarmonyInstance.Create("mo_controls.camera");
            harmony.Patch(AccessTools.Method(typeof(MouseLook), "Update"), new HarmonyMethod(typeof(CameraManager), "MouseLook_Update"));
        }

        /* Hooks */
        public static bool MouseLook_Update() {
            return false; /* skip orignal method */
        }
    }
}
