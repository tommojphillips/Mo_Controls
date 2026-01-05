using UnityEngine;

namespace TommoJProductions.MoControlsV2 {
    public abstract class FPSLook : MonoBehaviour {
      
        public MouseLook mouse_look;
        public static float rotationY;

        public bool use_raw_input;

        public abstract float inputX { get; }
        public abstract float inputY { get; }

        public virtual void Start() {
            mouse_look = GetComponent<MouseLook>();
        }

        public virtual void Update() {
            if (!mouse_look.enabled) {
                return;
            }

            float deltaX = inputX * mouse_look.sensitivityX;
            float deltaY = inputY * mouse_look.sensitivityY;

            if (use_raw_input) {
                deltaX *= Time.deltaTime;
                deltaY *= Time.deltaTime;
            }

            if (mouse_look.axes == MouseLook.RotationAxes.MouseX) {
                mouse_look.transform.Rotate(0f, deltaX, 0f);
            }
            else {
                rotationY += deltaY;
                rotationY = Mathf.Clamp(rotationY, mouse_look.minimumY, mouse_look.maximumY);
                mouse_look.transform.localEulerAngles = new Vector3(-rotationY, mouse_look.transform.localEulerAngles.y, 0f);
            }
        }
    }
}
