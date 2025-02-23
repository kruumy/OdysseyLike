using Godot;
using System;

public partial class VelocityTilter : Node3D
{
    private CharacterBody3D Player;

    [Export] public float TiltStrength = 2f;
    [Export] public float TiltSpeed = 10f;

    public override void _Ready()
    {
        Player = GetParent<CharacterBody3D>();
    }

    public override void _Process(double delta)
    {
        float forwardSpeed = Player.Velocity.Dot(-Player.GlobalTransform.Basis.Z);
        float rightSpeed = Player.Velocity.Dot(Player.GlobalTransform.Basis.X);
        float verticalSpeed = Player.Velocity.Y;

        Vector3 newRotation = Vector3.Zero;
        newRotation.X = Mathf.Lerp(RotationDegrees.X, (forwardSpeed * TiltStrength) + (verticalSpeed * TiltStrength), (float)delta * TiltSpeed);
        newRotation.Z = Mathf.Lerp(RotationDegrees.Z, -rightSpeed * TiltStrength, (float)delta * TiltSpeed);

        RotationDegrees = newRotation;
    }
}
