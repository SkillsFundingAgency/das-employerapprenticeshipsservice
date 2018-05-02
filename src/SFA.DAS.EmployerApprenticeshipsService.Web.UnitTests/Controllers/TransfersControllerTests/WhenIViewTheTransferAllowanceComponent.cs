using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Mappings;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersControllerTests
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

        [SetUp]
        public void Arrange()
        {
            _query = new GetTransferAllowanceQuery();
            _response = new GetTransferAllowanceResponse { TransferAllowance = 123.456m };
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
            Assert.That(model.TransferAllowance, Is.EqualTo(_response.TransferAllowance));
        }
    }
}