﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountPAYESchemesForAuthorisedUser
{
    class WhenIGetAccountPayeSchemes : QueryBaseTest<GetAccountPayeSchemesForAuthorisedUserQueryHandler, GetAccountPayeSchemesForAuthorisedUserQuery, GetAccountPayeSchemesResponse>
    {
        private const long AccountId = 2;
        private static readonly DateTime UpdateDate = DateTime.Now;

        private PayeView _payeView;
        private Mock<IPayeSchemesService> _payeSchemesService;
        private string _hashedAccountId;

        public override GetAccountPayeSchemesForAuthorisedUserQuery Query { get; set; }
        public override GetAccountPayeSchemesForAuthorisedUserQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountPayeSchemesForAuthorisedUserQuery>> RequestValidator { get; set; }
       

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _payeView = new PayeView
            {
                AccountId = AccountId,
                Ref = "123/ABC"
            };

            new DasEnglishFraction
            {
                EmpRef = _payeView.Ref,
                DateCalculated = UpdateDate,
                Amount = 0.5m
            };

            _hashedAccountId = "123ABC";

            Query = new GetAccountPayeSchemesForAuthorisedUserQuery
            {
                HashedAccountId = _hashedAccountId,
                ExternalUserId = "1234"
            };

            _payeSchemesService = new Mock<IPayeSchemesService>();

            _payeSchemesService
                .Setup(
                    m => m.GetPayeSchemsWithEnglishFractionForHashedAccountId(_hashedAccountId)
                )
                .ReturnsAsync(new List<PayeView> { _payeView });

            RequestHandler = new GetAccountPayeSchemesForAuthorisedUserQueryHandler(
                _payeSchemesService.Object,
                RequestValidator.Object
            );
                
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            _payeSchemesService.Verify(x => x.GetPayeSchemsWithEnglishFractionForHashedAccountId(_hashedAccountId), Times.Once);
        }

        [Test]
        public void ThenAnUnauthorizedAccessExceptionIsThrownIfTheValidationReturnsNotAuthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountPayeSchemesForAuthorisedUserQuery>()))
                .ReturnsAsync(new ValidationResult
                {
                    IsUnauthorized = true,
                    ValidationDictionary = new Dictionary<string, string>()
                });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(Query));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(1, result.PayeSchemes.Count);
            Assert.AreEqual(_payeView, result.PayeSchemes.First());
        }
    }
}
