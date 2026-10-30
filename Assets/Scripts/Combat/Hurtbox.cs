using UnityEngine;

namespace LSA.Combat
{
    /// <summary>
    /// Damage receiver. Size matches visible body — no magnetic assist.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class Hurtbox : MonoBehaviour
    {
        [SerializeField] TeamId team = TeamId.Neutral;
        [SerializeField] float maxHealth = 100f;

        public TeamId Team => team;
        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public bool IsAlive => CurrentHealth > 0f;

        public System.Action<float, GameObject> Damaged;
        public System.Action Died;

        void Awake()
        {
            CurrentHealth = maxHealth;
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        public void Configure(TeamId newTeam, float health)
        {
            team = newTeam;
            maxHealth = health;
            CurrentHealth = health;
        }

        public bool TryApplyDamage(float amount, TeamId sourceTeam, GameObject source)
        {
            if (!IsAlive || amount <= 0f)
                return false;
            if (sourceTeam != TeamId.Neutral && sourceTeam == team)
                return false;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            Damaged?.Invoke(amount, source);
            if (!IsAlive)
                Died?.Invoke();
            return true;
        }
    }
}
