using MoreMountains.CorgiEngine;

public class FlyingCharacterDash : CharacterDash
{
    private CharacterFly _fly;
    private bool _flying;
    protected override void Initialization()
    {
        base.Initialization();
        _fly = _character.FindAbility<CharacterFly>();
    }

    public override void InitiateDash()
    {
        _fly.enabled = false;
        _flying = _movement.CurrentState == CharacterStates.MovementStates.Flying;
        if (_flying) _fly.StopFlight();
        base.InitiateDash();
    }
    
    public override void StopDash()
    {
        base.StopDash();
        if (_flying) _fly.StartFlight();
        _fly.enabled = true;
    }
}
