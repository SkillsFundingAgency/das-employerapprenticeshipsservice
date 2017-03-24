using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityById;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetALegalEntity : AccountLegalEntitiesControllerTests
    {
        [Test]
        public async Task ThenTheLegalEntityIsReturned()
        {
            var hashedAccountId = "ABC123";
            var legalEntityId = 123;
            var legalEntityResponse = new GetLegalEntityByIdResponse
            {
                LegalEntity = new LegalEntityView
                    {
                        Id = legalEntityId,
                        Name = "Test",
                        Code = "ABC123",
                        Status = "active",
                        DateOfInception = DateTime.Now.AddYears(-3),
                        Address = "Some address",
                        Source = "Companies House"
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetLegalEntityByIdQuery>(q => q.Id == legalEntityId))).ReturnsAsync(legalEntityResponse);

            var response = await Controller.GetLegalEntity(hashedAccountId, legalEntityId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<LegalEntityViewModel>>(response);
            var model = response as OkNegotiatedContentResult<LegalEntityViewModel>;

            model?.Content.Should().NotBeNull();
            model.Content.ShouldBeEquivalentTo(legalEntityResponse.LegalEntity, options => options.Excluding(x => x.DasAccountId).Excluding(x => x.LegalEntityId));
            model.Content.DasAccountId.Should().Be(hashedAccountId);
            model.Content.LegalEntityId.Should().Be(legalEntityId);
        }

        [Test]
        public async Task AndTheLegalEntityDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var legalEntityId = 123;
            var legalEntityResponse = new GetLegalEntityByIdResponse { LegalEntity = null };

            Mediator.Setup(x => x.SendAsync(It.Is<GetLegalEntityByIdQuery>(q => q.Id == legalEntityId))).ReturnsAsync(legalEntityResponse);

            var response = await Controller.GetLegalEntity(hashedAccountId, legalEntityId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
