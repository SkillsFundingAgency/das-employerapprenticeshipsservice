namespace SFA.DAS.EAS.LevyAnalyser.Models
{
    public interface IValidateableObject
    {
        object Id { get; }
        LevyDeclaration[] LevyDeclarations { get;  }
        TransactionLine[] Transactions { get;  }
    }
}