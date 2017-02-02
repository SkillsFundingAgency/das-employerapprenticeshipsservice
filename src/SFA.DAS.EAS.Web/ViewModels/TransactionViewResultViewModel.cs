using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TransactionViewResultViewModel
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
    }
}