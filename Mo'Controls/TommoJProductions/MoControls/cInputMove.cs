using UnityEngine;

namespace TommoJProductions.MoControls
{
    internal class cInputMove : FPSMove
    {
        // Written, 31.07.2022

        public override float inputX => cInput.GetAxisRaw("PlayerHorizontal"); 
        public override float inputY => cInput.GetAxisRaw("PlayerVertical");
        public override bool inputJump => cInput.GetButtonDown("Jump");
    }
}
