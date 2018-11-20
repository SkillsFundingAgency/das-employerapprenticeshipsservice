using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public static class ValidateableObjectExtensions
    {
        public static TransactionLine GetMatchingTransaction(this IValidateableObject validateableObject, LevyDeclaration declaration)
        {
            return validateableObject.Transactions.Single(transaction =>
                transaction.SubmissionId == declaration.SubmissionId);
        }

        public static bool TryGetMatchingTransaction(this IValidateableObject validateableObject, LevyDeclaration declaration, out TransactionLine matchingTransaction)
        {
            matchingTransaction = validateableObject.Transactions.SingleOrDefault(transaction =>
                transaction.SubmissionId == declaration.SubmissionId);

            return matchingTransaction != null;
        }
    }
}
