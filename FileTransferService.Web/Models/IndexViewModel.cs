namespace FileTransferService.Web.Models
{
    public class IndexViewModel
    {
        public long MaxBufferedFileSize { get; set; }
        public string DestinationStorageAccount { get; set; } = "";
    }
}
