using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetALegalEntity
    {
        private LegalEntitiesController _controller;
        private Mock<IMediator> _mediator;
        private GetLegalEntityResponse _response;
        private LegalEntity _legalEntity;
        private long _legalEntityId;
        private string _accountHashedId;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _legalEntityId = long.MaxValue;
            _accountHashedId = "H4#h3d";
            _legalEntity = new LegalEntity();
            _response = new GetLegalEntityResponse { LegalEntity = _legalEntity };

            _mediator.Setup(m => m.SendAsync(
                    It.Is<GetLegalEntityQuery>(
                        q => q.AccountHashedId.Equals(_accountHashedId) && q.LegalEntityId.Equals(_legalEntityId))))
                .ReturnsAsync(_response);

            _controller = new LegalEntitiesController(_mediator.Object);
        }

        [Test]
        public async Task ThenAGetLegalEntityQueryShouldBeSent()
        {
            await _controller.GetLegalEntity(_accountHashedId, _legalEntityId);

            _mediator.Verify(
                m => m.SendAsync(
                    It.Is<GetLegalEntityQuery>(
                        q => q.AccountHashedId.Equals(_accountHashedId) && q.LegalEntityId.Equals(_legalEntityId) && q.IncludeAllAgreements.Equals(false))),
                Times.Once);
        }

        [Test]
        public async Task ThenAGetLegalEntityQueryShouldBeSentWithIncludeAllAgreementsSwitch()
        {
            await _controller.GetLegalEntity(_accountHashedId, _legalEntityId, true);

            _mediator.Verify(
                m => m.SendAsync(
                    It.Is<GetLegalEntityQuery>(
                        q => q.AccountHashedId.Equals(_accountHashedId) && q.LegalEntityId.Equals(_legalEntityId) && q.IncludeAllAgreements.Equals(true))),
                Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnLegalEntity()
        {
            var result = await _controller.GetLegalEntity(_accountHashedId, _legalEntityId) as OkNegotiatedContentResult<LegalEntity>;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(_legalEntity));
        }
    }
}
