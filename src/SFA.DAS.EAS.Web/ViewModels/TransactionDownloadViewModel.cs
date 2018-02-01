using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TransactionDownloadViewModel : ViewModel<GetTransactionsDownloadQuery>
    {
        public string AccountHashedId { get; set; }

        public TransactionsDownloadStartDateMonthYearDateTime StartDate
        {
            get
            {
                if (Message != null)
                {
                    return Message.StartDate;
                }

                return null;
            }
            set
            {
                if (Message == null)
                {
                    Message = new GetTransactionsDownloadQuery();
                }

                Message.StartDate = value;
            }
        }

        public TransactionsDownloadEndDateMonthYearDateTime EndDate
        {
            get
            {
                if (Message != null)
                {
                    return Message.EndDate;
                }

                return null;
            }
            set
            {
                if (Message == null)
                {
                    Message = new GetTransactionsDownloadQuery();
                }

                Message.EndDate = value;
            }
        }

        public bool Valid => Message.StartDate.Valid && Message.EndDate.Valid;
    }
}