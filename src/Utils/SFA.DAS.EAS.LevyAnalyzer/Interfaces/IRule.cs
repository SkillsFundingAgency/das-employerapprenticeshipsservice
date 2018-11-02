using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Represents a single rule that can be asserted.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        ///     A descriptive name that appears in the analyser output.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Apply a single rule to an account. The account will have access to all of the transactions and 
        ///     declarations for the account, both of which will be in date created order. 
        /// </summary>
        /// <param name="account">
        ///     The account with all transactions and declarations.
        /// </param>
        /// <param name="validationResult">
        ///     Represents the results of applying this rule. The rule may add messages as info, warnings or violations
        ///     using the appropriate method on <see cref="RuleEvaluationResult"/>. The rule will be considered to violated 
        ///     (i.e the account does not obey the rule) if the rule adds any <see cref="RuleEntryLevel.Violation"/> 
        ///     level messages. 
        /// </param>
        void Validate(Account account, RuleEvaluationResult validationResult);
    }
}
