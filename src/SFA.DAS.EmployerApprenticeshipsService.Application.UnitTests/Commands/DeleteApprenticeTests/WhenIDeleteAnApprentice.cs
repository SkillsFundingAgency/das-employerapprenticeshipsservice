using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.DeleteApprentice;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.DeleteApprenticeTests
{
    [TestFixture]
    public class WhenIDeleteAnApprentice
    {
        private Mock<ICommitmentsService> _commitmentsService;
        private Mock<IValidator<DeleteApprenticeCommand>> _validator;
        private DeleteApprenticeCommandHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _commitmentsService = new Mock<ICommitmentsService>();
            _commitmentsService.Setup(x => x.DeleteEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<object>(null));

            _validator = new Mock<IValidator<DeleteApprenticeCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeleteApprenticeCommand>()))
                .Returns(new ValidationResult());

            _handler = new DeleteApprenticeCommandHandler(_commitmentsService.Object, _validator.Object);
        }

        [Test]
        public async Task TheCommitmentsServiceShouldBeCalledIfTheRequestIsValid()
        {
            //Arrange
            var command = new DeleteApprenticeCommand
            {
                AccountId = 1,
                ApprenticeshipId = 2
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsService.Verify(x => x.DeleteEmployerApprenticeship(It.Is<long>(l=> l==command.AccountId), It.Is<long>(l=>l == command.ApprenticeshipId)), Times.Once);
        }


        [Test]
        public async Task ThenIShouldGetAInvalidRequestExceptionIfValidationFails()
        {
            //Arrange
            var command = new DeleteApprenticeCommand();
            _validator = new Mock<IValidator<DeleteApprenticeCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeleteApprenticeCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        { "test", "test validation error"}
                    }
                });

            _handler = new DeleteApprenticeCommandHandler(_commitmentsService.Object, _validator.Object);

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(command));
        }

    }
}
