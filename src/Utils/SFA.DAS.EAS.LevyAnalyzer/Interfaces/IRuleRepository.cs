using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyzer.Models;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IRuleRepository
    {
        IEnumerable<RuleEvaluationResult> ApplyAllRules(Account account);
    }
}