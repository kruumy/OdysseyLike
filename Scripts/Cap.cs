using Godot;
using System;

namespace Scripts
{
	public partial class Cap : Area3D
	{
		[Export]
		public PlayerController Player;
		[Export]
		public Node3D CapPoint;
		public const float ThrowDistance = 5f;
		public bool CanCapJump = true;
		public bool IsThrown = false;
		public Tween CapThrowTween;
		public void Throw()
		{
			IsThrown = true;
			this.Rotation = Vector3.Zero;
			var temp = Player.GlobalPosition + ((Player.CurrentState.InputDirection != Vector3.Zero ? Player.CurrentState.InputDirection : -Player.GlobalTransform.Basis.Z) * ThrowDistance);
			AnimateCapThrow(temp,new Vector3(1f,0.75f,1f), 0.2f);
			if(Player.CurrentState.InputDirection != Vector3.Zero)
				Player.LookAt(Player.GlobalPosition + Player.CurrentState.InputDirection);
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

		public void AnimateCapThrow(Vector3 newPosition, Vector3 newScale, float duration)
		{
			CapThrowTween = CreateTween();
			CapThrowTween.SetParallel(true);
			CapThrowTween.TweenProperty(this, "global_position", newPosition, duration)
				.SetTrans(Tween.TransitionType.Quad)
				.SetEase(Tween.EaseType.Out);
			CapThrowTween.TweenProperty(this, "scale", newScale, duration)
				.SetTrans(Tween.TransitionType.Quad)
				.SetEase(Tween.EaseType.Out);
		}

		public void _on_body_entered(Node3D body)
		{
			if(body == Player)
			{
				if(IsThrown && !CapThrowTween.IsRunning())
				{
					if(CanCapJump && Player.CurrentState is not PlayerState.CapPull)
					{
						if(!Player.IsOnFloor())
							CanCapJump = false;
						Player.CurrentState = new PlayerState.CapJump();
					}
					Return();
				}
			}
			else
			{
				CapThrowTween?.Kill();
			}
		}
	}
}