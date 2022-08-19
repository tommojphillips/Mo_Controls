using System.Linq;

using HutongGames.PlayMaker;

using MSCLoader;

using TommoJProductions.MoControls.XInputInterpreter;
using UnityEngine;

namespace TommoJProductions.MoControls
{
	public class ControllerMove : FPSMove
	{
		// Written, 31.07.2022

		private XboxController controller;

		private Vector2 _moveInput;

        public override float inputX => _moveInput.x;

        public override float inputY => _moveInput.y;

		public override bool inputJump => cInput.GetButtonDown("Jump");

		public void Start()
		{			
			controller = XboxControllerManager.instance.controller;
		}
		public override void Update()
		{
			if (controller.isConnected && MoControlsSaveData.loadedSaveData.usePlayerMoveAsInput)
			{
				_moveInput = controller.getInputFromTypeRaw(MoControlsSaveData.loadedSaveData.playerMove);
				if (!MoControlsSaveData.loadedSaveData.playerMoveUseRawInput)
					_moveInput.doDeadzoneCheck(MoControlsSaveData.loadedSaveData.playerMoveDeadzoneType, MoControlsSaveData.loadedSaveData.playerMoveDeadzone);

				base.Update();				
			}
		}
	}
}
