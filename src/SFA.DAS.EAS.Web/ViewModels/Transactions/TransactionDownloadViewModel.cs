using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;

namespace SFA.DAS.EAS.Web.ViewModels.Transactions
{
    public class TransactionDownloadViewModel : ViewModel<GetTransactionsDownloadQuery>
    {
        public string HashedAccountId { get; set; }
    }
}