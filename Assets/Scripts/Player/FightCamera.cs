using UnityEngine;

namespace LSA.Player
{
    public sealed class FightCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new Vector3(0f, 11f, -10f);
        [SerializeField] float followLerp = 8f;

        public void SetTarget(Transform t) => target = t;

        void LateUpdate()
        {
            if (target == null)
                return;
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followLerp * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 1.2f - transform.position, Vector3.up);
        }
    }
}
