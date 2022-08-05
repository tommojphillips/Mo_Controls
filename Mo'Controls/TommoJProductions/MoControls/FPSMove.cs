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
			motor = Camera.main.transform.root.gameObject.GetComponent<CharacterMotor>();
		}
		public virtual void Update()
		{
			input.x = inputX;
			input.z = inputY;
			motor.inputMoveDirection = transform.rotation * input;
			motor.inputJump = inputJump;
		}
	}
}
