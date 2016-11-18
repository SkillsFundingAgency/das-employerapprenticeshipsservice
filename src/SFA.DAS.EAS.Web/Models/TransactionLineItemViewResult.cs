using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Web.Models
{
    public class TransactionLineItemViewResult<T> where T : TransactionLine
    {
        public Account Account   { get; set; }
        public TransactionLineViewModel<T> Model { get; set; }
    }
}