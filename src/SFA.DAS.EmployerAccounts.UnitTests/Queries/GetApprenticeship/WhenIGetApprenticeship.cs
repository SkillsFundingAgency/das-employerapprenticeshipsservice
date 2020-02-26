using Moq;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;
using SFA.DAS.Validation;
using System;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetApprenticeship
{
    public class WhenIGetApprenticeship : QueryBaseTest<GetApprenticeshipHandler, GetApprenticeshipRequest, EmployerAccounts.Queries.GetApprenticeship.GetApprenticeshipResponse>
    {
        public override GetApprenticeshipRequest Query { get; set; }
        public override GetApprenticeshipHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetApprenticeshipRequest>> RequestValidator { get; set; }

        private Mock<ICommitmentV2Service> _commitmentV2Service;
        private Mock<ILog> _logger;
        private long _accountId;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountId = 123;
            _logger = new Mock<ILog>();
            
            _commitmentV2Service = new Mock<ICommitmentV2Service>();
            _commitmentV2Service.Setup(m => m.GetApprenticeship(_accountId)).ReturnsAsync(new GetApprenticeshipsResponse());
            
            RequestHandler = new GetApprenticeshipHandler(RequestValidator.Object, _logger.Object, _commitmentV2Service.Object);
            
            Query = new GetApprenticeshipRequest
            {
                AccountId = _accountId
            };
        }


        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _commitmentV2Service.Verify(x => x.GetApprenticeship(_accountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert            
            Assert.IsNotNull(response.ApprenticeshipDetailsResponse);
        }
    }
}
