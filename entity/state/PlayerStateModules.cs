using System;
using Godot;

public partial class PlayerStateModule : GodotObject {
    public virtual void Process(PlayerState state, double delta, Player player) { }
    public virtual void PhysicsProcess(PlayerState state, double delta, Player player) { }
}

public partial class WalkingModule : PlayerStateModule {
    private WalkingModule() { }
    private static WalkingModule _instance;
    public static WalkingModule Get() {
        _instance ??= new WalkingModule();
        return _instance;
    }
    public override void PhysicsProcess(PlayerState state, double delta, Player player)
    {
        base.PhysicsProcess(state, delta, player);

        Vector3 velocity = player.Velocity;
        PlayerInputState inputState = player.GetInputState();

        Vector2 movementInput = inputState.GetMovementAxis();

        float speed = player.GetSpeed();
        float acceleration = player.GetAcceleration();

        Vector3 movementVector = new Vector3(movementInput.X * speed, velocity.Y, movementInput.Y * speed);
        movementVector = player.ToCameraRelative(movementVector);

        velocity = velocity.MoveToward(movementVector, acceleration * (float)delta);

        player.Velocity = velocity;

        player.SetWalkTilt(-movementInput.X);
    }
}

public partial class GravityModule : PlayerStateModule {
    private GravityModule() { }
    private static GravityModule _instance;
    public static GravityModule Get() {
        _instance ??= new GravityModule();
        return _instance;
    }
    public override void PhysicsProcess(PlayerState state, double delta, Player player)
    {
        base.PhysicsProcess(state, delta, player);

        player.Velocity += player.GetGravity() * (float)delta;
    }
}

public partial class GroundJumpModule : PlayerStateModule {
    private GroundJumpModule() { }
    private static GroundJumpModule _instance;
    public static GroundJumpModule Get() {
        _instance ??= new GroundJumpModule();
        return _instance;
    }
    public override void PhysicsProcess(PlayerState state, double delta, Player player)
    {
        base.PhysicsProcess(state, delta, player);

        PlayerInputState inputState = player.GetInputState();

        if (inputState.IsJumping()) {
            inputState.ClearJumpBuffer();
            player.Velocity = new Vector3(player.Velocity.X, player.GetJumpForce(), player.Velocity.Z);
            player.SetJustJumped();
        }
    }
}

public partial class CameraModule : PlayerStateModule {
    private CameraModule() {}
    private static CameraModule _instance;
    public static CameraModule Get() {
        _instance ??= new CameraModule();
        return _instance;
    }

    public override void Process(PlayerState state, double delta, Player player)
    {
        base.Process(state, delta, player);
        PlayerInputState inputState = player.GetInputState();
        Vector2 mouseMotion = inputState.GetMouseMotion();
        player.RotateCamera(new Vector3(-mouseMotion.Y * 0.001f, -mouseMotion.X * 0.001f, 0));
    }
}