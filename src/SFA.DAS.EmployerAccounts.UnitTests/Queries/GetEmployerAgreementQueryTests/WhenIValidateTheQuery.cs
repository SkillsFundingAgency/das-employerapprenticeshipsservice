using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementQueryTests
{
    public class WhenIValidateTheQuery
    {
        private GetEmployerAgreementQueryValidator _validator;
        private GetEmployerAgreementRequest _query;
        
        [SetUp]
        public void Arrange()
        {
            _query = new GetEmployerAgreementRequest
            {
                ExternalUserId = "ASDABASD",
                HashedAccountId = "ASDANSDLKN123",
                HashedAgreementId = "123EDADS"
            };

            _validator = new GetEmployerAgreementQueryValidator();
        }

        [Test]
        public async Task ThenIfTheFieldsAreEmptyThenValidationFails()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAgreementRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenIfAllFieldsArePopulatedThenTheRequestIsValid()
        {
            
            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}
