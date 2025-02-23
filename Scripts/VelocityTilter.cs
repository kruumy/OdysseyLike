using Godot;
using System;
using Scripts;

public partial class VelocityTilter : Node3D
{
    private PlayerController Player;
	[Export]
    public float TiltStrength = 1f;
	[Export]
    public float TiltSpeed = 10f;
    public override void _Ready()
    {
        Player = GetParent<PlayerController>();
    }
    public override void _Process(double delta)
    {
        float forwardSpeed = Player.Velocity.Dot(-Player.GlobalTransform.Basis.Z);
        float rightSpeed = Player.Velocity.Dot(Player.GlobalTransform.Basis.X);

        Vector3 newRotation = Vector3.Zero;
        newRotation.X = Mathf.Lerp(RotationDegrees.X, forwardSpeed * TiltStrength, (float)delta * TiltSpeed);
        newRotation.Z = Mathf.Lerp(RotationDegrees.Z, -rightSpeed * TiltStrength, (float)delta * TiltSpeed);
        RotationDegrees = newRotation;
    }
}
