using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TransactionDownloadViewModel : ViewModel<GetTransactionsDownloadQuery>
    {
        public string AccountHashedId { get; set; }

        public TransactionsDownloadStartDateMonthYearDateTime StartDate { get; set; } =
            new TransactionsDownloadStartDateMonthYearDateTime();

        public TransactionsDownloadEndDateMonthYearDateTime EndDate { get; set; } =
            new TransactionsDownloadEndDateMonthYearDateTime();
        
        public void SetMessageFromViewModel()
        {
            Message.StartDate = StartDate;
            Message.EndDate = EndDate;
        }

        public bool Valid => Message.StartDate.Valid && Message.EndDate.Valid;
    }
}