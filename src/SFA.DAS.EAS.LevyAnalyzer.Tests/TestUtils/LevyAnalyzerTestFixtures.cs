using System;
using System.Collections.Generic;
using Moq;
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

        public LevyAnalyzerTestFixtures WithLevy(long submissionId, string payrollYear, byte payrollMonth, DateTime submissionDate, bool isOnTime)
        {
            var levy = new LevyDeclaration
            {
                Id = submissionId,
                SubmissionDate = submissionDate,
                PayrollYear = payrollYear,
                PayrollMonth = payrollMonth,
                SubmissionId = submissionId
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
            const long testAccountId = 123;

            var account = new Account(testAccountId, Transactions.ToArray(), Declarations.ToArray());

            RuleEvaluationResult result = new RuleEvaluationResult(rule.Name);

            rule.Validate(account, result);

            check(result, account);
        }
    }
}