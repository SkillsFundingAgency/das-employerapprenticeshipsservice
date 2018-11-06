namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Represents a message that was added when evaluating a specific rule for an account.
    /// </summary>
    public class RuleEvaluationMessage
    {
        public RuleEvaluationMessage(RuleMessageLevel level, string message)
        {
            Message = message;
            Level = level;
        }

        public string Message { get;}
        public RuleMessageLevel Level { get; }
    }
}