using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyzer.Rules;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IRuleEvaluationResult
    {
        string RuleName { get; }
        bool IsValid { get; }
        IReadOnlyCollection<RuleEvaluationResultEntry> Messages { get; }
    }
}