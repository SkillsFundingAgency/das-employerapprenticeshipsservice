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

        public IEnumerable<RuleEvaluationResult> ApplyAllRules(Account account)
        {
            Employer[] employersInAccount = null;

            foreach (var rule in _allRules)
            {
                switch (rule.RequiredValidationObject)
                {
                    case ValidationObject.Account:
                        yield return ApplyRule(account, rule);
                        break;

                    case ValidationObject.Employer:
                        employersInAccount = employersInAccount ?? account.SeperateEmployers();

                        foreach (var employer in employersInAccount)
                        {
                            yield return ApplyRule(employer, rule);
                        }

                        break;
                }
            }
        }

        private RuleEvaluationResult ApplyRule(IValidateableObject account, IRule rule)
        {
            var result = new RuleEvaluationResult(rule.Name, rule.RequiredValidationObject, account.Id);
            rule.Validate(account, result);
            return result;
        }
    }
}