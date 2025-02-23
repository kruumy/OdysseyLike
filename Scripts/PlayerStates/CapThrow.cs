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
                
                Cap.ThrowDirection throwDirection = Cap.ThrowDirection.None;
                if(Godot.Input.IsActionPressed("CapThrowRight"))
                {
                    throwDirection = Cap.ThrowDirection.Right;
                }
                else if(Godot.Input.IsActionPressed("CapThrowLeft"))
                {
                    throwDirection = Cap.ThrowDirection.Left;
                }
                else if(Godot.Input.IsActionPressed("CapThrowDown"))
                {
                    throwDirection = Cap.ThrowDirection.Down;
                }
                else if(Godot.Input.IsActionPressed("CapThrowUp"))
                {
                    throwDirection = Cap.ThrowDirection.Up;
                }
                Player.Cap.Throw(throwDirection);
                Player.CoolDowns["CapThrowCooldown"]  = CapThrowCoolDown;
            }
            Player.CurrentState = Player.LastState is CapThrow ? new Idle() : Player.LastState;
        }
    }
}

