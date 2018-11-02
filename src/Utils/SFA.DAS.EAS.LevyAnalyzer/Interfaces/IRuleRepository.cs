using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Contains a collection of all the rules defined by the application.
    /// </summary>
    public interface IRuleRepository
    {
        IEnumerable<RuleEvaluationResult> ApplyAllRules(Account account);
    }
}