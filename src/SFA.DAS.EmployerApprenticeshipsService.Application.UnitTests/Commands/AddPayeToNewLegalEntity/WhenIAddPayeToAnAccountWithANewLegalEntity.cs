using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.AddPayeToNewLegalEntity
{
    public class WhenIAddPayeToAnAccountWithANewLegalEntity
    {
        private AddPayeToNewLegalEnttiyCommandHandler _addPayeToNewLegalEnttiyCommandHandler;
        private Mock<IValidator<AddPayeToNewLegalEntityCommand>> _validator;
        private Mock<IAccountRepository> _accountRepository;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();
            
            _validator = new Mock<IValidator<AddPayeToNewLegalEntityCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<AddPayeToNewLegalEntityCommand>())).Returns(new ValidationResult());

            _addPayeToNewLegalEnttiyCommandHandler = new AddPayeToNewLegalEnttiyCommandHandler(_validator.Object, _accountRepository.Object);
        }

        [Test]
        public void ThenTheValidatorIsCalled()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<AddPayeToNewLegalEntityCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> { { "",""} }});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _addPayeToNewLegalEnttiyCommandHandler.Handle(new AddPayeToNewLegalEntityCommand()));

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<AddPayeToNewLegalEntityCommand>()), Times.Once);
            _accountRepository.Verify(x => x.AddPayeToAccountForNewLegalEntity(It.IsAny<Paye>(), It.IsAny<LegalEntity>()), Times.Never);
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
                   c.AccountId.Equals(command.AccountId)
                );
        }
    }
}
