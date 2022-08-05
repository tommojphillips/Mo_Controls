using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    public class ControllerLook : MonoBehaviour
	{
		internal MouseLook mouseLook;
		internal MouseLook.RotationAxes axis;
		internal float minY = -80;
		internal float maxY = 80;

		internal float rotationY;

		private XboxController controller;

		private Vector2 input => controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.mouseInputType);

		private void Start() 
		{
			controller = MoControlsGO.controlManager.xboxController;
		}

		private void Update()
		{
			if (axis == MouseLook.RotationAxes.MouseX)
			{
				transform.Rotate(0f, input.x * mouseLook.sensitivityX, 0f);
			}
			else
			{
				rotationY += input.y * mouseLook.sensitivityY;
				rotationY = Mathf.Clamp(rotationY, minY, maxY);
				transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0f);
			}
		}
	}
}
