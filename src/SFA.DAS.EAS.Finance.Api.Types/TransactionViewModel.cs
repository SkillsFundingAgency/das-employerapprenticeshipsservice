using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class TransactionViewModel
    {
        public string Description { get; set; }
        public TransactionItemType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionViewModel> SubTransactions { get; set; }
        public string ResourceUri { get; set; }
    }
}
