using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementTests
{
    public class WhenIValidateTheRequest
    {
        private const string ExpectedExternalUserId = "456";
        private const string ExpectedHashedId = "456487";
        private GetAccountEmployerAgreementsValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedId, ExpectedExternalUserId)).ReturnsAsync(new MembershipView());

            _validator = new GetAccountEmployerAgreementsValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllTheFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementsRequest { ExternalUserId = ExpectedExternalUserId, HashedAccountId = ExpectedHashedId });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task TheFalseIsReturnedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementsRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenIfTheRequestIsValidTheUserIsCheckedToSeeIfTheyAreConnectedToTheAccount()
        {
            //Act
            await _validator.ValidateAsync(new GetAccountEmployerAgreementsRequest { ExternalUserId = ExpectedExternalUserId, HashedAccountId = ExpectedHashedId });

            //Arrange
            _membershipRepository.Verify(x=>x.GetCaller(ExpectedHashedId,ExpectedExternalUserId));
        }

        [Test]
        public async Task ThenUnauthorizedIsSetToTrueIfTheUserIsNotPartOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedId, ExpectedExternalUserId)).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountEmployerAgreementsRequest { ExternalUserId = ExpectedExternalUserId, HashedAccountId = ExpectedHashedId });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
