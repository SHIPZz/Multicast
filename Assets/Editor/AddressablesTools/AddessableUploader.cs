using System;
using System.Collections.Generic;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Editor.AddressablesTools
{
    public class AddressablesUploader : EditorWindow
    {
        private const string AccessKey = "YCAJENApVJ026YJRwoFdYtSs0";
        private const string SecretKey = "YCP0k2fqL5JXVv9toc_wOGyp6FxG1guXt2SPk1KU";
        private const string BucketName = "multicasttest";
        private const string RemoteFolder = "Adressables/";
        private const string ServiceUrl = "https://storage.yandexcloud.net";

        [MenuItem("Tools/Addressables/Build & Upload to Yandex")]
        public static void BuildAndUpload()
        {
            Debug.Log("🛠 Building Addressables...");
            AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.BuildPlayerContent();

            UploadAddressablesAsync().Forget();
        }

        private static async UniTask UploadAddressablesAsync()
        {
            string platformName = EditorUserBuildSettings.activeBuildTarget.ToString();
            string buildPath = Path.Combine("ServerData", platformName);

            if (!Directory.Exists(buildPath))
            {
                Debug.LogError($"❌ Addressables build path not found: {buildPath}");
                return;
            }

            var config = new AmazonS3Config
            {
                ServiceURL = ServiceUrl,
            };

            using var s3Client = new AmazonS3Client(AccessKey, SecretKey, config);
            var transferUtility = new TransferUtility(s3Client);

            // Step 1: Delete existing objects in the remote folder
            Debug.Log($"🗑 Deleting existing objects in {RemoteFolder}...");
            await DeleteObjectsInFolderAsync(s3Client);

            // Step 2: Upload new Addressables files
            Debug.Log("📤 Starting upload of new Addressables...");
            string[] localFiles = Directory.GetFiles(buildPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in localFiles)
            {
                string fileName = Path.GetFileName(filePath);
                string key = $"{RemoteFolder}{fileName}";

                Debug.Log($"⬆ Uploading: {fileName} → {key}");

                var request = new TransferUtilityUploadRequest
                {
                    BucketName = BucketName,
                    FilePath = filePath,
                    Key = key,
                    CannedACL = S3CannedACL.PublicRead
                };

                await transferUtility.UploadAsync(request);
                Debug.Log($"✅ Uploaded: {fileName}");
            }

            Debug.Log("🎉 All new Addressables uploaded.");
        }

        private static async UniTask DeleteObjectsInFolderAsync(AmazonS3Client client)
        {
            try
            {
                string continuationToken = null;
                var objectsToDelete = new List<KeyVersion>();

                do
                {
                    var listRequest = new ListObjectsV2Request
                    {
                        BucketName = BucketName,
                        Prefix = RemoteFolder,
                        ContinuationToken = continuationToken
                    };

                    var listResponse = await client.ListObjectsV2Async(listRequest);
                    foreach (var obj in listResponse.S3Objects)
                    {
                        objectsToDelete.Add(new KeyVersion { Key = obj.Key });
                    }

                    continuationToken = listResponse.IsTruncated ? listResponse.NextContinuationToken : null;
                } while (continuationToken != null);

                if (objectsToDelete.Count > 0)
                {
                    var deleteRequest = new DeleteObjectsRequest
                    {
                        BucketName = BucketName,
                        Objects = objectsToDelete
                    };

                    var deleteResponse = await client.DeleteObjectsAsync(deleteRequest);
                    Debug.Log($"✅ Deleted {deleteResponse.DeletedObjects.Count} objects from {RemoteFolder}");

                    if (deleteResponse.DeleteErrors.Count > 0)
                    {
                        foreach (var error in deleteResponse.DeleteErrors)
                        {
                            Debug.LogError($"❌ Failed to delete {error.Key}: {error.Message}");
                        }
                    }
                }
                else
                {
                    Debug.Log($"ℹ No objects found in {RemoteFolder} to delete.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error deleting objects in {RemoteFolder}: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        private static async UniTask<Dictionary<string, long>> GetRemoteFileSizes(AmazonS3Client client)
        {
            var sizes = new Dictionary<string, long>();
            string continuationToken = null;

            try
            {
                do
                {
                    var request = new ListObjectsV2Request
                    {
                        BucketName = BucketName,
                        Prefix = RemoteFolder,
                        ContinuationToken = continuationToken
                    };

                    var response = await client.ListObjectsV2Async(request);
                    foreach (var obj in response.S3Objects)
                    {
                        sizes[obj.Key] = obj.Size;
                    }

                    continuationToken = response.IsTruncated ? response.NextContinuationToken : null;
                } while (continuationToken != null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error fetching remote file sizes: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return sizes;
        }
    }
}