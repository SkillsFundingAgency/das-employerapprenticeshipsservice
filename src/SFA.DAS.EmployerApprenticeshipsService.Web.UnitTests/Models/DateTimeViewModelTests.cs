using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Models.Types;

namespace SFA.DAS.EAS.Web.UnitTests.Models
{
    [TestFixture]
    public sealed class DateTimeViewModelTests
    {
        [Test]
        public void ShouldBeNullWhenPassedNull()
        {
            var sut = new DateTimeViewModel(null);
            sut.DateTime.Should().Be(null);
        }

        [TestCase(1, 2, null)]
        [TestCase(1, null, 3)]
        [TestCase(31, 2, 2015)]
        [TestCase(-1, 2, 2015)]
        [TestCase(1, 0, 2015)]
        [TestCase(1, 13, 2015)]
        [TestCase(1, 12, -1)]
        [TestCase(1, 12, 999)]
        public void ShouldBeNullWhenPassedInvalidDateValues(int? day, int? month, int? year)
        {
            var sut = new DateTimeViewModel(day, month, year);
            sut.DateTime.Should().NotHaveValue();
        }

        [Test]
        public void ShouldBeValid()
        {
            var sut = new DateTimeViewModel(DateTime.Parse("2009-09-25"));
            sut.DateTime?.ToString("dd/MM/yyyy").Should().Be("25/09/2009");
        }

        [TestCase(null, 2, 3, "01/02/2003")]
        [TestCase(28, 2, 13, "28/02/2013")]
        [TestCase(null, 2, 99, "01/02/1999")]
        [TestCase(15, 12, 1995, "15/12/1995")]
        [TestCase(12, 12, 2024, "12/12/2024")]
        public void ShouldBeValid(int? day, int? month, int? year, string expected)
        {
            var sut = new DateTimeViewModel(day, month, year);
            sut.DateTime?.ToString("dd/MM/yyyy").Should().Be(expected);
        }
    }
}
