using System;
using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransactionLineViewModel<T> where T : TransactionLine
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public List<T> SubTransactions { get; set; }
    }
}