using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateAccountCommandTests
{
    public class WhenIValidateTheCommand
    {
        private CreateAccountCommandValidator _createCommandValidator;
        private Mock<IEmployerSchemesRepository> _employerSchemesRepository;
        private CreateAccountCommand _createAccountCommand;

        [SetUp]
        public void Arrange()
        {
            _createAccountCommand = new CreateAccountCommand
            {
                OrganisationType = OrganisationType.CompaniesHouse,
                ExternalUserId = "123ADF",
                OrganisationReferenceNumber = "ABV123",
                OrganisationName = "Test Company",
                PayeReference = "980/EEE",
                OrganisationStatus = "active"
            };

            _employerSchemesRepository = new Mock<IEmployerSchemesRepository>();
            _employerSchemesRepository.Setup(x => x.GetSchemeByRef(_createAccountCommand.PayeReference)).ReturnsAsync(() => null);
            _createCommandValidator = new CreateAccountCommandValidator(_employerSchemesRepository.Object);

        }

        [Test]
        public async Task ThenAllFieldsAreValidatedToSeeIfTheyHaveBeenPopulated()
        {
            //Act
            var actual = await _createCommandValidator.ValidateAsync(_createAccountCommand);

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
            await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            _employerSchemesRepository.Verify(x=>x.GetSchemeByRef(_createAccountCommand.PayeReference), Times.Once);
        }

        [Test]
        public async Task ThenFalseIsReturnedIftheSchemeIsAlreadtInUse()
        {
            //Arrange
            _employerSchemesRepository.Setup(x => x.GetSchemeByRef(_createAccountCommand.PayeReference)).ReturnsAsync(new PayeScheme());

            //Act
            var result = await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenShouldPassValidationIfIsAPublicOrganisationAndHasNoOrganisationReferenceNumber()
        {
            //Arrange
            _createAccountCommand.OrganisationType = OrganisationType.PublicBodies;
            _createAccountCommand.OrganisationReferenceNumber = null;

            //Act
            var actual = await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenShouldPassValidationIfIsAOtherOrganisationAndHasNoOrganisationReferenceNumber()
        {
            //Arrange
            _createAccountCommand.OrganisationType = OrganisationType.Other;
            _createAccountCommand.OrganisationReferenceNumber = null;

            //Act
            var actual = await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenShouldFailValidationIfIsACompanyAndHasNoOrganisationReferenceNumber()
        {
            //Arrange
            _createAccountCommand.OrganisationType = OrganisationType.CompaniesHouse;
            _createAccountCommand.OrganisationReferenceNumber = null;

            //Act
            var actual = await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenShouldFailValidationIfIsACharityAndHasNoOrganisationReferenceNumber()
        {
            //Arrange
            _createAccountCommand.OrganisationType = OrganisationType.Charities;
            _createAccountCommand.OrganisationReferenceNumber = null;

            //Act
            var actual = await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenShouldFailValidationIfIsAPensionsRegulatorOrganisationAndHasNoOrganisationReferenceNumber()
        {
            //Arrange
            _createAccountCommand.OrganisationType = OrganisationType.PensionsRegulator;
            _createAccountCommand.OrganisationReferenceNumber = null;

            //Act
            var actual = await _createCommandValidator.ValidateAsync(_createAccountCommand);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }
    }
}
