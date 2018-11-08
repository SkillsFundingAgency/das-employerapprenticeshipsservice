using System;

namespace SFA.DAS.EAS.LevyAnalyser.Models
{
    public class Account : IValidateableObject
    {
        public Account(
            long accountId,
            TransactionLine[] transactions, 
            LevyDeclaration[] levyDeclarations)
        {
            AccountId = accountId;
            Transactions = transactions;
            LevyDeclarations = levyDeclarations;
        }

        public long AccountId { get; }
        public TransactionLine[] Transactions { get; }
        public LevyDeclaration[] LevyDeclarations { get; }

        public object Id => AccountId;
    }
}