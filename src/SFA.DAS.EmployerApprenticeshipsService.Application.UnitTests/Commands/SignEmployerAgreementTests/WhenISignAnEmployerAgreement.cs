using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;

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
        private Mock<IValidator<SignEmployerAgreementCommand>> _validator;

        private const long AgreementId = 123433;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(AgreementId);

            _validator = new Mock<IValidator<SignEmployerAgreementCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<SignEmployerAgreementCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _agreementRepository = new Mock<IEmployerAgreementRepository>();

            _handler = new SignEmployerAgreementCommandHandler(_membershipRepository.Object, _agreementRepository.Object, _hashingService.Object,_validator.Object);

            _command = new SignEmployerAgreementCommand
            {
                HashedAccountId = "1AVCFD",
                HashedAgreementId = "2EQWE34",
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
        public void ThenTheValidatorIsCalledAndAnInvalidRequestExceptionIsThrownIfItIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<SignEmployerAgreementCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public void ThenIfTheUserIsNotConnectedToTheAccountThenAnUnauthorizedExceptionIsThrown()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(null);

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        [TestCase(Role.None)]
        public void ThenIfTheUserIsNotAnOwnerThenAnUnauthorizedExceptionIsThrown(Role role)
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView {RoleId = (short)role});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenIfTheCommandIsValidTheRepositoryIsCalledWithThePassedParameters()
        {
            //Arrange
            var agreementId = 87761263;
            _hashingService.Setup(x => x.DecodeValue(_command.HashedAgreementId)).Returns(agreementId);

            //Act
            await _handler.Handle(_command);

            //Assert
            _agreementRepository.Verify(x=>x.SignAgreement(It.Is<SignEmployerAgreement>(c=>c.SignedDate.Equals(_command.SignedDate) 
                                && c.AgreementId.Equals(agreementId) 
                                && c.SignedDate.Equals(_command.SignedDate)
                                && c.SignedById.Equals(_owner.UserId)
                                && c.SignedByName.Equals($"{_owner.FirstName} {_owner.LastName}")
                                )));
        }
    }
}