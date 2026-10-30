using UnityEngine;

namespace LSA.Bootstrap
{
    /// <summary>
    /// Spawns the compact arena prototype when Play starts.
    /// Auto-creates itself if the open scene has no bootstrap object.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] bool buildOnAwake = true;
        bool _didBuild;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBoot()
        {
            if (Object.FindObjectOfType<GameBootstrap>() != null)
                return;
            var go = new GameObject("LSA_Bootstrap");
            go.AddComponent<GameBootstrap>();
        }

        void Awake()
        {
            if (!buildOnAwake || _didBuild)
                return;
            _didBuild = true;
            PrototypeFactory.BuildAll();
        }
    }
}
