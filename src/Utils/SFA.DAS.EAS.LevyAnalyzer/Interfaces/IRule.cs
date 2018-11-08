using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    public enum ValidationObject
    {
        Account,
        Employer
    }

    /// <summary>
    ///     Represents a single rule that can be asserted.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        ///     Rules are applied at either the account level or the employer level. This dictates
        ///     the type of <see cref="IValidateableObject"></see> passed to <see cref="Validate"/>.
        /// </summary>
        ValidationObject RequiredValidationObject { get; }

        /// <summary>
        ///     A descriptive name that appears in the analyser output.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Apply a single rule to an account. The account will have access to all of the transactions and 
        ///     declarations for the account, both of which will be in date created order. 
        /// </summary>
        /// <param name="validationObject">
        ///     An object that contains the declarations and transactions that are applicable to either the
        ///     account or employer. 
        /// </param>
        /// <param name="validationResult">
        ///     Represents the results of applying this rule. The rule may add messages as info, warnings or violations
        ///     using the appropriate method on <see cref="RuleEvaluationResult"/>. The rule will be considered to violated 
        ///     (i.e the account does not obey the rule) if the rule adds any <see cref="RuleMessageLevel.Violation"/> 
        ///     level messages. 
        /// </param>
        void Validate(IValidateableObject validationObject, RuleEvaluationResult validationResult);
    }
}
