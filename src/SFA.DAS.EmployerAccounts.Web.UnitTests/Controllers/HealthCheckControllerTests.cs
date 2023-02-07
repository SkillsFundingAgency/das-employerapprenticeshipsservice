using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers;

[TestFixture]
public class HealthCheckControllerTests : FluentTest<HealthCheckControllerTestsFixture>
{
    [Test]
    public Task Index_WhenGettingTheIndexAction_ThenShouldReturnTheIndexView()
    {
        return TestAsync(fixture => fixture.Index(), (fixture, result) =>
        {
            result.Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().BeNull();

            var model = result.As<ViewResult>().Model as HealthCheckViewModel;

            model.Should().NotBeNull();
            model.HealthCheck.Should().Be(fixture.GetHealthCheckQueryResponse.HealthCheck);
        });
    }

    [Test]
    public Task Index_WhenPostingTheIndexAction_ThenShouldRedirectToTheIndexAction()
    {
        return TestAsync(fixture => fixture.PostIndex(), (fixture, result) =>
        {
            result.Should().NotBeNull();
            
            var redirect = (RedirectToActionResult)result ;
            redirect.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().BeNull();
        });
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

    public Task<IActionResult> Index()
    {
        GetHealthCheckQuery = new GetHealthCheckQuery();

        GetHealthCheckQueryResponse = new GetHealthCheckQueryResponse
        {
            HealthCheck = new HealthCheckDto()
        };

        Mediator.Setup(m => m.Send(GetHealthCheckQuery, It.IsAny<CancellationToken>())).ReturnsAsync(GetHealthCheckQueryResponse);

        return HealthCheckController.Index(GetHealthCheckQuery);
    }

    public Task<IActionResult> PostIndex()
    {
        RunHealthCheckCommand = new RunHealthCheckCommand();

        Mediator.Setup(m => m.Send(RunHealthCheckCommand, It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        return HealthCheckController.Index(RunHealthCheckCommand);
    }
}