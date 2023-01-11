using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Api.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.Testing.AutoFixture;
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
                    new List<AccountLegalEntity>
                    {
                        new AccountLegalEntity
                        {
                            Id = 1,
                            LegalEntityId = 5
                        },
                        new AccountLegalEntity
                        {
                            Id = 4,
                            LegalEntityId = 9
                        }
                    }
            };
                
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId))).ReturnsAsync(_response);

            SetupUrlHelperForAccountLegalEntityOne();
            SetupUrlHelperForAccountLegalEntityTwo();

            var response = await Controller.GetLegalEntities(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<EmployerAccounts.Api.Types.ResourceList>>(response);
            var model = response as OkNegotiatedContentResult<EmployerAccounts.Api.Types.ResourceList>;

            model?.Content.Should().NotBeNull();

            foreach (var legalEntity in _response.LegalEntities)
            {
                var matchedEntity = model.Content.Single(x => x.Id == legalEntity.LegalEntityId.ToString());
                matchedEntity.Href.Should().Be($"/api/accounts/{_hashedAccountId}/legalentities/{legalEntity.LegalEntityId}");
            }
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_If_Set_To_Include_Details_Then_AccountLegalEntity_List_Returned(
            List<AccountLegalEntity> legalEntities)
        {
            var expectedModel = legalEntities.Select(c=>LegalEntityMapping.MapFromAccountLegalEntity(c, null, false)).ToList();
            _hashedAccountId = "ABC123";
            _response = new GetAccountLegalEntitiesByHashedAccountIdResponse
            {
                LegalEntities = legalEntities
            };
                
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId))).ReturnsAsync(_response);

            SetupUrlHelperForAccountLegalEntityOne();
            SetupUrlHelperForAccountLegalEntityTwo();

            var response = await Controller.GetLegalEntities(_hashedAccountId, true);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<Types.LegalEntity>>>(response);
            var model = response as OkNegotiatedContentResult<List<Types.LegalEntity>>;
            model?.Content.Should().NotBeNull();
            model?.Content.ShouldBeEquivalentTo(expectedModel);
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
                        LegalEntities = new List<AccountLegalEntity>(0)
                    });

            var response = await Controller.GetLegalEntities(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        private void SetupUrlHelperForAccountLegalEntityOne()
        {
            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(
                    x => x.Route(
                        "GetLegalEntity",
                        It.Is<object>(
                            obj => IsAccountLegalEntityOne(obj))))
                .Returns(
                    $"/api/accounts/{_hashedAccountId}/legalentities/{_response.LegalEntities[0].LegalEntityId}");
        }

        private void SetupUrlHelperForAccountLegalEntityTwo()
        {
            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(
                    x => x.Route(
                        "GetLegalEntity",
                        It.Is<object>(
                            obj => IsAccountLegalEntityTwo(obj))))
                .Returns(
                    $"/api/accounts/{_hashedAccountId}/legalentities/{_response.LegalEntities[1].LegalEntityId}");
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
                o.GetPropertyValue<long>("legalEntityId").Equals(_response.LegalEntities[positionIndex].LegalEntityId);
        }
    }
}
