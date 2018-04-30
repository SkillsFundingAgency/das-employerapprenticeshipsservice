using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSignedEmployerAgreementPdfTests
{
    public class WhenIValidateTheRequest
    {
        private GetSignedEmployerAgreementPdfValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        private const string ExpectedHashedAccountId = "123ASQ";
        private readonly Guid ExpectedUserId = Guid.NewGuid();

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(new MembershipView
                {
                    Role = Role.Transactor
                });
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedAccountId, ExpectedUserId))
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
                ExternalUserId = ExpectedUserId,
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
            Assert.Contains(new KeyValuePair<string,string>("ExternalUserId","ExternalUserId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task ThenIfTheUserIsNotAnOwnerThenTheUnAuhtorizedFlagIsSet()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetSignedEmployerAgreementPdfRequest {HashedAccountId = "123", ExternalUserId = Guid.NewGuid(), HashedLegalAgreementId="123RFV"});

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
