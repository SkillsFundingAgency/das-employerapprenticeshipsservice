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

        public RuleEvaluationResult(string ruleName, ValidationObject validationObject, object id)
        {
            RuleName = ruleName;
            IsValid = true;
            ValidationObject = validationObject;
            Id = id;
        }

        /// <summary>
        ///     A descriptive, unique name for the rule.
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        ///     The type of object being validated (either account or employer).
        /// </summary>
        public ValidationObject ValidationObject { get; set; }

        /// <summary>
        ///     The id of the object being validated. This is either the account id or the PAYE ref.
        /// </summary>
        public object Id { get; set; }

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