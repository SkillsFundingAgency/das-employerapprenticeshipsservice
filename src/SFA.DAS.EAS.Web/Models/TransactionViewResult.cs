using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Models
{
    public class TransactionViewResult
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
    }
}