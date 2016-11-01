using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemovePayeFromAccountTests
{
    public class WhenIRemoveAPayeSchemeFromAnAccount
    {
        private RemovePayeFromAccountCommandHandler _handler;
        private Mock<IValidator<RemovePayeFromAccountCommand>> _validator;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();

            _validator = new Mock<IValidator<RemovePayeFromAccountCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult());

            _hashingService = new Mock<IHashingService>();
            

            _handler = new RemovePayeFromAccountCommandHandler(_validator.Object, _accountRepository.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _handler.Handle(new RemovePayeFromAccountCommand());

            //Assert
            _validator.Verify(x=>x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>()), Times.Once);
        }

        [Test]
        public void ThenAnInvalidOperationExceptionIsThrownIfTheCommandIsNotValidAndTheRespositoryIsNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new RemovePayeFromAccountCommand()));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public void ThenAnUnauthorizedExceptionIsThrownIfTheCommandIsValidAndUnauthorizeddAndTheRespositoryIsNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(new RemovePayeFromAccountCommand()));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledWhenTheCommandIsValid()
        {
            //Arrange
            var accountId = 8487533;
            var hashedId = "12FFF";
            var payeRef = "fkn/123";
            var userId = "abc";
            _hashingService.Setup(x => x.DecodeValue(hashedId)).Returns(accountId);

            //Act
            await _handler.Handle(new RemovePayeFromAccountCommand { HashedId = hashedId, PayeRef = payeRef, UserId = userId});

            //Assert
            _accountRepository.Verify(x=>x.RemovePayeFromAccount(accountId,payeRef), Times.Once);
        }
    }
}
