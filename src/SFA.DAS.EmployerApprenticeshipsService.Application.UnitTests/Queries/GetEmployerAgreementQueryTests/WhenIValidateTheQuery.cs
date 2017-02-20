using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementQueryTests
{
    public class WhenIValidateTheQuery
    {
        private GetEmployerAgreementQueryValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;
        private GetEmployerAgreementRequest _query;

        [SetUp]
        public void Arrange()
        {
            _query = new GetEmployerAgreementRequest
            {
                ExternalUserId = "ASDABASD",
                HashedAccountId = "ASDANSDLKN123",
                HashedAgreementId = "123EDADS"
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {RoleId = (short)Role.Owner});

            _validator = new GetEmployerAgreementQueryValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenIfTheFieldsAreEmptyThenValidationFails()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAgreementRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
            _membershipRepository.Verify(x=>x.GetCaller(It.IsAny<string>(),It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ThenIfTheUserIsNotConnectedToTheAccountAnUnauthorizedErrorIsReturned()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        [TestCase(Role.None)]
        public async Task ThenIfTheUserIsNotAnOwnerOnTheAccountAnUnauthorizedErrorIsReturned(Role role)
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {RoleId = (short)role});

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }
        [Test]
        public async Task ThenIfAllFieldsArePopulatedAndTheMemberIsPartOfTheAccountThenTheRequestIsValid()
        {
            
            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}
