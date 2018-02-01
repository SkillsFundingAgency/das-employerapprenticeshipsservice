using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Domain.Models.Transaction;
using ValidationResult = SFA.DAS.EAS.Application.Validation.ValidationResult;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public class GetTransactionsDownloadResponse
    {
        public byte[] FileDate { get; set; }

        public string MimeType { get; set; }

        public string FileExtension { get; set; }

        public ValidationResult ValidationResult { get; set; }
    }
}