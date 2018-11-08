using System;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules
{
    [TestFixture]
    public class LevyDeclaredForMonthShouldMatchExpectedTests
    {
        [Test]
        public void Validate_AccountWithNoDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new LevyDeclaredForMonthShouldMatchExpected(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithCorrectMonthlyValues_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(124, "18-19", 2, new DateTime(2018, 06, 10), 150)
                .WithTransaction(123, 100)
                .WithTransaction(124, 50);

            var rule = new LevyDeclaredForMonthShouldMatchExpected(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithManyCorrectMonthlyValues_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(124, "18-19", 2, new DateTime(2018, 06, 10), 150)
                .WithOntimeLevy(125, "18-19", 3, new DateTime(2018, 07, 10), 225)
                .WithOntimeLevy(126, "18-19", 4, new DateTime(2018, 08, 10), 325)
                .WithTransaction(123, 100)
                .WithTransaction(124, 50)
                .WithTransaction(125, 75)
                .WithTransaction(126, 100);

            var rule = new LevyDeclaredForMonthShouldMatchExpected(fixtures.HmrcDateService);

            fixtures.AssertIsValid(rule);
        }

        [Test]
        public void Validate_AccountWithIncorrectMonthlyValues_IsNotValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(124, "18-19", 2, new DateTime(2018, 06, 10), 150)
                .WithTransaction(123, 100)
                .WithTransaction(124, 51);

            var rule = new LevyDeclaredForMonthShouldMatchExpected(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsFalse(result.IsValid));
        }
    }
}
