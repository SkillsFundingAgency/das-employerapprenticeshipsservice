using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Models;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyzer.Rules
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
