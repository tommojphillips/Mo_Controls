using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls
{
	public class ControllerMove : FPSMove
	{
		// Written, 31.07.2022

		private XboxController controller;

		private Vector2 input;

        public override float inputX => input.x;

        public override float inputY => input.y;

		public override bool inputJump => controller.getButtonDown(MoControlsSaveData.loadedSaveData.playerJump);

        public void Start()
		{
			controller = MoControlsGO.controlManager.xboxController;
		}
		public override void Update()
		{
			if (controller.isConnected)
			{
				input = controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.playerMove);
				base.Update();				
			}
		}
	}
}
