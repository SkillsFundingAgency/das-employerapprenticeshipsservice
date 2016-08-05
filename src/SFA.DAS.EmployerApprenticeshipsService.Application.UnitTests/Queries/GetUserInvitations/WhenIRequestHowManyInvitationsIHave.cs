using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetUserInvitations
{
    public class WhenIRequestHowManyInvitationsIHave
    {
        private GetNumberOfUserInvitationsHandler _getUserInvitationsCountHandler;
        private Mock<IValidator<GetNumberOfUserInvitationsQuery>> _validator;
        private Mock<IInvitationRepository> _invitationRepository;
        private string ExpectedEmployerId = "1234dfe";

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetNumberOfUserInvitationsQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetNumberOfUserInvitationsQuery>())).Returns(new ValidationResult());

            _invitationRepository = new Mock<IInvitationRepository>();
            _invitationRepository.Setup(x => x.GetNumberOfInvites(ExpectedEmployerId)).ReturnsAsync(1);

            _getUserInvitationsCountHandler = new GetNumberOfUserInvitationsHandler(_validator.Object, _invitationRepository.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _getUserInvitationsCountHandler.Handle(new GetNumberOfUserInvitationsQuery());

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<GetNumberOfUserInvitationsQuery>()), Times.Once);
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheMessageIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<GetNumberOfUserInvitationsQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _getUserInvitationsCountHandler.Handle(new GetNumberOfUserInvitationsQuery()));
            _invitationRepository.Verify(x => x.GetNumberOfInvites(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            await _getUserInvitationsCountHandler.Handle(new GetNumberOfUserInvitationsQuery { UserId = ExpectedEmployerId });

            //Act
            _invitationRepository.Verify(x => x.GetNumberOfInvites(It.Is<string>(c => c.Equals(ExpectedEmployerId))), Times.Once);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var actual = await _getUserInvitationsCountHandler.Handle(new GetNumberOfUserInvitationsQuery { UserId = ExpectedEmployerId });

            //Assert
            Assert.AreEqual(1,actual.NumberOfInvites);

        }
    }
}
