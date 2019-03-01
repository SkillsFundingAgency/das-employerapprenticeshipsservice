using System;
using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    /// <summary>
    ///     Validates that levy declarations that were received on time result in a transaction.
    ///     Only applies to non-period 12.
    /// </summary>
    public class OntimeLevyShouldResultInATransaction : IRule
    {
        private readonly IHmrcDateService _hmrcDateService;

        public OntimeLevyShouldResultInATransaction(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;
        }

        public string Name => nameof(OntimeLevyShouldResultInATransaction);

        public ValidationObject RequiredValidationObject => ValidationObject.Account;

        public void Validate(IValidateableObject account, RuleEvaluationResult validationResult)
        {

            var nonSupercededOnTimeDeclarations = account.LevyDeclarations
                .ActiveDeclarations(_hmrcDateService)
                .ExcludeEndOfYearAdjustments(_hmrcDateService)
                .ExcludeNullYearToDateValues()
                .OrderBy(declaration => declaration.SubmissionDate);

            foreach (var declaration in nonSupercededOnTimeDeclarations)
            {
                if (declaration.AccountId != (long)account.Id)
                {
                    continue;
                }

                if (!account.TryGetMatchingTransaction(declaration, out _))
                {
                    validationResult.AddRuleViolation($"{declaration.EmpRef}: The on-time submission {declaration.Id} is missing an associated transaction", declaration.EmpRef, declaration.CreatedDate, null, null);
                }
            }
        }
    }
}
