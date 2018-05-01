using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon.ObjectMothers;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.AddPayeToAccountTests
{
    public class WhenIValidateTheCommand
    {
        private AddPayeToAccountCommandValidator _validator;
        private Mock<IMembershipRepository> _membershiprepository;
        private readonly Guid ExpectedOwnerUserId = Guid.NewGuid();
        private readonly Guid ExpectedNonOwnerUserId = Guid.NewGuid();


        [SetUp]
        public void Arrange()
        {
            _membershiprepository = new Mock<IMembershipRepository>();
            _membershiprepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<Guid>())).ReturnsAsync(null);
            _membershiprepository.Setup(x => x.GetCaller(It.IsAny<string>(), ExpectedOwnerUserId)).ReturnsAsync(new MembershipView {Role = Role.Owner});
            _membershiprepository.Setup(x => x.GetCaller(It.IsAny<string>(), ExpectedNonOwnerUserId)).ReturnsAsync(new MembershipView {Role = Role.Viewer});

            _validator = new AddPayeToAccountCommandValidator(_membershiprepository.Object);
        }

        [Test]
        public async Task ThenTheCommandIsInvalidIfTheFieldsArentPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new AddPayeToAccountCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            _membershiprepository.Verify(x => x.Get(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task ThenTheMembershipRepositoryIsCheckedToMakeSureTheUserIsAnAccountOwner()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(Guid.NewGuid());

            //Act
            await _validator.ValidateAsync(command);

            //Assert
            _membershiprepository.Verify(x=>x.GetCaller(command.HashedAccountId, command.ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsInvalidIfTheUserIsNotPartOfTheAccount()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(Guid.NewGuid());

            //Act
            var actual = await _validator.ValidateAsync(command);

            //Assert
            _membershiprepository.Verify(x => x.GetCaller(command.HashedAccountId, command.ExternalUserId), Times.Once);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("member", "Unauthorised: User not connected to account"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheCommandIsUnauthorisedIfTheUserIsNotAnOwner()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(ExpectedNonOwnerUserId);

            //Act
            var actual = await _validator.ValidateAsync(command);

            //Assert
            _membershiprepository.Verify(x => x.GetCaller(command.HashedAccountId, command.ExternalUserId), Times.Once);
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheCommandIsValidIfAllFieldsArePopulatedAndTheUserIsAnOwner()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(ExpectedOwnerUserId);

            //Act
            var actual = await _validator.ValidateAsync(command);

            //Assert
            _membershiprepository.Verify(x => x.GetCaller(command.HashedAccountId, command.ExternalUserId), Times.Once);
            Assert.IsTrue(actual.IsValid());
        }
    }
}
