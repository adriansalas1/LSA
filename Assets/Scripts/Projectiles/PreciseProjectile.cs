using LSA.Combat;
using UnityEngine;

namespace LSA.Projectiles
{
    /// <summary>
    /// Straight-line projectile with a small collider. No homing / magnetic aim.
    /// A lateral move of a few centimeters can dodge it.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public sealed class PreciseProjectile : MonoBehaviour
    {
        [SerializeField] float speed = 14f;
        [SerializeField] float lifetime = 2.5f;
        [SerializeField] float damage = 18f;
        [Tooltip("Hit radius in meters. ~0.08 lets a few-cm sidestep clear the shot.")]
        [SerializeField] float radius = 0.08f;

        TeamId _team;
        GameObject _owner;
        Vector3 _direction;
        float _despawnAt;
        bool _spent;

        public void Launch(Vector3 origin, Vector3 direction, TeamId team, GameObject owner,
            float projectileSpeed = 14f, float projectileDamage = 18f, float hitRadius = 0.08f)
        {
            transform.position = origin;
            _direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;
            transform.rotation = Quaternion.LookRotation(_direction);
            _team = team;
            _owner = owner;
            speed = projectileSpeed;
            damage = projectileDamage;
            radius = hitRadius;
            _despawnAt = Time.time + lifetime;
            _spent = false;

            var col = GetComponent<SphereCollider>();
            col.isTrigger = true;
            // Collider radius is in local space; visual scale is diameter = radius*2.
            float scale = Mathf.Max(transform.lossyScale.x, 0.0001f);
            col.radius = radius / scale;

            // Kinematic RB so trigger overlaps against other triggers register.
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        void Update()
        {
            if (_spent)
                return;

            // Discrete step (no seek). Fixed speed keeps dodge windows predictable.
            float step = speed * Time.deltaTime;
            if (Physics.SphereCast(transform.position, radius, _direction, out var hit, step,
                    ~0, QueryTriggerInteraction.Collide))
            {
                TryHit(hit.collider);
                if (_spent)
                    return;
                // Non-hurtbox solid (arena wall, ground edge): stop.
                if (!hit.collider.isTrigger)
                {
                    Despawn();
                    return;
                }
            }

            transform.position += _direction * step;

            if (Time.time >= _despawnAt)
                Despawn();
        }

        void OnTriggerEnter(Collider other) => TryHit(other);

        void TryHit(Collider other)
        {
            if (_spent)
                return;
            var hurt = other.GetComponent<Hurtbox>() ?? other.GetComponentInParent<Hurtbox>();
            if (hurt == null)
                return;
            if (hurt.TryApplyDamage(damage, _team, _owner))
                Despawn();
        }

        void Despawn()
        {
            _spent = true;
            Destroy(gameObject);
        }
    }
}
