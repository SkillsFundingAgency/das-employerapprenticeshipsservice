using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SignEmployerAgreementTests
{
    [TestFixture]
    public class WhenISignAnEmployerAgreement
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEmployerAgreementRepository> _agreementRepository;
        private SignEmployerAgreementCommandHandler _handler;
        private SignEmployerAgreementCommand _command;
        private MembershipView _owner;
        private Mock<IHashingService> _hashingService;

        private const long AgreementId = 123433;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _agreementRepository = new Mock<IEmployerAgreementRepository>();
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(AgreementId);
            _handler = new SignEmployerAgreementCommandHandler(_membershipRepository.Object, _agreementRepository.Object, _hashingService.Object);

            _command = new SignEmployerAgreementCommand
            {
                HashedAccountId = "1",
                HashedAgreementId = "2",
                ExternalUserId = Guid.NewGuid().ToString(),
                SignedDate = DateTime.Now
            };

            _owner = new MembershipView
            {
                UserId = 1,
                RoleId = (short) Role.Owner,
                FirstName = "Fred",
                LastName = "Bloggs"
            };

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(_owner);
        }

        [Test]
        public void ThenAnInvalidCommandThrowsExcption()
        {
            _command = new SignEmployerAgreementCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(4));
        }

        [Test]
        public void ThenNonMemberOfAccountThrowsExcption()
        {
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenNonOwnerOfAccountThrowsExcption()
        {
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(new MembershipView
                {
                    RoleId = (short)Role.Viewer
                });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenAgreementNotFoundThrowsExcption()
        {
            _agreementRepository.Setup(x => x.GetEmployerAgreement(AgreementId))
                .ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenAgreementIsAlreadySignedThrowsExcption()
        {
            _agreementRepository.Setup(x => x.GetEmployerAgreement(AgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    Status = EmployerAgreementStatus.Signed
                });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenAgreementHasExpiredThrowsExcption()
        {
            _agreementRepository.Setup(x => x.GetEmployerAgreement(AgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    Status = EmployerAgreementStatus.Pending,
                    ExpiredDate = DateTimeProvider.Current.UtcNow.AddDays(-1)
                });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenAgreementIsSigned()
        {
            _agreementRepository.Setup(x => x.GetEmployerAgreement(AgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    Status = EmployerAgreementStatus.Pending
                });

            await _handler.Handle(_command);

            _agreementRepository.Verify(x => x.SignAgreement(AgreementId, _owner.UserId, $"{_owner.FirstName} {_owner.LastName}", _command.SignedDate), Times.Once);
        }
    }
}