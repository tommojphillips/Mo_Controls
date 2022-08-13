using UnityEngine;

namespace TommoJProductions.MoControls
{
    public abstract class FPSMove : MonoBehaviour
	{
		// Written, 31.07.2022

		private Vector3 input;

		protected CharacterMotor motor;

		public abstract float inputX { get; }
		public abstract float inputY { get; }
		public abstract bool inputJump { get; }

		public virtual void Awake()
		{
			input = new Vector3();
			motor = ControlManager.instance.player.gameObject.GetComponent<CharacterMotor>();
		}
		public virtual void Update()
		{
			input.x = inputX;
			input.z = inputY; 
			if (input != Vector3.zero)
			{
				float num = input.magnitude;
				input /= num;
				num = Mathf.Min(1, num);
				num *= num;
				input *= num;
			}
			motor.inputMoveDirection = transform.rotation * input;
			motor.inputJump = inputJump;
		}
	}
}
