using System.Collections.Generic;
using UnityEngine;

namespace LSA.Combat
{
    /// <summary>
    /// Short-lived sword sweep. Hits only overlapping hurtboxes — no magnet.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class MeleeHitbox : MonoBehaviour
    {
        [SerializeField] float damage = 12f;
        [SerializeField] float activeSeconds = 0.12f;

        TeamId _team;
        GameObject _owner;
        float _expireAt;
        readonly HashSet<Hurtbox> _hit = new HashSet<Hurtbox>();
        Collider _collider;

        void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = false;

            var rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void Configure(TeamId team, GameObject owner, float dmg)
        {
            _team = team;
            _owner = owner;
            damage = dmg;
        }

        public void Activate()
        {
            _hit.Clear();
            _expireAt = Time.time + activeSeconds;
            _collider.enabled = true;
        }

        void Update()
        {
            if (_collider.enabled && Time.time >= _expireAt)
                _collider.enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!_collider.enabled)
                return;
            var hurt = other.GetComponent<Hurtbox>() ?? other.GetComponentInParent<Hurtbox>();
            if (hurt == null || !_hit.Add(hurt))
                return;
            hurt.TryApplyDamage(damage, _team, _owner);
        }
    }
}
