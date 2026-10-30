using UnityEngine;

namespace LSA.Player
{
    /// <summary>
    /// CharacterController movement + mandatory jump for all fighters.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class FighterMotor : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5.5f;
        [SerializeField] float jumpSpeed = 8.5f;
        [SerializeField] float gravity = 28f;
        [SerializeField] float turnSpeed = 720f;

        CharacterController _cc;
        Vector3 _velocity;
        Vector2 _moveInput;

        public bool IsGrounded => _cc != null && _cc.isGrounded;
        public Vector3 PlanarFacing { get; private set; } = Vector3.forward;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        public void SetMoveInput(Vector2 planar)
        {
            _moveInput = Vector2.ClampMagnitude(planar, 1f);
        }

        public void TryJump()
        {
            if (IsGrounded)
                _velocity.y = jumpSpeed;
        }

        void Update()
        {
            Vector3 wish = new Vector3(_moveInput.x, 0f, _moveInput.y);
            if (wish.sqrMagnitude > 0.001f)
            {
                PlanarFacing = wish.normalized;
                Quaternion target = Quaternion.LookRotation(PlanarFacing, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            Vector3 horizontal = wish * moveSpeed;
            if (_cc.isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;
            _velocity.y -= gravity * Time.deltaTime;

            Vector3 delta = new Vector3(horizontal.x, _velocity.y, horizontal.z) * Time.deltaTime;
            _cc.Move(delta);
        }
    }
}
