using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    public class RuleRepository : IRuleRepository
    {
        private readonly IRule[] _allRules;

        public RuleRepository(IRule[] allRules)
        {
            _allRules = allRules;
        }

        public IRuleSetEvaluationResult ApplyAllRules(Account account)
        {
            Employer[] employersInAccount = null;

            var result = new RuleSetEvaluationResult();

            foreach (var rule in _allRules)
            {
                switch (rule.RequiredValidationObject)
                {
                    case ValidationObject.Account:
                        result.AddRuleResult(ApplyRule(account, rule));
                        break;

                    case ValidationObject.Employer:
                        employersInAccount = employersInAccount ?? account.SeperateEmployers();

                        foreach (var employer in employersInAccount)
                        {
                            result.AddRuleResult(ApplyRule(employer, rule));
                        }

                        break;
                }
            }

            return result;
        }

        public IReadOnlyCollection<IRule> AvailableRules => _allRules;

        private RuleEvaluationResult ApplyRule(IValidateableObject account, IRule rule)
        {
            var result = new RuleEvaluationResult(rule.Name, rule.RequiredValidationObject, account.Id);
            rule.Validate(account, result);
            return result;
        }
    }
}