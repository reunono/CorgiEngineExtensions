using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

public class CharacterTimeToJumpApex : MonoBehaviour, MMEventListener<MMCharacterEvent>
{
    [SerializeField] private float TimeToJumpApex = .1f;

    private Character _character;
    private CharacterJump _characterJump;
    private CorgiController _controller;
    private float _reachApexTime = float.PositiveInfinity;
    private float _initialGravity;

    private void Awake()
    {
        _character = GetComponentInParent<Character>();
        _characterJump = _character.FindAbility<CharacterJump>();
        _controller = _character.GetComponent<CorgiController>();
    }

    private void Update()
    {
        if (Time.time < _reachApexTime) return;
        _controller.SetVerticalForce(0);
        _controller.Parameters.Gravity = _initialGravity;
        _reachApexTime = float.PositiveInfinity;
    }

    public void OnMMEvent(MMCharacterEvent characterEvent)
    {
        if (characterEvent.TargetCharacter != _character ||
            characterEvent.EventType != MMCharacterEventTypes.Jump) return;
        _initialGravity = _controller.Parameters.Gravity;
        _controller.Parameters.Gravity = -2 * _characterJump.JumpHeight / (TimeToJumpApex * TimeToJumpApex);
        _reachApexTime = Time.time + TimeToJumpApex;
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }
    
    private void OnDisable()
    {
        this.MMEventStopListening();
    }
}
