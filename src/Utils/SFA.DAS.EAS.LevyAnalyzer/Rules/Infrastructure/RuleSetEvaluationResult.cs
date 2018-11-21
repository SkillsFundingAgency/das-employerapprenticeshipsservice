using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    public class RuleSetEvaluationResult : IRuleSetEvaluationResult
    {
        private readonly List<IRuleEvaluationResult> _ruleEvaluationResults;
        private readonly List<DeclarationSummary> _declarationSummaries;
        
        public RuleSetEvaluationResult()
        {
            _ruleEvaluationResults = new List<IRuleEvaluationResult>();    
            _declarationSummaries = new List<DeclarationSummary>();
        }

        public IReadOnlyCollection<IRuleEvaluationResult> RuleEvaluationResults => _ruleEvaluationResults;
        public IReadOnlyCollection<DeclarationSummary> DeclarationSummaries => _declarationSummaries;
        public bool IsValid => _ruleEvaluationResults.All(ruleResult => ruleResult.IsValid);

        public void AddRuleResult(IRuleEvaluationResult ruleEvaluationResult)
        {
            _ruleEvaluationResults.Add(ruleEvaluationResult);
        }

        public void AddDeclarationSummary(DeclarationSummary declarationSummary)
        {
            _declarationSummaries.Add(declarationSummary);
        }
    }
}