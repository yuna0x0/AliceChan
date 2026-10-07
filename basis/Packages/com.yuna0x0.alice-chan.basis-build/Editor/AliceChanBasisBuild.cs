using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using yuna0x0.Basis.Convert.Pipeline;

namespace yuna0x0.AliceChan.BasisBuild
{
    // Batch mode entry point:
    //   Unity -batchmode -executeMethod AliceChanBasisBuild.Build
    //         -aliceChanVersion 2.0.0 -aliceChanTargets StandaloneWindows64,StandaloneLinux64,Android
    //
    // Instantiates the package prefab in an empty scene, converts its VRM components with Watari,
    // fills in the bundle description, and hands the result to the Basis SDK, which writes
    // AssetBundles/AliceChan/<id>.BEE, the password file and the premade server config.
    // The SDK build is asynchronous, so the editor is kept alive until it finishes and then
    // exits with 0 on success or 1 on failure.
    public static class AliceChanBasisBuild
    {
        const string PrefabPath = "Packages/com.yuna0x0.alice-chan/Models/AliceChan.prefab";
        const string ThumbnailPath = "Packages/com.yuna0x0.alice-chan/Models/AliceChan.vrm";
        const string BundleName = "AliceChan";

        public static void Build()
        {
            try
            {
                string version = Argument("-aliceChanVersion", "0.0.0");
                List<BuildTarget> targets = ParseTargets(Argument("-aliceChanTargets", "StandaloneWindows64"));
                Debug.Log($"AliceChan Basis build {version} for {string.Join(", ", targets)}");

                GameObject avatar = Prepare(version);
                Task<(bool, string)> build = BasisBundleBuild.GameObjectBundleBuild(
                    Thumbnail(), avatar.GetComponent<BasisAvatar>(), targets);
                WaitFor(build);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        static GameObject Prepare(string version)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                throw new FileNotFoundException("The avatar prefab did not load.", PrefabPath);
            }
            var avatar = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            avatar.name = BundleName;

            // Watari reads the prefab the instance comes from and writes Basis components onto
            // the instance: jiggle rigs for the spring bones, Vixxy controls for the expressions,
            // and the BasisAvatar with the eye position from the VRM look-at offset.
            AvatarConversionPlan plan = AvatarConversionPlanner.Plan(avatar);
            ConversionResult result = AvatarConverter.Apply(plan, avatar, "AliceChan Basis build");
            foreach (var diagnostic in plan.Diagnostics.Concat(result.Diagnostics))
            {
                Debug.Log($"Watari: {diagnostic}");
            }
            Debug.Log($"Watari wrote {result.TotalWritten} components, skipped {result.TotalSkipped}.");

            var basisAvatar = avatar.GetComponent<BasisAvatar>();
            if (basisAvatar == null)
            {
                throw new InvalidOperationException("Watari did not write a BasisAvatar; the conversion report above says why.");
            }
            basisAvatar.BasisBundleDescription ??= new BasisBundleDescription();
            basisAvatar.BasisBundleDescription.AssetBundleName = BundleName;
            basisAvatar.BasisBundleDescription.AssetBundleDescription =
                $"Alice Chan (Uniform ver.) {version}, a VRoid Studio character. CC BY-SA 4.0, https://github.com/yuna0x0/AliceChan";
            return avatar;
        }

        // The SDK takes the icon as a base64 PNG. The VRM thumbnail is the sub-asset texture the
        // importer exposes; it may be non-readable, so it is copied through a render texture.
        static string Thumbnail()
        {
            Texture2D source = AssetDatabase.LoadAllAssetsAtPath(ThumbnailPath)
                .OfType<Texture2D>()
                .FirstOrDefault(t => t.name.IndexOf("thumbnail", StringComparison.OrdinalIgnoreCase) >= 0);
            if (source == null)
            {
                Debug.LogWarning("No thumbnail texture found in the VRM; building without an icon.");
                return string.Empty;
            }
            var render = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(source, render);
            var previous = RenderTexture.active;
            RenderTexture.active = render;
            var copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            copy.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(render);
            byte[] png = copy.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(copy);
            return Convert.ToBase64String(png);
        }

        static void WaitFor(Task<(bool, string)> build)
        {
            void Poll()
            {
                if (!build.IsCompleted)
                {
                    return;
                }
                EditorApplication.update -= Poll;
                if (build.IsFaulted)
                {
                    Debug.LogException(build.Exception);
                    EditorApplication.Exit(1);
                    return;
                }
                var (ok, message) = build.Result;
                Debug.Log($"Basis build {(ok ? "succeeded" : "failed")}: {message}");
                EditorApplication.Exit(ok ? 0 : 1);
            }
            EditorApplication.update += Poll;
        }

        static List<BuildTarget> ParseTargets(string list)
        {
            var targets = new List<BuildTarget>();
            foreach (string name in list.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Enum.TryParse(name.Trim(), out BuildTarget target))
                {
                    throw new ArgumentException($"Unknown build target {name}.");
                }
                targets.Add(target);
            }
            return targets;
        }

        static string Argument(string name, string fallback)
        {
            string[] args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(args, name);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : fallback;
        }
    }
}
