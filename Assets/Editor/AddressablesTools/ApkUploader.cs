using System.IO;
using CodeBase.Common.Services.ServerDataUploader;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.AddressablesTools
{
    public class ApkUploader : EditorWindow
    {
        private const string AccessKey = "YCAJENApVJ026YJRwoFdYtSs0";
        private const string SecretKey = "YCP0k2fqL5JXVv9toc_wOGyp6FxG1guXt2SPk1KU";
        private const string BucketName = "multicasttest";
        private const string RemoteFolder = "Apk/";
        private const string ServiceUrl = "https://storage.yandexcloud.net";

        private static readonly IAmazonS3Service _s3Service = new AmazonS3Service(AccessKey, SecretKey, BucketName, ServiceUrl);

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
        }

        private static void BuildPlayerHandler(BuildPlayerOptions options)
        {
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == BuildResult.Succeeded)
            {
                UploadApkAsync(report.summary.outputPath).Forget();
            }
        }

        private static async UniTask UploadApkAsync(string apkPath)
        {
            if (!File.Exists(apkPath))
            {
                Debug.LogError($"❌ APK file not found: {apkPath}");
                return;
            }

            string fileName = Path.GetFileName(apkPath);
            string key = $"{RemoteFolder}{fileName}";

            Debug.Log($"🗑 Deleting existing APK in {RemoteFolder}...");
            await _s3Service.DeleteObjectsInFolderAsync(RemoteFolder);

            Debug.Log($"⬆ Uploading APK: {fileName} → {key}");
            await _s3Service.UploadFileAsync(apkPath, key);

            Debug.Log("🎉 APK uploaded successfully.");
        }
    }
} 