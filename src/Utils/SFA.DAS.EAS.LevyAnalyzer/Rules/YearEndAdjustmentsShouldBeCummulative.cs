using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    /// <summary>
    ///     Validates that year end adjustments are cumulative, regardless of what is used for a period 12 value.
    /// </summary>
    /// <remarks>
    ///     Cumulative means that an adjustment adjusts prior adjustments, not replaces.
    ///     Example:
    ///         P12 declaration  £200
    ///         P12 adjustment 1 £300
    ///         P12 adjustment 2 £350
    ///     Should have transactions £200, £100 and £50, not £200, £100 and £150.
    /// </remarks>
    public class YearEndAdjustmentsShouldBeCummulative : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public YearEndAdjustmentsShouldBeCummulative(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(YearEndAdjustmentsShouldBeCummulative);

        public ValidationObject RequiredValidationObject => ValidationObject.Employer;

        public void Validate(IValidateableObject employer, RuleEvaluationResult validationResult)
        {
            var yearEndAdjustments = employer.LevyDeclarations.CalculateYearEndAdjustments(_hmrcDateService).ToArray();

            validationResult.AddRuleInfo($"Account has {yearEndAdjustments.Length} year end adjustments");

            foreach (var yearEndAdjustment in yearEndAdjustments)
            {
                if (!employer.TryGetMatchingTransaction(yearEndAdjustment.CurrentDeclaration,
                    out var matchingTransaction))
                {
                    validationResult.AddRuleViolation($"{yearEndAdjustment.CurrentDeclaration.EmpRef}: The year end adjustment {yearEndAdjustment.CurrentDeclaration.SubmissionId} does not have a corresponding transaction");
                    continue;
                }

                if (matchingTransaction.LevyDeclared != yearEndAdjustment.CalculatedAdjustment)
                {
                    validationResult.AddRuleViolation(
                        $"{yearEndAdjustment.CurrentDeclaration.EmpRef}: The year end adjustment {yearEndAdjustment.CurrentDeclaration.SubmissionId} declared for period {yearEndAdjustment.CurrentDeclaration.PayrollYear} / {yearEndAdjustment.CurrentDeclaration.PayrollMonth} should be {yearEndAdjustment.CalculatedAdjustment} but is actually {matchingTransaction.LevyDeclared}");
                }
            }
        }
    }
}
