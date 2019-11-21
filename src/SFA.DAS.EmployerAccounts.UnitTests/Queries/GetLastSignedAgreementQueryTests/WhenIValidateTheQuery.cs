using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetLastSignedAgreementQueryTests
{
    public class WhenIValidateTheQuery
    {
        private GetLastSignedAgreementQueryValidator _validator;
        private GetLastSignedAgreementRequest _query;
        
        [SetUp]
        public void Arrange()
        {
            _query = new GetLastSignedAgreementRequest
            {
            };

            _validator = new GetLastSignedAgreementQueryValidator();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public async Task ThenIfTheFieldsAreEmptyThenValidationFails(long accountLegalEntityId)
        {
            //Arrange
            _query.AccountLegalEntityId = accountLegalEntityId;

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenIfAllFieldsArePopulatedThenTheRequestIsValid()
        {
            //Arrange
            _query.AccountLegalEntityId = 12342345;


            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}
