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
    public class OntimeLevyShouldResultInATransactionTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldNotThrowException()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new OntimeLevyShouldResultInATransaction(fixtures.HmrcDateService);

            Assert.Pass("Did not throw exception");
        }

        [Test]
        public void Validate_AccountWithNoDeclarations_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures();

            var rule = new OntimeLevyShouldResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithOnTimeDeclarationAndTransaction_IsValid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 5, new DateTime(2018, 12, 25), true)
                .WithTransaction(123);

            var rule = new OntimeLevyShouldResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsTrue(result.IsValid));
        }

        [Test]
        public void Validate_AccountWithOnTimeDeclarationAndNoTransaction_IsInvalid()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLevy(123, "18-19", 5, new DateTime(2018, 12, 25), true);

            var rule = new OntimeLevyShouldResultInATransaction(fixtures.HmrcDateService);

            fixtures.RunValidate(rule, (result, account) => Assert.IsFalse(result.IsValid));
        }
    }
}
