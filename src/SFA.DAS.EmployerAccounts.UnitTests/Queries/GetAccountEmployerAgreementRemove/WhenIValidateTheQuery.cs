using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountEmployerAgreementRemove
{
    public class WhenIValidateTheQuery
    {
        private GetAccountEmployerAgreementRemoveValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller("ABC123", "XYZ987")).ReturnsAsync(new MembershipView { Role = Role.Owner });

            _validator = new GetAccountEmployerAgreementRemoveValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheDictionaryIsPopulatedIfNoValuesAreSupplied()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementRemoveRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAgreementId", "HashedAgreementId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToMakeSureThatTheyAreConnectedToTheAccount()
        {
            //Act
            _membershipRepository.Setup(x => x.GetCaller("ABC123", "XYZ987")).ReturnsAsync(() => null);
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementRemoveRequest { HashedAccountId = "ABC123", UserId = "XYZ987", HashedAgreementId = "546TGF"});

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToMakeSureThatTheyAreAnOwnerOnTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller("ABC123", "XYZ987")).ReturnsAsync(new MembershipView { Role = Role.Viewer });

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementRemoveRequest { HashedAccountId = "ABC123", UserId = "XYZ987", HashedAgreementId = "546TGF" });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenIfAllFieldsArePopulatedAndTheUserIsAnOWnerOfTheAccountTrueIsReturned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementRemoveRequest { HashedAccountId = "ABC123", UserId = "XYZ987", HashedAgreementId = "546TGF" });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }
    }
}
