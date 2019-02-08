using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    /// <summary>
    ///     Validates that late levy declarations do not result in a transaction.
    ///     Only applies to non-period 12.
    /// </summary>
    public class LateLevyShouldNotResultInATransaction : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public LateLevyShouldNotResultInATransaction(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public ValidationObject RequiredValidationObject => ValidationObject.Account;

        public string Name => nameof(LateLevyShouldNotResultInATransaction);

        public void Validate(IValidateableObject account, RuleEvaluationResult validationResult)
        {
            bool foundLate = false;

            foreach (var declaration in account.LevyDeclarations.ExcludeInvalidDeclarations().ExcludePeriod12())
            {
                if (!_hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value, declaration.SubmissionDate.Value))
                {
                    foundLate = true;
                    validationResult.AddRuleInfo($"{declaration.EmpRef}: Submission {declaration.Id} for period {declaration.PayrollMonth} year {declaration.PayrollYear} was submitted late on {declaration.SubmissionDate}");

                    if (account.Transactions.Any(txn => txn.SubmissionId == declaration.Id))
                    {
                        validationResult.AddRuleViolation($"{declaration.EmpRef}:The late submission {declaration.Id} has an associated transaction");
                    }
                }
            }

            if (!foundLate)
            {
                validationResult.AddRuleInfo($"Account has no late submissions");
            }
        }
    }
}
