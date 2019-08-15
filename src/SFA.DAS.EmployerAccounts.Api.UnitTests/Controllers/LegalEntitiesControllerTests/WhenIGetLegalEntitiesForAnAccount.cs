using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using It = Moq.It;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetLegalEntitiesForAnAccount : LegalEntitiesControllerTests
    {
        private string _hashedAccountId;
        private GetEmployerAccountResponse _accountResponse;

        [Test]
        public async Task ThenTheLegalEntitiesAreReturned()
        {
            _hashedAccountId = "ABC123";
            _accountResponse = new GetEmployerAccountResponse
            {
                Account =
                    new Account
                    {
                        HashedId = _hashedAccountId,
                        AccountLegalEntities = new List<AccountLegalEntity>
                        {
                            new AccountLegalEntity
                            {
                                LegalEntityId = 1
                            },
                            new AccountLegalEntity
                            {
                                LegalEntityId = 4
                            }
                        }
                    }
            };
                
            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == _hashedAccountId))).ReturnsAsync(_accountResponse);

            SetupUrlHelperForAccountLegalEntityOne();
            SetupUrlHelperForAccountLegalEntityTwo();

            var response = await Controller.GetLegalEntities(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ResourceList>>(response);
            var model = response as OkNegotiatedContentResult<ResourceList>;

            model?.Content.Should().NotBeNull();

            foreach (var legalEntity in _accountResponse.Account.AccountLegalEntities)
            {
                var matchedEntity = model.Content.Single(x => x.Id == legalEntity.LegalEntityId.ToString());
                matchedEntity.Href.Should().Be($"/api/accounts/{_hashedAccountId}/legalentities/{legalEntity.LegalEntityId}");
            }
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            Mediator.Setup(
                    x => x.SendAsync(
                        It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == _hashedAccountId)))
                .ReturnsAsync(
                    new GetEmployerAccountResponse
                    {
                        Account = null
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
                    $"/api/accounts/{_hashedAccountId}/legalentities/{_accountResponse.Account.AccountLegalEntities.ToList()[0].LegalEntityId}");
        }

        private void SetupUrlHelperForAccountLegalEntityTwo()
        {
            UrlHelper.Setup(
                    x => x.Route(
                        "GetLegalEntity",
                        It.Is<object>(
                            obj => IsAccountLegalEntityTwo(obj))))
                .Returns(
                    $"/api/accounts/{_hashedAccountId}/legalentities/{_accountResponse.Account.AccountLegalEntities.ToList()[1].LegalEntityId}");
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
                o.GetPropertyValue<long>("LegalEntityId").Equals(_accountResponse.Account.AccountLegalEntities.ToList()[positionIndex].LegalEntityId);
        }
    }
}
