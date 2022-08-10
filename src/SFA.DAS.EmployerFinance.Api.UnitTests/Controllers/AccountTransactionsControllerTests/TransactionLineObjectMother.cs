using SFA.DAS.EmployerFinance.Models.Transaction;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    public static class TransactionLineObjectMother
    {
        public static AggregationData Create()
        {
            return new AggregationData
            {
                TransactionLines = (new TransactionLine[]
                {
                    new TransactionLine
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
                }   })
            };
        }
    }
}
