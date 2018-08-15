using System.ComponentModel.DataAnnotations;
using SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransactionDownloadViewModel
    {
        [Required]
        public GetTransactionsDownloadQuery GetTransactionsDownloadQuery { get; set; }
    }
}