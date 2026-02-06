using Godot;
using System;

public partial class Player : CharacterBody3D
{
    protected PlayerInputState _inputState = new();
    
    public PlayerInputState GetInputState() { return _inputState; }

    protected PlayerState _currentState;

    [Export] protected float _baseSpeed = 5.0f;
    [Export] protected float _baseAccelerationTime = 0.5f;

    [Export] protected float _baseJumpHeight = 3.0f;

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
        _currentState = StandingState.Get();
    }

    public override void _Process(double delta) {

        _inputState.Update(delta, null);
        _currentState = _currentState.Process(delta, this);

    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _justJumped = false;
        _currentState = _currentState.PhysicsProcess(delta, this);

        MoveAndSlide();
    }

}
