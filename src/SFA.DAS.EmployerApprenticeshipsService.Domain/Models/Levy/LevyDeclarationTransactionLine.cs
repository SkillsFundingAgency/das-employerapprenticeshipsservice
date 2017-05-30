using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class LevyDeclarationTransactionLine : TransactionLine
    {
        public long SubmissionId { get; set; }
        public string EmpRef { get; set; }
        public string PayeSchemeName { get; set; }
        public decimal EnglishFraction { get; set; }
        public decimal TopUp { get; set; }
        public decimal LineTotal { get; set; }
        public decimal LineAmount { get; set; }

        public ICollection<LevyDeclarationTransactionLine> SubLevyDeclarationTransactions =>
            SubTransactions?.OfType<LevyDeclarationTransactionLine>().ToList() ??
            new List<LevyDeclarationTransactionLine>();
    }
}
