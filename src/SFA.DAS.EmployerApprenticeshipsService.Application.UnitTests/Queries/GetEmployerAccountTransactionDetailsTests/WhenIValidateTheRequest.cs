using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionDetailsTests
{
    public class WhenIValidateTheRequest
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private GetEmployerAccountTransactionDetailQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _validator = new GetEmployerAccountTransactionDetailQueryValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllFieldsArePopulatedAndTheMemberIsPartOfTheAccount()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery
                {
                    ExternalUserId = "test",
                    HashedAccountId = "test",
                    FromDate = DateTime.Now.AddDays(-10),
                    ToDate = DateTime.Now.AddDays(-2)
                });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheValidtionDictionaryIsPopulatedWhenFieldsArentSupplied()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("FromDate", "From date has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("ToDate", "To date has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheUserDoesNotValidateAgainstTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery
            {
                ExternalUserId = "test",
                HashedAccountId = "test",
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(-2)
            });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
