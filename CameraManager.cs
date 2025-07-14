using UnityEngine;
using Harmony;

namespace TommoJProductions.MoControlsV2 {
    public class CameraManager {

        public ControllerLook controller_look_x;
        public ControllerLook controller_look_y;
        public MoMouseLook mouseLookX;
        public MoMouseLook mouseLookY;

        public void load() {
            hook_mouse_look();

            GameObject player = GameObject.Find("PLAYER");
            controller_look_x = player.AddComponent<ControllerLook>();
            mouseLookX = player.AddComponent<MoMouseLook>();

            GameObject pov = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera");
            controller_look_y = pov.AddComponent<ControllerLook>();
            mouseLookY = pov.AddComponent<MoMouseLook>();
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
