using UnityEditor;
using UnityEngine;

namespace yuna0x0.AliceChan.Editor
{
    // When UniVRM and this package enter a project together, Unity can import AliceChan.vrm
    // before UniVRM's shaders exist. The VRM importer then fails on a null shader and leaves no
    // model behind until the file is imported again. This reimports it once the shader is there.
    [InitializeOnLoad]
    internal static class ImportOrderCheck
    {
        const string VrmPath = "Packages/com.yuna0x0.alice-chan/Models/AliceChan.vrm";
        const string PrefabPath = "Packages/com.yuna0x0.alice-chan/Models/AliceChan.prefab";
        const string ShaderName = "VRM10/MToon10";

        static ImportOrderCheck()
        {
            EditorApplication.delayCall += () => Repair();
        }

        // Returns true when a reimport was needed and done.
        internal static bool Repair()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(VrmPath) != null || Shader.Find(ShaderName) == null)
            {
                return false;
            }
            AssetDatabase.ImportAsset(VrmPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(PrefabPath, ImportAssetOptions.ForceUpdate);
            return true;
        }
    }
}
