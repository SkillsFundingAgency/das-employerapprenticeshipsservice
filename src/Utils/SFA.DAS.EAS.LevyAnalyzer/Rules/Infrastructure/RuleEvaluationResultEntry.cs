namespace SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure
{
    public class RuleEvaluationResultEntry
    {
        public RuleEvaluationResultEntry(RuleEntryLevel level, string message)
        {
            Message = message;
            Level = level;
        }

        public string Message { get;}
        public RuleEntryLevel Level { get; }
    }
}