using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.Validation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RefreshPaymentDataTests
{
    [TestFixture]
    public class LevyImportCleanupStrategyTests
    { 
        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var fixtures = new LevyImportCleanupStrategyTestFixtures();
            var strategy = fixtures.CreateStrategy();
        }

        [Test]
        public async Task Cleanup_DuplicateSubsidyIdsInInput_DuplicatedSubsidyIdsShouldBeRemoved()
        {
            //Arrange 
            var fixtures = new LevyImportCleanupStrategyTestFixtures()
                            .WithDeclarations(123, 123);

            // act
            await fixtures.RunStrategy();

            //Assert 
            const int expectedCount = 1;
            Assert.AreEqual(expectedCount, fixtures.Result.Length);
        }

        [Test]
        public async Task Cleanup_DuplicateSubsidyIdsInInput_DuplicatedSubsidyIdsShouldBeLogged()
        {
            //Arrange 
            var fixtures = new LevyImportCleanupStrategyTestFixtures()
                .WithDeclarations(123, 123);

            // act
            await fixtures.RunStrategy();

            //Assert 
            const int expectedInfoMessages = 1;
            Assert.AreEqual(expectedInfoMessages, fixtures.InfoLog.Count);
        }

        [Test]
        public async Task Cleanup_MultipleLevyAdjustments_OnlyAdjustmentsShouldBeMArkedAsAdjustments()
        {
            //Arrange 
            var fixtures = new LevyImportCleanupStrategyTestFixtures()
                .WithDeclaration(123, 100, "17-18", 12, new DateTime(2018, 04, 19))
                .WithDeclaration(124, 150, "17-18", 12, new DateTime(2018, 05, 19))
                .WithDeclaration(125, 200, "17-18", 12, new DateTime(2018, 06, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            Assert.IsFalse(fixtures.Result[0].EndOfYearAdjustment, "normal period 12 declaration is marked as adjustment");
            Assert.IsTrue(fixtures.Result[1].EndOfYearAdjustment, "period 12 adjustment is not marked as adjustment");
            Assert.IsTrue(fixtures.Result[2].EndOfYearAdjustment, "period 12 adjustment is not marked as adjustment");
        }

        [Test(Description = "An adjustment should apply on top of previous adjustments, not replace")]
        public async Task Cleanup_MultipleLevyAdjustments_AdjustmentsShouldApplyToPriorAdjustments()
        {
            //Arrange 
            var fixtures = new LevyImportCleanupStrategyTestFixtures()
                .WithDeclaration(123, 100, "17-18", 12, new DateTime(2018, 04, 19))
                .WithDeclaration(124, 150, "17-18", 12, new DateTime(2018, 05, 19))
                .WithDeclaration(125, 200, "17-18", 12, new DateTime(2018, 06, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            const decimal expectedYearEndAdjustment = (200 - 150); // y/end adjustments are inverted
            Assert.AreEqual(expectedYearEndAdjustment, fixtures.Result[2].EndOfYearAdjustmentAmount);
        }
    }

    internal class LevyImportCleanupStrategyTestFixtures
    {
        public LevyImportCleanupStrategyTestFixtures()
        {
            DasLevyRepositoryMock = new Mock<IDasLevyRepository>();
            LogMock = new Mock<ILog>();
            Declarations = new List<DasDeclaration>();
            InfoLog = new List<string>();
            LogMock.Setup(l => l.Info(It.IsAny<string>())).Callback<string>(s => InfoLog.Add(s));
        }

        public Mock<IDasLevyRepository> DasLevyRepositoryMock { get; set; }
        public IDasLevyRepository DasLevyRepository => DasLevyRepositoryMock.Object;

        public IHmrcDateService HmrcDateService { get; } = new HmrcDateService();

        public Mock<ILog> LogMock { get; set; }
        public ILog Log => LogMock.Object;

        public List<string> InfoLog { get; }

        public string EmpRef { get; set; }

        public List<DasDeclaration> Declarations { get; }

        public LevyImportCleanupStrategy CreateStrategy()
        {
            return new LevyImportCleanupStrategy(
                DasLevyRepository,
                HmrcDateService,
                Log
                );
        }

        public LevyImportCleanupStrategyTestFixtures WithDeclarations(params long[] subsidyIds)
        {
            foreach (var subsidyId in subsidyIds)
            {
                WithDeclaration(subsidyId);
            }

            return this;
        }

        public LevyImportCleanupStrategyTestFixtures WithDeclaration(long subsidyId)
        {
            return WithDeclaration(subsidyId, 1M);
        }

        public LevyImportCleanupStrategyTestFixtures WithDeclaration(long subsidyId, decimal levyDueYtd)
        {
            return WithDeclaration(subsidyId, levyDueYtd, "17-18", 12, new DateTime(2017, 04, 19));
        }

        public LevyImportCleanupStrategyTestFixtures WithAdjustmentDeclaration(long subsidyId, decimal levyDueYtd, DateTime submissionDate)
        {
            return WithDeclaration(subsidyId, levyDueYtd, "16-17", 12, submissionDate);
        }

        public LevyImportCleanupStrategyTestFixtures WithDeclaration(long subsidyId, decimal levyDueYtd, string payrollYear, short payrollMonth, DateTime submissionDate)
        {
            return WithDeclaration(new DasDeclaration
            {
                Id = subsidyId.ToString(CultureInfo.InvariantCulture),
                SubmissionId = subsidyId,
                LevyDueYtd = levyDueYtd,
                PayrollYear = payrollYear,
                PayrollMonth = payrollMonth,
                SubmissionDate = submissionDate
            });
        }

        public LevyImportCleanupStrategyTestFixtures WithDeclaration(DasDeclaration declaration)
        {
            Declarations.Add(declaration);

            return this;
        }

        public LevyImportCleanupStrategyTestFixtures WithEmpRef(string empRef)
        {
            EmpRef = empRef;
            return this;
        }

        public DasDeclaration[] Result { get; private set; }

        public async Task RunStrategy()
        {
            var strategy = CreateStrategy();
            var tempResult = await strategy.Cleanup(EmpRef, Declarations);

            Result = tempResult.ToArray();
        }
    }
}
