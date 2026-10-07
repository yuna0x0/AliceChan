using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("yuna0x0.AliceChan.Editor.Tests")]

namespace yuna0x0.AliceChan.Editor
{
    // The package does not declare UniVRM as a UPM dependency, because a registry dependency
    // breaks git URL and VPM installs in projects without the OpenUPM scoped registry. This logs
    // once per editor session when the importer for the .vrm file is missing.
    [InitializeOnLoad]
    internal static class DependencyCheck
    {
        static DependencyCheck()
        {
#if !ALICECHAN_HAS_VRM10
            Debug.LogWarning(
                "Alice Chan: UniVRM 0.131.2 or newer (com.vrmc.vrm) is not installed, so " +
                "Models/AliceChan.vrm cannot be imported. Install it from OpenUPM " +
                "(openupm add com.vrmc.vrm) or from https://github.com/vrm-c/UniVRM/releases.");
#endif
        }
    }
}
