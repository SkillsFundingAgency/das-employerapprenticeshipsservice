using System.Threading.Tasks;

using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateApprenticeshipUpdate
{
    [TestFixture]
    public class WhenCreatingApprenticeshipUpdate
    {
        private Mock<IEmployerCommitmentApi> _commitmentsApi;

        private Mock<IValidator<CreateApprenticeshipUpdateCommand>> _validator;

        private CreateApprenticeshipUpdateCommandHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(m => m.CreateApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateRequest>()))
                .Returns(() => Task.FromResult(new Unit()));

            _validator = new Mock<IValidator<CreateApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult());

            _handler = new CreateApprenticeshipUpdateCommandHandler(_commitmentsApi.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheCOmmandIsValidated()
        {
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate(),
                EmployerId = 321,
                UserId = "Tester" 
            };

            await _handler.Handle(command);

            _validator.Verify(m => m.Validate(It.IsAny<CreateApprenticeshipUpdateCommand>()), Times.Once());
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToCreateTheUpdate()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate { ApprenticeshipId = 987 },
                EmployerId = 321,
                UserId = "Tester",
                UserEmailAddress = "test@email.com",
                UserDisplayName = "Bob"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(
                x =>
                    x.CreateApprenticeshipUpdate(command.EmployerId, command.ApprenticeshipUpdate.ApprenticeshipId,
                        It.Is<ApprenticeshipUpdateRequest>(
                            y =>
                                y.ApprenticeshipUpdate == command.ApprenticeshipUpdate && y.UserId == command.UserId && y.LastUpdatedByInfo.Name == command.UserDisplayName &&
                                y.LastUpdatedByInfo.EmailAddress == command.UserEmailAddress)), Times.Once);
        }
    }
}
