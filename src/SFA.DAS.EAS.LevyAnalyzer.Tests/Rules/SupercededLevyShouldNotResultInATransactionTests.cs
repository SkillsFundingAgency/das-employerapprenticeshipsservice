using System;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules
{
    [TestFixture]
    public class SupercededLevyShouldNotResultInATransactionTests
    {
        [Test]
        public void Validate_AccountWithNoDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new SupercededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithNoSupercededDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 1, new DateTime(2018, 05, 10), true)
                .WithLevy(124, "18-19", 2, new DateTime(2018, 06, 10), true)
                .WithTransaction(123)
                .WithTransaction(124);

            var rule = new SupercededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithSupercededButLateDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 1, new DateTime(2018, 05, 10), true)
                .WithLevy(124, "18-19", 1, new DateTime(2018, 05, 21), false)
                .WithTransaction(123)
                // we're testing the rule does not validate late submissions - not that late submissions do not generate a transaction
                .WithTransaction(124);  


            var rule = new SupercededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithSupercededDeclarationsWithoutTransactions_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 1, new DateTime(2018, 05, 10), true)
                .WithLevy(124, "18-19", 1, new DateTime(2018, 05, 11), true)
                .WithTransaction(124);

            var rule = new SupercededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithSupercededDeclarationsWithTransactions_IsNotValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 1, new DateTime(2018, 05, 10), true)
                .WithLevy(124, "18-19", 1, new DateTime(2018, 05, 11), true)
                .WithTransaction(123)
                .WithTransaction(124);

            var rule = new SupercededLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsFalse(result.IsValid));
        }
    }
}
