﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementPdfTests
{
    public class WhenIValidateTheRequest
    {
        private GetEmployerAgreementPdfValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        private readonly Guid _externalUserId = Guid.NewGuid();


        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(new MembershipView {Role = Role.Owner});

            _validator = new GetEmployerAgreementPdfValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAgreementPdfRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedLegalAgreementId", "HashedLegalAgreementId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAgreementPdfRequest {HashedAccountId = "1234RFV",HashedLegalAgreementId = "1231FG",ExternalUserId = _externalUserId });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenIfIAmNotConnectedToTheAccountTheUnauthorizedFlagIsSet()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAgreementPdfRequest { HashedAccountId = "1234RFV", HashedLegalAgreementId = "1231FG", ExternalUserId = _externalUserId });

            //Act
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [TestCase(Role.None)]
        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        public async Task ThenIfIAmNotAnOwnerOfTheAccountThenTheUnauthorizedFlagIsSet(Role role)
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(new MembershipView { Role = role });

            //Act
            var actual = await _validator.ValidateAsync(new GetEmployerAgreementPdfRequest { HashedAccountId = "1234RFV", HashedLegalAgreementId = "1231FG", ExternalUserId = _externalUserId });

            //Act
            Assert.IsTrue(actual.IsUnauthorized);
        }

    }
}
