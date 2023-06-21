using NUnit.Framework;
using SFA.DAS.EAS.Support.Infrastructure.Services;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.Services;

[TestFixture]
public class WhenTestingDatetimeService
{
    [SetUp]
    public void Setup()
    {
        _unit = new DatetimeService();
    }

    private DatetimeService? _unit;

    [TestCase("1-Apr-2016", "1-Apr-2016")]
    [TestCase("1-May-2016", "1-Apr-2016")]
    [TestCase("1-Jun-2016", "1-Apr-2016")]
    [TestCase("1-Jul-2016", "1-Apr-2016")]
    [TestCase("1-Aug-2016", "1-Apr-2016")]
    [TestCase("1-Sep-2016", "1-Apr-2016")]
    [TestCase("1-Oct-2016", "1-Apr-2016")]
    [TestCase("1-Nov-2016", "1-Apr-2016")]
    [TestCase("1-Dec-2016", "1-Apr-2016")]
    [TestCase("1-Jan-2017", "1-Apr-2016")]
    [TestCase("1-Feb-2017", "1-Apr-2016")]
    [TestCase("1-Mar-2017", "1-Apr-2016")]
    [TestCase("31-Mar-2017", "1-Apr-2016")]
    [TestCase("1-Apr-2017", "1-Apr-2017")]
    public void ItShouldTestTheBehaviour(string testDate, string expected)
    {
        Assert.That(_unit!.GetBeginningFinancialYear(DateTime.Parse(testDate)), Is.EqualTo(DateTime.Parse(expected)));
    }
}