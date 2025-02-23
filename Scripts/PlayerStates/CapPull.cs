using Godot;

namespace Scripts.PlayerState
{
    public class CapPull : State
    {
        public const float CapPullVelocity = 20f;
        public const float MinDistanceToCap = 2f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            if(Player.Cap.IsThrown && Player.Cap.CanCapPull)
            {
                Player.CoolDowns.Remove("GroundPoundFreeze");
                if(Player.IsOnFloor())
                    Player.CoolDowns.Remove("CapThrowCooldown");
                Vector3 directionToCap = Player.Cap.GlobalPosition - Player.GlobalPosition;
                if (directionToCap.Length() > MinDistanceToCap)
                {
                    newVelocity = directionToCap.Normalized() * Mathf.Max(newVelocity.Length(), CapPullVelocity);
                }
            }
            else
            {
                if(!Player.IsOnFloor())
                    Player.Cap.CanCapPull = false; 
                
                if(Godot.Input.IsActionPressed("CapThrow") && Player.IsOnFloor())
                    Player.CurrentState = new CapThrow();
                else
                    Player.CurrentState = new Idle();
            }
        }
    }
}

