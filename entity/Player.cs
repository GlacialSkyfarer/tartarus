using Godot;
using System;

public partial class Player : CharacterBody3D
{
    private PlayerInputState _inputState = new();
    
    public PlayerInputState GetInputState() { return _inputState; }

    private PlayerState _currentState;

    [ExportGroup("Nodes")]
    [Export] private NodePath p_cameraHolder;
    private Node3D _cameraHolder;
    [Export] private NodePath p_tempSprite;
    private AnimatedSprite3D _tempSprite; 
    [ExportGroup("Variables")]
    [Export] private float _baseSpeed = 5.0f;
    [Export] private float _baseAccelerationTime = 0.5f;

    [Export] private float _baseJumpHeight = 3.0f;

    [Export] private float _cameraVerticalSensitivity = 1f;
    [Export] private float _cameraHorizontalSensitivity = 1f;
    [Export(PropertyHint.Range, "-90,0")] private float _cameraMinimumX = -80f;
    [Export(PropertyHint.Range, "0,90")] private float _cameraMaximumX = 80f;

    [Export] private float _walkTiltAmount = 10f;
    [Export] private float _walkTiltTime = 0.25f;

    private Camera3D _currentCamera;
    private float _walkTilt = 0f;

    private float _jumpForce;

    private bool _justJumped = false;
    public void SetJustJumped() { _justJumped = true; }
    public bool GetJustJumped() { return _justJumped; }

    private bool _hasDoubleJumped = false;
    public void SetDoubleJumped(bool value) {
        _hasDoubleJumped = value;
    }
    public bool GetDoubleJumped() {
        return _hasDoubleJumped;
    }

    public void SetWalkTilt(float value) {
        _walkTilt = value;
    }
    public void RotateCamera(Vector3 rotation) {
        this.RotateY(rotation.Y * _cameraHorizontalSensitivity);
        _cameraHolder.RotateX(rotation.X * _cameraVerticalSensitivity);
        Vector3 cameraRotation = _cameraHolder.Rotation;
        cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(_cameraMinimumX), Mathf.DegToRad(_cameraMaximumX));
        _cameraHolder.Rotation = cameraRotation;
    }

    public Vector3 ToCameraRelative(Vector3 input) {
        return input.Rotated(Vector3.Up, CurrentCamera().GlobalRotation.Y);
    }

    public Camera3D CurrentCamera() {
        _currentCamera ??= GetViewport().GetCamera3D();
        return _currentCamera;
    }

    public void PlayAnimation(string animation, float speed = 1f) {
        _tempSprite.Play(animation, speed);
    }

    public float GetSpeed() {
        return _baseSpeed;
    }
    public float GetAcceleration() {
        return _baseSpeed / _baseAccelerationTime;
    }
    public float GetJumpForce() {
        if (_jumpForce == 0) _jumpForce = Mathf.Sqrt(2 * _baseJumpHeight * 9.8f);
        return _jumpForce;
    }

    public bool IsGrounded() {
        return !_justJumped && IsOnFloor();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _inputState.Update(0.0, @event);
        base._UnhandledInput(@event);
    }

    public override void _Ready()
    {
        base._Ready();
        _tempSprite = GetNode<AnimatedSprite3D>(p_tempSprite);
        _cameraHolder = GetNode<Node3D>(p_cameraHolder);
        _currentState = StandingState.Get();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta) {
        _inputState.Update(delta, null);
        _currentState = _currentState.Process(delta, this);
        Camera3D currentCamera = CurrentCamera();
        float cameraZ = Mathf.Lerp(currentCamera.Rotation.Z, Mathf.DegToRad(_walkTilt * _walkTiltAmount), (float)delta / _walkTiltTime);
        currentCamera.Rotation = new(currentCamera.Rotation.X, currentCamera.Rotation.Y, cameraZ);
        CallDeferred("LateProcess");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _justJumped = false;
        _currentState = _currentState.PhysicsProcess(delta, this);

        MoveAndSlide();
    }

    private void LateProcess() {
        _walkTilt = 0f;
        _inputState.ResetMouse();
    }

}
