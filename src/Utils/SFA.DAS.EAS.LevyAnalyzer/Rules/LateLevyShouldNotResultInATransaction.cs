using System.Linq;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Models;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyzer.Rules
{
    public class LateLevyShouldNotResultInATransaction : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public LateLevyShouldNotResultInATransaction(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(LateLevyShouldNotResultInATransaction);

        public void Validate(Account account, RuleEvaluationResult validationResult)
        {
            bool foundLate = false;

            foreach (var declaration in account.LevyDeclarations)
            {
                if (!_hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value, declaration.SubmissionDate.Value))
                {
                    foundLate = true;
                    validationResult.AddRuleInfo($"Submission {declaration.Id} for {declaration.PayrollYear}-{declaration.PayrollMonth} was submitted late on {declaration.SubmissionDate}");

                    if (account.Transactions.Any(txn => txn.SubmissionId == declaration.Id))
                    {
                        validationResult.AddRuleViolation($"The late submission {declaration.Id} has an associated transaction");
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
