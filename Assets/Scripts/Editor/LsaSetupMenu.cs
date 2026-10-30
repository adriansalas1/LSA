#if UNITY_EDITOR
using LSA.Bootstrap;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSA.EditorTools
{
    public static class LsaSetupMenu
    {
        const string ArenaScenePath = "Assets/Scenes/Arena.unity";

        [MenuItem("LSA/Open Arena Scene")]
        public static void OpenArena()
        {
            EnsureArenaSceneAsset();
            EditorSceneManager.OpenScene(ArenaScenePath);
        }

        [MenuItem("LSA/Rebuild Arena Scene Asset")]
        public static void RebuildArenaScene()
        {
            EnsureArenaSceneAsset();
            EditorUtility.DisplayDialog(
                "LSA",
                "Arena.unity is ready. Press Play — GameBootstrap builds the compact arena, mannequin + sword, MOBA HUD, and precise fireball at runtime.\n\n" +
                "Controls: WASD / stick, Space / JMP, LMB / ATK (sword), Q or RMB / FB (fireball from sword tip).",
                "OK");
        }

        static void EnsureArenaSceneAsset()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");

            var existing = AssetDatabase.LoadAssetAtPath<SceneAsset>(ArenaScenePath);
            if (existing != null)
            {
                var scene = EditorSceneManager.OpenScene(ArenaScenePath);
                EnsureBootstrapInScene(scene);
                EditorSceneManager.SaveScene(scene);
                return;
            }

            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EnsureBootstrapInScene(newScene);
            EditorSceneManager.SaveScene(newScene, ArenaScenePath);
            AssetDatabase.Refresh();

            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ArenaScenePath, true)
            };
            EditorBuildSettings.scenes = scenes;
        }

        static void EnsureBootstrapInScene(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.GetComponent<GameBootstrap>() != null)
                    return;
            }

            var go = new GameObject("LSA_Bootstrap");
            go.AddComponent<GameBootstrap>();
            EditorSceneManager.MarkSceneDirty(scene);
        }
    }
}
#endif
