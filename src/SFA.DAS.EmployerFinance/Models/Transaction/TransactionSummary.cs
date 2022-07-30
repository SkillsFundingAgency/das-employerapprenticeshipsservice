using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public class TransactionSummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
    }
}
