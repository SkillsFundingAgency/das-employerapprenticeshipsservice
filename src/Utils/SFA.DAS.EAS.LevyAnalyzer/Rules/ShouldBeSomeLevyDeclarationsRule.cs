using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    public class ShouldBeSomeLevyDeclarationsRule : IRule
    {
        public string Name => nameof(ShouldBeSomeLevyDeclarationsRule);

        public void Validate(Account account, RuleEvaluationResult validationResult)
        {
            if (account.LevyDeclarations.Length == 0)
            {
                validationResult.AddRuleViolation("The account does not have any levy declarations");
            }

            validationResult.AddRuleInfo($"The account has {account.LevyDeclarations.Length} levy declarations");
        }
    }
}
