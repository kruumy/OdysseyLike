using System;
using Godot;

namespace Scripts.PlayerState
{
    public class WallGrab : State
    {
        private float SavedSpeed = 0f;
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            Player.CoolDowns.Remove("GroundPoundFreeze");
            if(SavedSpeed == 0f)
            {
                SavedSpeed = Math.Max(WallKick.WallKickYVelocity,HorizontalVelocity.Length()); 
            }
            newVelocity.X = 0;
            newVelocity.Z = 0;
            newVelocity.Y = -1;
            Player.LookAt(Player.GlobalPosition + Player.GetWallNormal() + (InputDirection != Vector3.Zero ? InputDirection : Vector3.One) * new Vector3(0.8f,0f,0.8f));
            if(Player.CoolDowns.ContainsKey("CoyoteJumpOpening"))
            {
                Player.CurrentState = new WallKick(SavedSpeed);
            }
        }
    }
}
