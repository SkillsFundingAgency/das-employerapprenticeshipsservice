using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityById;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetALegalEntity : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountIsReturned()
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
                        CompanyStatus = "active",
                        DateOfIncorporation = DateTime.Now.AddYears(-3),
                        RegisteredAddress = "Some address",
                        Source = "Companies House"
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetLegalEntityByIdQuery>(q => q.Id == legalEntityId))).ReturnsAsync(legalEntityResponse);

            var response = await Controller.GetLegalEntity(hashedAccountId, legalEntityId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<LegalEntityViewModel>>(response);
            var model = response as OkNegotiatedContentResult<LegalEntityViewModel>;

            model?.Content.Should().NotBeNull();
            model.Content.ShouldBeEquivalentTo(legalEntityResponse.LegalEntity);
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
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
