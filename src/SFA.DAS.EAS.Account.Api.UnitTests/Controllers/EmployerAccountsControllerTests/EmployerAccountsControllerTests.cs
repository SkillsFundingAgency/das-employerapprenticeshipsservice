using System.Web.Http.Routing;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController _controller;
        protected Mock<ILog> Logger;
        protected Mock<UrlHelper> _urlHelper;
        protected Mock<IMapper> _mapper;
        protected Mock<IHashingService> _hashingService;
        protected Mock<IEmployerAccountsApiService> _employerAccountsApiService;
        protected Mock<IEmployerFinanceApiService> _employerFinanceApiService;

        [SetUp]
        public void Arrange()
        {
            Logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();
            _hashingService = new Mock<IHashingService>();
            _employerAccountsApiService = new Mock<IEmployerAccountsApiService>();
            _employerFinanceApiService = new Mock<IEmployerFinanceApiService>();
          
            var orchestrator = new AccountsOrchestrator(Logger.Object, _mapper.Object, _hashingService.Object, _employerAccountsApiService.Object, _employerFinanceApiService.Object);
            _controller = new EmployerAccountsController(orchestrator, _employerAccountsApiService.Object);

            _urlHelper = new Mock<UrlHelper>();
            _controller.Url = _urlHelper.Object;
        }
    }
}
