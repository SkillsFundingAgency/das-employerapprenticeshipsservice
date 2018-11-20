using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    public class RuleSetEvaluationResult : IRuleSetEvaluationResult
    {
        private readonly List<IRuleEvaluationResult> _ruleEvaluationResults;

        
        public RuleSetEvaluationResult()
        {
            _ruleEvaluationResults = new List<IRuleEvaluationResult>();    
        }

        public IReadOnlyCollection<IRuleEvaluationResult> RuleEvaluationResults => _ruleEvaluationResults;
        public bool IsValid => _ruleEvaluationResults.All(ruleResult => ruleResult.IsValid);

        public void AddRuleResult(IRuleEvaluationResult ruleEvaluationResult)
        {
            _ruleEvaluationResults.Add(ruleEvaluationResult);
        }
    }
}