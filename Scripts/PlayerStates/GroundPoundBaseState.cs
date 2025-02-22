using Godot;

namespace Scripts.PlayerState
{
    public abstract class GroundPoundBaseState : State
    {
        public const float GroundPoundingAccelerationMultiplier = 0.01f;
        protected override void UpdateAcceleration(ref float newAcceleration, float delta)
        {
            newAcceleration *= GroundPoundingAccelerationMultiplier;
        }
        public override void Input(InputEvent @event)
        {
            base.Input(@event);
            if(@event.IsActionPressed("CapThrow") || @event.IsActionPressed("Kick"))
            {
                Player.CurrentState = new Dive();
            }
        }
    }
}

