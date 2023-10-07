using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Projectile))]
public class ProjectileHoming : MonoBehaviour{
    [SerializeField] private ContactFilter2D TargetDetectionFilter;
    [SerializeField] private float TargetDetectionRadius = 100;
    [SerializeField] private AnimationCurve Speed = AnimationCurve.EaseInOut(0, 0, 5, 1000);
    [SerializeField, HideInInspector] private Projectile Projectile;
    [SerializeField, HideInInspector] private Collider2D Collider;
    [SerializeField, HideInInspector] private MMPoolableObject Poolable;
    private Collider2D _target;
    private float _time;
    private readonly Collider2D[] _targets = new Collider2D[10];

    private void OnSpawnComplete(){
        _time = 0;
        _target = null;
        var count = Physics2D.OverlapCircle(Collider.bounds.center, TargetDetectionRadius, TargetDetectionFilter, _targets);
        if (count == 0) return;
        var smallestAngle = 180f;
        _target = _targets[0];
        for (var i = 0; i < count; i++)
        {
            var angleToTarget = Vector3.Angle(Projectile.Direction, _targets[i].bounds.center - transform.position);
            if (angleToTarget > smallestAngle) continue;
            smallestAngle = angleToTarget;
            _target = _targets[i];
        }
    }
    private void Update(){
        if (!_target) return;
        var direction = Vector3.RotateTowards(Projectile.Direction, _target.bounds.center - Collider.bounds.center,
            Speed.Evaluate(_time) * Time.deltaTime, 0);
        var rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        Projectile.SetDirection(direction, rotation);
        _time += Time.deltaTime;
    }

    private void OnValidate(){
        Projectile ??= GetComponent<Projectile>();
        Collider ??= GetComponent<Collider2D>();
        Poolable ??= GetComponent<MMPoolableObject>();
    }
    private void OnEnable() => Poolable.OnSpawnComplete += OnSpawnComplete;
    private void OnDisable() => Poolable.OnSpawnComplete -= OnSpawnComplete;
    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, TargetDetectionRadius);
    }
    private void Reset(){
        TargetDetectionFilter.useTriggers = true;
        TargetDetectionFilter.SetLayerMask(LayerManager.EnemiesLayerMask);
    }
}
