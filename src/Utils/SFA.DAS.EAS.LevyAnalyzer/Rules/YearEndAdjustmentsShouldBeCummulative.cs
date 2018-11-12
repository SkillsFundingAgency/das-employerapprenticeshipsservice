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
            var declarations = employer.LevyDeclarations
                                    .YearEndAdjustments(_hmrcDateService)
                                    .OrderBy(declaration => declaration.SubmissionDate);

            foreach (var declaration in declarations.CalculateMonthlyValues())
            {
                var matchingTransaction = employer.Transactions.First(transaction =>
                    transaction.SubmissionId == declaration.CurrentDeclaration.SubmissionId && transaction.EmpRef == employer.Id.ToString());

                if (matchingTransaction.LevyDeclared != declaration.CalculatedLevyAmountForMonth)
                {
                    validationResult.AddRuleViolation($"The levy declared for period {declaration.CurrentDeclaration.PayrollYear} / {declaration.CurrentDeclaration.PayrollMonth} should be {declaration.CalculatedLevyAmountForMonth} but is actually {matchingTransaction.LevyDeclared}");
                }
            }
        }
    }
}
