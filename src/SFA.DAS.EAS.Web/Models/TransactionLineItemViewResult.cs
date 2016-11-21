using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Web.Models
{
    public class TransactionLineItemViewResult
    {
        public Account Account   { get; set; }
        public TransactionLineItemViewModel Model { get; set; }
    }
}