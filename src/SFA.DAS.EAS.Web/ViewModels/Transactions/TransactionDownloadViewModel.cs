using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;

namespace SFA.DAS.EAS.Web.ViewModels.Transactions
{
    public class TransactionDownloadViewModel
    {
        [Required]
        public GetTransactionsDownloadQuery GetTransactionsDownloadQuery { get; set; }
    }
}