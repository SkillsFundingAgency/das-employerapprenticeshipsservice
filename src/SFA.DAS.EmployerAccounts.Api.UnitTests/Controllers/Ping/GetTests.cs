using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.Ping
{
    [TestFixture]
    [Parallelizable]
    public class GetTests : FluentTest<GetTestsFixture>
    {
        [Test]
        public void WhenGettingGetAction_ThenShouldReturnOkResult()
        {
            Test(f => f.Get(), (f, r) => r.Should().NotBeNull().And.BeOfType<OkResult>());
        }
    }

    public class GetTestsFixture
    {
        public PingController Controller { get; set; }

        public GetTestsFixture()
        {
            Controller = new PingController();
        }

        public IActionResult Get()
        {
            return Controller.Get();
        }
    }
}