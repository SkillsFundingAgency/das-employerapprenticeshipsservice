using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.Transfers;
using SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferAllowanceComponent
    {
        private TransfersController _controller;
        private GetTransferAllowanceQuery _query;
        private GetTransferAllowanceResponse _response;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
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
            _controller = new TransfersController(null, _mapper, _mediator.Object);
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