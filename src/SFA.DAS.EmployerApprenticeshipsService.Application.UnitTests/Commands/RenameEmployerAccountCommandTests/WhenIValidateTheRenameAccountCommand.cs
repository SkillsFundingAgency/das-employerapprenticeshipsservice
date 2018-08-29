﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RenameEmployerAccountCommandTests
{
    public class WhenIValidateTheRenameAccountCommand
    {
        private RenameEmployerAccountCommandValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { RoleId = (short)Role.Owner });
            _membershipRepository.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new TeamMember { IsUser = false });

            _validator = new RenameEmployerAccountCommandValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenNewAccountNameCannotBeEmpty()
        {
            //Arrange
            var command = new RenameEmployerAccountCommand
            {
                NewName = String.Empty
            };

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenNewAccountNameIsValidIfNotEmpty()
        {
            //Arrange
            var command = new RenameEmployerAccountCommand
            {
                NewName = "Test Renamed Account"
            };

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}
