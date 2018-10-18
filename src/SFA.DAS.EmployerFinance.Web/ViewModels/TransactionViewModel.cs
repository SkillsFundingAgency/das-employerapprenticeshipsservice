using SFA.DAS.EmployerFinance.Models.Transaction;
using System;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransactionViewModel
    {
        public decimal CurrentBalance { get; set; }
        public DateTime CurrentBalanceCalcultedOn { get; set; }
        public AggregationData Data { get; set; }
    }
}