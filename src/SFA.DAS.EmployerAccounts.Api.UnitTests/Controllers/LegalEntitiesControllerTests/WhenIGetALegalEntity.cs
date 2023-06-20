using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Mappings;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetALegalEntity
    {
        [Test, RecursiveMoqAutoData]
        public async Task ThenAGetLegalEntityQueryShouldBeSentAndLegalEntityReturned(
            string hashedAccountId,
            long legalEntityId,
            bool includeAllAgreements,
            GetLegalEntityResponse mediatorResponse,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] LegalEntitiesController controller)
        {
            var expectedModel = LegalEntityMapping.MapFromAccountLegalEntity(mediatorResponse.LegalEntity, mediatorResponse.LatestAgreement, includeAllAgreements);

            mediator.Setup(m => m.Send(
                    It.Is<GetLegalEntityQuery>(q => q.AccountHashedId == hashedAccountId && q.LegalEntityId == legalEntityId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorResponse);
            
            var result = await controller.GetLegalEntity(hashedAccountId, legalEntityId, includeAllAgreements) as OkObjectResult;

            Assert.IsNotNull(result);
            var model = ((OkObjectResult)result).Value;
            model.Should().BeEquivalentTo(expectedModel);
        }

    }
}
