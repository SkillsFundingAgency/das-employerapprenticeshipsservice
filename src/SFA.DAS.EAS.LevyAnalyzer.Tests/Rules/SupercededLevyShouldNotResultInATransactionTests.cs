using System;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules
{
    [TestFixture]
    public class SupersededLevyShouldNotResultInATransactionTests
    {
        [Test]
        public void Validate_AccountWithNoDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new SupersededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithNoSupersededDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10))
                .WithOntimeLevy(124, "18-19", 2, new DateTime(2018, 06, 10))
                .WithTransaction(123)
                .WithTransaction(124);

            var rule = new SupersededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithSupersededButLateDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10))
                .WithLateLevy(124, "18-19", 1, new DateTime(2018, 05, 21))
                .WithTransaction(123)
                // we're testing the rule does not validate late submissions - not that late submissions do not generate a transaction
                .WithTransaction(124);  


            var rule = new SupersededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithSupersededDeclarationsWithoutTransactions_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10))
                .WithOntimeLevy(124, "18-19", 1, new DateTime(2018, 05, 11))
                .WithTransaction(124);

            var rule = new SupersededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithSupersededDeclarationsWithTransactions_IsNotValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "18-19", 1, new DateTime(2018, 05, 10))
                .WithOntimeLevy(124, "18-19", 1, new DateTime(2018, 05, 11))
                .WithTransaction(123)
                .WithTransaction(124);

            var rule = new SupersededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsFalse(result.IsValid));
        }
    }
}
