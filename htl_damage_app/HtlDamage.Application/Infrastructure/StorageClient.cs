using Azure.Storage.Blobs;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Infrastructure
{
    public class StorageClient
    {
        private readonly string _connStr;

        public StorageClient(string connStr)
        {
            _connStr = connStr;
        }

        private static readonly Dictionary<string, string> ContentTypes = new()
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
        };

        public record BlobResult(Guid ImageId, string Link);

        public async Task<BlobResult> UploadFileToAzure(string containerName, string filename, byte[] content)
        {
            var imageId = Guid.NewGuid();
            var blobName = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + Path.GetExtension(filename);
            if (!ContentTypes.TryGetValue(Path.GetExtension(filename), out var mimeType))
            {
                throw new ApplicationException("Invalid file extension.");
            }
            // _blobConnectionStr is a member variable, initialized in the constructor
            var container = new BlobContainerClient(_connStr, containerName);
            if (!await container.ExistsAsync())
            {
                container.Create(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }

            var blob = container.GetBlobClient(blobName);
            using var stream = new MemoryStream(content);
            await blob.UploadAsync(
                stream,
                new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = mimeType },
                new Dictionary<string, string>()
                {
            { "image_id", imageId.ToString("N") }
                });
            return new BlobResult(ImageId: imageId, Link: $"{container.Uri}/{blobName}");
        }
    }
}
