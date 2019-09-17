using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Util;
using MediatR;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.UnitTests.Queries;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.PayeSchemes
{
    class WhenIGetAccountPayeSchemes
    {
        private const long AccountId = 2;
        private static readonly DateTime UpdateDate = DateTime.Now;

        private PayeView _payeView;
        private DasEnglishFraction _englishFraction;

        private Mock<IPayeRepository> _payeSchemesRepository;
        private Mock<IEnglishFractionRepository> _englishFractionsRepository;
        private Mock<IHashingService> _hashingService;
        private IPayeSchemesService SUT;
        private string _hashedAccountId;

        [SetUp]
        public void Arrange()
        {
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

            _hashedAccountId = "123ABC";

            _payeSchemesRepository = new Mock<IPayeRepository>();
            _englishFractionsRepository = new Mock<IEnglishFractionRepository>();
            _hashingService = new Mock<IHashingService>();

            _payeSchemesRepository.Setup(x => x.GetPayeSchemesByAccountId(It.IsAny<long>())).ReturnsAsync(new List<PayeView>
            {
                _payeView
            });

            _englishFractionsRepository.Setup(x => x.GetCurrentFractionForSchemes(It.IsAny<long>(), It.IsAny<IEnumerable<string>>()))
                                       .ReturnsAsync(new List<DasEnglishFraction> { _englishFraction });

            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>()))
                .Returns(AccountId);

            SUT = new PayeSchemesService(
                _payeSchemesRepository.Object,
                _englishFractionsRepository.Object,
                _hashingService.Object);

        }

        [Test]
        public  async Task ThenIfAccountIdIsValidTheRepositoryIsCalled()
        {
            await SUT.GetPayeSchemsWithEnglishFractionForHashedAccountId(_hashedAccountId);

            _englishFractionsRepository.Verify(x => x.GetCurrentFractionForSchemes(AccountId, It.Is<IEnumerable<string>>(y => y.Single() == _payeView.Ref)), Times.Once);
        }

        [Test]
        public void ThenInvalidRequestExceptionIsThrownForInvalidAccountId()
        {
            _hashingService
                .Setup(
                    m => m.DecodeValue(It.IsAny<string>()))
                .Throws<IndexOutOfRangeException>();

            Assert.ThrowsAsync<InvalidRequestException>(async () => await SUT.GetPayeSchemsWithEnglishFractionForHashedAccountId(_hashedAccountId));
        }

        [Test]
        public  async Task ThenIfAccountIdIsValidPayeSchemesAreReturned()
        {
            //Act
            var result = await SUT.GetPayeSchemsWithEnglishFractionForHashedAccountId(_hashedAccountId);

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(_payeView, result.First());
            Assert.AreEqual(_englishFraction, result.First().EnglishFraction);
        }

        [Test]
        public async Task ThenIfNotSchemesCanBeFoundNoEnglishFractionsAreCollected()
        {
            _payeSchemesRepository.Setup(x => x.GetPayeSchemesByAccountId(It.IsAny<long>()))
                              .ReturnsAsync(new List<PayeView>());

            var result = await SUT.GetPayeSchemsWithEnglishFractionForHashedAccountId(_hashedAccountId);

            //Assert
            Assert.IsEmpty(result);
            _englishFractionsRepository.Verify(x => x.GetCurrentFractionForSchemes(It.IsAny<long>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }
        
    }
}
