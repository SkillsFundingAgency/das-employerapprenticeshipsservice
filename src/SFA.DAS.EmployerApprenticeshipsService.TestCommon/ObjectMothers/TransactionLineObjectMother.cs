using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class TransactionLineObjectMother
    {
        public static TransactionLine Create()
        {
            return new TransactionLine
            {
                AccountId = 123,
                Balance = 500.43m,
                Description = "Line",
                PayrollYear = "17-18",
                PayrollMonth = 3,
                Amount = 100.11m,
                DateCreated = DateTime.Now.AddDays(-1),
                PayrollDate = DateTime.Now.AddDays(-4),
                TransactionDate = DateTime.Now,
                TransactionType = TransactionItemType.Declaration,
                SubTransactions = new List<TransactionLine>()
            };
        }
    }
}
