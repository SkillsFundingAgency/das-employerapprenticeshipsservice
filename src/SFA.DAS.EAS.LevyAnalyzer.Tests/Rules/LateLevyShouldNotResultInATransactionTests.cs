using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Models;
using SFA.DAS.EAS.LevyAnalyzer.Rules;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyzer.Tests.Rules
{
    [TestFixture]
    public class LateLevyShouldNotResultInATransactionTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldNotThrowException()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new LateLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            Assert.Pass("Did not throw exception");
        }

        [Test]
        public void Validate_AccountWithNoDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new LateLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountHasLateDeclarationAndNoTransaction_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 5, new DateTime(2018, 12, 25), false);

            var rule = new LateLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountHasLateDeclarationAndTransaction_IsInvalid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 5, new DateTime(2018, 12, 25), false)
                .WithTransaction(123);

            var rule = new LateLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsFalse(result.IsValid));
        }
    }

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

        public LevyAnalyzerTestFixtures WithLevy(long subsidyId, string payrollYear, byte payrollMonth, DateTime submissionDate, bool isOnTime)
        {
            var levy = new LevyDeclaration
            {
                Id = subsidyId,
                SubmissionDate = submissionDate,
                PayrollYear = payrollYear,
                PayrollMonth = payrollMonth
            };

            HmrcDateServiceMock
                .Setup(ds => ds.IsDateInPayrollPeriod(payrollYear, payrollMonth, submissionDate))
                .Returns(isOnTime);

            Declarations.Add(levy);

            return this;
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

        public void RunValidate(IRule rule, Action<RuleEvaluationResult, Account> check)
        {
            var account = new Account(Transactions.ToArray(), Declarations.ToArray());

            RuleEvaluationResult result = new RuleEvaluationResult(rule.Name);

            rule.Validate(account, result);

            check(result, account);
        }
    }
}
