using Godot;
using System;

namespace Scripts
{
	public partial class DebugLabel : Label3D
	{
		[Export]
		public PlayerController Player;
		[Export]
		public Cap Cap;

		public override void _Process(double delta)
		{
			if(Player == null)
			{
				return;
			}
			GlobalPosition = Player.GlobalPosition + Vector3.Up * 1.7f;
			Text = $"{nameof(Engine.GetFramesPerSecond)}: {Engine.GetFramesPerSecond()}\n" +
				$"{nameof(Cap.CanCapJump)}: {Cap.CanCapJump}\n" +
				$"{nameof(Cap.CanCapPull)}: {Cap.CanCapPull}\n" +
				$"{Player.CurrentState}\n" +
				$"{nameof(Cap.IsThrown)}: {Cap.IsThrown}\n" +
				$"{nameof(Player.IsOnFloor)}: {Player.IsOnFloor()}\n" +
				$"{nameof(Player.IsOnWallOnly)}: {Player.IsOnWallOnly()}\n" +
				$"Velocity: {new Vector3(Player.Velocity.X, 0, Player.Velocity.Z).Length():F2} / {Player.CurrentState.MaxSpeed:F2}\n" +
				$"{nameof(PlayerState.Jump.LastJump)}: {PlayerState.Jump.LastJump.VelocityMultiplier}";
			foreach (var key in Player.CoolDowns.Keys)
			{
				Text += $"\n{key}: {Mathf.RoundToInt(Player.CoolDowns[key] * 10)}";
			}
			
		}
	}
}