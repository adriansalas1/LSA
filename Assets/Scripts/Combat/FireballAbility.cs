using LSA.Projectiles;
using UnityEngine;

namespace LSA.Combat
{
    /// <summary>
    /// Base magic. When a sword is equipped, the fireball channels from the sword tip.
    /// </summary>
    public sealed class FireballAbility : MonoBehaviour
    {
        [SerializeField] float cooldown = 0.85f;
        [SerializeField] float speed = 14f;
        [SerializeField] float damage = 18f;
        [SerializeField] float hitRadius = 0.08f;
        [SerializeField] Material projectileMaterial;

        WeaponSocket _weapon;
        TeamId _team;
        GameObject _owner;
        float _readyAt;

        public bool IsReady => Time.time >= _readyAt;

        public void Configure(WeaponSocket weapon, TeamId team, GameObject owner, Material mat = null)
        {
            _weapon = weapon;
            _team = team;
            _owner = owner;
            if (mat != null)
                projectileMaterial = mat;
        }

        public bool TryCast(Vector3 aimDirection)
        {
            if (!IsReady)
                return false;

            Transform origin = _weapon != null ? _weapon.ChannelOrigin : transform;
            Vector3 dir = aimDirection.sqrMagnitude > 0.0001f
                ? aimDirection.normalized
                : (origin.forward.sqrMagnitude > 0.0001f ? origin.forward : transform.forward);

            // Spawn slightly ahead of the tip so the caster's own hurtbox is not hit.
            Vector3 spawnPos = origin.position + dir * 0.15f;

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Fireball";
            Object.Destroy(go.GetComponent<Collider>());
            var sphere = go.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 1f; // scaled by transform; PreciseProjectile resets radius after scale

            var rend = go.GetComponent<MeshRenderer>();
            if (projectileMaterial != null)
                rend.sharedMaterial = projectileMaterial;
            else
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                rend.material = new Material(shader) { color = new Color(1f, 0.35f, 0.05f, 1f) };
            }

            // Visual matches hit radius (~8cm) so dodge readability stays honest.
            go.transform.localScale = Vector3.one * (hitRadius * 2f);
            var proj = go.AddComponent<PreciseProjectile>();
            proj.Launch(spawnPos, dir, _team, _owner, speed, damage, hitRadius);

            _readyAt = Time.time + cooldown;
            return true;
        }
    }
}
