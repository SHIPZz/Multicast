using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.ServerDataUploader
{
    public interface IAmazonS3Service
    {
        UniTask UploadFileAsync(string localFilePath, string remoteKey, bool makePublic = true, CancellationToken token = default);
        UniTask DeleteObjectsInFolderAsync(string folderPrefix, CancellationToken cancellationToken = default);
        UniTask<Dictionary<string, long>> GetRemoteFileSizes(string folderPrefix, CancellationToken cancellationToken = default);
        UniTask<bool> CheckIfFolderExists(string folderPrefix, CancellationToken cancellationToken = default);
    }
}