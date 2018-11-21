using System.Collections.Generic;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    public class RuleRepository : IRuleRepository
    {
        private readonly IRule[] _allRules;
        private readonly IDeclarationSummaryFactory _declarationSummaryFactory;

        public RuleRepository(IRule[] allRules, IDeclarationSummaryFactory declarationSummaryFactory)
        {
            _allRules = allRules;
            _declarationSummaryFactory = declarationSummaryFactory;
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

            if (!result.IsValid)
            {
                AddDeclarationSummaries(result, account);
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

        private void AddDeclarationSummaries(RuleSetEvaluationResult ruleSetEvaluationResult, Account account)
        {
            for (int i = 0; i < account.LevyDeclarations.Length; i++)
            {
                var declarationSummary = _declarationSummaryFactory.Create(account, i);
                ruleSetEvaluationResult.AddDeclarationSummary(declarationSummary);
            }
        }
    }
}