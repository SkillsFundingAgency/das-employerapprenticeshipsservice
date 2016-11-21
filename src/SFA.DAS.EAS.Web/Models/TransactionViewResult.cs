using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Web.Models
{
    public class TransactionViewResult
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
    }
}