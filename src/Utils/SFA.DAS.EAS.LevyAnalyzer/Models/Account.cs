namespace SFA.DAS.EAS.LevyAnalyzer.Models
{
    public class Account
    {
        public Account(
            TransactionLine[] transactions, 
            LevyDeclaration[] levyDeclarations)
        {
            Transactions = transactions;
            LevyDeclarations = levyDeclarations;
        }

        public TransactionLine[] Transactions { get; }
        public LevyDeclaration[] LevyDeclarations { get; }
    }
}