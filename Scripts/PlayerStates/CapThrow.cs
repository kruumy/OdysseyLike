using Godot;

namespace Scripts.PlayerState
{
    public class CapThrow : State
    {
        public const float CapThrowCoolDown = 0.5f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            if(!Player.CoolDowns.ContainsKey("CapThrowCooldown"))
            {
                if(!Player.IsOnFloor())
                {
                    newVelocity.X *= 0.2f;
                    newVelocity.Y = 6f;
                    newVelocity.Z *= 0.2f;
                }
                Player.Cap.Throw();
                Player.CoolDowns["CapThrowCooldown"]  = CapThrowCoolDown;
            }

            Player.CurrentState = Player.LastState;
        }
    }
}

