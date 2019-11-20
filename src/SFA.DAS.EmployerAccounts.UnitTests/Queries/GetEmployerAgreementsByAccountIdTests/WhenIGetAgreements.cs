﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementsByAccountIdTests
{
    internal class WhenIGetAgreements : QueryBaseTest<GetEmployerAgreementsByAccountIdRequestHandler, GetEmployerAgreementsByAccountIdRequest, GetEmployerAgreementsByAccountIdResponse>
    {
        private long AccountId => 123456;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private List<EmployerAgreement> _agreements;

        public override GetEmployerAgreementsByAccountIdRequest Query { get; set; }
        public override GetEmployerAgreementsByAccountIdRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAgreementsByAccountIdRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _agreements = new List<EmployerAgreement>()
            {
                new EmployerAgreement
                {
                    Id = 1,
                    SignedByName = "Test Agreement"
                }
            };

            RequestHandler = new GetEmployerAgreementsByAccountIdRequestHandler(
                _employerAgreementRepository.Object,
                RequestValidator.Object);

            Query = new GetEmployerAgreementsByAccountIdRequest
            {
                AccountId = this.AccountId,
            };

            _employerAgreementRepository.Setup(x => x.GetAccountAgreements(It.IsAny<long>()))
                                        .ReturnsAsync(_agreements);

        }
      
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAgreementRepository.Verify(x => x.GetAccountAgreements(AccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_agreements, response.EmployerAgreements);
        }

        [Test]
        public void ThenShouldThrowExceptionIfRequestIsInvalid()
        {
            //Arrange
            var mockRepository = new Mock<IEmployerAgreementRepository>();
            var handler = new GetEmployerAgreementsByAccountIdRequestHandler(mockRepository.Object, new GetEmployerAgreementsByAccountIdRequestValidator());
            var query = new GetEmployerAgreementsByAccountIdRequest();

            //Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => handler.Handle(query));
        }
    }
}
