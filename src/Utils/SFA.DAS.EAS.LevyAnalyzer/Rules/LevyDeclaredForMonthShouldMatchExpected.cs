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
    ///     Validates that the stored levy declared for the month matches the expected value.
    /// </summary>
    public class LevyDeclaredForMonthShouldMatchExpected : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public LevyDeclaredForMonthShouldMatchExpected(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(LevyDeclaredForMonthShouldMatchExpected);

        public ValidationObject RequiredValidationObject => ValidationObject.Employer;

        public void Validate(IValidateableObject employer, RuleEvaluationResult validationResult)
        {
            var declarations = employer.LevyDeclarations
                                    .ActiveDeclarations(_hmrcDateService)
                                    .ExcludeEndOfYearAdjustments(_hmrcDateService)
                                    .OrderBy(declaration => declaration.SubmissionDate);

            foreach (var declaration in declarations.CalculateMonthlyValues())
            {
                if (!employer.TryGetMatchingTransaction(declaration.CurrentDeclaration, out var matchingTransaction))
                {
                    validationResult.AddRuleViolation($"{declaration.CurrentDeclaration.EmpRef}: Levy {declaration.CurrentDeclaration.SubmissionId} for period {declaration.CurrentDeclaration.PayrollYear} / {declaration.CurrentDeclaration.PayrollMonth} does not have a matching transaction.", declaration.CurrentDeclaration.EmpRef, declaration.CurrentDeclaration.CreatedDate, null, null);
                    continue;
                }

                if (matchingTransaction.LevyDeclared != declaration.CalculatedLevyAmountForMonth)
                {
                    validationResult.AddRuleViolation($"{declaration.CurrentDeclaration.EmpRef}: The levy {declaration.CurrentDeclaration.SubmissionId} declared for period {declaration.CurrentDeclaration.PayrollYear} / {declaration.CurrentDeclaration.PayrollMonth} should be {declaration.CalculatedLevyAmountForMonth} but is actually {matchingTransaction.LevyDeclared}", declaration.CurrentDeclaration.EmpRef, matchingTransaction.TransactionDate, declaration.CalculatedLevyAmountForMonth, matchingTransaction.LevyDeclared);
                }
            }
        }
    }
}
