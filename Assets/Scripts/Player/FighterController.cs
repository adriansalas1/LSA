using LSA.Combat;
using UnityEngine;

namespace LSA.Player
{
    /// <summary>
    /// Wires input to motor, sword melee, and fireball (channeled via weapon).
    /// </summary>
    public sealed class FighterController : MonoBehaviour
    {
        [SerializeField] FighterMotor motor;
        [SerializeField] WeaponSocket weapon;
        [SerializeField] FireballAbility fireball;
        [SerializeField] MeleeHitbox swordHitbox;
        [SerializeField] float meleeDamage = 12f;
        [SerializeField] float meleeCooldown = 0.35f;

        float _meleeReadyAt;
        TeamId _team = TeamId.Player;

        public FighterMotor Motor => motor;
        public TeamId Team => _team;

        public void Bind(FighterMotor m, WeaponSocket w, FireballAbility f, MeleeHitbox hitbox, TeamId team)
        {
            motor = m;
            weapon = w;
            fireball = f;
            swordHitbox = hitbox;
            _team = team;
            swordHitbox?.Configure(team, gameObject, meleeDamage);
            fireball?.Configure(weapon, team, gameObject);
        }

        public void SetMove(Vector2 planar) => motor?.SetMoveInput(planar);
        public void Jump() => motor?.TryJump();

        public void BasicAttack()
        {
            if (Time.time < _meleeReadyAt)
                return;
            if (weapon == null || !weapon.HasSwordEquipped || swordHitbox == null)
                return;

            swordHitbox.Activate();
            _meleeReadyAt = Time.time + meleeCooldown;
        }

        public void UseAbility(int index)
        {
            // Prototype stubs: ability 0 = fireball; 1–2 reserved.
            if (index == 0)
                CastFireball();
        }

        void CastFireball()
        {
            if (fireball == null)
                return;
            Vector3 aim = motor != null ? motor.PlanarFacing : transform.forward;
            fireball.TryCast(aim);
        }
    }
}
