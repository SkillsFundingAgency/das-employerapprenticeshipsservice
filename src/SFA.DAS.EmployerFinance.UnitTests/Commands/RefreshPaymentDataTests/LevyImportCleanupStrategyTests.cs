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
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RefreshPaymentDataTests
{
    [TestFixture]
    public class LevyImportCleanerStrategyTests
    { 
        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var fixtures = new LevyImportCleanerStrategyTestFixtures();
            fixtures.CreateStrategy();
        }

        [Test]
        public async Task Cleanup_DuplicateSubsidyIdsInInput_DuplicatedSubsidyIdsShouldBeRemoved()
        {
            //Arrange 
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
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
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
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
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
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

        [Test(Description = "The first adjustment should apply to the P12 declaration")]
        public async Task Cleanup_MultipleLevyAdjustments_FirstAdjustmentShouldApplyToLatestDeclaration()
        {
            const decimal p12KLevyDeclarationAmount = 25;
            const decimal firstAdjustmentValue = 150;
            const decimal secondAdjustmentValue = 200;

            //Arrange 
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
                .WithDeclaration(123, p12KLevyDeclarationAmount, "17-18", 12, new DateTime(2018, 04, 19))
                .WithDeclaration(124, firstAdjustmentValue, "17-18", 12, new DateTime(2018, 05, 19))
                .WithDeclaration(125, secondAdjustmentValue, "17-18", 12, new DateTime(2018, 06, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            const decimal expectedYearEndAdjustment = (firstAdjustmentValue - p12KLevyDeclarationAmount) * -1; // adjustments are inverted
            Assert.AreEqual(expectedYearEndAdjustment, fixtures.Result[1].EndOfYearAdjustmentAmount);
        }

        [Test(Description = "A later adjustment should apply on top of earlier adjustments, not replace")]
        public async Task Cleanup_MultipleLevyAdjustments_LaterAdjustmentsShouldApplyToPriorAdjustments()
        {
            const decimal firstAdjustmentValue = 150;
            const decimal secondAdjustmentValue = 200;

            //Arrange 
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
                .WithDeclaration(123, 100, "17-18", 12, new DateTime(2018, 04, 19))
                .WithDeclaration(124, firstAdjustmentValue, "17-18", 12, new DateTime(2018, 05, 19))
                .WithDeclaration(125, secondAdjustmentValue, "17-18", 12, new DateTime(2018, 06, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            const decimal expectedYearEndAdjustment = (secondAdjustmentValue - firstAdjustmentValue) * -1; // adjustments are inverted
            Assert.AreEqual(expectedYearEndAdjustment, fixtures.Result[2].EndOfYearAdjustmentAmount);
        }

        [Test, Description("When an adjustment is received and a period 12 declaration has been made the adjustment should apply to the period 12 declaration")]
        public async Task Cleanup_SingleLevyAdjustmentWithPeriod12Value_AdjustmentShouldApplyToPeriod12Value()
        {
            const decimal period12Value = 150;
            const decimal adjustmentValue = 200;

            //Arrange 
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
                .WithDeclaration(123, period12Value, "17-18", 12, new DateTime(2018, 04, 19))
                .WithDeclaration(124, adjustmentValue, "17-18", 12, new DateTime(2018, 05, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            const decimal expectedYearEndAdjustment = (adjustmentValue - period12Value) * -1; // adjustments are inverted
            Assert.AreEqual(expectedYearEndAdjustment, fixtures.Result[1].EndOfYearAdjustmentAmount);
        }

        [Test, Description("When an adjustment is received and the latest declaration is not period 12 the adjustment should apply to the latest declaration")]
        public async Task Cleanup_SingleLevyAdjustmentWithPeriod8Value_AdjustmentShouldApplyToPeriod8Value()
        {
            const decimal period8Value = 150;
            const decimal adjustmentValue = 200;

            //Arrange 
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
                .WithDeclaration(123, period8Value, "17-18", 8, new DateTime(2017, 12, 19))
                .WithDeclaration(124, adjustmentValue, "17-18", 12, new DateTime(2018, 05, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            const decimal expectedYearEndAdjustment = (adjustmentValue - period8Value) * -1; // adjustments are inverted
            Assert.AreEqual(expectedYearEndAdjustment, fixtures.Result[1].EndOfYearAdjustmentAmount);
        }

        [Test, Description("When an adjustment is received but there is no declaration the adjustment should apply to an assumed zero declaration")]
        public async Task Cleanup_AdjustmentWithNoDeclaration_AdjustmentShouldApplyToAnAssummedZeroDeclaration()
        {
            const decimal adjustmentValue = 200;

            //Arrange 
            var fixtures = new LevyImportCleanerStrategyTestFixtures()
                .WithDeclaration(124, adjustmentValue, "17-18", 12, new DateTime(2018, 05, 19));

            // act
            await fixtures.RunStrategy();

            //Assert 
            const decimal expectedYearEndAdjustment = adjustmentValue * -1; // adjustments are inverted
            Assert.AreEqual(expectedYearEndAdjustment, fixtures.Result[0].EndOfYearAdjustmentAmount);
        }
    }

    internal class LevyImportCleanerStrategyTestFixtures
    {
        public LevyImportCleanerStrategyTestFixtures()
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

        public LevyImportCleanerStrategy CreateStrategy()
        {
            return new LevyImportCleanerStrategy(
                DasLevyRepository,
                HmrcDateService,
                Log
                );
        }

        public LevyImportCleanerStrategyTestFixtures WithDeclarations(params long[] subsidyIds)
        {
            foreach (var subsidyId in subsidyIds)
            {
                WithDeclaration(new DasDeclaration{SubmissionId = subsidyId});
            }

            return this;
        }

        public LevyImportCleanerStrategyTestFixtures WithDeclaration(long subsidyId, decimal levyDueYtd, string payrollYear, short payrollMonth, DateTime submissionDate)
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

        public LevyImportCleanerStrategyTestFixtures WithDeclaration(DasDeclaration declaration)
        {
            Declarations.Add(declaration);

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
