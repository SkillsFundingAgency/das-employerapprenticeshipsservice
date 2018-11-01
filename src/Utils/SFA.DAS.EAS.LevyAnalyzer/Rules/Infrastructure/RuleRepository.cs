using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Models;

namespace SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure
{
    public class RuleRepository : IRuleRepository
    {
        private readonly IRule[] _allRules;

        public RuleRepository(IRule[] allRules)
        {
            _allRules = allRules;
        }

        public IEnumerable<RuleEvaluationResult> ApplyAllRules(Account account)
        {
            foreach (var rule in _allRules)
            {
                var result = new RuleEvaluationResult(rule.Name);
                rule.Validate(account, result);

                yield return result;
            }
        }
    }
}