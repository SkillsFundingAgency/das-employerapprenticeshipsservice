using System;
using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Represents the result of evaluating a specific rule against an account.
    ///     The rule may have a number of messages attached to it<see cref="Messages"/>
    ///     which may add further information about the rule.
    /// </summary>
    public class RuleEvaluationResult : IRuleEvaluationResult
    {
        private readonly List<RuleEvaluationMessage> _messages = new List<RuleEvaluationMessage>();

        public RuleEvaluationResult(string ruleName)
        {
            RuleName = ruleName;
            IsValid = true;
        }

        /// <summary>
        ///     A descriptive, unique name for the rule.
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        ///     Did the rule validate the account without issue?
        /// </summary>
        public bool IsValid { get; private set; }

        public IReadOnlyCollection<RuleEvaluationMessage> Messages => _messages;

        public void AddRuleViolation(string message)
        {
            AddEntry(RuleMessageLevel.Violation, message);
        }

        public void AddRuleWarning(string message)
        {
            AddEntry(RuleMessageLevel.Suspicous, message);
        }

        public void AddRuleInfo(string message)
        {
            AddEntry(RuleMessageLevel.Info, message);
        }

        private void AddEntry(RuleMessageLevel level, string message)
        {
            var result = new RuleEvaluationMessage(level, message);
            _messages.Add(result);
            if (result.Level != RuleMessageLevel.Info)
            {
                IsValid = false;
            }
        }
    }
}