using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RemoveLegalEntity;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemoveLegalEntityTests
{
    public class WhenIValidateTheCommand
    {
        private RemoveLegalEntityCommandValidator _removeLegalEntityCommandValidator;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;

        private const string ExpectedHashedAccountId = "12313";
        private const long ExpectedAccountId = 123123777;
        private const long ExpectedLegalEntityId = 46435;
        private const string ExpectedUserId = "AFGF456";
        private const string ExpectedNonOwnerUserId = "AGVFF456";
        private const long ExpectedAgreementId = 4534590;
        private const string ExpectedHashedAgreementId = "ABDSFS1234";

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedAccountId, ExpectedUserId)).ReturnsAsync(new MembershipView {RoleId = (short) Role.Owner});
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedAccountId, ExpectedNonOwnerUserId)).ReturnsAsync(new MembershipView { RoleId = (short)Role.Transactor });

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountId)).Returns(ExpectedAccountId);
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAgreementId)).Returns(ExpectedAgreementId);

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(
                x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Pending, AccountId = ExpectedAccountId, LegalEntityId = ExpectedLegalEntityId});
            _employerAgreementRepository.Setup(
                x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId, false))
                .ReturnsAsync(new List<LegalEntity>
                {
                    new LegalEntity {Id = 432244},
                    new LegalEntity {Id = ExpectedLegalEntityId}
                });

            _removeLegalEntityCommandValidator = new RemoveLegalEntityCommandValidator(_membershipRepository.Object, _employerAgreementRepository.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCheckedToSeeIfAllFieldsArePopulatedAndFalseIsReturnedAndTheErrorDictionaryPopulatedIfNot()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserId","UserId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedLegalAgreementId", "HashedLegalAgreementId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x=>x.GetCaller(It.IsAny<string>(),It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreConnectedToTheAccount()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand {HashedAccountId = ExpectedHashedAccountId, UserId = "TGB678", HashedLegalAgreementId = "ewr" });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreAnOwnerOnTheAccount()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId,  UserId = ExpectedNonOwnerUserId, HashedLegalAgreementId = "fdgd" });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheAgreementIsCheckedToSeeIfItHasBeenSigned()
        {
            //Arrange
            _employerAgreementRepository.Setup(
                x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Signed });

            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId,  UserId = ExpectedUserId, HashedLegalAgreementId = ExpectedHashedAgreementId });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedLegalAgreementId", "Agreement has already been signed"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfTheFieldsArePopulatedAndTheUserIsAnAccountOwner()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, UserId= ExpectedUserId, HashedLegalAgreementId = ExpectedHashedAgreementId });

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
                .ReturnsAsync(new List<LegalEntity>
                {
                    new LegalEntity {Id = ExpectedLegalEntityId}
                });

            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, UserId = ExpectedUserId, HashedLegalAgreementId = ExpectedHashedAgreementId });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedLegalAgreementId", "There must be at least one legal entity on the account"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenIfTheLegalAgreementDoesntExistOnTheAccountThenAnUnauthorizedAcceessIsSetToTrue()
        {
            //Arrange
            _employerAgreementRepository.Setup(
                x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Pending, AccountId = 12399 });

            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, UserId = ExpectedUserId,  HashedLegalAgreementId = ExpectedHashedAgreementId });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}