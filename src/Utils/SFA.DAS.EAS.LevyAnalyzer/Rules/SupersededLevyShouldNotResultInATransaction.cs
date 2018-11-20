using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{

    /// <summary>
    ///     Validates that only the latest on-time submission for a period results in a transaction.
    ///     Only applies to non-period 12.
    /// </summary>
    public class SupersededLevyShouldNotResultInATransaction : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public SupersededLevyShouldNotResultInATransaction(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(SupersededLevyShouldNotResultInATransaction);

        public ValidationObject RequiredValidationObject => ValidationObject.Employer;

        public void Validate(IValidateableObject employer, RuleEvaluationResult validationResult)
        {
            var periodsWithMultipleOntimeSubmissions = employer.LevyDeclarations
                .ExcludeLateSubmissions(_hmrcDateService)
                .GroupByPayrollPeriod()
                .Where(grp => grp.Declarations.Length > 1)
                .ToArray();

            if (periodsWithMultipleOntimeSubmissions.Length == 0)
            {
                validationResult.AddRuleInfo($"Account has no superseded declarations");
                return;
            }

            var submissionsThatHaveBeenSupersededButStillHaveATransaction = periodsWithMultipleOntimeSubmissions
                            .Where(grp => grp.PayrollPeriod.PayrollMonth != 12)
                            .SelectMany(grp => grp.Declarations.Reverse().Skip(1))
                            .Where(declaration => employer.Transactions.Any(transaction => transaction.SubmissionId == declaration.SubmissionId));

            foreach (var declaration in submissionsThatHaveBeenSupersededButStillHaveATransaction)
            {
                validationResult.AddRuleViolation($"Submission {declaration.SubmissionId} for period {declaration.PayrollYear}/{declaration.PayrollMonth} was superseded but there is a transaction for this.");
            }
        }
    }
}
