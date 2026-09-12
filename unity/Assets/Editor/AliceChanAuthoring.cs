using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEditor.SceneManagement;
using UnityEngine;

// Dev-project tooling, not shipped. Regenerates the assets that derive from Models/AliceChan.vrm:
// the prefab variant, the demo sample scene, and the FBX under source/exports.
//   unity run unity -- -executeMethod AliceChanAuthoring.Regenerate
public static class AliceChanAuthoring
{
    const string Package = "Packages/com.yuna0x0.alice-chan";
    const string VrmPath = Package + "/Models/AliceChan.vrm";
    const string PrefabPath = Package + "/Models/AliceChan.prefab";
    const string SampleStaging = Package + "/Samples";
    const string SampleFolder = Package + "/Samples~/Demo";
    const string ScenePath = SampleStaging + "/Demo/AliceChan_Demo.unity";
    // Outside the Unity project: the repository keeps the source and its exports in source/.
    static readonly string FbxPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "source", "exports", "AliceChan.fbx"));

    [MenuItem("Tools/Alice Chan/Regenerate derived assets")]
    public static void Regenerate()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(VrmPath);
        if (model == null)
        {
            throw new FileNotFoundException("The VRM did not import.", VrmPath);
        }

        foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            Debug.Log($"{renderer.name}: rootBone = {(renderer.rootBone ? renderer.rootBone.name : "none")}, bones = {renderer.bones.Length}");
        }

        // Prefab variant. Saving a linked instance of the imported prefab produces a variant.
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        instance.name = "AliceChan";
        PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
        Object.DestroyImmediate(instance);
        var variant = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Debug.Log($"Prefab variant: {PrefabPath}, type {PrefabUtility.GetPrefabAssetType(variant)}");

        // Demo scene. The AssetDatabase does not see folders ending in a tilde, so the scene is
        // saved under Samples/ and moved into Samples~/ with its .meta afterwards.
        Directory.CreateDirectory(SampleStaging + "/Demo");
        AssetDatabase.Refresh();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var avatar = (GameObject)PrefabUtility.InstantiatePrefab(variant);
        avatar.name = "AliceChan";
        var camera = Camera.main;
        camera.transform.position = new Vector3(0f, 1.2f, 2.2f);
        camera.transform.rotation = Quaternion.Euler(5f, 180f, 0f);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Directory.CreateDirectory(SampleFolder);
        foreach (string file in Directory.GetFiles(SampleStaging + "/Demo"))
        {
            string target = Path.Combine(SampleFolder, Path.GetFileName(file));
            File.Delete(target);
            File.Move(file, target);
        }
        AssetDatabase.DeleteAsset(SampleStaging);
        Debug.Log($"Scene: {SampleFolder}/AliceChan_Demo.unity");

        // FBX for engines that read no VRM. Exported from the imported model, so it carries the
        // humanoid rig and the blendshapes.
        Directory.CreateDirectory(Path.GetDirectoryName(FbxPath)!);
        var exportInstance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        exportInstance.name = "AliceChan";
        var options = new ExportModelOptions
        {
            ExportFormat = ExportFormat.Binary,
            ModelAnimIncludeOption = Include.Model,
            ObjectPosition = ObjectPosition.LocalCentered,
        };
        string exported = ModelExporter.ExportObjects(FbxPath, new Object[] { exportInstance }, options);
        Object.DestroyImmediate(exportInstance);
        Debug.Log($"FBX: {exported}");

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        AssetDatabase.SaveAssets();
    }
}
