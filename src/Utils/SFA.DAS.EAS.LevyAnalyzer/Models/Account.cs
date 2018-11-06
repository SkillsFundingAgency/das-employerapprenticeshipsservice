namespace SFA.DAS.EAS.LevyAnalyser.Models
{
    public class Account
    {
        public Account(
            long id,
            TransactionLine[] transactions, 
            LevyDeclaration[] levyDeclarations)
        {
            Id = id;
            Transactions = transactions;
            LevyDeclarations = levyDeclarations;
        }

        public long Id { get; }
        public TransactionLine[] Transactions { get; }
        public LevyDeclaration[] LevyDeclarations { get; }
    }
}