using System;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TransactionViewModel   
    {
        public decimal CurrentBalance { get; set; }
        public DateTime CurrentBalanceCalcultedOn { get; set; }
        public AggregationData Data { get; set; }
    }
}