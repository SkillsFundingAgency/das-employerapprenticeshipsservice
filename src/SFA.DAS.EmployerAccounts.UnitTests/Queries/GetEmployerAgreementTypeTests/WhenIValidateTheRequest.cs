using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementType;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementTypeTests
{
    public class WhenIValidateTheRequest
    {
        private GetEmployerAgreementTypeValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetEmployerAgreementTypeValidator();
        }

        [Test]
        public async Task ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAgreementTypeRequest() { HashedAgreementId = "ABC123" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenShouldReturnInvalidIfNoAgreementIdIsProvided()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAgreementTypeRequest() { HashedAgreementId = "" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
