using System;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using SFA.DAS.EAS.LevyAnalyser.Tests.TestUtils;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Rules
{
    [TestFixture]
    public class ScenarioTests
    {
        [Test]
        public void Scenario01_P1toP11AllOnTime()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(121, "18-19",  1, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(122, "18-19",  2, new DateTime(2018, 06, 10), 200)
                .WithOntimeLevy(123, "18-19",  3, new DateTime(2018, 07, 10), 300)
                .WithOntimeLevy(124, "18-19",  4, new DateTime(2018, 08, 10), 400)
                .WithOntimeLevy(125, "18-19",  5, new DateTime(2018, 09, 10), 500)
                .WithOntimeLevy(126, "18-19",  6, new DateTime(2018, 10, 10), 600)
                .WithOntimeLevy(127, "18-19",  7, new DateTime(2018, 11, 10), 700)
                .WithOntimeLevy(128, "18-19",  8, new DateTime(2018, 12, 10), 800)
                .WithOntimeLevy(129, "18-19",  9, new DateTime(2019, 01, 10), 900)
                .WithOntimeLevy(130, "18-19", 10, new DateTime(2019, 02, 10), 1000)
                .WithOntimeLevy(131, "18-19", 11, new DateTime(2019, 03, 10), 1100)

                .WithTransaction(121, 100)
                .WithTransaction(122, 100)
                .WithTransaction(123, 100)
                .WithTransaction(124, 100)
                .WithTransaction(125, 100)
                .WithTransaction(126, 100)
                .WithTransaction(127, 100)
                .WithTransaction(128, 100)
                .WithTransaction(129, 100)
                .WithTransaction(130, 100)
                .WithTransaction(131, 100);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario02_P12OnTime()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(132, "18-19", 12, new DateTime(2019, 04, 10), 1200)

                .WithTransaction(132, 1200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario03_P1toP12AllOnTime()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(121, "18-19", 1, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(122, "18-19", 2, new DateTime(2018, 06, 10), 200)
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 07, 10), 300)
                .WithOntimeLevy(124, "18-19", 4, new DateTime(2018, 08, 10), 400)
                .WithOntimeLevy(125, "18-19", 5, new DateTime(2018, 09, 10), 500)
                .WithOntimeLevy(126, "18-19", 6, new DateTime(2018, 10, 10), 600)
                .WithOntimeLevy(127, "18-19", 7, new DateTime(2018, 11, 10), 700)
                .WithOntimeLevy(128, "18-19", 8, new DateTime(2018, 12, 10), 800)
                .WithOntimeLevy(129, "18-19", 9, new DateTime(2019, 01, 10), 900)
                .WithOntimeLevy(130, "18-19", 10, new DateTime(2019, 02, 10), 1000)
                .WithOntimeLevy(131, "18-19", 11, new DateTime(2019, 03, 10), 1100)
                .WithOntimeLevy(132, "18-19", 12, new DateTime(2019, 04, 10), 1200)

                .WithTransaction(121, 100)
                .WithTransaction(122, 100)
                .WithTransaction(123, 100)
                .WithTransaction(124, 100)
                .WithTransaction(125, 100)
                .WithTransaction(126, 100)
                .WithTransaction(127, 100)
                .WithTransaction(128, 100)
                .WithTransaction(129, 100)
                .WithTransaction(130, 100)
                .WithTransaction(131, 100)
                .WithTransaction(132, 100);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario04_P1toP11SomeLate()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(121, "18-19", 1, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(122, "18-19", 2, new DateTime(2018, 06, 10), 200)
                .WithOntimeLevy(123, "18-19", 3, new DateTime(2018, 07, 10), 300)
                .WithLateLevy(124, "18-19", 4, new DateTime(2018, 08, 10), 400)
                .WithOntimeLevy(125, "18-19", 5, new DateTime(2018, 09, 10), 500)
                .WithOntimeLevy(126, "18-19", 6, new DateTime(2018, 10, 10), 600)
                .WithOntimeLevy(127, "18-19", 7, new DateTime(2018, 11, 10), 700)
                .WithLateLevy(128, "18-19", 8, new DateTime(2018, 12, 10), 800)
                .WithOntimeLevy(129, "18-19", 9, new DateTime(2019, 01, 10), 900)
                .WithOntimeLevy(130, "18-19", 10, new DateTime(2019, 02, 10), 1000)
                .WithOntimeLevy(131, "18-19", 11, new DateTime(2019, 03, 10), 1100)

                .WithTransaction(121, 100)
                .WithTransaction(122, 100)
                .WithTransaction(123, 100)
                .WithTransaction(125, 200)
                .WithTransaction(126, 100)
                .WithTransaction(127, 100)
                .WithTransaction(129, 200)
                .WithTransaction(130, 100)
                .WithTransaction(131, 100);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario05_P1toP11AllLate()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithLateLevy(121, "18-19", 1, new DateTime(2018, 05, 10), 100)
                .WithLateLevy(122, "18-19", 2, new DateTime(2018, 06, 10), 200)
                .WithLateLevy(123, "18-19", 3, new DateTime(2018, 07, 10), 300)
                .WithLateLevy(124, "18-19", 4, new DateTime(2018, 08, 10), 400)
                .WithLateLevy(125, "18-19", 5, new DateTime(2018, 09, 10), 500)
                .WithLateLevy(126, "18-19", 6, new DateTime(2018, 10, 10), 600)
                .WithLateLevy(127, "18-19", 7, new DateTime(2018, 11, 10), 700)
                .WithLateLevy(128, "18-19", 8, new DateTime(2018, 12, 10), 800)
                .WithLateLevy(129, "18-19", 9, new DateTime(2019, 01, 10), 900)
                .WithLateLevy(130, "18-19", 10, new DateTime(2019, 02, 10), 1000)
                .WithLateLevy(131, "18-19", 11, new DateTime(2019, 03, 10), 1100);

            fixtures.AssertAllRulesAreValid();
        }


        [Test]
        public void Scenario06_P1toP11WithSuperseded()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(121, "18-19", 2, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(122, "18-19", 2, new DateTime(2018, 05, 11), 200)

                .WithTransaction(122, 200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario07_P1toP12WithSupersededP12()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(121, "18-19", 2, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(122, "18-19", 12, new DateTime(2019, 04, 11), 200)
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2019, 04, 10), 400)

                .WithTransaction(121, 100)
                .WithTransaction(123, 300);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario08_P12WithSupersededP12()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(122, "18-19", 12, new DateTime(2019, 04, 11), 200)
                .WithOntimeLevy(123, "18-19", 12, new DateTime(2019, 04, 10), 400)

                .WithTransaction(123, 400);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario09_PostiveYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(122, "18-19", 12, new DateTime(2019, 04, 11), 200)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 400)

                .WithTransaction(122, 200)
                .WithTransaction(123, 200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario10_NegativeYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(122, "18-19", 12, new DateTime(2019, 04, 11), 200)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 100)

                .WithTransaction(122, 200)
                .WithTransaction(123, -100);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario11_OnTimePeriodP1toP11AndNegativeYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(122, "18-19", 2, new DateTime(2018, 06, 11), 200)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 100)

                .WithTransaction(122, 200)
                .WithTransaction(123, -100);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario13_OnTimePeriodP1toP11AndYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLateLevy(122, "18-19", 2, new DateTime(2018, 06, 11), 200)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 100)

                .WithTransaction(123, 100);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario14_OnTimeP1toP11WithSupersededAndYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(121, "18-19", 2, new DateTime(2018, 06, 11), 200)
                .WithOntimeLevy(122, "18-19", 2, new DateTime(2018, 06, 12), 300)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 500)

                .WithTransaction(122, 300)
                .WithTransaction(123, 200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario15_OnTimeP12WithSupersededP12AndYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(121, "18-19", 12, new DateTime(2019, 04, 11), 200)
                .WithOntimeLevy(122, "18-19", 12, new DateTime(2019, 04, 12), 300)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 500)

                .WithTransaction(122, 300)
                .WithTransaction(123, 200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario16_OnTimeP1toP11WithMultipleAdjacentYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(121, "18-19", 2, new DateTime(2018, 06, 11), 200)
                .WithOntimeLevy(122, "18-19", 3, new DateTime(2018, 07, 12), 300)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 05, 21), 500)
                .WithYearEndAdjustment(124, "18-19", new DateTime(2019, 05, 22), 800)
                .WithYearEndAdjustment(125, "18-19", new DateTime(2019, 05, 23), 1200)

                .WithTransaction(121, 200)
                .WithTransaction(122, 100)
                .WithTransaction(123, 200)
                .WithTransaction(124, 300)
                .WithTransaction(125, 400);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario17_OnTimeP12WithMultipleIsolatedYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithOntimeLevy(121, "18-19", 12, new DateTime(2019, 04, 10), 200)
                .WithYearEndAdjustment(122, "18-19", new DateTime(2019, 05, 21), 500)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 06, 22), 900)
                .WithYearEndAdjustment(124, "18-19", new DateTime(2019, 07, 23), 1400)

                .WithTransaction(121, 200)
                .WithTransaction(122, 300)
                .WithTransaction(123, 400)
                .WithTransaction(124, 500);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario18_LateP1toP11MultipleIsolatedYearEndAdjustments()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithLateLevy(121, "18-19", 2, new DateTime(2018, 05, 10), 200)
                .WithYearEndAdjustment(122, "18-19", new DateTime(2019, 05, 21), 500)
                .WithYearEndAdjustment(123, "18-19", new DateTime(2019, 06, 22), 900)
                .WithYearEndAdjustment(124, "18-19", new DateTime(2019, 07, 23), 1200)

                .WithTransaction(122, 500)
                .WithTransaction(123, 400)
                .WithTransaction(124, 300);

            fixtures.AssertAllRulesAreValid();
        }
    }
}
