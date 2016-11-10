using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionDetailTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountTransactionDetailValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _validator = new GetAccountTransactionDetailValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheResultIsValidWhenAllFieldsArePopulated()
        {
            //Assign
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .ReturnsAsync(new MembershipView());

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTransactionDetailQuery
            {
                AccountId = 1,
                ExternalUserId = "2",
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(-2)
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenTheResultIsNotValidWhenFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Assign
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .ReturnsAsync(new MembershipView());

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTransactionDetailQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AccountId","Account ID has not been supplied"), actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "External user ID has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("FromDate","From date has not been supplied"), actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("ToDate","To date has not been supplied"), actual.ValidationDictionary );
        }

        [Test]
        public async Task ThenTheResultIsNotValidWhenUserDoesNotHaveAccessToAccount()
        {
            //Assign
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTransactionDetailQuery()
            {
                AccountId = 1,
                ExternalUserId = "2",
                ToDate = DateTime.Now.AddDays(-2),
                FromDate = DateTime.Now.AddDays(-10)
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.IsUnauthorized);
            Assert.Contains(new KeyValuePair<string, string>("Membership", "User is not a member of this Account"), actual.ValidationDictionary);
          
        }
    }
}
