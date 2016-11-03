using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class TransactionLine
    {
        public long AccountId { get; set; }
        public long SubmissionId { get; set; }
        public Guid? PaymentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public LevyItemType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string EmpRef { get; set; }
    }
}
