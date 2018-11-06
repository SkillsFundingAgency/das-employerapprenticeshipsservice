using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
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
                    validationResult.AddRuleInfo($"Submission {declaration.Id} for period {declaration.PayrollMonth} year {declaration.PayrollYear} was submitted late on {declaration.SubmissionDate}");

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
