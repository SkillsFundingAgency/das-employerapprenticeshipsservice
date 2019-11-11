using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;
using SFA.DAS.HashingService;

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

        [Test]
        public async Task ThenIfTheFieldsAreEmptyThenValidationFails()
        {
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
