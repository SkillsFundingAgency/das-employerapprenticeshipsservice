using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Models
{
    [TestFixture]
    public class DateTimeViewModelTests
    {
        [Test]
        public void SholudBeNull()
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
        public void ShouldBeNull(int? day, int? month, int? year)
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

        [TestCase(null, 2, 3, "01/02/2103")]
        [TestCase(null, 2, 99, "01/02/2099")]
        [TestCase(15, 12, 1995, "15/12/1995")]
        [TestCase(12, 12, 2024, "12/12/2024")]
        [TestCase(1, 12, 59, "01/12/2059")]
        [TestCase(1, 12, 60, "01/12/2060")]
        [TestCase(1, 12, 61, "01/12/2061")]
        public void ShouldBeValidFuture(int? day, int? month, int? year, string expected)
        {
            var sut = new DateTimeViewModel(day, month, year);
            sut.DateTime?.ToString("dd/MM/yyyy").Should().Be(expected);
        }

        [Test]
        public void ShouldBeValidPast()
        {
            var year = DateTime.Now.Year - 5;
            var yearTwoDigit = int.Parse(year.ToString().Substring(2));

            var sut = new DateTimeViewModel(null, 2, yearTwoDigit, 0);
            (sut.DateTime == null).Should().BeFalse();
            sut.DateTime?.ToString("dd/MM/yyyy").Should().Be("01/02/" + year);
        }
    }
}
