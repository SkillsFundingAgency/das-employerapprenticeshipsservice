using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.NLog.Logger;
using AutoMapper;
using AutoFixture;
using System;
using System.Reflection;
using Castle.Core.Internal;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTheTransactionSummaryForAnAccount
    {
        private AccountTransactionsController _controller;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<ILog> _logger;
        private Mock<UrlHelper> _urlHelper;
        private  Mock<IEmployerFinanceApiService> _financeApiService;
        protected IMapper Mapper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _urlHelper = new Mock<UrlHelper>();
            _mapper = new Mock<IMapper>();
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            Mapper = ConfigureMapper();
            var orchestrator = new AccountTransactionsOrchestrator(_mediator.Object, Mapper, _logger.Object, _financeApiService.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object;
        }

        [Test]
        public async Task ThenTheTransactionSummaryIsReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";           
            var fixture = new Fixture();
            ICollection<Finance.Api.Types.TransactionSummaryViewModel> apiResponse = new List<Finance.Api.Types.TransactionSummaryViewModel>()
            {
                 fixture.Create<Finance.Api.Types.TransactionSummaryViewModel>(),
                 fixture.Create<Finance.Api.Types.TransactionSummaryViewModel>()
            };

            _financeApiService.Setup(x => x.GetTransactionSummary(hashedAccountId)).ReturnsAsync(apiResponse);

            var firstExpectedUri = "someuri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.First().Year, month = apiResponse.First().Month }))))
                .Returns(firstExpectedUri);
            var secondExpectedUri = "someotheruri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.Last().Year, month = apiResponse.Last().Month }))))
                .Returns(secondExpectedUri);

            //Act
            var response = await _controller.Index(hashedAccountId);

            
            //Assert            
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountResourceList<TransactionSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<AccountResourceList<TransactionSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(apiResponse, x => x.Excluding(y => y.Href));
            model?.Content.First().Href.Should().Be(firstExpectedUri);
            model?.Content.Last().Href.Should().Be(secondExpectedUri);
        }

        private IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load($"SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}
