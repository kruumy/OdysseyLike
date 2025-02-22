using Godot;

namespace Scripts.PlayerState
{
    public class WallKick : State
    {
        public const float WallKickYVelocity = 10f;
        public readonly float WallKickSpeed = WallKickYVelocity;
        public WallKick(float WallKickSpeed)
        {
            this.WallKickSpeed = WallKickSpeed;
        }
        
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            newVelocity.Y = WallKickYVelocity;
            newVelocity.X = -Player.GlobalTransform.Basis.Z.X * WallKickSpeed;
            newVelocity.Z = -Player.GlobalTransform.Basis.Z.Z * WallKickSpeed;
            Player.CoolDowns.Remove("ResetJump");
            Player.CurrentState = new Idle();
        }
    }
}
