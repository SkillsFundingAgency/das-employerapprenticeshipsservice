using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.DeleteApprentice;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.DeleteApprenticeTests
{
    [TestFixture]
    public class WhenIDeleteAnApprentice
    {
        private Mock<ICommitmentsApi> _commitmentsService;
        private Mock<IValidator<DeleteApprenticeshipCommand>> _validator;
        private DeleteApprenticeshipCommandHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _commitmentsService = new Mock<ICommitmentsApi>();
            _commitmentsService.Setup(x => x.DeleteEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DeleteRequest>()))
                .Returns(Task.FromResult<object>(null));

            _validator = new Mock<IValidator<DeleteApprenticeshipCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeleteApprenticeshipCommand>()))
                .Returns(new ValidationResult());

            _handler = new DeleteApprenticeshipCommandHandler(_commitmentsService.Object, _validator.Object);
        }

        [Test]
        public async Task TheCommitmentsServiceShouldBeCalledIfTheRequestIsValid()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand
            {
                AccountId = 1,
                ApprenticeshipId = 2
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsService.Verify(x => x.DeleteEmployerApprenticeship(It.Is<long>(l=> l==command.AccountId), It.Is<long>(l=>l == command.ApprenticeshipId), It.IsAny<DeleteRequest>()), Times.Once);
        }

        [Test]
        public void ThenIShouldGetAInvalidRequestExceptionIfValidationFails()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand();
            _validator = new Mock<IValidator<DeleteApprenticeshipCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeleteApprenticeshipCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        { "test", "test validation error"}
                    }
                });

            _handler = new DeleteApprenticeshipCommandHandler(_commitmentsService.Object, _validator.Object);

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(command));
        }
    }
}
