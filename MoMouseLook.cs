using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    public class Mo_Mouse_Look : FPS_Look {
        public override float inputX => Input.GetAxis("Mouse X");
        public override float inputY => Input.GetAxis("Mouse Y");
    }
}
