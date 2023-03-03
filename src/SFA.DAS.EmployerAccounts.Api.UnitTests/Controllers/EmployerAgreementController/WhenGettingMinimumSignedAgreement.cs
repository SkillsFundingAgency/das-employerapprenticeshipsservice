using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAgreementController;

public class WhenGettingMinimumSignedAgreement
{
    [Test, MoqAutoData]
    public async Task Then_The_AccountId_Is_Decoded_Handler_Called_And_MinimumSignedAgreementVersion_Returned(
        string hashedAccountId,
        long accountId,
        GetMinimumSignedAgreementVersionResponse response,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IMediator> mediator)
    {
        //Arrange
        encodingService.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        mediator.Setup(x => x.Send(It.Is<GetMinimumSignedAgreementVersionQuery>(c => c.AccountId.Equals(accountId)),
            CancellationToken.None)).ReturnsAsync(response);
        var orchestrator = new AgreementOrchestrator(mediator.Object, Mock.Of<ILogger<AgreementOrchestrator>>(),
            Mock.Of<IMapper>());
        var controller = new Api.Controllers.EmployerAgreementController(orchestrator, encodingService.Object);
        
        //Act
        var actual = await controller.GetMinimumSignedAgreementVersionByHashedId(hashedAccountId) as OkObjectResult;
        var model = actual.Value as MinimumSignedAgreementResponse;

        //Assert
        Assert.AreEqual(response.MinimumSignedAgreementVersion,model.MinimumSignedAgreementVersion);
    }
}