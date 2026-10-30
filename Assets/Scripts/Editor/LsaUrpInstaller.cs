#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_PIPELINE_URP || true
using UnityEngine.Rendering.Universal;
#endif

namespace LSA.EditorTools
{
    /// <summary>
    /// Creates URP pipeline assets with real package script references (not hand-written YAML).
    /// </summary>
    public static class LsaUrpInstaller
    {
        const string SettingsFolder = "Assets/Settings";
        const string PipelinePath = SettingsFolder + "/LSA-URP.asset";
        const string RendererPath = SettingsFolder + "/LSA-URP-Renderer.asset";

        [MenuItem("LSA/Configure URP (Mobile)")]
        public static void ConfigureUrp()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");

            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            pipeline.name = "LSA-URP";
            EditorUtility.SetDirty(pipeline);
            EditorUtility.SetDirty(renderer);

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "LSA URP",
                "Universal Render Pipeline assigned for mobile-friendly rendering.\n\nOpen Assets/Scenes/Arena and press Play.",
                "OK");
        }

        [InitializeOnLoadMethod]
        static void OfferOnce()
        {
            // Non-blocking: if URP package is present and no pipeline assigned, auto-configure.
            if (GraphicsSettings.defaultRenderPipeline != null)
                return;
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            // Defer one frame so package import can finish.
            EditorApplication.delayCall += () =>
            {
                if (GraphicsSettings.defaultRenderPipeline != null)
                    return;
                try
                {
                    ConfigureUrpSilent();
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("[LSA] URP auto-configure skipped: " + ex.Message +
                                     " — use menu LSA/Configure URP (Mobile) after packages import.");
                }
            };
        }

        static void ConfigureUrpSilent()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");

            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
            AssetDatabase.SaveAssets();
            Debug.Log("[LSA] URP pipeline assigned automatically.");
        }
    }
}
#endif
