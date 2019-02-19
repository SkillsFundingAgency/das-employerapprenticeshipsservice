using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Extensions
{
    [TestFixture]
    public class LevyDeclarationExtensionsTests
    {
        [Test]
        public void GroupByPayrollPeriod_MultipleDeclarationsInSamePeriod_ShouldBeGrouped()
        {
            var levyDeclarations = new[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1, LevyDueYTD = 100, SubmissionDate = new DateTime(2017, 04, 01)},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1, LevyDueYTD = 200, SubmissionDate = new DateTime(2017, 04, 01)}
            };

            var groups = levyDeclarations.GroupByPayrollPeriod().ToArray();

            Assert.AreEqual(1, groups.Length);
        }

        [Test]
        public void GroupByPayrollPeriod_MultipleDeclarationsInDifferentPeriods_ShouldNotBeGrouped()
        {
            var levyDeclarations = new[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1, LevyDueYTD = 100, SubmissionDate = new DateTime(2017, 04, 01)},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 2, LevyDueYTD = 200, SubmissionDate = new DateTime(2017, 05, 01)}
            };

            var groups = levyDeclarations.GroupByPayrollPeriod().ToArray();

            Assert.AreEqual(2, groups.Length);
        }

        [Test]
        public void GroupByPayrollPeriod_MultipleDeclarationsInSameMonthButDifferentYear_ShouldNotBeGrouped()
        {
            var levyDeclarations = new[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1, LevyDueYTD = 100, SubmissionDate = new DateTime(2017, 04, 01)},
                new LevyDeclaration {PayrollYear = "18-19", PayrollMonth = 1, LevyDueYTD = 200, SubmissionDate = new DateTime(2018, 04, 01)}
            };

            var groups = levyDeclarations.GroupByPayrollPeriod().ToArray();

            Assert.AreEqual(2, groups.Length);
        }

        [Test]
        public void CalculateMonthlyValues_SomeTransaction_ShouldCalculateMonthlyLevyDeclaration()
        {
            var levyDeclarations = new[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1, LevyDueYTD = 100, SubmissionDate = new DateTime(2017, 04, 01)},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 2, LevyDueYTD = 225, SubmissionDate = new DateTime(2017, 05, 01)},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 3, LevyDueYTD = 375, SubmissionDate = new DateTime(2017, 06, 01)},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 4, LevyDueYTD = 300, SubmissionDate = new DateTime(2017, 07, 01)}
            };

            var monthlyLevyAmounts = levyDeclarations.CalculateMonthlyValues().ToArray();

            Assert.AreEqual(levyDeclarations.Length, monthlyLevyAmounts.Length, "Incorrect number of monthly amounts has been returned");

            Assert.AreEqual(100, monthlyLevyAmounts[0].CalculatedLevyAmountForMonth, "Value for month 1 is incorrect");
            Assert.AreEqual(125, monthlyLevyAmounts[1].CalculatedLevyAmountForMonth, "Value for month 2 is incorrect");
            Assert.AreEqual(150, monthlyLevyAmounts[2].CalculatedLevyAmountForMonth, "Value for month 3 is incorrect");
            Assert.AreEqual(-75, monthlyLevyAmounts[3].CalculatedLevyAmountForMonth, "Value for month 4 is incorrect");
        }


        [Test]
        public void CalculateMonthlyValues_SomeTransactionsSpanYearStart_ShouldCalculateMonthlyLevyDeclaration()
        {
            var levyDeclarations = new[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 11, LevyDueYTD = 100, SubmissionDate = new DateTime(2017, 02, 01)},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 12, LevyDueYTD = 225, SubmissionDate = new DateTime(2017, 03, 01)},
                new LevyDeclaration {PayrollYear = "18-19", PayrollMonth = 1, LevyDueYTD = 375, SubmissionDate = new DateTime(2018, 04, 01)},
                new LevyDeclaration {PayrollYear = "18-19", PayrollMonth = 2, LevyDueYTD = 300, SubmissionDate = new DateTime(2018, 05, 01)}
            };

            var monthlyLevyAmounts = levyDeclarations.CalculateMonthlyValues().ToArray();

            Assert.AreEqual(levyDeclarations.Length, monthlyLevyAmounts.Length, "Incorrect number of monthly amounts has been returned");

            Assert.AreEqual(100, monthlyLevyAmounts[0].CalculatedLevyAmountForMonth, "Value for month 1 is incorrect");
            Assert.AreEqual(125, monthlyLevyAmounts[1].CalculatedLevyAmountForMonth, "Value for month 2 is incorrect");
            Assert.AreEqual(375, monthlyLevyAmounts[2].CalculatedLevyAmountForMonth, "Value for month 3 is incorrect");
            Assert.AreEqual(-75, monthlyLevyAmounts[3].CalculatedLevyAmountForMonth, "Value for month 4 is incorrect");
        }

        [Test]
        public void ActiveDeclarations_AllOnTimeWithNoSuperseded_ShouldReturnAllDeclarations()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 05, 05))
                .WithOntimeLevy(124, "18-19", 4, new DateTime(2018, 06, 05))
                .WithOntimeLevy(125, "18-19", 5, new DateTime(2018, 07, 05));

            fixtures.AssertActiveDeclarations(123, 124, 125);
        }

        [Test]
        public void ActiveDeclarations_AllOnTimeWithSuperseded_ShouldReturnOnlyLastInPeriod()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 05, 05))
                .WithOntimeLevy(124, "18-19", 4, new DateTime(2018, 06, 05))
                .WithOntimeLevy(125, "18-19", 4, new DateTime(2018, 06, 06))
                .WithOntimeLevy(126, "18-19", 5, new DateTime(2018, 07, 05));

            fixtures.AssertActiveDeclarations(123, 125, 126);
        }

        [Test]
        public void ActiveDeclarations_SomeLateWithNoSuperseded_ShouldNotReturnLateDeclarations()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 05, 05))
                .WithLateLevy(125, "18-19", 4, new DateTime(2018, 06, 06))
                .WithOntimeLevy(126, "18-19", 5, new DateTime(2018, 07, 05));

            fixtures.AssertActiveDeclarations(123, 126);
        }

        [Test]
        public void ActiveDeclarations_OnTimeWithYearEndAdjustment_ShouldReturnAllOnTimeAndAdjustment()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 05, 05))
                .WithOntimeLevy(124, "18-19", 4, new DateTime(2018, 06, 05))
                .WithYearEndAdjustment(125, "18-19", new DateTime(2018, 07, 05), 100);

            fixtures.AssertActiveDeclarations(123, 124, 125);
        }

        [Test]
        public void ActiveDeclarations_OnTimeWithYearEndAdjustments_ShouldReturnAllOnTimeAndAllAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 05, 05))
                .WithOntimeLevy(124, "18-19", 4, new DateTime(2018, 06, 05))
                .WithYearEndAdjustment(125, "18-19", new DateTime(2018, 07, 05), 100)
                .WithYearEndAdjustment(126, "18-19", new DateTime(2018, 07, 06), 100);

            fixtures.AssertActiveDeclarations(123, 124, 125, 126);
        }
    }
}
