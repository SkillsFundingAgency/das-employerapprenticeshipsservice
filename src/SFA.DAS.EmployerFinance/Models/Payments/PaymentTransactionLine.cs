using SFA.DAS.EmployerFinance.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Models.Payments
{
    public class PaymentTransactionLine : TransactionLine
    {
        public Guid? PaymentId { get; set; }
        public long UkPrn { get; set; }
        public string PeriodEnd { get; set; }
        public string ProviderName { get; set; }
        public decimal LineAmount { get; set; }
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
        public string PathwayName { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public string ApprenticeName { get; set; }
        public string ApprenticeNINumber { get; set; }
        public decimal SfaCoInvestmentAmount { get; set; }
        public decimal EmployerCoInvestmentAmount { get; set; }

        public bool IsCoInvested => SfaCoInvestmentAmount != 0 || EmployerCoInvestmentAmount != 0;

        public ICollection<PaymentTransactionLine> SubPayments =>
            SubTransactions?.OfType<PaymentTransactionLine>().ToList() ??
                new List<PaymentTransactionLine>();
    }
}
