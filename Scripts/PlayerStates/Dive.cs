using Godot;

namespace Scripts.PlayerState
{
    public class Dive : State
    {
        public const float DiveVelocity = 10f;
        public const float DivingAccelerationMultiplier = 0.1f;
        private bool HasPerformedDive = false;
        protected override void UpdateAcceleration(ref float newAcceleration, float delta)
        {
            newAcceleration *= 0.1f;
        }
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            if(HasPerformedDive)
                return;
            CoolDowns.Remove("GroundPoundFreeze");
            CoolDowns.Remove("ResetJump");
            newVelocity.Y = DiveVelocity / 2 ;
            newVelocity.X = (InputDirection != Vector3.Zero ? InputDirection : -Player.GlobalTransform.Basis.Z).X * DiveVelocity;
            newVelocity.Z = (InputDirection != Vector3.Zero ? InputDirection : -Player.GlobalTransform.Basis.Z).Z * DiveVelocity;
            if(InputDirection != Vector3.Zero)
            {
                Player.LookAt(Player.GlobalPosition + InputDirection);
            }
            HasPerformedDive = true;
        }
        protected override void PostUpdate()
        {
            if(Player.IsOnFloor())
            {
                Player.CurrentState = new Idle();
            }
        }
    }
}

