using Godot;

namespace Scripts.PlayerState
{
    public class CapJump : State
    {
        public readonly Jump.JumpMultiplier CapJumpVelocity = new( 1.15f );
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            newVelocity.Y = CapJumpVelocity.NewJumpVelocity;
            CoolDowns.Remove("ResetJump");
            Player.CurrentState = new Idle();
        }
    }
}

