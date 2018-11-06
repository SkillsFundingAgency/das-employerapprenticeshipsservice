using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Represents the validation results for the whole run.
    /// </summary>
    public class AllAccountValidationResult
    {
        private readonly List<AccountValidationResult> _accountResults = new List<AccountValidationResult>();

        public string AccountIds { get; set; }
        public bool IsValid => Accounts != null && Accounts.All(account => account.IsValid);
        public IReadOnlyCollection<AccountValidationResult> Accounts => _accountResults;

        public void AddResult(AccountValidationResult accountResult)
        {
            _accountResults.Add(accountResult);
        }
    }
}