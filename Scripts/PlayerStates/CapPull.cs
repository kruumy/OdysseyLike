using Godot;

namespace Scripts.PlayerState
{
    public class CapPull : State
    {
        public const float CapPullVelocity = 20f;
        public const float MinDistanceToCap = 2f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            if(Player.Cap.IsThrown)
            {
                Player.CoolDowns.Remove("GroundPoundFreeze");
                Vector3 directionToCap = Player.Cap.GlobalPosition - Player.GlobalPosition;
                if (directionToCap.Length() > MinDistanceToCap)
                {
                    newVelocity = directionToCap.Normalized() * Mathf.Max(newVelocity.Length(), CapPullVelocity);
                }
                else
                {
                    Player.CurrentState = new Idle();
                }
            }
            else
            {
                Player.CurrentState = new Idle();
            }
        }
    }
}

