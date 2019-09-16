using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.Validation;
using It = Moq.It;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetLegalEntitiesForAnAccount : LegalEntitiesControllerTests
    {
        private string _hashedAccountId;
        private GetAccountLegalEntitiesByHashedAccountIdResponse _response;

        [Test]
        public async Task ThenTheLegalEntitiesAreReturned()
        {
            _hashedAccountId = "ABC123";
            _response = new GetAccountLegalEntitiesByHashedAccountIdResponse
            {
                LegalEntities =
                    new List<AccountSpecificLegalEntity>
                    {
                        new AccountSpecificLegalEntity
                        {
                            Id = 1
                        },
                        new AccountSpecificLegalEntity
                        {
                            Id = 4
                        }
                    }
            };
                
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId))).ReturnsAsync(_response);

            SetupUrlHelperForAccountLegalEntityOne();
            SetupUrlHelperForAccountLegalEntityTwo();

            var response = await Controller.GetLegalEntities(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ResourceList>>(response);
            var model = response as OkNegotiatedContentResult<ResourceList>;

            model?.Content.Should().NotBeNull();

            foreach (var legalEntity in _response.LegalEntities)
            {
                var matchedEntity = model.Content.Single(x => x.Id == legalEntity.Id.ToString());
                matchedEntity.Href.Should().Be($"/api/accounts/{_hashedAccountId}/legalentities/{legalEntity.Id}");
            }
        }

        [Test]
        public async Task AndTheAccountCannotBeDecodedThenItIsNotReturned()
        {
            Mediator.Setup(
                    x => x.SendAsync(
                        It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId)))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            var response = await Controller.GetLegalEntities(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            Mediator.Setup(
                    x => x.SendAsync(
                        It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId)))
                .ReturnsAsync(
                    new GetAccountLegalEntitiesByHashedAccountIdResponse
                    {
                        LegalEntities = new List<AccountSpecificLegalEntity>(0)
                    });

            var response = await Controller.GetLegalEntities(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        private void SetupUrlHelperForAccountLegalEntityOne()
        {
            UrlHelper.Setup(
                    x => x.Route(
                        "GetLegalEntity",
                        It.Is<object>(
                            obj => IsAccountLegalEntityOne(obj))))
                .Returns(
                    $"/api/accounts/{_hashedAccountId}/legalentities/{_response.LegalEntities[0].Id}");
        }

        private void SetupUrlHelperForAccountLegalEntityTwo()
        {
            UrlHelper.Setup(
                    x => x.Route(
                        "GetLegalEntity",
                        It.Is<object>(
                            obj => IsAccountLegalEntityTwo(obj))))
                .Returns(
                    $"/api/accounts/{_hashedAccountId}/legalentities/{_response.LegalEntities[1].Id}");
        }

        private bool IsAccountLegalEntityTwo(object o)
        {
            return IsAccountLegalEntityInPosition(o, 1);
        }
        private bool IsAccountLegalEntityOne(object o)
        {
            return IsAccountLegalEntityInPosition(o, 0);
        }
        private bool IsAccountLegalEntityInPosition(object o, int positionIndex)
        {
            return
                o.GetPropertyValue<string>("hashedAccountId").Equals(_hashedAccountId)
                &&
                o.GetPropertyValue<long>("legalEntityId").Equals(_response.LegalEntities[positionIndex].Id);
        }
    }
}
