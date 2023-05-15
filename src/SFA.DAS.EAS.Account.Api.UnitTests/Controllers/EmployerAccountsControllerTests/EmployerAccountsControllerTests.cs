using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests;

public abstract class EmployerAccountsControllerTests
{
    protected EmployerAccountsController? Controller;
    private Mock<ILogger<AccountsOrchestrator>>? _logger;
    private Mock<IUrlHelper>? _urlHelper;
    private Mock<IMapper>? _mapper;
    protected Mock<IEncodingService>? EncodingService;
    protected Mock<IEmployerAccountsApiService>? EmployerAccountsApiService;
    protected Mock<IEmployerFinanceApiService>? EmployerFinanceApiService;

    [SetUp]
    public void Arrange()
    {
        _logger = new Mock<ILogger<AccountsOrchestrator>>();
        _mapper = new Mock<IMapper>();
        EncodingService = new Mock<IEncodingService>();
        EmployerAccountsApiService = new Mock<IEmployerAccountsApiService>();
        EmployerFinanceApiService = new Mock<IEmployerFinanceApiService>();
          
        var orchestrator = new AccountsOrchestrator(_logger.Object, _mapper.Object, EncodingService.Object, EmployerAccountsApiService.Object, EmployerFinanceApiService.Object);
        Controller = new EmployerAccountsController(orchestrator, EmployerAccountsApiService.Object);

        _urlHelper = new Mock<IUrlHelper>();
        Controller.Url = _urlHelper.Object;
    }
}