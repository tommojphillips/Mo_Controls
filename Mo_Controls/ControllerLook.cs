using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    public class Controller_Look : FPS_Look {
        public override float inputX => Control_Manager.get_axis("MouseLookX") * sensitivity;
        public override float inputY => Control_Manager.get_axis("MouseLookY") * sensitivity;

        public float sensitivity;

    }
}
