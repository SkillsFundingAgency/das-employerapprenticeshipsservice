using System;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Represents a message that was added when evaluating a specific rule for an account.
    /// </summary>
    public class RuleEvaluationMessage
    {
        public RuleEvaluationMessage(RuleMessageLevel level, string message, string empRef, DateTime? transactionDate, decimal? expectedAmount, decimal? actualAmount)
        {
            Message = message;
            EmpRef = empRef;
            Level = level;
            ExpectedAmount = expectedAmount;
            ActualAmount = actualAmount;
            TransactionDate = transactionDate;
        }

        public string Message { get;}
        public string EmpRef { get; }
        public RuleMessageLevel Level { get; }
        public decimal? ExpectedAmount { get; }
        public decimal? ActualAmount { get; }
        public DateTime? TransactionDate { get; }
    }
}