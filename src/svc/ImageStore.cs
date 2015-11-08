using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BryanPorter.SlackMeme.Service
{
    public class ImageStore 
        : IBlobStore
    {
        const string ContainerName = "memes";
        const string DevConnectionString = "UseDevelopmentStorage=true";

        static readonly CloudStorageAccount Account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        readonly CloudBlobClient _client;
        readonly CloudBlobContainer _container;

        public ImageStore()
        {
            _client = Account.CreateCloudBlobClient();
            _container = _client.GetContainerReference(ContainerName);

            _container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
        }

        public Uri GetUri(string imageIdentifier)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);
            return blob.Uri;
        }

        public bool Exists(string imageIdentifier)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);
            return blob.Exists();
        }

        public void Store(string imageIdentifier, Stream imageStream)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);

            if (!blob.Exists())
            {
                blob.UploadFromStream(imageStream);
            }
        }

        public void Delete(string imageIdentifier)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);
            blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
