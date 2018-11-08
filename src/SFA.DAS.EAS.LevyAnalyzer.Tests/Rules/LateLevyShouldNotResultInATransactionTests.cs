using System;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules
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
                .WithLateLevy(123, "18-19", 5, new DateTime(2018, 12, 25));

            var rule = new LateLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountHasLateDeclarationAndTransaction_IsInvalid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLateLevy(123, "18-19", 5, new DateTime(2018, 12, 25))
                .WithTransaction(123);

            var rule = new LateLevyShouldNotResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsFalse(result.IsValid));
        }
    }
}
