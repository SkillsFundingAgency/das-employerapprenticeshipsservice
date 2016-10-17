using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateAccountCommandTests
{
    public class WhenIValidateTheCommand
    {
        private CreateAccountCommandValidator _createCommandValidator;
        private Mock<IEmployerSchemesRepository> _employerSchemesRepository;
        public CreateAccountCommand CreateAccountCommand;

        [SetUp]
        public void Arrange()
        {
            CreateAccountCommand = new CreateAccountCommand
            {
                ExternalUserId = "123ADF",
                CompanyNumber = "ABV123",
                CompanyName = "Test Company",
                EmployerRef = "980/EEE"
            };

            _employerSchemesRepository = new Mock<IEmployerSchemesRepository>();
            _employerSchemesRepository.Setup(x => x.GetSchemeByRef(CreateAccountCommand.EmployerRef)).ReturnsAsync(null);
            _createCommandValidator = new CreateAccountCommandValidator(_employerSchemesRepository.Object);

        }

        [Test]
        public async Task ThenAllFieldsAreValidatedToSeeIfTheyHaveBeenPopulated()
        {
            //Act
            var actual = await _createCommandValidator.ValidateAsync(CreateAccountCommand);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfThefieldsArentPopulated()
        {
            //Act
            var actual = await _createCommandValidator.ValidateAsync(new CreateAccountCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenTheEmployerRefIsCheckedToSeeIfItHasAlreadyBeenRegistered()
        {
            //Act
            await _createCommandValidator.ValidateAsync(CreateAccountCommand);

            //Assert
            _employerSchemesRepository.Verify(x=>x.GetSchemeByRef(CreateAccountCommand.EmployerRef), Times.Once);
        }

        [Test]
        public async Task ThenFalseIsReturnedIftheSchemeIsAlreadtInUse()
        {
            //Arrange
            _employerSchemesRepository.Setup(x => x.GetSchemeByRef(CreateAccountCommand.EmployerRef)).ReturnsAsync(new Scheme());

            //Act
            var result = await _createCommandValidator.ValidateAsync(CreateAccountCommand);

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
