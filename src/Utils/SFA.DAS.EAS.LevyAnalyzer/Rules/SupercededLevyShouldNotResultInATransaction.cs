using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    public class SupercededLevyShouldNotResultInATransaction : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public SupercededLevyShouldNotResultInATransaction(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(SupercededLevyShouldNotResultInATransaction);

        public void Validate(Account account, RuleEvaluationResult validationResult)
        {
            var x = account.LevyDeclarations
                .ExcludePeriod12()
                .ExcludeLateSubmissions(_hmrcDateService)
                .GroupByPayrollPeriod();

            var periodsWithMultipleOntimeSubmissions = account.LevyDeclarations
                .ExcludePeriod12()
                .ExcludeLateSubmissions(_hmrcDateService)
                .GroupByPayrollPeriod()
                .Where(grp => grp.Count() > 1)
                .ToArray();

            if (periodsWithMultipleOntimeSubmissions.Length == 0)
            {
                return;
            }

            var submissionsThatHaveBeenSupercededButStillHaveATransaction = periodsWithMultipleOntimeSubmissions
                            .SelectMany(grp => grp.OrderByDescending(declaration => declaration.SubmissionDate).Skip(1))
                            .Where(declaration => account.Transactions.Any(transaction => transaction.SubmissionId == declaration.SubmissionId));

            foreach (var declaration in submissionsThatHaveBeenSupercededButStillHaveATransaction)
            {
                validationResult.AddRuleViolation($"Submission {declaration.SubmissionId} for period {declaration.PayrollYear}/{declaration.PayrollMonth} was superseded but there is a transaction for this.");
            }
        }
    }
}
