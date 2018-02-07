using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Services;

namespace SFA.DAS.EAS.Application.UnitTests.Services.MembershipServiceTests
{
    public class WhenIValidateAccountMembership
    {
        private const string AccountHashedId = "ABC123";
        private static readonly Guid UserExternalId = Guid.NewGuid();

        private IMembershipService _membershipService;
        private Mock<EmployerAccountDbContext> _db;

        [SetUp]
        public void SetUp()
        {
            _db = new Mock<EmployerAccountDbContext>();

            _db.Setup(d => d.SqlQuery<bool>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>())).Returns(new List<bool> { true });

            _membershipService = new MembershipService(_db.Object);
        }

        [Test]
        public void ThenShouldCallValidateAccountMembershipSproc()
        {
            _membershipService.ValidateAccountMembership(AccountHashedId, UserExternalId);

            _db.Verify(d => d.SqlQuery<bool>("[employer_account].[ValidateAccountMembership] @accountHashedId = {0}, @userExternalId = {1}", AccountHashedId, UserExternalId));
        }

        [Test]
        public void ThenShouldNotThrowUnauthorizedAccessExceptionIfAccountMembershipIsValid()
        {
            _membershipService.ValidateAccountMembership(AccountHashedId, UserExternalId);
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfAccountMembershipIsInvalid()
        {
            _db.Setup(d => d.SqlQuery<bool>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>())).Returns(new List<bool> { false });
            
            Assert.Throws<UnauthorizedAccessException>(() => _membershipService.ValidateAccountMembership(AccountHashedId, UserExternalId));
        }
    }
}