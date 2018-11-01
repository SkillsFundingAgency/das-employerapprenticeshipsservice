using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure
{
    public class RuleEvaluationResult : IRuleEvaluationResult
    {
        private readonly List<RuleEvaluationResultEntry> _messages = new List<RuleEvaluationResultEntry>();

        public RuleEvaluationResult(string ruleName)
        {
            RuleName = ruleName;
            IsValid = true;
        }

        public string RuleName { get; }

        public bool IsValid { get; private set; }

        public IReadOnlyCollection<RuleEvaluationResultEntry> Messages => _messages;

        public void AddRuleViolation(string message)
        {
            AddEntry(RuleEntryLevel.Violation, message);
        }

        public void AddRuleWarning(string message)
        {
            AddEntry(RuleEntryLevel.Suspicous, message);
        }

        public void AddRuleInfo(string message)
        {
            AddEntry(RuleEntryLevel.Info, message);
        }

        private void AddEntry(RuleEntryLevel level, string message)
        {
            var result = new RuleEvaluationResultEntry(level, message);
            _messages.Add(result);
            if (result.Level != RuleEntryLevel.Info)
            {
                IsValid = false;
            }
        }
    }
}