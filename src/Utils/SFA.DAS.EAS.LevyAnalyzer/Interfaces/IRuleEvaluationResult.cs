using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Represents the result of applying a single rule to a single account.
    /// </summary>
    public interface IRuleEvaluationResult
    {
        string RuleName { get; }

        /// <summary>
        ///     Indicates whether the account obeys the rule. This will be false if 
        ///     <see cref="Messages"/> contains any <see cref="RuleMessageLevel.Violation"/> messages.
        /// </summary>
        bool IsValid { get; }

        IReadOnlyCollection<RuleEvaluationMessage> Messages { get; }
    }
}