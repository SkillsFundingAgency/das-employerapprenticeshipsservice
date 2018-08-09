namespace SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadResponse
    {
        public byte[] FileData { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }
    }
}