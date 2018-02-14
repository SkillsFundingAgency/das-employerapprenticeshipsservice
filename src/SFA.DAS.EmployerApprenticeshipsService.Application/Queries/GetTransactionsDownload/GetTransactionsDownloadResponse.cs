namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadResponse
    {
        public byte[] FileData { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }
    }
}