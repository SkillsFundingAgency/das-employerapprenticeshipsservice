using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.HashingService;

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
