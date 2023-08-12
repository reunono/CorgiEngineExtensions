using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class PogoWeapon : Weapon
{
    [MMInspectorGroup("Weapons", true, 36)]
    public Weapon Pogo;
    public Weapon Weapon;
    private Weapon _weapon;
    private Weapon _last;
    protected override void LateUpdate(){}
    private void Awake()
    {
        Pogo.enabled = false;
        Weapon.enabled = false;
        _weapon = Weapon;
    }
    public override void SetOwner(Character newOwner, CharacterHandleWeapon handleWeapon)
    {
        base.SetOwner(newOwner, handleWeapon);
        Pogo.SetOwner(newOwner, handleWeapon);
        Weapon.SetOwner(newOwner, handleWeapon);
        _controller = Owner.GetComponent<CorgiController>();
    }
    public override void Initialization()
    {
        base.Initialization();
        Pogo.Initialization();
        Weapon.Initialization();
    }
    public override void TurnWeaponOn()
    {
        if (_last && _last.enabled) return;
        _last = !_controller.State.IsGrounded && Owner.LinkedInputManager.PrimaryMovement.y < -Owner.LinkedInputManager.Threshold.y ? Pogo : _weapon;
        _last.WeaponState.OnStateChange += DisableOnStop;
        _last.enabled = true;
        _last.TurnWeaponOn();
    }
    private void DisableOnStop()
    {
        if (_last.WeaponState.CurrentState != WeaponStates.WeaponStop) return;
        _last.WeaponState.OnStateChange -= DisableOnStop;
        _last.enabled = false;
    }
    protected override void Update()
    {
        base.Update();
        if (CharacterHandleWeapon.CurrentWeapon == this) return;
        // we store which weapon ComboWeapon changed to and change CurrentWeapon back to this
        _weapon = CharacterHandleWeapon.CurrentWeapon;
        CharacterHandleWeapon.CurrentWeapon = this;
        Weapon.transform.SetParent(transform);
        _weapon.enabled = false;
    }

    public override void FlipWeapon()
    {
        base.FlipWeapon();
        Weapon.GetComponent<ComboWeapon>()?.FlipUnusedWeapons();
    }
}
