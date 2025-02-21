using Godot;
using System;

public partial class CameraController : SpringArm3D
{
    [Export] public Node3D Target;
    [Export] public float SmoothSpeed = 5.0f; 
    [Export] public float RotationSensitivity = 5.0f;
    [Export] public float RotationSmoothness = 10.0f; // Higher values = smoother rotation
    [Export] public float MinVerticalAngle = -75.0f; 
    [Export] public float MaxVerticalAngle = 75.0f;
    private Vector3 _smoothedPosition;
    private Vector2 _rotationInput;
    private float _targetVerticalAngle = 0.0f;
    private float _currentVerticalAngle = 0.0f;
    private float _targetYaw = 0.0f;

    public override void _Ready()
    {
        if (Target is PhysicsBody3D targetBody)
        {
            AddExcludedObject(targetBody.GetRid());
        }

        // Initialize rotation angles
        _targetYaw = Rotation.Y;
        _currentVerticalAngle = Rotation.X;
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GlobalPosition.Lerp(new Vector3(GlobalPosition.X,Target.GlobalPosition.Y,GlobalPosition.Z), (float)delta * SmoothSpeed);
        GlobalPosition = GlobalPosition.Lerp(new Vector3(Target.GlobalPosition.X,GlobalPosition.Y,Target.GlobalPosition.Z), (float)delta * SmoothSpeed * SmoothSpeed);

        // Get camera input from keyboard/controller
        float rotateX = Input.GetActionStrength("camera_right") - Input.GetActionStrength("camera_left");
        float rotateY = Input.GetActionStrength("camera_down") - Input.GetActionStrength("camera_up");

        _rotationInput = new Vector2(rotateX, rotateY) * RotationSensitivity * (float)delta;

        // Smooth target yaw rotation (left/right)
        _targetYaw -= _rotationInput.X;
        Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y, _targetYaw, RotationSmoothness * (float)delta), Rotation.Z);

        // Smooth vertical rotation (up/down) with clamping
        _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle - _rotationInput.Y, Mathf.DegToRad(MinVerticalAngle), Mathf.DegToRad(MaxVerticalAngle));
        _currentVerticalAngle = Mathf.Lerp(_currentVerticalAngle, _targetVerticalAngle, RotationSmoothness * (float)delta);
        Rotation = new Vector3(_currentVerticalAngle, Rotation.Y, Rotation.Z);
    }
}
