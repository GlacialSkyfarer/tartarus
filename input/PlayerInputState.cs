using Godot;

public class PlayerInputState {
    
    private const string MOVEMENT_LEFT = "movement_left";
    private const string MOVEMENT_RIGHT = "movement_right";
    private const string MOVEMENT_UP = "movement_up";
    private const string MOVEMENT_DOWN = "movement_down";

    private const string JUMP = "jump";

    private const float JUMP_BUFFER_MAX = 0.15f;

    private float _xMovementAxis = 0f;
    private float _yMovementAxis = 0f;
    private Vector2 _movementAxis = Vector2.Zero;
    private float _jumpBuffer = 0f;
    
    public bool IsJumping() {
        return _jumpBuffer > 0;
    }

    public bool JumpCut() {
        return !Input.IsActionPressed(JUMP);
    }

    public void ClearJumpBuffer() {
        _jumpBuffer = 0;
    }

    public Vector2 GetMovementAxis() {
        _movementAxis.X = _xMovementAxis;
        _movementAxis.Y = _yMovementAxis;
        return _movementAxis.Normalized();
    }

    public void Update(double delta, InputEvent inputEvent) {
        if (inputEvent == null) {
            _xMovementAxis = Input.GetAxis(MOVEMENT_LEFT, MOVEMENT_RIGHT);
            _yMovementAxis = Input.GetAxis(MOVEMENT_UP, MOVEMENT_DOWN);
            _jumpBuffer -= (float)delta;
            
            if (Input.IsActionJustPressed(JUMP)) _jumpBuffer = JUMP_BUFFER_MAX;
        }
    }
}