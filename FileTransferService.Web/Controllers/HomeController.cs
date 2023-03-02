using FileTransferService.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace FileTransferService.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureStorageConfiguration _configuration;

        public HomeController(IOptions<AzureStorageConfiguration> configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadFile)
        {
            string fileName = uploadFile.FileName;
            string baseStoragePath = "blob.core.usgovcloudapi.net";
            string accountName = _configuration.AccountName;
            string uploadContainer = _configuration.UploadContainer;
            string path = $"https://{accountName}.{baseStoragePath}/{uploadContainer}/{fileName}";

            Uri blobUri = new Uri(path);

            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(_configuration.AccountName, _configuration.AccountKey);

            BlobClient client = new BlobClient(blobUri, credential);

            using(Stream uploadStream = uploadFile.OpenReadStream())
            {
                if (uploadStream != null)
                {
                    await client.UploadAsync(uploadStream);
                }
            }

            bool uploadSuccessFul = await Task.FromResult(true);

            return uploadSuccessFul ? Ok() : BadRequest();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}