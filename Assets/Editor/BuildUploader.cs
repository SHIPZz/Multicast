using System.IO;
using CodeBase.Common.Services.ServerDataUploader;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildUploader : EditorWindow
    {
        private const string AccessKey = "YCAJENApVJ026YJRwoFdYtSs0";
        private const string SecretKey = "YCP0k2fqL5JXVv9toc_wOGyp6FxG1guXt2SPk1KU";
        private const string BucketName = "multicasttest";
        private const string RemoteFolder = "Build/";
        private const string ServiceUrl = "https://storage.yandexcloud.net";

        private static readonly IAmazonS3Service _s3Service = new AmazonS3Service(AccessKey, SecretKey, BucketName, ServiceUrl);

        public static async UniTask UploadApkAsync(string apkPath)
        {
            if (!File.Exists(apkPath))
            {
                Debug.LogError($"❌ APK file not found: {apkPath}");
                return;
            }

            string platformFolder = EditorUserBuildSettings.activeBuildTarget.ToString();
            string fileName = Path.GetFileName(apkPath);
            string key = $"{RemoteFolder}{platformFolder}/{fileName}";

            Debug.Log($"📦 Output path from build: {apkPath}");
            Debug.Log($"🗑 Deleting existing APK in {platformFolder}...");

            await _s3Service.DeleteObjectsInFolderAsync($"{RemoteFolder}{platformFolder}/");

            bool folderExists = await _s3Service.CheckIfFolderExists($"{RemoteFolder}{platformFolder}");

            if (!folderExists)
            {
                Debug.Log($"ℹ Folder {platformFolder} doesn't exist. Creating...");
            }

            Debug.Log($"⬆ Uploading APK: {fileName} → {key}");
            await _s3Service.UploadFileAsync(apkPath, key);

            Debug.Log("🎉 APK uploaded successfully.");
        }
    }
}
