using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    public class ControllerLook : FPSLook {
        public override float inputX => ControlManager.get_axis("MouseLookX") * sensitivity;
        public override float inputY => ControlManager.get_axis("MouseLookY") * sensitivity;

        public float sensitivity;

    }
}
