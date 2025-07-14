using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    public abstract class FPSLook : MonoBehaviour {
      
        public MouseLook mouse_look;
        public static float rotationY;

        public abstract float inputX { get; }
        public abstract float inputY { get; }

        public virtual void Start() {
            this.mouse_look = GetComponent<MouseLook>();
        }

        public virtual void Update() {
            if (!mouse_look.enabled) {
                return;
            }

            if (mouse_look.axes == MouseLook.RotationAxes.MouseX) {
                mouse_look.transform.Rotate(0f, inputX * mouse_look.sensitivityX, 0f);
            }
            else {
                rotationY += inputY * mouse_look.sensitivityY;
                rotationY = Mathf.Clamp(rotationY, mouse_look.minimumY, mouse_look.maximumY);
                mouse_look.transform.localEulerAngles = new Vector3(-rotationY, mouse_look.transform.localEulerAngles.y, 0f);
            }
        }
    }
}
