using Godot;

namespace Scripts.PlayerState
{
    public class CapPull : State
    {
        public const float CapPullVelocity = 20f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            if(Player.Cap.IsThrown)
            {
                Vector3 directionToCap = Player.Cap.GlobalPosition - Player.GlobalPosition;
                newVelocity = directionToCap.Normalized() * Mathf.Max(newVelocity.Length(), CapPullVelocity) ;
            }
            else
            {
                Player.CurrentState = new Idle();
            }
        }
    }
}

