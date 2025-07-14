using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    public class MoMouseLook : FPSLook {
        public override float inputX => Input.GetAxis("Mouse X");
        public override float inputY => Input.GetAxis("Mouse Y");
    }
}
