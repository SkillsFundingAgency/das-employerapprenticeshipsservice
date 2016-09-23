using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetEmployerAccountTests
{
    public class WhenIValidateTheGetAccountByIdRequest
    {
        private GetEmployerAccountValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        private const long ExpectedAccountId = 4567;
        private const string ExpectedUserId = "asdf4660";

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, ExpectedUserId)).ReturnsAsync(new MembershipView());

            _validator = new GetEmployerAccountValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheResultIsValidWhenAllFieldsArePopulatedAndTheUserIsPartOfTheAccount()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountQuery {AccountId = ExpectedAccountId,ExternalUserId = ExpectedUserId});

            //Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheUserIsNotPartOfTheAccount()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountQuery());

            //Assert
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheDictionaryIsPopulatedWithValidationErrors()
        {
            //Act
            var result= await _validator.ValidateAsync(new GetEmployerAccountQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("ExternalUserId", "UserId has not been supplied"),result.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("AccountId", "AccountId has not been supplied"),result.ValidationDictionary );
            _membershipRepository.Verify(x=>x.GetCaller(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
    }
}
