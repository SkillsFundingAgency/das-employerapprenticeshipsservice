using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountPAYESchemes
{
    class WhenIGetAccountPayeSchemes : QueryBaseTest<GetAccountPayeSchemesQueryHandler, GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>
    {
        private const long AccountId = 2;
        private static readonly DateTime UpdateDate = DateTime.Now;

        private PayeView _payeView;
        private DasEnglishFraction _englishFraction;

        private Mock<IPayeRepository> _accountRepository;
        private Mock<IEnglishFractionRepository> _englishFractionsRepository;
        private Mock<IHashingService> _hashingService;

        public override GetAccountPayeSchemesQuery Query { get; set; }
        public override GetAccountPayeSchemesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountPayeSchemesQuery>> RequestValidator { get; set; }
       

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _payeView = new PayeView
            {
                AccountId = AccountId,
                Ref = "123/ABC"
            };

            _englishFraction = new DasEnglishFraction
            {
                EmpRef = _payeView.Ref,
                DateCalculated = UpdateDate,
                Amount = 0.5m
            };

            Query = new GetAccountPayeSchemesQuery()
            {
                HashedAccountId = "123ABC",
                ExternalUserId = "1234"
            };

            _accountRepository = new Mock<IPayeRepository>();
            _englishFractionsRepository = new Mock<IEnglishFractionRepository>();
            _hashingService = new Mock<IHashingService>();

            _accountRepository.Setup(x => x.GetPayeSchemesByAccountId(It.IsAny<long>())).ReturnsAsync(new List<PayeView>
            {
                _payeView
            });

            _englishFractionsRepository.Setup(x => x.GetCurrentFractionForSchemes(It.IsAny<long>(), It.IsAny<IEnumerable<string>>()))
                                       .ReturnsAsync(new List<DasEnglishFraction> { _englishFraction });

            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>()))
                .Returns(AccountId);

            RequestHandler = new GetAccountPayeSchemesQueryHandler(
                _accountRepository.Object, 
                _englishFractionsRepository.Object,
                _hashingService.Object, 
                RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _accountRepository.Verify(x => x.GetPayeSchemesByAccountId(AccountId), Times.Once);
            _englishFractionsRepository.Verify(x => x.GetCurrentFractionForSchemes(AccountId, It.Is<IEnumerable<string>>(y => y.Single() == _payeView.Ref)), Times.Once);
        }

        [Test]
        public void ThenAnUnauthorizedAccessExceptionIsThrownIfTheValidationReturnsNotAuthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountPayeSchemesQuery>()))
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
            Assert.AreEqual(_englishFraction, result.PayeSchemes.First().EnglishFraction);
        }

        [Test]
        public async Task ThenIfNotSchemesCanBeFoundNoEnglishFractionsAreCollected()
        {
            //Arrabge
            _accountRepository.Setup(x => x.GetPayeSchemesByAccountId(It.IsAny<long>()))
                              .ReturnsAsync(new List<PayeView>());

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsEmpty(result.PayeSchemes);
            _englishFractionsRepository.Verify(x => x.GetCurrentFractionForSchemes(It.IsAny<long>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }
        
    }
}
