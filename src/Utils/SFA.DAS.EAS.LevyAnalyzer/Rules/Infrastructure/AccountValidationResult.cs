using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Represents the validation results for a single account.
    /// </summary>
    public class AccountValidationResult
    {
        public long AccountId { get; set; }
        public bool IsValid => Rules.IsValid;
        public IRuleSetEvaluationResult Rules { get; set; }
    }
}
