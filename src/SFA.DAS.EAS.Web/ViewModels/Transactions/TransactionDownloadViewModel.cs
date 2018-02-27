using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;

namespace SFA.DAS.EAS.Web.ViewModels.Transactions
{
    public class TransactionDownloadViewModel : ViewModel<GetTransactionsDownloadQuery>
    {
        [Required]
        public GetTransactionsDownloadQuery GetTransactionsDownloadQuery { get; set; }

        public override void Map(GetTransactionsDownloadQuery message)
        {
            GetTransactionsDownloadQuery = message;
        }
    }
}