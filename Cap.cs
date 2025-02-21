using Godot;
using System;

public partial class Cap : Area3D
{
	[Export]
	public PlayerController Player;
	[Export]
	public Node3D CapPoint;
	public const float ThrowDistance = 5f;
	public bool CanCapJump = true;
	public bool IsThrown = false;
	public Tween Tween;
	public void Throw()
	{
		IsThrown = true;
		var temp = Player.GlobalPosition + ((Player.IsMoving ? Player.InputDirection : -Player.GlobalTransform.Basis.Z) * ThrowDistance);
		AnimateCapMovement(temp,new Vector3(1f,0.75f,1f), 0.2f);
		if(Player.IsMoving)
			Player.LookAt(Player.GlobalPosition + Player.InputDirection);
	}
	public void Return()
	{
		Scale = new Vector3(0.5f,0.5f,0.5f);
		IsThrown = false;
	}
    public override void _Ready()
    {
		Return();
    }
    public override void _Process(double delta)
	{
		if(Player.IsOnFloor())
		{
			CanCapJump = true;
		}

		if(IsThrown)
		{
			RotateY(Mathf.DegToRad(1440 * (float)delta));
		}
		else
		{
			GlobalPosition = CapPoint.GlobalPosition;
			GlobalRotation = CapPoint.GlobalRotation;
		}
	}

	public void AnimateCapMovement(Vector3 newPosition, Vector3 newScale, float duration)
	{
		Tween = CreateTween();
		Tween.SetParallel(true);
		Tween.TweenProperty(this, "global_position", newPosition, duration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
		Tween.TweenProperty(this, "scale", newScale, duration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
	}

	public void _on_body_entered(Node3D body)
	{
		if(body == Player)
		{
			if(IsThrown && !Tween.IsRunning())
			{
				if(CanCapJump)
				{
					if(!Player.IsOnFloor())
						CanCapJump = false;
					Player.CapJumpNextFrame = true;
				}
				Return();
			}
		}
		else
		{
			Tween?.Kill();
		}
	}
}
