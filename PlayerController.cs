using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerController : CharacterBody3D
{
    [Export] public Node3D Camera;
	[Export] public Cap Cap;
    public bool Active = true;
    public const float MaxGroundVelocity = 7.0f;
    public const float Acceleration = 50.0f;
    public readonly struct JumpMultiplier
    {
        public const float BaseJumpVelocity = 12.0f;
        public readonly float VelocityMultiplier;
        public float NewJumpVelocity => BaseJumpVelocity * VelocityMultiplier;
        public JumpMultiplier(float VelocityMultiplier) => this.VelocityMultiplier = VelocityMultiplier;
    }
    
    public JumpMultiplier FirstJump = new(1f);
    public JumpMultiplier SecondJump = new(1.1f);
    public JumpMultiplier ThirdJump = new(1.5f);
    public JumpMultiplier GroundpoundJump = new(1.4f);
    public JumpMultiplier CapJump = new(1.15f);
    public JumpMultiplier LastJump;
    public const float WallKickMinVelocity = 10f;
    public const float DiveVelocity = 10f;
    public const float MultiJumpResetCooldown = 2f;
	public const float AirAccelerationMultiplier = 0.5f;
	public Vector3 InputDirection = Vector3.Zero;
	public bool IsInputMoving => InputDirection != Vector3.Zero;
    public bool CapJumpNextFrame = false;
    public bool IsDiving = false;
    public const float DivingAccelerationMultiplier = 0.1f;
    public const float GroundPoundingAccelerationMultiplier = 0.01f;
    public Dictionary<string,float> CoolDowns = new();
    public HashSet<string> CoolDownsRemovedThisFrame = new();
    public const float CapThrowCoolDown = 0.5f;
    public const float GroundPoundJumpOpeningCoolDown = 0.5f;
    public const float CoyoteJumpOpeningCoolDown = 0.2f;
    public bool IsGroundPounding = false;
    public float GroundPoundFreezeLength = 0.5f;
    public bool IsBonked = false;
    public bool IsCapPulling = false;
    public float ReturnToRunningDeceleration = 4f;
    public float VelocityLeavingGround = MaxGroundVelocity;
    private Vector3 LastSecondJumpDirection = Vector3.Zero; 
    public override void _Process(double delta)
    {
        CoolDownsRemovedThisFrame.Clear();
        foreach (var key in CoolDowns.Keys)
        {
            CoolDowns[key] -= (float)delta;
            if (CoolDowns[key] <= 0)
            {
                CoolDowns.Remove(key);
                CoolDownsRemovedThisFrame.Add(key);
            }
                
        }
		Vector3 newVelocity = Velocity;
        Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
        InputDirection = Camera.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

		InputDirection.Y = 0; 
		InputDirection = InputDirection.Normalized();

        float realAcceleration = Acceleration;
        if(IsDiving)
        {
            realAcceleration *= DivingAccelerationMultiplier;
        }
        else if(IsGroundPounding)
        {
            realAcceleration *= GroundPoundingAccelerationMultiplier;
        }
        else if(!IsOnFloor())
        {
            realAcceleration *= AirAccelerationMultiplier;
        }

        Vector3 targetVelocity = InputDirection * (IsOnFloor() ? MaxGroundVelocity : VelocityLeavingGround) * inputDir.Length();
        newVelocity.X = Mathf.MoveToward(newVelocity.X, targetVelocity.X, realAcceleration * (float)delta);
        newVelocity.Y += GetGravity().Y * (float)delta;
        newVelocity.Z = Mathf.MoveToward(newVelocity.Z, targetVelocity.Z, realAcceleration * (float)delta);
		
        if (newVelocity.Length() > 0f && !CoolDowns.ContainsKey("GroundPoundFreeze"))
        {
            Vector3 lessYInfluenceVelocity = IsOnFloor()  ? newVelocity.Normalized() : InputDirection;
            lessYInfluenceVelocity.Y = 0; 
            GlobalTransform = GlobalTransform.InterpolateWith(GlobalTransform.LookingAt( GlobalPosition + -GlobalTransform.Basis.Z + lessYInfluenceVelocity ), (float)delta * realAcceleration );
        }

        if(CoolDownsRemovedThisFrame.Contains("GroundPoundFreeze") && !IsDiving)
        {
            newVelocity.Y = GetGravity().Y * GroundPoundFreezeLength;
            IsGroundPounding = true;
        }
        else if(CoolDowns.ContainsKey("GroundPoundFreeze"))
        {
            newVelocity.X = Mathf.MoveToward(newVelocity.X, 0f, GroundPoundFreezeLength * 100 * (float)delta);
            newVelocity.Y = Mathf.MoveToward(newVelocity.Y, 0f, GroundPoundFreezeLength * 100 * (float)delta * 10);
            newVelocity.Z = Mathf.MoveToward(newVelocity.Z, 0f, GroundPoundFreezeLength * 100 * (float)delta);
        }

        if(Input.IsActionJustPressed("Jump"))
        {
            CoolDowns["CoyoteJumpOpening"] = CoyoteJumpOpeningCoolDown;
        }
        
        if (Input.IsActionJustPressed("CapPull") && Cap.IsThrown)
        {
            IsCapPulling = true;
            IsDiving = false;
            CoolDowns.Remove("GroundPoundFreeze");
            IsGroundPounding = false;
        }
        if(IsCapPulling)
        {
            if(Cap.IsThrown)
            {
                Vector3 directionToCap = Cap.GlobalPosition - GlobalPosition;
                newVelocity = directionToCap.Normalized() * Mathf.Max(newVelocity.Length(), DiveVelocity * 2f) ;
            }
            else
            {
                IsCapPulling = false;
            }
        }
        if(IsOnFloor() || CapJumpNextFrame )
        {
            IsDiving = false;
            if (!CoolDowns.ContainsKey("GroundPoundJumpOpening") && IsGroundPounding)
            {
                CoolDowns["GroundPoundJumpOpening"] = GroundPoundJumpOpeningCoolDown;
            }
            IsGroundPounding = false;      
        }
        
        
        if (CapJumpNextFrame || (CoolDowns.ContainsKey("CoyoteJumpOpening") && IsOnFloor() && !CoolDowns.ContainsKey("GroundPoundFreeze")))
        {
            Input.ActionRelease("GroundPound");
            JumpMultiplier NextJump = FirstJump;
            CoolDowns["CoyoteJumpOpening"] = 0f;

            if(CapJumpNextFrame)
            {
                NextJump = CapJump;
            }
            else if( CoolDowns.ContainsKey("GroundPoundJumpOpening") )
            {
                NextJump = GroundpoundJump;
                CoolDowns["GroundPoundJumpOpening"] = 0f;
            }
            else if(CoolDowns.ContainsKey("ResetJump"))
            {
                if( LastJump.Equals(FirstJump) )
                {
                    NextJump = SecondJump;
                    LastSecondJumpDirection = InputDirection; 
                }
                else if( LastJump.Equals(SecondJump) && Velocity.Length() >= MaxGroundVelocity * 0.8f )
                {
                    if (LastSecondJumpDirection != Vector3.Zero)
                    {
                        // only triple jump if the angle changed since second jump is less and 90 degrees
                        float angle = Mathf.Acos(LastSecondJumpDirection.Dot(InputDirection));
                        if (angle <= Mathf.Pi / 2)
                        {
                            NextJump = ThirdJump;
                        }
                    }
                }
            }
            LastJump = NextJump;
            if(InputDirection != Vector3.Zero)
            {
                LookAt(GlobalPosition + InputDirection);
            }
            newVelocity.Y = NextJump.NewJumpVelocity;
            CoolDowns["ResetJump"] = MultiJumpResetCooldown;
            var currentHorizontalVelocity = new Vector3(newVelocity.X,0,newVelocity.Z);
            CapJumpNextFrame = false;
            VelocityLeavingGround = Math.Max(currentHorizontalVelocity.Length(), MaxGroundVelocity);
        }
        else if(Input.IsActionPressed("GroundPound") && Input.GetActionStrength("GroundPound") >= 1f && !IsDiving && !IsOnFloor() && !IsOnWallOnly() )
        {
            if (!CoolDowns.ContainsKey("GroundPoundFreeze") && !IsGroundPounding)
            {
                CoolDowns["GroundPoundFreeze"] = GroundPoundFreezeLength;
            }
        }
        
        if (Input.IsActionJustPressed("CapThrow") && !IsDiving && !IsOnWallOnly() )
        {
            if((CoolDowns.ContainsKey("GroundPoundFreeze") || IsGroundPounding) && !IsOnFloor())
            {
                IsGroundPounding = false;
                if(CoolDowns.ContainsKey("GroundPoundFreeze"))
                {
                    CoolDowns["GroundPoundFreeze"] = 0f;
                }
                newVelocity.Y = DiveVelocity / 2 ;
                newVelocity.X = (IsInputMoving ? InputDirection : -GlobalTransform.Basis.Z).X * DiveVelocity;
                newVelocity.Z = (IsInputMoving ? InputDirection : -GlobalTransform.Basis.Z).Z * DiveVelocity;
                if (IsInputMoving)
                    LookAt(GlobalPosition + InputDirection);
                VelocityLeavingGround = DiveVelocity;
                IsDiving = true;
            }
            else if(!CoolDowns.ContainsKey("Stalling"))
            {
                if( !IsOnFloor() )
                    AirStall(ref newVelocity);
                CoolDowns["Stalling"] = CapThrowCoolDown;
                Cap.Throw();
                VelocityLeavingGround = DiveVelocity;
            }
        }

        if(IsOnWallOnly() && !IsDiving && newVelocity.Y <= 5)
        {
            newVelocity.X = 0;
            newVelocity.Z = 0;
            newVelocity.Y = -1;

            LookAt(GlobalPosition + GetWallNormal() + InputDirection * new Vector3(0.8f,0f,0.8f));
            
            if(CoolDowns.ContainsKey("CoyoteJumpOpening"))
            {
                float wallKickSpeed = Math.Max(WallKickMinVelocity, VelocityLeavingGround);
                newVelocity.Y = wallKickSpeed;
                newVelocity.X = -GlobalTransform.Basis.Z.X * wallKickSpeed;
                newVelocity.Z = -GlobalTransform.Basis.Z.Z * wallKickSpeed;
                VelocityLeavingGround = wallKickSpeed;
                CoolDowns["ResetJump"] = 0f;
                IsGroundPounding = false;
                if( CoolDowns.ContainsKey("GroundPoundFreeze") )
                {
                    CoolDowns["GroundPoundFreeze"] = 0;
                }
            }
        }

        Velocity = newVelocity;
        MoveAndSlide();
    }
    private void AirStall(ref Vector3 newVelocity)
    {
        newVelocity.X *= 0.2f;
        newVelocity.Y = 6f;
        newVelocity.Z *= 0.2f;
    }
}
