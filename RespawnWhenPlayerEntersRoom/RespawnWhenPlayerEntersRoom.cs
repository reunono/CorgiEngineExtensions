using System.Linq;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

public class RespawnWhenPlayerEntersRoom : MonoBehaviour
{
    private UnityEvent _onPlayerEntersRoom;
    private Health _health;
    private AIBrain _brain;
    
    private void Awake()
    {
        _health = GetComponent<Health>();
        _brain = GetComponentInChildren<AIBrain>(true);
        _onPlayerEntersRoom = FindObjectsOfType<Room>().Single(room => room.GetComponent<Collider2D>().bounds.Contains(transform.position.MMSetZ(room.transform.position.z))).OnPlayerEntersRoom;
        _onPlayerEntersRoom.AddListener(Respawn);
    }

    private void OnDestroy()
    {
        _onPlayerEntersRoom.RemoveListener(Respawn);
    }

    private void Respawn()
    {
        gameObject.SetActive(true);
        _health?.Revive();
        _brain?.ResetBrain();
    }
}
