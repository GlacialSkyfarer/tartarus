using Godot;
using System;

public partial class SteamgunWeapon : Weapon
{

    enum State {
        Idle,
        Reloading,
        Crunched
    }

    [Export] private int _maximumAmmo = 6;
    private int _currentAmmo = 6;
    [Export] private int _totalDamage = 35;
    private int _pelletDamage;
    [Export] private int _pelletCount = 7;
    [Export] private int _maximumSlugDamage = 350;
    private bool _firingSlug = false;
    private State _currentState = State.Idle;

    [Export] private float _fireCooldown = 0.6f;
    private float _fireTimer = 0.0f;
    [Export] private float _reloadCooldown = 0.6f;
    private float _reloadTimer = 0.0f;
    [Export] private float _reloadDelay = 1.0f;
    private float _delayTimer = 0.0f;

    private Label _ammoLabel = new();

    private void SetPelletDamage() {
        _pelletDamage = _totalDamage / _pelletCount;
    }

    public override void Init(Player player)
    {
        base.Init(player);
        _currentAmmo = _maximumAmmo;
        SetPelletDamage();
        player.AddChild(_ammoLabel);
        _ammoLabel.SetPosition(new Vector2(50,50));
    }

    public override void SwitchIn(Player player)
    {
        base.SwitchIn(player);
        player.PlayGunAnimation("SteamgunIdle");
    }

    public override void Process(double delta, Player player)
    {

        _ammoLabel.Text = _currentAmmo + "/" + _maximumAmmo;

        base.Process(delta, player);
        switch (_currentState) {
            case State.Idle:
                if (_fireTimer > 0) _fireTimer -= (float)delta;
                if (_delayTimer > 0) _delayTimer -= (float)delta;
                else if (_currentAmmo < _maximumAmmo) {
                    _currentState = State.Reloading;
                    _fireTimer = 0;
                    _delayTimer = 0;
                }
                break;
            case State.Reloading:
                if (_reloadTimer > 0) _reloadTimer -= (float)delta;
                else {
                    _currentAmmo += 1;
                    _reloadTimer = _reloadCooldown;
                    player.PlayGunAnimation("SteamgunReload");
                    if (_currentAmmo >= _maximumAmmo) {
                        _currentAmmo = _maximumAmmo;
                        _currentState = State.Idle;
                    }
                }
                break;
            default:
                break;
        }
    }

    public override void Fire(Player player) {
        base.Fire(player);
        if (_currentAmmo > 0 && _fireTimer <= 0) {
            _currentAmmo--;
            player.PlayGunAnimation("SteamgunFire");
            _delayTimer = _reloadDelay;
            _fireTimer = _fireCooldown;
            _currentState = State.Idle;
        }
    }

}
