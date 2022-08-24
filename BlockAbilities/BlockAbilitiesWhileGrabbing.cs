using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

public class BlockAbilitiesWhileGrabbing : MonoBehaviour, MMEventListener<MMCharacterEvent>
{
    public CharacterAbilities[] AbilitiesToBlock;
    private Character _character;

    private void Awake()
    {
        _character = GetComponentInParent<Character>();
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }
    
    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMCharacterEvent characterEvent)
    {
        if (characterEvent.TargetCharacter != _character || characterEvent.EventType != MMCharacterEventTypes.Grab) return;
        CharacterAbilitiesUtils.PermitAbilities(_character, AbilitiesToBlock, characterEvent.Moment == MMCharacterEvent.Moments.End);
    }
}
