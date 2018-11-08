using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.LevyAnalyser.Models
{
    public class Employer : IValidateableObject
    {
        public Employer(
            string empRef, 
            IEnumerable<LevyDeclaration> declarations, 
            IEnumerable<TransactionLine> transactions)
        {
            EmpRef = empRef;
            LevyDeclarations = declarations.ToArray();
            Transactions = transactions.ToArray();
        }

        public string EmpRef { get; }
        public TransactionLine[] Transactions { get; }
        public object Id => EmpRef;
        public LevyDeclaration[] LevyDeclarations { get; }
    }
}