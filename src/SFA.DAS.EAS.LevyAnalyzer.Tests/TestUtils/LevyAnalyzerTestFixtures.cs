using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils
{
    public class LevyAnalyzerTestFixtures
    {
        public LevyAnalyzerTestFixtures()
        {
            HmrcDateServiceMock = new Mock<IHmrcDateService>();    
            Transactions = new List<TransactionLine>();
            Declarations = new List<LevyDeclaration>();
        }

        public Mock<IHmrcDateService> HmrcDateServiceMock { get; set; }

        public IHmrcDateService HmrcDateService => HmrcDateServiceMock.Object;

        public List<TransactionLine> Transactions { get; set; }

        public List<LevyDeclaration> Declarations { get; set; }

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

        public void RunValidate(IRule rule, Action<RuleEvaluationResult, Account> check)
        {
            const long testAccountId = 123;

            var account = new Account(testAccountId, Transactions.ToArray(), Declarations.ToArray());

            RuleEvaluationResult result = new RuleEvaluationResult(rule.Name, rule.RequiredValidationObject, account.Id);

            rule.Validate(account, result);

            check(result, account);
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

        private void DumpValidationMessages(RuleEvaluationResult result)
        {
            foreach (var message in result.Messages)
            {
                Console.WriteLine($"{message.Level}: {message.Message}");
            }
        }
    }
}