using System.Linq;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Represents the validation results for a single account.
    /// </summary>
    public class AccountValidationResult
    {
        public long AccountId { get; set; }
        public bool IsValid => Rules != null && Rules.All(rule => rule.IsValid);
        public RuleEvaluationResult[] Rules { get; set; }
    }
}
