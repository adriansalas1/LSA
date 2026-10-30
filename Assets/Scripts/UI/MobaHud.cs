using LSA.InputUI;
using LSA.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSA.UI
{
    /// <summary>
    /// Touch MOBA layout: left stick; right 3 abilities; bottom basic attack + jump.
    /// Also mirrors WASD / Space / mouse for Editor playtests.
    /// </summary>
    public sealed class MobaHud : MonoBehaviour
    {
        [SerializeField] FighterController fighter;
        [SerializeField] VirtualJoystick joystick;
        [SerializeField] Button ability0;
        [SerializeField] Button ability1;
        [SerializeField] Button ability2;
        [SerializeField] Button basicAttack;
        [SerializeField] Button jump;

        public void Bind(FighterController target, VirtualJoystick stick,
            Button a0, Button a1, Button a2, Button attack, Button jmp)
        {
            fighter = target;
            joystick = stick;
            ability0 = a0;
            ability1 = a1;
            ability2 = a2;
            basicAttack = attack;
            jump = jmp;
            WireButtons();
        }

        void WireButtons()
        {
            ability0?.onClick.RemoveAllListeners();
            ability1?.onClick.RemoveAllListeners();
            ability2?.onClick.RemoveAllListeners();
            basicAttack?.onClick.RemoveAllListeners();
            jump?.onClick.RemoveAllListeners();

            ability0?.onClick.AddListener(() => fighter?.UseAbility(0));
            ability1?.onClick.AddListener(() => fighter?.UseAbility(1));
            ability2?.onClick.AddListener(() => fighter?.UseAbility(2));
            basicAttack?.onClick.AddListener(() => fighter?.BasicAttack());
            jump?.onClick.AddListener(() => fighter?.Jump());
        }

        void Update()
        {
            if (fighter == null)
                return;

            Vector2 move = joystick != null ? joystick.Value : Vector2.zero;

            // Keyboard fallback for Editor / desktop playtests.
            float kx = 0f, kz = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) kx -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) kx += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) kz -= 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) kz += 1f;
            Vector2 key = new Vector2(kx, kz);
            if (key.sqrMagnitude > 0.01f)
                move = Vector2.ClampMagnitude(key, 1f);

            fighter.SetMove(move);

            if (Input.GetKeyDown(KeyCode.Space))
                fighter.Jump();
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUi())
                fighter.BasicAttack();
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
                fighter.UseAbility(0);
            if (Input.GetKeyDown(KeyCode.E))
                fighter.UseAbility(1);
            if (Input.GetKeyDown(KeyCode.R))
                fighter.UseAbility(2);
        }

        static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
