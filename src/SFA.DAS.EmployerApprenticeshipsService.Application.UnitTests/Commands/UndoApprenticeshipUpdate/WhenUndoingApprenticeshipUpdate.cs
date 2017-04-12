using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Application.Commands.UndoApprenticeshipUpdate;

using ValidationResult = SFA.DAS.EAS.Application.Validation.ValidationResult;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UndoApprenticeshipUpdate
{
    [TestFixture]
    public class WhenUndoingApprenticeshipUpdate
    {
        private UndoApprenticeshipUpdateCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
        private Mock<Validation.IValidator<UndoApprenticeshipUpdateCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<Validation.IValidator<UndoApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<UndoApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult());

            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.PatchApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateSubmission>()))
                .Returns(() => Task.FromResult(new Unit()));

            _handler = new UndoApprenticeshipUpdateCommandHandler(_validator.Object, _commitmentsApi.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new UndoApprenticeshipUpdateCommand();

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<UndoApprenticeshipUpdateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenIfTheRequestIsNotValidThenAnExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UndoApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string> { {"Test", "Error"} }
                });

            var command = new UndoApprenticeshipUpdateCommand();

            //Act & Assert
            Func<Task> act = async () => { await _handler.Handle(command); };
            act.ShouldThrow<ValidationException>();
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToSubmitTheUpdate()
        {
            //Arrange
            var command = new UndoApprenticeshipUpdateCommand
            {
                ApprenticeshipId = 1,
                AccountId = 2,
                UserId = "tester"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(x => x.PatchApprenticeshipUpdate(
                It.IsAny<long>(),
                It.IsAny<long>(),
                It.Is<ApprenticeshipUpdateSubmission>(s => s.UpdateStatus == ApprenticeshipUpdateStatus.Deleted)),
                Times.Once);
        }
    }
}
