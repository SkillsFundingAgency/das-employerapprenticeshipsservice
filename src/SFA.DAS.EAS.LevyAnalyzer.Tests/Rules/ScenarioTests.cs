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
        public void Scenario1_P1toP11AllOnTime()
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
        public void Scenario2_P12OnTime()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(132, "18-19", 12, new DateTime(2019, 04, 10), 1200)

                .WithTransaction(132, 1200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario3_P1toP12AllOnTime()
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
        public void Scenario4_P1toP11SomeLate()
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
        public void Scenario5_P1toP11AllLate()
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
        public void Scenario6_P1toP11WithSuperseded()
        {
            var fixtures = new LevyAnalyzerTestFixtures()
                .WithEmpRef("ABC/012345")
                .WithOntimeLevy(121, "18-19", 2, new DateTime(2018, 05, 10), 100)
                .WithOntimeLevy(122, "18-19", 2, new DateTime(2018, 05, 11), 200)

                .WithTransaction(122, 200);

            fixtures.AssertAllRulesAreValid();
        }

        [Test]
        public void Scenario7_P1toP12WithSupersededPeriod12()
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
    }
}
