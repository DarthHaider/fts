using System;
using System.IO;
using Serilog;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;

namespace ScanHttpServer
{
    public static class FileUtilities
    {
       public static string SaveToTempFile(Stream fileData)
        {
            string tempFileName = Path.GetTempFileName();
            Log.Information("tmpFileName: {tempFileName}", tempFileName);
            try
            {
                using (var fileStream = File.OpenWrite(tempFileName))
                {
                    fileData.CopyTo(fileStream);
                }
                Log.Information("File created Successfully");
                return tempFileName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception caught when trying to save temp file {tempFileName}.", tempFileName);
                return null;
            }
        }

        public static string DownloadToTempFile(string blobName, string blobContainer)
        {
            string tempFileName = Path.GetTempFileName();
            Log.Information("tmpFileName: {tempFileName}", tempFileName);
            try
            {
                string baseStoragePath = "blob.core.usgovcloudapi.net";
                string accountName = Environment.GetEnvironmentVariable("FtsStorageAccountName");
                Console.WriteLine($"Account name: {accountName}");

                string accountKey = Environment.GetEnvironmentVariable("FtsStorageAccountKey");
                Console.WriteLine($"Account key: {accountKey}");

                string path = $"https://{accountName}.{baseStoragePath}/{blobContainer}/{blobName}";

                Uri blobUri = new Uri(path);

                Console.WriteLine("Create BlobBlockCient");
                StorageSharedKeyCredential credential = new StorageSharedKeyCredential(accountName, accountKey);
                BlockBlobClient blockBlobClient = new BlockBlobClient(blobUri, credential);

                //using (var fileStream = File.OpenWrite(tempFileName))
                //{
                //    fileData.CopyTo(fileStream);
                //}
                using (var stream = blockBlobClient.OpenRead())
                {
                    FileStream fileStream = File.OpenWrite(tempFileName);
                    stream.CopyTo(fileStream);
                }
                Log.Information("File created Successfully");

                return tempFileName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception caught when trying to save temp file {tempFileName}.", tempFileName);
                return null;
            }
        }
    }
}
