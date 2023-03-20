using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FileTransferService.Functions
{
    public class ScanUploadedFile
    {
        private HttpClient httpClient = new HttpClient();

        [FunctionName("ScanUploadedFile")]
        public async Task Run([BlobTrigger("new-files/{name}", Connection = "uploadstorage_conn")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var scannerHost = Environment.GetEnvironmentVariable("windowsdefender_host");
            var scannerPort = Environment.GetEnvironmentVariable("windowsdefender_port");
            var scanresultprocessorUri = Environment.GetEnvironmentVariable("scanresultprocessor_uri");

            log.LogInformation($"Scanner Host: {scannerHost} and Scanner Port: {scannerPort}");

            ScannerProxy scanner = new ScannerProxy(log, scannerHost);
            ScanResults scanResults = scanner.Scan(myBlob, name);

            if(scanResults == null) 
            {
                log.LogInformation("Error: failure to acquire scan results.");
                return;
            }
            log.LogInformation($"Scan Results isThreat: {scanResults.isThreat.ToString()} and threatType: {scanResults.threatType}");

            FileInfo fileInfo = new FileInfo()
            {
                fileName = name,
                containerName = "general",
                groupName = "root",
                impactLevel = EnvironmentImpactLevel.IL5,
                isThreat = scanResults.isThreat,
                threatType = scanResults.threatType
            };

            string jsonFileInfo = JsonSerializer.Serialize(fileInfo);
            log.LogInformation($"jsonFileInfo: {jsonFileInfo}");

            StringContent payload = new StringContent(jsonFileInfo, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(scanresultprocessorUri, payload);
            Stream responseStream = response.Content.ReadAsStream();
            StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);

            log.LogInformation($"Response Message: {responseStreamReader.ReadToEnd()}");
        }
    }
}
