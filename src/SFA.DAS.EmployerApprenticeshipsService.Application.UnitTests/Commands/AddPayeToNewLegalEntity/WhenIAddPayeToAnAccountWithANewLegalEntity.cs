using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.AddPayeToNewLegalEntity
{
    public class WhenIAddPayeToAnAccountWithANewLegalEntity
    {
        private AddPayeToNewLegalEnttiyCommandHandler _addPayeToNewLegalEnttiyCommandHandler;
        private Mock<IValidator<AddPayeToNewLegalEntityCommand>> _validator;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IHashingService> _hashingService;
        private const long ExpectedAccountId = 54564;

        [SetUp]
        public void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();

            _accountRepository = new Mock<IAccountRepository>();
            
            _validator = new Mock<IValidator<AddPayeToNewLegalEntityCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddPayeToNewLegalEntityCommand>())).ReturnsAsync(new ValidationResult());

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(ExpectedAccountId);

            _addPayeToNewLegalEnttiyCommandHandler = new AddPayeToNewLegalEnttiyCommandHandler(_validator.Object, _accountRepository.Object, _messagePublisher.Object, _hashingService.Object);
        }

        [Test]
        public void ThenTheValidatorIsCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddPayeToNewLegalEntityCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string> { { "",""} }});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _addPayeToNewLegalEnttiyCommandHandler.Handle(new AddPayeToNewLegalEntityCommand()));

            //Assert
            _validator.Verify(x => x.ValidateAsync(It.IsAny<AddPayeToNewLegalEntityCommand>()), Times.Once);
            _accountRepository.Verify(x => x.AddPayeToAccountForNewLegalEntity(It.IsAny<Paye>(), It.IsAny<LegalEntity>()), Times.Never);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create();

            //Act
            await _addPayeToNewLegalEnttiyCommandHandler.Handle(command);

            //Assert
            _accountRepository.Verify(x => x.AddPayeToAccountForNewLegalEntity(
                                                AssertPayeScheme(command),
                                                AssertLegalEntity(command)), Times.Once);
        }

        [Test]
        public async Task ThenAMessageIsAddedToTheQueueToRefreshTheLevyData()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create();

            //Act
            await _addPayeToNewLegalEnttiyCommandHandler.Handle(command);

            //Assert
            _messagePublisher.Verify(x=>x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(c=>c.AccountId.Equals(ExpectedAccountId))));
        }


        private static LegalEntity AssertLegalEntity(AddPayeToNewLegalEntityCommand command)
        {
            return It.Is<LegalEntity>(
                c=>c.Name.Equals(command.LegalEntityName) &&
                c.Code.Equals(command.LegalEntityCode) &&
                c.DateOfIncorporation.Equals(command.LegalEntityDate) &&
                c.RegisteredAddress.Equals(command.LegalEntityAddress)
                );
        }

        private static Paye AssertPayeScheme(AddPayeToNewLegalEntityCommand command)
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
