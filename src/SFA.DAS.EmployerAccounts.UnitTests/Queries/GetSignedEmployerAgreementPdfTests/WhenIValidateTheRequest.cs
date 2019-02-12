using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSignedEmployerAgreementPdfTests
{
    public class WhenIValidateTheRequest
    {
        private GetSignedEmployerAgreementPdfValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        private const string ExpectedHashedAccountId = "123ASQ";
        private const string ExpectedUserId = "123ASQ";

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new MembershipView
                {
                    Role = Role.Transactor
                });
            _membershipRepository.Setup(x => x.GetCaller(ExpectedUserId, ExpectedHashedAccountId))
                .ReturnsAsync(new MembershipView
                {
                    Role = Role.Owner
                } );

            _validator = new GetSignedEmployerAgreementPdfValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheRequestIsValidWhenAllFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetSignedEmployerAgreementPdfRequest {
                HashedAccountId = ExpectedHashedAccountId,
                UserId = ExpectedUserId,
                HashedLegalAgreementId = "1234RFV"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
            _membershipRepository.Verify(x => x.GetCaller(ExpectedHashedAccountId, ExpectedUserId), Times.Once);
        }

        [Test]
        public async Task ThenTheRequestIsNotValidAndTheErrorDictionaryIsPopulatedWhenTheFieldsArentSupplied()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetSignedEmployerAgreementPdfRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedLegalAgreementId", "HashedLegalAgreementId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserId","UserId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenIfTheUserIsNotAnOwnerThenTheUnAuhtorizedFlagIsSet()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetSignedEmployerAgreementPdfRequest {HashedAccountId = "123", UserId = "123", HashedLegalAgreementId="123RFV"});

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
