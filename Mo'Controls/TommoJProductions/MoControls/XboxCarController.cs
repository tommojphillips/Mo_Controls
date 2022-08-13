using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TommoJProductions.MoControls.XInputInterpreter;

namespace TommoJProductions.MoControls
{
    public class XboxCarController : CarController
	{
		XboxController controller;

		internal float _throttle => controller.rT.state;
		internal float _brake => controller.lT.state;
		internal float _steer => controller.getLeftStick().x;
		internal float _clutch => controller.getLeftStick().x;
		internal float _handbrake => Convert.ToSingle(controller.getButtonPressed(XboxButtonEnum.Y));
		internal bool _shiftUp => controller.getButtonPressed(XboxButtonEnum.RB);
		internal bool _shiftDown => controller.getButtonPressed(XboxButtonEnum.LB);

		private void Start() 
		{
			controller = XboxControllerManager.instance.controller;
			drivetrain = GetComponent<Drivetrain>();
		}

		protected override void GetInput(out float throttleInput, out float brakeInput, out float steerInput, out float handbrakeInput, out float clutchInput, out bool startEngineInput, out int targetGear)
		{
			throttleInput = _throttle;
			brakeInput = _brake;
			steerInput = _steer;
			handbrakeInput = _handbrake;
			clutchInput = _clutch;
			startEngineInput = false;

			targetGear = drivetrain.gear;

			if (_shiftUp)
			{
				targetGear++;
			}
			if (_shiftDown)
			{
				targetGear--;
			}
		}
	}

}
