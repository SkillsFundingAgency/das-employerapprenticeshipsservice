using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils
{
    public class LevyAnalyzerTestFixtures
    {

        private readonly Lazy<IRuleRepository> _lazyRuleRepository;


        public LevyAnalyzerTestFixtures()
        {
            HmrcDateServiceMock = new Mock<IHmrcDateService>();    
            Transactions = new List<TransactionLine>();
            Declarations = new List<LevyDeclaration>();
            _lazyRuleRepository = new Lazy<IRuleRepository>(InitialiseRuleRepository);
        }

        public Mock<IHmrcDateService> HmrcDateServiceMock { get; set; }

        public IHmrcDateService HmrcDateService => HmrcDateServiceMock.Object;

        public List<TransactionLine> Transactions { get; set; }

        public List<LevyDeclaration> Declarations { get; set; }

        public string EmpRef { get; private set; }

        public LevyAnalyzerTestFixtures WithEmpRef(string empRef)
        {
            EmpRef = empRef;
            return this;
        }

        public LevyAnalyzerTestFixtures WithLateLevy(long submissionId, string payrollYear, byte payrollMonth, DateTime submissionDate)
        {
            return WithLateLevy(submissionId, payrollYear, payrollMonth, submissionDate, 0);
        }

        public LevyAnalyzerTestFixtures WithOntimeLevy(long submissionId, string payrollYear, byte payrollMonth, DateTime submissionDate)
        {
            return WithOntimeLevy(submissionId, payrollYear, payrollMonth, submissionDate, 0);
        }

        public LevyAnalyzerTestFixtures WithLateLevy(long submissionId, string payrollYear, byte payrollMonth, DateTime submissionDate, decimal levyDueYtd)
        {
            return WithLevy(submissionId, payrollYear, payrollMonth, submissionDate, false, levyDueYtd);
        }

        public LevyAnalyzerTestFixtures WithYearEndAdjustment(long submissionId, string payrollYear, DateTime submissionDate, decimal levyDueYtd)
        {
            return WithLevy(submissionId, payrollYear, 12, submissionDate, false, levyDueYtd);
        }

        public LevyAnalyzerTestFixtures WithOntimeLevy(long submissionId, string payrollYear, byte payrollMonth, DateTime submissionDate, decimal levyDueYtd)
        {
            return WithLevy(submissionId, payrollYear, payrollMonth, submissionDate, true, levyDueYtd);
        }

        public LevyAnalyzerTestFixtures WithTransaction(long submissionId)
        {
            var transaction = new TransactionLine
            {
                SubmissionId = submissionId
            };

            Transactions.Add(transaction);

            return this;
        }

        public LevyAnalyzerTestFixtures WithTransaction(long submissionId, decimal levyDeclaredForMonth)
        {
            WithTransaction(submissionId);
            Transactions[Transactions.Count - 1].LevyDeclared = levyDeclaredForMonth;
            return this;
        }

        private Account CreateAccount()
        {
            const long testAccountId = 123;

            return new Account(testAccountId, Transactions.ToArray(), Declarations.ToArray());
        }

        public void RunValidate(IRule rule, Action<RuleEvaluationResult, Account> check)
        {
            var account = CreateAccount();

            RuleEvaluationResult result = new RuleEvaluationResult(rule.Name, rule.RequiredValidationObject, account.Id);

            rule.Validate(account, result);

            check(result, account);
        }

        public void AssertAllRulesAreValid()
        {
            var account = CreateAccount();

            var ruleRepository = _lazyRuleRepository.Value;

            var ruleSetEvaluationResult = ruleRepository.ApplyAllRules(account);

            if (!ruleSetEvaluationResult.IsValid)
            {
                DumpValidationMessages(ruleSetEvaluationResult);
                Assert.Fail("Expected the rule set to pass but it has failed");
            }
        }

        public void AssertIsValid(IRule rule)
        {
            RunValidate(rule, (result, account) => {
                if (!result.IsValid)
                {
                    DumpValidationMessages(result);
                    Assert.Fail("Expected the rule to pass but it has failed");
                }});
        }

        public void AssertIsInvalid(IRule rule)
        {
            RunValidate(rule, (result, account) => {
                if (result.IsValid)
                {
                    DumpValidationMessages(result);
                    Assert.Fail("Expected the rule to fail but it has passed");
                }
            });
        }

        private LevyAnalyzerTestFixtures WithLevy(long submissionId, string payrollYear, byte payrollMonth, DateTime submissionDate, bool isOnTime, decimal levyDueYtd)
        {
            var levy = new LevyDeclaration
            {
                Id = submissionId,
                SubmissionDate = submissionDate,
                PayrollYear = payrollYear,
                PayrollMonth = payrollMonth,
                SubmissionId = submissionId,
                LevyDueYTD = levyDueYtd
            };

            HmrcDateServiceMock
                .Setup(ds => ds.IsDateInPayrollPeriod(payrollYear, payrollMonth, submissionDate))
                .Returns(isOnTime);

            Declarations.Add(levy);

            return this;
        }

        private void DumpValidationMessages(IRuleSetEvaluationResult result)
        {
            foreach (var ruleResult in result.RuleEvaluationResults)
            {
                DumpValidationMessages(ruleResult);
            }
        }

        private void DumpValidationMessages(IRuleEvaluationResult result)
        {
            var msg = $"{result.RuleName} : {(result.IsValid ? "Valid" : "Violated")}";
            Console.WriteLine(msg);
            Console.WriteLine(new string('-', msg.Length));

            foreach (var message in result.Messages)
            {
                Console.WriteLine($"{message.Level}: {message.Message}");
            }

            Console.WriteLine();
        }

        private IRuleRepository InitialiseRuleRepository()
        {
            var rules = new IRule[]
            {
                new LateLevyShouldNotResultInATransaction(HmrcDateService),
                new LevyDeclaredForMonthShouldMatchExpected(HmrcDateService),
                new OntimeLevyShouldResultInATransaction(HmrcDateService),
                new SupersededLevyShouldNotResultInATransaction(HmrcDateService),
                new YearEndAdjustmentsShouldBeCummulative(HmrcDateService)
            };

            return new RuleRepository(rules);
        }
    }
}