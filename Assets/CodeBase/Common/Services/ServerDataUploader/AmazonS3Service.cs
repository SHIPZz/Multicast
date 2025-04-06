using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Common.Services.ServerDataUploader
{
    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _s3Client;
        private readonly TransferUtility _transferUtility;

        public AmazonS3Service(string accessKey, string secretKey, string bucketName, string serviceUrl)
        {
            _bucketName = bucketName;

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, config);
            _transferUtility = new TransferUtility(_s3Client);
        }

        public async UniTask UploadFileAsync(string localFilePath, string remoteKey, bool makePublic = true, CancellationToken token = default)
        {
            var request = new TransferUtilityUploadRequest
            {
                BucketName = _bucketName,
                FilePath = localFilePath,
                Key = remoteKey,
                CannedACL = makePublic ? S3CannedACL.PublicRead : S3CannedACL.Private
            };

            await _transferUtility.UploadAsync(request,token);
            Debug.Log($"✅ Uploaded: {Path.GetFileName(localFilePath)} → {remoteKey}");
        }
        
        public async UniTask<bool> CheckIfFolderExists(string folderPrefix, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = folderPrefix,
                    MaxKeys = 1 
                };

                var response = await _s3Client.ListObjectsV2Async(request, cancellationToken);

                return response.S3Objects.Count > 0; 
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error checking folder existence: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return false;
            }
        }

        public async UniTask DeleteObjectsInFolderAsync(string folderPrefix, CancellationToken cancellationToken = default)
        {
            try
            {
                string continuationToken = null;
                var objectsToDelete = new List<KeyVersion>();

                do
                {
                    var listRequest = new ListObjectsV2Request
                    {
                        BucketName = _bucketName,
                        Prefix = folderPrefix,
                        ContinuationToken = continuationToken
                    };

                    var listResponse = await _s3Client.ListObjectsV2Async(listRequest, cancellationToken);
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
                        BucketName = _bucketName,
                        Objects = objectsToDelete
                    };

                    var deleteResponse = await _s3Client.DeleteObjectsAsync(deleteRequest, cancellationToken);
                    Debug.Log($"✅ Deleted {deleteResponse.DeletedObjects.Count} objects from {folderPrefix}");

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
                    Debug.Log($"ℹ No objects found in {folderPrefix} to delete.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error deleting objects in {folderPrefix}: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        public async UniTask<Dictionary<string, long>> GetRemoteFileSizes(string folderPrefix, CancellationToken cancellationToken = default)
        {
            var sizes = new Dictionary<string, long>();
            string continuationToken = null;

            try
            {
                do
                {
                    var request = new ListObjectsV2Request
                    {
                        BucketName = _bucketName,
                        Prefix = folderPrefix,
                        ContinuationToken = continuationToken
                    };

                    ListObjectsV2Response response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
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