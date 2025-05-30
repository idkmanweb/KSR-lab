using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using System.IO;

namespace Rot13Shared
{

    public class StorageHelper
    {
        public BlobContainerClient InputContainer { get; }
        public BlobContainerClient OutputContainer { get; }
        public QueueClient Queue { get; }

        public StorageHelper(string connectionString)
        {
            InputContainer = new BlobContainerClient(connectionString, "input");
            OutputContainer = new BlobContainerClient(connectionString, "output");
            Queue = new QueueClient(connectionString, "blobencodequeue");

            InputContainer.CreateIfNotExists();
            OutputContainer.CreateIfNotExists();
            Queue.CreateIfNotExists();
        }

        public async Task UploadToInputContainerAsync(string name, string content)
        {
            var blobClient = InputContainer.GetBlobClient(name);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
        }

        public async Task EnqueueMessageAsync(string name)
        {
            await Queue.SendMessageAsync(name);
        }

        public async Task<string> DownloadFromOutputContainerAsync(string name)
        {
            var blob = OutputContainer.GetBlobClient(name);
            using (var stream = await blob.OpenReadAsync())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

}
