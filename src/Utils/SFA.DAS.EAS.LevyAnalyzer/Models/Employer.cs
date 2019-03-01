using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.LevyAnalyser.Models
{
    public class Employer : IValidateableObject
    {
        public Employer(
            long accountId,
            string empRef, 
            IEnumerable<LevyDeclaration> declarations, 
            IEnumerable<TransactionLine> transactions)
        {
            Id = accountId;
            EmpRef = empRef;
            LevyDeclarations = declarations.ToArray();
            Transactions = transactions.ToArray();
        }

        public string EmpRef { get; }
        public TransactionLine[] Transactions { get; }
        public object Id { get; }
        public LevyDeclaration[] LevyDeclarations { get; }
    }
}