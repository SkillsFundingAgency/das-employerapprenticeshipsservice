using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPreviousTransactionsCountTests
{
    public class WhenIValidateTheRequest
    {
        private GetPreviousTransactionsCountRequestValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new MembershipView { RoleId = (short)Role.Owner });

            _validator = new GetPreviousTransactionsCountRequestValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheValidationShouldPassIfUsingCorrectValues()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetPreviousTransactionsCountRequest
            {
                AccountId = 1,
                FromDate = DateTime.Now
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenTheValidationShouldFailIfUsingIncorrectValues()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetPreviousTransactionsCountRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetPreviousTransactionsCountRequest.AccountId), "AccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetPreviousTransactionsCountRequest.FromDate), "FromDate has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRequestIsInValidIfTheUserIsNotATeamMemberOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetPreviousTransactionsCountRequest { ExternalUserId = "123ABC", AccountId = 1, FromDate = DateTime.Now});

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
