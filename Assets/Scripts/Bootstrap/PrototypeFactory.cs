using LSA.Arena;
using LSA.Combat;
using LSA.InputUI;
using LSA.Player;
using LSA.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSA.Bootstrap
{
    /// <summary>
    /// Builds the local 1v1 prototype at runtime (placeholder art).
    /// </summary>
    public static class PrototypeFactory
    {
        public const float ArenaHalfExtent = 10f;

        public static void BuildAll()
        {
            EnsureTags();
            BuildLighting();
            var arena = BuildArena();
            var player = BuildMannequin("PlayerMannequin", new Vector3(-3f, 0f, 0f), TeamId.Player,
                new Color(0.75f, 0.78f, 0.85f));
            var dummy = BuildMannequin("TrainingDummy", new Vector3(3.5f, 0f, 0f), TeamId.Opponent,
                new Color(0.85f, 0.45f, 0.4f));
            dummy.GetComponent<FighterController>().enabled = false;
            dummy.GetComponent<FighterMotor>().enabled = false;

            var cam = BuildCamera(player.transform);
            BuildHud(player.GetComponent<FighterController>());

            Debug.Log(
                "[LSA] Prototype ready. WASD move, Space jump, LMB/Basic sword, Q/RMB fireball (from sword tip). " +
                $"Arena ±{ArenaHalfExtent}m. Projectile radius 8cm — a few-cm sidestep can dodge.");
            _ = arena;
            _ = cam;
            _ = dummy;
        }

        static void EnsureTags()
        {
            // Tags are declared in TagManager; runtime CreatePrimitive uses Untagged by default.
        }

        static void BuildLighting()
        {
            if (Object.FindObjectOfType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.color = new Color(1f, 0.96f, 0.9f);
                light.intensity = 1.1f;
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.35f, 0.38f, 0.45f);
        }

        static ArenaBounds BuildArena()
        {
            var root = new GameObject("Arena");
            var bounds = root.AddComponent<ArenaBounds>();
            bounds.Build(ArenaHalfExtent);
            return bounds;
        }

        static GameObject BuildMannequin(string name, Vector3 position, TeamId team, Color bodyColor)
        {
            var root = new GameObject(name);
            root.transform.position = position;

            var cc = root.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.28f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.skinWidth = 0.04f;

            // Body
            var torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.name = "Body";
            torso.transform.SetParent(root.transform, false);
            torso.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            torso.transform.localScale = new Vector3(0.55f, 0.9f, 0.55f);
            Object.Destroy(torso.GetComponent<Collider>());
            Tint(torso, bodyColor);

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            head.transform.localScale = Vector3.one * 0.35f;
            Object.Destroy(head.GetComponent<Collider>());
            Tint(head, bodyColor * 1.05f);

            // Hurtbox matches visible capsule (tight — precision matters).
            var hurtGo = new GameObject("Hurtbox");
            hurtGo.transform.SetParent(root.transform, false);
            hurtGo.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            var capsule = hurtGo.AddComponent<CapsuleCollider>();
            capsule.isTrigger = true;
            capsule.height = 1.8f;
            capsule.radius = 0.28f;
            var hurt = hurtGo.AddComponent<Hurtbox>();
            hurt.Configure(team, 100f);

            // Sword in right hand
            var hand = new GameObject("RightHand");
            hand.transform.SetParent(root.transform, false);
            hand.transform.localPosition = new Vector3(0.38f, 1.05f, 0.15f);

            var sword = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sword.name = "Sword";
            sword.transform.SetParent(hand.transform, false);
            sword.transform.localPosition = new Vector3(0f, 0f, 0.45f);
            sword.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            sword.transform.localScale = new Vector3(0.06f, 0.9f, 0.02f);
            Object.Destroy(sword.GetComponent<Collider>());
            Tint(sword, new Color(0.7f, 0.75f, 0.85f));

            var tip = new GameObject("SwordTip");
            tip.transform.SetParent(sword.transform, false);
            tip.transform.localPosition = new Vector3(0f, 0.5f, 0f);

            var hitGo = new GameObject("SwordHitbox");
            hitGo.transform.SetParent(sword.transform, false);
            hitGo.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            var box = hitGo.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(0.25f, 0.95f, 0.35f);
            var melee = hitGo.AddComponent<MeleeHitbox>();

            var socket = root.AddComponent<WeaponSocket>();
            socket.Bind(tip.transform, melee, true);

            var motor = root.AddComponent<FighterMotor>();
            var fireball = root.AddComponent<FireballAbility>();
            var fighter = root.AddComponent<FighterController>();
            fighter.Bind(motor, socket, fireball, melee, team);

            if (team == TeamId.Opponent)
                root.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            return root;
        }

        static FightCamera BuildCamera(Transform target)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
            cam.fieldOfView = 50f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 120f;

            var follow = cam.GetComponent<FightCamera>() ?? cam.gameObject.AddComponent<FightCamera>();
            follow.SetTarget(target);
            cam.transform.position = target.position + new Vector3(0f, 11f, -10f);
            return follow;
        }

        static void BuildHud(FighterController fighter)
        {
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("MobaHudCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Left stick
            var stickRoot = CreatePanel(canvasGo.transform, "Joystick",
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(220f, 220f),
                new Vector2(140f, 140f), new Color(1f, 1f, 1f, 0.08f));
            var stickBg = stickRoot.GetComponent<RectTransform>();
            var knob = CreatePanel(stickRoot.transform, "Knob",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(70f, 70f),
                Vector2.zero, new Color(1f, 1f, 1f, 0.35f));
            var joystick = stickRoot.AddComponent<VirtualJoystick>();
            joystick.Bind(stickBg, knob.GetComponent<RectTransform>(), 70f);

            // Right ability cluster
            Button a0 = CreateHudButton(canvasGo.transform, "Ability0_Fireball",
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-120f, 200f), "FB",
                new Color(1f, 0.45f, 0.15f, 0.75f));
            Button a1 = CreateHudButton(canvasGo.transform, "Ability1_Stub",
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-220f, 140f), "A2",
                new Color(0.4f, 0.55f, 0.9f, 0.55f));
            Button a2 = CreateHudButton(canvasGo.transform, "Ability2_Stub",
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-120f, 90f), "A3",
                new Color(0.4f, 0.7f, 0.55f, 0.55f));

            // Bottom: basic attack + jump
            Button attack = CreateHudButton(canvasGo.transform, "BasicAttack",
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-70f, 70f), "ATK",
                new Color(0.85f, 0.85f, 0.9f, 0.7f));
            Button jump = CreateHudButton(canvasGo.transform, "Jump",
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(70f, 70f), "JMP",
                new Color(0.7f, 0.85f, 1f, 0.7f));

            var hud = canvasGo.AddComponent<MobaHud>();
            hud.Bind(fighter, joystick, a0, a1, a2, attack, jump);

            // Hint label
            var hint = CreatePanel(canvasGo.transform, "Hint",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(700f, 40f),
                new Vector2(0f, -36f), new Color(0f, 0f, 0f, 0.35f));
            var text = hint.AddComponent<Text>();
            text.text = "LSA prototype — stick/WASD · ATK/LMB sword · FB/Q fireball from tip · JMP/Space · dodge by cm";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 18;
            text.font = UiFont();
            text.raycastTarget = false;
        }

        static Font UiFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                   ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 size, Vector2 anchoredPos, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;
            go.GetComponent<Image>().color = color;
            return go;
        }

        static Button CreateHudButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, string label, Color color)
        {
            var go = CreatePanel(parent, name, anchorMin, anchorMax, new Vector2(96f, 96f), anchoredPos, color);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = go.GetComponent<Image>();
            var textGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var trt = textGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;
            var text = textGo.GetComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 22;
            text.fontStyle = FontStyle.Bold;
            text.font = UiFont();
            text.raycastTarget = false;
            return btn;
        }

        static void Tint(GameObject go, Color color)
        {
            var rend = go.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            rend.material = new Material(shader) { color = color };
        }
    }
}
