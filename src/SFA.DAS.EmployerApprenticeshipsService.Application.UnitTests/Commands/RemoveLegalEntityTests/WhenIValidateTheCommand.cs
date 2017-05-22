using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RemoveLegalEntity;
using SFA.DAS.EAS.Domain.Data.Repositories;
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

        private const string ExpectedHashedAccountId = "12313";
        private const string ExpectedHashedLegalEntityId = "GFDSF567";
        private const string ExpectedUserId = "AFGF456";
        private const string ExpectedNonOwnerUserId = "AGVFF456";

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedAccountId, ExpectedUserId)).ReturnsAsync(new MembershipView {RoleId = (short) Role.Owner});
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedAccountId, ExpectedNonOwnerUserId)).ReturnsAsync(new MembershipView { RoleId = (short)Role.Transactor });

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(
                x => x.GetLatestAccountLegalEntityAgreement(ExpectedHashedAccountId, ExpectedHashedLegalEntityId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Pending});

            _removeLegalEntityCommandValidator = new RemoveLegalEntityCommandValidator(_membershipRepository.Object, _employerAgreementRepository.Object);
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
            Assert.Contains(new KeyValuePair<string,string>("HashedLegalEntityId", "HashedLegalEntityId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x=>x.GetCaller(It.IsAny<string>(),It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreConnectedToTheAccount()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand {HashedAccountId = ExpectedHashedAccountId,HashedLegalEntityId = ExpectedHashedLegalEntityId,UserId = "TGB678"});

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreAnOwnerOnTheAccount()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, HashedLegalEntityId = ExpectedHashedLegalEntityId, UserId = ExpectedNonOwnerUserId });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheAgreementIsCheckedToSeeIfItHasBeenSigned()
        {
            //Arrange
            _employerAgreementRepository.Setup(
                x => x.GetLatestAccountLegalEntityAgreement(ExpectedHashedAccountId, ExpectedHashedLegalEntityId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Signed });

            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, HashedLegalEntityId = ExpectedHashedLegalEntityId, UserId = ExpectedUserId });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedLegalEntityId", "Agreement has already been signed"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfTheFieldsArePopulatedAndTheUserIsAnAccountOwner()
        {
            //Act
            var actual = await _removeLegalEntityCommandValidator.ValidateAsync(new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, UserId= ExpectedUserId, HashedLegalEntityId = ExpectedHashedLegalEntityId });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}