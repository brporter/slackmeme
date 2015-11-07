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
    {
        const string ContainerName = "memes";
        const string DevConnectionString = "UseDevelopmentStorage=true";

        readonly CloudStorageAccount _account;
        readonly CloudBlobClient _client;
        readonly CloudBlobContainer _container;
        readonly bool _failedInitialization = true;

        public ImageStore()
        {
            _account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _client = _account.CreateCloudBlobClient();
            _container = _client.GetContainerReference(ContainerName);

            _container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
        }

        public string GetUrl(string imageIdentifier)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);
            return blob.Uri.ToString();
        }

        public bool Exists(string imageIdentifier)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);
            return blob.Exists();
        }

        public void StoreImage(string imageIdentifier, Stream imageStream)
        {
            var blob = _container.GetBlockBlobReference(imageIdentifier);

            if (!blob.Exists())
            {
                blob.UploadFromStream(imageStream);
            }
        }
    }
}
