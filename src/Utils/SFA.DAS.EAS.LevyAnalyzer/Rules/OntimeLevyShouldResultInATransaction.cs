using System.Linq;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Models;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyzer.Rules
{
    public class OntimeLevyShouldResultInATransaction : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public OntimeLevyShouldResultInATransaction(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(OntimeLevyShouldResultInATransaction);

        public void Validate(Account account, RuleEvaluationResult validationResult)
        {
            foreach (var declaration in account.LevyDeclarations)
            {
                if (_hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value, declaration.SubmissionDate.Value))
                {
                    if (account.Transactions.All(txn => txn.SubmissionId != declaration.Id))
                    {
                        validationResult.AddRuleViolation($"The on-time submission {declaration.Id} is missing an associated transaction");
                    }
                }
            }
        }
    }
}
