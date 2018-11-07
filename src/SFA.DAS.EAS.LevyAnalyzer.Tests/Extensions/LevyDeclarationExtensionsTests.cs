using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Tests.Extensions
{
    [TestFixture]
    public class LevyDeclarationExtensionsTests
    {
        [Test]
        public void GroupByPayrollPeriod_MultipleDeclarationsInSamePeriod_ShouldBeGrouped()
        {
            var levyDeclarations = new LevyDeclaration[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1}
            };

            var groups = levyDeclarations.GroupByPayrollPeriod().ToArray();

            Assert.AreEqual(1, groups.Length);
        }

        [Test]
        public void GroupByPayrollPeriod_MultipleDeclarationsInDifferentPeriods_ShouldNotBeGrouped()
        {
            var levyDeclarations = new LevyDeclaration[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1},
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 2}
            };

            var groups = levyDeclarations.GroupByPayrollPeriod().ToArray();

            Assert.AreEqual(2, groups.Length);
        }

        [Test]
        public void GroupByPayrollPeriod_MultipleDeclarationsInSameMonthButDifferentYear_ShouldNotBeGrouped()
        {
            var levyDeclarations = new LevyDeclaration[]
            {
                new LevyDeclaration {PayrollYear = "17-18", PayrollMonth = 1},
                new LevyDeclaration {PayrollYear = "18-19", PayrollMonth = 1}
            };

            var groups = levyDeclarations.GroupByPayrollPeriod().ToArray();

            Assert.AreEqual(2, groups.Length);
        }

    }
}
