using Godot;

namespace Scripts.PlayerState
{
    public class Jump : State
    {
        public const float CoyoteJumpOpeningCoolDown = 0.2f;
        public readonly struct JumpMultiplier
        {
            public const float BaseJumpVelocity = 12.0f;
            public readonly float VelocityMultiplier;
            public float NewJumpVelocity => BaseJumpVelocity * VelocityMultiplier;
            public JumpMultiplier(float VelocityMultiplier) => this.VelocityMultiplier = VelocityMultiplier;
        }
        public static JumpMultiplier LastJump;
        public const float MultiJumpResetCooldown = 2f;
        public static Vector3 LastSecondJumpDirection;
        public static readonly JumpMultiplier FirstJump = new(1f);
        public static readonly JumpMultiplier SecondJump = new(1.1f);
        public static readonly JumpMultiplier ThirdJump = new(1.5f);
        public static readonly JumpMultiplier GroundPoundJump = new(1.4f);
        protected override void UpdateVelocity(ref Vector3 newVelocity, float delta)
        {
            CoolDowns.Remove("CoyoteJumpOpening");
            JumpMultiplier NextJump = FirstJump;
            if(CoolDowns.ContainsKey("GroundPoundJumpOpening"))
            {
                NextJump = GroundPoundJump;
            }
            else if(CoolDowns.ContainsKey("ResetJump"))
            {
                if( LastJump.Equals(FirstJump) )
                {
                    NextJump = SecondJump;
                     LastSecondJumpDirection = InputDirection; 
                }
                else if( LastJump.Equals(SecondJump) && HorizontalVelocity.Length() >= MaxGroundSpeed * 0.8f && LastSecondJumpDirection != Vector3.Zero )
                {
                    // only triple jump if the angle changed since second jump is less and 90 degrees
                    float angle = Mathf.Acos(LastSecondJumpDirection.Dot(InputDirection));
                    if (angle <= Mathf.Pi / 2)
                    {
                            NextJump = ThirdJump;
                    }
                }
            }
            LastJump = NextJump;
            if(InputDirection != Vector3.Zero)
            {
                Player.LookAt(Player.GlobalPosition + InputDirection);
            }
            newVelocity.Y = NextJump.NewJumpVelocity;
            CoolDowns["ResetJump"] = MultiJumpResetCooldown;
            Player.CurrentState = new Idle();
        }
    }
}

