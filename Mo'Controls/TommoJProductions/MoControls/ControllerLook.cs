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

		private Vector2 _input;

		private Vector2 getInput() 
		{
				if (MoControlsSaveData.loadedSaveData.playerLookUseRawInput)
					return controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.playerLook);
				else
					return controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.playerLook)
						.doDeadzoneCheck(MoControlsSaveData.loadedSaveData.playerLookDeadzoneType, MoControlsSaveData.loadedSaveData.playerLookDeadzone)
						.doSensitivityOperation(MoControlsSaveData.loadedSaveData.playerLookSensitivityMultiplier);			
		}

		private void Start() 
		{
			controller = XboxControllerManager.instance.controller;
		}

		private void Update()
		{
			_input = getInput();

			if (axis == MouseLook.RotationAxes.MouseX)
			{
				transform.Rotate(0f, _input.x * mouseLook.sensitivityX, 0f);
			}
			else
			{
				rotationY += _input.y * mouseLook.sensitivityY;
				rotationY = Mathf.Clamp(rotationY, minY, maxY);
				transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0f);
			}
		}
	}
}
