using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetCohorts;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetCohorts
{
    public class WhenIGetCohorts : QueryBaseTest<GetCohortsHandler, GetCohortsRequest, EmployerAccounts.Queries.GetCohorts.GetCohortsResponse>
    {
        public override GetCohortsRequest Query { get; set; }
        public override GetCohortsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetCohortsRequest>> RequestValidator { get; set; }

        private Mock<ICommitmentV2Service> _commitmentV2Service;
        private Mock<IEncodingService> _encodingService;
        private Mock<ILog> _logger;
        private long _accountId;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountId = 123;
            _logger = new Mock<ILog>();

            _commitmentV2Service = new Mock<ICommitmentV2Service>();            
            _commitmentV2Service.Setup(m => m.GetCohorts(_accountId)).ReturnsAsync(new SFA.DAS.CommitmentsV2.Api.Types.Responses.GetCohortsResponse
                (new List<CohortSummary>()
                {
                    new CohortSummary()
                    {
                        CohortId = 1,
                        AccountId = _accountId
                    }
                }));
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            RequestHandler = new GetCohortsHandler(RequestValidator.Object, _logger.Object, _commitmentV2Service.Object, _encodingService.Object);

            Query = new GetCohortsRequest
            {
                AccountId = _accountId
            };
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert            
            Assert.IsNotNull(response.CohortsResponse);
            Assert.AreEqual("1_Encoded", response.HashedCohortReference);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _commitmentV2Service.Verify(x => x.GetCohorts(_accountId), Times.Once);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            throw new NotImplementedException();
        }
    }
}
