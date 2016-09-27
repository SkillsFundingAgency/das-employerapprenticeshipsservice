using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateAccountCommandTests
{
    public class WhenICallCreateAccount
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IUserRepository> _userRepository;
        private CreateAccountCommandHandler _handler;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IMediator> _mediator;
        private Mock<IValidator<CreateAccountCommand>> _validator;
        private Mock<IHashingService> _hashingService;
        private const long ExpectedAccountId = 12343322;
        private const long ExpectedLegalEntityId = 2222;
        private const string ExpectedHashString = "123ADF23";

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _accountRepository.Setup(x => x.GetPayeSchemes(ExpectedAccountId)).ReturnsAsync(new List<PayeView> { new PayeView { LegalEntityId = ExpectedLegalEntityId } });
            _accountRepository.Setup(x => x.CreateAccount(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(ExpectedAccountId);

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(new User());

            _messagePublisher = new Mock<IMessagePublisher>();
            _mediator = new Mock<IMediator>();
            _validator = new Mock<IValidator<CreateAccountCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateAccountCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(ExpectedAccountId)).Returns(ExpectedHashString);

            _handler = new CreateAccountCommandHandler(_accountRepository.Object, _userRepository.Object, _messagePublisher.Object, _mediator.Object, _validator.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenIfThereAreMoreThanOneEmprefPassedThenTheyAreAddedToTheAccount()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand {EmployerRef = "123/abc,456/123", AccessToken = "123rd",RefreshToken = "45YT"};
            
            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _accountRepository.Verify(x=>x.CreateAccount(It.IsAny<long>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<DateTime>(),"123/abc", "123rd", "45YT", false), Times.Once);
            _accountRepository.Verify(x=>x.GetPayeSchemes(ExpectedAccountId), Times.Once);
            _accountRepository.Verify(x=>x.AddPayeToAccountForExistingLegalEntity(ExpectedAccountId, ExpectedLegalEntityId, "456/123","123rd","45YT"), Times.Once);
        }

        [Test]
        public async Task ThenTheIdHashingServiceIsCalledAfterTheAccountIsCreated()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand { EmployerRef = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT" };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _hashingService.Verify(x=>x.HashValue(ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenTheAccountIsUpdatedWithTheHashedId()
        {
            //Arrange
            
            var createAccountCommand = new CreateAccountCommand { EmployerRef = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT" };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _accountRepository.Verify(x=>x.SetHashedId(ExpectedHashString, ExpectedAccountId),Times.Once);
        }

        [Test]
        public async Task WillCallRepositoryToCreateNewAccount()
        {
            const int accountId = 23;

            var user = new User()
            {
                Id = 33
            };

            var cmd = new CreateAccountCommand
            {
                ExternalUserId = Guid.NewGuid().ToString(),
                CompanyNumber = "QWERTY",
                CompanyName = "Qwerty Corp",
                CompanyRegisteredAddress = "Innovation Centre, Coventry, CV1 2TT",
                CompanyDateOfIncorporation = DateTime.Today.AddDays(-1000),
                EmployerRef = "120/QWERTY",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                SignAgreement = false
            };

            _userRepository.Setup(x => x.GetById(cmd.ExternalUserId)).ReturnsAsync(user);
            _accountRepository.Setup(x => x.CreateAccount(user.Id, cmd.CompanyNumber, cmd.CompanyName, cmd.CompanyRegisteredAddress, cmd.CompanyDateOfIncorporation, cmd.EmployerRef, cmd.AccessToken, cmd.RefreshToken, cmd.SignAgreement)).ReturnsAsync(accountId);

            await _handler.Handle(cmd);

            _accountRepository.Verify(x => x.CreateAccount(user.Id, cmd.CompanyNumber, cmd.CompanyName, cmd.CompanyRegisteredAddress, cmd.CompanyDateOfIncorporation, cmd.EmployerRef, cmd.AccessToken, cmd.RefreshToken, cmd.SignAgreement));
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(c => c.AccountId == accountId)), Times.Once());
        }
        
    }
}
