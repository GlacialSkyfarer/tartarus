using Godot;

public partial class Weapon : Resource
{
    public virtual void Process(double delta, Player player) { }
    public virtual void PhysicsProcess(double delta, Player player) { }
    public virtual void Fire(Player player) { }
    public virtual void AltFire(Player player) { }
    public virtual void Init(Player player) { }
    public virtual void SwitchIn(Player player) { }
    public virtual void SwitchOut(Player player) { }
}
