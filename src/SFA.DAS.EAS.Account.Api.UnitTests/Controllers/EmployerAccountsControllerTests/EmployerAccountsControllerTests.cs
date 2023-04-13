using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;


namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController _controller;
        protected Mock<ILogger<AccountsOrchestrator>> Logger;
        protected Mock<IUrlHelper> _urlHelper;
        protected Mock<IMapper> _mapper;
        protected Mock<IEncodingService> _hashingService;
        protected Mock<IEmployerAccountsApiService> _employerAccountsApiService;
        protected Mock<IEmployerFinanceApiService> _employerFinanceApiService;

        [SetUp]
        public void Arrange()
        {
             Logger = new Mock<ILogger<AccountsOrchestrator>>();
            _mapper = new Mock<IMapper>();
            _hashingService = new Mock<IEncodingService>();
            _employerAccountsApiService = new Mock<IEmployerAccountsApiService>();
            _employerFinanceApiService = new Mock<IEmployerFinanceApiService>();
          
            var orchestrator = new AccountsOrchestrator(Logger.Object, _mapper.Object, _hashingService.Object, _employerAccountsApiService.Object, _employerFinanceApiService.Object);
            _controller = new EmployerAccountsController(orchestrator, _employerAccountsApiService.Object);

            _urlHelper = new Mock<IUrlHelper>();
            _controller.Url = _urlHelper.Object;
        }
    }
}
