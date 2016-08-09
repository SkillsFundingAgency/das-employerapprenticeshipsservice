using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetAccountLegalEntities
{
    public class WhenIGetAccountLegalEntities : QueryBaseTest<GetAccountLegalEntitiesQueryHandler, GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        public override GetAccountLegalEntitiesRequest Query { get; set; }
        public override GetAccountLegalEntitiesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntitiesRequest>> RequestValidator { get; set; }

        private const long ExpectedAccountId = 123;
        private readonly string _expectedUserId = Guid.NewGuid().ToString();

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            RequestHandler = new GetAccountLegalEntitiesQueryHandler(RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //TODO
            //Act
            await RequestHandler.Handle(new GetAccountLegalEntitiesRequest {Id = ExpectedAccountId, UserId = _expectedUserId});

            
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //TODO
            //Act
            await RequestHandler.Handle(new GetAccountLegalEntitiesRequest { Id = ExpectedAccountId, UserId = _expectedUserId });
        }
    }
}
