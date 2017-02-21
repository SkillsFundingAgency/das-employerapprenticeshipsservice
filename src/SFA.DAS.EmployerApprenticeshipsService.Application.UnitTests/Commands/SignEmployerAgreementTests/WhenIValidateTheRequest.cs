using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SignEmployerAgreementTests
{
    public class WhenIValidateTheRequest
    {
        private SignEmployerAgreementCommandValidator _validator;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _hashingService = new Mock<IHashingService>();

            _validator = new SignEmployerAgreementCommandValidator(_employerAgreementRepository.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenIfAllFieldsArePopulatedAndTheAgreementIsPendingTheRequestIsValid()
        {
            //aRRANGE
            var employerAgreementId = 12345;
            var hashedAgreementId = "123ASD";
            _hashingService.Setup(x => x.DecodeValue(hashedAgreementId)).Returns(employerAgreementId);
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(employerAgreementId)).ReturnsAsync(new EmployerAgreementView { Status = EmployerAgreementStatus.Pending });

            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand
            {
                ExternalUserId = "123asd",
                SignedDate = new DateTime(2016,01,01),
                HashedAgreementId = "123ASD",
                HashedAccountId = "GHT432"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenIfTheFieldsAreNotPopulatedThenFalseIsReturnedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand());
            
            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.AreEqual(4,actual.ValidationDictionary.Count);
        }

        [Test]
        public async Task ThenIfTheAgreementDoesNotExistThenTheRequestIsNotValid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand
            {
                ExternalUserId = "123asd",
                SignedDate = new DateTime(2016, 01, 01),
                HashedAgreementId = "12345",
                HashedAccountId = "GHT432"
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [TestCase(EmployerAgreementStatus.Signed)]
        [TestCase(EmployerAgreementStatus.Expired)]
        [TestCase(EmployerAgreementStatus.Superseded)]
        public async Task ThenIfTheAgreementIsAlreadySignedThenTheRequestIsNotValid(EmployerAgreementStatus employerAgreementStatus)
        {
            //Arrange
            var employerAgreementId = 12345;
            var hashedAgreementId = "123ASD";
            _hashingService.Setup(x => x.DecodeValue(hashedAgreementId)).Returns(employerAgreementId);
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(employerAgreementId)).ReturnsAsync(new EmployerAgreementView {Status = employerAgreementStatus});

            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand
            {
                ExternalUserId = "123asd",
                SignedDate = new DateTime(2016, 01, 01),
                HashedAgreementId = hashedAgreementId,
                HashedAccountId = "GHT432"
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }
    }
}
