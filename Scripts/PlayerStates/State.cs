using Godot;
using System;
using System.Collections.Generic;

namespace Scripts.PlayerState
{
    public abstract class State
    {
        public PlayerController Player;
        public Vector3 InputDirection = Vector3.Zero;

        public const float GroundAcceleration = 50f;
        public const float AirAcceleration = 25f;
        public const float MaxGroundSpeed = 8f;
        public const float MinAirMaxSpeed = 10f;
        public float MaxSpeed { get; private set; }
        public Vector3 HorizontalVelocity => new Vector3(Player.Velocity.X,0f,Player.Velocity.Z);
        public void Update(float delta)
        {
            if(
                Player.IsOnWallOnly() && 
                (Player.CurrentState is Idle || Player.CurrentState is Dive))
            {
                Player.CurrentState = new WallGrab();
            }

            Vector3 newVelocity = Player.Velocity;
            Vector2 inputDir = Godot.Input.GetVector("Left", "Right", "Forward", "Backward");

            Vector3 forward = -Player.Camera.GlobalTransform.Basis.Z;
            forward.Y = 0;
            Vector3 right = Player.Camera.GlobalTransform.Basis.X;
            right.Y = 0;

            InputDirection = (right.Normalized() * inputDir.X) - (forward.Normalized() * inputDir.Y); // input direction according to camera forward's horizontal components
            InputDirection = InputDirection.Normalized();

            MaxSpeed = Player.IsOnFloor() ? MaxGroundSpeed : Math.Max(MinAirMaxSpeed,HorizontalVelocity.Length());
            float realAcceleration = Player.IsOnFloor() ? GroundAcceleration : AirAcceleration;
            realAcceleration = realAcceleration * MaxGroundSpeed / Mathf.Max(MaxGroundSpeed,MaxSpeed * 0.5f); // limit acceleration depending on speed
            UpdateAcceleration(ref realAcceleration, delta);
            Vector3 targetVelocity = InputDirection * MaxSpeed * inputDir.Length();
            newVelocity.X = Mathf.MoveToward(newVelocity.X, targetVelocity.X, realAcceleration * delta);
            newVelocity.Y += Player.GetGravity().Y * delta;
            newVelocity.Z = Mathf.MoveToward(newVelocity.Z, targetVelocity.Z, realAcceleration * delta);
            UpdateVelocity(ref newVelocity, delta);

            Transform3D newTransform = Player.GlobalTransform.LookingAt( Player.GlobalPosition + -Player.GlobalTransform.Basis.Z + (Player.IsOnFloor()  ? HorizontalVelocity.Normalized() : InputDirection));
            Player.GlobalTransform = Player.IsOnFloor() ? newTransform : Player.GlobalTransform.InterpolateWith(newTransform, (float)delta * realAcceleration );

            Player.Velocity = newVelocity;
            Player.MoveAndSlide();
            PostUpdate();
        }
        protected virtual void UpdateAcceleration(ref float newAcceleration, float delta){}
        protected virtual void UpdateVelocity(ref Vector3 newVelocity, float delta){}
        protected virtual void PostUpdate(){}
        public virtual void Input(InputEvent @event)
        {
            if(@event.IsActionPressed("Jump"))
            {
                Player.CoolDowns["CoyoteJumpOpening"] = Jump.CoyoteJumpOpeningCoolDown;
            }
            else if(
                @event.IsActionPressed("CapThrow") && 
                (Player.CurrentState is Idle || Player.CurrentState is CapPull))
            {
                Player.CurrentState = new CapThrow();
            }
            else if(
                @event.IsActionPressed("CapPull") &&
                (Player.CurrentState is Idle || Player.CurrentState is GroundPoundFreeze))
            {
                Player.CurrentState = new CapPull();
            }
            else if(
                @event.IsActionPressed("GroundPound") && 
                @event.GetActionStrength("GroundPound") >= 1f &&
                Player.CurrentState is Idle && 
                !Player.IsOnFloor())
            {
                Player.CurrentState = new GroundPoundFreeze();
            }
        }
    }
}

