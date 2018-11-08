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

            foreach (var declaration in account.LevyDeclarations.ExcludeInvalidDeclarations().ExcludePeriod12())
            {
                try
                {
                    if (_hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value, declaration.SubmissionDate.Value))
                    {
                        if (account.Transactions.All(txn => txn.SubmissionId.HasValue && txn.SubmissionId != declaration.Id))
                        {
                            validationResult.AddRuleViolation($"The on-time submission {declaration.Id} is missing an associated transaction");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to process transaction - {e.Message}");
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(declaration));
                    throw;
                }
            }
        }
    }
}
