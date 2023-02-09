using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetApprenticeship
{
    public class WhenIGetApprenticeship : QueryBaseTest<GetApprenticeshipsHandler, GetApprenticeshipsRequest, EmployerAccounts.Queries.GetApprenticeship.GetApprenticeshipsResponse>
    {
        public override GetApprenticeshipsRequest Query { get; set; }
        public override GetApprenticeshipsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetApprenticeshipsRequest>> RequestValidator { get; set; }

        private Mock<ICommitmentV2Service> _commitmentV2Service;
        private Mock<ILogger<GetApprenticeshipsHandler>> _logger;
        private long _accountId;
        
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountId = 123;
            _logger = new Mock<ILogger<GetApprenticeshipsHandler>>();
            
            _commitmentV2Service = new Mock<ICommitmentV2Service>();

            _commitmentV2Service.Setup(m => m.GetApprenticeships(_accountId)).ReturnsAsync(new List<Apprenticeship> { new Apprenticeship { Id = 3 } });

            RequestHandler = new GetApprenticeshipsHandler(RequestValidator.Object, _logger.Object, _commitmentV2Service.Object);
            
            Query = new GetApprenticeshipsRequest
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
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _commitmentV2Service.Verify(x => x.GetApprenticeships(_accountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert            
            Assert.IsNotNull(response.Apprenticeships);
        }
    }
}
