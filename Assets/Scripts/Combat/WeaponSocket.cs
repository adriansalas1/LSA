using UnityEngine;

namespace LSA.Combat
{
    /// <summary>
    /// Equipped weapon defines role. Channel origin is the sword tip when a sword is equipped.
    /// </summary>
    public sealed class WeaponSocket : MonoBehaviour
    {
        [SerializeField] Transform channelOrigin;
        [SerializeField] MeleeHitbox meleeHitbox;
        [SerializeField] bool swordEquipped = true;

        public bool HasSwordEquipped => swordEquipped && channelOrigin != null;
        public Transform ChannelOrigin => HasSwordEquipped ? channelOrigin : transform;
        public MeleeHitbox MeleeHitbox => meleeHitbox;

        public void Bind(Transform tip, MeleeHitbox hitbox, bool equipped)
        {
            channelOrigin = tip;
            meleeHitbox = hitbox;
            swordEquipped = equipped;
        }
    }
}
