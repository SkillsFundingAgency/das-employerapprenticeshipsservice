using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.AddPayeToAccountTests
{
    public class WhenIAddPayeToAnAccount
    {
        private AddPayeToAccountCommandHandler _addPayeToAccountCommandHandler;
        private Mock<IValidator<AddPayeToAccountCommand>> _validator;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IHashingService> _hashingService;
        private const long ExpectedAccountId = 54564;

        [SetUp]
        public void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();

            _accountRepository = new Mock<IAccountRepository>();
            
            _validator = new Mock<IValidator<AddPayeToAccountCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddPayeToAccountCommand>())).ReturnsAsync(new ValidationResult());

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(ExpectedAccountId);

            _addPayeToAccountCommandHandler = new AddPayeToAccountCommandHandler(_validator.Object, _accountRepository.Object, _messagePublisher.Object, _hashingService.Object);
        }

        [Test]
        public void ThenTheValidatorIsCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddPayeToAccountCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string> { { "",""} }});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _addPayeToAccountCommandHandler.Handle(new AddPayeToAccountCommand()));

            //Assert
            _validator.Verify(x => x.ValidateAsync(It.IsAny<AddPayeToAccountCommand>()), Times.Once);
            _accountRepository.Verify(x => x.AddPayeToAccount(It.IsAny<Paye>()), Times.Never);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create();

            //Act
            await _addPayeToAccountCommandHandler.Handle(command);

            //Assert
            _accountRepository.Verify(x => x.AddPayeToAccount(
                                                AssertPayeScheme(command)), Times.Once);
        }

        [Test]
        public async Task ThenAMessageIsAddedToTheQueueToRefreshTheLevyData()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create();

            //Act
            await _addPayeToAccountCommandHandler.Handle(command);

            //Assert
            _messagePublisher.Verify(x=>x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(c=>c.AccountId.Equals(ExpectedAccountId))));
        }

        private static Paye AssertPayeScheme(AddPayeToAccountCommand command)
        {
            return It.Is<Paye>(
                c=>c.AccessToken.Equals(command.AccessToken)  &&
                   c.RefreshToken.Equals(command.RefreshToken) &&
                   c.EmpRef.Equals(command.Empref) &&
                   c.AccountId.Equals(ExpectedAccountId)
                );
        }
    }
}
