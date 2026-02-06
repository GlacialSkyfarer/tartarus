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

        velocity = velocity.MoveToward(new Vector3(movementInput.X * speed, velocity.Y, movementInput.Y * speed), acceleration * (float)delta);

        player.Velocity = velocity;
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