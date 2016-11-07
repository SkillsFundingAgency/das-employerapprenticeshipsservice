using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionDetailsTests
{
    public class WhenIGetEmployerTransactionDetails : QueryBaseTest<GetEmployerAccountTransactionDetailHandler, GetEmployerAccountTransactionDetailQuery, GetEmployerAccountTransactionDetailResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        public override GetEmployerAccountTransactionDetailQuery Query { get; set; }
        public override GetEmployerAccountTransactionDetailHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountTransactionDetailQuery>> RequestValidator { get; set; }
        private const long ExpectedId = 123123123;
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetTransactionDetailById(It.IsAny<long>())).ReturnsAsync(new List<TransactionLineDetail> {new TransactionLineDetail()});

            Query = new GetEmployerAccountTransactionDetailQuery {Id = ExpectedId};

            RequestHandler = new GetEmployerAccountTransactionDetailHandler(RequestValidator.Object, _dasLevyService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasLevyService.Verify(x=>x.GetTransactionDetailById(ExpectedId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var expectedId = 12344123;

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.TransactionDetail);
        }

        [Test]
        public void ThenAnUnauhtorizedExceptionIsThrownIfTheValidationResultReturnsUnauthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountTransactionDetailQuery>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetEmployerAccountTransactionDetailQuery()));


        }
    }
}
