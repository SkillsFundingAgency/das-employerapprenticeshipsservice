using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetALegalEntity
    {
        private LegalEntitiesController _controller;
        private Mock<IMediator> _mediator;
        private GetLegalEntityQuery _query;
        private GetLegalEntityResponse _response;
        private LegalEntity _legalEntity;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _query = new GetLegalEntityQuery();
            _legalEntity = new LegalEntity();
            _response = new GetLegalEntityResponse { LegalEntity = _legalEntity };

            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _controller = new LegalEntitiesController(_mediator.Object);
        }

        [Test]
        public async Task ThenAGetLegalEntityQueryhouldBeSent()
        {
            await _controller.GetLegalEntity(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnLegalEntity()
        {
            var result = await _controller.GetLegalEntity(_query) as OkNegotiatedContentResult<LegalEntity>;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(_legalEntity));
        }
    }
}
