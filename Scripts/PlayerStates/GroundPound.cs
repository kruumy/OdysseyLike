using Godot;

namespace Scripts.PlayerState
{
    public class GroundPound : GroundPoundBaseState
    {
        public const float GroundPoundMultiplier = 0.5f;
        public const float GroundPoundJumpOpeningCoolDown = 0.4f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            newVelocity.Y = Player.GetGravity().Y * GroundPoundMultiplier;
        }
        protected override void PostUpdate()
        {
            if(Player.IsOnFloor())
            {
                Player.CoolDowns["GroundPoundJumpOpening"] = GroundPoundJumpOpeningCoolDown;
                Player.CurrentState = new Idle();
            }
        }
    }
}

