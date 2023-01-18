using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.Validation;
using PayeScheme = SFA.DAS.EmployerAccounts.Api.Types.PayeScheme;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    [TestFixture]
    public class WhenIGetAPayeScheme : AccountPayeSchemesControllerTests
    {
        private string _hashedAccountId;
        private GetAccountPayeSchemesResponse _accountResponse;

        [Test]
        public async Task ThenThePayeSchemesAreReturned()
        {
            _hashedAccountId = "ABC123";
            _accountResponse = new GetAccountPayeSchemesResponse
            {
                PayeSchemes =
                    new List<PayeView>
                    {
                        new PayeView
                        {
                            Ref = "ABC/123",
                        },
                        new PayeView
                        {
                            Ref = "ZZZ/999"
                        }
                    }
            };

            Mediator.Setup(x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.HashedAccountId == _hashedAccountId), It.IsAny<CancellationToken>())).ReturnsAsync(_accountResponse);

            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => IsAccountPayeSchemeOne(o)))).Returns($"/api/accounts/{_hashedAccountId}/payeschemes/{_accountResponse.PayeSchemes[0].Ref.Replace(@"/", "%2f")}");
            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => IsAccountPayeSchemeTwo(o)))).Returns($"/api/accounts/{_hashedAccountId}/payeschemes/{_accountResponse.PayeSchemes[1].Ref.Replace(@"/", "%2f")}");


            var response = await Controller.GetPayeSchemes(_hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ResourceList>>(response);
            var model = response as OkNegotiatedContentResult<ResourceList>;

            model?.Content.Should().NotBeNull();

            foreach (var payeScheme in _accountResponse.PayeSchemes)
            {
                var matchedScheme = model?.Content.Single(x => x.Id == payeScheme.Ref);
                matchedScheme?.Href.Should().Be($"/api/accounts/{_hashedAccountId}/payeschemes/{payeScheme.Ref.Replace(@"/", "%2f")}");
            }
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetAccountPayeSchemesResponse();

            Mediator.Setup(x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>())).ReturnsAsync(accountResponse);

            var response = await Controller.GetPayeSchemes(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task AndTheAccountCannotBeDecodedThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";

            Mediator.Setup(
                    x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                .Throws(new InvalidRequestException(new Dictionary<string, string>()));

            var response = await Controller.GetPayeSchemes(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task ThenTheAccountIsReturned()
        {
            var hashedAccountId = "ABC123";
            var payeSchemeRef = "ZZZ/123";
            var payeSchemeResponse = new GetPayeSchemeByRefResponse
            {
                PayeScheme = new PayeSchemeView
                {
                    Ref = payeSchemeRef,
                    Name = "Test",
                    AddedDate = DateTime.Now.AddYears(-10),
                    RemovedDate = DateTime.Now
                }
            };
            Mediator.Setup(x => x.Send(It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

            var response = await Controller.GetPayeScheme(hashedAccountId, payeSchemeRef.Replace("/", "%2f"));

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PayeScheme>>(response);
            var model = response as OkNegotiatedContentResult<PayeScheme>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldBeEquivalentTo(payeSchemeResponse.PayeScheme, options => options.Excluding(x => x.DasAccountId));
            model?.Content.DasAccountId.Should().Be(hashedAccountId);
        }

        [Test]
        public async Task AndThePayeSchemeDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var payeSchemeRef = "ZZZ/123";
            var payeSchemeResponse = new GetPayeSchemeByRefResponse { PayeScheme = null };

            Mediator.Setup(x => x.Send(It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

            var response = await Controller.GetPayeScheme(hashedAccountId, payeSchemeRef);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        private bool IsAccountPayeSchemeTwo(object o)
        {
            return IsAccountPayeSchemeInPosition(o, 1);
        }

        private bool IsAccountPayeSchemeOne(object o)
        {
            return IsAccountPayeSchemeInPosition(o, 0);
        }

        private bool IsAccountPayeSchemeInPosition(object o, int positionIndex)
        {
            return
                o.GetPropertyValue<string>("hashedAccountId").Equals(_hashedAccountId)
                &&
                o.GetPropertyValue<string>("payeSchemeRef").Equals(_accountResponse.PayeSchemes[positionIndex].Ref.Replace(@"/", "%2f"));
        }
    }
}
