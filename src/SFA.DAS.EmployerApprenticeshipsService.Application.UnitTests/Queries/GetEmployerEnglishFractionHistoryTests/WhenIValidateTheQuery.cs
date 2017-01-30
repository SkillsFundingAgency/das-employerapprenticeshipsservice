using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerEnglishFractionHistoryTests
{
    public class WhenIValidateTheQuery
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private GetEmployerEnglishFractionValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new MembershipView());

            _validator = new GetEmployerEnglishFractionValidator(_membershipRepository.Object);
            
        }

        [Test]
        public async Task ThenWhenAllFieldsAreSuppliedTheMessageIsValid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerEnglishFractionQuery()
            {
                EmpRef = "123ABC",
                HashedAccountId = "12345",
                UserId = "asdasd"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
            
        }

        [Test]
        public async Task ThenFalseIsReturnedWhenTheFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerEnglishFractionQuery());
            
            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("EmpRef", "EmpRef has not been supplied"),
                actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"),
                actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"),
                actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheyAreNotPartOfTheAccount()
        {
            //Arrange
            var expectedUserId = "123fds";
            var expectedAccountId = "456TGH";
            _membershipRepository.Setup(x => x.GetCaller(expectedAccountId, expectedUserId)).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerEnglishFractionQuery
            {
                EmpRef = "123ABC",
                HashedAccountId = expectedAccountId,
                UserId = expectedUserId
            });

            //Assert
            _membershipRepository.Verify(x => x.GetCaller(expectedAccountId, expectedUserId));
            Assert.IsTrue(actual.IsUnauthorized);
        }

    }
}
