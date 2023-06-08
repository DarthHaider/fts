using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Blobs;

namespace FileTransferService.Functions
{
    public class ProcessCleanFile
    {

        [FunctionName("ProcessCleanFile")]
        public async Task Run([ActivityTrigger] string blobName, ILogger log) 
        {
            string baseStoragePath = "blob.core.usgovcloudapi.net";
            string destAccountName = Environment.GetEnvironmentVariable("destinationstorage_name");
            string destAccountSas = Environment.GetEnvironmentVariable("destinationstorage_sas");
            string destContainer = Environment.GetEnvironmentVariable("destinationstorage_sas");
            
            string srcAccountName = Environment.GetEnvironmentVariable("uploadstorage_name");
            string srcContainer = Environment.GetEnvironmentVariable("cleanfiles_container");
            string srcContainerSas = Environment.GetEnvironmentVariable("cleanfiles_container_sas");

            int fileNameStartIndex = 38;
            string destFileName = blobName.Substring(fileNameStartIndex, blobName.Length);

            string destPath = $"https://{destAccountName}.{baseStoragePath}/{destContainer}/{destFileName}";
            string srcPath = $"https://{srcAccountName}.{baseStoragePath}/{srcContainer}/{blobName}";

            Uri destUri = new Uri(destPath);
            Uri srcUri = new Uri(srcPath);
            Uri srcUriWithSas = new Uri($"{srcPath}?{srcContainerSas}");

            AzureSasCredential credential = new AzureSasCredential(destAccountSas);

            BlobClient destClient = new BlobClient(destUri, credential);
            await destClient.StartCopyFromUriAsync(srcUriWithSas);

            BlobClient srcClient = new BlobClient(srcUri, credential);
            await srcClient.DeleteAsync();
        }
    }
}