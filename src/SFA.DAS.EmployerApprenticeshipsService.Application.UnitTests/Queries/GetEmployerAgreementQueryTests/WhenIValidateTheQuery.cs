using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementQueryTests
{
    public class WhenIValidateTheQuery
    {
        private GetEmployerAgreementQueryValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;
        private GetEmployerAgreementRequest _query;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;
        private const long ExpectedAgreementId = 912790137;

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
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(_query.HashedAgreementId)).Returns(ExpectedAgreementId);

            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {RoleId = (short)Role.Owner});

            _validator = new GetEmployerAgreementQueryValidator(_membershipRepository.Object, _employerAgreementRepository.Object, _hashingService.Object);
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
        public async Task ThenIfTheUserIsNotAnOwnerOnTheAccountAndItIsNotSignedAnUnauthorizedErrorIsReturned(Role role)
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView
                {
                    HashedAccountId = _query.HashedAccountId,
                    Status = EmployerAgreementStatus.Pending
                });
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {RoleId = (short)role});

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        [TestCase(Role.Owner)]
        [TestCase(Role.None)]
        public async Task ThenIfTheAgreementIsSignedThenAnyoneCanViewIt(Role role)
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView
            {
                HashedAccountId = _query.HashedAccountId,
                Status = EmployerAgreementStatus.Signed
            });
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { RoleId = (short)role });

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenIfTheAgreementIsNotConnectedToTheAccountTheRequestIsNotAuthorized()
        {

            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView
            {
                HashedAccountId = "YUH78",
                Status = EmployerAgreementStatus.Signed
            });
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { RoleId = (short)Role.Owner});

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenIfThereIsNoAgreementTheValidationResultIsReturned()
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(null);
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { RoleId = (short)Role.Owner });

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsFalse(result.IsUnauthorized);
            Assert.IsTrue(result.IsValid());
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
