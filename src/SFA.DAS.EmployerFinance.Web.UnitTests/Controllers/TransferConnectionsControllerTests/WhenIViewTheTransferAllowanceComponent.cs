using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Queries.GetTransferAllowance;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Mappings;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferAllowanceComponent
    {
        private TransferConnectionsController _controller;
        private GetTransferAllowanceQuery _query;
        private GetTransferAllowanceResponse _response;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureToggleService;
        private TransferAllowance _transferAllowance;
        private EmployerFinanceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerFinanceConfiguration { TransferAllowancePercentage = .25m };
            _transferAllowance = new TransferAllowance
            {
                RemainingTransferAllowance = 123.456m,
                StartingTransferAllowance = 234.56M,
            };
            _query = new GetTransferAllowanceQuery();
            _response = new GetTransferAllowanceResponse { TransferAllowance = _transferAllowance, TransferAllowancePercentage = _configuration.TransferAllowancePercentage };
            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _featureToggleService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();

            _controller = new TransferConnectionsController(null, _mapper, _mediator.Object, _featureToggleService.Object);
        }

        [Test]
        public void ThenAGetTransferAllowanceQueryShouldBeSent()
        {
            _controller.TransferAllowance(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public void ThenIShouldBeShownTheTransferAllowanceComponent()
        {
            var result = _controller.TransferAllowance(_query) as PartialViewResult;
            var model = result?.Model as TransferAllowanceViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model.RemainingTransferAllowance, Is.EqualTo(_response.TransferAllowance.RemainingTransferAllowance));
        }

        [Test]
        public void ThenIShouldBeShownTheCorrectTransferAllowancePercentage()
        {
            //Act
            var result = _controller.TransferAllowance(_query) as PartialViewResult;
            var model = result?.Model as TransferAllowanceViewModel;

            //Assert
            Assert.AreEqual(25m, model.TransferAllowancePercentage);
        }
    }
}