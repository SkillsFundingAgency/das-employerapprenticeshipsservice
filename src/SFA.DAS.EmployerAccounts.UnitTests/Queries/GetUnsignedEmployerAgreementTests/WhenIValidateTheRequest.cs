using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUnsignedEmployerAgreementTests
{
    public class WhenIValidateTheRequest
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private GetUnsignedEmployerAgreementValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _validator = new GetUnsignedEmployerAgreementValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetUnsignedEmployerAgreementRequest { HashedAccountId = "ABC123", ExternalUserId = Guid.NewGuid().ToString() });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenShouldReturnInvalidIfNoAccountIdIsProvided()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetUnsignedEmployerAgreementRequest { ExternalUserId = Guid.NewGuid().ToString() });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenShouldReturnInvalidIfNoUserIdIsProvided()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetUnsignedEmployerAgreementRequest { HashedAccountId = "ABC123" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenShouldReturnAuthorisedIfTheUserIsAssociatedToTheAccount()
        {
            var request = new GetUnsignedEmployerAgreementRequest { HashedAccountId = "ABC123", ExternalUserId = Guid.NewGuid().ToString() };

            _membershipRepository.Setup(x => x.GetCaller(request.HashedAccountId, request.ExternalUserId)).ReturnsAsync(new MembershipView());

            //Act
            var result = await _validator.ValidateAsync(request);

            //Assert
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenShouldReturnUnauthorisedIfTheUserIsAssociatedToTheAccount()
        {
            var request = new GetUnsignedEmployerAgreementRequest { HashedAccountId = "ABC123", ExternalUserId = Guid.NewGuid().ToString() };

            _membershipRepository.Setup(x => x.GetCaller(request.HashedAccountId, request.ExternalUserId)).ReturnsAsync((MembershipView)null);

            //Act
            var result = await _validator.ValidateAsync(request);

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }
    }
}
