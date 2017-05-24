using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementsRemove
{
    public class WhenIGetAccountEmployerAgreementsToRemove : QueryBaseTest<GetAccountEmployerAgreementsRemoveQueryHandler,GetAccountEmployerAgreementsRemoveRequest, GetAccountEmployerAgreementsRemoveResponse>
    {
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<IHashingService> _hashingService;
        public override GetAccountEmployerAgreementsRemoveRequest Query { get; set; }
        public override GetAccountEmployerAgreementsRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountEmployerAgreementsRemoveRequest>> RequestValidator { get; set; }

        private const string ExpectedUserId = "456TGFD";
        private const string ExpectedHashedAccountId = "456TGFD";
        private const long ExpectedAccountId = 98172938;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetAccountEmployerAgreementsRemoveRequest {HashedAccountId = ExpectedHashedAccountId, UserId = ExpectedUserId};

            _repository = new Mock<IEmployerAgreementRepository>();
            _repository.Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
                .ReturnsAsync(new List<RemoveEmployerAgreementView>
                {
                    new RemoveEmployerAgreementView {Name = "test company", CanBeRemoved = true }   
                });

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountId)).Returns(ExpectedAccountId);

            RequestHandler = new GetAccountEmployerAgreementsRemoveQueryHandler(RequestValidator.Object, _repository.Object,_hashingService.Object);
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountEmployerAgreementsRemoveRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountEmployerAgreementsRemoveRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetEmployerAgreementsToRemove(ExpectedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsTrue(actual.Agreements.Any());
        }
    }
}
