namespace FileTransferService.Web.Models
{
    public class AzureStorageConfiguration
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string UploadContainer { get; set; }
        public string DestinationStorageAccount { get; set; }
    }
}
