using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TransactionLineViewModel<T> where T : TransactionLine
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public List<T> SubTransactions { get; set; }
    }
}