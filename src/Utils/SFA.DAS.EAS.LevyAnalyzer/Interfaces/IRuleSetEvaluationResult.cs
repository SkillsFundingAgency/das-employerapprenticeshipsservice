using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Represents the result of applying multiple rules to a single account.
    /// </summary>
    public interface IRuleSetEvaluationResult
    {
        IReadOnlyCollection<IRuleEvaluationResult> RuleEvaluationResults { get; }

        bool IsValid { get; }

        IReadOnlyCollection<DeclarationSummary> DeclarationSummaries { get; }
    }
}