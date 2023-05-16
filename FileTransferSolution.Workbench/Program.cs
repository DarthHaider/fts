using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace FileTransferSolution.Workbench
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string fileDir = @"C:\temp\";
            string fileName = "console-test-file.txt";
            string filePath = fileDir + fileName;

            string baseStoragePath = "blob.core.usgovcloudapi.net";
            string accountName = Environment.GetEnvironmentVariable("FtsStorageAccountName");
            Console.WriteLine($"Account name: {accountName}");
            string accountKey = Environment.GetEnvironmentVariable("FtsStorageAccountKey");
            Console.WriteLine($"Account key: {accountKey}");
            string uploadContainer = Environment.GetEnvironmentVariable("FtsUploadContainer");
            Console.WriteLine($"Upload container: {uploadContainer}");
            string path = $"https://{accountName}.{baseStoragePath}/{uploadContainer}/{fileName}";

            Uri blobUri = new Uri(path);

            while (true)
            {
                Console.WriteLine("Select 'y' if you want to execute or 'n' to exit");
                if(Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    if(File.Exists(filePath))
                    {
                        Console.WriteLine("Create BlobBlockCient");
                        StorageSharedKeyCredential credential = new StorageSharedKeyCredential(accountName, accountKey);
                        BlockBlobClient blockBlobClient = new BlockBlobClient(blobUri, credential); 

                        using (Stream uploadStream = File.OpenRead(filePath))
                        {
                            if(uploadStream != null)
                            {
                                Console.WriteLine("Upload stream");
                                await blockBlobClient.UploadAsync(uploadStream);
                            }
                        }
                        bool uploadSuccessFul = await Task.FromResult(true);
                        if(uploadSuccessFul)
                        {
                            Console.WriteLine("Upload successful");
                            Thread.Sleep(1000);
                        }
                    }
                    break;
                }
                else if(Console.ReadKey(true).Key == ConsoleKey.N) 
                {
                    Console.WriteLine("In no");
                    Thread.Sleep(1000);
                    break;
                }
            }
        }
    }
}
