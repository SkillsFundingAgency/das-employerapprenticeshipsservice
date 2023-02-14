using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.RemoveLegalEntityTests
{
    public class WhenIValidateTheCommand
    {
        private RemoveLegalEntityCommandValidator _removeLegalEntityCommandValidator;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private const long ExpectedAccountId = 123123777;
        private const long ExpectedLegalEntityId = 46435;
        private const string ExpectedUserId = "AFGF456";
        private const string ExpectedNonOwnerUserId = "AGVFF456";
        private const long ExpectedAgreementId = 4534590;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, ExpectedUserId)).ReturnsAsync(new MembershipView { Role = Role.Owner });
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, ExpectedNonOwnerUserId)).ReturnsAsync(new MembershipView { Role = Role.Transactor });

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(
                x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Pending, AccountId = ExpectedAccountId, LegalEntityId = ExpectedLegalEntityId });
            _employerAgreementRepository.Setup(
                x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId, false))
                .ReturnsAsync(new List<AccountSpecificLegalEntity>
                {
                    new AccountSpecificLegalEntity {Id = 432244},
                    new AccountSpecificLegalEntity {Id = ExpectedLegalEntityId}
                });

            _removeLegalEntityCommandValidator = new RemoveLegalEntityCommandValidator(_membershipRepository.Object, _employerAgreementRepository.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCheckedToSeeIfAllFieldsArePopulatedAndFalseIsReturnedAndTheErrorDictionaryPopulatedIfNot()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("AccountLegalEntityId", "AccountLegalEntityId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreConnectedToTheAccount()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { AccountId = ExpectedAccountId, UserId = "TGB678", AccountLegalEntityId = 198 });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreAnOwnerOnTheAccount()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { AccountId = ExpectedAccountId, UserId = ExpectedNonOwnerUserId, AccountLegalEntityId = 198 });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfTheFieldsArePopulatedAndTheUserIsAnAccountOwner()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { AccountId = ExpectedAccountId, UserId = ExpectedUserId, AccountLegalEntityId = ExpectedLegalEntityId });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenIfThereIsOnlyOneLegalEntityConnectedToTheAccountThenFalseIsReturned()
        {
            //Arrange
            _employerAgreementRepository.Setup(
                x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId, false))
                .ReturnsAsync(new List<AccountSpecificLegalEntity>
                {
                    new AccountSpecificLegalEntity {Id = ExpectedLegalEntityId}
                });

            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { AccountId = ExpectedAccountId, UserId = ExpectedUserId, AccountLegalEntityId = ExpectedLegalEntityId });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountLegalEntityId", "There must be at least one legal entity on the account"), actual.ValidationDictionary);
        }
    }
}