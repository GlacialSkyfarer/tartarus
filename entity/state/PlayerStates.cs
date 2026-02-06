using System;
using Godot;
using Godot.Collections;

public enum InputAction {
    Jump
}

public partial class PlayerState : GodotObject
{
    protected Array<PlayerStateModule> _modules = new();

    public virtual PlayerState PhysicsProcess(double delta, Player player) { 
        ProcessModules(true, delta, player);
        return this;
    }
    public virtual PlayerState Process(double delta, Player player) {
        ProcessModules(false, delta, player);
        return this; 
    }

    protected void ProcessModules(bool physics, double delta, Player player) {
        foreach (PlayerStateModule m in _modules) {
            if (physics) m.PhysicsProcess(this, delta, player);
            else m.Process(this, delta, player);
        }
    }
}

public partial class StandingState : PlayerState {
    protected StandingState() { 
        _modules.Add(WalkingModule.Get());
        _modules.Add(GravityModule.Get());
        _modules.Add(GroundJumpModule.Get());
        _modules.Add(CameraModule.Get());
    }

    private static StandingState _instance;
    public static StandingState Get() {
        _instance ??= new StandingState();
        return _instance;
    }

    public override PlayerState PhysicsProcess(double delta, Player player)
    {
        base.PhysicsProcess(delta, player);
        
        if (!player.IsGrounded()) {
            return FallingState.Get(player.GetJustJumped());
        }

        return this;
    }
}

public partial class FallingState : PlayerState {
    protected const float CUT_FACTOR = 0.75f;

    private bool _jumping = false;

    protected FallingState() { 
        _modules.Add(WalkingModule.Get());
        _modules.Add(GravityModule.Get());
        _modules.Add(CameraModule.Get());
    }
    public static FallingState Get(bool jumping = false) {
        FallingState state = new()
        {
            _jumping = jumping
        };
        return state;
    }

    public override PlayerState PhysicsProcess(double delta, Player player)
    {
        base.PhysicsProcess(delta, player);
    
        if (_jumping && player.Velocity.Y > 0 && player.GetInputState().JumpCut()) {
            player.Velocity = new Vector3(player.Velocity.X, player.Velocity.Y * CUT_FACTOR, player.Velocity.Z);
            _jumping = false;
        }

        if (player.IsOnFloor()) {
            return StandingState.Get();
        }
        return this;
    }

}