﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Commands.CreateInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateInvitationTests
{
    public class WhenIValidateCreateInvitation
    {
        private CreateInvitationCommandValidator _validator;
        private CreateInvitationCommand _createInvitationCommand;
        private Mock<IMembershipRepository> _membershipRepository;
        
        [SetUp]
        public void Arrange()
        {
            _createInvitationCommand = new CreateInvitationCommand
            {
                EmailOfPersonBeingInvited = "so'me@email.com",
                ExternalUserId = "123",
                HashedAccountId = "123dfg",
                NameOfPersonBeingInvited = "Test",
                RoleIdOfPersonBeingInvited = Role.Owner
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {RoleId = (short)Role.Owner});
            _membershipRepository.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new TeamMember { IsUser = false });

            _validator = new CreateInvitationCommandValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfAllFieldsArePopulated()
        {
            //Act
            var result = await _validator.ValidateAsync(_createInvitationCommand);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenTheErrorDictionaryIsPopulatedWithErrorsWhenInvalid()
        {
            //Act
            var result = await _validator.ValidateAsync(new CreateInvitationCommand());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("EmailOfPersonBeingInvited", "Enter email address"),result.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "No HashedAccountId supplied"),result.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("NameOfPersonBeingInvited", "Enter name"),result.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("RoleIdOfPersonBeingInvited", "Select team member role"),result.ValidationDictionary );
        }

        [TestCase("notvalid")]
        [TestCase("notvalid.com")]
        [TestCase("notvalid@valid")]
        public async Task ThenTheEmailFormatIsValidatedWhenPopulatedAndReturnsFalseForNonValidEmails(string email)
        {
            //Act
            var result = await _validator.ValidateAsync(new CreateInvitationCommand
            {
                EmailOfPersonBeingInvited = email,
                ExternalUserId = "123",
                HashedAccountId = "123dfg",
                NameOfPersonBeingInvited = "Test",
                RoleIdOfPersonBeingInvited = Role.Owner
            });

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("EmailOfPersonBeingInvited", "Enter a valid email address"), result.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreAssocaitedWithTheAccountAndTheResultIsNotValidIfTheyArent()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            var result = await _validator.ValidateAsync(_createInvitationCommand);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.IsUnauthorized);
            Assert.Contains(new KeyValuePair<string, string>("Membership", "User is not a member of this Account"), result.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreAnOwnerAndFalseIsReturnedIfTheyArent()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {RoleId = (short)Role.Transactor});

            //Act
            var result = await _validator.ValidateAsync(_createInvitationCommand);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.IsUnauthorized);
            Assert.Contains(new KeyValuePair<string, string>("Membership", "User is not an Owner"), result.ValidationDictionary);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfTheEmailIsAlreadyInUse()
        {
            //Arrange
            _membershipRepository.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new TeamMember {IsUser = true});

            //Act
            var result = await _validator.ValidateAsync(_createInvitationCommand);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("EmailOfPersonBeingInvited", $"{_createInvitationCommand.EmailOfPersonBeingInvited} is already invited"), result.ValidationDictionary);
        }
        
    }
}
