using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules.Infratsructure
{
    [TestFixture]
    public class DeclarationSummaryFactoryTests
    {
        [Test]
        public void Create_OkayLevy_ShouldSetStateToNormal()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "R18-19", 1, new DateTime(2018, 05, 01))
                .WithTransaction(123);

            fixtures.AssertDeclarationSummaryIs(123, DeclarationState.Normal);
        }

        [Test]
        public void Create_LevyWithoutTransaction_ShouldSetStateToNoTransaction()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "R18-19", 1, new DateTime(2018, 05, 01));

            fixtures.AssertDeclarationSummaryIs(123, DeclarationState.NoTransaction);
        }

        [Test]
        public void Create_LateWithoutTransaction_ShouldSetStateToNoTransactionAndLateSubmission()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLateLevy(123, "R18-19", 1, new DateTime(2018, 05, 01));

            fixtures.AssertDeclarationSummaryIs(123, DeclarationState.NoTransaction | DeclarationState.LateSubmission);
        }

        [Test]
        public void Create_P12Late_ShouldSetStateToP12AndLateSubmission()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithYearEndAdjustment(123, "R18-19", new DateTime(2018, 05, 01), 100)
                .WithTransaction(123);

            fixtures.AssertDeclarationSummaryIs(123, DeclarationState.IsPeriod12 | DeclarationState.LateSubmission);
        }

        [Test]
        public void Create_P12LateTransaction_ShouldSetStateToYearEndAdjustment()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithYearEndAdjustment(123, "R18-19", new DateTime(2018, 05, 01), 100)
                .WithTransaction(123);

            fixtures.AssertDeclarationSummaryIs(123, DeclarationState.IsYearEndAdjustment);
        }

        [Test]
        public void Create_LevyWithSupersedingLevy_ShouldSetStateToWasSuperseded()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(123, "R18-19", 1, new DateTime(2018, 05, 01))
                .WithOntimeLevy(124, "R18-19", 1, new DateTime(2018, 05, 02))
                .WithTransaction(124);

            fixtures.AssertDeclarationSummaryIs(123, DeclarationState.NoTransaction | DeclarationState.WasSuperseded);
        }
    }
}
