using Microsoft.AspNetCore.Components;

namespace FileTransferService.Spa.Components
{
    public class UploadProgressHandler : IProgress<long>
    {
        public long FileSize { get; set; }

        public void Report(long value)
        {
            Console.WriteLine($"Raw value from report: {value}");
            Console.WriteLine($"File size from report: {FileSize}");
            decimal decValue = value;
            decimal decFileSize = FileSize;

            decimal rawCalculatedValue = decValue / decFileSize * 100;
            Console.WriteLine($"rawCalculatedValue: {rawCalculatedValue}");

            int calculatedPercentage = (int)Math.Floor(rawCalculatedValue);
            Console.WriteLine($"calculatedPercentage: {rawCalculatedValue}");

            ProgressUpdatedEventArgs progressUpdatedEventArgs = new ProgressUpdatedEventArgs
            {
                NumericValue = calculatedPercentage,
                FormattedStringValue = $"{calculatedPercentage}%"
            };
            OnProgressUpdated(progressUpdatedEventArgs);
        }

        protected virtual void OnProgressUpdated(ProgressUpdatedEventArgs e)
        {
            if(ProgressUpdated != null)
            {
                ProgressUpdatedEventHandler handler = ProgressUpdated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        public event ProgressUpdatedEventHandler? ProgressUpdated;

    }

    public class ProgressUpdatedEventArgs
    {
        public int NumericValue { get; set; } = 0;
        public string FormattedStringValue { get; set; } = "0%";
    }

    public delegate void ProgressUpdatedEventHandler(Object sender, ProgressUpdatedEventArgs e);
}
