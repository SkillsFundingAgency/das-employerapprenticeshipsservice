using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Queries.GetHealthCheck;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Mappings;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HealthCheckControllerTests : FluentTest<HealthCheckControllerTestsFixture>
    {
        [Test]
        public Task Index_WhenGettingTheIndexAction_ThenShouldReturnTheIndexView()
        {
            return RunAsync(f => f.Index(), (f, r) =>
            {
                r.Should().NotBeNull().And.Match<ViewResult>(a => a.ViewName == "");
                r.As<ViewResult>().Model.Should().NotBeNull().And.Match<HealthCheckViewModel>(m => m.HealthCheck == f.GetHealthCheckQueryResponse.HealthCheck);
            });
        }

        [Test]
        public Task Index_WhenPostingTheIndexAction_ThenShouldRedirectToTheIndexAction()
        {
            return RunAsync(f => f.PostIndex(), (f, r) => r.Should().NotBeNull().And.Match<RedirectToRouteResult>(a =>
                a.RouteValues["Action"].Equals("Index") &&
                a.RouteValues["Controller"] == null));
        }
    }

    public class HealthCheckControllerTestsFixture
    {
        public HealthCheckController HealthCheckController { get; set; }
        public GetHealthCheckQuery GetHealthCheckQuery { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public IMapper Mapper { get; set; }
        public GetHealthCheckQueryResponse GetHealthCheckQueryResponse { get; set; }
        public RunHealthCheckCommand RunHealthCheckCommand { get; set; }

        public HealthCheckControllerTestsFixture()
        {
            Mediator = new Mock<IMediator>();
            Mapper = new MapperConfiguration(c => c.AddProfile<HealthCheckMappings>()).CreateMapper();
            HealthCheckController = new HealthCheckController(Mediator.Object, Mapper);
        }

        public Task<ActionResult> Index()
        {
            GetHealthCheckQuery = new GetHealthCheckQuery();

            GetHealthCheckQueryResponse = new GetHealthCheckQueryResponse
            {
                HealthCheck = new HealthCheckDto()
            };

            Mediator.Setup(m => m.SendAsync(GetHealthCheckQuery)).ReturnsAsync(GetHealthCheckQueryResponse);

            return HealthCheckController.Index(GetHealthCheckQuery);
        }

        public Task<ActionResult> PostIndex()
        {
            RunHealthCheckCommand = new RunHealthCheckCommand();

            Mediator.Setup(m => m.SendAsync(RunHealthCheckCommand)).ReturnsAsync(Unit.Value);

            return HealthCheckController.Index(RunHealthCheckCommand);
        }
    }
}