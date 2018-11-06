namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Describes the severity of a message. Adding a message with a severity of Violation
    ///     will result in the rule being considered a fail.
    /// </summary>
    public enum RuleMessageLevel
    {
        Info,
        Suspicous,
        Violation
    }
}