using System;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules
{
    [TestFixture(Description = "For these tests we're only interested in whether the year-end adjustments have a correct transaction")]
    public class YearEndAdjustmentsShouldBeCummulativeTests
    {
        [Test]
        public void Validate_AccountWithNoTearEndAdjustments_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithNoP12AndOnlyP12Adjustment_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithYearEndAdjustment(123, "18-19", new DateTime(2018, 06, 10), 100)
                .WithTransaction(123, 100);

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithP12AndP12Adjustment_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2018, 05, 10), 100)
                .WithYearEndAdjustment(124, "18-19", new DateTime(2018, 06, 10), 200)
                .WithTransaction(124, 100);

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithMultipleOnTimeP12AndP12Adjustment_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(124, "18-19", 12, new DateTime(2018, 05, 11), 200)
                .WithOntimeLevy(125, "18-19", 12, new DateTime(2018, 05, 11), 300)
                .WithYearEndAdjustment(126, "18-19", new DateTime(2018, 06, 10), 200)
                .WithTransaction(126, -100);

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithMultipleAdjustment_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2018, 05, 10), 100)
                .WithYearEndAdjustment(126, "18-19", new DateTime(2018, 06, 10), 200)
                .WithYearEndAdjustment(127, "18-19", new DateTime(2018, 06, 11), 325)
                .WithYearEndAdjustment(128, "18-19", new DateTime(2018, 06, 12), 500)
                .WithTransaction(126, 100)
                .WithTransaction(127, 125)
                .WithTransaction(128, 175);

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithP12AdjustmentButNoTransaction_IsInvalid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2018, 05, 10), 100)
                .WithYearEndAdjustment(126, "18-19", new DateTime(2018, 06, 10), 200);

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsInvalid(rule);
        }

        [Test]
        public void Validate_AccountWithIncorrectTransactionLineAdjustmentValue_IsInvalid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2018, 05, 10), 100)
                .WithYearEndAdjustment(126, "18-19", new DateTime(2018, 06, 10), 200)
                .WithTransaction(126, 75); // should be 100

            var rule = new YearEndAdjustmentsShouldBeCummulative(fixtures.HmrcDateService);

            fixtures.AssertIsInvalid(rule);
        }
    }
}
