using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
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

        /// <summary>
        ///     Assert that the declarations and transactions that have been set up pass all the 
        ///     rules defined in the system. Once verified each transaction will be temporarily removed 
        ///     to verify the rules now fail.
        /// </summary>
        public void AssertAllRulesAreValidAndThenCheckNegativeCase()
        {
            AssertAllRulesAreValid();

            RemoveEachTranactionInTurnAndAssertAllRulesAreFailing();
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

        public void RemoveEachTranactionInTurnAndAssertAllRulesAreFailing()
        {
            for (int i = 0; i < Transactions.Count; i++)
            {
                RemoveSpecifiedTransactionAndAssertAllRulesAreFailing(i);
            }
        }

        public void RemoveSpecifiedTransactionAndAssertAllRulesAreFailing(int transactionIndex)
        {
            Assert.IsTrue(Transactions.Count > transactionIndex, $"Cannot use method {nameof(RemoveSpecifiedTransactionAndAssertAllRulesAreFailing)} to remove index {transactionIndex} for this test because the test has only set up {Transactions.Count} transactions");

            var transactionToRemove = Transactions[transactionIndex];
            Transactions.RemoveAt(transactionIndex);
            try
            {
                var account = CreateAccount();

                var ruleRepository = _lazyRuleRepository.Value;

                var ruleSetEvaluationResult = ruleRepository.ApplyAllRules(account);

                if (ruleSetEvaluationResult.IsValid)
                {
                    Assert.Fail("Expected the rule set to fail but it has passed");
                }
            }
            finally
            {
                Transactions.Insert(transactionIndex, transactionToRemove);
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

        /// <summary>
        ///     Verifies that the supplied declarations consists of only the supplied submission ids.
        /// </summary>
        public void AssertActiveDeclarations(params long[] submissionIds)
        {
            var matchResults = new []
            {
                new List<long>(), new List<long>(), new List<long>()
            };

            const int expectedAndFound = 0;
            const int expectedButNotFound = 1;
            const int foundButNotExpected = 2;

            var sourceList = Declarations.ActiveDeclarations(HmrcDateService).ToList();

            foreach (var submissionId in submissionIds)
            {
                var matched = sourceList.SingleOrDefault(declaration => declaration.SubmissionId == submissionId);

                if (matched != null)
                {
                    matchResults[expectedAndFound].Add(submissionId);
                    sourceList.Remove(matched);
                }
                else
                {
                    matchResults[expectedButNotFound].Add(submissionId);
                }
            }

            matchResults[foundButNotExpected].AddRange(sourceList.Select(declaration => declaration.SubmissionId));

            var sb = new StringBuilder();

            var foundErrors = AddSubmissionIds(sb,
                                  "The following submissions were expected to be active but were not:",
                                  matchResults[expectedButNotFound])
                              ||
                              AddSubmissionIds(sb,
                                  "The following submissions were not unexpectedly found to be active:",
                                  matchResults[foundButNotExpected]);

            Assert.IsFalse(foundErrors, "The active submissions are not those expected");
        }

        private bool AddSubmissionIds(StringBuilder sb, string message, List<long> submissionIds)
        {
            if (submissionIds.Count == 0)
            {
                return false;
            }

            sb.AppendLine(message);
            foreach (var submissionId in submissionIds)
            {
                sb.Append(submissionId);
                sb.Append(", ");
            }

            sb.Length = sb.Length - 2;

            return true;
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
            var msg = $"{result.RuleName} : {(result.IsValid ? "Pass" : "Violated")}";
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