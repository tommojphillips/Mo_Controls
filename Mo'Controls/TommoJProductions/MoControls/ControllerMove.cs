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
			controller = XboxControllerManager.instance.controller;
		}
		public override void Update()
		{
			if (controller.isConnected && MoControlsSaveData.loadedSaveData.usePlayerMoveAsInput)
			{
				input = controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.playerMove);
				if (!MoControlsSaveData.loadedSaveData.playerMoveUseRawInput)
					input.doDeadzoneCheck(MoControlsSaveData.loadedSaveData.playerMoveDeadzoneType, MoControlsSaveData.loadedSaveData.playerMoveDeadzone);

				input = controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.playerMove);
				base.Update();				
			}
		}
	}
}
