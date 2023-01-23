using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.SignEmployerAgreementTests
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
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreementStatus(employerAgreementId)).ReturnsAsync(EmployerAgreementStatus.Pending);

            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand
            {
                HashedAccountId = "GHT432",
                ExternalUserId = "123asd",
                SignedDate = new DateTime(2016, 01, 01),
                HashedAgreementId = "123ASD"
            });


            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenIfTheFieldsAreNotPopulatedThenFalseIsReturnedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand
            {
                SignedDate = new DateTime(2016, 01, 01)
            });
            
            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.AreEqual(3,actual.ValidationDictionary.Count);
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
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreementStatus(employerAgreementId)).ReturnsAsync(employerAgreementStatus);

            //Act
            var actual = await _validator.ValidateAsync(new SignEmployerAgreementCommand
            {
                HashedAccountId = "GHT432",
                ExternalUserId = "123asd",
                SignedDate = new DateTime(2016, 01, 01),
                HashedAgreementId = hashedAgreementId
            });


            //Assert
            Assert.IsFalse(actual.IsValid());
        }
    }
}
