using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public class BuildCoordinator : EditorWindow
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(HandleBuild);
        }

        private static void HandleBuild(BuildPlayerOptions options)
        {
            BuildReport report = BuildPipeline.BuildPlayer(options);
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("✅ Build succeeded, starting uploads...");
                
                ApkUploader.UploadApkAsync(report.summary.outputPath).Forget();
                
                AddressablesTools.AddressablesUploader.UploadAddressablesAsync().Forget();
            }
            else
            {
                Debug.LogError($"❌ Build failed: {report.summary.result}");
            }
        }
    }
} 